using System;
using System.Threading.Tasks;

using CnV.Activation;

using Windows.ApplicationModel.Activation;
//using Microsoft.UI.Notifications;
using Windows.UI.Notifications;

namespace CnV.Services
{
	using Activation;

	internal partial class ToastNotificationsService : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        public static ToastNotificationsService instance = new ToastNotificationsService();
        public void ShowToastNotification(ToastNotification toastNotification)
        {
            try
            {
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            }
            catch (Exception)
            {
                // TODO WTS: Adding ToastNotification can fail in rare conditions, please handle exceptions as appropriate to your scenario.
            }
        }

        protected override async Task HandleInternalAsync(ToastNotificationActivatedEventArgs args)
        {
            //// TODO WTS: Handle activation from toast notification
            //// More details at https://docs.microsoft.com/windows/uwp/design/shell/tiles-and-notifications/send-local-toast

            await Task.CompletedTask;
        }
    }
}
