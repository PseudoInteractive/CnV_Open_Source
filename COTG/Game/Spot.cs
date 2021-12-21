using CnV.Helpers;
using CnV.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
//using Windows.UI.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using CnV.Views;
using TroopTypeCounts = CnV.Game.TroopTypeCounts;
//COTG.DArray<COTG.Game.TroopTypeCount>;
using TroopTypeCountsRef = CnV.Game.TroopTypeCounts;
using static CnV.Game.TroopTypeCountHelper;
//COTG.DArrayRef<COTG.Game.TroopTypeCount>;

using static CnV.Debug;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Windows.System;
//using Microsoft.UI.Xaml.Controls;

using System.Text.Json;
using System.Net.Http;
using static CnV.Game.Troops;
using Windows.ApplicationModel.DataTransfer;
using static CnV.Views.ShellPage;
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
using MenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using MenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using ToggleMenuFlyoutItem = Microsoft.UI.Xaml.Controls.ToggleMenuFlyoutItem;
using MenuFlyoutSubItem = Microsoft.UI.Xaml.Controls.MenuFlyoutSubItem;
using System.Collections.ObjectModel;
//using PointerPoint = Microsoft.UI.Input.Experimental.ExpPointerPoint;
using Cysharp.Text;
using DiscordCnV;
//using Windows.UI.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Net.Http.Json;

namespace CnV.Game
{
	using CnV;
	using Helpers;
//	// using PInvoke
	using Services;
	using Syncfusion.UI.Xaml.DataGrid;
	using Syncfusion.UI.Xaml.Grids.ScrollAxis;
	using Views;
	using DateTimePicker = Views.DateTimePicker;

