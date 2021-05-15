using COTG.Helpers;
using COTG.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using COTG.Views;

using static COTG.Debug;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.System;
//using Windows.UI.Xaml.Controls;
using COTG.JSON;
using System.Text.Json;
using Windows.Web.Http;
using static COTG.Game.Enum;
using Windows.ApplicationModel.DataTransfer;
using static COTG.Views.ShellPage;
using ContentDialog = Windows.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Windows.UI.Xaml.Controls.ContentDialogResult;
using MenuFlyoutItem = Windows.UI.Xaml.Controls.MenuFlyoutItem;
using MenuFlyout = Windows.UI.Xaml.Controls.MenuFlyout;
using ToggleMenuFlyoutItem = Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem;
using MenuFlyoutSubItem = Windows.UI.Xaml.Controls.MenuFlyoutSubItem;
using System.Collections.ObjectModel;

namespace COTG.Game
{
	//public interface IKeyedItem
	//{
	//  public  int GetKey();
	//  public  void Ctor(int id);
	//}
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public abstract class Spot : IEquatable<Spot>, INotifyPropertyChanged
	{
		public static ConcurrentDictionary<int, City> allSpots = new ConcurrentDictionary<int, City>(); // keyed by cid
		public static HashSet<int> selected = new HashSet<int>();
		public static City[] defendersI = Array.Empty<City>();
		public static City[] defendersO = Array.Empty<City>();

		// indexed by PackedContinentId
		public const ulong continentFilterAll = 0xfffffffffffffffful;
		public static ulong continentFilter = continentFilterAll;
		public static ulong ContinentFilterFlag(int id) => 1ul << id;
		public static bool isContinentFilterAll => continentFilter == continentFilterAll;
		public static bool TestContinentFilterPacked(int id) => (continentFilter& ContinentFilterFlag(id))!=0;
		public static void SetContinentFilterPacked(int id, bool set) => continentFilter = (continentFilter&(~ContinentFilterFlag(id))) | ( set ? ContinentFilterFlag(id) : 0ul );
		public bool testContinentFilter => TestContinentFilter(cid);
		public static bool TestContinentFilter(int cid) => (continentFilter & ContinentFilterFlag(World.CidToPackedContinent(cid))) != 0;

		public static bool TryGet(int cid, out City spot) => allSpots.TryGetValue(cid, out spot);
		public static int focus; // city that has focus (selected, but not necessarily building.  IF you click a city once, it goes to this state

		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		
		public bool isFriend => Player.IsFriend(pid); // this is set if it is one of our cities or our ally cities that we can visit

		internal static City GetFocus()
		{
			return focus == 0 ? null : GetOrAdd(focus);
		}

		public static City invalid = new City() { _cityName = "?" };
		//public static City pending = new City() { _cityName = "pending" };

		//public static City[] emptySpotSource = new[] { pending };

		public string nameAndRemarks => remarks.IsNullOrEmpty() ? cityName : $"{cityName} - {remarks}";
		public string remarks { get; set; } = string.Empty; // only for city
		public string notes { get; set; } = string.Empty; // only for city

		public static bool IsFocus(int cid)
		{
			return focus == cid;
		}
		public static City GetOrAdd(int cid, string cityName = null)
		{
			if (cid <= 0)
			{
				return invalid;
			}
			if (!Spot.TryGet(cid, out var rv))
			{
				var worldC = cid.CidToWorld();
				var info = World.GetInfo(worldC);
				//    Assert(info.type == World.typeCity);
				

				rv = new City() { cid = cid, pid = info.player };
				
				//       Assert( info.player != 0);
				rv.type = (byte)(info.type);
				if (info.type == 0)
				{
					Log("Uninitialized");
				}
				rv.isTemple = info.isTemple;
				rv.isOnWater = info.isWater;
				rv.isCastle = info.isCastle;
				rv.points = (ushort)(info.isBig ? 9000 : 2500);

				Spot.allSpots.TryAdd(cid, rv);
				if (Player.IsFriend(info.player))
					City.CitiesChanged();
				
			}
			if (cityName != null)
				rv._cityName = cityName;
			return (rv);
		}
		public static void UpdateName(int cid, string name) => GetOrAdd(cid, name);

		internal bool HasTag(Tags tag) => tag.IsSet(remarks);

		public bool isMine => pid == Player.myId;

		public string _cityName;
		public string cityName => _cityName ?? xy;

		public int cid; // x,y combined into 1 number
		public string xy => cid.CidToString();//$"({cid % 65536}:{cid / 65536})";
		public int x => cid % 65536;
		public int y => cid >> 16;



		public int _tsHome; // cache for when tsHome is missing
		public int _tsTotal;
		public int tsRaid => (troopsHome.Any() ? troopsHome.TSRaid() : _tsHome);
		public int tsHome => (troopsHome.Any() ? troopsHome.TS() : _tsHome);
		public int tsDefHome => (troopsHome.Any() ? troopsHome.TSDef() : _tsHome);
		public int tsDefTotal => (troopsTotal.Any() ? troopsTotal.TSDef() : _tsTotal);
		public float raidIdle => troopsHome.TSRaid()/ (float)troopsTotal.TSRaid().Max(1);

		public int tsTotal => (troopsTotal.Any() ? troopsTotal.TS() : _tsTotal);
		public int tsRaidTotal => (troopsTotal.Any() ? troopsTotal.TSRaid() : _tsTotal);
		public int tsDefMax 
		{ 
			get 
			{ 
				if(incoming.Any())
				{
					return incomingDefTS;
				}
				return reinforcementsIn.TS() + (troopsHome.Any()?troopsHome.TSDef() :_tsHome); 
			} 
		}
		public int tsOff { get { var i = incomingOffTS; return (i > 0) ? i : troopsHome.TSOff(); } }

		public Reinforcement[] reinforcementsIn = Array.Empty<Reinforcement>();
		public Reinforcement[] reinforcementsOut = Array.Empty<Reinforcement>();


		public int pid { get; set; }
		public string playerName => Player.Get(pid).name;
		public Player player => Player.Get(pid);
		public string alliance => Player.Get(pid).allianceName; // todo:  this should be an into alliance id
		public ushort allianceId => Player.Get(pid).alliance; // todo:  this should be an into alliance id
		public byte type;
		public const byte typeCity = 0x1;
		public const byte typeShrine = 0x2;
		public const byte typePortal = 0x3;
		public const byte typeBoss = 0x4;
		public const byte typeDungeon = 0x5;
		public const byte typeNone = 0x0;
		// Water and obscurred terriain are not defined :(
		public bool isCityOrCastle => type == typeCity;
		public bool isBoss => type == typeBoss;
		public bool isDungeon => type == typeDungeon;
		public bool isEmpty => type == typeNone;

		//public DateTimeOffset lastAccessed { get; set; } // lass user access
		public int attackCluster { get; set; } = -1; // For attackTab, 


		public AttackType attackType { get; set; }
		public bool isAttackTypeAssault => attackType == AttackType.assault;
		public bool isAttackTypeSenator => attackType == AttackType.senator;
		public bool isAttackTypeSenatorFake => attackType == AttackType.senatorFake;
		public bool isAttackTypeSE => attackType == AttackType.se;
		public bool isAttackTypeSEFake => attackType == AttackType.seFake;
		public bool isAttackTypeReal => attackType == AttackType.assault || attackType == AttackType.se || attackType == AttackType.senator;
		public bool isAttackTypeSiege => attackType == AttackType.se || attackType == AttackType.senator;
		public bool isAttackTypeFake => attackType == AttackType.seFake || attackType == AttackType.senatorFake;
		public bool isAttackTypeNone => attackType == AttackType.none;

		public const int attackClusterNone = -1;


		public bool isAttackClusterNone => attackCluster == attackClusterNone;

		public enum Classification : byte
		{
			unknown,
			vanqs,
			rt,
			sorcs,
			druids,
			praetor,
			priestess,
			horses,
			arbs,
			se,
			hub,
			navy, // warships
			stingers,
			misc,
			pending,
			missing
		}
		public struct ClassificationExtended
		{
			public Classification classification;
			public byte stables;
			public byte academies;
			public byte training;
			public byte sorc;
			public byte se;
			public byte shipyards;
			public byte ports;
			public byte forums;
			public bool castle;
			static public implicit operator Classification(ClassificationExtended e) => e.classification;
			static public implicit operator ClassificationExtended(Classification e)
			{
				var rv = new ClassificationExtended();
				rv.classification = e;
				return rv;
			}
		};
		public Classification classification { get; set; }
		public sbyte academyInfo = -1;
		public bool ? hasAcademy => academyInfo == 0 ? false : academyInfo==1 ? true : null;
		public bool isNotClassified => classification == Classification.unknown; // does not include pending
		public bool isClassified => classification != Classification.unknown && classification != Classification.pending;
		public bool isPending => classification == Classification.pending;
		private static string[] classifications =
		{
			"unknown",
			"vanqs",
			"rt",
			"sorcs",
			"druids",
			"praetor",
			"priestess",
			"horses",
			"arbs",
			"scorps",
			"hub",
			"navy",
			"stingers",
			"misc",
			"pending",
			"no intel"
		};
		private static int[] classificationTTs =
		{
			ttGuard,
			ttVanquisher,
			ttRanger,
			ttSorcerer,
			ttDruid,
			ttPraetor,
			ttPriestess,
			ttHorseman,
			ttArbalist,
			ttScorpion,
			ttSenator,
			ttWarship,
			ttStinger,
			ttBallista,

			ttPending,
			ttPending
		};
		public string classificationString => classifications[(int)classification];
		public int classificationTT => classificationTTs[(int)classification];
		public string attackers
		{
			get
			{
				var rv = new HashSet<string>();

				foreach (var atk in incoming)
				{
					if (atk.isAttack)
					{
						rv.Add(atk.sPlayer);
					}
				}
				if (rv.IsNullOrEmpty())
					return "None";
				else
					return string.Join(',', rv);
			}
		}
		public string firstIncoming
		{
			get
			{


				foreach (var atk in incoming)
				{
					if (atk.isAttack)
					{
						return atk.time.FormatDefault();
					}
				}
				return "??";
			}
		}

		public bool isCastle { get; set; }
		public bool isOnWater { get; set; }
		public bool isTemple { get; set; }

		public bool pinned { get; set; }
		public void SetPinned(bool _pinned)
		{
			if (_pinned == pinned)
				return;
			pinned = _pinned;
			if (_pinned)
				SettingsPage.pinned = SettingsPage.pinned.ArrayAppendIfAbsent(cid);
			else
				SettingsPage.pinned = SettingsPage.pinned.Where(a => a != cid).ToArray();
			App.DispatchOnUIThreadSneakyLow(() => OnPropertyChanged(nameof(pinned)));
		}  // pinned in MRU
		public byte claim; // only if this is under attack
		public byte shipyards { get; set; }
		public byte ports { get; set; }
		public string Claim => $"{(int)claim:00}%";
		public bool isBlessed { get; set; }
		public float scoutRange { get; set; }
		public ushort points { get; set; }
		public BitmapImage icon => ImageHelper.FromImages(isBlessed ? "Icons/blessed.png" :
			 ($"{(isTemple ? "Icons/temple" : isCastle ? "Icons/castle" : "Icons/city")}{GetSize()}{(isOnWater ? "w" : "")}.png"));
		public int cont => cid.CidToContinent();

		public static bool operator ==(Spot left, Spot right)
		{
			return EqualityComparer<Spot>.Default.Equals(left, right);
		}

		public static bool operator !=(Spot left, Spot right)
		{
			return !(left == right);
		}

		public void ExportToDefenseSheet()
		{
			using var work = new WorkScope("Export Def..");
			var sb = new StringBuilder();
			foreach (var _cid in GetSelectedForContextMenu(cid, false))
			{
				var s = Spot.GetOrAdd(_cid);
				if (s.incoming.Any())
				{
					sb.Append(s.playerName);
					sb.Append('\t');
					sb.Append(s.cont);
					sb.Append('\t');
					sb.Append(s.cid.CidToCoords());
					sb.Append('\t');
					sb.Append(s.incoming.Where(x => x.isAttack).First()?.time.FormatSkipDateIfToday() ?? "??");
					sb.Append('\t');
					sb.Append(s.incTT);
					sb.Append('\t');
					sb.Append("1000000");
					sb.Append('\t');
					var ts = s.tsDefMax;
					sb.Append(ts);
					sb.Append('\n');
					sb.Append(incomingAttacks);
					sb.Append('\n');
				}
			}
			App.CopyTextToClipboard(sb.ToString());

			Note.Show("Exported to clipboard for sheets");
		}

		// The UIElement returned will be the RadDataGrid
		public static (Spot spot, DataGridCellInfo column, PointerPoint pt, UIElement uie) HitTest(object sender, PointerRoutedEventArgs e)
		{
			e.KeyModifiers.UpdateKeyModifiers();
			var grid = sender as RadDataGrid;
			var physicalPoint = e.GetCurrentPoint(grid);
			var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
			var cell = grid.HitTestService.CellInfoFromPoint(point);
			//var row = grid.HitTestService.RowItemFromPoint(point);
			//if(cell?.Item != row )
			//{
			//    Note.Show($"{cell} {row} {cell?.Column}");

			//}

			var spot = point.Y > 34 ? (cell?.Item as Spot) : null; // workaround for clicking on the header
			ClearHover();
			viewHover = spot != null ? spot.cid : 0;
			Player.viewHover = spot != null ? spot.pid : 0;


			return (spot, cell, physicalPoint, grid);
		}




		public static void GridPressed(object sender, PointerRoutedEventArgs e)
		{
			(sender as RadDataGrid).Focus();
			e.KeyModifiers.UpdateKeyModifiers();

			var hit = Spot.HitTest(sender, e);
			var spot = hit.spot;
			//	uiPress = spot != null ? spot.cid : 0;
			uiPressColumn = hit.column.CellText() ;
			// The UIElement returned will be the RadDataGrid
			if (spot != null)
				spot.ProcessClick(uiPressColumn, hit.pt, hit.uie, e.KeyModifiers);
		}
		public static void ProcessPointerExited()
		{
			ClearHover();
		}

		public static void GridExited(object sender, PointerRoutedEventArgs e)
		{
			ClearHover();
		}

		public static void ClearHover()
		{
			ShellPage.ClearHover();
		}

		public byte primaryTroopType => GetPrimaryTroopType();
		public byte GetPrimaryTroopType(bool onlyHomeTroops = false, bool includeWater=true)
		{
			var troops= (onlyHomeTroops ? troopsHome : troopsTotal);
			if(troops.IsNullOrEmpty())
				return (byte)classificationTT;
			
			byte best = 0; // if no raiding troops we return guards 
			var bestTS = 0;
			foreach (var ttc in troops)
			{
				var type = ttc.type;
				if (IsTTNaval(type) && !includeWater)
					continue;
				var ts = ttc.ts;
				if (ts > bestTS)
				{
					bestTS = ts;
					best = (byte)type;
				}

			}
			if (best == 0)
				return (byte)classificationTT;
			else
				return best;
		}


		public async void ProcessClick(string column, PointerPoint pt, UIElement uie, VirtualKeyModifiers modifiers)
		{
			modifiers.UpdateKeyModifiers();
			//    Note.Show($"{this} {column} {pt.Position}");

			if (pt.Properties.IsLeftButtonPressed && !(modifiers.IsShiftOrControl())) // ignore selection style clicks
			{


				// If we are already selected and we get clicked, there will be no selection chagne to raids are not scanned automatically
				//	var wantRaidingScan = (City.CanVisit(cid) && MainPage.IsVisible());
				var wantRaidScan = isFocus;
				//                var needCityData = 
				var wantSelect = true;
				var wantClick = false;
				switch (column)
				{
					case nameof(nameAndRemarks):
						// first click selects
						// second acts as coord click
						if (IsSelected(cid))
						{
							ProcessCoordClick(cid, false, modifiers, false);
							wantRaidScan = false;
						}
						break;
					case nameof(City.bStage):
						DoTheStuff();
						break;
					case nameof(xy):
						ProcessCoordClick(cid, false, modifiers, false);
						wantRaidScan = false;
						break;
					case nameof(icon):
						if (City.CanVisit(cid))
						{
							
							var wasBuild = City.IsBuild(cid);

							if (!await JSClient.CitySwitch(cid, false, true, false))
								return;

							if (wasBuild)
							{
								JSClient.ChangeView(ShellPage.viewMode.GetNext() );

							}

						}
						else
						{
							JSClient.ShowCity(cid, false, true, false);
						}
						wantSelect = false;
						wantRaidScan = false;
						break;
					case nameof(City.dungeonsToggle):
						{
							ShowDungeons();
							wantRaidScan = false;
							wantSelect = false;
							break;
						}
					case nameof(City.tsTotal):
						if (City.CanVisit(cid) && MainPage.IsVisible())
						{
							Raiding.UpdateTS(true, true);
						}

						wantRaidScan = false;
						break;
					case nameof(tsHome):
					case nameof(tsRaid):
						if (City.CanVisit(cid) && MainPage.IsVisible())
						{
							Raiding.UpdateTS(true, true);
						}
						wantRaidScan = false;
						break;
					case nameof(City.raidReturn):
						if (City.CanVisit(cid) && MainPage.IsVisible())
						{
							Raiding.ReturnFast(cid, true);
						}
						wantRaidScan = false;
						break;
					case nameof(pinned):
						var newSetting = !pinned;

						SetPinned(newSetting);

						return;
					case nameof(City.AutoWalls):
						(this as City).AutoWalls = !(this as City).autoWalls;
						return;
					case nameof(City.AutoTowers):
						(this as City).AutoTowers = !(this as City).autoTowers;
						return;
					case nameof(City.raidCarry):
						if (City.CanVisit(cid) && MainPage.IsVisible())
						{
							Raiding.ReturnSlow(cid, true);
						}
						wantRaidScan = false;
						break;
					default:
						wantRaidScan = true;
						break;
				}


				//if (MainPage.IsVisible() && isMine && wantRaidScan)
				//{
				//	//                MainPage.SetRaidCity(cid,true);
				//	ScanDungeons.Post(cid, true, false);
				//}
				if (wantSelect)
					SetFocus(false,true,true,false);
				NavStack.Push(cid);

			}
			else if (pt.Properties.IsRightButtonPressed)
			{
				if (!modifiers.IsShiftOrControl())
					SetFocus(false, true, true);
				ShowContextMenu(uie, pt.Position);

			}
			else if (pt.Properties.IsMiddleButtonPressed)
			{
				var text = ToTsv();
				Note.Show($"Copied to clipboard: {text}");
				App.CopyTextToClipboard(text);
				SetFocus(false);
			}
			else
			{
				// if shift or conntrol is pressed normal processing takes place
			}
			SpotTab.TouchSpot(cid, modifiers);
		}
	
		public async Task ShowDungeons()
		{
			await ScanDungeons.Post(cid, true, false);
		}

		public string ToTsv()
		{
			return $"{cid.CidToCoords()}\t{this.playerName}\t{this.cityName}\t{this.remarks ?? ""}\t{this.alliance}\t{this.isCastle}\t{this.isOnWater}";
		}

		public static async void ProcessCoordClick(int cid, bool lazyMove, VirtualKeyModifiers mod, bool scrollIntoUI = false)
		{
			mod.UpdateKeyModifiers();

			if (City.CanVisit(cid) && !mod.IsShiftOrControl())
			{
				if (City.IsBuild(cid))
				{
					JSClient.ChangeView(ShellPage.viewMode.GetNext() );// toggle between city/region view
					if (scrollIntoUI)
					{
						Spot.SetFocus(cid, scrollIntoUI, true, true, lazyMove);
					}
					else
					{
						cid.BringCidIntoWorldView(lazyMove, false);
					}
				}
				else
				{
					await JSClient.CitySwitch(cid, lazyMove, false, scrollIntoUI); // keep current view, switch to city
				//	JSClient.ChangeView(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
				}
				NavStack.Push(cid);

			}
			else
			{
				JSClient.ShowCity(cid, lazyMove, false, scrollIntoUI);
				NavStack.Push(cid);
			}
			//Spot.GetOrAdd(cid).SelectMe(false,mod);
			SpotTab.TouchSpot(cid, mod, true);

			if (mod.IsShiftAndControl() && ( Player.isAvatarOrTest|| City.CanVisit(cid) ) )
			{
				//     var spot = Spot.GetOrAdd(cid);
				//     GetCity.Post(cid, spot.pid, (js, city) => Log(js));

				var str = await Post.SendForText("includes/gLay.php", $"cid={cid}", World.CidToPlayerOrMe(cid));
				Log(str);

				App.DispatchOnUIThreadSneaky(() =>
				{
						// set is water var
						str = $"{City.shareStringStart}{(World.GetInfoFromCid(cid).isWater ? ';' : ':')}{str.Substring(18)}";
					App.CopyTextToClipboard(str);

					Launcher.LaunchUriAsync(new Uri($"https://cotgopt.com/?map={str}"));
				});
			}
		}
		internal async Task<Classification> ClassifyIfNeeded()
		{
			if (isClassified)
			{
				return classification;
			}
			else if (classification == Classification.pending)
			{
				for (; ; )
				{
					await Task.Delay(300);
					if (classification != Classification.pending)
						return classification;
				}
			}
			else
			{
				return (await Classify()) ;
			}
		}

		internal void QueueClassify(bool isIncomingAttack)
		{
			if (isNotClassified)
			{
				Classify(isIncomingAttack);
			}
		}
		
		Classification TagsToClassification()
		{
			if (HasTag(Tags.Vanq))
				return Classification.vanqs;
			else if (HasTag(Tags.Scorp))
				return Classification.se;
			else if (HasTag(Tags.Prae))
				return Classification.praetor;
			else if (HasTag(Tags.Priest))
				return Classification.priestess;
			else if (HasTag(Tags.RT) || HasTag(Tags.VT) || HasTag(Tags.VRT))
				return Classification.rt;
			else if (HasTag(Tags.Sorc))
				return Classification.sorcs;
			else if (HasTag(Tags.Druid))
				return Classification.druids;
			else if (HasTag(Tags.Horse))
				return Classification.horses;
			else if (HasTag(Tags.Arb))
				return Classification.arbs;
			else if ( HasTag(Tags.Stinger))
				return Classification.stingers;
			else if (HasTag(Tags.Galley) || HasTag(Tags.Stinger) || HasTag(Tags.Warship))
				return Classification.navy;
			else if (HasTag(Tags.Hub))
				return Classification.hub;
			return Classification.unknown;
		}

		public async ValueTask<Classification> ClassifyEx(bool isIncomingAttack)
		{
			var rv = await Classify(isIncomingAttack);
			if (hasAcademy.HasValue)
				return rv;
			return await ClassifyFromBuildings(isIncomingAttack);
		}

		private async ValueTask<Classification> ClassifyFromBuildings(bool isIncomingAttack)
		{
			var str = await Post.SendForText("includes/gLay.php", $"cid={cid}", World.CidToPlayerOrMe(cid));


		 byte stables=0;
		 byte academies = 0;
		 byte training = 0;
		 byte sorc = 0;
		 byte se = 0;
		byte shipyards = 0;
		byte ports = 0;
		 byte forums = 0;
			bool castle = false;
			var classification = Classification.unknown;

			try
			{
				const int start = 14;
				const int end = 459;

				for (int i = start; i < end; ++i)
				{
					switch (str[i])
					{
						case 'E': ++stables; break;
						case 'Y': ++se; break;
						case 'J': ++sorc; break;
						case 'G': ++training; break;
						case 'Z': ++academies; break;
						case 'X': castle = true; break;
						case 'R': ++ports; break;
						case 'V': ++shipyards; break;
						case 'P': ++forums; break;


					}
				}
				academyInfo = academies > 0 ? (sbyte)1 : (sbyte)0;
				var mx = stables.Max(academies).Max(training.Max(sorc)).Max(academies.Max(training)).Max(se).Max(shipyards).Max(forums).Max(ports);
				if (mx <= 4)
				{
					classification = Classification.misc;
				}
				else if (mx == stables)
				{
					if (se > 0 || academies > 0 || (stables < 32) || isIncomingAttack)
						classification = Classification.horses;
					else
					{
						classification = Classification.arbs;
					}
				}
				else if (mx == sorc)
				{
					if (sorc == 45 || sorc == 40 || sorc == 29 || sorc == 27)
						classification = Classification.druids;
					else
						classification = Classification.sorcs;
				}
				else if (mx == training)
				{
					if (se > 0 || academies > 0 || (training < 28 && training >= 26) || (training < 22) || isIncomingAttack)
						classification = Classification.vanqs;
					else
						classification = Classification.rt;

				}
				else if (mx == academies)
				{
					classification = Classification.praetor; // todo!
				}
				else if (mx == se)
				{
					classification = Classification.se;
				}
				else if (mx == shipyards)
				{
					classification = Classification.navy;
				}
				else if (mx == forums || mx == ports)
				{
					classification = Classification.hub;
				}
				else
				{
					classification = Classification.misc;
				}

			}
			catch (Exception e)
			{
				LogEx(e);
			}
			if (this.classification == Classification.pending || this.classification == Classification.unknown)
				this.classification = classification;
			
			return (this.classification);
		}

		internal async ValueTask<Classification> Classify(bool isIncomingAttack=false)
		{
			if (isFriend)
			{
				classification = TagsToClassification();
				if (classification != Classification.unknown)
					return classification;
				
				foreach(var tt in troopsTotal.OrderByDescending(a=>a.count))
				{
					var i = classificationTTs.FindIndex((byte)tt.type);
					if (i != -1)
					{
						classification = (Classification)i;
						return classification;
					}
				}
			}
			if (!Player.isAvatarOrTest && !Alliance.wantsIntel)
			{
				classification = Classification.missing;
				return classification;
			}

			//ClassificationExtended rv = new ClassificationExtended(); ;
			if(classification == Classification.unknown)
				classification = Classification.pending;

			return (await ClassifyFromBuildings(isIncomingAttack));

		}


		// Incoming attacks
		public Army[] incoming { get; set; } = Army.empty;
		public TroopTypeCount[] troopsHome = TroopTypeCount.empty;
		public TroopTypeCount[] troopsTotal = TroopTypeCount.empty;
		public int incomingAttacks
		{
			get
			{
				var rv = 0;
				foreach (var a in incoming)
				{
					if (a.isAttack)
						++rv;
				}
				return rv;
			}
		}
		public int incomingDefenders
		{
			get
			{
				var rv = 0;
				foreach (var a in incoming)
				{
					if (a.isDefense)
						++rv;
				}
				return rv;
			}
		}
		public TroopTypeCount[] combinedIncoming
		{
			get
			{
				TroopTypeCount[] rv = null;
				foreach (var i in incoming)
				{
					if (i.isAttack)
						rv = rv != null ? rv.Add(i.troops) : i.troops;
				}
				return rv;
			}
		}


		public int incomingDefTS
		{
			get
			{
				var rv = 0;
				foreach (var a in incoming)
				{
					if (a.isDefense)
						rv += a.troops.TSDef();
				}
				return rv;
			}
		}
		public int incomingOffTS
		{
			get
			{
				var rv = 0;
				foreach (var a in incoming)
				{
					if (a.isDefense)
						rv += a.troops.TSOff();
				}
				return rv;
			}
		}
		public int incTotal
		{
			get
			{
				var rv = 0;
				foreach (var a in incoming)
				{
					if (!a.isDefense)
						rv += a.ts;
				}
				return rv;
			}
		}
		public string incTT
		{
			get
			{
				var rv = string.Empty;
				foreach (var a in incoming)
				{
					if (!a.isDefense)
					{
						rv += a.Format();
					}
				}
				return rv;
			}
		}
		public int incMax
		{
			get
			{
				var rv = 0;
				foreach (var a in incoming)
				{
					if (!a.isDefense)
						rv = rv.Max(a.ts);
				}
				return rv;
			}
		}
		public string troopsString => GetTroopsString();
		public string GetTroopsString(string separator = ", ")
		{
			if (troopsTotal.IsNullOrEmpty())
			{
				if (isFriend && _tsTotal==0)
					return "No troops";

				return classificationString;
			}
				string rv = string.Empty;
				string sep = string.Empty;
				foreach (var ttc in troopsTotal)
				{
					rv += $"{sep}{troopsHome.Count(ttc.type):N0}/{ttc.count:N0} {Enum.ttNameWithCaps[ttc.type]}";
					sep = separator;
				}
				return rv;
			
		}
		public bool underSiege
		{
			get
			{
				var rv = 0;
				foreach (var a in incoming)
				{
					if (a.type == reportSieging)
						return true;
				}
				return false;
			}
		}
		public enum IncomingClassification
		{
			none,
			incoming,
			underSiege
		}




		//public int activeSieges
		//{
		//    get 
		//        {
		//            var rv = 0;
		//            foreach (var a in incoming)
		//            {
		//                if (a.isSiege)
		//                    ++rv;
		//            }
		//            return rv;
		//        }
		//    }
		public bool iNav => incoming.Any((a) => a.hasNaval);
		public bool iSenny => incoming.Any((a) => a.hasSenator);
		public bool iArt => incoming.Any((a) => a.hasArt);

		public bool isFocus => focus == cid;
		public bool isHover => viewHover == cid;
		public static bool IsHover(int cid)
		{
			return viewHover == cid;
		}

		public static List<int> GetSelectedForContextMenu(int cid, bool onlyIfShiftPressed = true, int ignoreCid=0, bool onlyCities=true, bool onlyMine=false)
		{
			var cids = new List<int>();
			if (cid != 0)
				cids.Add(cid);

			if (!onlyIfShiftPressed || App.IsKeyPressedShift() || App.IsKeyPressedControl())
			{
				foreach (var sel in Spot.selected.ToArray())
				{
					if (sel != cid && sel != ignoreCid && (!onlyCities||City.Get(sel).isCityOrCastle)   )
						cids.Add(sel);
				}
			}
			return cids;
		}

		public static HashSet<int> GetSelectedIncludingHover()
		{
			var rv = new HashSet<int>(selected);
			try
			{
				rv.UnionWith(SettingsPage.pinned);
				if (viewHover != 0)
					rv.Add(viewHover);
				if (focus != 0)
					rv.Add(focus);
				if (City.build != 0)
					rv.Add(City.build);


			}
			catch (Exception ex)
			{
				LogEx(ex);
				return new HashSet<int>(); // if might be corrupt
			}
			return rv;

			
		}

		public void SelectMe(bool showClick=false, VirtualKeyModifiers mod = VirtualKeyModifiers.Shift, bool scrollIntoView = true)
		{
			if(showClick || scrollIntoView)
				NavStack.Push(cid);
			SpotTab.AddToGrid(this, mod, true, scrollIntoView);
			if (showClick)
			{
				JSClient.ShowCity(cid, true);
			}
		}

		
		public static bool TryGetGrid(out RadDataGrid grid)
		{

			if (MainPage.IsVisible())
				grid = MainPage.instance.cityGrid;
			else if (BuildTab.IsVisible())
				grid = BuildTab.instance.cityGrid;
			else
			{
				grid = null;
				return false;
			}

			return true;
		}

		public void ProcessSelection(VirtualKeyModifiers mod, bool forceSelect = false, bool scrollIntoView = true)
		{
			++SpotTab.silenceSelectionChanges;

			App.DispatchOnUIThreadSneaky(() =>
			{
				try
				{
				//	var sel0 = SpotTab.instance.selectedGrid.SelectedItems;
					//var grid = GetGrid();
					//var sel1 = grid.SelectedItems;
					var sel = selected;
					var present = sel.Contains(cid);
					var wantUISync = false;
					if (mod.IsShift() || mod.IsControl() || forceSelect)
					{
						if (present)
						{
							if (!forceSelect && !mod.IsShift())
							{
								selected = new HashSet<int>(sel.Where(a => a != cid));
						//		sel0.Remove(this);
						//		sel1.Remove(this);

							}
							else
							{
							//	wantUISync = true;
							}
						}
						else
						{
							var newSel = new HashSet<int>(sel);
							newSel.Add(cid);
							selected = newSel;

							//sel0.Add(this);
						//	sel1.Add(this);
							wantUISync = true;

						}
						//                 SpotTab.SelectedToGrid();
					}

					else
					{
						wantUISync = true;
						// clear selection and select this
						if (present && selected.Count == 1)
						{
							/// nothing
						}
						else
						{
							selected = new HashSet<int>(new[] { cid });

							///sel0.Clear();
							//sel0.Add(this);

							//sel1.Clear();
							//sel1.Add(this);
						}
						//                   SpotTab.SelectOne(this);
					}
					SyncUISelection(scrollIntoView,  this);
				}
				catch (Exception e)
				{
					LogEx(e);
				}
				finally
				{
					--SpotTab.silenceSelectionChanges;
				}
			});
			//    SpotTab.SelectedToGrid();
		}

		public static void SyncUISelection(bool scrollIntoView, Spot spot=null )
		{
			++SpotTab.silenceSelectionChanges;
			try
			{

				foreach (var grid in UserTab.spotGrids)
				{
					var uiInSync = false;
					var sel1 = grid.SelectedItems;
					if (selected.Count == sel1.Count)
					{
						uiInSync = true;
						foreach (var i in sel1)
						{
							if (!selected.Contains((i as City).cid))
							{
								uiInSync = false;
								break;
							}
						}
					}
					if (!uiInSync)
					{
						sel1.Clear();
						foreach (var i in selected)
						{
							sel1.Add(City.GetOrAddCity(i));
						}
					}
					if ((scrollIntoView || !uiInSync) && (sel1.Any() ||spot != null ) )
					{
						grid.ScrollItemIntoView(spot ?? (City.GetBuild().isSelected ? City.GetBuild(): sel1.First() ) );
					}
				}
			}
			finally
			{
				--SpotTab.silenceSelectionChanges;
			}

		}

		public static bool AreAnySelected()
		{
			return selected.Count != 0;// || viewHover != 0 || uiHover != 0;
		}
		public static bool IsSelectedOrHovered(int cid, bool noneIsAll)
		{
			// if nothing is selected we treat it as if everything is selected
			return (noneIsAll && selected.Count == 0) ? true : (cid == viewHover || selected.Contains(cid) || City.IsFocus(cid));
		}
		public static bool IsSelectedOrHovered(int cid0, int cid1, bool noneIsAll)
		{
			// if nothing is selected we treat it as if everything is selected
			return (noneIsAll && selected.Count == 0) ? true : (cid0 == viewHover || selected.Contains(cid0) || City.IsFocus(cid0)
												|| cid1 == viewHover || selected.Contains(cid1) || City.IsFocus(cid1));
		}

		public static bool IsSelected(int cid) => selected.Contains(cid);
		public bool isSelected
		{
			get => selected.Contains(cid);
			set => selected.Remove(cid);
		}
		public static int viewHover; // in the view menu

		//        public static string uiHoverColumn = string.Empty;
		//	public static int uiPress; //  set when pointerPressed is recieved, at this point a contect menu might come up, causing us to lose uiHover
		public static string uiPressColumn = string.Empty;

		readonly static int[] pointSizes = { 1000, 6000 };
		public uint spatialIndex => cid.ZCurveEncodeCid();
		const int pointSizeCount = 2;

		int GetSize()
		{
			for (int i = 0; i < pointSizeCount; ++i)
				if (points < pointSizes[i])
					return i;
			return pointSizeCount;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Spot);
		}

