using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CnV.Helpers;
using CnV.Services;

using static CnV.Debug;

namespace CnV.Game
{
	using Services;

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
		public SmallTime time;
		public SortedList<int, PlayerStats> playerStats = new();
		public SortedList<int, AllianceStats> allianceStats = new();
		//public int UncompressedSizeEstimate()
		//{
		//	var rv = 1024;
		//	foreach(var p in playerStats)
		//	{
		//		rv += 7 * 4 + p.Value.perContinent.count * 3 * 4;
		//	}
		//	foreach (var a in allianceStats)
		//	{
		//		rv += 2 * 4 + a.Value.perContinent.count * 5 * 4;
		//	}
		//}

		public static async Task<Snapshot> GetStats()
		{
			var snap = new Snapshot();
			snap.time = SmallTime.serverNow;
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
						var js = await Post.SendForJson("includes/gR.php", $"a=0&b={contId.y}{contId.x}");
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
						var js = await Post.SendForJson("includes/gR.php", $"a=1&b={contId.y}{contId.x}");
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
				// per continent military
				// include the "all" continent at the end
				for (int id = 0; id < Continent.count; ++id)
				{
					if (Continent.all[id].isOpen)
					{
						var contId = Continent.GetContIdFromPacked(id);
						var js = await Post.SendForJson("includes/gR.php", $"a=20&b={contId.y}{contId.x}");
						foreach (var i in js.RootElement.GetProperty("20").EnumerateArray())
						{
							var pid = Alliance.NameToId(i.GetAsString("2"));
							if (!snap.allianceStats.TryGetValue(pid, out var ps))
								continue;
							foreach (var pc in ps.perContinent)
							{
								if (pc.continent == id)
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
	class TSContinentStats
	{
		public int continent;
		public List<TSContinentPlayerStats> players = new(); // ordered by score
		public void FixupPlayers(ContinentAllianceStats[]  allianceStats, TSContinentStats predicted )
		{
			//struct allianceTemps
			//{
			//	var reportedTs;
			//}
			var allianceCount = allianceStats.Length;

			// sort by continent
			for (; ; )
			{
				var changes = false;
				// swap TS to match alliances
				

			}

		}
	}
	class TSContinentPlayerStats
	{
		public int pid; 
		public int score; // for sorting unknown players
		public int cities; // for sorting unknown players
		public int tsTotal; // approximate
		public int tsOff; // approximate
		public int tsDef;// tsDef is difference
	}

	class TSSnapshot
	{
		const int maxPlayersPerSnapshot = 1024;
		public SmallTime time;
		public List<TSContinentStats> continents = new();

		public static async Task<TSSnapshot> GetStats()
		{
			var snap = new TSSnapshot();
			snap.time = SmallTime.serverNow;
			try
			{
				// per continent

				for (int id = 0; id < Continent.count; ++id)
				{
					if (Continent.all[id].isOpen)
					{
						TSContinentStats cs = new();
						cs.continent = id;
						snap.continents.Add(cs);
						const int minScore = 100;
						var contId = Continent.GetContIdFromPacked(id);
						{
							var js = await Post.SendForJson("includes/gR.php", $"a=0&b={contId.y}{contId.x}");
							// score, cities, alliance
							foreach (var i in js.RootElement[0].EnumerateArray())
							{
								var score = i.GetAsInt("3");
								if (score <= minScore)
									continue;

								var pid = Player.NameToId(i.GetAsString("1"));
								var ps = new TSContinentPlayerStats();

								ps.pid = pid;
								ps.score = score;
								ps.cities = i.GetAsInt("5");

								cs.players.Add(ps);
								if (cs.players.Count >= maxPlayersPerSnapshot)
									break;
							}
						}
						// these are unknown values
						List<int> tsTotal = new();
						List<int> tsOff = new();
						List<int> tsDef = new();

						{
							// total ts
							var js = await Post.SendForJson("includes/gR.php", $"a=16&b={contId.y}{contId.x}");
							// score, cities, alliance
							foreach (var i in js.RootElement.GetProperty("16").EnumerateArray())
							{
								var ts = i[2].GetAsInt();
								var name = i[1];
								if (name.ValueKind != System.Text.Json.JsonValueKind.String)
								{
									tsTotal.Add(ts);
								}
								else
								{
									var pid = Player.NameToId(name.GetAsString());
									foreach (var player in cs.players)
									{
										if (player.pid == pid)
										{
											player.tsTotal = ts;
											break;
										}
									}
								}
							}

						}
						{
							// offense ts
							var js = await Post.SendForJson("includes/gR.php", $"a=17&b={contId.y}{contId.x}");
							// score, cities, alliance
							foreach (var i in js.RootElement.GetProperty("17").EnumerateArray())
							{
								var ts = i[2].GetAsInt();
								var name = i[1];
								if (name.ValueKind != System.Text.Json.JsonValueKind.String)
								{
									tsOff.Add(ts);
								}
								else
								{
									var pid = Player.NameToId(name.GetAsString());
									foreach (var player in cs.players)
									{
										if (player.pid == pid)
										{
											player.tsOff = ts;
											break;
										}
									}
								}
							}

						}
						{
							// defense ts
							var js = await Post.SendForJson("includes/gR.php", $"a=18&b={contId.y}{contId.x}");
							// score, cities, alliance
							foreach (var i in js.RootElement.GetProperty("18").EnumerateArray())
							{
								var ts = i[2].GetAsInt();
								var name = i[1];
								if (name.ValueKind != System.Text.Json.JsonValueKind.String)
								{
									tsDef.Add(ts);
								}
								else
								{
									var pid = Player.NameToId(name.GetAsString());
									foreach (var player in cs.players)
									{
										if (player.pid == pid)
										{
											player.tsDef = ts;
											break;
										}
									}
								}
							}

						}
						//
						// these are sorted by score
						// 
						foreach (var pp in cs.players)
						{
							if (tsTotal.IsNullOrEmpty())
								break;
							if(pp.tsTotal == 0)
							{
								var ts = tsTotal.Max();
								pp.tsTotal = ts;
								tsTotal.RemoveAt(tsTotal.IndexOf(ts));
								// find matching offense and defense
								var tsOCount = tsOff.Count;
								var tsDCount = tsDef.Count;
								
								// first check for only def
								for (int defId = 0; defId < tsDCount; ++defId)
								{
									var tsD = tsDef[defId];
									// only def
									if (ts == tsD)
									{
										pp.tsDef = tsD;
										pp.tsOff = 0;
										tsDef.RemoveAt(defId);
										goto __found;
									}
								}

								for (int offId = 0; offId < tsOCount; ++offId)
								{
									var tsO = tsOff[offId];
									var delta = ts - tsO;
									// check for only off
									if(delta==0)
									{
										pp.tsOff = tsO;
										pp.tsDef = 0;
										tsOff.RemoveAt(offId);
										goto __found;
									}
									// check for combo
									for (int defId = 0; defId < tsDCount; ++defId)
									{
										var tsD = tsDef[defId];
										// combo off and def (most common)
										if (delta == tsD)
										{
											pp.tsOff = tsO;
											pp.tsDef = tsD;
											tsOff.RemoveAt(offId);
											tsDef.RemoveAt(defId);
											goto __found;
										}
									}

								}
								Log($"Missing: {Player.IdToName(pp.pid)},ts={ts}, cont={contId.y}{contId.x}" );
								__found:;
	
							}
							
						}				
					}
				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
				return null;
			}
			return snap;
		}
	}
}
		


	



