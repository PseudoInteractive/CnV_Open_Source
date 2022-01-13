using System;
using System.Threading.Tasks;

using CnV.Views;

using Windows.ApplicationModel.Activation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

//namespace COTG.Activation
//{
//    internal class ShareTargetActivationHandler : ActivationHandler<ShareTargetActivatedEventArgs>
//    {
//        protected override async Task HandleInternalAsync(ShareTargetActivatedEventArgs args)
//        {
//            // Activation from ShareTarget opens the app as a new modal window which requires a new activation.
//            var frame = new Microsoft.UI.Xaml.Controls.Frame();
//            Window.Current.Content = frame;
//            frame.Navigate(typeof(ShareTargetPage), args.ShareOperation);
//            Window.Current.Activate();

//            await Task.CompletedTask;
//        }
//    }
//}