	//public interface IKeyedItem
	//{
	//  public  int GetKey();
	//  public  void Ctor(int id);
	//}
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public abstract partial class Spot: SpotS, IEquatable<Spot>, IANotifyPropertyChanged
	{
		public override bool Equals(object obj)
		{
			return Equals(obj as Spot);
		}
		
		public bool Equals(Spot other)
		{
			return other != null &&
			       cid == other.cid;
		}
		public bool Equals(int _cid)
		{
			return   cid == _cid;
		}
		public override int GetHashCode()
		{
			return cid;
		}
		public static bool operator ==(Spot a, int cid) => a.cid == cid;
		public static bool operator !=(Spot a, int cid) => a.cid != cid;

		
		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void CallPropertyChanged(string members = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members));
		}

		public void OnPropertyChanged(string members = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged(members,(cid ==City.focus || cid ==City.build ));
		}
		public static ConcurrentDictionary<int, City> allSpots = new ConcurrentDictionary<int, City>(); // keyed by cid
		public static HashSet<int> selected = new HashSet<int>();
		public static City[] defendersI = Array.Empty<City>();
		public static City[] defendersO = Array.Empty<City>();
		public static City[] attackersI = Array.Empty<City>();
		public static City[] attackersO = Array.Empty<City>();
		public City _City => this as City;
		public static HashSet<City> allianceCitiesWithOutgoing
		{
			get
			{
				HashSet<City> rv = new();
				foreach (var d in defendersO)
				{
					foreach (var att in d.incoming)
					{
						rv.Add(att.sourceCity);
					}
				}
				return rv;
			}
		}

		// indexed by PackedContinentId
		public const ulong continentFilterAll = 0xfffffffffffffffful;
		public static ulong continentFilter = continentFilterAll;

		public static Tags tagFilter = default;

		public static ulong ContinentFilterFlag(int id) => 1ul << id;
		public static bool isContinentFilterAll => continentFilter == continentFilterAll;
		public static bool TestContinentFilterPacked(int id) => (continentFilter & ContinentFilterFlag(id)) != 0;
		public static void SetContinentFilterPacked(int id, bool set) => continentFilter = (continentFilter & (~ContinentFilterFlag(id))) | (set ? ContinentFilterFlag(id) : 0ul);
		public bool testContinentFilter => TestContinentFilter(cid);
		public bool testTagFilter => (tagFilter == default) | ((tagFilter & tags) != 0);
		public bool testContinentAndTagFilter => TestContinentFilter(cid) & testTagFilter;

		public static bool TestContinentFilter(int cid) => (continentFilter & ContinentFilterFlag(World.CidToPackedContinent(cid))) != 0;
		public static bool TestContinentAndFlagFilter(int cid) => cid.AsCity().testContinentAndTagFilter;
		
		// slow
		public static bool TryGet(string nameAndRemarks,bool mine,out City spot) 
		{
			City[] cities = mine ? allSpots.Values as City[] : City.myCities;
			foreach(var i in cities)
			{
				if(i.nameAndRemarks == nameAndRemarks)
				{
					spot = i;
					return true;
				}

			}
			spot = null; 
			return false;
		}
		public static bool TryGet(int cid, out City spot) => allSpots.TryGetValue(cid, out spot);
	
		public bool isFriend => Player.IsFriend(pid); // this is set if it is one of our cities or our ally cities that we can visit
		public bool? isAlly => Alliance.alliancesFetched ? cid.CidIsAlly() : null; // this is set if it is one of our cities or our ally cities that we can visit
		public bool? isEnemy => Alliance.alliancesFetched ? cid.CidIsEnemy() : null; // this is set if it is one of our cities or our ally cities that we can visit

		internal static City GetFocus()
		{
			return focus == 0 ? null : GetOrAdd(focus);
		}

		public static City invalid = new City() { _cityName = "?" };
		//public static City pending = new City() { _cityName = "pending" };

		//public static City[] emptySpotSource = new[] { pending };

		public string nameAndRemarksAndPlayer => isMine? nameAndRemarks : $"{nameAndRemarks} - {playerName}";


		public string nameAndRemarks {
			get
			{
				using var sb = ZString.CreateUtf8StringBuilder();
				sb.Append(cityName);
				if (!remarks.IsNullOrEmpty())
				{
					sb.Append(" - ");
					sb.Append(remarks);
				}

				sb.Append(statusString);
				return sb.ToString();
			}
		}
		public string statusString
		{
			get
			{
				string rv = string.Empty;
				if(incoming.Length > 0)
				{
					var sieged = false;
					var hasSen = false;
					var hasArt = false;
					foreach(var i in incoming)
					{
						if(i.isAttack)
						{
							sieged |= i.isSiege;
							hasSen |= i.hasSenator;
							hasArt |= i.hasArt;
						}
					}
					rv += (sieged ? (hasArt && hasSen ? "(SA)" : hasArt ? "(A)" : hasSen ? "(S)" : "(n)") : "(i)");
				}
				if(outGoing!=OutGoing.none)
				{
					rv += "(O)";
				}
				if(isEnemy == true )
					rv += "(e)";

				return rv;
			}
		}
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
				//	Assert(!World.loadedAtLeastOnce);
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

		internal bool HasTag(Tags tag) => tags.HasFlag(tag);

		public bool isMine => pid == Player.myId;

		public string _cityName;
		public string cityName => _cityName ?? xy;
		public string cityNameOrNull => _cityName ?? string.Empty;
		



		public int _tsHome; // cache for when tsHome is missing
		public int _tsTotal;
		public int tsRaid => (troopsHome.Any() ? troopsHome.TSRaid() : _tsHome);
		public int tsHome => (troopsHome.Any() ? troopsHome.TS() : _tsHome);
		public int tsDefCityHome => (troopsHome.Any() ? troopsHome.TSDef() : _tsHome);
		public int tsDefCityTotal => (troopsTotal.Any() ? troopsTotal.TSDef() : _tsTotal);
		public float raidIdle => troopsHome.TSRaid() / (float)troopsTotal.TSRaid().Max(1);

		public int tsTotal => (troopsTotal.Any() ? troopsTotal.TS() : _tsTotal);
		public int tsRaidTotal => (troopsTotal.Any() ? troopsTotal.TSRaid() : _tsTotal);
		public int tsDefMax
		{
			get
			{
				if (incoming.Any())
				{
					return incomingDefTS;
				}
				return reinforcementsIn.TS() + (troopsHome.Any() ? troopsHome.TSDef() : _tsHome);
			}
		}
		public int tsOff { get { var i = incomingOffTS; return (i > 0) ? i : troopsHome.TSOff(); } }

		public NotifyCollection<Reinforcement>? reinforcementsIn = null;
		public NotifyCollection<Reinforcement>? reinforcementsOut = null;

		// need properties for UI
		public NotifyCollection<Reinforcement>? reinforcementsInProp =>  reinforcementsIn;
		public NotifyCollection<Reinforcement>? reinforcementsOutProp => reinforcementsOut;

		public void SetReinforcementsOut(IEnumerable<Reinforcement> l)
		{
			AUtil.SetNullable(ref reinforcementsOut,
				l.OrderByDescending(r => r.targetCity.reinforcementSortScore).ToArray());
		}

		public void AppendReinforcementsOut(Reinforcement r)
		{
			SetReinforcementsOut( reinforcementsOut.AppendNullable(r));
		}
		public void SetReinforcementsIn(IEnumerable<Reinforcement> l)
		{
			AUtil.SetNullable(ref reinforcementsIn,
				l.OrderByDescending(r => r.sourceCity.reinforcementSortScore).ToArray());
		}
		public void AppendReinforcementsIn(Reinforcement r)
		{
			SetReinforcementsIn( reinforcementsIn.AppendNullable(r));
		}

		//public Reinforcement[] reinforcementsInSorted =>
		//	reinforcementsIn.OrderByDescending(r => r.sourceCity.reinforcementSortScore).ToArray();
		//public Reinforcement[] reinforcementsOutSorted =>
		//	reinforcementsOut.OrderByDescending(r => r.targetCity.reinforcementSortScore).ToArray();

		public string defString => GetDefString(", ");
		public string GetDefString(string separator)
		{
			TroopTypeCounts all = new();
			if (incoming.Any())
			{
				foreach (var a in incoming)
				{
					if (a.isDefense)
						Add(ref all, a.troops);
				}

			}
			else
			{
				Add(ref all, troopsHome, (t => t.isDef || t.isSenator));
				if (reinforcementsIn.AnyNullable())
				{
					foreach (var i in reinforcementsIn)
					{
						Add(ref all, i.troops);
						;
					}
				}
			}
			bool first = true;
			using var sb = ZString.CreateUtf8StringBuilder();
			foreach (var tt in all.Enumerate())
			{
				if (first)
				{
					first = false;
				}
				else
				{
					sb.Append(separator);
				}
				sb.AppendFormat("{0:N0} {1}", tt.count, Troops.ttNameWithCaps[tt.type]);
			}

			return sb.ToString();
		}

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
		public Classification classification;
		public sbyte academyInfo = -1;
		public bool? hasAcademy
		{
			get => academyInfo == 0 ? false : academyInfo == 1 ? true : null;
			set => academyInfo = value == true ? (sbyte)1 : (sbyte)0;
		}
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
		protected static byte[] classificationTTs =
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
		public DateTimeOffset firstIncoming
		{
			get
			{
				foreach (var atk in incoming)
				{
					if (atk.isAttack)
					{
						return atk.time;
					}
				}
				return AUtil.dateTimeZero;
			}
		}
		public string firstIncomingString
		{
			get
			{
				var t = firstIncoming;
				return t.IsZero() ? "??" : t.Format();
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
			OnPropertyChanged(nameof(pinned));
		}  // pinned in MRU
		public byte claim; // only if this is under attack
		public byte shipyards { get; set; }
		public byte ports { get; set; }
		public string Claim => $"{(int)claim:00}%";
		public bool isBlessed { get; set; }
		public float scoutRange { get; set; }
		public ushort points { get; set; }
		private const int iconHeight = 32;
		private const int iconWidth = iconHeight;
		public BitmapImage icon => ImageHelper.FromImages(isBlessed ? "Icons/blessed.png" :
			 ($"{(isTemple ? "Icons/temple" : isCastle ? "Icons/castle" : "Icons/city")}{GetSize()}{(isOnWater ? "w" : "")}.png"), iconWidth,iconHeight);
		public string iconUri => ImageHelper.FromImagesLink(isBlessed ? "Icons/blessed.png" :
			($"{(isTemple ? "Icons/temple" : isCastle ? "Icons/castle" : "Icons/city")}{GetSize()}{(isOnWater ? "w" : "")}.png"));
		public int cont => cid.CidToContinent();
		public int packedContinent => cid.CidToPackedContinent();

		public static bool operator ==(Spot left, Spot right)
		{
			return left is null ? (right is null ? true : false) : left.Equals( right);
		}

		public static bool operator !=(Spot left, Spot right)
		{
			return !(left == right);
		}

		public void ExportToDefenseSheet()
		{
			var cids = GetSelectedForContextMenu(cid, false);
			ExportToDefenseSheet(cids);
		}
		public static void ExportToDefenseSheet(List<int> cids)
		{

			using var work = new WorkScope($"Export Def");
			var sb = new StringBuilder();
			sb.Append("Player\tCont\tCoords\tFirst\tClaim\tDefIncoming\t#Attacks\tTemple\tWater\tIntel\n");
			foreach (var _cid in cids)
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
					sb.Append(s.incoming.Where(x => x.isAttack).First()?.time.Format() ?? "??");
					sb.Append('\t');
					sb.Append(s.claim);
					sb.Append('\t');
					var ts = s.tsDefMax;
					sb.Append(ts);
					sb.Append('\t');
					sb.Append(s.incomingAttacks);
					sb.Append('\t');
					sb.Append(s.isTemple);
					sb.Append('\t');
					sb.Append(s.isOnWater);
					sb.Append('\t');
					sb.Append(s.incTT);
					sb.Append('\n');
				}
			}
			App.CopyTextToClipboard(sb.ToString());

			Note.Show($"Exported {cids.Count} to clipboard for sheets");
		}

		// The UIElement returned will be the SfDataGrid
		//public static (Spot spot, DataGridCellInfo column, PointerPoint pt, UIElement uie) HitTest(object sender, PointerRoutedEventArgs e)
		//{
		//	e.KeyModifiers.UpdateKeyModifiers();
		//	var grid = sender as SfDataGrid;
		//	Assert(grid is not null);
		//	var physicalPoint = e.GetCurrentPoint(grid);
		//	var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
		//	var cell = grid.HitTestService.CellInfoFromPoint(point);
		//	//var row = grid.HitTestService.RowItemFromPoint(point);
		//	//if(cell?.Item != row )
		//	//{
		//	//    Note.Show($"{cell} {row} {cell?.Column}");

		//	//}

		//	var spot = point.Y > 34 ? (cell?.Item as Spot) : null; // workaround for clicking on the header
		//	ClearHover();
		//	viewHover = spot != null ? spot.cid : 0;
		//	Player.viewHover = spot != null ? spot.pid : 0;


		//	return (spot, cell, physicalPoint, grid);
		//}




		//public static void GridPressed(object sender, PointerRoutedEventArgs e)
		//{
		//	//(sender as SfDataGrid).Focus();
		//	try
		//	{
		//		e.KeyModifiers.UpdateKeyModifiers();

		//		var hit = Spot.HitTest(sender,e);
		//		var spot = hit.spot;
		//		//	uiPress = spot != null ? spot.cid : 0;
		//		uiPressColumn = hit.column.CellText();
		//		// The UIElement returned will be the SfDataGrid
		//		if(spot != null)
		//			spot.ProcessClick(uiPressColumn,hit.pt,hit.uie,e.KeyModifiers);
		//	}
		//	catch(Exception ex)
		//	{
		//		Log(ex);
		//	}
		//}

		

		public static void ProcessPointerExited()
		{
			ClearHover();
		}

	

		public static void ClearHover()
		{
			ShellPage.ClearHover();
		}

		


		public async void ProcessClick(string column,PointerPoint pt, UIElement uie, VirtualKeyModifiers modifiers)
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
						if (!await DoClick()) return;
						wantSelect = false;
						wantRaidScan = false;
						break;
					case "+":
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
					SetFocus(false, true, true, false);
				NavStack.Push(cid);

			}
			else if (pt.Properties.IsRightButtonPressed)
			{
				if (!modifiers.IsShiftOrControl())
					SetFocus(false, true, true);
				ShowContextMenu(uie, pt.Position);

			}
			//else if (pt.Properties.IsMiddleButtonPressed)
			//{
			//	var text = ToTsv();
			//	Note.Show($"Copied to clipboard: {text}");
			//	App.CopyTextToClipboard(text);
			//	SetFocus(false);
			//}
			else
			{
				// if shift or conntrol is pressed normal processing takes place
			}
			SpotTab.TouchSpot(cid, modifiers);
		}

		public async Task<bool> DoClick()
		{
			if (City.CanVisit(cid))
			{
				var wasBuild = City.IsBuild(cid);

				if (!await JSClient.CitySwitch(cid, false, true, false))
					return false;

				if (wasBuild)
				{
					JSClient.ChangeView(ShellPage.viewMode.GetNext());
				}
			}
			else
			{
				JSClient.ShowCity(cid, false, true, false);
			}

			return true;
		}

		public async Task ShowDungeons()
		{
			await ScanDungeons.Post(cid, true, false);
		}

		public string ToTsv()
		{
			return $"{cid.CidToCoords()}\t{this.playerName}\t{this.cityName}\t{this.remarks ?? ""}\t{this.alliance}\t{this.isCastle}\t{this.isOnWater}\t{this.isTemple}\t{GetTroopsString(";")}";
		}
		public static int lastCoordClick;

		public static async void ProcessCoordClick(int cid, bool lazyMove, VirtualKeyModifiers mod, bool scrollIntoUI = false)
		{
			mod.UpdateKeyModifiers();

			if(mod.IsShiftAndControl() && AttackTab.IsVisible() && City.Get(cid).isCastle)
			{
				var city = City.Get(cid);
				{
					using var __lock = await AttackTab.instance.TouchLists();
					var prior = AttackPlan.Get(cid);
					var isAttack = prior != null ? prior.isAttack : city.IsAllyOrNap();
					if (isAttack)
					{
						AttackPlan.AddOrUpdate( new(city, isAttack,city.attackType switch
						{ AttackType.assault => AttackType.senator, AttackType.senator => AttackType.se, AttackType.se => AttackType.none, _ => AttackType.assault }));
					}
					else
					{
						AttackPlan.AddOrUpdate( new(city, isAttack, city.attackType switch
						{
							AttackType.seFake => AttackType.se,
							AttackType.se => AttackType.senatorFake,
							AttackType.senatorFake => AttackType.senator,
							AttackType.senator => AttackType.none,
							_ => AttackType.seFake
						}));
					}
				}
				Note.Show($"{city.nameAndRemarks} set to {city.attackType}", Debug.Priority.high);
				AttackTab.WritebackAttacks();
				AttackTab.WaitAndSaveAttacks();
			}
			else if (City.CanVisit(cid) && !mod.IsShiftOrControl())
			{
				if (City.IsBuild(cid))
				{
					JSClient.ChangeView(ShellPage.viewMode.GetNext());// toggle between city/region view
					if (scrollIntoUI)
					{
						Spot.SetFocus(cid, scrollIntoUI, true, true, lazyMove);
					}
					else
					{
						cid.BringCidIntoWorldView(lazyMove);
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

			if (mod.IsShiftAndControl() && (Player.isSpecial || City.CanVisit(cid)) && (!AttackTab.IsVisible()) )
			{
				//     var spot = Spot.GetOrAdd(cid);
				//     GetCity.Post(cid, spot.pid, (js, city) => Log(js));

				var str = await Post.SendForText("includes/gLay.php", $"cid={cid}", World.CidToPlayerOrMe(cid));
				Log(str);

				AppS.DispatchOnUIThreadLow(() =>
				{
					// set is water var
					str = $"{City.shareStringStart}{(World.GetInfoFromCid(cid).isWater ? ';' : ':')}{str.Substring(18)}";
					App.CopyTextToClipboard(str);

					Launcher.LaunchUriAsync(new Uri($"https://cotgopt.com/?map={str}",UriKind.Absolute));
				});
			}
		}

		internal async Task<Classification> ClassifyIfNeeded(Action onComplete=null)
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
				return (await Classify());
			}
		}

		internal void QueueClassify(bool isIncomingAttack, Action onComplete = null)
		{
			if (isNotClassified)
			{
				Classify(isIncomingAttack);
			}
		}

		Classification TagsToClassification()
		{

			if (HasTag(Tags.Scorp))
				return Classification.se;
			else if (HasTag(Tags.Prae))
				return Classification.praetor;
			else if (HasTag(Tags.Priest))
				return Classification.priestess;
			else if (HasTag(Tags.RT) || HasTag(Tags.VT) || HasTag(Tags.VRT))
				return Classification.rt;
			// Must come after VT and VRT
			else if (HasTag(Tags.Vanq))
				return Classification.vanqs;
			else if (HasTag(Tags.Sorc))
				return Classification.sorcs;
			else if (HasTag(Tags.Druid))
				return Classification.druids;
			else if (HasTag(Tags.Horse))
				return Classification.horses;
			else if (HasTag(Tags.Arb))
				return Classification.arbs;
			else if (HasTag(Tags.Stinger))
				return Classification.stingers;
			else if (HasTag(Tags.Galley) || HasTag(Tags.Warship))
				return Classification.navy;
			else if (HasTag(Tags.Hub))
				return Classification.hub;
			else if (HasTag(Tags.Storage))
				return Classification.hub;
			else if (tags != 0)
				return Classification.misc;
			return Classification.unknown;
		}

		public async ValueTask<Classification> ClassifyEx(bool isIncomingAttack)
		{
			var rv = await Classify(isIncomingAttack);
			if (hasAcademy.HasValue || (!Alliance.wantsIntel && !isMine) )
				return rv;
			return await ClassifyFromBuildings(isIncomingAttack);
		}

		private async ValueTask<Classification> ClassifyFromBuildings(bool isIncomingAttack)
		{
			if(!Alliance.wantsIntel && !isMine)
			{
				// todo
				//return;

			}

			var str = await Post.SendForText("includes/gLay.php", $"cid={cid}", World.CidToPlayerOrMe(cid));
			if (str.IsNullOrEmpty())
			{
				return Classification.missing;
			}

			byte stables = 0;
			byte academies = 0;
			byte training = 0;
			byte sorc = 0;
			byte se = 0;
			byte shipyards = 0;
			byte ports = 0;
			byte forums = 0;
			bool castle = false;
			var classification = Classification.missing;

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
				hasAcademy = academies > 0;
				var mx = stables.Max(academies).Max(training.Max(sorc)).Max(academies.Max(training)).Max(se).Max(shipyards).Max(forums).Max(ports);
				if (mx <= 4)
				{
					classification = Classification.misc;
				}
				else if (mx == stables)
				{
					if (se > 0 /*|| academies > 0*/ || (stables < 32) || isIncomingAttack)
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
					if (se > 0 /*|| academies > 0*/ || (training < 28 && training >= 26) || (training < 22) || isIncomingAttack)
						classification = Classification.vanqs;
					else
						classification = Classification.rt;

				}
				else if (mx == academies)
				{
					if ((academies == 34) || (academies == 25) || (academies == 19))
						classification = Classification.priestess; // todo!
					else
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
		static public bool TryConvertTroopTypeToClassification(byte ttype, out Classification classification)
		{
			var i = classificationTTs.FindIndex((byte)ttype);
			if(i!= -1)
			{
				classification = (Classification)i;
				return true;
			}
			classification = Classification.unknown;
			return false;
		}

		internal bool TouchClassification()
		{
			if (classification != Classification.unknown)
				return true;
			if (isFriend)
			{
				classification = TagsToClassification();
				if (classification != Classification.unknown)
					return true;

				foreach (var tt in troopsTotal.GetListDescending())
				{
					// some have two classifications
					var type = tt.type switch { ttTriari => ttRanger, ttRam => ttScorpion, var a => a };

					var i = classificationTTs.FindIndex((byte)type);
					if (i != -1)
					{
						classification = (Classification)i;
						return true;
					}
				}
			}
			if (!Player.isSpecial && !Alliance.wantsIntel)
			{
				classification = Classification.missing;
				return true;
			}
			return false;
		}

		internal async ValueTask<Classification> Classify(bool isIncomingAttack = false)
		{
			if (TouchClassification())
				return classification;

			//ClassificationExtended rv = new ClassificationExtended(); ;
			if (classification == Classification.unknown)
				classification = Classification.pending;

			return (await ClassifyFromBuildings(isIncomingAttack));

		}


		// Incoming attacks
		public Army[] incoming { get; set; } = Army.empty;
		[Flags]
		public enum OutGoing : byte
		{ 
			none=0,
			scheduled=1,
			sending=2,
			sieging=4,
		}
		public OutGoing outGoing;
		public TroopTypeCounts troopsHome;
		public TroopTypeCounts troopsTotal;


		//public int raidTroopCount
		//{
		//	get
		//	{

		//		int rv = 0;
		//		for (var type = 0; type < ttCount; ++type)
		//		{
		//			var tsHome = troopsHome[type];
		//			if (!includeRaiders[type])
		//				continue;
		//			rv += tsHome * ttToTs[type];
		//		}
		//		return rv;
		//	}
		//}


		public bool hasEnemyIncoming => incoming.Any(w => w.isAttack && !Alliance.IsAllyOrNap(w.sourceAlliance));
		public bool HasIncomingFrom(int pid) => incoming.Any(i => i.sPid == pid);
			
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
		//public TroopTypeCounts combinedIncoming
		//{
		//	get
		//	{
		//		TroopTypeCounts rv = null;
		//		foreach (var i in incoming)
		//		{
		//			if (i.isAttack)
		//				rv = rv != null ? rv.Add(i.troops) : i.troops;
		//		}
		//		return rv;
		//	}
		//}


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
						rv += a.time.Format() + " " + a.Format() + ";";
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
			if (!troopsTotal.Any())
			{
				if (isFriend && _tsTotal == 0)
					return "No troops";

				return classificationString;
			}
			string rv = string.Empty;
			string sep = string.Empty;
			for (int tt = 0; tt < ttCount; ++tt)
			{
				var c = troopsTotal[tt];
				if (c > 0)
				{
					var ch = troopsHome[tt];
					rv += $"{sep}{ch:N0}/{c:N0} {Troops.ttNameWithCaps[tt]}";
					sep = separator;
				}
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

		public static List<int> GetSelectedForContextMenu(int cid=0, bool onlyIfShiftPressed = false, int ignoreCid = 0, bool onlyCities = true, bool onlyMine = false)
		{
			var cids = new List<int>();
			if (cid != 0)
				cids.Add(cid);

			if (!onlyIfShiftPressed || App.IsKeyPressedShift() || App.IsKeyPressedControl())
			{
				foreach (var sel in Spot.selected)
				{
					if (sel != cid && sel != ignoreCid && (!onlyCities || City.Get(sel).isCityOrCastle))
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

		public void SelectMe(bool showClick = false, VirtualKeyModifiers mod = VirtualKeyModifiers.Shift, bool scrollIntoView = true)
		{
			if (showClick || scrollIntoView)
				NavStack.Push(cid);
			SpotTab.AddToGrid(this, mod, true, scrollIntoView);
			if (showClick)
			{
				JSClient.ShowCity(cid, true);
			}
		}


		public static bool TryGetSelected(out System.Collections.IEnumerable selected)
		{

			if (MainPage.IsVisible())
				selected = MainPage.instance.cityGrid.SelectedItems;
			else if (BuildTab.IsVisible())
				selected = BuildTab.instance.cityGrid.SelectedItems;
			else
			{
				selected = null;
				return false;
			}

			return true;
		}

		public void ProcessSelection(VirtualKeyModifiers mod, bool forceSelect = false, bool scrollIntoView = true)
		{
			++SpotTab.silenceSelectionChanges;

			AppS.DispatchOnUIThreadLow(() =>
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
					SyncSelectionToUI(scrollIntoView, this);
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



		public static void SyncSelectionToUI(bool scrollIntoView, Spot focusSpot = null)
		{
			++SpotTab.silenceSelectionChanges;
			try
			{
				foreach (var gridX in UserTab.dataGrids)
				{

					var grid = gridX.Key;
					
					if (!gridX.Value?.isFocused == true)
						continue;

					if (grid.IsCityGrid())
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
							selected.SyncList(sel1, (cid, spot) => cid == ((Spot) spot).cid,
								(cid) => City.Get(cid) );
						}

						if ((scrollIntoView) && (sel1.Any() || focusSpot != null))
						{
							var current = focusSpot ?? (City.GetBuild().isSelected ? City.GetBuild() : null);
							if (current != null )
							{
								grid.CurrentItem = current;
							}

							var any = current ?? sel1.First();
							{
								var rowIndex = grid.ResolveToRowIndex(any);
								var columnIndex = grid.ResolveToStartColumnIndex();
								if (rowIndex >= 0)
									grid.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
							}
						}

						if (AttackTab.IsVisible() && focusSpot != null )
						{
							try
							{
								if ( AttackTab.attacks.Contains(focusSpot.cid)
								     && !AttackTab.instance.attackGrid.SelectedItems.Contains(focusSpot) )
								{
									AttackTab.instance.attackGrid.SelectedItem = focusSpot as City;
									AttackTab.instance.attackGrid.ScrollIntoView(focusSpot, null);
								}

								if (AttackTab.targets.Contains(focusSpot.cid)
								    && !AttackTab.instance.targetGrid.SelectedItems.Contains(focusSpot))
								{
									AttackTab.instance.targetGrid.SelectedItem = focusSpot as City;
									AttackTab.instance.targetGrid.ScrollIntoView(focusSpot, null);
								}
							}
							catch
							{
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogEx(ex);
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
		public static bool IsSelectedOrHovered(int cid, bool noneIsAll = false)
		{
			// if nothing is selected we treat it as if everything is selected
			return (noneIsAll && selected.Count == 0) ? true : (cid == viewHover || selected.Contains(cid) || City.IsFocus(cid));
		}
		public static bool IsSelectedOrHovered(int cid0, int cid1, bool noneIsAll = false)
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

		
		//public void ShowCity(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
		//{
		//	JSClient.CitySwitch(cid, false);

		//}

		public string GetDebuggerDisplay()
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

		public static void SetFocus(int cid, bool scrollintoView, bool select = true, bool bringIntoView = true, bool lazyMove = true)
		{
			World.UpdateRegionInfo(cid);
			var changed = cid != focus;
			var spot = Spot.GetOrAdd(cid);
			if (select)
				spot.SelectMe(false, App.keyModifiers, scrollintoView);
			if (changed)
			{
				focus = cid;
				AppS.QueueOnUIThread(UpdateFocusText);
			}
			if (bringIntoView)
				cid.BringCidIntoWorldView(lazyMove);
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
					if (await AppS.DoYesNoBox("Busy", "Please wait for current operation to complete") != 1)
					{
						return false;
					}
				}
			}
			return true;
		}

		

		public void ReturnSlowClick()
		{
			Raiding.ReturnSlow(cid, true);
		}
		public void ReturnAt(object sender, RoutedEventArgs e)
		{
			ShowReturnAt(true);
		}

		public async Task ShowReturnAt(bool wantDialog)
		{




			//if (!IsBuild(cid))
			{
				await App.DispatchOnUIThreadExclusive(cid, async () =>
				{

					try
					{
						DateTimeOffset? time = null;
						var ogaStr = await JSClient.view.ExecuteScriptAsync("getOGA()");
						using var jsDoc = JsonDocument.Parse(ogaStr.Replace("\\\"","\"").Trim('"'));
						foreach (var i in jsDoc.RootElement.EnumerateArray())
						{
							try
							{
								var type = i[0].GetAsInt();
								if (type == 5) // raid
									continue;
								Trace(type);
								var timing = i[6].GetAsString();
								var id = timing.IndexOf("Departs:");
								if (id == -1)
									continue;
								timing = timing.Substring(id + 9);
								var t = CnVServer.ServerTime();
								;
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
									t = timing.ParseDateTime(true);
								}

								t -= TimeSpan.FromHours(SettingsPage.returnRaidsBias);
								if (time == null || time > t)
									time = t;
							}
							catch (Exception ex)
							{
								LogEx(ex);
							}
						}


						if (wantDialog)
						{
							(time, var okay) = await DateTimePicker.ShowAsync("Return By:", time);
							if (!okay)
								return; // aborted
						}

						if (time != null)
						{
							await Raiding.ReturnAt(cid, time.Value);
							Note.Show($"{City.Get(cid).nameMarkdown} end raids at {time.Value.Format()}");
						}
						else
						{
							Note.Show($"{City.Get(cid).nameMarkdown} no scheduled outgoing");
						}
					}
					catch (Exception ex)
					{
						LogEx(ex);
					}
				});

			}
		}

		public bool isHubOrStorage => HasTag(Tags.Hub) || HasTag(Tags.Storage);
		public bool isHub => HasTag(Tags.Hub);
		public bool isStorage => HasTag(Tags.Storage);
		public bool is7Point => HasTag(Tags.SevenPoint);

		public async void ReturnAtBatch(object sender, RoutedEventArgs e)
		{
			(var at, var okay) = await Views.DateTimePicker.ShowAsync("Return By:");
			if (!okay)
				return; // aborted
			using var work = new WorkScope("Return At..");

			var cids = Spot.GetSelectedForContextMenu(cid);
			foreach (var _cid in cids)
			{
				var __cid = _cid;
				await Raiding.ReturnAt(__cid, at);
			}
			Note.Show($"End {cids.Count} raids at {at.Format()} ");

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
				AppS.DispatchOnUIThreadLow(() =>
			   {
				   var dialog = new ContentDialog()
				   {
					   Title = "Spot has Changed",
					   Content = cid.CidToString(),
					   PrimaryButtonText = "Okay"
				   };
		   //SettingsPage.BoostVolume();
		   ElementSoundPlayer.Play(ElementSoundKind.Invoke);
				   ToastNotificationsService.instance.SpotChanged($"{cid.CidToString()} has changed");
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
			await Post.Get("overview/rcallall.php", "a=" + cid, World.CidToPlayerOrMe(cid));
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
			JSClient.view.ExecuteScriptAsync($"{func}({cid % 65536},{cid >> 16})");
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
			var sel = GetSelectedForContextMenu(0, false, cid, true);
			var _cid = (sel.Count == 1) ? sel[0] : City.build;


			// todo cart travel time, ship travel time
			var dist = cid.DistanceToCidD(_cid);
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
				var dt = TimeSpan.FromSeconds(dist * TTTravel(i));
				if (IsTTNaval(i))
					dt += TimeSpan.FromHours(1.0);
				sb.Append($"\n{ttName[i]}: {dt.ToString(AUtil.defaultTimeSpanFormat)}");
			}
			var str = sb.ToString();
			App.CopyTextToClipboard(str);
			Note.Show(str, timeout: 20 * 1000);
		}
		public bool canVisit => isFriend;


		public string nameMarkdown => $"[{nameAndRemarks}](/c/{cid.CidToString()})";
		public Tags tags;
		public bool isMilitary => tags.MilitaryTroops()!=0;
		public bool wantTriari => tags.HasTroopTypeTag(ttTriari);
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

		public void AddToFlyout(MenuFlyout flyout,bool useSelected=false )
		{
			var me = this as City;
			var aMisc = flyout.AddSubMenu("Misc..");
			var aExport = flyout.AddSubMenu("Import/Export..");
			var aSetup = AApp.AddSubMenu(flyout,"Setup..");
			var aWar = AApp.AddSubMenu(flyout,"War..");
			if(this.isCityOrCastle)
			{
				// Look - its my city!
				if(this.isFriend)
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
					var aRaid = AApp.AddSubMenu(flyout,"Raid..");
					aRaid.AddItem($"Raid ..",() => ScanDungeons.Post(cid,true,false));

					int count = 1;
					if(useSelected)
					{
						count = MainPage.GetContextCidCount(cid);
					}
					if(count > 1)
					{
						aRaid.AddItem($"End Raids x{count} selected",MainPage.ReturnSlowClick,cid);
						aRaid.AddItem($"Return At...x{count}",this.ReturnAtBatch);

					}
					else
					{

						aRaid.AddItem("End Raids",this.ReturnSlowClick);
						aRaid.AddItem("Return At...",this.ReturnAt);
					}


					aSetup.AddItem("Setup...",(_,_) => Spot.InfoClick(cid));
					aSetup.AddItem("Find Hub",(_,_) => CitySettings.SetClosestHub(cid));
					aSetup.AddItem("Set Recruit",(_,_) => CitySettings.SetRecruitFromTag(cid));
					aSetup.AddItem("Change...",(_,_) => ShareString.Show(cid,default));
					aSetup.AddItem("Move Stuff",(_,_) => me.MoveStuffLocked());
					//aSetup.AddItem("Remove Castle", (_, _) => 
					//{
					//	CityBuild.

					//}
					//   AApp.AddItem(flyout, "Clear Res", (_, _) => JSClient.ClearCenterRes(cid) );
					aSetup.AddItem("Clear Res",me.ClearRes);


					aExport.AddItem("Troops to Sheets",CopyForSheets);
				}
				else
				{
					if(_cityName == null)
					{
						JSClient.FetchCity(cid);
					}

				}
				{
					var sel = Spot.GetSelectedForContextMenu(cid,false);
					{
						var multiString = sel.Count > 1 ? $" _x {sel.Count} selected" : "";
						aWar.AddItem("Cancel Attacks..",CancelAttacks);
						var afly = aWar.AddSubMenu("Attack Planner");
						if(!Alliance.IsAllyOrNap(this.allianceId))
						{
							afly.AddItem("Ignore Player" + multiString,(_,_) => AttackTab.IgnorePlayer(cid.CidToPid()));
						}
						afly.AddItem("Add as Target" + multiString, (_, _) => AttackTab.AddTarget(sel));
							afly.AddItem("Add as Attacker" + multiString,(_,_) =>
							{
								using var work = new WorkScope("Add as attackers..");

								string s = string.Empty;
								foreach(var id in sel)
								{
									s = s + id.CidToString() + "\t";
								}
								AttackTab.AddAttacksFromString(s,false);
								Note.Show($"Added attacker {s}");

							});
						
					}
					//else
					if(!Alliance.IsAllyOrNap(this.allianceId))
					{
						aWar.AddItem("Add funky Attack String",async (_,_) =>
						{
							using var work = new WorkScope("Add to attack string..");

							foreach(var id in sel)
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
				//if (cid != City.build)
				{
					aSetup.AddItem("Set target hub",(_,_) => CitySettings.SetTargetHub(City.build,cid));
					aSetup.AddItem("Set source hub",(_,_) => CitySettings.SetSourceHub(City.build,cid));
					//if(Player.myName == "Avatar")
					//    AApp.AddItem(flyout, "Set target hub I", (_, _) => CitySettings.SetOtherHubSettings(City.build, cid));
				}


				aWar.AddItem("Attack",(_,_) => Spot.JSAttack(cid));
				aWar.AddItem("Near Defence",DefendMe);
				if(incoming.Any())
					aWar.AddItem("Incoming",ShowIncoming);

				//	if (Raid.test)
				aWar.AddItem("Recruit Sen",(_,_) => Recruit.Send(cid,ttSenator,1,true));
				aWar.AddItem("Send Defence",(_,_) => JSDefend(cid));
				aWar.AddItem("Show Reinforcements",(_,_) => Reinforcement.ShowReinforcements(cid,null));
				aWar.AddItem("Show All Reinforcements",(_,_) => Reinforcement.ShowReinforcements(0,null));
				aExport.AddItem("Defense Sheet",ExportToDefenseSheet);
				AApp.AddItem(flyout,"Send Res",(_,_) => Spot.JSSendRes(cid));
				AApp.AddItem(flyout,"Near Res",ShowNearRes);
				if(isFriend)
				{
					AApp.AddItem(flyout,"Do the stuff",(_,_) => DoTheStuff());
					AApp.AddItem(flyout,"Food Warnings",(_,_) => CitySettings.SetFoodWarnings(cid));
					flyout.AddItem("Ministers",me.ministersOn.IsTrueOrNull,(this as City).SetMinistersOn);
				}
			}
			else if(this.isDungeon || this.isBoss)
			{
				AApp.AddItem(flyout,"Raid",(_,_) => Spot.JSRaid(cid));

			}
			else if(this.isEmpty && DGame.isValidForIncomingNotes)
			{
				AApp.AddItem(flyout,"Claim",this.DiscordClaim);

			}
			aMisc.AddItem("Notify on Decay",DecayQuery);
			if(Raid.test)
			{
				aMisc.AddItem("Settle whenever water",(_,_) => TrySettle(City.build,cid,true));
				aMisc.AddItem("Settle whenever land",(_,_) => TrySettle(City.build,cid,false));
			}
			aMisc.AddItem("Distance",(_,_) => ShowDistanceTo());
			aMisc.AddItem("Select",(_,_) => SelectMe(true,App.keyModifiers));
			aMisc.AddItem("Coords to Chat",() => CoordsToChat(cid));
			flyout.RemoveEmpy();
		}
		public void ShowContextMenu(UIElement uie, Windows.Foundation.Point position)
		{

			//   SelectMe(false) ;
			var me = this as City;
			var flyout = new MenuFlyout();
			AddToFlyout(flyout,uie == MainPage.CityGrid || uie == BuildTab.CityGrid );
			flyout.CopyXamlRootFrom(uie);

			//   flyout.XamlRoot = uie.XamlRoot;
			flyout.ShowAt(uie, position);
		}

		private async void CancelAttacks(object sender, RoutedEventArgs e)
		{
			var ogaStr = await JSClient.view.ExecuteScriptAsync("getOGA()");
			using var js = JsonDocument.Parse(ogaStr.Replace("\\\"","\"").Trim('"'));
			
	
			foreach(var (i,o) in new JsonArrayEnumerator(js.RootElement) )
			{
				if(o[2].TryGetInt64(out var xx))
				{
					await Post.CancelAttack(cid,xx);


				}

			}
		}

		public static async void CoordsToChat(int _cid)
		{
			var targets = Spot.GetSelectedForContextMenu(_cid, false, onlyMine: false, onlyCities: false);
			StringBuilder sb = new ();
			var first = true;
			foreach (var cid in targets)
			{
				if (first)
					first = false;
				else
					sb.Append('\t');
				sb.Append(cid.CidToCoords());
			}
			var str = sb.ToString();
			App.CopyTextToClipboard(str);
			ChatTab.PasteToChatInput(str);
		}
		public async Task DoTheStuff()
		{
			await App.DispatchOnUIThreadExclusive(cid, async () =>
			 {
				 await CnV.DoTheStuff.Go(this as City, true, true);
			 });
		}
		public static async void InfoClick(int _cid)
		{
			//	var cids = MainPage.GetContextCids(sender);
			//	foreach (var cid in cids)
			//	{
			//		var _cid = cid;
				   await ShareString.Show(_cid);

			//	{
			//		break;
			//	}
			//	}
		}
		public void DefendMe()
		{
			var cids = GetSelectedForContextMenu(cid,false);

			NearDefenseTab.defendants.Set(cids.Select(a=>City.Get(a)),true);

			var tab = NearDefenseTab.instance;
			tab.ShowOrAdd(true);
			tab.refresh.Go();
		}


		public void ShowNearRes()
		{
			var tab = NearRes.instance;
			tab.target = (City)this;
			if (!tab.isOpen)
			{
				tab.ShowOrAdd(true);
			}
			else
			{
				if (!tab.isFocused)
					TabPage.Show(tab);
				else
					tab.refresh.Go();
			}
		}
		public void BuildStageDirty()
		{
			OnPropertyChanged(nameof(City.bStage));
		}
		public async void ShowIncoming()
		{
			if (allianceId == Alliance.myId)
			{
				var tab = IncomingTab.instance;
				AppS.DispatchOnUIThread(() => tab.Show());
				for (; ; )
				{
					await Task.Delay(1000);
					if (tab.defenderGrid.ItemsSource != null)
						break;
				}
				AppS.DispatchOnUIThreadIdle(() =>
				{
					tab.defenderGrid.SelectedItem=(this);
					tab.defenderGrid.ScrollItemIntoView(this);
				});

			}
			else
			{
				var tab = OutgoingTab.instance;
				AppS.DispatchOnUIThread(() => tab.Show());
				for (; ; )
				{
					await Task.Delay(1000);
					if (tab.attackerGrid.ItemsSource != null)
						break;
				}
				AppS.DispatchOnUIThreadIdle(() =>
				{
					tab.attackerGrid.SelectedItem=(this);
					tab.attackerGrid.ScrollItemIntoView(this);
				});
			}
		}



		public async void DiscordClaim()
		{
			if (!DGame.isValidForIncomingNotes)
			{
				Log("Invalid");
				return;
			}
			try
			{
				Note.Show($"Registering claim on {xy}");
				var client = JSClient.genericClient;


				var message = new DGame.Message() { username = "Cord Claim", content = $"{xy} claimed by {Player.myName}", avatar_url = "" };

				//var content =  JsonContent.Create(message);
				//, JSON.jsonSerializerOptions), Encoding.UTF8,
					//	   "application/json");

				var result = await client.PostAsJsonAsync(DGame.discordHook, message);
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
				sb.Append(classificationString);
				sb.Append('\t');
				sb.Append(s.tsTotal);
				sb.Append('\t');
				sb.Append(s.hasAcademy.GetValueOrDefault() ? "Academy\t" : "None\t");
				sb.Append(s.xy + "\n");

			}
			App.CopyTextToClipboard(sb.ToString());
			Note.Show($"Copied {counter} castles to clipboard for sheets");
		}

		public static void ScrollIntoView(int cid)
		{
			//         await Task.Delay(2000);
			//          instance.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () =>
			//           {
			//   await Task.Delay(200);
			AppS.QueueOnUIThread(() =>
			{

				{
			/// MainPage.CityGrid.SelectedItem = this;
			//                      MainPage.CityGrid.SetCurrentItem(this);

			//     MainPage.CityGrid.SetCurrentItem(this,false);
					if (MainPage.IsVisible())
						MainPage.CityGrid.ScrollItemIntoView(City.GetOrAdd(cid));
					if (BuildTab.IsVisible())
						BuildTab.CityGrid.CurrentItem = (City.GetOrAdd(cid));
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

		struct sndnc
		{
			public int cid { get; set; }
			public int rcid { get; set; }
			public string tr { get; set; }
			public int type { get; set; }
			public int snd { get; set; }
		}

		public static HashSet<(int cid, int rcid)> settles = new();
		public async static void TrySettle(int cid, int target, bool bySea)
		{
			var _cid = cid;
			var _pid = City.Get(_cid).pid;
			settles.Add((_cid, target));
			var rcid = target;
			for (; ; )
			{
				try
				{
					var sc = new sndnc() { cid = _cid, rcid = rcid, type = 5, snd = bySea ? 2 : 1, tr = @"[{'tt':17,'tv':1}]" };
					var magic = "Sx2xxresa" + _pid.ToString() + "sa2dT123ol";
					var txt = await Post.SendEncryptedForText("includes/sndNC.php",JsonSerializer.Serialize(sc,JSON.jsonSerializerOptions),magic,_pid,false);
					var tr = txt.Trim();
					if (tr.Length > 0 && tr[0] == '{')
					{
						await AppS.DoYesNoBox("Something sent", "or someting");
						try
						{
							settles.Remove((_cid, rcid));
						}
						catch (Exception ex1)
						{

						}
						return;
					}
					else
					{
						Log(txt); // some random error
					}

				}
				catch (Exception ex)
				{
					Log(ex);
				}

				await Task.Delay(60 * 1000); // 60 seconds

			}

		}

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
	//	public static string CellText(this DataGridCellInfo cell) => (cell?.Column as DataGridTypedColumn)?.PropertyName ?? string.Empty;
		public static Windows.Foundation.Point Show(this Microsoft.UI.Xaml.Controls.Flyout me,Windows.Foundation.Point sc,UIElement element)
		{
			me.CopyXamlRootFrom(element);
			me.ShowAt(element,new FlyoutShowOptions() { Position = new Windows.Foundation.Point(sc.X,sc.Y),Placement = FlyoutPlacementMode.Auto,ShowMode=FlyoutShowMode.TransientWithDismissOnPointerMoveAway });
			return sc;
		}
	}
}
