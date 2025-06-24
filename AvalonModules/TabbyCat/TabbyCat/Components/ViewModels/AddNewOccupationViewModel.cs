using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Enums;
using TuDog.Interfaces.IDialogServers;
using TuDog.IocAttribute;

namespace TabbyCat.Components.ViewModels;

[Register]
public sealed partial class AddNewOccupationViewModel(
    IUser user,
    ICustomOccupationSyncService customOccupationSyncService,
    ILogger<AddNewOccupationViewModel> logger,
    ICustomOccupationHubSyncManager customOccupationSyncManager,
    ICustomAssistantOccupationService customAssistantOccupationService)
    : DialogViewModelBaseAsync<bool, CustomAssistantOccupationEntity>
{
    [ObservableProperty] private string _name;

    [ObservableProperty] private string _description;

    public override async Task<bool> CanConfirmAsync()
    {
        if (string.IsNullOrEmpty(Name))
        {
            ErrorMessageAction?.Invoke(AppResources.MustInputContact, AppResources.Warning,
                MessageState.Error);
            return false;
        }

        if (string.IsNullOrEmpty(Description))
        {
            ErrorMessageAction?.Invoke(AppResources.MustInputContactDescription, AppResources.Warning,
                MessageState.Error);
            return false;
        }

        if ((await customAssistantOccupationService.QueryAsync(x =>
                x.Name == Name && x.Email == user.Email && !x.IsDeleted)).Any())
        {
            ErrorMessageAction?.Invoke(string.Format(AppResources.ExistContactName, Name), AppResources.Warning,
                MessageState.Error);
            return false;
        }

        return true;
    }

    public override async Task<CustomAssistantOccupationEntity> ConfirmAsync()
    {
        var entity = new CustomAssistantOccupationEntity
        {
            Name = Name,
            Description = Description,
            Email = user.Email
        };

        await customAssistantOccupationService.AddAsync(entity);

        // 如果是登陆状态，那么需要进行同步
        if (user.LoginSuccess())
        {
            // 获得最新版本
            var latestVersionResult = await customOccupationSyncService.QueryLatestVersionAsync();
            if (latestVersionResult is not { Ok: true, Data: var version })
                throw new Exception(latestVersionResult.ErrorMsg);

            logger.LogInformation("从远程获得最新的版本号是{0}", version);

            var customAssistants =
                (await customAssistantOccupationService.QueryAsync(x => x.Email == user.Email))
                .ToArray();
            logger.LogInformation("当前自定义角色集合:{0}", string.Join(",", customAssistants.Select(x => x.Name)));

            var latestVersion = version + 1;

            logger.LogInformation("即将上传的版本号为:{0}", latestVersion);

            var updateTime = DateTime.Now;
            foreach (var item in customAssistants)
            {
                item.Version = latestVersion;
                item.LastUpdateTime = updateTime;
            }

            await customOccupationSyncService.UploadRemoteAsync(customAssistants);

            await customAssistantOccupationService.UpdateRangeAsync(customAssistants);

            entity.Version = latestVersion;
        }

        return entity;
    }

    public override Task<CustomAssistantOccupationEntity> CancelAsync()
    {
        return Task.FromResult<CustomAssistantOccupationEntity>(null);
    }
}