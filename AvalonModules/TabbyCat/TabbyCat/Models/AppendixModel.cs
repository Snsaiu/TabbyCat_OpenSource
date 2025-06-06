using CommunityToolkit.Mvvm.ComponentModel;
using TabbyCat.Enums;
using TuDog.Bootstrap;

namespace TabbyCat.Models;

public partial class AppendixModel:ModelBase
{
    [ObservableProperty]
    private AppendixType appendixType;

    [ObservableProperty]
    private string _content;
}