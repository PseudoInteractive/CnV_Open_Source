using COTG.Helpers;
using COTG.Services;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using static COTG.Debug;

namespace COTG.Game
{
	public enum Diplomacy : byte
	{
		none = 0,
		allied = 1,
		nap = 2,
		enemy = 3
	}

	public class Alliance
	{
		public int id;
		public string name = string.Empty;

		// public static JsonDocument aldt;

		public static Alliance my = new Alliance();
		public static int myId;

		// Ass an assert
		public static int MyId
		{
			get
			{
				Assert(myId != 0);
				return myId;
			}
		}

		public static Dictionary<int, Alliance> all = new Dictionary<int, Alliance>();
		public static Dictionary<string, int> nameToId = new Dictionary<string, int>();

		public static int NameToId(string s)
		{
			Assert(alliancesFetched);
			return nameToId.TryGetValue(s, out var rv) ? rv : 0;
		}

		public static bool diplomacyFetched;

		public static bool PartNameToId(string name, out int id)
		{
			name = name.ToLower();
			foreach (var a in nameToId)
			{
				if (a.Key.ToLower().Contains(name))
				{
					id = a.Value;
					return true;
				}
			}
			id = -1;
			return false;
		}

		public static bool wantsIntel => ((myId == 42)) && JSClient.world == 23;

		public static string IdToName(int id)
		{
		//	Assert(alliancesFetched);
			if (all.TryGetValue(id, out var a))
			{
				return a.name;
			}

			return string.Empty;
		}

		public static Diplomacy GetDiplomacy(int allianceId)
		{
			Assert(diplomacyFetched);
			if (myId == allianceId)
			{
				return Diplomacy.allied;
			}

			if (diplomacy.TryGetValue((byte)allianceId, out var result) == false)
			{
				return Diplomacy.none;
			}

			return result switch
			{
				1 => Diplomacy.allied,
				2 => Diplomacy.nap,
				3 => Diplomacy.enemy,
				_ => Diplomacy.none,
			};
		}

		public static bool alliancesFetched;
		public static SortedList<byte, byte> diplomacy = new SortedList<byte, byte>(); // small Dictionary
		public static Alliance none = new Alliance() { id = 0, name = "No Alliance" };

		public static async void Ctor(JsonDocument _aldt)
		{
			Log("Fetch Aldt");
			if (diplomacyFetched)
			{
				Log("Already got aldt"); // should not happen
				return;
			}
			using var work = new Views.ShellPage.WorkScope("fetch alliances");

			var _all = new Dictionary<int, Alliance>();
			var _nameToId = new Dictionary<string, int>();
			var _diplomacy = new SortedList<byte, byte>();

			_all.Add(0, Alliance.none);
			_nameToId.Add(none.name, 0);

			try
			{
				var element = _aldt.RootElement.GetProperty("aldt");
				// if we have an alliance
				if (element.ValueKind == JsonValueKind.Object)
				{
					myId = my.id = element.GetAsInt("id");
					my.name = element.GetString("n");

					_all.Add(my.id, my);
					_nameToId.Add(my.name, my.id);

					// all.Add(my.id, my); nameToId.Add(my.name, my.id);

					if (element.TryGetProperty("d", out var dRoot))
					{
						foreach (var prop in dRoot.EnumerateObject())
						{
							byte relationship = (byte)int.Parse(prop.Name);

							foreach (var a in prop.Value.EnumerateArrayOrObject())
							{
								var alliance = new Alliance();
								byte allianceId = (byte)a.GetAsInt("id");
								string aname = a.GetAsString("n");

								alliance.name = aname;
								alliance.id = allianceId;

								_all.Add(allianceId, alliance);
								_nameToId.Add(aname, allianceId);

								var good = _diplomacy.TryAdd(allianceId, relationship);
								Assert(good == true);
							}

							// { "1":[{ "id":"7","n":"España"}],"2":[{ "id":"80","n":"Blood &
							// Thunder"}],"3":[{ "id":"1","n":"Horizon"},{ "id":"2","n":"The Lunatic
							// Asylum"},{ "id":"49","n":"Unidos-"},{ "id":"62","n":"OvernightObservation"}]}
						}
					}
					else
					{
						Log("Alliance Error!");
					}
				}
			}
			catch (Exception _e)
			{
				LogEx(_e);
			}

			diplomacy = _diplomacy;
			nameToId = new Dictionary<string, int>(_nameToId);
			all = new Dictionary<int, Alliance>(_all);
			diplomacyFetched = true;

			for (; ; )
			{
				if (!Player.all.IsNullOrEmpty())
				{
					break;
				}

				await Task.Delay(1000);
			}
			Assert(!Player.all.IsNullOrEmpty());
			var alliances = new List<string>();
			try
			{
				using (var jso = await Post.SendForJson("includes/gR.php", "a=1"))
				{
					var r = jso.RootElement;
					if (r.TryGetProperty("1", out var prop2))
					{
						foreach (var alliance in prop2.EnumerateArray())
						{
							var alName = alliance.GetAsString("1");
							// var al = alName == my.name ? my : new Alliance() { name = alName }; Log(alName);
							alliances.Add(alName);
						}
					}
				}

				foreach (var _al in alliances)
				{
					var alName = _al;
					// var al = _al;
					using (var jsa = await Post.SendForJson("includes/gAd.php", "a=" + HttpUtility.UrlEncode(alName)))
					{
						var id = jsa.RootElement.GetAsInt("id");
						if (all.TryGetValue(id, out var al) == false)
						{
							al = new Alliance() { id = id, name = alName };
							_all.Add(id, al);
							_nameToId.Add(alName, id);
						}

						// _all.Add(id, al); _nameToId.Add(alName, id);
						int counter = 0;
						if (jsa.RootElement.TryGetProperty("me", out var meList))
						{
							foreach (var me in meList.EnumerateArray())
							{
								var meName = me.GetString("n");
								if (meName == null)
								{
									//Log("Missing name? " + counter);
									//foreach (var member in me.EnumerateObject())
									//{
									//    Log($"{member.Name}:{member.Value.ToString()}");
									//}
								}
								else if (Player.nameToId.TryGetValue(meName, out var pId))
								{
									++counter;
									var p = Player.all[pId];
									p.alliance = (ushort)id;
									// p.cities = (byte)me.GetInt("c");
									p.points = (me.GetInt("s"));
								}
								else
								{
									Log("Error: " + meName);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LogEx(e);
			}

			nameToId = _nameToId;
			all = _all;
			// await Cosmos.GetSpotDB();
			alliancesFetched = true;
			// start this off once the fetches are finished
			Blobs.ProcessStats();
		}

		internal static bool IsMine(int allianceId)
		{
			return myId == allianceId;
		}

		internal static int FromPlayer(int player0)
		{
			Assert(alliancesFetched);
			if (player0 > 0)
			{
				var p = Player.Get(player0);
				if (p != null)
				{
					return p.alliance;
				}
			}
			return 0;
		}

		public static bool IsNap(int allianceId) => GetDiplomacy(allianceId) == Diplomacy.nap;

		public static bool IsAlly(int allianceId) => GetDiplomacy(allianceId) == Diplomacy.allied;

		public static bool IsAllyOrNap(int allianceId) => GetDiplomacy(allianceId) switch { Diplomacy.allied or Diplomacy.nap => true, _ => false };

		public static bool IsEnemy(int allianceId) => GetDiplomacy(allianceId) == Diplomacy.enemy;
	}
}