		public bool Equals(Spot other)
		{
			return other != null &&
				   cid == other.cid;
		}

		public override int GetHashCode()
		{
			return cid;
		}

		//public void ShowCity(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		//{
		//	JSClient.CitySwitch(cid, false);

		//}

		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		public override string ToString()
		{
			return $"{{{cid},{cityName}, {xy},{playerName},{tsHome.ToString()}ts}}";
		}
		public void SetFocus(bool scrollIntoView, bool select = true, bool bringIntoWorldView = true, bool lazyMove = true)
		{
			SetFocus(cid, scrollIntoView, select, bringIntoWorldView, lazyMove);
		}

		public static void UpdateFocusText()
		{
			ShellPage.instance.focus.Content = Spot.GetOrAdd(focus).nameAndRemarks;
			ShellPage.instance.coords.Text = focus.CidToString();
		}

		public static void SetFocus(int cid, bool scrollintoView, bool select = true, bool bringIntoView = true,bool lazyMove = true)
		{
			World.UpdateRegionInfo(cid);
			var changed = cid != focus;
			var spot = Spot.GetOrAdd(cid);
			if (select)
				spot.SelectMe(false, App.keyModifiers, scrollintoView);
			if (changed)
			{
				focus = cid;
				App.DispatchOnUIThreadSneakyLow(UpdateFocusText);
			}
			if (bringIntoView)
				cid.BringCidIntoWorldView(lazyMove,false);
		}
		public static int build; // city that has Build selection.  I.e. in city view, the city you are in
		public static int lockedBuild; // 
		public bool isBuild => cid == build;
		public static bool IsBuild(int cid)
		{
			return build == cid;
		}

