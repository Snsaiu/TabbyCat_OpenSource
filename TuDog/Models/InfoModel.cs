using CommunityToolkit.Mvvm.ComponentModel;
using TuDog.Bootstrap;
using TuDog.Enums;

namespace TuDog.Models;

/// <summary>
/// 消息模型
/// </summary>
public partial class InfoModel:ModelBase
{
   [ObservableProperty]
   private string _message=string.Empty;

   [ObservableProperty]
   private string _title=string.Empty;

   [ObservableProperty]
   private Guid _key=Guid.CreateVersion7();

   [ObservableProperty]
   private bool _showClose = true;

   [ObservableProperty]
   private MessageState _type = MessageState.Warning;

   public Action<Guid> CloseAction { get; set; }

   private InfoModel()
   {
      var t = new System.Timers.Timer();
      t.Interval = 5000;
      t.AutoReset = false;
      t.Elapsed += (s, e) => { CloseAction?.Invoke(Key); };
      t.Start();
   }


   public static InfoModel Create(string message, string title, bool showClose, MessageState type)
   {
      return new() { Message = message, Title = title, ShowClose = showClose, Type = type };
   }

   //TODO warning..


}