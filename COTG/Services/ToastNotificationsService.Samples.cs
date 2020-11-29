using Microsoft.Toolkit.Uwp.Notifications;

using System;

using Windows.UI.Notifications;

namespace COTG.Services
{
    internal partial class ToastNotificationsService
    {

        public void ShowIncomingNotification(string info)
        {

            // Create the toast content
            var content = new ToastContent()
            {
                // More about the Launch property at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastcontent
                Launch = "ToastContentActivationParams",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = info
                            },

                        }
                    },


                },

                Actions = new ToastActionsCustom()
                {

                    Buttons =
                    {
                        // More about Toast Buttons at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastbutton
                        new ToastButton("OK", "incomingNotification")
                        {
                            ActivationType = ToastActivationType.Foreground,
                           
                        },


                        new ToastButtonDismiss(),
                        new ToastButtonSnooze() {SelectionBoxId="snoozeTime",},


                    },
                    Inputs =
                    {
                        new ToastSelectionBox("snoozeTime")
                    {
                        DefaultSelectionBoxItemId = "5",
                        Items =
                        {
                            new ToastSelectionBoxItem("1", "1 minute"),
                            new ToastSelectionBoxItem("5", "5 minutes"),
                            new ToastSelectionBoxItem("15", "15 minutes"),
                            new ToastSelectionBoxItem("60", "1 hour"),
                            new ToastSelectionBoxItem("240", "4 hours"),
                        }
                    },
                    }
                }
            };


            // Add the content to the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // TODO WTS: Set a unique identifier for this notification within the notification group. (optional)
                // More details at https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification.tag
                Tag = "ToastTag"
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
