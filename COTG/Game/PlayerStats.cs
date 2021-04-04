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
		public int cities; // could pull this from world data
		public int score;
		public int reputation;
		public int offensiveRep;
		public int defensiveRep;
		public int unitKills;
		public int alliance;
		public int raiding;

	}

	class AllianceStats
	{
		public int aid;
		public int cities; // could pull this from world data
		public int score;
		public int players;
		public int reputation;
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
					ps.pid = pid;
					ps.score = points;
					ps.cities = i.GetAsInt("5");
					ps.alliance = Alliance.NameToId(i.GetAsString("4"));
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
			// Now alliances
			{
				var js = await Post.SendForJson("includes/gR.php", "a=1");
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
					ps.score = points;
					ps.cities = i.GetAsInt("5");
					ps.players = i.GetAsInt("4");
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
				var js = await Post.SendForJson("includes/gR.php", "a=20&b=56");
				// score, cities, alliance
				foreach (var i in js.RootElement.GetProperty("20").EnumerateArray())
				{
					var pid = Alliance.NameToId(i.GetAsString("2"));
					if (snap.allianceStats.TryGetValue(pid, out var ps))
					{
						ps.military = i.GetAsInt("3");
					}
				}
			}
			return snap;
		}
	}
}

