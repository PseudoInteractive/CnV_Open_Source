using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Services.Store;
using Windows.UI.Popups;

namespace COTG.Services
{
	public class StoreHelper
	{
		public static StoreHelper instance = new StoreHelper();
		private StoreContext context = null;

		// Downloads and installs package updates in separate steps.
		public async void DownloadAndInstallAllUpdatesAsync()
		{
			if (System.Diagnostics.Debugger.IsAttached)
				return;
			try
			{
				if (context == null)
				{
					context = StoreContext.GetDefault();
				}

				//Debug.Assert(App.IsOnUIThread());

				// Get the updates that are available.
				IReadOnlyList<StorePackageUpdate> updates =
					await context.GetAppAndOptionalStorePackageUpdatesAsync();

				if (updates != null && updates.Count != 0)
				{
					// Download the packages.

					await InstallPackageUpdatesAsync(updates);

				}
			}
			catch(Exception ex)
			{
				Debug.LogEx(ex);
			}
		}

		//// Helper method for downloading package updates.
		//private async Task<bool> DownloadPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
		//{
		//	bool downloadedSuccessfully = false;

		//	IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> downloadOperation =
		//		this.context.RequestDownloadStorePackageUpdatesAsync(updates);

		//	// The Progress async method is called one time for each step in the download process for each
		//	// package in this request.
		//	downloadOperation.Progress = async (asyncInfo, progress) =>
		//	{
		//		//await this.Dispatcher.RunAsync(Windows.UI.Core.DispatcherQueuePriority.Normal,
		//		//() =>
		//		//{
		//		//	downloadProgressBar.Value = progress.PackageDownloadProgress;
		//		//});
		//	};

		//	StorePackageUpdateResult result = await downloadOperation.AsTask();

		//	switch (result.OverallState)
		//	{
		//		case StorePackageUpdateState.Completed:
		//			downloadedSuccessfully = true;
		//			break;
		//		default:
		//			// Get the failed updates.
		//			var failedUpdates = result.StorePackageUpdateStatuses.Where(
		//				status => status.PackageUpdateState != StorePackageUpdateState.Completed);

		//			// See if any failed updates were mandatory
		//			if (updates.Any(u => u.Mandatory && failedUpdates.Any(
		//				failed => failed.PackageFamilyName == u.Package.Id.FamilyName)))
		//			{
		//				// At least one of the updates is mandatory. Perform whatever actions you
		//				// want to take for your app: for example, notify the user and disable
		//				// features in your app.
		//				HandleMandatoryPackageError();
		//			}
		//			break;
		//	}

		//	return downloadedSuccessfully;
		//}

		// Helper method for installing package updates.
		private async Task InstallPackageUpdatesAsync(IEnumerable<StorePackageUpdate> updates)
		{
			IAsyncOperationWithProgress<StorePackageUpdateResult, StorePackageUpdateStatus> installOperation =
				this.context.RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

			// The package updates were already downloaded separately, so this method skips the download
			// operatation and only installs the updates; no download progress notifications are provided.
			StorePackageUpdateResult result = await installOperation.AsTask().ConfigureAwait(true);

			switch (result.OverallState)
			{
				case StorePackageUpdateState.Completed:
					break;
				default:
					// Get the failed updates.
					var failedUpdates = result.StorePackageUpdateStatuses.Where(
						status => status.PackageUpdateState != StorePackageUpdateState.Completed);

					// See if any failed updates were mandatory
					if (updates.Any(u => u.Mandatory && failedUpdates.Any(failed => failed.PackageFamilyName == u.Package.Id.FamilyName)))
					{
						// At least one of the updates is mandatory, so tell the user.
						HandleMandatoryPackageError();
					}
					break;
			}
		}

		// Helper method for handling the scenario where a mandatory package update fails to
		// download or install. Add code to this method to perform whatever actions you want
		// to take, such as notifying the user and disabling features in your app.
		private void HandleMandatoryPackageError()
		{
		
		}
	}
}