		public static bool CanChangeCity(int cid)
		{
			Assert(City.CanVisit(cid));
			var changed = cid != build;
			if (changed)
			{

				if (lockedBuild != 0 && cid != lockedBuild)
				{
					
					return false;
					
				}
			}
			return true;
		}
		public static async Task<bool> CanChangeCityAsync(int cid)
		{
			Assert(City.CanVisit(cid));
			var changed = cid != build;
			if (changed)
			{
				if (!City.CanVisit(cid))
					return false;

				if (lockedBuild != 0 && cid != lockedBuild)
				{
					Note.Show("Please wait for current operation to complete");
					if (await App.DoYesNoBox("Busy", "Please wait for current operation to complete") != 1)
					{
						return false;
					}
				}
			}
			return true;
		}

		public async Task<bool> SetBuildInternal(bool scrollIntoView, bool select = true, bool isLocked=false)
		{
			var changed = cid != build ;
			if (changed)
			{
				if (lockedBuild != 0 && cid != lockedBuild)
				{
					Note.Show("Please wait for current operation to complete");
					if (await App.DoYesNoBox("Busy", "Please wait for current operation to complete") != 1)
					{
						throw new System.Exception("SetBuildOverlap");
					}
				}
				bool wantUnblock = false;
					if(!isLocked)
						await App.uiSema.WaitAsync();
					try
					{

						var wasPlanner = CityBuild.isPlanner;

						if (wasPlanner)
						{
							var b = City.GetBuild();
							b.BuildingsCacheToShareString();
							await b.SaveLayout();
							CityBuild.isPlanner = false;
						}
						City.build = cid;
						Assert(pid == Player.activeId);
						//Cosmos.PublishPlayerInfo(JSClient.jsBase.pid, City.build, JSClient.jsBase.token, JSClient.jsBase.cookies); // broadcast change

						foreach (var p in PlayerPresence.all)
						{
							if (p.pid != Player.myId && p.cid == cid)
							{
								Note.Show($"You have joined {p.name } in {City.Get(p.cid).nameMarkdown}");
							}
						}

						City.CitySwitched();
						if (wasPlanner)
						{
							await GetCity.Post(cid );
							await CityBuild._IsPlanner(true, false);
						}
					// async
						wantUnblock = true;
					}
					finally
					{
					   if(!isLocked)
						App.uiSema.Release();
					}

				if(wantUnblock)
					CityBuildQueue.UnblockQueue(cid);

			}
			SetFocus(scrollIntoView, select);
			City.SyncCityBox();
			return changed;
			//if (!noRaidScan)
			// {
			//      if (changed)
			//          ScanDungeons.Post(cid, getCityData);
			//  }
		}

