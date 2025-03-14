using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;
using TuDog.IocAttribute;
using YouYan.Rabbit.Models;

namespace YouYan.Rabbit.Components.ViewModels;

[Register]
public sealed partial class WhatNewViewModel : DialogViewModelBase<bool>
{
    [ObservableProperty] private AppReleaseModel model;


    protected override Task OnLoaded()
    {
        if (Parameter is AppReleaseModel m)
        {
            Model = m;

        }
        return base.OnLoaded();
    }

    protected override Task<bool> OnConfirmAsync()
    {
        return Task.FromResult(true);
    }
}