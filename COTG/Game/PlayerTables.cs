using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	using CnVChat;
	using CnV;

	// Assume that world is set at this point
	internal static class PlayerTables
	{
		// We should ditch these after load
		// There might be a lot of these, we might optimize this one day
		internal static Dictionary<PlayerId, PlayerGameEntity> playersGame = new();

		public static async Task LoadGamePlayers()
		{
			// Fetch all game players

			await foreach (var a in PlayerGameEntity.QueryAsync( ))
			{
				var added = playersGame.TryAdd(a.playerId, a);
				Assert(added);
			}
		}

		public static async Task LoadWorldPlayers()
		{
#if CNV
			// Fetch all game players
			await foreach (var pw in PlayerWorldEntity.QueryAsync(World.world))
			{
				var playerId = pw.playerId;
				var pg = playersGame[ playerId];
				var player = new Player()
				{
					// Global info
					id = playerId,
					name = pg.playerName,
					avatarUrl = pg.avatarURL,
					discordUserName = pg.discordUserName,
					// Game info
					alliance = pw.alliance.GetValueOrDefault().AsUShort(),
					allianceRole = pw.allianceRole.GetValueOrDefault().AsByte(),
				};
				Player.all.Add(playerId, player);
				Player.playerByDiscordUserName.TryAdd(player.discordUserName,player );
				Player.playerByDiscordId.TryAdd(player.discordId,player);
				Player.nameToId.TryAdd(player.name, player.id);
			}
			
			await foreach (var pw in PlayerGameEntity.QueryAsync())
			{
				var pid = pw.playerId;
				if (Player.all.TryGetValue(pid, out var player))
				{
					player.discordId = pw.discordId.GetValueOrDefault();
					player.playfabId = pw.playFabId.GetValueOrDefault();
				}
				else
				{
					Assert(false);
				}
			}

			#endif
		}
		// Fetch all world Players

	}
}
