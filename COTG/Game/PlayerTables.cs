using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	using CnVChat;
	using COTG;

	// Assume that world is set at this point
	internal static class PlayerTables
	{
		// We should ditch these after load
		internal static Dictionary<PlayerId, PlayerGameEntity> playersGame = new();

		public static async Task LoadGamePlayers()
		{
			// Fetch all game players

			await foreach (var a in PlayerGameEntity.QueryAsync( ))
			{
				playersGame.TryAdd(a.playerId, a);
			}
		}
		public static async Task LoadWorldPlayers()
		{
			// Fetch all game players

			await foreach (var pw in PlayerWorldEntity.QueryAsync(World.world))
			{
				var playerId = pw.playerId;
				var pg = playersGame[ playerId];
				var player = new Player()
				{
					// Global info
					discordId = pg.discordId.GetValueOrDefault(),
					id=playerId,
					name = pg.playerName,
					avatarUrl = pg.avatarURL,
					playfabId = pg.playFabId.GetValueOrDefault(),
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
		}
		// Fetch all world Players
	}
}
