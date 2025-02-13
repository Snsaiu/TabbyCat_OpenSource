namespace TabbyCat.App.Components.Pages;

public partial class ShortChatPage : ContentPage
{
    public ShortChatPage()
    {
        InitializeComponent();
    }

    //    protected override void OnHandlerChanged()
    //    {
    //#if WINDOWS
    //        //   Microsoft.UI.Xaml.Window window =
    //        //(Microsoft.UI.Xaml.Window)App.Current.Windows.First<Window>().Handler.PlatformView;

    //        foreach (var item in App.Current.Windows)
    //        {
    //            if (item.Title == ConstParams.ShortChatWindowKey)
    //            {
    //                MauiAppExtension.HideTaskBar((Microsoft.UI.Xaml.Window)item.Handler.PlatformView);
    //                break;
    //            }
    //        }
    //#endif
    //    }
}