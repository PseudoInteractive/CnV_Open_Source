using COTG.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using static COTG.Debug;

namespace COTG.Helpers
{
    public static class NavStack
    {
        public static void Push(int cid)
        {
            var cityView = JSClient.IsCityView();
            // end current stack naviation session?
            if (position != -1)
            {
               backStack.RemoveRange(position, backStack.Count-1-position);
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
        public static bool Back()
        {
            if (position == -1)
            {
                if (backStack.Count <= 1)
                {
                    Note.Show("Please navigate first");
                    return false;
                }
                position = backStack.Count - 1;
            }
            if (position <= 0)
            {
                Note.Show("This is the first city");
                return false;
            }
            --position;
             backStack[position].Go();
            return true;
        }

        public static bool Forward()
        {
            if (position == -1 || position >= backStack.Count - 1)
            {
                Note.Show("No more forwards left");
                return false;
            }
            ++position;
            backStack[position].Go();
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
                    if (!City.IsBuild(cid))
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
            Back();
            args.Handled = true;
        }
        public static void ForwardInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            Forward();
            args.Handled = true;
        }

        public static void BackClick(object sender, RoutedEventArgs e)
        {
            Back();
        }
        public static void ForwardClick(object sender, RoutedEventArgs e)
        {
            Forward();
        }
    }
}
