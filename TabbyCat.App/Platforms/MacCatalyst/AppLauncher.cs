using Foundation;

using TabbyCat.App.Interfaces;

using UIKit;

namespace TabbyCat.App
{
    public class AppLauncher:IAppLauncher
    {
        public bool LaunchApp(string urlScheme)
        {
            var url = new NSUrl(urlScheme);
            if( UIApplication.SharedApplication.CanOpenUrl(url) )
                return UIApplication.SharedApplication.OpenUrl(url);
            return false;
        }
    }
}