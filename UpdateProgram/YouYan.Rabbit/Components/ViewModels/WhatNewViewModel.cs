using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;
using TuDog.IocAttribute;
using YouYan.Rabbit.Models;

namespace YouYan.Rabbit.Components.ViewModels;

[Register]
public sealed partial class WhatNewViewModel : DialogViewModelBaseAsync<AppReleaseModel,bool>
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


    public override Task<bool> ConfirmAsync()
    {
        return Task.FromResult(true);
    }

    public override Task<bool> CancelAsync()
    {
        return Task.FromResult(false);
    }
}