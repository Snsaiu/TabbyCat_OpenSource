using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;

namespace TabbyCat.Models;

public partial class NavigationMenuItem:ModelBase
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
        return new NavigationMenuItem { Header = header, Icon = icon, Children = children };
    }

    public static NavigationMenuItem Create(string header, string icon, Type? content,AiMediaWorkType mediaWorkType)
    {
       return new NavigationMenuItem { Header = header, Icon = icon, Content = content ,MediaWorkType = mediaWorkType};
       
    }
}