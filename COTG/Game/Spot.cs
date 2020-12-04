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
	public class Spot : IEquatable<Spot>, INotifyPropertyChanged
	{
		public static ConcurrentDictionary<int, Spot> allSpots = new ConcurrentDictionary<int, Spot>(); // keyed by cid
		public static HashSet<int> selected = new HashSet<int>();
		public static Spot[] defendersI = Array.Empty<Spot>();
		public static Spot[] defendersO = Array.Empty<Spot>();

		public static bool TryGet(int cid, out Spot spot) => allSpots.TryGetValue(cid, out spot);
		public static int focus; // city that has focus (selected, but not necessarily building.  IF you click a city once, it goes to this state

		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		internal static Spot GetFocus()
		{
			return focus == 0 ? null : GetOrAdd(focus);
		}

		public static Spot invalid = new Spot() { _cityName = "?" };
		public static Spot pending = new Spot() { _cityName = "pending" };

		public static Spot[] emptySpotSource = new[] { pending };

		public string nameAndRemarks => remarks.IsNullOrEmpty() ? _cityName : $"{_cityName} - {remarks}";
		public string remarks { get; set; } = string.Empty; // only for city
		public string notes { get; set; } = string.Empty; // only for city

		public static bool IsFocus(int cid)
		{
			return focus == cid;
		}
		public static Spot GetOrAdd(int cid, string cityName = null)
		{
			if (cid <= 0)
			{
				return invalid;
			}
			if (!Spot.TryGet(cid, out var rv))
			{
				Assert(City.allCities.ContainsKey(cid) == false);
				var worldC = cid.CidToWorld();
				var info = World.GetInfo(worldC);

				//    Assert(info.type == World.typeCity);
				if (info.player == Player.myId)
					rv = City.GetOrAddCity(cid);
				else
					rv = new Spot() { cid = cid, pid = info.player };

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
			}
			if (cityName != null)
				rv._cityName = cityName;
			return (rv);
		}
		public static void UpdateName(int cid, string name) => GetOrAdd(cid, name);

		internal bool HasTag(Tag tag)
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

		public int tsRaid { get; set; }
		public int tsHome { get; set; }
		public int tsDefHome => (this is City city ? city.troopsHome.TSDef() : tsHome);
		public int tsDefTotal => (this is City city ? city.troopsTotal.TSDef() : tsTotal);

		public Reinforcement[] reinforcementsIn = Array.Empty<Reinforcement>();
		public Reinforcement[] reinforcementsOut = Array.Empty<Reinforcement>();

		public int _tsTotal;
		public int tsTotal { get => _tsTotal > 0 ? _tsTotal : tsHome; set => _tsTotal = value; }
		public int tsDefMax { get { var i = incomingDefTS; return (i > 0) ? i : (reinforcementsIn.TS() + ((this is City city) ? city.troopsHome.TSDef() : tsHome)); } }
		public int tsOff { get { var i = incomingOffTS; return (i > 0) ? i : (this is City city) ? city.troopsHome.TSOff() : 0; } }
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

		public DateTimeOffset lastAccessed { get; set; } // lass user access
		public byte attackCluster { get; set; } // For attackTab
		public bool attackFake { get; set; } // For attackTab
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
			pending
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
			"pending"
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
			ttGuard,
			ttBallista
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
			App.DispatchOnUIThreadSneaky(() => OnPropertyChanged(nameof(pinned)));
		}  // pinned in MRU
		public byte claim; // only if this is under attack
		public byte shipyards { get; set; }
		public byte ports { get; set; }
		public string Claim => $"{(int)claim:00}%";
		public bool isBlessed { get; set; }
		public float scoutRange { get; set; }
		public ushort points { get; set; }
		public BitmapImage icon => ImageHelper.FromImages(isBlessed ? "blessed.png" :
			 ($"{(isTemple ? "temple" : isCastle ? "castle" : "city")}{GetSize()}{(isOnWater ? "w" : "")}.png"));
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
				sb.Append(s.player);
				sb.Append('\t');
				sb.Append(s.cont);
				sb.Append('\t');
				sb.Append(s.xy);
				sb.Append('\t');
				sb.Append(s.incoming.Where(x => x.isAttack).First()?.time.FormatTimeDefault() ?? "??");
				sb.Append('\t');
				sb.Append(s.incTT);
				sb.Append('\t');
				sb.Append("1000000");
				sb.Append('\t');
				var ts = s.tsDefMax;
				sb.Append(ts);
				sb.Append('\n');
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
			viewHover = spot != null ? spot.cid : 0;
			Player.viewHover = spot != null ? spot.pid : 0;


			return (spot, cell, physicalPoint, grid);
		}




		public static void ProcessPointerPress(UserTab tab,object sender, PointerRoutedEventArgs e)
		{
			tab.FocusOn(sender as DependencyObject);
			e.KeyModifiers.UpdateKeyModifiers();

			var hit = Spot.HitTest(sender, e);
			var spot = hit.spot;
			uiPress = spot != null ? spot.cid : 0;
			uiPressColumn = hit.column.CellText();
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
			viewHover = 0;
			Player.viewHover = 0;
			//    uiHoverColumn = string.Empty;
		}

		public virtual byte GetPrimaryTroopType(bool onlyHomeTroops)
		{
			return classification switch
			{
				Classification.unknown => ttGuard,
				Classification.vanqs => ttVanquisher,
				Classification.rt => ttTriari,
				Classification.sorcs => ttSorcerer,
				Classification.druids => ttDruid,
				Classification.academy => ttPraetor,
				Classification.horses => ttHorseman,
				Classification.arbs => ttArbalist,
				Classification.se => ttScorpion,
				Classification.hub => ttBallista,
				Classification.navy => ttWarship,
				_ => ttBallista
			};
		}

		public void ProcessClick(string column, PointerPoint pt, UIElement uie, VirtualKeyModifiers modifiers)
		{
			modifiers.UpdateKeyModifiers();
			//    Note.Show($"{this} {column} {pt.Position}");

			if (pt.Properties.IsLeftButtonPressed && !(modifiers.IsShiftOrControl())) // ignore selection style clicks
			{


				// If we are already selected and we get clicked, there will be no selection chagne to raids are not scanned automatically
				var wantRaidingScan = (City.IsMine(cid) && MainPage.IsVisible());
				var wantRaidScan = isFocus;
				//                var needCityData = 

				switch (column)
				{
					case nameof(xy):
						ProcessCoordClick(cid, false, modifiers);
						wantRaidScan = false;
						break;
					case "I":
						if (City.IsMine(cid))
						{
							var wasBuild = City.IsBuild(cid);
							JSClient.ChangeCity(cid, false);
							if (wasBuild)
							{
								JSClient.ChangeView(!JSClient.IsCityView());

							}

						}
						else
						{
							JSClient.ShowCity(cid, false);
						}
						wantRaidScan = false;
						break;
					case nameof(City.tsTotal):
						if (City.IsMine(cid) && MainPage.IsVisible())
						{
							Raiding.UpdateTS(true, true);
						}
						wantRaidScan = false;
						break;
					case nameof(tsHome):
					case nameof(tsRaid):
						if (City.IsMine(cid) && MainPage.IsVisible())
						{
							Raiding.UpdateTS(true, true);
						}
						wantRaidScan = false;
						break;
					case nameof(City.raidReturn):
						if (City.IsMine(cid) && MainPage.IsVisible())
						{
							Raiding.ReturnFast(cid, true);
						}
						wantRaidScan = false;
						break;
					case nameof(pinned):
						SetPinned(!pinned);

						return;
					case nameof(City.raidCarry):
						if (City.IsMine(cid) && MainPage.IsVisible())
						{
							Raiding.ReturnSlow(cid, true);
						}
						wantRaidScan = false;
						break;
					default:
						wantRaidScan = true;
						break;
				}


				if (MainPage.IsVisible() && isMine && wantRaidScan)
				{
					//                MainPage.SetRaidCity(cid,true);
					ScanDungeons.Post(cid, true, false);
				}
				SetFocus(false);
				NavStack.Push(cid);

			}
			else if (pt.Properties.IsRightButtonPressed)
			{
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

		public string ToTsv()
		{
			return $"{cid.CidToCoords()}\t{this.player}\t{this._cityName ?? ""}\t{this.remarks ?? ""}\t{this.alliance}\t{this.isCastle}\t{this.isOnWater}";
		}

		public static async void ProcessCoordClick(int cid, bool lazyMove, VirtualKeyModifiers mod)
		{
			mod.UpdateKeyModifiers();
			if (City.IsMine(cid) && !mod.IsShiftOrControl())
			{
				if (City.IsBuild(cid))
				{
					JSClient.ChangeView(!JSClient.IsCityView());// toggle between city/region view

					cid.BringCidIntoWorldView(lazyMove);
				}
				else
				{
					JSClient.ChangeCity(cid, lazyMove); // keep current view, switch to city
				}
				NavStack.Push(cid);

			}
			else
			{
				JSClient.ShowCity(cid, lazyMove);
				NavStack.Push(cid);


			}
			SpotTab.TouchSpot(cid, mod);

			if (mod.IsShiftAndControl())
			{
				//     var spot = Spot.GetOrAdd(cid);
				//     GetCity.Post(cid, spot.pid, (js, city) => Log(js));

				var str = await Post.SendForText("includes/gLay.php", $"cid={cid}");
				Log(str);

				App.DispatchOnUIThreadSneaky(() =>
				{
					if (World.GetInfoFromCid(cid).isWater)


						// set is water var
						str = $"[ShareString.1.3]{(World.GetInfoFromCid(cid).isWater ? ';' : ':')}{str.Substring(18)}";
					App.CopyTextToClipboard(str);

					Launcher.LaunchUriAsync(new Uri($"http://cotgopt.com/?map={str}"));
				});
			}
		}
		internal async ValueTask<Classification> ClassifyIfNeeded()
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
		internal async ValueTask<ClassificationExtended> Classify()
		{
			classification = Classification.pending;
			var str = await Post.SendForText("includes/gLay.php", $"cid={cid}");
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
		public void SelectMe(bool showClick)
		{
			NavStack.Push(cid);
			SpotTab.AddToGrid(this, App.keyModifiers, true);
			ProcessSelection(App.keyModifiers);
			if (showClick)
			{
				JSClient.ShowCity(cid, true);
			}
		}

		public void ProcessSelection(VirtualKeyModifiers mod, bool forceSelect = false)
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
						}
						else
						{
							var newSel = new HashSet<int>(sel);
							newSel.Add(cid);
							selected = newSel;

							sel0.Add(this);
							sel1.Add(this);

						}
						//                 SpotTab.SelectedToGrid();
					}

					else
					{
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


		public static int viewHover; // in the view menu

		//        public static string uiHoverColumn = string.Empty;
		public static int uiPress; //  set when pointerPressed is recieved, at this point a contect menu might come up, causing us to lose uiHover
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
		public void SetFocus(bool selectInUI)
		{
			SetFocus(cid, selectInUI);
		}
		public static void SetFocus(int cid, bool selectInUI)
		{
			var changed = cid != focus;
			var spot = Spot.GetOrAdd(cid);
			spot.SelectMe(false);
			if (changed)
			{
				focus = cid;
				App.DispatchOnUIThreadSneaky(() =>
				{
					ShellPage.instance.focus.Content = spot.nameAndRemarks;
					ShellPage.instance.coords.Text = cid.CidToString();
					if (selectInUI)
						spot.SelectInUI(true);

				}

				);
			}
			cid.BringCidIntoWorldView(true);
		}
		public void ReturnSlowClick()
		{
			Raiding.ReturnSlow(cid, true);
		}
		public async void ReturnAt(object sender, RoutedEventArgs e)
		{
			(var at, var okay) = await Views.DateTimePicker.ShowAsync2("Return By:");
			if (!okay)
				return; // aborted

			await Raiding.ReturnAt(cid, at);
			Note.Show($"{cid.CidToStringMD()} end raids at {at.FormatDefault()}");
		}
		public async void ReturnAtBatch(object sender, RoutedEventArgs e)
		{
			(var at, var okay) = await Views.DateTimePicker.ShowAsync2("Return By:");
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
			await Post.Send("overview/rcallall.php", "a=" + cid);
			await Post.SendEncrypted("includes/UrOA.php", "{\"a\":" + cid + ",\"c\":0,\"b\":2}", "Rx3x5DdAxxerx3");
		}

		public void ReturnFastClick()
		{
			if (App.IsKeyPressedShift())
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
			for (int i = 1; i < ttCount; ++i)
			{
				var dt = TimeSpan.FromMinutes(dist * TTTravel(i));
				sb.Append($"\n{ttName[i]}: {dt.ToString()}");
			}
			var str = sb.ToString();
			App.CopyTextToClipboard(str);
			Note.Show(str);
		}
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
					if (spot is City city)
						city.SetBuild(false);
					else
						spot.SetFocus(false);
					return true;
				case VirtualKey.Right:
					

					return true;
				case VirtualKey.Application:
					break;
				case VirtualKey.Sleep:
					break;
				case VirtualKey.NumberPad0:
					break;
				case VirtualKey.NumberPad1:
					break;
				case VirtualKey.NumberPad2:
					break;
				case VirtualKey.NumberPad3:
					break;
				case VirtualKey.NumberPad4:
					break;
				case VirtualKey.NumberPad5:
					break;
				case VirtualKey.NumberPad6:
					break;
				case VirtualKey.NumberPad7:
					break;
				case VirtualKey.NumberPad8:
					break;
				case VirtualKey.NumberPad9:
					break;
				case VirtualKey.Multiply:
					break;
				case VirtualKey.Add:
					break;
				case VirtualKey.Separator:
					break;
				case VirtualKey.Subtract:
					break;
				case VirtualKey.Decimal:
					break;
				case VirtualKey.Divide:
					break;
				case VirtualKey.F1:
					break;
				case VirtualKey.F2:
					break;
				case VirtualKey.F3:
					break;
				case VirtualKey.F4:
					break;
				case VirtualKey.F5:
					break;
				case VirtualKey.F6:
					break;
				case VirtualKey.F7:
					break;
				case VirtualKey.F8:
					break;
				case VirtualKey.F9:
					break;
				case VirtualKey.F10:
					break;
				case VirtualKey.F11:
					break;
				case VirtualKey.F12:
					break;
				case VirtualKey.F13:
					break;
				case VirtualKey.F14:
					break;
				case VirtualKey.F15:
					break;
				case VirtualKey.F16:
					break;
				case VirtualKey.F17:
					break;
				case VirtualKey.F18:
					break;
				case VirtualKey.F19:
					break;
				case VirtualKey.F20:
					break;
				case VirtualKey.F21:
					break;
				case VirtualKey.F22:
					break;
				case VirtualKey.F23:
					break;
				case VirtualKey.F24:
					break;
				case VirtualKey.NavigationView:
					break;
				case VirtualKey.NavigationMenu:
					break;
				case VirtualKey.NavigationUp:
					break;
				case VirtualKey.NavigationDown:
					break;
				case VirtualKey.NavigationLeft:
					break;
				case VirtualKey.NavigationRight:
					break;
				case VirtualKey.NavigationAccept:
					break;
				case VirtualKey.NavigationCancel:
					break;
				case VirtualKey.NumberKeyLock:
					break;
				case VirtualKey.Scroll:
					break;
				case VirtualKey.LeftShift:
					break;
				case VirtualKey.RightShift:
					break;
				case VirtualKey.LeftControl:
					break;
				case VirtualKey.RightControl:
					break;
				case VirtualKey.LeftMenu:
					break;
				case VirtualKey.RightMenu:
					break;
				case VirtualKey.GoBack:
					break;
				case VirtualKey.GoForward:
					break;
				case VirtualKey.Refresh:
					break;
				case VirtualKey.Stop:
					break;
				case VirtualKey.Search:
					break;
				case VirtualKey.Favorites:
					break;
				case VirtualKey.GoHome:
					break;
				case VirtualKey.GamepadA:
					break;
				case VirtualKey.GamepadB:
					break;
				case VirtualKey.GamepadX:
					break;
				case VirtualKey.GamepadY:
					break;
				case VirtualKey.GamepadRightShoulder:
					break;
				case VirtualKey.GamepadLeftShoulder:
					break;
				case VirtualKey.GamepadLeftTrigger:
					break;
				case VirtualKey.GamepadRightTrigger:
					break;
				case VirtualKey.GamepadDPadUp:
					break;
				case VirtualKey.GamepadDPadDown:
					break;
				case VirtualKey.GamepadDPadLeft:
					break;
				case VirtualKey.GamepadDPadRight:
					break;
				case VirtualKey.GamepadMenu:
					break;
				case VirtualKey.GamepadView:
					break;
				case VirtualKey.GamepadLeftThumbstickButton:
					break;
				case VirtualKey.GamepadRightThumbstickButton:
					break;
				case VirtualKey.GamepadLeftThumbstickUp:
					break;
				case VirtualKey.GamepadLeftThumbstickDown:
					break;
				case VirtualKey.GamepadLeftThumbstickRight:
					break;
				case VirtualKey.GamepadLeftThumbstickLeft:
					break;
				case VirtualKey.GamepadRightThumbstickUp:
					break;
				case VirtualKey.GamepadRightThumbstickDown:
					break;
				case VirtualKey.GamepadRightThumbstickRight:
					break;
				case VirtualKey.GamepadRightThumbstickLeft:
					break;
				default:
					break;
			}
			return false;
		}

		public void ShowContextMenu(UIElement uie, Windows.Foundation.Point position)
        {
         ;
         //   SelectMe(false) ;
            var flyout = new MenuFlyout();

            if (this.isCityOrCastle)
            {
                // Look - its my city!
                if (this.isMine)
                {
                    // This one has multi select
                    int count = 1;
                    if (uie == MainPage.CityGrid || uie == NearDefenseTab.instance.supportGrid)
                    {
                        count = MainPage.GetContextCidCount(cid);
                    }
                    if (count > 1)
                    {
                        App.AddItem(flyout, $"End Raids x{count} selected", MainPage.ReturnSlowClick, cid);
                        App.AddItem(flyout, $"Home Please x{count} selected", MainPage.ReturnFastClick, cid);
                        App.AddItem(flyout, $"Return At...x{count}", this.ReturnAtBatch);

                    }
                    else
                    {

                        App.AddItem(flyout, "End Raids", this.ReturnSlowClick);
                        App.AddItem(flyout, "Home Please", this.ReturnFastClick);
                        App.AddItem(flyout, "Return At...", this.ReturnAt);
                    }

                    App.AddItem(flyout, "Set Hub", (_, _) => CitySettings.SetCitySettings(cid));
                    App.AddItem(flyout, "Set Recruit", (_, _) => CitySettings.SetRecruitFromTag(cid));

                    App.AddItem(flyout, "Rename", (_, _) => CityRename.RenameDialog(cid));
                    //   App.AddItem(flyout, "Clear Res", (_, _) => JSClient.ClearCenterRes(cid) );
                    App.AddItem(flyout, "Clear Center Res", (_, _) => JSClient.ClearCenter(cid));


					App.AddItem(flyout, "Troops to Sheets",CopyForSheets);
				}
                else
                {
                    if (AttackTab.instance.isActive)
                    {
                        App.AddItem(flyout, "Add as Real", (_, _) => AttackTab.AddTarget(cid, false));
                        App.AddItem(flyout, "Add as Fake", (_, _) => AttackTab.AddTarget(cid, true));
                    }
                    App.AddItem(flyout, "Add to Attack Sender", async (_, _) =>
                    {
						using var work = new WorkScope("Add to attack sender..");

						foreach (var id in Spot.GetSelectedForContextMenu(cid, false))
                        {
                            await JSClient.AddToAttackSender(id);
                        }
                    }
                    );

					//App.AddItem(flyout, "Add as Fake (2)", (_, _) => AttackTab.AddTarget(cid, 2));
					//App.AddItem(flyout, "Add as Fake (3)", (_, _) => AttackTab.AddTarget(cid, 3));
					//App.AddItem(flyout, "Add as Fake (4)", (_, _) => AttackTab.AddTarget(cid, 3));
                }
                if (cid != City.build)
                {
                    App.AddItem(flyout, "Set target hub", (_, _) => CitySettings.SetTargetHub(City.build, cid));
                    //if(Player.myName == "Avatar")
                    //    App.AddItem(flyout, "Set target hub I", (_, _) => CitySettings.SetOtherHubSettings(City.build, cid));
                }
                if (AttackTab.instance.isActive)
                {
                    App.AddItem(flyout, "Add as Attacker", (_, _) =>
                    {
						using var work = new WorkScope("Add as attackers..");

						string s = string.Empty;
                        foreach (var id in Spot.GetSelectedForContextMenu(cid, false))
                        {
                           s = s + id.CidToString() + "\t";
                        }
                        AttackTab.instance.AddAttacksFromString(s);
                        Note.Show($"Added attacker {s}");

                    });
                }

                App.AddItem(flyout, "Attack", (_, _) => Spot.JSAttack(cid));
                App.AddItem(flyout, "Near Defence", DefendMe);
                if (incoming.Any())
                    App.AddItem(flyout, "Incoming", ShowIncoming);


                App.AddItem(flyout, "Send Defence", (_, _) => JSDefend(cid));
                App.AddItem(flyout, "Send Res", (_, _) => Spot.JSSendRes(cid));
                App.AddItem(flyout, "Return ReIn", (_, _) => Reinforcement.ShowReturnDialog(cid, uie));
                App.AddItem(flyout, "Defense Sheet", ExportToDefenseSheet);
            }
            else if (this.isDungeon || this.isBoss)
            {
                App.AddItem(flyout, "Raid", (_, _) => Spot.JSRaid(cid));

            }
            else if (this.isEmpty && Discord.isValid)
            {
                App.AddItem(flyout, "Claim", this.DiscordClaim);

            }
			App.AddItem(flyout, "Notify on Decay", DecayQuery);

			App.AddItem(flyout, "Distance", (_, _) => ShowDistanceTo(Spot.focus));
            App.AddItem(flyout, "Select",(_,_)=> SelectMe(true) );
            App.AddItem(flyout, "Coords to Chat", () => ChatTab.PasteToChatInput(cid.CidToCoords(), true));
            flyout.CopyXamlRoomFrom(uie);

            //   flyout.XamlRoot = uie.XamlRoot;
            flyout.ShowAt(uie, position);
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
					if (tab.defenderGrid.ItemsSource != null && tab.defenderGrid.ItemsSource != Spot.emptySpotSource)
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
					if (tab.attackerGrid.ItemsSource != null && tab.attackerGrid.ItemsSource != Spot.emptySpotSource)
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
            await Task.Delay(200);
            App.DispatchOnUIThreadSneaky(() =>
            {
                if (this is City)
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
                    if (City.IsBuild(cid))
                    {
                        ShellPage.instance.cityBox.SelectedItem = this;
                    }
                }
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
        public static string CellText(this DataGridCellInfo cell) => cell?.Column.Header?.ToString() ?? string.Empty;
    }
}
