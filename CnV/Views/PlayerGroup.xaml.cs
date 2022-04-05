using CnV.Game;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using static CnV.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CnV.Views
{
	using Game;

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
		public static void Suggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args, IEnumerable<string> options)
		{
			if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
			{
				
				var txt = sender.Text.ToLower();
				if (txt.Length == 0)
				{
					sender.ItemsSource = options;
					return;
				}
				var items = new List<string>();
				var startsWith = txt.Length <= 1;
				foreach (var p in options)
				{
					if (startsWith ? p.StartsWith(txt, StringComparison.InvariantCultureIgnoreCase) : p.Contains(txt,StringComparison.InvariantCultureIgnoreCase))
					{
						items.Add(p);
					}
				}
				if (items.Count > 0)
					sender.ItemsSource = items;
				else
					sender.ItemsSource = new string[] { "None found" };
			}
		}

		public static void PlayerNameSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
			Suggest_TextChanged(sender, args, Player.all.Select(p => p.name));
        }
		public static void CityNameSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			Suggest_TextChanged(sender, args, City.subCities.Select(p => p.nameAndRemarksAndPlayer));
		}
		public static void AllianceSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			Suggest_TextChanged(sender, args, Alliance.all.Select(p => p.Value.name));

		}


		public static void CitySuggest_TextChanged(AutoSuggestBox sender,AutoSuggestBoxTextChangedEventArgs args)
		{
			Suggest_TextChanged(sender,args,City.subCities.Select(p => p.nameAndRemarks));

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
			return await AppS.DispatchOnUIThreadTask(async () =>
			{
				PlayerGroup pg = new PlayerGroup();
				if (names != null)
				{
					foreach (var i in names)
						pg.names.Items.Add(i);
				}
				if (title != null)
					pg.Title = title;
				var rv = await pg.ShowAsync2();

				ElementSoundPlayer.Play(ElementSoundKind.Show);

				if (rv == ContentDialogResult.Primary)
				{
					names = new string[pg.names.Items.Count];
					int put = 0;
					foreach (var name in pg.names.Items)
					{
						names[put++] = name as string;
					}
					return names;
				}
				else
				{
					return null;
				}
			});
        }

        

        private void NamesClick(object sender, ItemClickEventArgs e)
        {
            Log(e.ClickedItem.ToString());
            names.Items.Remove(e.ClickedItem);
            Note.Show($"Removed {e.ClickedItem.ToString()}");
        }
    }
}
