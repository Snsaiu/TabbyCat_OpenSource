using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Duende.IdentityModel.OidcClient;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.Logging;
using TabbyCat.Enums;
using TabbyCat.Extensions;
using TabbyCat.IServices;
using TabbyCat.IServices.LocalConfigs;
using TabbyCat.Models.Users;
using TabbyCat.Shared.Languages;
using TuDog.Interfaces.Navigations;
using TuDog.IocAttribute;

namespace TabbyCat.ViewModels;

[Register]
public partial class MobilePersonViewModel(
    INavigationService navigationService) : LoginViewModelBase
{

    [RelayCommand]
    private Task NavigationToSetting()
    {
        return navigationService.PushAsync<SettingViewModel>(null);
    }

    
}