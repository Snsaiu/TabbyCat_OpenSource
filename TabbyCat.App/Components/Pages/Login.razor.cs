using TabbyCat.App.Interfaces;
using TabbyCat.App.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using TabbyCat.VisualBase.Bases;

namespace TabbyCat.App.Components.Pages;

public partial class Login : PageComponentBase
{
    #region MyRegion

    private string? UserName { get; set; }

    private string? DeviceNiceName { get; set; }


    #endregion Parameters


    #region Injects


    [Inject] private IUserService? UserService { get; set; }


    [Inject] private new IDialogService? DialogService { get; set; }

    #endregion


    protected override async Task OnPageInitializedAsync(string? url, Dictionary<string, object>? data)
    {

        if (UserService is null)
            throw new NullReferenceException();
        try
        {
            IsBusy = true;
            var userResult = await UserService.GetCurrentUserAsync();
            if (userResult.Ok)
            {
                NavigationManager?.NavigateTo("/home");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// 登陆
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            DialogService?.ShowError("用户名称不能为空", "错误");
            return;
        }

        if (string.IsNullOrWhiteSpace(DeviceNiceName))
        {
            DialogService?.ShowError("设备昵称不能为空", "错误");
            return;
        }

        if (UserService is null)
            throw new NullReferenceException(nameof(UserService));
        IsBusy = true;
        try
        {
            await UserService.SaveUserAsync(new UserInfo() { Name = UserName, DeviceNickName = DeviceNiceName });
        }
        finally
        {
            IsBusy = false;
        }

        NavigationManager?.NavigateTo($"/home");
    }
}