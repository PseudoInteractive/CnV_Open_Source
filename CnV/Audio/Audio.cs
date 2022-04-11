using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	using Microsoft.Xna.Framework.Media;

	public static class Audio
	{

		public static Microsoft.Xna.Framework.Media.Song[]? music;
		//	bool inputInitialized;

		const  int musicCount = 7;
		static int lastSongPlayed;
		public static void UpdateMusic()
		{
			try
			{

			if (Settings.musicVolume > 0 && !Sim.isSub)
			{
				if (music == null)
				{
					if (!AGame.contentLoadingStarted)
						return;
					music = new Song[musicCount];
					for (int i = 0; i < musicCount; ++i)
						music[i] = GameClient.instance.Content.Load<Song>($"Audio/music{i}");
				}

				MediaPlayer.Volume = Settings.musicVolume;
				if (MediaPlayer.State == MediaState.Stopped)
				{
					MediaPlayer.Play(music[AMath.random.Next(musicCount)]);
				}
			}
			else
			{
				if (MediaPlayer.State != MediaState.Stopped)
					MediaPlayer.Stop();
			}
			}
			catch(Exception e)
			{
				Log(e);
			
			}
		}
	}
}
