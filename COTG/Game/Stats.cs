using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using COTG.Helpers;
using COTG.Services;

namespace COTG.Game
{
	class PlayerStats
	{
		public int pid;
		public int reputation;
		public int offensiveRep;
		public int defensiveRep;
		public int unitKills;
		public int alliance;
		public int raiding;

		public List<ContinentPlayerStats> perContinent = new();
	}
	class ContinentPlayerStats
	{
		public int continent;
		public int cities;
		public int score;
	}

	class AllianceStats
	{
		public int aid;

		public int reputation;
		public List<ContinentAllianceStats> perContinent = new();
	}
	class ContinentAllianceStats
	{
		public int continent;

		public int cities; // could pull this from world data
		public int score;
		public int players;
		public int military;
	}

	class Snapshot
	{
		public DateTimeOffset dateTime;
		public SortedList<int, PlayerStats> playerStats = new();
		public SortedList<int, AllianceStats> allianceStats = new();


		public static async Task<Snapshot> GetStats()
		{
			var snap = new Snapshot();
			{
				var js = await Post.SendForJson("includes/gR.php", "a=0&b=56");
				// score, cities, alliance
				foreach (var i in js.RootElement[0].EnumerateArray())
				{

					var points = i.GetAsInt("3");
					if (points <= 1000)
						continue;

					var pid = Player.NameToId(i.GetAsString("1"));
					if (pid <= 0)
						continue;
					var ps = new PlayerStats();
					var pc = new ContinentPlayerStats();
					pc.continent = Continent.idAll;
					ps.pid = pid;
					pc.score = points;
					pc.cities = i.GetAsInt("5");
					ps.alliance = Alliance.NameToId(i.GetAsString("4"));
					ps.perContinent.Add(pc);
					snap.playerStats.Add(pid, ps);
				}


			}

			{
				var js = await Post.SendForJson("includes/gR.php", "a=8");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("8").EnumerateArray())
				{
					var pid = Player.NameToId(i.GetAsString("1"));
					if (snap.playerStats.TryGetValue(pid, out var ps))
					{
						ps.reputation = i.GetAsInt("4");
					}

				}
			}
			{
				var js = await Post.SendForJson("includes/gR.php", "a=8");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("8").EnumerateArray())
				{
					var pid = Player.NameToId(i.GetAsString("1"));
					if (snap.playerStats.TryGetValue(pid, out var ps))
					{
						ps.reputation = i.GetAsInt("4");
					}
				}
			}
			{
				var js = await Post.SendForJson("includes/gR.php", "a=4");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("4").EnumerateArray())
				{
					var pid = Player.NameToId(i.GetAsString("1"));
					if (snap.playerStats.TryGetValue(pid, out var ps))
					{
						ps.defensiveRep = i.GetAsInt("4");
					}
				}
			}
			{
				var js = await Post.SendForJson("includes/gR.php", "a=3");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("3").EnumerateArray())
				{
					var pid = Player.NameToId(i.GetAsString("1"));
					if (snap.playerStats.TryGetValue(pid, out var ps))
					{
						ps.offensiveRep = i.GetAsInt("4");
					}
				}
			}
			{
				var js = await Post.SendForJson("includes/gR.php", "a=5");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("5").EnumerateArray())
				{
					var pid = Player.NameToId(i.GetAsString("1"));
					if (snap.playerStats.TryGetValue(pid, out var ps))
					{
						ps.unitKills = i.GetAsInt("4");
					}
				}
			}
			{
				var js = await Post.SendForJson("includes/gR.php", "a=7");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("7").EnumerateArray())
				{
					var pid = Player.NameToId(i.GetAsString("1"));
					if (snap.playerStats.TryGetValue(pid, out var ps))
					{
						ps.unitKills = i.GetAsInt("4");
					}
				}
			}
			{
				// per continent

				for (int id = 0; id < Continent.count - 1; ++id)
				{
					if (Continent.all[id].isOpen)
					{

						var contId = Continent.GetContIdFromPacked(id);
						var js = await Post.SendForJson("includes/gR.php", $"a=0&b={contId.x}{contId.y}");
						// score, cities, alliance
						foreach (var i in js.RootElement[0].EnumerateArray())
						{
							var pid = Player.NameToId(i.GetAsString("1"));
							if (!snap.playerStats.TryGetValue(pid, out var ps))
								continue;
							var cnt = new ContinentPlayerStats();
							cnt.continent = id;
						
							cnt.score = i.GetAsInt("3"); 
							cnt.cities = i.GetAsInt("5");
							ps.perContinent.Add(cnt);
						}



					}
				}
			}
			// Now alliances
			{
				var js = await Post.SendForJson("includes/gR.php", "a=1&b=56");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("1").EnumerateArray())
				{

					var points = i.GetAsInt("3");
					if (points <= 1000)
						continue;

					var pid = Alliance.NameToId(i.GetAsString("1"));
					if (pid <= 0)
						continue;
					var ps = new AllianceStats();
					ps.aid = pid;
					var pc = new ContinentAllianceStats();
					pc.continent = Continent.idAll; ;
					pc.score = points;
					pc.cities = i.GetAsInt("5");
					pc.players = i.GetAsInt("4");
					ps.perContinent.Add(pc);
					snap.allianceStats.Add(pid, ps);
				}
			}
			{
				var js = await Post.SendForJson("includes/gR.php", "a=19");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("19").EnumerateArray())
				{
					var pid = Alliance.NameToId(i.GetAsString("2"));
					if (snap.allianceStats.TryGetValue(pid, out var ps))
					{
						ps.reputation = i.GetAsInt("3");
					}
				}
			}
			//  TODO:  Faith
			
			{
				// per continent

				for (int id = 0; id < Continent.count - 1; ++id)
				{
					if (Continent.all[id].isOpen)
					{

						var contId = Continent.GetContIdFromPacked(id);
						var js = await Post.SendForJson("includes/gR.php", $"a=1&b={contId.x}{contId.y}");
						// score, cities, alliance
						foreach (var i in js.RootElement.GetProperty("1").EnumerateArray())
						{
							var pid = Alliance.NameToId(i.GetAsString("1"));
							if (!snap.allianceStats.TryGetValue(pid, out var ps))
								continue;
							var cnt = new ContinentAllianceStats();
							cnt.continent = id;

							cnt.score = i.GetAsInt("3");
							cnt.cities = i.GetAsInt("5");
							cnt.players = i.GetAsInt("4");
							ps.perContinent.Add(cnt);
						}



					}
				}
			}
			{
				// per continent mulitary
				// include the "all" continent at the end
				for (int id = 0; id < Continent.count; ++id)
				{
					if (Continent.all[id].isOpen)
					{
						var contId = Continent.GetContIdFromPacked(id);
						var js = await Post.SendForJson("includes/gR.php", $"a=20&b={contId.x}{contId.y}");
						foreach (var i in js.RootElement.GetProperty("20").EnumerateArray())
						{
							var pid = Alliance.NameToId(i.GetAsString("2"));
							if (!snap.allianceStats.TryGetValue(pid, out var ps))
								continue;
							foreach(var pc in ps.perContinent)
							{
								if(pc.continent == id)
								{
									pc.military = i.GetAsInt("3");
									break;
								}

							}
						}



					}
				}
			}
			return snap;
		}
	}
}