		public void ReturnSlowClick()
		{
			Raiding.ReturnSlow(cid, true);
		}
		public async void ReturnAt(object sender, RoutedEventArgs e)
		{
			DateTimeOffset? time = null;
			try
			{
				await JSClient.CitySwitch(cid, lazyMove: true, false, false, waitOnChange: true);
				var ogaStr = await JSClient.view.InvokeScriptAsync("getOGA", null);
				var jsDoc = JsonDocument.Parse(ogaStr);
				foreach (var i in jsDoc.RootElement.EnumerateArray())
				{
					var type = i[0].GetAsInt();
					if (type ==5) // raid
						continue;
					Trace(type);
					var timing = i[6].GetAsString();
					var id = timing.IndexOf("Departs:");
					if (id == -1)
						continue;
					timing = timing.Substring(id + 9);
					var t = JSClient.ServerTime(); ;
					var today = timing.StartsWith("Today");
					var tomorrow = timing.StartsWith("Tomorrow");
					if (today || tomorrow)
					{
						timing = today ? timing.Substring(6) : timing.Substring(9);
						var hr = int.Parse(timing.Substring(0, 2));
						var min = int.Parse(timing.Substring(3, 2));
						var sec = int.Parse(timing.Substring(6, 2));
						t = new DateTimeOffset(t.Year, t.Month, t.Day, hr, min, sec, TimeSpan.Zero);
						if (tomorrow)
							t += TimeSpan.FromDays(1);
					}
					else
					{
						t = timing.ParseDateTime(true) ;
					}
					Trace(t);
					t -= TimeSpan.FromHours(SettingsPage.returnRaidsBias);
					if (time == null || time > t)
						time = t;
				}
			}
			catch (Exception ex)
			{
				LogEx(ex, eventName:"OGA");
			}
			(var at, var okay) = await Views.DateTimePicker.ShowAsync("Return By:", time);
			if (!okay)
				return; // aborted

			await Raiding.ReturnAt(cid, at);
			Note.Show($"{City.Get(cid).nameMarkdown} end raids at {at.FormatDefault()}");
		}

