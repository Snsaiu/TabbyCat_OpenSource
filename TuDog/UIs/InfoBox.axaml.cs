using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using TuDog.Bootstrap;
using TuDog.Enums;
using TuDog.Interfaces.MessageBarService;
using TuDog.Interfaces.MessageBarService.Impl;
using TuDog.Models;

namespace TuDog.UIs;


[TemplatePart(Name = "PART_ItemControl", Type = typeof(ItemsControl))]
public partial class InfoBox : TemplatedControl
{
    private const string PART_ItemControl=nameof(PART_ItemControl);

    private ItemsControl _itemControl;

    private IMessageBarService _messageBarService =
        TuDogApplication.ServiceProvider.GetRequiredService<IMessageBarService>();
    public InfoBox()
    {
      ((MessageBarService)_messageBarService ).RegisterInfoBox(this);
    }

    public void AddNewMessage(InfoModel message)
    {
        message.CloseAction += RemoveItem;
        _itemControl.Items.Insert(0,message);

    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemControl = e.NameScope.Get<ItemsControl>(PART_ItemControl);
    }


    [RelayCommand]
    private Task Close(InfoModel parameter)
    {
        RemoveItem(parameter.Key);
        return Task.CompletedTask;
    }

    private void RemoveItem(Guid key)
    {
        var find = _itemControl.Items.FirstOrDefault(x => ((InfoModel)x).Key == key);
        if (find != null) _itemControl.Items.Remove(find);
    }

}