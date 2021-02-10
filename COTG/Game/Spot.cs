﻿using COTG.Helpers;
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
using Windows.UI.Xaml.Controls;
using COTG.JSON;
using System.Text.Json;
using Windows.Web.Http;
using static COTG.Game.Enum;
using Windows.ApplicationModel.DataTransfer;
using static COTG.Views.ShellPage;

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
		public static City pending = new City() { _cityName = "pending" };

		//public static City[] emptySpotSource = new[] { pending };

		public string nameAndRemarks => remarks.IsNullOrEmpty() ? _cityName : $"{_cityName} - {remarks}";
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
				rv.type = (byte)(info.type >> 28);
				if (info.type == 0)
				{
					Log("Uninitialized");
				}
				rv.isTemple = info.isTemple;
				rv.isOnWater = info.isWater;
				rv.isCastle = info.isCastle;
				rv.points = (ushort)(info.isBig ? 8000 : 1500);

				Spot.allSpots.TryAdd(cid, rv);
				if (Player.IsFriend(info.player))
					City.CitiesChanged();
			}
			if (cityName != null)
				rv._cityName = cityName;
			return (rv);
		}
		public static void UpdateName(int cid, string name) => GetOrAdd(cid, name);

		internal bool HasTag(Tags tag)
		{
			return remarks.Contains(tag.ToString(), StringComparison.OrdinalIgnoreCase);
		}

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

		public int tsTotal => (troopsTotal.Any() ? troopsTotal.TS() : _tsTotal);
		public int tsDefMax { get { var i = incomingDefTS; return (i > 0) ? i : (reinforcementsIn.TS() + (troopsHome.Any()?troopsHome.TSDef() :_tsHome)); } }
		public int tsOff { get { var i = incomingOffTS; return (i > 0) ? i : troopsHome.TSOff(); } }

		public Reinforcement[] reinforcementsIn = Array.Empty<Reinforcement>();
		public Reinforcement[] reinforcementsOut = Array.Empty<Reinforcement>();


		public int pid { get; set; }
		public string player => Player.Get(pid).name;
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
		public int attackCluster { get; set; } // For attackTab, 


		public AttackType attackType { get; set; }
		public bool isAttackTypeAssault => attackType == AttackType.assault;
		public bool isAttackTypeSenator => attackType == AttackType.senator;
		public bool isAttackTypeSenatorFake => attackType == AttackType.senatorFake;
		public bool isAttackTypeSE => attackType == AttackType.se;
		public bool isAttackTypeSEFake => attackType == AttackType.seFake;
		public bool isAttackTypeReal => attackType == AttackType.se || attackType == AttackType.senator;
		public bool isAttackTypeFake => attackType == AttackType.seFake || attackType == AttackType.senatorFake;

		public const int attackClusterNone = -1;


		public bool isAttackClusterNone => attackCluster == attackClusterNone;

		public enum Classification : byte
		{
			unknown,
			vanqs,
			rt,
			sorcs,
			druids,
			academy,
			horses,
			arbs,
			se,
			hub,
			navy,
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
			static public explicit operator Classification(ClassificationExtended e) => e.classification;
		};
		public Classification classification { get; set; }
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
			"academy",
			"horses",
			"arbs",
			"scorps",
			"hub",
			"navy",
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
			ttHorseman,
			ttArbalist,
			ttScorpion,
			ttSenator,
			ttWarship,
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
					sb.Append(s.player);
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




		public static void ProcessPointerPress(UserTab tab, object sender, PointerRoutedEventArgs e)
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

		public static void ClearHover()
		{
			ShellPage.ClearHover();
		}

		public byte primaryTroopType => GetPrimaryTroopType();
		public byte GetPrimaryTroopType(bool onlyHomeTroops = false)
		{
			var troops= (onlyHomeTroops ? troopsHome : troopsTotal);
			if(troops.IsNullOrEmpty())
				return (byte)classificationTT;
			
			byte best = 0; // if no raiding troops we return guards 
			var bestTS = 0;
			foreach (var ttc in troops)
			{
				var type = ttc.type;
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


		public void ProcessClick(string column, PointerPoint pt, UIElement uie, VirtualKeyModifiers modifiers)
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
					case nameof(xy):
						ProcessCoordClick(cid, false, modifiers, false);
						wantRaidScan = false;
						break;
					case nameof(icon):
						if (City.CanVisit(cid))
						{
							var wasBuild = City.IsBuild(cid);
							JSClient.ChangeCity(cid, false, true, false);
							if (wasBuild)
							{
								JSClient.ChangeView(!ShellPage.IsCityView());

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
							ToggleDungeons(uie as RadDataGrid, false, false);
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
				if (!modifiers.IsShift())
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
			SpotTab.TouchSpot(cid, modifiers);
		}

		public async void ToggleDungeons(RadDataGrid uie, bool forceClose = false, bool forceOpen = false)
		{
			if ((forceClose || MainPage.expandedCity == this) && !forceOpen)
			{
				if (MainPage.expandedCity != null)
					(uie).HideRowDetailsForItem(MainPage.expandedCity);
				MainPage.expandedCity = null;
			}
			else
			{
				MainPage.expandedCity = this as City;
				await ScanDungeons.Post(cid, true, false);
				(uie).ShowRowDetailsForItem(this);

			}
		}

		public string ToTsv()
		{
			return $"{cid.CidToCoords()}\t{this.player}\t{this._cityName ?? ""}\t{this.remarks ?? ""}\t{this.alliance}\t{this.isCastle}\t{this.isOnWater}";
		}

		public static async void ProcessCoordClick(int cid, bool lazyMove, VirtualKeyModifiers mod, bool scrollIntoUI = false)
		{
			mod.UpdateKeyModifiers();

			if (City.CanVisit(cid) && !mod.IsShiftOrControl())
			{
				if (City.IsBuild(cid))
				{
					JSClient.ChangeView(!ShellPage.IsCityView());// toggle between city/region view

					cid.BringCidIntoWorldView(lazyMove);
				}
				else
				{
					JSClient.ChangeCity(cid, lazyMove, false, scrollIntoUI); // keep current view, switch to city
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

			if (mod.IsShiftAndControl() && Player.isAvatarOrTest)
			{
				//     var spot = Spot.GetOrAdd(cid);
				//     GetCity.Post(cid, spot.pid, (js, city) => Log(js));

				var str = await Post.SendForText("includes/gLay.php", $"cid={cid}", World.CidToPlayer(cid));
				Log(str);

				App.DispatchOnUIThreadSneaky(() =>
				{
					if (World.GetInfoFromCid(cid).isWater)


						// set is water var
						str = $"[ShareString.1.3]{(World.GetInfoFromCid(cid).isWater ? ';' : ':')}{str.Substring(18)}";
					App.CopyTextToClipboard(str);

				//	Launcher.LaunchUriAsync(new Uri($"http://cotgopt.com/?map={str}"));
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
				return (await Classify()).classification;
			}
		}
		internal void QueueClassify()
		{
			if (isNotClassified)
			{
				Classify();
			}
		}
		internal async ValueTask<ClassificationExtended> Classify()
		{
			if (!Discord.isValid && !Player.isAvatarOrTest)
			{
				classification = Classification.missing;
				return new ClassificationExtended() { classification = classification };
			}
			classification = Classification.pending;
			var str = await Post.SendForText("includes/gLay.php", $"cid={cid}", World.CidToPlayer(cid));
			ClassificationExtended rv = new ClassificationExtended(); ;
			try
			{
				const int start = 14;
				const int end = 459;

				for (int i = start; i < end; ++i)
				{
					switch (str[i])
					{
						case 'E': ++rv.stables; break;
						case 'Y': ++rv.se; break;
						case 'J': ++rv.sorc; break;
						case 'G': ++rv.training; break;
						case 'Z': ++rv.academies; break;
						case 'X': rv.castle = true; break;
						case 'R': ++rv.ports; break;
						case 'V': ++rv.shipyards; break;
						case 'P': ++rv.forums; break;


					}
				}
				var mx = rv.stables.Max(rv.academies).Max(rv.training.Max(rv.sorc)).Max(rv.academies.Max(rv.training)).Max(rv.se).Max(rv.shipyards).Max(rv.forums).Max(rv.ports);
				if (mx <= 4)
				{
					classification = Classification.misc;
				}
				else if (mx == rv.stables)
				{
					if (rv.se > 0 || rv.academies > 0 || (rv.stables != 25 && rv.stables == 22))
						classification = Classification.horses;
					else
					{
						classification = Classification.arbs;
					}
				}
				else if (mx == rv.sorc)
				{
					if (rv.sorc == 45 || rv.sorc == 40 || rv.sorc == 29 || rv.sorc == 27)
						classification = Classification.druids;
					else
						classification = Classification.sorcs;
				}
				else if (mx == rv.training)
				{
					if (rv.se > 0 || rv.academies > 0 || rv.training == 26 || rv.training == 27 || rv.training <= 18)
						classification = Classification.vanqs;
					else
						classification = Classification.rt;

				}
				else if (mx == rv.academies)
				{
					classification = Classification.academy;
				}
				else if (mx == rv.se)
				{
					classification = Classification.se;
				}
				else if (mx == rv.shipyards)
				{
					classification = Classification.navy;
				}
				else if (mx == rv.forums || mx == rv.ports)
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
				Log(e);
			}
			rv.classification = classification;
			return rv;


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
						rv = rv != null ? rv.Sum(i.troops) : i.troops;
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
		public string troopsString
		{
			get
			{
				if(troopsTotal.IsNullOrEmpty())
					return classificationString;

				string rv = string.Empty;
				string sep = string.Empty;
				foreach (var ttc in troopsTotal)
				{
					rv += $"{sep}{troopsHome.Count(ttc.type):N0}/{ttc.count:N0} {Enum.ttNameWithCaps[ttc.type]}";
					sep = ", ";
				}
				return rv;
			}
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

		public static List<int> GetSelectedForContextMenu(int cid, bool onlyIfShiftPressed = true)
		{
			var cids = new List<int>();
			if (cid != 0)
				cids.Add(cid);

			if (!onlyIfShiftPressed || App.IsKeyPressedShift() || App.IsKeyPressedControl())
			{
				foreach (var sel in Spot.selected.ToArray())
				{
					if (sel != cid)
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
				Log(ex);
				return new HashSet<int>(); // if might be corrupt
			}
			return rv;

		}
		public void SelectMe(bool showClick, VirtualKeyModifiers mod, bool scrollIntoView = true)
		{
			NavStack.Push(cid);
			SpotTab.AddToGrid(this, mod, true, scrollIntoView);
			if (showClick)
			{
				JSClient.ShowCity(cid, true);
			}
		}

		public void ProcessSelection(VirtualKeyModifiers mod, bool forceSelect = false, bool scrollIntoView = true)
		{
			++SpotTab.silenceSelectionChanges;

			App.DispatchOnUIThreadSneaky(() =>
			{
				try
				{
					var sel0 = SpotTab.instance.selectedGrid.SelectedItems;
					var sel1 = MainPage.instance.cityGrid.SelectedItems;
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
								sel0.Remove(this);
								sel1.Remove(this);

							}
							else
							{
								wantUISync = true;
							}
						}
						else
						{
							var newSel = new HashSet<int>(sel);
							newSel.Add(cid);
							selected = newSel;

							sel0.Add(this);
							sel1.Add(this);
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

							sel0.Clear();
							sel0.Add(this);

							sel1.Clear();
							sel1.Add(this);
						}
						//                   SpotTab.SelectOne(this);
					}
					if (wantUISync && scrollIntoView)
						SelectInUI(true);
				}
				catch (Exception e)
				{
					Log(e);
				}
				finally
				{
					--SpotTab.silenceSelectionChanges;
				}
			});
			//    SpotTab.SelectedToGrid();
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
		public bool isSelected => selected.Contains(cid);
		public static int viewHover; // in the view menu

		//        public static string uiHoverColumn = string.Empty;
		//	public static int uiPress; //  set when pointerPressed is recieved, at this point a contect menu might come up, causing us to lose uiHover
		public static string uiPressColumn = string.Empty;

		readonly static int[] pointSizes = { 1000, 6000 };

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

		public void ShowCity(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			JSClient.ChangeCity(cid, false);

		}

		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		public override string ToString()
		{
			return $"{{{cid},{cityName}, {xy},{player},{tsHome.ToString()}ts}}";
		}
		public void SetFocus(bool scrollIntoView, bool select = true, bool bringIntoWorldView = true, bool lazyMove=true)
		{
			SetFocus(cid, scrollIntoView, select, bringIntoWorldView,lazyMove);
		}
		public static void SetFocus(int cid, bool scrollintoView, bool select = true, bool bringIntoWorldView = true,bool lazyMove = true)
		{
			var changed = cid != focus;
			var spot = Spot.GetOrAdd(cid);
			if (select)
				spot.SelectMe(false, App.keyModifiers, scrollintoView);
			if (changed)
			{
				focus = cid;
				App.DispatchOnUIThreadSneaky(() =>
				{
					ShellPage.instance.focus.Content = spot.nameAndRemarks;
					ShellPage.instance.coords.Text = cid.CidToString();
					if (scrollintoView)
						spot.SelectInUI(true);

				}

				);
			}
			if (bringIntoWorldView)
				cid.BringCidIntoWorldView(lazyMove);
		}
		public static int build; // city that has Build selection.  I.e. in city view, the city you are in

		public bool isBuild => cid == build;
		public static bool IsBuild(int cid)
		{
			return build == cid;
		}
	
		public bool SetBuild(bool scrollIntoView, bool select = true)
		{
			var changed = cid != build;
			if (changed)
			{
				var _layout = CityBuild.isPlanner;
				if (_layout)
				{
					City.GetBuild().SaveBuildingsToLayout();
					CityBuild.isPlanner = false;
				}
				City.build = cid;
				Assert(pid == Player.activeId);
				Cosmos.PublishPlayerInfo(JSClient.jsBase.pid, City.build, JSClient.jsBase.token, JSClient.jsBase.cookies); // broadcast change

				foreach (var p in PlayerPresence.all)
				{
					if (p.pid != Player.myId && p.cid == cid)
					{
						Note.Show($"You have joined {p.name } in {p.cid.CidToStringMD()}");
					}
				}

				City.CitySwitched();
				if(_layout)
				{
					GetCity.Post( cid, (_,_)=>  CityBuild._isPlanner = true );
				}
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
			(var at, var okay) = await Views.DateTimePicker.ShowAsync("Return By:");
			if (!okay)
				return; // aborted

			await Raiding.ReturnAt(cid, at);
			Note.Show($"{cid.CidToStringMD()} end raids at {at.FormatDefault()}");
		}
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
			await Post.Send("overview/rcallall.php", "a=" + cid, World.CidToPlayer(cid));
			await Post.SendEncrypted("includes/UrOA.php", "{\"a\":" + cid + ",\"c\":0,\"b\":2}", "Rx3x5DdAxxerx3", World.CidToPlayer(cid));
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
		public void ShowDistanceTo(int _cid)
		{
			// todo cart travel time, ship travel time
			var dist = cid.DistanceToCid(_cid);
			StringBuilder sb = new StringBuilder();
			sb.Append(dist.ToString("0.00"));

			sb.Append($"\nCarts: {TimeSpan.FromMinutes(dist * cartTravel).ToString(AUtil.defaultTimeFormat)}, ");
			if (isOnWater && Spot.GetOrAdd(_cid).isOnWater)
			{
				sb.Append($"\nShips: {TimeSpan.FromMinutes(dist * shipTravel + 60).ToString(AUtil.defaultTimeFormat)}");
			}
			for (int i = 1; i < ttCount; ++i)
			{
				var dt = TimeSpan.FromMinutes(dist * TTTravel(i));
				sb.Append($"\n{ttName[i]}: {dt.ToString(AUtil.defaultTimeFormat)}");
			}
			var str = sb.ToString();
			App.CopyTextToClipboard(str);
			Note.Show(str, false, false, 20 * 1000);
		}
		public bool canVisit => isFriend;
		
		public static bool OnKeyDown(object _spot, VirtualKey key)
		{
			var spot = _spot as Spot;
			switch (key)
			{
				case VirtualKey.Enter:
					spot.SetFocus(false);
					return true;
					break;
				case VirtualKey.Left:
					{
						if (spot.canVisit)
							JSClient.ChangeCity(spot.cid, false);
						else
							spot.SetFocus(false);
						return true;
					}
				case VirtualKey.Right:
					return true;
				case VirtualKey.Space:
					{
						if (spot.canVisit)
							spot.ToggleDungeons(MainPage.CityGrid);
						else
							spot.SetFocus(false);
						return true;
						break;
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
					if (uie == MainPage.CityGrid || uie == NearDefenseTab.instance.supportGrid)
					{
						count = MainPage.GetContextCidCount(cid);
					}
					if (count > 1)
					{
						aRaid.AddItem( $"End Raids x{count} selected", MainPage.ReturnSlowClick, cid);
						aRaid.AddItem( $"Home Please x{count} selected", MainPage.ReturnFastClick, cid);
						aRaid.AddItem( $"Return At...x{count}", this.ReturnAtBatch);

					}
					else
					{

						aRaid.AddItem( "End Raids", this.ReturnSlowClick);
						aRaid.AddItem( "Home Please", this.ReturnFastClick);
						aRaid.AddItem( "Return At...", this.ReturnAt);
					}

					
					aSetup.AddItem("Setup", Spot.InfoClick, cid);
					aSetup.AddItem( "Set Hub", (_, _) => CitySettings.SetHub(cid));
					aSetup.AddItem( "Set Recruit", (_, _) => CitySettings.SetRecruitFromTag(cid));

					//   AApp.AddItem(flyout, "Clear Res", (_, _) => JSClient.ClearCenterRes(cid) );
					aSetup.AddItem("Clear Center Res", (_, _) => JSClient.ClearCenter(cid));


					aExport.AddItem("Troops to Sheets", CopyForSheets);
				}

				{
					var sel = Spot.GetSelectedForContextMenu(cid, false);
					if (AttackTab.instance.isActive)
					{
						var multiString = sel.Count > 1 ? $" _x {sel.Count} selected" : "";
						var afly = AApp.AddSubMenu(flyout, "Attack Planner");

						if (this.allianceId != Alliance.myId)
						{
							afly.AddItem("Add as Real Cap" + multiString, (_, _) => AttackTab.AddTarget(sel, AttackType.senator));
							afly.AddItem("Add as Fake Cap" + multiString, (_, _) => AttackTab.AddTarget(sel, AttackType.senatorFake));
							afly.AddItem("Add as Real SE" + multiString, (_, _) => AttackTab.AddTarget(sel, AttackType.se));
							afly.AddItem("Add as Fake SE" + multiString, (_, _) => AttackTab.AddTarget(sel, AttackType.seFake));
						}
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
					}
					else
					{
						aExport.AddItem("Add funky Attack String", async (_, _) =>
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
					//if(Player.myName == "Avatar")
					//    AApp.AddItem(flyout, "Set target hub I", (_, _) => CitySettings.SetOtherHubSettings(City.build, cid));
				}
				if (AttackTab.instance.isActive)
				{

				}

				aWar.AddItem( "Attack", (_, _) => Spot.JSAttack(cid));
				aWar.AddItem( "Near Defence", DefendMe);
				if (incoming.Any())
					aWar.AddItem( "Incoming", ShowIncoming);


				aWar.AddItem( "Send Defence", (_, _) => JSDefend(cid));
				aWar.AddItem( "Return ReIn", (_, _) => Reinforcement.ShowReturnDialog(cid, uie));
				aExport.AddItem( "Defense Sheet", ExportToDefenseSheet);
				AApp.AddItem(flyout, "Send Res", (_, _) => Spot.JSSendRes(cid));
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

			aMisc.AddItem( "Distance", (_, _) => ShowDistanceTo(Spot.focus));
			aMisc.AddItem( "Select", (_, _) => SelectMe(true, App.keyModifiers));
			aMisc.AddItem("Coords to Chat", () => ChatTab.PasteToChatInput(cid.CidToCoords(), true));
			flyout.RemoveEmpy();
			flyout.CopyXamlRoomFrom(uie);

			//   flyout.XamlRoot = uie.XamlRoot;
			flyout.ShowAt(uie, position);
		}

		private void SetTag(Tags id, bool on)
		{
			remarks = CityRename.SetTag(remarks,id.ToString(), on);


		}
		public static async void InfoClick(object sender, RoutedEventArgs e)
		{
			var cids = MainPage.GetContextCids(sender);
			foreach (var cid in cids)
			{
				await CityRename.RenameDialog(cid);
			}
		}
		public void DefendMe()
		{
			NearDefenseTab.defendant = this;
			var tab = NearDefenseTab.instance;
			if (!tab.isActive)
			{
				TabPage.mainTabs.AddTab(tab, true);
			}
			else
			{
				if (!tab.isVisible)
					TabPage.Show(tab);
				else
					tab.Refresh();
			}
		}
		public async void ShowIncoming()
		{
			if (allianceId == Alliance.myId)
			{
				IncomingTab tab = IncomingTab.instance;
				tab.Show();
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
				tab.Show();
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
						  JsonSerializer.Serialize(message), Windows.Storage.Streams.UnicodeEncoding.Utf8,
						   "application/json");

				var result = await client.PostAsync(Discord.discordHook, content);
				result.EnsureSuccessStatusCode();
			}
			catch (Exception ex)
			{
				Log(ex);
			}


		}
		async void CopyForSheets()
		{
			var sb = new StringBuilder();
			int counter = 0;
			foreach (var _cid in GetSelectedForContextMenu(cid, false))
			{
				++counter;
				var s = Spot.GetOrAdd(_cid);
				var c = await s.Classify();
				switch (c.classification)
				{
					case Classification.sorcs:
						sb.Append("Sorc\t");
						break;
					case Classification.druids:
						sb.Append("Druids\t");
						break;
					case Classification.academy:
						sb.Append("prae\t");
						break;
					case Classification.horses:
					case Classification.arbs:
						sb.Append("Horses\t");
						break;

					case Classification.se:
						sb.Append("Siege engines\t");
						break;
					case Classification.navy:
						sb.Append("Warships\t");
						break;
					default:
						sb.Append("vanq\t");
						break;
				}
				sb.Append(s.tsTotal + "\t");
				sb.Append(c.academies > 0 ? "Yes\t" : "No\t");
				sb.Append(s.xy + "\t");

			}
			App.CopyTextToClipboard(sb.ToString());
			Note.Show($"Copied {counter} castles to clipboard for sheets");
		}

		public async void SelectInUI(bool scrollIntoView)
		{
			//         await Task.Delay(2000);
			//          instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
			//           {
			//   await Task.Delay(200);
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				
					if (scrollIntoView && MainPage.IsVisible())
					{
						/// MainPage.CityGrid.SelectedItem = this;
						//                      MainPage.CityGrid.SetCurrentItem(this);

						//     MainPage.CityGrid.SetCurrentItem(this,false);
						MainPage.CityGrid.ScrollItemIntoView(this);
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
		//public List<Dungeon> raidDungeons =>
		//    {

		//    };

	}
	public static class SpotHelper
	{
		public static string CellText(this DataGridCellInfo cell) => (cell?.Column as DataGridTypedColumn)?.PropertyName ?? string.Empty;
	}
}
