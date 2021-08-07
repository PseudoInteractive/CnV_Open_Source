using COTG;
using COTG.Game;
using COTG.Helpers;

using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
using Nito.AsyncEx;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using static COTG.Debug;
using static COTG.Game.AttackPlan;
using static COTG.Game.Enum;
namespace COTG.Views
{

	public sealed partial class AttackTab : UserTab, INotifyPropertyChanged
    {
		public static AsyncLock asyncLock = new ();
		public static StorageFolder folder => ApplicationData.Current.LocalFolder;
        public static AttackTab instance;
		
        public static bool IsVisible() => instance.isVisible;

        public static DumbCollection<City> attacksUI = new ();
        public static DumbCollection<City> targetsUI = new ();
		public static ImmutableArray<AttackPlanCity> attacks => AttackPlan.plan.attacks;
		public static ImmutableArray<AttackPlanCity> targets => AttackPlan.plan.targets;
		
		public AttackTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
        }
		static void UpdateArrivalUI()
		{
			if(0 != plan.senTime )
				instance.arrivalSen.Content = senTime.FormatDefault();
			if (0 != plan.seTime)
				instance.arrivalSE.Content = seTime.FormatDefault();
		}


		public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName="") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		
		static Dictionary<int, string> attackStrings = new();
		static Dictionary<int, string> playerCommands = new();

		public static int GetFixedTarget(AttackPlanCity city) => city.fixedTarget;
		public static bool IsTargetFixed(AttackPlanCity city) => GetFixedTarget(city) != 0;
		private async void AttackSender_Tapped(object ____=null, object __=null)
        {
			try
			{
				using var _ = await TouchLists();

				SortAttacks();


				//       var clusterCount = attackClusters.Length;
				//            var used = new HashSet<int>(readable.attacks.Length);

				//        var scripts = new string[attackClusters.Max(a=>a.id) + 1];
				//        //int clusterId = 0;
				//        foreach (var cluster in attackClusters)
				//        {
				//            var atk = new AttackSenderScript() { type = new List<int>(), x = new List<int>(), y = new List<int>() };
				//            // group all attacks for this player
				//            foreach (var a1 in cluster.targets.OrderBy((a) => City.GetOrAdd(a).isAttackTypeFake))
				//            {
				//                var t = City.GetOrAdd(a1);

				//                atk.type.Add(t.isAttackTypeFake ? 0 : 1);

				//                var xy = a1.CidToWorld();
				//                atk.x.Add(xy.x);
				//                atk.y.Add(xy.y);
				//            }
				//var time = SettingsPage.attackPlannerTime;

				//atk.time = new string[] { time.Hour.ToString("00"), time.Minute.ToString("00"), time.Second.ToString("00"), time.ToString("MM/dd/yyyy") };
				//            scripts[cluster.id]= System.Text.Json.JsonSerializer.Serialize(atk, Json.jsonSerializerOptions);
				//        }
				attackStrings.Clear();
				playerCommands.Clear();
				StringBuilder sb = new StringBuilder();
				var players = new List<int>();
				var assaultOffsets = new int[attackClusters.Length];
				string allCommands = "";
				foreach (var a in attacks)
				{
					var player = City.GetOrAdd(a.cid).pid;
					if (players.Contains(player))
						continue;
					players.Add(player);
				}
				foreach (var player in players)
				{
					sb.Append($"\n\n{Player.IdToName(player)}");
					foreach (var a in attacks)
					{
						if (City.GetOrAdd(a.cid).pid != player || a.attackCluster == -1)
							continue;
						if (true)
						{
							var atk = new AttackSenderScript() { type = new List<int>(), x = new List<int>(), y = new List<int>() };
							atk.cid = a.cid;
							// group all attacks for this player
							var c = attackClusters[a.attackCluster];
							foreach (var t in c.targets.Select(a => AttackPlan.Get(a)).OrderBy(a => a.isAttackTypeFake))
							{
								atk.type.Add(t.isAttackTypeFake ? 0 : 1);
								var xy = t.cid.CidToWorld();
								atk.x.Add(xy.x);
								atk.y.Add(xy.y);
							}

							bool isSE = c.real.AsCity().attackType == AttackType.se;
							var time = isSE ? seTime : senTime;
							atk.command = a.isAttackTypeAssault ? "Assault" : "Siege";
							if (a.isAttackTypeAssault && !isSE)
							{
								// We can assault twice
								if (a.TraveltimeMinutes(c.real) <= (plan.ticksToCapture - 1) * 25)
								{
									// hit at start first
								}
								else
								{
									// var offset = ref assaultOffsets[a.attackCluster];
									if (assaultOffsets[a.attackCluster] == 0)
										assaultOffsets[a.attackCluster] = AMath.random.Next();
									assaultOffsets[a.attackCluster] += plan.ticksToCapture / 2 + 1;
									time += TimeSpan.FromHours(assaultOffsets[a.attackCluster] % plan.ticksToCapture);
								}
							}
							atk.time = new string[] { time.Hour.ToString("00"), time.Minute.ToString("00"), time.Second.ToString("00"), time.ToString("MM/dd/yyyy") };
							var scrpipt = System.Text.Json.JsonSerializer.Serialize(atk, Json.jsonSerializerOptions);

							attackStrings.Add(a.cid, scrpipt);

							sb.Append($"\n\n<player>{scrpipt}</player>");
						}
						sb.Append('\n');
						sb.Append($"{a.cid.CidToCoords()} {a.city.classificationString} {a.attackType}");
					}
					var command = sb.ToString();
					playerCommands.Add(player, command);
					allCommands += command;
					sb.Clear();
				}
				App.CopyTextToClipboard(allCommands);
				Note.Show("Copied Attack sender scripts to clipboard");
				SaveAttacks();
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
		}

        private async void AttackTargetCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var atk = i.DataContext as City;
				if (atk.isMine)
				{
					if (attackStrings.TryGetValue(atk.cid, out var script))
					{
						await JSClient.OpenAttackSender(script);

					}
					else
					{
						Note.Show("No attack string");
					}
				}
				else
				{

				}

			var cluster = atk.attackCluster;
			if(cluster != -1)
			{


			}
			//if (atk.targets.Any() )
			//{
			//    var id = atk.targets.FindIndex(City.focus)+1;
			//    if (id >= atk.targets.Length)
			//        id =0;
			//    var cid = atk.targets[id];
			//    var spot = City.GetOrAdd(cid);
			//    City.ProcessCoordClick(cid, false,  App.keyModifiers);
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

		private void RemoveSortingIndicators(DataGrid dg)
		{
			foreach (var dgColumn in dg.Columns)
			{
				
					dgColumn.SortDirection = null;
				
			}

		}
		private void Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
		{
			var dg = sender as DataGrid;
			var cities = dg == attackGrid ? attacksUI : targetsUI;
			var tag = e.Column.Tag != null? e.Column.Tag.ToString() : e.Column.Header.ToString();
			//Use the Tag property to pass the bound column name for the sorting implementation
			Comparison<City> comparer = null;
			switch (tag)
			{
				case "ts": comparer = (a, b) => a.tsTotal.CompareTo(b.tsTotal); break;
				case nameof(City.classification): comparer = (a, b) => a.classification.CompareTo(b.classification); break;
				case "name": comparer = (a, b) => b.nameAndRemarks.CompareTo(a.nameAndRemarks); break;
				case nameof(City.attackCluster): comparer = (a, b) => b.attackCluster.CompareTo(a.attackCluster); break;
				case nameof(City.attackType): comparer = (a, b) => b.attackType.CompareTo(a.attackType); break;
				case "Points": comparer = (a, b) => b.points.CompareTo(a.points); break;
				case nameof(City.xy): comparer = (a, b) => CompareCid(a.cid,b.cid); break;
				case "Water": comparer = (a, b) => a.isOnWater.CompareTo(b.isOnWater); break;
				case "Player": comparer = (a, b) => a.playerName.CompareTo(b.playerName); break;
				case "Alliance": comparer = (a, b) => a.alliance.CompareTo(b.alliance); break;
			}

			if (comparer != null)
			{
			
				if (e.Column.SortDirection == null)
				{
					e.Column.SortDirection = DataGridSortDirection.Descending;
					cities.SortSmall(comparer);
					cities.NotifyReset();
				}
				else if (e.Column.SortDirection == DataGridSortDirection.Descending)
				{
					e.Column.SortDirection = DataGridSortDirection.Ascending;
					cities.SortSmall((b, a) => comparer(a, b)); // swap order of comparison
					cities.NotifyReset();
				}
				else
				{
					e.Column.SortDirection = null;

				}
			}
			// add code to handle sorting by other columns as required

			// Remove sorting indicators from other columns
			//foreach (var dgColumn in dg.Columns)
			//{
			//	if (dgColumn.Tag != null && dgColumn.Tag.ToString() != tag)
			//	{
			//		dgColumn.SortDirection = null;
			//	}
			//}
		}
		private void AttackSourceCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as City;
            if (spot.cid != 0)
            {

                City.ProcessCoordClick(spot.cid, false, App.keyModifiers,false);
            }
        }
        static bool loaded = false;

		// read only cache to enable threads to read the attacks white another thread is writing
		
		public static DateTimeOffset senTime { get => new SmallTime(plan.senTime).dateTime; set => plan.senTime = new SmallTime(value); }
		public static DateTimeOffset seTime { get => new SmallTime(plan.seTime).dateTime; set => plan.seTime = new SmallTime(value); }



		private static void SyncList(ImmutableArray<AttackPlanCity> s,DumbCollection<City> ui)
		{
			int iter = ui.Count;
			while (--iter >= 0)
			{
				if (!s.Contains(ui[iter].cid))
					ui.RemoveAt(iter);
			}
			foreach (var i in s)
			{
				var cid = i.cid;
				if (!ui.Any(b => b.cid == cid))
					ui.Add(i.city);
			}
		}
		internal static void SyncUIGrids()
		{
			SyncList(attacks, attacksUI);
			SyncList(targets, targetsUI);
		}

		internal static void WritebackAttacks()
        {
			Assert(loaded);
		//	SyncGrids();         
			instance.UpdateStats();
            BuildAttackClusters();
        }

		
		public class AttackCluster
        {
			public int id;
            public int[] attacks;// first is SE or siege
            public int[] targets;// first is real
            public Vector2 topLeft;
            public Vector2 bottomRight;
			internal int real => targets.FirstOrDefault(a => AttackPlan.GetForRead(a).isAttackTypeReal);
			//Select(a=>City.Get(a)).Where( a => a.isAttackTypeReal ).DefaultIfEmpty(City.invalid).OrderBy(a=>a.spatialIndex).First().cid;
			///            public int real => (targets.FirstOrDefault((a) => !City.GetOrAdd(a).isSiege));


		}

		public static AttackCluster[] attackClusters = Array.Empty<AttackCluster>();
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
        private void UpdateStats()
        {
            App.DispatchOnUIThreadSneakyLow(() =>
            {
               var attacks = plan.attacks.Where(a => a.attackType != AttackType.none).ToArray();
				var targets =plan.targets.Where(a => a.attackType != AttackType.none).ToArray();

				attackCount.Text = $"Attacks: {attacks.Length}";
				var seCount = attacks.Count((a) => a.attackType ==AttackType.se);
                SE.Text=$"SE: {seCount}";
				sen.Text = $"Sen: {attacks.Count((a) => a.attackType==AttackType.senator) }";
				vanqs.Text=$"Vanqs: {attacks.Count((a) => a.city.primaryTroopType==ttVanquisher)}";
                sorcs.Text=$"Sorc: {attacks.Count((a) => a.city.primaryTroopType==ttSorcerer)}";
                horses.Text=$"Horse: {attacks.Count((a) => a.city.primaryTroopType==ttHorseman)}";
                var fakes = targets.Count(a => a.isAttackTypeFake );
                var senSieges =  targets.Count(a=> a.isAttackTypeSenator );
				var seSieges = targets.Count(a => a.isAttackTypeSE );
				fakeCount.Text=$"Fake Count: {fakes}";
				fakeRatio.Text = $"Fakes Per Target: {fakes / (float)(seSieges+senSieges).Max(1):0.00}";
				senSiegeCount.Text = $"Sen Sieges: {senSieges}";
				seSiegeCount.Text = $"SE Sieges: {seSieges}";
				sePerTarget.Text = $"SE/Target: {seCount/(float)seSieges.Max(1):0.00}";
                attacksPerTarget.Text = $"Attacks/Target: {attacks.Length/(float)(senSieges+seSieges).Max(1):0.00}";
				OnPropertyChanged("");
				plan.OnPropertyChanged();


			});
        }
		static string attacksFile => "attacks" + JSClient.world.ToString();
		internal static Task SaveAttacks()
		{
			if (loaded)
			{
				return folder.SaveAsyncBackup(attacksFile, plan,lastSave);
			}
			return Task.CompletedTask;
		}
		public static async Task WaitAndSaveAttacks()
		{
			if (loaded)
			{
				using var _ = await asyncLock.LockAsync().ConfigureAwait(false);
				await SaveAttacks();
			}
		}
		public static async Task SaveAttacksBlock()
		{
			if (loaded)
			{
				using var _ = await asyncLock.LockAsync();
				await SaveAttacks();
			}
		}
		static string lastSave; 
		public async Task<IDisposable> TouchLists()
        {
			// First init
			//  AttackTab.attacks.Clear();
			var rv = await asyncLock.LockAsync();

			if (!loaded)
            {

                using var work = new ShellPage.WorkScope("load attacks");
                (plan,lastSave) = await folder.ReadAsyncForBackup<AttackPlan>(attacksFile, plan);
				// App.DispatchOnUIThreadSneaky(() =>
				//  {
				var attacks = new List<City>();
				foreach (var att in plan.attacks)
                {
                    var spot = City.GetOrAdd(att.cid);
					att.CopyTo(spot);
					attacks.Add(spot);
                }
                var targets = new List<City>();
                if (plan.targets != null)
                {
                    foreach (var target in plan.targets)
                    {
                        var t = City.GetOrAdd(target.cid);
						target.CopyTo(t);
                        targets.Add(t);

                    }
                }

				SyncUIGrids();// AttackTab.attacksUI.Set(attacks);
				//AttackTab.targetsUI.Set(targets);
                //});
                UpdateStats();
                BuildAttackClusters();
				Bindings.Update(); 
				loaded = true;
			}
			return rv;

        }

		void DoRefresh()
		{
			App.DispatchOnUIThreadSneaky(() =>
			{
				SyncUIGrids();
				attacksUI.NotifyReset();
				targetsUI.NotifyReset();
				foreach (var i in attacksUI)
					i.OnPropertyChanged(string.Empty);
				foreach (var i in targetsUI)
					i.OnPropertyChanged(string.Empty);
			});

		}
        public async override Task VisibilityChanged(bool visible)
        {
            if (visible)
            {
				App.DispatchOnUIThreadSneaky( UpdateArrivalUI );
				using var _ = await TouchLists();
				WritebackAttacks();
				DoRefresh();

			}
			else
            {

            }
        }

        private void TargetCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as City;
            City.ProcessCoordClick(spot.cid, false, App.keyModifiers,false);
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
                    var temp = new List<City>();
                    foreach (var sel in attackGrid.SelectedItems)
                    {
                        temp.Add(sel as City);
                    }
					plan.attacks = plan.attacks.RemoveAll(a => temp.Any(c => c.cid == a.cid));
                }
                if (removeTargets.IsChecked.GetValueOrDefault())
                {
                    var temp = new List<City>();
                    foreach (var sel in targetGrid.SelectedItems)
                    {
                        temp.Add(sel as City);
                    }
					plan.targets = plan.targets.RemoveAll(a => temp.Any(c => c.cid == a.cid));
                }
				SyncUIGrids();
                WritebackAttacks();
			}
        }

		//public async void AddAttacksFromSheets(object sender, RoutedEventArgs e)
  //      {
		//	try
		//	{
		//		var text = await App.GetClipboardText();
		//		await AddAttacksFromString(text);
		//	}
		//	catch (Exception ex)
  //          {
  //              LogEx(ex);
  //              Note.Show("Invalid format");

  //          }


  //      }

