using CommunityToolkit.Mvvm.ComponentModel;
using Duende.IdentityModel.OidcClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Shared.Languages;
using TabbyCat.ViewModels.Bases;
using TuDog.Bootstrap;
using TuDog.Interfaces.IDialogServers;

namespace TabbyCat.ViewModels;

public partial class LoginViewModelBase : ViewModelBase
{

    [ObservableProperty]
    private bool isLogined;
    
    private ILogger<LoginViewModelBase> logger = TuDogApplication.ServiceProvider.GetRequiredService<ILogger<LoginViewModelBase>>();

    protected IDialogServer dialogService = TuDogApplication.ServiceProvider.GetRequiredService<IDialogServer>();
    
    protected OidcClient oidcClient = TuDogApplication.ServiceProvider.GetRequiredService<OidcClient>();
    
    
}