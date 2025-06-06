using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;

namespace TabbyCat.Models;

public partial class NavigationMenuItem : ModelBase
{
    [ObservableProperty] private string _header = string.Empty;

    //avares://TabbyCat/Assets/logo.png
    [ObservableProperty] private string _icon = string.Empty;

    [ObservableProperty] private Type? _content;

    [ObservableProperty] private IEnumerable<NavigationMenuItem> _children = [];

    [ObservableProperty] private bool isSelected;

    [ObservableProperty] private AiMediaWorkType _mediaWorkType;

    public static NavigationMenuItem Create(string header, string icon, IEnumerable<NavigationMenuItem> children)
    {
        var lan = LocalizationResourceManager.Instance[header];
        return new NavigationMenuItem { Header = lan, Icon = icon, Children = children };
    }

    public static NavigationMenuItem Create(string header, string icon, Type? content, AiMediaWorkType mediaWorkType)
    {
        var lan = LocalizationResourceManager.Instance[header];
        return new NavigationMenuItem { Header = lan, Icon = icon, Content = content, MediaWorkType = mediaWorkType };
    }
}