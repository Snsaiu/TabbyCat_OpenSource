using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;

namespace TabbyCat.Models;

public partial class NavigationMenuItem:ModelBase
{

    [ObservableProperty]
    private string header=String.Empty;

    //avares://TabbyCat/Assets/logo.png
    [ObservableProperty]
    private string icon = string.Empty;

    [ObservableProperty]
    private Type content ;
}