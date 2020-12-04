using COTG.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using static COTG.Debug;

namespace COTG.Helpers
{
    public class NavStack : ICommand
    {

        public static void Push(int cid)
        {
            var cityView = false;// JSClient.IsCityView();
            // end current stack naviation session?
            if (position != -1)
            {
                if (backStack[position].cid == cid)
                    return;// altready here

               backStack.RemoveRange(position+1, backStack.Count-1-position);
                Assert(backStack.Count-1 == position);
                position = -1;
            }
            var prior = backStack.Count - 1;
            if( prior >= 0)
            {
                if (backStack[prior].cid == cid && backStack[prior].cityView == cityView)
                    return;
            }
            backStack.Add(new Entry() { cid = cid, cityView = cityView });
            // filter duplicates

        }
        public static bool Back(bool cityBackOnEmpty=false, bool scanOnly = false)
        {
            if (position == -1)
            {
                if (backStack.Count <= 1)
                {
                    Note.Show("Please navigate first");
                    if (cityBackOnEmpty)
                        Views.ShellPage.instance.ChangeCityClick(-1);
                    return false;
                }
                position = backStack.Count - 1;
            }
            if (position <= 0)
            {
                Note.Show("This is the first city");
                if(cityBackOnEmpty)
                    Views.ShellPage.instance.ChangeCityClick(-1);
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
            var p = (position == -1 ? backStack.Count - 1 : position) + delta;
            if (p >= 0 && p < backStack.Count)
                return Spot.GetOrAdd(backStack[p].cid).nameAndRemarks;
            return null;
        }
        public static bool Forward( bool cityForwardOnEmpty=false, bool scanOnly = false)
        {
            if (position == -1 || position >= backStack.Count - 1)
            {
                Note.Show("No more forwards left");
                if (cityForwardOnEmpty)
                    Views.ShellPage.instance.ChangeCityClick(1);
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
                if (City.IsMine(cid))
                {
                    Note.Show($"Navigate to {City.GetOrAddCity(cid).nameAndRemarks} {position} {NavStack.backStack.Count}");
                    if (!City.IsBuild(cid) || !City.IsFocus(cid))
                    {
                        JSClient.ChangeCity(cid,false);
                    }
                    if (cityView != JSClient.IsCityView())
                    {
                        JSClient.ChangeView(cityView);
                    }
                }
                else
                {
                    JSClient.ShowCity(cid, false);
                }
            }
        }

        static List<Entry> backStack = new List<Entry>();
        static int position=-1; // -1 means top of stack (no forward), otherwise it is a position in the stack (user is doing forward/backward navigation)

        // Keyboard Helpers
        public static void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            Back(true);
            //args.Handled = true;
        }
        public static void ForwardInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            Forward(true);
            //args.Handled = true;
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

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            var delta = (int)parameter;
            if (delta < 0)
            {
                do
                {
                    Back(false,++delta != 0);
                } while (delta < 0);
            }
            else if (delta > 0)
            {
                do
                {
                    Forward(false,--delta != 0);
                } while (delta > 0);
            }
        }
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
            }

            remove
            {
            }
        }
    }
}