//		static Regex regexSen = new Regex(@"\b(?:sen|senator|academy)\b", RegexOptions.Compiled);
		static Regex regexSE = new Regex(@"\b(?:se|cat|catapult|scorp|scorpion)s?\b", RegexOptions.Compiled);
//		static Regex regexHorse = new Regex(@"\b(?:horse|knight)s?\b", RegexOptions.Compiled);
//		static Regex regexVanq1 = new Regex(@"\b(?:van|vanq|vanquisher)s?\b", RegexOptions.Compiled);
		static Regex regexVanq1 = new Regex(@"\bv\b", RegexOptions.Compiled);
		static Regex regexHorse1 = new Regex(@"\b(?:h|k)\b", RegexOptions.Compiled);
		static Regex regexSorc1 = new Regex(@"\b(?:s|m)\b", RegexOptions.Compiled);
		static Regex regexDruid = new Regex(@"\bd\b", RegexOptions.Compiled);
		static Regex regexPrae= new Regex(@"\bp\b", RegexOptions.Compiled);

		public static void FindTags(string s , out byte? troopType,out bool hasAcademy )
		{
			troopType = null;
			hasAcademy = false;
			s = s.ToLower();

			if (s.Contains("van") || s.Contains("zerk") || regexVanq1.IsMatch(s) )
				troopType = ttVanquisher;
			else if (s.Contains("hors") || s.Contains("knight") || regexHorse1.IsMatch(s) )
				troopType = ttHorseman;
			else if (s.Contains("sorc") || s.Contains("sors") || s.Contains("mage") || regexSorc1.IsMatch(s))
				troopType = ttSorcerer;
			else if (s.Contains("druid") || regexDruid.IsMatch(s) )
				troopType = ttDruid;
			else if (regexSE.IsMatch(s) )
				troopType = ttScorpion;
			else if (s.Contains("prae") || regexPrae.IsMatch(s) )
			{
				troopType = ttPraetor;
				hasAcademy = true;
			}
			else if (s.Contains("ranger"))
				troopType = ttRanger;

			if (s.Contains("acad") || s.Contains("sen"))
				hasAcademy = true;

		}

		
		public static async Task AddAttacks(List<AttackPlanCity> cities)
		{
			using var __lock = await instance.TouchLists();
			try
			{
				int prior = attacks.Length;
				var duplicates = 0;
				//int count = strs.Length;
				foreach (var atk in cities)
				{
					var cid = atk.cid;
					var city = atk.city;

					if (!atk.isAttack)
					{
						Note.Show("Warning - bad person almost added as attacker");
						continue;
					}
					if(!city.isCastle)
					{
						Note.Show("Oops not a castle");
						continue;
					}

					var isNew = !attacks.Contains(cid);
					if (isNew)
					{
						AttackPlan.AddOrUpdate(atk);
						//if(atk.attackType == AttackType.none)
						//	atk.attackType = atk.classification == Classification.se ? AttackType.se : (atk.hasAcademy.GetValueOrDefault() ? AttackType.senator : AttackType.assault);

						//attacks.Add(atk);
					}
					else
					{
						++duplicates;
					}
				}
				Note.Show($"Added {attacks.Length - prior}, updated {duplicates}");
				WritebackAttacks();
				await SaveAttacks();

			}
			catch (Exception ex)
			{
				Note.Show("Something went wrong");
				LogEx(ex);
			}
		}

		public static async Task AddAttacksFromString(string text,bool updateExisting)
		{
			using var __lock = await instance.TouchLists();

			try
			{
				var strs = text.Split(new char[] { '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
				int prior = attacks.Length;
				var duplicates = 0;
				//int count = strs.Length;
				foreach (var str in strs)
				{

					var match = AUtil.coordsRegex.Matches(str);
					if (match.Count == 0)
						continue;
					if (match.Count > 1)
					{
						Note.Show($"Invalid format:  No separator between two coordinates {str}");
						continue;
					}

					var cid = match[0].Groups[0].Captures[0].Value.FromCoordinate();
					var atk = City.Get(cid);
					
					if (!atk.IsAllyOrNap() )
					{
						Note.Show("Warning - enemy almost added as attacker");
						continue;
					}

					if ( !atk.isCastle)
					{
						Note.Show("Warning - non castle almost added as attacker");
						continue;
					}

					var isNew = !attacks.Contains(cid);
					if (!isNew)
						++duplicates;
					//if (isNew)
					 
						FindTags(str, out var troopType, out var hasAcademy);
						_ = await atk.ClassifyEx(true);
						if (troopType.HasValue )
						{
							City.TryConvertTroopTypeToClassification(troopType.Value,out atk.classification);
							
						}
						else
						{
							hasAcademy = atk.hasAcademy.GetValueOrDefault();
						}
					if (isNew || ((troopType.HasValue || atk.attackType == AttackType.none) && updateExisting))
					{
						// academy may be overriden for the purpose of attack type
						AttackPlan.AddOrUpdate(new(atk, atk.classification == City.Classification.se ? AttackType.se : (hasAcademy ? AttackType.senator : AttackType.assault),atk.primaryTroopType ));
					}
				}
				Note.Show($"Added {attacks.Length - prior}, updated {duplicates}");
				WritebackAttacks();
				await SaveAttacks();
			
			}
			catch (Exception ex)
			{
				Note.Show("Something went wrong");
				LogEx(ex);
			}
		}

		public async void AddAttacksFromClipboard(object sender, RoutedEventArgs e)
        {
			addAttackFlyout.Hide();
            try
            {
				var updateExisting = UpdateExisting.IsChecked.GetValueOrDefault();
				var text = await Clipboard.GetContent().GetTextAsync();

                if (text.IsNullOrEmpty())
                {
                    Note.Show("No clipboard text");
                    return;
                }
                await AddAttacksFromString(text,updateExisting);

            }
            catch (Exception ex)
            {
                LogEx(ex);
                Note.Show("Invalid clipboard text");
            }
        }
		static async Task<(AttackType atk,bool update)> ChooseTargetType(bool showUpdate)
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
			var panel = new StackPanel();
			if(showUpdate)
				panel.Children.Add(check);
			panel.Children.Add(combo);

				var msg = new ContentDialog()
				{
					Title = "Add Targets",
					Content = panel,
					IsPrimaryButtonEnabled = true,
					PrimaryButtonText = "Add",
					CloseButtonText = "Cancel"


				};
				if (await msg.ShowAsync2() != ContentDialogResult.Primary)
					return (AttackType.invalid,false);
				return (combo.SelectedIndex switch { 0 => AttackType.senator, 1 => AttackType.senatorFake, 2 => AttackType.se, 3 => AttackType.seFake, _=>AttackType.none },check.IsChecked.GetValueOrDefault());
		}
		static async Task<AttackType> ChooseAttackerType()
		{
			var combo = new RadioButtons() { Header = "Type" };
			combo.Items.Add("SE");
			combo.Items.Add("Senator");
			combo.Items.Add("Assault");
			combo.Items.Add("Fake SE");
			combo.Items.Add("Fake Senator");
			combo.Items.Add("Fake Assault");
			combo.Items.Add("None");
			//	combo.SelectedIndex. = SettingsPage.chooseAttackTypeUpdate;


			var msg = new ContentDialog()
			{
				Title = "Attacker Type",
				Content = combo,
				IsPrimaryButtonEnabled = true,
				PrimaryButtonText = "Change",
				CloseButtonText = "Cancel"


			};
			if (await msg.ShowAsync2() != ContentDialogResult.Primary)
				return AttackType.invalid;
			return combo.SelectedIndex switch { 0 => AttackType.se, 1 => AttackType.senator, 2 => AttackType.assault, 3 => AttackType.seFake, 4=> AttackType.senatorFake,5=> AttackType.assault, _=>AttackType.none };
		}
		
		public async void AddTargetsFromClipboard(object sender, RoutedEventArgs e)
        {
			var attackType = await ChooseTargetType(true);
			if (attackType.atk == AttackType.invalid)
				return;

			using var _ = await TouchLists();
			
			using var work = new ShellPage.WorkScope("Adding attacks...");

            int duplicates = 0;
            var reals = 0;

                //     text = text.Replace("\r", "");
            var text = await Clipboard.GetContent().GetTextAsync();

                //    var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (Match m in AUtil.coordsRegex2.Matches(text))
                {

                    try
                    {
                       
                        //  var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var cid = m.Groups[0].Value.FromCoordinate();

                        var spot = City.GetOrAdd(cid);
					if(Alliance.GetDiplomacy( spot.allianceId) == Diplomacy.allied)
					{
						Note.Show($"You are not supposed to attack your friend {Player.IdToName(spot.pid)}");
						continue;
					}
					var present = targets.Contains(spot.cid);
					if (!present || attackType.update)
					{
						await spot.Classify();
						AttackPlan.AddOrUpdate(new(spot, attackType.atk,spot.primaryTroopType));
					}
						if (present)
						{
							++duplicates;
						}
						else
						{
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

		//public static async void AddAttacks(List<int> cids)
		//{
		//	try
		//	{
		//		using var work = new ShellPage.WorkScope("Adding attacks...");

		//		using var _ = await instance.TouchLists();
		//		if( cids.All( cid=> attacks.Contains(City.Get(cid) ) ))
		//		{
		//			var rv = await App.DoYesNoBox("Already Here", "Remove?");
		//			if( rv == 1)
		//			{
		//				int count = 0;
		//				foreach (var cid in cids)
		//				{
		//					attacks.Remove(City.Get(cid));
		//					++count;
		//				}
		//				Note.Show($"Removed {count} attacks");
		//				return;

		//			}
		//			if (rv == -1)
		//				return;
		//		}


		//		int duplicates = 0;
		//		var reals = 0;
		//		var fakes = 0;
		//		//     text = text.Replace("\r", "");
		//		//    var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		//		foreach (var cid in cids)
		//		{

		//			try
		//			{
		//				//  var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		//				var atk = attacks.Find((a) => a.cid == cid);
		//				var isNew = (atk == null);
		//				if (isNew)
		//				{
		//					atk = City.GetOrAdd(cid);
		//					++reals;
		//				}
		//				else
		//				{
		//					++duplicates;
		//				}
		//				var spot = City.GetOrAdd(cid);
		//				var cl = await spot.ClassifyEx(true);
		//				//  string s = $"{cid.CidToString()} {Player.myName} {cl.} {(cl.academies == 1 ? 2 : 0)} {tsTotal}\n";

		//				if (cl == City.Classification.se)
		//					atk.attackType = AttackType.se;
		//				else if (spot.hasAcademy.GetValueOrDefault())
		//					atk.attackType = AttackType.senator;
		//				else atk.attackType = AttackType.assault;

		//				//atk.type = (int)Attack.Type.senator;

		//				//atk.fake = parts[3].ToLowerInvariant() == "fake";
		//				//var tt = parts[2].ToLowerInvariant();
		//				//if (tt.StartsWith("sorc"))
		//				//    atk.troopType = ttSorcerer;
		//				//else if (tt.StartsWith("horse"))
		//				//    atk.troopType = ttHorseman;
		//				//else if (tt.StartsWith("van"))
		//				//    atk.troopType = ttVanquisher;
		//				//else if (tt.StartsWith("druid"))
		//				//    atk.troopType = ttDruid;
		//				//else atk.troopType = ttGuard;

		//				//atk.
		//				//if (int.TryParse(parts[2], out var ts))
		//				//{
		//				//    atk.ts = ts;
		//				//}

		//				atk.attackCluster = City.attackClusterNone;


		//				if (isNew)
		//					attacks.Add(atk);
		//			}
		//			catch (Exception ex)
		//			{
		//				LogEx(ex);
		//			}
		//		}
		//		Note.Show($"{reals - duplicates} added, {duplicates} updated");
		//		WritebackAttacks();
		//		await SaveAttacks();
		//	}
		//	catch (Exception ex)
		//	{
		//		LogEx(ex);
		//	}
		//}

		//public async void AddAttacksFromString(string text)
  //      {
		//	try
		//	{
		//		List<int> cids = new();
		//		foreach (Match m in AUtil.coordsRegex.Matches(text))
		//		{

		//			try
		//			{
		//				if (m.Value.EndsWith(':') || m.Value.StartsWith(':'))
		//					continue;
		//				//  var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		//				var cid = m.Value.FromCoordinate();
		//				cids.Add(cid);

		//			}
		//			catch (Exception ex)
		//			{
		//				Log(ex);
		//			}
		//		}
		//		AddAttacks(cids);
		//	}
		//	catch(Exception ex2)
		//	{
		//		Log(ex2);

		//	}
		//}

        private void SortAttacks()
        {
			//		using var _ = await TouchLists();
			plan.attacks = plan.attacks.OrderBy(a => a.city.playerName).ThenBy(a => a.attackCluster).ToImmutableArray();
			plan.targets = plan.targets.OrderBy(a => a.city.playerName).ThenBy(a => a.attackCluster).ToImmutableArray(); 
          //  attacks.NotifyReset();
           // targets.NotifyReset();
            WritebackAttacks();
            //SaveAttacks();  
        }

        internal static async void AddTarget(IEnumerable<int> cids)
        {
			using var _ = await instance.TouchLists();

			var allHere = cids.All(c => targets.Contains(c));
			if( allHere )
			{
				var id = await App.DoYesNoBox("Already Here", "Remove?");
				if (id == 1)
				{
					int count = 0;
					foreach(var cid in cids)
					{
						var spot = City.GetOrAdd(cid);
						AttackPlan.Remove(spot);
						++count;
					}

					Note.Show($"Removed: {count} targets");

					WritebackAttacks();
					await SaveAttacks();

					return;
				}
				else if (id == -1)
					return;
			}

			var attackType = await ChooseTargetType(true);
			
			if (attackType.atk == AttackType.invalid)
				return;
			
			int added = 0, cities=0,updated = 0, wrongAlliance=0;
			foreach (var cid in cids)
			{
				var spot = City.GetOrAdd(cid);
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
				if (targets.Contains(spot))
				{
					if (attackType.update)
					{
						AttackPlan.AddOrUpdate(new(spot,attackType.atk));

					}
					++updated;
				}
				else
				{
					await spot.Classify();

					AttackPlan.AddOrUpdate(new(spot, attackType.atk));
					++added;
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
				if(t.city.pid == pid )
				{
					t.attackType = AttackType.none;
					++removed;
				}
			}
			Note.Show($"Ignoring:{removed} targets from {Player.IdToName(pid)}");
			WritebackAttacks();
			await SaveAttacks();
		}

		//private static void CleanTargets()
		//{
		//	// var set = new HashSet<int>(targets.Count);
		//	foreach (var t in targets)
		//	{
		//		t.attackCluster = City.attackClusterNone;
		//		// set.Add(t.cid);
		//	}

		//	foreach (var a in attacks)
		//	{
		//		a.attackCluster = City.attackClusterNone;
		//		City.GetOrAdd(a.cid).attackCluster = City.attackClusterNone;
		//	}
		//}

		// internal working struct
		class Cluster
		{
			public int id; // redundant
			public AttackCategory category;
			public List<AttackPlanCity> fakes;// first is SE or siege
			public AttackPlanCity real;
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
			public Span2 CalculateSpanWithout(AttackPlanCity exclude) => new Span2(fakes.Where(a=>a!=exclude).Append(real));
			public IEnumerable<AttackPlanCity> targets => fakes.Append(real);

			public void UpdateSpan() => span = CalculateSpan();
			
			// Lower is better
			// always positive
			public float CalculateAttackCost(AttackPlanCity attacker, int currentAttacks)
			{
				if (!isValid)
					return float.MaxValue;

				var score = span.Distance(attacker.cid.ToWorldC()) * plan.distancePenalty;
				score += Player.MoralePenalty(attacker.pid, real.pid) * plan.moralPenalty;
				score += currentAttacks * plan.unbalancedAssaultPenalty;

				// fakes don't matter for distance
				var troopMatch = 6;
				switch (attacker.troopType)
				{
					case ttSorcerer:
					case ttDruid:
						{
							switch (real.troopType)
							{
								case ttVanq:
									troopMatch -= 2;
									break;
								case ttRanger:
									troopMatch -= 6;
									break;
								case ttSorc:
								case ttDruid:
									troopMatch += 2;
									break;
								case ttPrae:
								case ttPriestess:
									troopMatch += 8;
									break;
								case ttHorse:
									troopMatch -= 2;
									break;
								case ttArb:
									troopMatch -= 4;
									break;
								case ttStinger:
								case ttWarship:
									troopMatch -= 4;
									break;
								default:
									break;
							}
							break;
						}
					case ttVanquisher:
					case ttRanger:
						{
							switch (real.troopType)
							{
								case ttVanq:
									troopMatch += 4;
									break;
								case ttRT:
									troopMatch += 6;
									break;
								case ttSorc:
								case ttDruid:
									troopMatch -= 0;
									break;
								case ttPraetor:
								case ttPriestess:
									troopMatch -= 5;
									break;
								case ttHorse:
									troopMatch -= 3;
									break;
								case ttArb:
									troopMatch -= 4;
									break;
								default:
									break;
							}
							break;
						}
					case ttHorseman:
					case ttArbalist:
					case ttPraetor:
						{
							switch (real.troopType)
							{
								case ttVanq:
									troopMatch += 4;
									break;
								case ttRT:
									troopMatch += 6;
									break;
								case ttSorc:
								case ttDruid:
									troopMatch -= 0;
									break;
								case ttPraetor:
									troopMatch -= 5;
									break;
								case ttPriestess:
									troopMatch -= 1;
									break;
								case ttHorse:
									troopMatch += 5;
									break;
								case ttArb:
									troopMatch += 7;
									break;
								default:
									break;
							}
							break;
						}
				}
				Assert(troopMatch >= 0);
				score += troopMatch*plan.troopMatchBonus;

				return score/attacker.priority.Max(0.125f);
			}
			public float GetTravelTimeHours(AttackPlanCity attacker)
			{
				Assert(isValid);
				// Todo: Ram attacks
				var tt = (attacker.attackType == AttackType.senator) ? ttSenator: attacker.troopType;
				var t = (float)tt.TravelTimeMinutes(attacker.cid, real.cid);
				foreach (var f in fakes)
				{
					t = t.Max((float)tt.TravelTimeMinutes(attacker.cid, f.cid));
				}
				return t /(60); // minutes to hours
			}
			public bool FakesValid()
			{
				if (real.attackType.GetCategory() != category)
					return false;
				return fakes.All(f => f.attackType.GetCategory() == category);
			}
			public bool IsInRange(AttackPlanCity attacker)
			{
				if (!isValid)
					return false;
				//if (category == AttackCategory.senator)
				//	return true;
				var tt = attacker.troopType;
				var time = GetTravelTimeHours(attacker);
				var limit = (category == AttackCategory.senator ? plan.attackMaxTravelHoursSen : plan.attackMaxTravelHoursSE);
				if (time <= limit)
					return true;
				
				return false;


			}
			public IEnumerable<AttackPlanCity> Attacks(bool wantAssaults) => attacks.Where(a => a.attackCluster == id && a.isAttackTypeAssault == wantAssaults );
			public int SiegeCount()
			{
				return attacks.Count(a => a.attackCluster == id && a.isAttackTypeSiege);
			}
			public int AttackCount(bool isAssault)
			{
				return isAssault ? attacks.Count(a => a.attackCluster == id && a.isAttackTypeAssault) : attacks.Count(a => a.attackCluster == id && a.isAttackTypeSiege);
			}

			public int AssaultsPerReal()
			{
				var reals = SiegeCount();

				return (attacks.Count(a => a.attackCluster == id && a.isAttackTypeAssault) + reals - 1) / (reals.Max(1));
			}
			public int NormalizedAttacks(bool isAssault) => (isAssault&&category==AttackCategory.se&&plan.normalizeAssaultsPerSeSiege) ? AssaultsPerReal() : AttackCount(isAssault);


		}

		private  async void AssignAttacks_Click(object sender, RoutedEventArgs e)
        {
			using var _ = await TouchLists();
			try
			{
				// just in case
				foreach (var a in AttackTab.attacks)
				{
					if (a.troopType == ttPending)
						await a.GuessTroopType();
				}
				var ignoredTargets = new HashSet<int>();// targets.Where(a=>a.attackType==AttackType.none).Select( a=>a.cid) );
														//   int[] initialClusterCount = { targets.Count(a => a.isAttackTypeReal && a.isAttackTypeSE),
														//	targets.Count(a => a.isAttackTypeReal && a.isAttackTypeSenator)};

				for (int ignoreIterator = 0; ; ++ignoreIterator)
				{
					Note.Show($"Pass {ignoreIterator}");
					// set them all, including "None"'s
					foreach (var a in AttackTab.attacks)
					{
						a.attackCluster = attackClusterNone;
					}

					var attacks = AttackTab.attacks.Where(a => a.attackType != AttackType.none).OrderBy(a => a.player.name).OrderByDescending(a => a.spatialIndex).ToArray();

					foreach (var f in targets)
					{
						f.attackCluster = attackClusterNone;
					}

					var reals = targets.Where((a) => a.isAttackTypeSiege && !ignoredTargets.Contains(a.cid)).OrderBy(a => a.spatialIndex).ToArray();
					if (!reals.Any())
					{
						Note.Show("No reals");
						return;
					}
					using var work = new ShellPage.WorkScope($"Planning.. (pass {ignoreIterator})");


					//var maxSEFakes =readable.attackSEMaxFakes;
					//var minAssaults = readable.attackSEMinAssaults;
					var clusterCount = reals.Length;
					var fakes = targets.Where((a) => a.isAttackTypeFake).OrderBy((a) => a.spatialIndex).ToArray();
					var seTargetCount = reals.Count(a => a.isAttackTypeSE);
					var senTargetCount = reals.Count(a => a.isAttackTypeSenator);
					var totalAttacks = attacks.Length;
					var seAttackCount = attacks.Count(a => a.isAttackTypeSE);
					var senAttackCount = attacks.Count(a => a.isAttackTypeSenator);
					var assaults = attacks.Count(a => a.isAttackTypeAssault);


					var clusters = new Cluster[clusterCount];

					for (var c0 = 0; c0 < clusterCount; ++c0)
					{
						var real = reals[c0];
						clusters[c0] = new Cluster() { id = c0, category = real.attackType.GetCategory(), fakes = new List<AttackPlanCity>(), real = real, span = new Span2(real.cid.ToWorldC()) };
						real.attackCluster = c0;
						// choose fakes
					}

					// Cluster to together fakes and reals
					for (AttackCategory category = 0; category < AttackCategory.count; category++)
					{
						var minFakes = category == AttackCategory.se ? plan.attackSEMinFakes : plan.attackSenMinFakes;
						var maxFakes = category == AttackCategory.se ? plan.attackSEMaxFakes : plan.attackSenMaxFakes;
						var clustersC = clusters.Where(a => a.category == category).ToArray();
						if (clustersC.Length == 0)
							continue;

						for (; ; )
						{
							var fakesC = fakes.Where(a => a.attackType.GetCategory() == category && a.attackCluster == -1).ToArray();
							if (!fakesC.Any())
								break;

							var fakes0 = clustersC.Min(a => a.fakes.Count);
							var fakes1 = clustersC.Max(a => a.fakes.Count);
							if (fakes0 >= maxFakes)
								break;
							AttackPlanCity best = null;
							float bestDist = float.MaxValue;
							Cluster bestCluster = null;

							// count 
							foreach (var c in clustersC)
							{
								var real = c.real;
								if (c.category != category)
									continue;
								var r0 = c.span.radius2;
								int fakeCount = c.fakes.Count;
								if (fakes0 < minFakes ? (fakeCount < minFakes) : (fakeCount < maxFakes))
								{
									// valid

									foreach (var fake in fakesC)
									{
										var d = (c.span + fake.cid.ToWorldC()).radius2 - r0;
										if (d < bestDist)
										{
											bestDist = d;
											bestCluster = c;
											best = fake;
										}
									}
								}
							}
							if (best == null)
								break;
							bestCluster.fakes.Add(best);
							best.attackCluster = bestCluster.id;
							bestCluster.span += best.cid.ToWorldC();

						}
						// optimize
						// first pass, shuffle the extras
						for (int iter = 0; iter < 64; ++iter)
						{
							int changes = 0;
							foreach (var cluster0 in clustersC)
							{
								if (!cluster0.isValid)
									continue;
								if (cluster0.fakes.Count <= minFakes)
									continue;
								var r0a = cluster0.span.radius2;
								AttackPlanCity best = null;
								Cluster bestCluster = null;
								float bestScore = 0;
								var count0 = cluster0.fakes.Count;
								foreach (var cluster1 in clustersC)
								{

									if (cluster1.fakes.Count >= maxFakes)
										continue;
									var r1a = cluster1.span.radius2;

									foreach (var f0 in cluster0.fakes)
									{
										Assert(f0.attackType.GetCategory() == category);
										var r0b = Span2.UnionWithout(cluster0.targets, f0).radius2;
										var r1b = new Span2(cluster1.targets.Append(f0)).radius2;

										//	var d1 = cluster1.span.Distance2(f0.cid.ToWorldC());
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
									++changes;
								}

							}

							//
							// Try swaps
							//
							//for (int iter = 0; iter < 32; ++iter)
							//{
							foreach (var cluster0 in clustersC)
							{
								if (!cluster0.isValid)
									continue;
								foreach (var cluster1 in clustersC)
								{
									if (cluster0.id >= cluster1.id)
										continue;

									if (cluster0.category != cluster1.category)
										continue;
									var span0a = cluster0.span;
									var span1a = cluster1.span;
									AttackPlanCity bestSwap0 = null;
									AttackPlanCity bestSwap1 = null;
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
										++changes;
									}

								}
							}
							Log("Fake swaps: " + changes);
							if (changes == 0)
								break;
						}
					} // for each category

					// Assign attacks to clusters

					//
					// Assign explicit attacks
					//
					/// siege, assault
					///  0 is SE, 1 is senator
					///  
					var minAttacksByType = new int[] {  seAttackCount / seTargetCount.Max(1), 1 , // siege 
														   plan.attackSEMinAssaults, plan.attackSenMinAssaults  }; // assault
					var maxAttacksByType = new[] { seAttackCount.DivideRoundUp(seTargetCount.Max(1)), // se siege
														senAttackCount.DivideRoundUp(senTargetCount.Max(1)) ,        // sen siege, todo:  Make this 1 always 
														plan.attackSEMaxAssaults,
														 plan.attackSenMaxAssaults  };
					//	Assert(minAttacksByType[1*2+ 0] == plan.attackSEMinAssaults);

					foreach (var persist in plan.attacks)
					{
						if (persist.fixedTarget != 0)
						{
							var cluster = AttackPlanCity.Get(persist.fixedTarget).attackCluster;
							persist.attackCluster = cluster;
						}
					}
					bool[] hasMin = new bool[4];
					for (; ; )
					{

						//	var minAttacks =stackalloc new int[4];
						//hasMin[2] = hasMin[3]=hasMin[1] = hasMin[0] = true;

						for (int siegeAssaultCounter = 0; siegeAssaultCounter <= 1; ++siegeAssaultCounter)
						{
							for (AttackCategory category = 0; category < AttackCategory.count; category++)
							{
								var id = siegeAssaultCounter * 2 + (int)category;
								var attacksForCategory = clusters.Where(c => c.category == category);
								//.Min(c => c.NormalizedAttacks(siegeAssaultCounter == 1));
								var atkCount = attacksForCategory.Any() ? attacksForCategory.Min(c => c.NormalizedAttacks(siegeAssaultCounter == 1)) : 0;
								//		minAttacks[id] = atkCount;
								hasMin[id] = (atkCount >= minAttacksByType[id]);
							}
						}
						// these two share
						//	hasMin[2] = hasMin[3] = hasMin[2] & hasMin[3];
						//var attacks1 = clustersC.Max(c => c.NormalizedAttacks(isAssault));

						AttackPlanCity best = null;
						float bestDist = float.MaxValue;
						Cluster bestCluster = null;

						for (AttackCategory category = 0; category < AttackCategory.count; category++)
						{
							var clustersC = clusters.Where(a => a.category == category).ToArray();
							if (clustersC.Length == 0)
								continue;
							for (int siegeAssaultCounter = 0; siegeAssaultCounter <= 1; ++siegeAssaultCounter)
							{
								var id = siegeAssaultCounter * 2 + (int)category;
								var isAssault = siegeAssaultCounter == 1;
								var isSiege = !isAssault;
								var minAtk = minAttacksByType[id];// isSiege ? 1 : category == AttackCategory.se ? plan.attackSEMinAssaults : plan.attackSenMinAssaults;
								var maxAtk = maxAttacksByType[id];// isSiege ? (category == AttackCategory.se ? 2 : 1) : category == AttackCategory.se ? plan.attackSEMaxAssaults : plan.attackSenMaxAssaults;

								{
									var attacksC = attacks.Where(a => a.attackCluster == -1 && a.isAttackTypeAssault == isAssault &&
																	(isAssault ||
																			(a.attackType.GetCategory() == category))).ToArray();
									if (!attacksC.Any())
										continue;


									// count 
									foreach (var c in clustersC)
									{
										//								var real = c.real;
										//	if (c.category != category)
										//		continue;
										var attackCount = c.NormalizedAttacks(isAssault);
										if (hasMin[id] ? (attackCount < maxAtk) : (attackCount < minAtk))
										{
											// valid

											foreach (var attack in attacksC)
											{
												if (!c.IsInRange(attack))
													continue;
												var cost = c.CalculateAttackCost(attack, attackCount);

												if (cost < bestDist)
												{
													bestDist = cost;
													bestCluster = c;
													best = attack;
												}
											}
										}
									}

								}
							}
						}
						if (best == null)
						{
							break;
						}
						//					break;
						best.attackCluster = bestCluster.id;

					}

					foreach (var atk in attacks)
					{
						if (atk.attackCluster == -1)
						{
							Note.Show($"{atk.city.nameMarkdown} has no targets in range or all targets have enough attacks of this ones type");
						}
					}


					// optimize

					// first pass, shuffle the extras
					for (int iter = 0; iter < 64; ++iter)
					{
						var changes = 0;
						foreach (var cluster0 in clusters)
						{
							if (!cluster0.isValid)
								continue;
							var category = cluster0.category;
							for (var siegeAssaultCounter = 0; siegeAssaultCounter < 2; ++siegeAssaultCounter)
							{
								var isAssault = siegeAssaultCounter == 1;
								var isSiege = !isAssault;
								var count0 = cluster0.NormalizedAttacks(isAssault);
								var id = siegeAssaultCounter * 2 + (int)category;

								if (count0 <= minAttacksByType[id])
									continue;
								var maxAttack = maxAttacksByType[id];
								AttackPlanCity best = null;
								Cluster bestCluster = null;
								float bestScore = 0;
								foreach (var cluster1 in clusters)
								{
									var count1 = cluster1.NormalizedAttacks(isAssault);

									if (count1 >= maxAttack || (isSiege && category != cluster1.category) || cluster0.id == cluster1.id)
										continue;
									foreach (var f0 in attacks)
									{
										if (f0.attackCluster != cluster0.id || (f0.isAttackTypeAssault != (isAssault)) || !cluster1.IsInRange(f0) || IsTargetFixed(f0))
											continue;
										var d0 = cluster0.CalculateAttackCost(f0, count0 - 1);
										var d1 = cluster1.CalculateAttackCost(f0, count1 + 1);
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
									++changes;
									best.attackCluster = bestCluster.id;
								}
							}
						}
						// next pass, symetric swaps
						foreach (var f0 in attacks)
						{
							//                    var s0 = AttackPlanCity.GetOrAdd(f0.cid);
							if (f0.isAttackClusterNone || IsTargetFixed(f0))
								continue;

							var c0 = f0.attackCluster;
							var attackType = f0.attackType;
							foreach (var f1 in attacks)
							{
								if (f1.isAttackClusterNone || f1.attackCluster == c0 || f1.attackType != attackType || IsTargetFixed(f1))
									continue;
								//var s1 = AttackPlanCity.GetOrAdd(f1.cid);
								var c1 = f1.attackCluster;
								if (!clusters[c0].IsInRange(f1) ||
									!clusters[c1].IsInRange(f0))
									continue;
								var isAssault = f0.isAttackTypeAssault;
								var atkCount0 = clusters[c0].NormalizedAttacks(isAssault);
								var atkCount1 = clusters[c1].NormalizedAttacks(isAssault);
								// least squares
								var d0 = clusters[c0].CalculateAttackCost(f0, atkCount0);
								var d1 = clusters[c1].CalculateAttackCost(f1, atkCount1);

								var dd0 = clusters[c0].CalculateAttackCost(f1, atkCount0);
								var dd1 = clusters[c1].CalculateAttackCost(f0, atkCount1);
								if (d0 + d1 > dd0 + dd1)
								{
									++changes;
									// a change improves things
									f1.attackCluster = c0;
									f0.attackCluster = c1;
									c0 = c1;
								}

							}

						}
						Log("Attack Swaps: " + changes);
						if (changes == 0)
							break;
					}
					// cull
					var initialCulledClusters = ignoredTargets.Count;
					{
						// cull clusters with too few attacks
						//var culled = 0;
						foreach (var cluster in clusters)
						{
							if (!cluster.isValid)
								continue;
							var culledSE = false; // todo (cluster.AttackCount(true) < plan.attackSEMinAssaults) && (cluster.category == AttackCategory.se);
							if (cluster.AttackCount(false) == 0 || culledSE)
							{
								if (cluster.AttackCount(false) == 0)
									Note.Show($"{cluster.real.nameMarkdown} {cluster.category}  culled, no reals or not in range");
								if (culledSE)
									Note.Show($"{AttackPlanCity.Get(cluster.real.cid).nameMarkdown} culled, not enough fakes in range");

								var a = ignoredTargets.Add(cluster.real.cid);
								Assert(a == true);

								foreach (var f0 in attacks)
								{
									if (f0.attackCluster == cluster.id)
									{
										f0.attackCluster = attackClusterNone;
									}
								}
								cluster.real.attackCluster = attackClusterNone;
								foreach (var t in cluster.fakes)
								{
									t.attackCluster = attackClusterNone;
								}
								cluster.SetInvalid();
							}
						}
					}

					if (!clusters.Any(a => a.isValid))
					{
						Note.Show("No valid attack clusters (not enough attacks or out of range?)");
						return;
					}
					if (ignoredTargets.Count > initialCulledClusters && ignoreIterator < 16)
					{
						continue;
					}

					WritebackAttacks();

					//         var targetCluster = new List<int>();
					//           targetCluster.Add(real.cid);
					//            targetCluster.Add(bestFake);
					{
						var seAttacks = attacks.Where((a) => a.isAttackTypeSE && !a.isAttackClusterNone);
						var assaultAttacks = attacks.Where((a) => a.isAttackTypeAssault && !a.isAttackClusterNone);
						var senatorAttacks = attacks.Where((a) => a.isAttackTypeSenator && !a.isAttackClusterNone);
						float maxDistanceToSE = seAttacks.Any() ? seAttacks.Max((a) => clusters[a.attackCluster].GetTravelTimeHours(a)) : 0;
						float averageDistanceToSE = seAttacks.Any() ? seAttacks.Average((a) => clusters[a.attackCluster].GetTravelTimeHours(a)) : 0;

						float maxDistanceToAssault = assaultAttacks.Any() ? assaultAttacks.Max((a) => clusters[a.attackCluster].GetTravelTimeHours(a)) : 0;
						float averageDistanceToAssault = assaultAttacks.Any() ? assaultAttacks.Average((a) => clusters[a.attackCluster].GetTravelTimeHours(a)) : 0;

						float maxDistanceToSenator = senatorAttacks.Any() ? senatorAttacks.Max((a) => clusters[a.attackCluster].GetTravelTimeHours(a)) : 0;
						float averageDistanceToSenator = senatorAttacks.Any() ? senatorAttacks.Average((a) => clusters[a.attackCluster].GetTravelTimeHours(a)) : 0;

						float maxClusterSize = clusters.Where(c => c.isValid).Max((a) => a.span.radius2.Sqrt());
						float averageClusterSize = clusters.Where(c => c.isValid).Average((a) => a.span.radius2.Sqrt());

						//           bad = $"{bad} Assigned {reals} reals and {fakes} fakes\nUnused: reals {unusedReals}, fakes {unusedFakes}";

						Note.Show($"Attack plan done, {ignoredTargets.Count} culled real targets, {attacks.Count(a => a.isAttackClusterNone)} culled attacks, {clusterCount} valid targets, SE Distance max: {maxDistanceToSE} av: {averageDistanceToSE}, Senator Distance max:{maxDistanceToSenator} av:{averageDistanceToSenator} Assault distance max:{maxDistanceToAssault} av:{averageDistanceToAssault} Cluster size max: {maxClusterSize} av: {averageClusterSize}");

						for (int j = 0; j < 2; ++j)
						{
							foreach (var a in (j == 0 ? senatorAttacks : seAttacks))
							{
								var d = clusters[a.attackCluster].GetTravelTimeHours(a);
								if (d >= (j == 0 ? maxDistanceToSenator : maxDistanceToSE) - 3.0f)
								{
									Note.Show($"{a.nameMarkdown} leaves at around {((j == 0 ? senTime : seTime) - TimeSpan.FromHours(d)).FormatDefault()}");
								}
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
			} catch (Exception ex)
			{
				LogEx(ex);
			}
        }

        private async void AttackRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
			using var _ = await TouchLists();

			var i = sender as FrameworkElement;

            var spot = i.DataContext as City;
            Remove(spot.cid);
            WritebackAttacks();
            await SaveAttacks();
        }

        private async void ArrivalSETapped(object sender, RoutedEventArgs e)
        {
            (var dateTime, var okay) = await DateTimePicker.ShowAsync("Send SE At", seTime);
            if (okay)
            {
				seTime = dateTime;
				UpdateArrivalUI();
			}
        }
		private async void ArrivalSenTapped(object sender, RoutedEventArgs e)
		{
			(var dateTime, var okay) = await DateTimePicker.ShowAsync("Send Sen At", senTime);
			if (okay)
			{
				senTime = dateTime;
				UpdateArrivalUI();
			}
		}

		//private void ClearTargets_Click(object sender, RoutedEventArgs e)
  //      {
  //          CleanTargets();
  //          attacks.NotifyReset();
		//	attackClusters= Array.Empty<AttackCluster>();
  //      }

		private void SelectTargets(object sender, RoutedEventArgs e)
		{
			var sel = targetGrid.SelectedItems;
			foreach (var i in targets)
			{
				var city = i.city;
				if (!sel.Contains(city))
					sel.Add(city);
				city.SelectMe(scrollIntoView: false);
			}
		}
		private void SelectAttackers(object sender, RoutedEventArgs e)
		{
			var sel = attackGrid.SelectedItems;
			foreach (var i in attacks)
			{
				var city = i.city;
				if (!sel.Contains(city))
					sel.Add(city);
				city.SelectMe(scrollIntoView:false);
			}
		}

		private async void SetAttackerType(object sender, RoutedEventArgs e)
		{
			var type = await ChooseAttackerType();
			if (type == AttackType.invalid)
				return;
			using var _ = await TouchLists();
			int counter = 0;
			await App.DispatchOnUIThreadTask(()=>
			{
				foreach (City sel in attackGrid.SelectedItems)
				{
					++counter;
					var cid = sel.cid;
					var atk = AttackPlan.Get(cid);
					atk.attackType = type;
					AddOrUpdate(atk);
					//sel.OnPropertyChanged();
				}
				return Task.CompletedTask;
			});
			WritebackAttacks();
			await SaveAttacks();
			attacksUI.NotifyReset();
			OnPropertyChanged();
			Note.Show($"Set {counter} to {type}");
		}

		private async void SetTargetType(object sender, RoutedEventArgs e)
		{
			var type = (await ChooseTargetType(false)).atk;
			if (type == AttackType.invalid)
				return;
			using var _ = await TouchLists();
			int counter = 0;
			await App.DispatchOnUIThreadTask(() =>
			{
				foreach (City sel in targetGrid.SelectedItems)
				{
					++counter;
					var cid = sel.cid;
					var atk = AttackPlan.Get(cid);
					atk.attackType = type; 
					AddOrUpdate(atk);
					sel.OnPropertyChanged();
				}
				return Task.CompletedTask;
			});
			WritebackAttacks();
			await SaveAttacks();
			attacksUI.NotifyReset();
			OnPropertyChanged();
			Note.Show($"Set {counter} to {type}");

		}

		private async void SetAttack_Click(object sender, RoutedEventArgs e)
			{
				using var rv = await TouchLists();
				var selAtk = attackGrid.SelectedItems;
				if (selAtk.Count == 0)
				{
					Note.Show("Please select at least 1 attacker and an optional target");
					return;
				}
				var selTargets = targetGrid.SelectedItems;
				if (selTargets.Count > 1)
				{
					Note.Show("Please select 1 target");
					return;
				}
				var targetCid = selTargets.Count > 0 ? (selTargets[0] as City).cid : 0;
				await App.DispatchOnUIThreadTask(() =>
			   {
				   foreach (City atk in selAtk)
				   {
					   var per = AttackPlan.Get(atk);
					   if (per != null)
					   {
						   per.fixedTarget = targetCid;
						   per.city.OnPropertyChanged();
						   if (targetCid == 0)
							   Note.Show($"Target cleared for {AttackPlanCity.Get(per.cid).nameMarkdown}");
						   else
							   Note.Show($"{AttackPlanCity.Get(per.cid).nameMarkdown} set to attack {AttackPlanCity.Get(targetCid).nameMarkdown}");
					   }
					   else
					   {
						   Assert(false);
					   }
				   }
				   return Task.CompletedTask;
			   });
				WritebackAttacks();
				await SaveAttacks();
				//attacks.NotifyReset();
				Note.Show($"Update attack to see results");
			}
			
		private void Player_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var i = sender as FrameworkElement;
			var city = i.DataContext as City;
			JSClient.view.InvokeScriptAsync("sendmail", new string[] { city.playerName, "test", playerCommands[city.pid].Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "&#10;") });

		}
	}
}
