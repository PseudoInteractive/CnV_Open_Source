using COTG.Game;
using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static COTG.Game.Enum;
using static COTG.Debug;
using System.Text.RegularExpressions;
using System.Numerics;
using Microsoft.Toolkit;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Controls;
using COTG;
using Microsoft.Toolkit.Uwp.UI.Controls;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
using Nito.AsyncEx;

namespace COTG.Views
{

	public sealed partial class AttackTab : UserTab, INotifyPropertyChanged
    {
		public static AsyncLock asyncLock = new ();
		public const float moralCostBias = 128;
		public static StorageFolder folder => ApplicationData.Current.LocalFolder;
        public static AttackTab instance;
        public static bool IsVisible() => instance.isVisible;

		
        public static DumbCollection<Spot> attacks = new DumbCollection<Spot>();
        public static DumbCollection<Spot> targets = new DumbCollection<Spot>();
        public AttackTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
        }
		static void UpdateArrivalUI()
		{
			if(AUtil.dateTimeZero != SettingsPage.attackPlannerTime)
				instance.arrival.Content = SettingsPage.attackPlannerTime.FormatDefault();
		}


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private async void AttackSender_Tapped(object ____=null, object __=null)
        {
			using var _ = await TouchLists();

			SortAttacks();


     //       var clusterCount = attackClusters.Length;
//            var used = new HashSet<int>(readable.attacks.Length);

            var scripts = new string[attackClusters.Max(a=>a.id) + 1];
            //int clusterId = 0;
            foreach (var cluster in attackClusters)
            {
                var atk = new AttackSenderScript() { type = new List<int>(), x = new List<int>(), y = new List<int>() };
                // group all attacks for this player
                foreach (var a1 in cluster.targets.OrderBy((a) => Spot.GetOrAdd(a).isAttackTypeFake))
                {
                    var t = Spot.GetOrAdd(a1);

                    atk.type.Add(t.isAttackTypeFake ? 0 : 1);

                    var xy = a1.CidToWorld();
                    atk.x.Add(xy.x);
                    atk.y.Add(xy.y);
                }
				var time = SettingsPage.attackPlannerTime;

				atk.time = new string[] { time.Hour.ToString("00"), time.Minute.ToString("00"), time.Second.ToString("00"), time.ToString("MM/dd/yyyy") };
                scripts[cluster.id]= System.Text.Json.JsonSerializer.Serialize(atk, Json.jsonSerializerOptions);
            }

            StringBuilder sb = new StringBuilder();
            var players = new List<int>();
			int assaultOffset = 0;
            foreach (var a in attacks)
            {
                var player = Spot.GetOrAdd(a.cid).pid;
                if (players.Contains(player))
                    continue;
                players.Add(player);
            }
            foreach (var player in players)
            {
                sb.Append($"\n\n{Player.IdToName(player)}");
                foreach (var a in attacks)
                {
                    if (Spot.GetOrAdd(a.cid).pid != player || a.attackCluster==-1)
                        continue;
                    if (true)
				    {
						var atk = new AttackSenderScript() { type = new List<int>(), x = new List<int>(), y = new List<int>()};
						atk.cid = a.cid;
						// group all attacks for this player
						var c = attackClusters[a.attackCluster];
						foreach (var a1 in c.targets.OrderBy((a) => Spot.GetOrAdd(a).isAttackTypeFake))
						{
							var t = Spot.GetOrAdd(a1);
							
							atk.type.Add(t.isAttackTypeFake ? 0 : 1);
							

							var xy = a1.CidToWorld();
							atk.x.Add(xy.x);
							atk.y.Add(xy.y);
						}

						var time = SettingsPage.attackPlannerTime;
						atk.command =a.isAttackTypeAssault ? "Assault" : "Siege";
						if( a.isAttackTypeAssault && c.real.AsCity().attackType == AttackType.senator )
						{
							time += TimeSpan.FromHours( (assaultOffset) % 6 );
							++assaultOffset;
							
						}
						atk.time = new string[] { time.Hour.ToString("00"), time.Minute.ToString("00"), time.Second.ToString("00"), time.ToString("MM/dd/yyyy") };
						var scrpipt = System.Text.Json.JsonSerializer.Serialize(atk, Json.jsonSerializerOptions);


                        sb.Append($"\n\n<player>{scrpipt}</player>");
                    }
                    sb.Append('\n');
                    sb.Append($"{a.cid.CidToCoords()} {a.attackType}" );
				}


			}
            App.CopyTextToClipboard(sb.ToString());
            Note.Show("Copied Attack sender scripts to clipboard");
			SaveAttacks();
		}

        private void AttackTargetCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var atk = i.DataContext as Spot;

