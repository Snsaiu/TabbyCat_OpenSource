using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models;
using TabbyCat.Models.Users;
using TabbyCat.Repository.Entities.AiEntities;
using TabbyCat.Service.AiServices;
using TabbyCat.Shared.Enums;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;
using TuDog.Interfaces.RegionManagers;

namespace TabbyCat.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{

    private ILogger<MainWindowViewModel> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<MainWindowViewModel>>();

    private IAiTemplateSettingService _aiTemplateSettingService =
        TuDogApplication.ServiceProvider.GetRequiredService<IAiTemplateSettingService>();
    
    private IRegionManager _regionManager { get; }=TuDogApplication.ServiceProvider.GetRequiredService<IRegionManager>();

    [ObservableProperty] private IBackgroundImageConfig _backgroundImageConfig =
        TuDogApplication.ServiceProvider.GetRequiredService<IBackgroundImageConfig>();
    
    protected override Task OnLoaded()
    {

      _regionManager.AddToRegion<MainViewModel>("mainContainer");
      return Task.CompletedTask;
    }
    

}