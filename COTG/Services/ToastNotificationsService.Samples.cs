using Microsoft.Toolkit.Uwp.Notifications;

using System;

using Windows.UI.Notifications;

namespace COTG.Services
{
    internal partial class ToastNotificationsService
    {

        public void ShowIncomingNotification(int count, int watchedCount)
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
                                Text = $"Incoming Attacks: {count} for you, {watchedCount} for watched players"
                            },

                        }
                    },


                },

                Actions = new ToastActionsCustom()
                {

                    Buttons =
                    {
                        // More about Toast Buttons at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastbutton
                        new ToastButton("OK", "ToastButtonActivationArguments")
                        {
                            ActivationType = ToastActivationType.Foreground
                        },

                        new ToastButtonDismiss("Cancel"),
                        new ToastButtonSnooze("Snooze"),

                    },
                    Inputs =
                    {
                        new ToastSelectionBox("snoozeTime")
                    {
                        DefaultSelectionBoxItemId = "15",
                        Items =
                        {
                            new ToastSelectionBoxItem("5", "5 minutes"),
                            new ToastSelectionBoxItem("15", "15 minutes"),
                            new ToastSelectionBoxItem("60", "1 hour"),
                            new ToastSelectionBoxItem("240", "4 hours"),
                            new ToastSelectionBoxItem("1440", "1 day")
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