            //if (atk.targets.Any() )
            //{
            //    var id = atk.targets.FindIndex(Spot.focus)+1;
            //    if (id >= atk.targets.Length)
            //        id =0;
            //    var cid = atk.targets[id];
            //    var spot = Spot.GetOrAdd(cid);
            //    Spot.ProcessCoordClick(cid, false,  App.keyModifiers);
            //    foreach (var t in targets)
            //    {
            //        if (t.cid == cid)
            //            targetGrid.ScrollIntoView(t, null);
            //    }
            //}
        }

		static int CompareCid(int cid0, int cid1)
		{
			return cid0.ZCurveEncodeCid().CompareTo(cid1.ZCurveEncodeCid());
		}

		private void Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
		{
			var dg = sender as DataGrid;
			var cities = dg == attackGrid ? attacks : targets;
			var tag = e.Column.Tag != null? e.Column.Tag.ToString() : e.Column.Header.ToString();
			//Use the Tag property to pass the bound column name for the sorting implementation
			Comparison<Spot> comparer = null;
			switch (tag)
			{
				case "ts": comparer = (a, b) => a.tsTotal.CompareTo(b.tsTotal); break;
				case nameof(Spot.classification): comparer = (a, b) => a.classification.CompareTo(b.classification); break;
				case "name": comparer = (a, b) => b.nameAndRemarks.CompareTo(a.nameAndRemarks); break;
				case nameof(Spot.attackCluster) : comparer = (a, b) => b.attackCluster.CompareTo(a.attackCluster); break;
				case nameof(Spot.attackType): comparer = (a, b) => b.attackType.CompareTo(a.attackType); break;
				case "Points": comparer = (a, b) => b.points.CompareTo(a.points); break;
				case nameof(Spot.xy): comparer = (a, b) => CompareCid(a.cid,b.cid); break;
				case "Water": comparer = (a, b) => a.isOnWater.CompareTo(b.isOnWater); break;
				case "Player": comparer = (a, b) => a.playerName.CompareTo(b.playerName); break;
				case "Alliance": comparer = (a, b) => a.alliance.CompareTo(b.alliance); break;

			}

			if (comparer != null)
			{
				//Implement sort on the column "Range" using LINQ
				if (e.Column.SortDirection == null)
				{
					e.Column.SortDirection = DataGridSortDirection.Descending;
					cities.Sort(comparer);
					cities.NotifyReset();
				}
				else if (e.Column.SortDirection == DataGridSortDirection.Descending)
				{
					e.Column.SortDirection = DataGridSortDirection.Ascending;
					cities.Sort((b, a) => comparer(a, b)); // swap order of comparison
					cities.NotifyReset();
				}
				else
				{
					e.Column.SortDirection = null;

				}
			}
			// add code to handle sorting by other columns as required

			// Remove sorting indicators from other columns
			foreach (var dgColumn in dg.Columns)
			{
				if (dgColumn.Tag != null && dgColumn.Tag.ToString() != tag)
				{
					dgColumn.SortDirection = null;
				}
			}
		}
		private void AttackSourceCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Spot;
            if (spot.cid != 0)
            {

                Spot.ProcessCoordClick(spot.cid, false, App.keyModifiers,false);
            }
        }
        static bool loaded = false;

        public class ReadableAttacks
        {
            public AttackDataPersist[] attacks { get; set; }
            public AttackDataPersist[] targets { get; set; }
			public int smallTime { get; set; }

        }
        // read only cache to enable threads to read the attacks white another thread is writing
        public static ReadableAttacks readable = new ReadableAttacks() { attacks= Array.Empty<AttackDataPersist>(), targets  = Array.Empty<AttackDataPersist>() };

        internal static void WritebackAttacks()
        {
		

			Assert(loaded);
            int targetCount = AttackTab.targets.Count;
            var targets = new AttackDataPersist[targetCount];
			var attackCount = AttackTab.attacks.Count;
			var attacks = new AttackDataPersist[attackCount];
			for (int i = 0; i < targetCount; ++i)
            {
                var t = AttackTab.targets[i];
                targets[i].attackCluster = t.attackCluster;
                targets[i].cid = t.cid;
                targets[i].attackType = t.attackType;
            }
			for (int i = 0; i < attackCount; ++i)
			{
				var t = AttackTab.attacks[i];
				attacks[i].attackCluster = t.attackCluster;
				attacks[i].cid = t.cid;
				attacks[i].attackType = t.attackType;
			}
			readable.attacks = attacks; // This is not completely atomic
            readable.targets = targets;
            UpdateStats();
            BuildAttackClusters();
        }
        public class AttackCluster
        {
			public int id;
            public int[] attacks;// first is SE or siege
            public int[] targets;// first is real
            public Vector2 topLeft;
            public Vector2 bottomRight;
			internal int real => targets.Length > 0 ? targets[0] : 0;
			//Select(a=>City.Get(a)).Where( a => a.isAttackTypeReal ).DefaultIfEmpty(City.invalid).OrderBy(a=>a.spatialIndex).First().cid;
			///            public int real => (targets.FirstOrDefault((a) => !Spot.GetOrAdd(a).isSiege));


		}
        public static AttackCluster[] attackClusters;
        public static void BuildAttackClusters()
        {
            var reals = targets.Where((a) => a.isAttackTypeSiege&&!a.isAttackClusterNone).OrderBy(a=>a.spatialIndex).ToArray();
            var _attackClusters = new List<AttackCluster>();
            foreach(var r in reals)
            {
				var i = r.attackCluster;
                var ac = new AttackCluster();
				ac.id = i;
                _attackClusters.Add(ac);
                ac.targets = targets.Where((a) => a.attackCluster == i).OrderBy( a=> (long)a.spatialIndex - ((long)a.attackType<<32)  ).Select((a) => a.cid).ToArray();
				ac.attacks = attacks.Where((a) => a.attackCluster == i).OrderBy(a => (long)a.spatialIndex - ((long)a.attackType<<32) ).Select((a) => a.cid).ToArray();
                if (ac.targets.Any())
                {
                    ac.topLeft.X = ac.targets.Select(a => a.ToWorldC().X).Min() - 0.5f;
                    ac.topLeft.Y = ac.targets.Select(a => a.ToWorldC().Y).Min() - 0.5f;
                    ac.bottomRight.X = ac.targets.Select(a => a.ToWorldC().X).Max() + 0.5f;
                    ac.bottomRight.Y = ac.targets.Select(a => a.ToWorldC().Y).Max() + 0.5f;
                }
            }
            attackClusters = _attackClusters.ToArray();
        }
        private static void UpdateStats()
        {
            App.DispatchOnUIThreadSneakyLow(() =>
            {
               var attacks = readable.attacks.Where(a => a.attackType != AttackType.none).ToArray();
				var targets =readable.targets.Where(a => a.attackType != AttackType.none).ToArray();

				instance.attackCount.Text = $"Attacks: {attacks.Length}";
				var seCount = attacks.Count((a) => a.attackType ==AttackType.se);
                instance.SE.Text=$"SE: {seCount}";
				instance.sen.Text = $"Sen: {attacks.Count((a) => a.attackType==AttackType.senator) }";
				instance.vanqs.Text=$"Vanqs: {attacks.Count((a) => a.spot.primaryTroopType==ttVanquisher)}";
                instance.sorcs.Text=$"Sorc: {attacks.Count((a) => a.spot.primaryTroopType==ttSorcerer)}";
                instance.horses.Text=$"Horse: {attacks.Count((a) => a.spot.primaryTroopType==ttHorseman)}";
                var fakes = targets.Count(a => a.spot.isAttackTypeFake );
                var reals =  targets.Count(a=> a.spot.isAttackTypeReal );
				instance.fakeCount.Text=$"Fake Ratio: {fakes/(float)reals.Max(1):0.00}";
                instance.realCount.Text = $"Reals: {reals }";
                instance.sePerTarget.Text = $"SE/Target: {seCount/(float)reals.Max(1):0.00}";
                instance.attacksPerTarget.Text = $"Attacks/Target: {attacks.Length/(float)reals.Max(1):0.00}";


            });
        }

		internal static async Task SaveAttacks()
		{
			if (loaded)
			{
				await folder.SaveAsync("attacks", readable,true);
			}
		}
		public static async Task WaitAndSaveAttacks()
		{
			if (loaded)
			{
				using var _ = await asyncLock.LockAsync();
				await SaveAttacks();
			}
		}
		public async Task<IDisposable> TouchLists()
        {
			// First init
			//  AttackTab.attacks.Clear();
			var rv = await asyncLock.LockAsync();

			if (!loaded)
            {

				loaded = true;
                using var work = new ShellPage.WorkScope("load attacks");
                readable = await folder.ReadAsync<ReadableAttacks>("attacks", readable);
				// App.DispatchOnUIThreadSneaky(() =>
				//  {
				var attacks = new List<Spot>();
				foreach (var att in readable.attacks)
                {
                    var spot = Spot.GetOrAdd(att.cid);
                    spot.attackCluster=att.attackCluster;
					spot.attackType = att.attackType;
                    spot.QueueClassify(true);
					attacks.Add(spot);
                }
                var spots = new List<Spot>();
                if (readable.targets != null)
                {
                    foreach (var target in readable.targets)
                    {
                        var t = Spot.GetOrAdd(target.cid);
                        t.attackCluster = target.attackCluster;
                        t.QueueClassify(false);
                        t.attackType = target.attackType;
                        spots.Add(t);

                    }
                }

				AttackTab.attacks.Set(attacks);
                targets.Set(spots);
                //});
                UpdateStats();
                BuildAttackClusters();
            }
			return rv;

        }

		void DoRefresh()
		{
			App.DispatchOnUIThreadSneaky(() =>
			{
				attacks.NotifyReset();
				targets.NotifyReset();
				foreach (var i in attacks)
					i.OnPropertyChanged("");
				foreach (var i in targets)
					i.OnPropertyChanged("");
			});

		}
        public async override Task VisibilityChanged(bool visible)
        {
            if (visible)
            {
				App.DispatchOnUIThreadSneaky( UpdateArrivalUI );
				using var _ = await TouchLists();

				DoRefresh();

			}
			else
            {

            }
        }

        private void TargetCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Spot;
            Spot.ProcessCoordClick(spot.cid, false, App.keyModifiers,false);
            //foreach (var t in attacks)
            //{
            //    // take any one
            //    if (t.typeT == Attack.Type.se && t.targets.Contains(spot.cid))
            //    {
            //        attackGrid.ScrollIntoView(t, null);
            //        break;
            //    }
            //}

        }


        private async void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {

			using var _ = await TouchLists();

			var removeAttacks = new CheckBox() { Content = $"{attackGrid.SelectedItems.Count} attacks", IsChecked=true };
            var removeTargets = new CheckBox() { Content = $"{targetGrid.SelectedItems.Count} targets", IsChecked=true };
            var panel = new StackPanel();

            var msg = new ContentDialog()
            {
                Title="Remove Selected",
                Content=panel,
                IsPrimaryButtonEnabled=true,
                PrimaryButtonText="Remove",
                CloseButtonText="Cancel"


            };
			//ElementSoundPlayer.Play(ElementSoundKind.Show);

			msg.CopyXamlRoomFrom(sender as UIElement);
            panel.Children.Add(removeAttacks);
            panel.Children.Add(removeTargets);

            var result = await msg.ShowAsync2();
            if (result == ContentDialogResult.Primary)
            {
                if (removeAttacks.IsChecked.GetValueOrDefault())
                {
                    var temp = new List<Spot>();
                    foreach (var sel in attackGrid.SelectedItems)
                    {
                        temp.Add(sel as Spot);
                    }
                    foreach (var sel in temp)
                    {
                        var id = attacks.IndexOf(sel as Spot);
                        if (id >= 0)
                        {
                            attacks.RemoveAt(id);
                        }

                    }
                }
                if (removeTargets.IsChecked.GetValueOrDefault())
                {
                    var temp = new List<Spot>();
                    foreach (var sel in targetGrid.SelectedItems)
                    {
                        temp.Add(sel as Spot);
                    }
                    foreach (var sel in temp)
                    {
                        var id = targets.IndexOf(sel);
                        if (id >= 0)
                        {
                            targets.RemoveAt(id);
                        }

                    }
                }
                WritebackAttacks();
                CleanTargets();
            }
        }
        public async void AddAttacksFromSheets(object sender, RoutedEventArgs e)
        {
            try
            {

				using var _ = await TouchLists();
                int duplicates = 0;
                int prior = attacks.Count;
                var text = await App.GetClipboardText();
                var strs = text.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                Assert((strs.Length %3)==0);
                int count = strs.Length;
                for (int i = 0; i<count; i+=3)
                {
                    var cid = strs[i+2].FromCoordinate();
                    var atk = attacks.Find((a) => a.cid == cid);
                    var isNew = (atk == null);
                    if (isNew)
                    {
						atk = Spot.GetOrAdd(cid);
                    }
                    else
                    {
                        ++duplicates;
                    }

                    //atk.troopType =  strs[i].ToLower() switch
                    //{
                    //    "vanq" => ttVanquisher,
                    //    "sorc" => ttSorcerer,
                    //    "horses" => ttHorseman,
                    //    "siege engines" => ttScorpion,
                    //    "druids" => ttDruid,
                    //    _ => throw new ArgumentException()
                    //};

                    atk.attackType = atk.primaryTroopType == ttScorpion ? AttackType.se : strs[i+1]=="Yes" ? AttackType.senator : AttackType.assault;


					atk.attackCluster = Spot.attackClusterNone;
                    if (isNew)
                        attacks.Add(atk);
                }
                Note.Show($"Added {attacks.Count-prior}, updated {duplicates}");
                WritebackAttacks();
                await SaveAttacks();
            }
            catch (Exception ex)
            {
                LogEx(ex);
                Note.Show("Invalid format");

            }


        }

        public async void AddAttacksFromClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
				using var _ = await TouchLists();

				var text = await Clipboard.GetContent().GetTextAsync();

                if (text.IsNullOrEmpty())
                {
                    Note.Show("No clipboard text");
                    return;
                }
                AddAttacksFromString(text);

            }
            catch (Exception ex)
            {
                LogEx(ex);
                Note.Show("Invalid clipboard text");
            }
        }
		static async Task<AttackType> ChooseAttackType()
		{
				var combo = new RadioButtons() { Header = "Add as" };
				combo.Items.Add("Real Cap");
				combo.Items.Add("Fake Cap");
				combo.Items.Add("Real Demo");
				combo.Items.Add("Fake Demo");
				combo.Items.Add("None (Disable)");
			//	combo.SelectedIndex. = SettingsPage.chooseAttackTypeUpdate;

			var check = new CheckBox() { Content = "Update Existing", IsChecked = true };
				//	var removeTargets = new CheckBox() { Content = $"{targetGrid.SelectedItems.Count} targets", IsChecked = true };
				//	var panel = new StackPanel();

				var msg = new ContentDialog()
				{
					Title = "Add Attacks",
					Content = combo,
					IsPrimaryButtonEnabled = true,
					PrimaryButtonText = "Add",
					CloseButtonText = "Cancel"


				};
				if (await msg.ShowAsync2() != ContentDialogResult.Primary)
					return AttackType.invalid;
				return combo.SelectedIndex switch { 0 => AttackType.senator, 1 => AttackType.senatorFake, 2 => AttackType.se, 3 => AttackType.seFake, _=>AttackType.none };
		}
        public async void AddTargetsFromClipboard(object sender, RoutedEventArgs e)
        {

			var attackType = await ChooseAttackType();
			if (attackType == AttackType.invalid)
				return;




			using var _ = await TouchLists();
			using var work = new ShellPage.WorkScope("Adding attacks...");

                int duplicates = 0;
                var reals = 0;
                //     text = text.Replace("\r", "");
                var text = await Clipboard.GetContent().GetTextAsync();


                //    var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (Match m in AUtil.coordsRegex.Matches(text))
                {

                    try
                    {
                        if (m.Value.EndsWith(':')||m.Value.StartsWith(':'))
                            continue;
                        //  var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var cid = m.Value.FromCoordinate();

                        var spot = Spot.GetOrAdd(cid);
					if(Alliance.GetDiplomacy( spot.allianceId) == Diplomacy.allied)
					{
						Note.Show($"You are not supposed to attack your friend {Player.IdToName(spot.pid)}");
						continue;
					}
					spot.attackType = attackType;
					spot.attackCluster = Spot.attackClusterNone;
					if (targets.Contains(spot))
					{
						++duplicates;
					}
					else
					{
						targets.Add(spot);
						++reals;
					}


                    }
                    catch (Exception ex)
                    {
                        LogEx(ex);
                    }
                }
                Note.Show($"{reals} added, {duplicates} updated");
                WritebackAttacks();
                await SaveAttacks();
            
        }


        public async void AddAttacksFromString(string text)
        {
            try
            {
                using var work = new ShellPage.WorkScope("Adding attacks...");

				using var _ = await TouchLists();
				
                int duplicates = 0;
                var reals = 0;
                var fakes = 0;
                //     text = text.Replace("\r", "");
                //    var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (Match m in AUtil.coordsRegex.Matches(text))
                {

                    try
                    {
                        if (m.Value.EndsWith(':')||m.Value.StartsWith(':'))
                            continue;
                        //  var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var cid = m.Value.FromCoordinate();
                        var atk = attacks.Find((a) => a.cid == cid);
                        var isNew = (atk == null);
                        if (isNew)
                        {
							atk =  Spot.GetOrAdd(cid);
							++reals;
                        }
                        else
                        {
                            ++duplicates;
                        }
                        var spot = Spot.GetOrAdd(cid);
                        var cl = await spot.ClassifyEx(true);
                        //  string s = $"{cid.CidToString()} {Player.myName} {cl.} {(cl.academies == 1 ? 2 : 0)} {tsTotal}\n";
                        
                        if (cl == Spot.Classification.se)
                            atk.attackType = AttackType.se;
                        else if (spot.hasAcademy.GetValueOrDefault())
                            atk.attackType = AttackType.senator;
                        else atk.attackType = AttackType.assault;

						//atk.type = (int)Attack.Type.senator;

						//atk.fake = parts[3].ToLowerInvariant() == "fake";
						//var tt = parts[2].ToLowerInvariant();
						//if (tt.StartsWith("sorc"))
						//    atk.troopType = ttSorcerer;
						//else if (tt.StartsWith("horse"))
						//    atk.troopType = ttHorseman;
						//else if (tt.StartsWith("van"))
						//    atk.troopType = ttVanquisher;
						//else if (tt.StartsWith("druid"))
						//    atk.troopType = ttDruid;
						//else atk.troopType = ttGuard;

						//atk.
						//if (int.TryParse(parts[2], out var ts))
						//{
						//    atk.ts = ts;
						//}

						atk.attackCluster = Spot.attackClusterNone;


						if (isNew)
                            attacks.Add(atk);
                    }
                    catch (Exception ex)
                    {
                        LogEx(ex);
                    }
                }
                Note.Show($"{reals-duplicates} added, {duplicates} updated");
                WritebackAttacks();
                await SaveAttacks();
            }
            catch (Exception ex)
            {
                LogEx(ex);
                Note.Show("Invalid clipboard text");
            }
        }

        private void SortAttacks()
        {
	//		using var _ = await TouchLists();
			
			attacks.Sort((a, b) =>
            {
                var c = a.playerName.CompareTo(b.playerName);
                if (c != 0)
                    return c;
                c = a.attackCluster.CompareTo(b.attackCluster); 
				if (c != 0)
					return c;
				return a.isAttackTypeAssault.CompareTo(b.isAttackTypeAssault);
			}
			);
            targets.Sort((a, b) =>
            {
                var c = a.attackCluster.CompareTo(b.attackCluster);
                if (c!=0)
                    return c;
                return a.isAttackTypeFake.CompareTo(b.isAttackTypeFake);
            });

            attacks.NotifyReset();
            targets.NotifyReset();

            WritebackAttacks();
            //SaveAttacks();
           
        }


        internal static async void AddTarget(IEnumerable<int> cids)
        {
			var attackType = await ChooseAttackType();
			using var _ =await instance.TouchLists();
			
			if (attackType == AttackType.invalid)
				return;
				int added = 0, cities=0,updated = 0, wrongAlliance=0;
			foreach (var cid in cids)
			{
				var spot = Spot.GetOrAdd(cid);
				if(Alliance.IsAllyOrNap(spot.allianceId) )
				{
					++wrongAlliance;
					continue;
				}
				if (!spot.isCastle)
				{
					++cities;
					continue;
				}
				spot.attackType = attackType;
				spot.QueueClassify(false);
				spot.attackCluster = Spot.attackClusterNone;
				if (targets.Contains(spot))
				{
					++updated;
				}
				else
				{
					++added;
					targets.Add(spot);
				}
			}
			Note.Show($"Added:{added}, Updated:{updated} wrong alliance:{wrongAlliance} cities:{cities}");
			WritebackAttacks();
            await SaveAttacks();

        }

		internal static async void IgnorePlayer(int pid)
		{
			using var _ = await instance.TouchLists();

			int removed = 0;
			foreach (var t in targets)
			{
				if(t.pid == pid )
				{
					t.attackType = AttackType.none;
					++removed;
				}
			}
			Note.Show($"Ignoring:{removed} targets from {Player.IdToName(pid)}");
			WritebackAttacks();
			await SaveAttacks();

		}

		private static void CleanTargets()
        {
            // var set = new HashSet<int>(targets.Count);
            foreach (var t in targets)
            {

                t.attackCluster= Spot.attackClusterNone;
                // set.Add(t.cid);
            }
            foreach (var a in attacks)
            {
                a.attackCluster = Spot.attackClusterNone;
				Spot.GetOrAdd(a.cid).attackCluster = Spot.attackClusterNone;

			}

        }
		// internal working struct
		class Cluster
		{
			public int id; // redundant
			public AttackCategory category;
			public int[] attackCounts = new int[2]; // 0 is real, 1 is fake
			public List<Spot> fakes;// first is SE or siege
			public Spot real;
			public Span2 span;
			//public uint sortKey => real!=null ? real.spatialIndex : (uint)id;
			public bool isValid => real != null && category!=AttackCategory.invalid;
			public void SetInvalid()
			{
				real = null;
				fakes.Clear();
				category = AttackCategory.invalid;
			}
			public Span2 CalculateSpan() => new Span2(fakes.Append(real));
			public Span2 CalculateSpanWithout(Spot exclude) => new Span2(fakes.Where(a=>a!=exclude).Append(real));
			public IEnumerable<Spot> targets => fakes.Append(real);

			

			public void UpdateSpan() => span = CalculateSpan();
			public float CalculateAttackCost(Spot attacker)
			{
				if (!isValid)
					return float.MaxValue;

				var morale = Player.MoralePenalty(attacker.pid, real.pid) * moralCostBias;

				if (category == AttackCategory.se)
					return span.Distance2(attacker.cid.ToWorldC())+morale;
				// fakes don't matter for distance
				var score = (attacker.cid.DistanceToCid(real.cid).Squared()+morale); // weighted down
				switch (attacker.primaryTroopType)
				{
					case ttSorcerer:
					case ttDruid:
						{
							switch (real.classification)
							{
								case Spot.Classification.unknown:
									break;
								case Spot.Classification.vanqs:
								case Spot.Classification.rt:
									score -= 3*4;
									break;
								case Spot.Classification.sorcs:
								case Spot.Classification.druids:
									score += 2 * 4;
									break;
								case Spot.Classification.praetor:
								case Spot.Classification.priestess:
									score += 8 * 4;
									break;
								case Spot.Classification.horses:
								case Spot.Classification.arbs:
									score -= 4 * 4;
									break;
								case Spot.Classification.se:
									break;
								case Spot.Classification.hub:
									break;
								case Spot.Classification.navy:
									break;
								case Spot.Classification.misc:
									break;
								default:
									break;
							}
							break;
						}
					case ttVanquisher:
						{
							switch (real.classification)
							{
								case Spot.Classification.unknown:
									break;
								case Spot.Classification.vanqs:
								case Spot.Classification.rt:

									score += 4 * 4;
									break;
								case Spot.Classification.sorcs:
								case Spot.Classification.druids:
									score -= 2 * 4;
									break;
								case Spot.Classification.praetor:
								case Spot.Classification.priestess:
									score -= 4 * 4;
									break;
								case Spot.Classification.horses:
								case Spot.Classification.arbs:

									score -= 3 * 4;
									break;
								case Spot.Classification.se:
									break;
								case Spot.Classification.hub:
									break;
								case Spot.Classification.navy:
									break;
								case Spot.Classification.stingers:
									break;
								case Spot.Classification.misc:
									break;
								default:
									break;
							}
							break;
						}
					case ttHorseman:
						{
							switch (real.classification)
							{
								case Spot.Classification.unknown:
									break;
								case Spot.Classification.vanqs:
								case Spot.Classification.rt:

									score += 2 * 4;
									break;
								case Spot.Classification.sorcs:
								case Spot.Classification.druids:
									score -= 2 * 4;
									break;
								case Spot.Classification.praetor:
									score -= 6 * 4;
									break;
								case Spot.Classification.priestess:
									score += 1 * 4;
									break;
								case Spot.Classification.horses:
								case Spot.Classification.arbs:

									score += 7 * 4;
									break;
								case Spot.Classification.se:
									break;
								case Spot.Classification.hub:
									break;
								case Spot.Classification.navy:
									break;
								case Spot.Classification.misc:
									break;
								default:
									break;
							}
							break;
						}
				}
				return score;
			}
			public float GetTravelTime(Spot attacker)
			{
				Assert(isValid);
				var tt = (category == AttackCategory.senator) ? ttSenator: attacker.primaryTroopType;
				var t = tt.ApproxTravelTime(attacker.cid, real.cid);
				foreach (var f in fakes)
				{
					t = t.Max(tt.ApproxTravelTime(attacker.cid, f.cid));
				}
				return t;
			}
			public bool FakesValid()
			{
			
				if (real.attackType.GetCategory() != category)
					return false;
				return fakes.All(f => f.attackType.GetCategory() == category);
			}
			public bool IsInRange(Spot attacker)
			{
				if (!isValid)
					return false;
				//if (category == AttackCategory.senator)
				//	return true;
				var tt = attacker.primaryTroopType;
				return GetTravelTime(attacker) <= (category == AttackCategory.senator ? SettingsPage.attackMaxTravelHoursSen : SettingsPage.attackMaxTravelHoursSE);


			}

		}

			
		private async void AssignAttacks_Click(object sender, RoutedEventArgs e)
        {
			using var _ = await TouchLists();
			var ignoredTargets = new HashSet<int>();
			//   int[] initialClusterCount = { targets.Count(a => a.isAttackTypeReal && a.isAttackTypeSE),
			//	targets.Count(a => a.isAttackTypeReal && a.isAttackTypeSenator)};

			for (int ignoreIterator = 0; ; ++ignoreIterator)
			{
				var attacks = AttackTab.attacks.OrderBy(a => ((long)a.player.points << 32) + a.spatialIndex).ToArray();

				// first create clusters
				foreach (var f in targets)
				{
					f.attackCluster = Spot.attackClusterNone;
				}
				foreach (var a in attacks)
				{
					a.attackCluster = Spot.attackClusterNone;
				}
				// Todo: order by score?
				var reals = targets.Where((a) => a.isAttackTypeSiege && !ignoredTargets.Contains(a.cid)).OrderBy(a=>a.spatialIndex).ToArray();
				if (!reals.Any())
				{
					Note.Show("No reals");
					return;
				}
				using var work = new ShellPage.WorkScope($"Planning.. (pass {ignoreIterator})");

			
				var maxFakes = (int)this.maxFakes.Value;
				var minAssaults = (int)this.minAssaults.Value;
				var clusterCount = reals.Length;
				var fakes = targets.Where( (a)=>a.isAttackTypeFake ).OrderBy( (a)=>a.spatialIndex ).ToArray();
				var clusters = new Cluster[clusterCount];
				{
					for (var c0 = 0; c0 < clusterCount; ++c0)
					{
						var real = reals[c0];
						clusters[c0]= new Cluster() { id = c0, category = real.attackType.GetCategory(), fakes = new List<Spot>(), real = real, span = new Span2(real.cid.ToWorldC()) };
						real.attackCluster = c0;
					}
				}
				//	var fakeCountPerCluster = new int[AttackCategory.count.Value()];
				//	for (int j = 0; j < (int)AttackCategory.count; ++j)
				//		fakeCountPerCluster[j] = (fakes[j].Length / initialClusterCount[j]).Min(maxFakes); // rounds down

				// Group together fakes
					foreach (var fake in fakes)
					{
						//   var a = Spot.GetOrAdd(attack.cid);
				//		var fake = _fake[(int)t];
						var attackType = fake.attackType;
					var category = attackType.GetCategory();
						Cluster best = null;
						float bestDist = float.MaxValue;

						foreach (var c in clusters )
						{
							if (c.category != category || !c.isValid)
								continue;
							var real = c.real;
						

							// Cannot offset even distribution

							if (c.fakes.Count >= maxFakes.Min(clusters.Where(c => c.category == category).Min(a => a.fakes.Count) + 1))
								continue;

							var d = c.span.Distance2(fake.cid.ToWorldC());
							if (d < bestDist)
							{
								bestDist = d;
								best = c;
							}
						}

						if (best != null)
						{
							
							best.fakes.Add(fake);
							Assert(best.FakesValid());
							fake.attackCluster = best.id;
							best.span += fake.cid.ToWorldC();
						}
					}
				

				// optimize
				// first pass, shuffle the extras
				for (int iter = 0; iter < clusterCount + 8; ++iter)
				{
					foreach(var cluster0 in clusters)
					{
						if (!cluster0.isValid)
							continue;
						var category = cluster0.category;
						var r0a = cluster0.span.radius2;
						Spot best = null;
						Cluster bestCluster = null;
						float bestScore = 0;
						var count0 = cluster0.fakes.Count;
						foreach (var cluster1 in clusters)
						{

							if (cluster1.fakes.Count <= count0 || cluster1.category != category)
								continue;
							var r1a = cluster1.span.radius2;

							foreach (var f0 in cluster0.fakes)
							{
								Assert(f0.attackType.GetCategory() == category);
								var r0b = Span2.UnionWithout(cluster0.targets, f0).radius2;
								var r1b = new Span2(cluster1.targets.Append(f0)).radius2;

								var d1 = cluster1.span.Distance2(f0.cid.ToWorldC());
								var score = (r0b + r1b) - (r0a + r1a);
								if (score < bestScore)
								{
									bestScore = score;
									bestCluster = cluster1;
									best = f0;
								}

							}

						}
						if (best != null)
						{
							best.attackCluster = bestCluster.id;
							bestCluster.fakes.Add(best);
							cluster0.fakes.Remove(best);
							Assert(cluster0.FakesValid());
							
							bestCluster.UpdateSpan();
							cluster0.UpdateSpan();
						}

					}
				}
				// optimize
				for (int iter = 0; iter < 32; ++iter)
				{
					foreach(var cluster0 in clusters)
					{
						if (!cluster0.isValid)
							continue;
						foreach (var cluster1 in clusters)
						{
							if (cluster0.id >= cluster1.id)
								continue;

							if (cluster0.category != cluster1.category)
								continue;
							var span0a = cluster0.span;
							var span1a = cluster1.span;
							Spot bestSwap0 = null;
							Spot bestSwap1 = null;
							float bestScore = 0;

							foreach (var f0 in cluster0.fakes)
							{
								var span0b = Span2.UnionWithout(cluster0.targets, f0);
								Assert(f0.attackType.GetCategory() == cluster0.category);
								foreach (var f1 in cluster1.fakes)
								{
									var span1b = Span2.UnionWithout(cluster1.targets, f1);
									Assert(f1.attackType.GetCategory() == cluster1.category);

									var span0c = span0b + f1.cid.ToWorldC();
									var span1c = span1b + f0.cid.ToWorldC();
									// L4 distance
									var scoreA = span0a.radius2 + span1a.radius2;
									var scoreC = span0c.radius2 + span1c.radius2;
									var delta = scoreC - scoreA;
									// swap if it improves things
									if (delta < bestScore)
									{
										bestSwap0 = f0;
										bestSwap1 = f1;
										bestScore = delta;
									}
								}
							}

							if (bestSwap0 != null)
							{
								cluster0.fakes[cluster0.fakes.IndexOf(bestSwap0)] = bestSwap1;
								cluster1.fakes[cluster1.fakes.IndexOf(bestSwap1)] = bestSwap0;
								bestSwap0.attackCluster = cluster1.id;
								bestSwap1.attackCluster = cluster0.id;

								Assert(cluster0.FakesValid());
								Assert(cluster1.FakesValid());

								cluster0.UpdateSpan();
								cluster1.UpdateSpan();
							}

						}
					}
				}


				
				// Assign attacks to clusters
				var attackCount = attacks.Length;
				var seCount = attacks.Count(a => a.isAttackTypeSE);
				var senCount = attacks.Count(a => a.isAttackTypeSenator);
				var assaults = attackCount - seCount - senCount;

				foreach (var attack in attacks)
				{
					///
					/// assaults are first allocated to SE
					/// 
					//   var a = Spot.GetOrAdd(attack.cid);
					var attackType = attack.attackType;
					int best = -1;
					float bestDist = float.MaxValue;
					var isAssault = attack.isAttackTypeAssault ? 1 : 0;
					for (int useAssaultForSenators = 0; useAssaultForSenators < 2; ++useAssaultForSenators)
					{
						foreach(var c in clusters)
						{
							if (!c.isValid)
								continue;
							var category = c.category;
							if (category == AttackCategory.se)
							{
								if (attackType != AttackType.se)
								{
									if (useAssaultForSenators == 1 || attackType != AttackType.assault)
										continue;
								}

							}
							else
							{
								// assaults are allocated to SE unless out of range
								if (attackType != AttackType.senator)
								{
									if (useAssaultForSenators == 0 || attackType != AttackType.assault)
										continue;
								}
							}

							if (!c.IsInRange(attack))
								continue;

							{
								var maxPerCluster = clusters.Where(a => a.category == category).Min(a => a.attackCounts[isAssault]) + 1;

								if (c.attackCounts[isAssault] >= maxPerCluster)
									continue;
							}

							var d = c.CalculateAttackCost(attack);
							if (d < bestDist)
							{
								bestDist = d;
								best = c.id;
							}
						}

						if (best >= 0)
						{
							++clusters[best].attackCounts[isAssault];
							attack.attackCluster = best;
							break;
						}
						if (isAssault == 0)
							break;
						// useAssaultsForSenators will be 1 here and isAssault will be 1
						// try a second search for assaults vs senators...
					}

				}



				// optimize

				// first pass, shuffle the extras, only if there are more attacks than targets
				for (int iter = 0; iter < clusterCount + 4; ++iter)
				{
					foreach(var cluster0 in clusters)
					{
						if (!cluster0.isValid)
							continue;
					
						for (var isAssault = 0; isAssault < 2; ++isAssault)
						{
							var count0 = cluster0.attackCounts[isAssault];
							if (count0 < clusters.Where(a => a.category == cluster0.category).Max(a => a.attackCounts[isAssault]))
								continue;
							Spot best = null;
							Cluster bestCluster = null;
							float bestScore = 0;
							foreach (var cluster1 in clusters)
							{
								
								var count1 = cluster1.attackCounts[isAssault];
								Assert(count1 <= count0);

								if (count1 >= count0 || cluster0.category!=cluster1.category)
									continue;
								foreach (var f0 in attacks)
								{
									if (f0.attackCluster != cluster0.id || (f0.isAttackTypeAssault != (isAssault == 1)) || !cluster1.IsInRange(f0))
										continue;
									var d0 = cluster0.CalculateAttackCost(f0);
									var d1 = cluster1.CalculateAttackCost(f0);
									var score = d1 - d0;
									if (score < bestScore)
									{
										bestScore = score;
										bestCluster = cluster1;
										best = f0;
									}

								}

							}
							if (best != null)
							{
								best.attackCluster = bestCluster.id;
								--cluster0.attackCounts[isAssault];
								++bestCluster.attackCounts[isAssault];
							}
						}
					}
				}
				// second pass, symetric swaps
				for (int iter = 0; iter < 16; ++iter)
				{
					foreach (var f0 in attacks)
					{
						//                    var s0 = Spot.GetOrAdd(f0.cid);
						if (f0.isAttackClusterNone )
							continue;
						var c0 = f0.attackCluster;
						var attackType = f0.attackType;
						foreach (var f1 in attacks)
						{
							if (f1.isAttackClusterNone || f1.attackType != attackType)
								continue;
							//var s1 = Spot.GetOrAdd(f1.cid);
							var c1 = f1.attackCluster;
							if (c0 == c1)
								continue;
							if (!clusters[c0].IsInRange(f1) ||
								!clusters[c1].IsInRange(f0))
								continue;
							// least squares
							var d0 = clusters[c0].CalculateAttackCost(f0);
							var d1 = clusters[c1].CalculateAttackCost(f1);

							var dd0 = clusters[c0].CalculateAttackCost(f1);
							var dd1 = clusters[c1].CalculateAttackCost(f0);
							if (d0 + d1 > dd0 + dd1)
							{
								// a change improves things
								f1.attackCluster = c0;
								f0.attackCluster = c1;
								c0 = c1;
							}

						}

					}
				}
				// cull
				var initialCulledClusters = ignoredTargets.Count;
				// cull clusters with too few attacks
				var culled = 0;
				foreach(var cluster in clusters)
				{
						if (!cluster.isValid)
							continue;
						if (cluster.attackCounts[0] == 0 || (cluster.attackCounts[1] < minAssaults && cluster.category == AttackCategory.se))
						{
							var a = ignoredTargets.Add(cluster.real.cid);
							Assert(a == true);

							foreach (var f0 in attacks)
							{
								if (f0.attackCluster == cluster.id)
								{
									f0.attackCluster = Spot.attackClusterNone;
								}
							}
							cluster.real.attackCluster = Spot.attackClusterNone;
							foreach (var t in cluster.fakes)
							{
								t.attackCluster = Spot.attackClusterNone;
							}
							cluster.SetInvalid();
						}
				}
				
				if(!clusters.Any(a=>a.isValid))
				{
					Note.Show("No valid attack clusters (not enough attacks or out of range?)");
					return;
				}
				if (ignoredTargets.Count > initialCulledClusters && ignoreIterator < 16)
					continue;


				WritebackAttacks();
				
				//         var targetCluster = new List<int>();
				//           targetCluster.Add(real.cid);
				//            targetCluster.Add(bestFake);
				var seAttacks = attacks.Where((a) => a.isAttackTypeSE && !a.isAttackClusterNone);
				var assaultAttacks = attacks.Where((a) => a.isAttackTypeAssault && !a.isAttackClusterNone);
				var senatorAttacks = attacks.Where((a) => a.isAttackTypeSenator && !a.isAttackClusterNone);
				float maxDistanceToSE = seAttacks.Any()? seAttacks.Max((a) => clusters[a.attackCluster].GetTravelTime(a) ): 0;
				float averageDistanceToSE = seAttacks.Any() ? seAttacks.Average((a) => clusters[a.attackCluster].GetTravelTime(a)):0;
				float maxDistanceToAssault = assaultAttacks.Any()?assaultAttacks.Max((a) => clusters[a.attackCluster].GetTravelTime(a)):0;
				float averageDistanceToAssault = assaultAttacks.Any()?assaultAttacks.Average((a) => clusters[a.attackCluster].GetTravelTime(a)):0;

				float maxDistanceToSenator= senatorAttacks.Any()?senatorAttacks.Max((a) => clusters[a.attackCluster].GetTravelTime(a)):0;
				float averageDistanceToSenator = senatorAttacks.Any()?senatorAttacks.Average((a) => clusters[a.attackCluster].GetTravelTime(a)):0;

				float maxClusterSize = clusters.Where(c=>c.isValid).Max((a) => a.span.radius2.Sqrt());
				float averageClusterSize = clusters.Where(c => c.isValid).Average((a) => a.span.radius2.Sqrt());

				//           bad = $"{bad} Assigned {reals} reals and {fakes} fakes\nUnused: reals {unusedReals}, fakes {unusedFakes}";

				Note.Show($"Attack plan done, {ignoredTargets.Count} culled real targets, {attacks.Count(a => a.isAttackClusterNone)} culled attacks, {clusterCount} valid targets, SE Distance max: {maxDistanceToSE} av: {averageDistanceToSE}, Senator Distance max:{maxDistanceToSenator} av:{averageDistanceToSenator} Assault distance max:{maxDistanceToAssault} av:{averageDistanceToAssault} Cluster size max: {maxClusterSize} av: {averageClusterSize}");

				for (int j = 0; j < 2; ++j)
				{
					foreach (var a in (j == 0 ? senatorAttacks : seAttacks))
					{
						var d = clusters[a.attackCluster].GetTravelTime(a);
						if (d >= (j == 0 ? maxDistanceToSenator : maxDistanceToSE) - 3.0f)
						{
							Note.Show($"{a.nameMarkdown} leaves at around {(SettingsPage.attackPlannerTime - TimeSpan.FromHours(d)).FormatDefault()}");
						}
					}
				}
				DoRefresh();
				//StringBuilder sb = new StringBuilder();
				//foreach (var a in attacks)
				//{
				//    if (a.target!=0)
				//    {
				//        sb.Append($"{a.player} <coords>{a.target.CidToString()}</coords> {AttackType.types[a.type].name} {(a.fake ? "Fake" : "Real")} at {time.FormatDefault()}\n");
				//    }
				//}
				//App.CopyTextToClipboard(sb.ToString());

				AttackSender_Tapped();
				
				return;
			}
        }

        //private void AssignAttacks_Click(object sender, RoutedEventArgs e)
        //{
        //    string bad = string.Empty;
        //    var reals = 0;
        //    var fakes = 0;
        //    CleanTargets();

        //    foreach (var a in attacks)
        //    {
        //        if (a.target != 0) // not already assigned
        //        {
        //            if (!a.fake)
        //                ++reals;
        //            else
        //                ++fakes;
        //            continue;
        //        }
        //        Spot best = null;
        //        float bestScore = float.MaxValue;
        //        foreach (var t in targets)
        //        {
        //            if (a.fake == (t.attackCluster == 0)) // real or fake match
        //                continue;
        //            if (t.attackAssignment != null)
        //                continue; // already taken
        //            var score = a.cid.DistanceToCid(t.cid);
        //            if (!a.fake)
        //            {
        //                switch (a.troopType)
        //                {
        //                    case ttSorcerer:
        //                    case ttDruid:
        //                        {
        //                            switch (t.classification)
        //                            {
        //                                case Spot.Classification.unknown:
        //                                    break;
        //                                case Spot.Classification.vanqs:
        //                                case Spot.Classification.rt:
        //                                    score -= 3;
        //                                    break;
        //                                case Spot.Classification.sorcs:
        //                                case Spot.Classification.druids:
        //                                    score += 2;
        //                                    break;
        //                                case Spot.Classification.academy:
        //                                    score += 8;
        //                                    break;
        //                                case Spot.Classification.horses:
        //                                case Spot.Classification.arbs:
        //                                    score -= 4;
        //                                    break;
        //                                case Spot.Classification.se:
        //                                    break;
        //                                case Spot.Classification.hub:
        //                                    break;
        //                                case Spot.Classification.navy:
        //                                    break;
        //                                case Spot.Classification.misc:
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        }
        //                    case ttVanquisher:
        //                        {
        //                            switch (t.classification)
        //                            {
        //                                case Spot.Classification.unknown:
        //                                    break;
        //                                case Spot.Classification.vanqs:
        //                                case Spot.Classification.rt:

        //                                    score += 4;
        //                                    break;
        //                                case Spot.Classification.sorcs:
        //                                case Spot.Classification.druids:
        //                                    score -= 2;
        //                                    break;
        //                                case Spot.Classification.academy:
        //                                    score -= 4;
        //                                    break;
        //                                case Spot.Classification.horses:
        //                                case Spot.Classification.arbs:

        //                                    score -= 3;
        //                                    break;
        //                                case Spot.Classification.se:
        //                                    break;
        //                                case Spot.Classification.hub:
        //                                    break;
        //                                case Spot.Classification.navy:
        //                                    break;
        //                                case Spot.Classification.misc:
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        }
        //                    case ttHorseman:
        //                        {
        //                            switch (t.classification)
        //                            {
        //                                case Spot.Classification.unknown:
        //                                    break;
        //                                case Spot.Classification.vanqs:
        //                                case Spot.Classification.rt:

        //                                    score += 2;
        //                                    break;
        //                                case Spot.Classification.sorcs:
        //                                case Spot.Classification.druids:
        //                                    score -= 2;
        //                                    break;
        //                                case Spot.Classification.academy:
        //                                    score -= 6;
        //                                    break;
        //                                case Spot.Classification.horses:
        //                                case Spot.Classification.arbs:

        //                                    score += 7;
        //                                    break;
        //                                case Spot.Classification.se:
        //                                    break;
        //                                case Spot.Classification.hub:
        //                                    break;
        //                                case Spot.Classification.navy:
        //                                    break;
        //                                case Spot.Classification.misc:
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        }
        //                }
        //            }
        //            if (score < bestScore)
        //            {
        //                bestScore = score;
        //                best = t;

        //            }

        //        }
        //        if (best == null)
        //        {
        //            bad = $"{bad}No target for {a.xy} isReal: {!a.fake}\n";

        //        }
        //        else
        //        {
        //            a.target = best.cid;
        //            best.attackAssignment = a;
        //            if (!a.fake)
        //                ++reals;
        //            else
        //                ++fakes;

        //        }
        //    }
        //    var unusedReals = 0;
        //    var unusedFakes = 0;
        //    foreach (var a in attacks)
        //    {
        //        if (a.target == 0)
        //        {
        //            if (a.fake)
        //                ++unusedFakes;
        //            else
        //                ++unusedReals;
        //        }
        //    }

        //    bad = $"{bad} Assigned {reals} reals and {fakes} fakes\nUnused: reals {unusedReals}, fakes {unusedFakes}";

        //    Note.Show(bad);
        //    attacks.NotifyReset();
        //    targets.NotifyReset();
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var a in attacks)
        //    {
        //        if (a.target!=0)
        //        {
        //            sb.Append($"{a.player} <coords>{a.target.CidToString()}</coords> {AttackType.types[a.type].name} {(a.fake ? "Fake" : "Real")} at {time.FormatDefault()}\n");
        //        }
        //    }
        //    App.CopyTextToClipboard(sb.ToString());
        //    SaveAttacks();
        //}

        private async void TargetRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
			using  var _ = await TouchLists();
			var i = sender as FrameworkElement;

            var spot = i.DataContext as Spot;
            targets.Remove(spot);
            CleanTargets();
            WritebackAttacks();
            await SaveAttacks();

        }
        private async void AttackRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
			using var _ = await TouchLists();

			var i = sender as FrameworkElement;

            var spot = i.DataContext as Spot;
            attacks.Remove(spot);
            WritebackAttacks();
            await SaveAttacks();
        }

        private async void ArrivalTapped(object sender, RoutedEventArgs e)
        {
            (var dateTime, var okay) = await DateTimePicker.ShowAsync("Send At", SettingsPage.attackPlannerTime);
            if (okay)
            {
				SettingsPage.attackPlannerTime = dateTime;
				UpdateArrivalUI();
			}
        }

        private void ClearTargets_Click(object sender, RoutedEventArgs e)
        {
            CleanTargets();
            attacks.NotifyReset();
        }

		private void SelectTargets(object sender, RoutedEventArgs e)
		{
			var sel = targetGrid.SelectedItems;
			foreach (var i in targets)
			{
				i.SelectMe();
				if (!sel.Contains(i))
					sel.Add(i);
			}
		}
		private void SelectAttackers(object sender, RoutedEventArgs e)
		{
			var sel = attackGrid.SelectedItems;
			foreach (var i in attacks)
			{
				i.SelectMe();
				if (!sel.Contains(i))
					sel.Add(i);
			}
		}
	}
}