		public bool isHubOrStorage => HasTag(Tags.Hub) || HasTag(Tags.Storage);
		
		public async void ReturnAtBatch(object sender, RoutedEventArgs e)
		{
			(var at, var okay) = await Views.DateTimePicker.ShowAsync("Return By:");
			if (!okay)
				return; // aborted
			using var work = new ShellPage.WorkScope("Return At..");

			var cids = MainPage.GetContextCids(cid);
			foreach (var _cid in cids)
			{
				var __cid = _cid;
				await Raiding.ReturnAt(__cid, at);
			}
			Note.Show($"End {cids.Count} raids at {at.FormatDefault()} ");

		}

		void DecayQuery()
		{
			JSClient.gStCB(cid, DecayQueryCB, AMath.random.Next());
		}

		async void DecayQueryCB(JsonElement jso)
		{
			var type = jso.GetAsInt("type");
			var _cid = jso.GetAsInt("cid");
			Assert(cid == _cid);
			if (type != 3 && type != -1) // 4 is empty, 3 is city or ruins, -1 means not open (for a continent)
			{
				App.DispatchOnUIThreadSneaky(() =>
			   {
				   var dialog = new ContentDialog()
				   {
					   Title = "Spot has Changed",
					   Content = cid.CidToString(),
					   PrimaryButtonText = "Okay"
				   };
				   //SettingsPage.BoostVolume();
				   ElementSoundPlayer.Play(ElementSoundKind.Invoke);
				   COTG.Services.ToastNotificationsService.instance.SpotChanged($"{cid.CidToString()} has changed");
				   dialog.ShowAsync2();
			   });
				JSClient.ShowCity(cid, false);
			}
			else
			{
				//	Note.Show($"Query {cid.CidToStringMD()},type:{type}");
				await Task.Delay(60 * 1000);
				DecayQuery();
			}
		}

