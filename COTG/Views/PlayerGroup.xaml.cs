using COTG.Game;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
    public sealed partial class PlayerGroup : ContentDialog
    {
        public PlayerGroup()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void Suggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var items = new List<string>();
                var txt = sender.Text.ToLower();
                if (txt.Length == 0)
                {
                    sender.ItemsSource = null;
                    return;
                }
                var startsWith = txt.Length <= 1;
                foreach (var p in Player.all)
                {
                    var comp = p.Value.name.ToLower();
                    if (startsWith ? comp.StartsWith(txt) : comp.Contains(txt) )
                    {
                        items.Add(p.Value.name);
                    }
                }
                if (items.Count > 0)
                    sender.ItemsSource = items;
                else
                    sender.ItemsSource = new string[] { "No results found" };
            }
        }

      

        private void Suggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null )
            {
                //User selected an item, take an action
                names.Items.Add(args.ChosenSuggestion as string );
                sender.Text = "";
                sender.ItemsSource = null;
  //              Note.Show($"Added {args.ChosenSuggestion.ToString()}");
            }
        }

        // returns null on cancel
        public static async Task<string[]> ChooseNames(string title, string[] names)
        {
            PlayerGroup pg = new PlayerGroup();
            if (names != null)
            {
                foreach(var i in names)
                   pg.names.Items.Add(i);
            }
            if(title!=null)
                pg.Title = title;
            var rv = await pg.ShowAsync();
            if(rv== ContentDialogResult.Primary)
            {
                names = new string[ pg.names.Items.Count];
                int put = 0;
                foreach(var name in pg.names.Items)
                {
                    names[put++] = name as string;
                }
                return names;
            }
            else
            {
                return null;
            }
        }

        

        private void NamesClick(object sender, ItemClickEventArgs e)
        {
            Log(e.ClickedItem.ToString());
            names.Items.Remove(e.ClickedItem);
            Note.Show($"Removed {e.ClickedItem.ToString()}");
        }
    }
}
