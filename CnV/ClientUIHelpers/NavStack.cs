//using System.Windows.Input;

namespace CnV
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Input;
	//	using System.Windows.Input;
	using Microsoft.UI.Xaml;
	using Microsoft.UI.Xaml.Input;

	public class NavStack:ICommand
	{
		event EventHandler ICommand.CanExecuteChanged
		{
			add
			{
			//	throw new NotImplementedException();
			}

			remove
			{
			//	throw new NotImplementedException();
			}
		}
		public static int navStackOperation;
		 readonly ref  struct NavStackFrame {
			public NavStackFrame() {
				++navStackOperation;
			}
			public void Dispose() {
				--navStackOperation; ;
			}
		}
		public static void Push(int cid)
		{
			if(navStackOperation > 0)
				return;
			var cityView = false;// CnVServer.IsCityView();
								 // end current stack naviation session?
			if (position != -1)
			{
				if (backStack[position].cid == cid)
					return;// altready here

				backStack.RemoveRange(position + 1, backStack.Count - 1 - position);
				Assert(backStack.Count - 1 == position);
				position = -1;
			}
			var prior = backStack.Count - 1;
			if (prior >= 0)
			{
				if (backStack[prior].cid == cid && backStack[prior].cityView == cityView)
					return;
			}
			backStack.Add(new Entry() { cid = cid, cityView = cityView });
			// filter duplicates

		}
		public static bool Back(bool cityBackOnEmpty = false, bool scanOnly = false)
		{
			using var __frame = new NavStackFrame();
			if (position == -1)
			{
				if (backStack.Count <= 1)
				{
					Note.Show("Please navigate first");
					if (cityBackOnEmpty)
					{
					   Views.ShellPage.instance.ChangeCityClick(-1);
					}

					return false;
				}
				position = backStack.Count - 1;
			}
			if (position <= 0)
			{
				Note.Show("This is the first city");
				if (cityBackOnEmpty)
				{
				
				   Views.ShellPage.instance.ChangeCityClick(-1);
				}
				return false;
			}
			--position;
			if (!scanOnly)
			{
				backStack[position].Go();
				//	ElementSoundPlayer.Play(ElementSoundKind.MovePrevious);
			}
			return true;
		}

		public static string GetSpotName(int delta)
		{
			var p = (position == -1 ? backStack.Count  : position) + delta;
			if (p >= 0 && p < backStack.Count)
				return Spot.GetOrAdd(backStack[p].cid).nameAndRemarks;
			return null;
		}
		public static bool Forward(bool cityForwardOnEmpty = false, bool scanOnly = false)
		{
			using var __frame = new NavStackFrame();

			if (position == -1 || position >= backStack.Count - 1)
			{
				Note.Show("No more forwards left");

				if (cityForwardOnEmpty)
				{
					Views.ShellPage.instance.ChangeCityClick(1);
				}
				return false;
			}
			++position;
			if (!scanOnly)
			{
				backStack[position].Go();
				//				ElementSoundPlayer.Play(ElementSoundKind.MoveNext);
			}
			return true;
		}


		public struct Entry
		{
			public int cid;
			public bool cityView; // only valid for My Cities, not for other cities or Misc Spots 
			public void Go()
			{
				using var __frame = new NavStackFrame();
				City.ProcessCoordClick(cid,true,AppS.keyModifiers);
			}
		}

		static List<Entry> backStack = new List<Entry>();
		static int position = -1; // -1 means top of stack (no forward), otherwise it is a position in the stack (user is doing forward/backward navigation)

		// Keyboard Helpers
		public static void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			Back(true);
			args.Handled = true;
		}
		public static void ForwardInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			Forward(true);
			args.Handled = true;
		}

		public static void BackClick(object sender, RoutedEventArgs e)
		{
			Back(true);
		}
		public static void ForwardClick(object sender, RoutedEventArgs e)
		{
			Forward(true);
		}

		public static NavStack instance = new NavStack();

		// override global no op
		public NavStack()
		{
		}


		// triggers global constructor
		public static NavStack 
				Init()
		{
			return instance;
		}
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			var delta = (int)parameter;
			if (delta < 0)
			{
				do
				{
					Back(false, ++delta != 0);
				} while (delta < 0);
			}
			else if (delta > 0)
			{
				do
				{
					Forward(false, --delta != 0);
				} while (delta > 0);
			}
		}

		//bool ICommand.CanExecute(object parameter)
		//{
		//	throw new NotImplementedException();
		//}

		//void ICommand.Execute(object parameter)
		//{
		//	throw new NotImplementedException();
		//}

	//	public event EventHandler<object> CanExecuteChanged;
	}
}