		public async Task SuperRaid()
		{
			await Post.Send("overview/rcallall.php", "a=" + cid, World.CidToPlayerOrMe(cid));
			await Post.SendEncrypted("includes/UrOA.php", "{\"a\":" + cid + ",\"c\":0,\"b\":2}", "Rx3x5DdAxxerx3", World.CidToPlayerOrMe(cid));
		}

		public void ReturnFastClick()
		{
			if (Raid.test)
			{
				SuperRaid();
			}
			else
			{
				Raiding.ReturnFast(cid, true);
			}
		}

		//int IKeyedItem.GetKey()
		//{
		//    return cid;
		//}
		//void IKeyedItem.Ctor(int i)
		//{
		//    cid = i;
		//}
		public static void JSCommand(string func, int cid)
		{
			JSClient.view.InvokeScriptAsync(func, new string[] { (cid % 65536).ToString(), (cid >> 16).ToString() });
		}
		public static void JSAttack(int cid)
		{
			JSCommand("spotAttack", cid);
		}
		public static void JSDefend(int cid)
		{
			JSCommand("spotDefend", cid);
		}
		public static void JSSendRes(int cid)
		{
			JSCommand("spotSendRes", cid);
		}
		public static void JSRaid(int cid)
		{
			JSCommand("spotRaid", cid);
		}
		public void ShowDistanceTo()
		{
			var sel = GetSelectedForContextMenu(0,false,cid,true);
			var _cid = (sel.Count == 1) ? sel[0] : City.build;


			// todo cart travel time, ship travel time
			var dist = cid.DistanceToCid(_cid);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"From {nameMarkdown} to {City.Get(_cid).nameMarkdown}");
			sb.Append(dist.ToString("0.00"));

