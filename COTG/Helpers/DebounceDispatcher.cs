using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Windows.UI.Core;

namespace COTG
{
	public static partial class AUtil
	{ 
		public static Action Debounce(this Action func, int milliseconds = 300)
		{
			CancellationTokenSource cancelTokenSource = null;

			return () =>
			{
				cancelTokenSource?.Cancel();
				cancelTokenSource = new CancellationTokenSource();

				Task.Delay(milliseconds, cancelTokenSource.Token)
					.ContinueWith(t =>
					{
						if (t.IsCompletedSuccessfully)
						{
							func();
						}
					}, TaskScheduler.Default);
			};
		}
		public static Action DebounceUI(this DispatchedHandler func, int milliseconds = 300)
		{
			CancellationTokenSource cancelTokenSource = null;

			return () =>
			{
				cancelTokenSource?.Cancel();
				cancelTokenSource = new CancellationTokenSource();

				Task.Delay(milliseconds, cancelTokenSource.Token)
					.ContinueWith(t =>
					{
						if (t.IsCompletedSuccessfully)
						{
							App.DispatchOnUIThreadSneaky( func);
						}
					}, TaskScheduler.Default);
			};
		}
	}
}