			sb.Append($"\nCarts: {TimeSpan.FromMinutes(dist * cartTravel).ToString(AUtil.defaultTimeSpanFormat)}, ");
			if (isOnWater && Spot.GetOrAdd(_cid).isOnWater)
			{
				sb.Append($"\nShips: {TimeSpan.FromMinutes(dist * shipTravel + 60).ToString(AUtil.defaultTimeSpanFormat)}");
			}
			for (int i = 1; i < ttCount; ++i)
			{
				var dt = TimeSpan.FromMinutes(dist * TTTravel(i));
				sb.Append($"\n{ttName[i]}: {dt.ToString(AUtil.defaultTimeSpanFormat)}");
			}
			var str = sb.ToString();
			App.CopyTextToClipboard(str);
			Note.Show(str, false, false, 20 * 1000);
		}
		public bool canVisit => isFriend;


		public string nameMarkdown => $"[{nameAndRemarks}](/c/{cid.CidToString()})";
		
		public static bool OnKeyDown(object _spot, VirtualKey key)
		{
			var spot = _spot as Spot;
			switch (key)
			{
				case VirtualKey.Enter:
					spot.SetFocus(false);
					return true;
					break;
				case VirtualKey.Space:
					{
						if (spot.canVisit)
							spot.ShowDungeons();
						else
							spot.SetFocus(false);
						return true;
					}
				
				default:
					break;
			}
			return false;
		}

		public void ShowContextMenu(UIElement uie, Windows.Foundation.Point position)
		{
			
			//   SelectMe(false) ;
			var flyout = new MenuFlyout();
			var aMisc = flyout.AddSubMenu( "Misc..");
			var aExport = flyout.AddSubMenu("Import/Export..");
			var aSetup = AApp.AddSubMenu(flyout, "Setup..");
			var aWar = AApp.AddSubMenu(flyout, "War..");
			if (this.isCityOrCastle)
			{
				// Look - its my city!
				if (this.isFriend)
				{

					//{
					//	var tags = TagHelper.GetTags(this);
					//	var tagFlyout = AApp.AddSubMenu(flyout, "Tags");
					//	foreach(var t in TagHelper.tags)
					//	{
					//		var n = t.s;
					//		var id = t.id;
					//		AApp.AddItem(tagFlyout, n,tags.HasFlag(id),(on)=>SetTag(id,on) );


					//	}
					//}
					// This one has multi select
					var aRaid = AApp.AddSubMenu(flyout, "Raid..");

					int count = 1;
					if (uie == MainPage.CityGrid || uie == BuildTab.CityGrid || uie == NearDefenseTab.instance.supportGrid)
					{
						count = MainPage.GetContextCidCount(cid);
					}
					if (count > 1)
					{
						aRaid.AddItem( $"End Raids x{count} selected", MainPage.ReturnSlowClick, cid);
						aRaid.AddItem( $"Return Asap x{count} selected", MainPage.ReturnFastClick, cid);
						aRaid.AddItem( $"Return At...x{count}", this.ReturnAtBatch);

					}
					else
					{

						aRaid.AddItem( "End Raids", this.ReturnSlowClick);
						aRaid.AddItem("Return Asap", this.ReturnFastClick);
						aRaid.AddItem( "Return At...", this.ReturnAt);
					}

					
					aSetup.AddItem("Setup...", Spot.InfoClick, cid);
					aSetup.AddItem( "Find Hub", (_, _) => CitySettings.SetHub(cid));
					aSetup.AddItem( "Set Recruit", (_, _) => CitySettings.SetRecruitFromTag(cid));
					aSetup.AddItem("Change...", (_, _) => ShareString.Show(cid));

					//   AApp.AddItem(flyout, "Clear Res", (_, _) => JSClient.ClearCenterRes(cid) );
					aSetup.AddItem("Clear Center Res", (this as City).ClearRes );


					aExport.AddItem("Troops to Sheets", CopyForSheets);
				}
				else
				{
					if( _cityName == null )
					{
						JSClient.FetchCity(cid);
					}

				}
				{
					var sel = Spot.GetSelectedForContextMenu(cid, false);
					if (AttackTab.instance.isVisible)
					{
						var multiString = sel.Count > 1 ? $" _x {sel.Count} selected" : "";
						var afly = AApp.AddSubMenu(flyout, "Attack Planner");

						if ( !Alliance.IsAllyOrNap(this.allianceId) )
						{
							afly.AddItem("Add as Target" + multiString, (_, _) => AttackTab.AddTarget(sel));
							afly.AddItem("Ignore Player" + multiString, (_, _) => AttackTab.IgnorePlayer(cid.CidToPid() ));
						}
						if (!Alliance.IsEnemy(this.allianceId))
						{
							afly.AddItem("Add as Attacker" + multiString, (_, _) =>
						{
							using var work = new WorkScope("Add as attackers..");

							string s = string.Empty;
							foreach (var id in sel)
							{
								s = s + id.CidToString() + "\t";
							}
							AttackTab.instance.AddAttacksFromString(s);
							Note.Show($"Added attacker {s}");

						});
						};
					}
					//else
					if (!Alliance.IsAllyOrNap(this.allianceId))
					{
						aWar.AddItem("Add funky Attack String", async (_, _) =>
					   {
						   using var work = new WorkScope("Add to attack string..");

						   foreach (var id in sel)
						   {
							   await JSClient.AddToAttackSender(id);
						   }
					   }
						);
					}
					//AApp.AddItem(flyout, "Add as Fake (2)", (_, _) => AttackTab.AddTarget(cid, 2));
					//AApp.AddItem(flyout, "Add as Fake (3)", (_, _) => AttackTab.AddTarget(cid, 3));
					//AApp.AddItem(flyout, "Add as Fake (4)", (_, _) => AttackTab.AddTarget(cid, 3));
				}
				if (cid != City.build)
				{
					aSetup.AddItem( "Set target hub", (_, _) => CitySettings.SetTargetHub(City.build, cid));
					aSetup.AddItem("Set source hub", (_, _) => CitySettings.SetSourceHub(City.build, cid));
					//if(Player.myName == "Avatar")
					//    AApp.AddItem(flyout, "Set target hub I", (_, _) => CitySettings.SetOtherHubSettings(City.build, cid));
				}
				

				aWar.AddItem( "Attack", (_, _) => Spot.JSAttack(cid));
				aWar.AddItem( "Near Defence", DefendMe);
				if (incoming.Any())
					aWar.AddItem( "Incoming", ShowIncoming);


				aWar.AddItem("Recruit Sen", (_, _) => Recruit.Send(cid, ttSenator,1));
				aWar.AddItem( "Send Defence", (_, _) => JSDefend(cid));
				aWar.AddItem( "Show Reinforcements", (_, _) => Reinforcement.ShowReturnDialog(cid, uie));
				aExport.AddItem( "Defense Sheet", ExportToDefenseSheet);
				AApp.AddItem(flyout, "Send Res", (_, _) => Spot.JSSendRes(cid));
				AApp.AddItem(flyout, "Near Res", ShowNearRes);
				if (isFriend)
				{
					AApp.AddItem(flyout, "Do the stuff", (_, _) => DoTheStuff());
					AApp.AddItem(flyout, "Food Warnings", (_, _) => CitySettings.SetFoodWarnings(cid) );
					flyout.AddItem( "Ministers", (this as City).ministersOn,  (this as City).SetMinistersOn );
				}
			}
			else if (this.isDungeon || this.isBoss)
			{
				AApp.AddItem(flyout, "Raid", (_, _) => Spot.JSRaid(cid));

			}
			else if (this.isEmpty && Discord.isValid)
			{
				AApp.AddItem(flyout, "Claim", this.DiscordClaim);

			}
			aMisc.AddItem( "Notify on Decay", DecayQuery);

			aMisc.AddItem( "Distance", (_, _) => ShowDistanceTo());
			aMisc.AddItem( "Select", (_, _) => SelectMe(true, App.keyModifiers));
			aMisc.AddItem("Coords to Chat", () => ChatTab.PasteToChatInput(cid.CidToCoords(), true));
			flyout.RemoveEmpy();
			flyout.CopyXamlRoomFrom(uie);

			//   flyout.XamlRoot = uie.XamlRoot;
			flyout.ShowAt(uie, position);
		}

		
		public async Task DoTheStuff()
		{
			await App.DispatchOnUIThreadExclusive(cid,async () =>
			{
				await QueueTab.DoTheStuff(this as City, true, true);
			});
		}
		public static async void InfoClick(object sender, RoutedEventArgs e)
		{
			var cids = MainPage.GetContextCids(sender);
			foreach (var cid in cids)
			{
				var _cid = cid;
				if(!await App.DispatchOnUIThreadExclusive(_cid, async () =>
				 {
				 return await CityRename.RenameDialog(_cid, true);
						
				}) )
			{
				break;
			}
			}
		}
		public void DefendMe()
		{
			NearDefenseTab.defendant = this;
			var tab = NearDefenseTab.instance;
			tab.ShowOrAdd(true);
			tab.refresh.Go();
		}

		
		public void ShowNearRes()
		{
			var tab = NearRes.instance;
			tab.target = (City)this;
			if (!tab.isActive)
			{
				tab.ShowOrAdd( true);
			}
			else
			{
				if (!tab.isVisible)
					TabPage.Show(tab);
				else
					tab.refresh.Go();
			}
		}
		public void BuildStageDirty()
		{
			App.DispatchOnUIThreadSneakyLow(() => OnPropertyChanged(nameof(City.buildStage)));
		}
		public async void ShowIncoming()
		{
			if (allianceId == Alliance.myId)
			{
				IncomingTab tab = IncomingTab.instance;
				App.DispatchOnUIThreadSneakyLow( ()=> tab.Show() );
				for (; ; )
				{
					await Task.Delay(2000);
					if (tab.defenderGrid.ItemsSource != null )
						break;
				}
				App.DispatchOnUIThreadSneaky(() =>
				{
					tab.defenderGrid.SelectItem(this);
					tab.defenderGrid.ScrollItemIntoView(this);
				});

			}
			else
			{
				var tab = OutgoingTab.instance;
				App.DispatchOnUIThreadSneakyLow(()=>tab.Show());
				for (; ; )
				{
					await Task.Delay(2000);
					if (tab.attackerGrid.ItemsSource != null )
						break;
				}
				App.DispatchOnUIThreadSneaky(() =>
				{
					tab.attackerGrid.SelectItem(this);
					tab.attackerGrid.ScrollItemIntoView(this);
				});
			}
		}



		public async void DiscordClaim()
		{
			if (!Discord.isValid)
			{
				Log("Invalid");
				return;
			}
			try
			{
				Note.Show($"Registering claim on {xy}");
				var client = JSClient.genericClient;


				var message = new Discord.Message() { username = "Cord Claim", content = $"{xy} claimed by {Player.myName}", avatar_url = "" };

				var content = new HttpStringContent(
						  JsonSerializer.Serialize(message, Json.jsonSerializerOptions), Windows.Storage.Streams.UnicodeEncoding.Utf8,
						   "application/json");

				var result = await client.PostAsync(Discord.discordHook, content);
				result.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}


		}
		async void CopyForSheets()
		{
			var sb = new StringBuilder();
			int counter = 0;
			var cids = GetSelectedForContextMenu(cid, false);
			foreach (var _cid in cids)
			{
				++counter;
				var s = Spot.GetOrAdd(_cid);
				var c = await s.ClassifyEx(true);
				switch (classification)
				{
					case Classification.sorcs:
						sb.Append("Sorc\t");
						break;
					case Classification.druids:
						sb.Append("Druids\t");
						break;
					case Classification.praetor:
						sb.Append("prae\t");
						break;
					case Classification.priestess:
						sb.Append("priest\t");
						break;
					case Classification.horses:
					case Classification.arbs:
						sb.Append("Horses\t");
						break;

					case Classification.se:
						sb.Append("Siege engines\t");
						break;
					case Classification.navy:
						sb.Append("Warships?\t");
						break;
					case Classification.stingers:
						sb.Append("Stungers\t");
						break;
					default:
						sb.Append("vanq\t");
						break;
				}
				sb.Append(s.tsTotal + "\t");
				sb.Append(s.hasAcademy.GetValueOrDefault() ? "Yes\t" : "No\t");
				sb.Append(s.xy + "\n");

			}
			App.CopyTextToClipboard(sb.ToString());
			Note.Show($"Copied {counter} castles to clipboard for sheets");
		}

		public static void ScrollIntoView(int cid)
		{
			//         await Task.Delay(2000);
			//          instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
			//           {
			//   await Task.Delay(200);
			App.DispatchOnUIThreadSneakyLow(() =>
			{

				{
					/// MainPage.CityGrid.SelectedItem = this;
					//                      MainPage.CityGrid.SetCurrentItem(this);

					//     MainPage.CityGrid.SetCurrentItem(this,false);
					if (MainPage.IsVisible())
						MainPage.CityGrid.ScrollItemIntoView(City.GetOrAdd(cid));
					if (BuildTab.IsVisible())
						BuildTab.CityGrid.ScrollItemIntoView(City.GetOrAdd(cid));
					// await Task.Delay(200);
					//MainPage.CityGrid.SelectItem(this);
					//var id = gridCitySource.IndexOf(this);
					//if (id != -1)
					//{
					//    MainPage.CityGrid.ScrollIndexIntoView(id);

					//}
				}
				// todo: donations page and boss hunting


				// ShellPage.instance.coords.Text = cid.CidToString();
				//            });
			});

		}
		public void ScrollMeIntoView() => ScrollIntoView(cid);
		//public List<Dungeon> raidDungeons =>
		//    {

		//    };

	}
	public static class SpotHelper
	{
		public static bool IsOffense(this Spot.Classification c)
		{
			return c switch
			{
				Spot.Classification.sorcs
				or Spot.Classification.vanqs
				or Spot.Classification.druids
				or Spot.Classification.horses
				or Spot.Classification.navy
				or Spot.Classification.se => true,
				_ => false
			};
		}
		public static bool IsDefense(this Spot.Classification c)
		{
			return c switch
			{
				Spot.Classification.rt or
				Spot.Classification.praetor or
				Spot.Classification.priestess or 
				Spot.Classification.arbs or
				Spot.Classification.stingers => true,
				_ => false
			};
		}
		public static string CellText(this DataGridCellInfo cell) => (cell?.Column as DataGridTypedColumn)?.PropertyName ?? string.Empty;
	}
}
