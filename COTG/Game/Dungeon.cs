﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Helpers;
using static COTG.Debug;
using static COTG.Game.Enum;
using System.Text.Json;
using COTG.Views;

namespace COTG.Game
{
    public class Dungeon 
    {

		public static void Initialize()
		{
		}
		public string dispatch => "Dispatch";
		public City city; // city to raid this, where distance is calculated from 
        public int cid; // dungeon id
        public string xy => cid.CidToString();
        //          [JsonProperty("t")]
        public int type { get; set; }
        public string kind => type switch
        {
            0 => "forest",
            1 => "hill",
            2 => "mountain",
            _ => "water"
        };

        public bool isWater => type == (int)DungeonType.water;
        

        //        [JsonProperty("l")]
        public int level { get; set; }

        //       [JsonProperty("x")]
        public int x => cid % 65536;

        //      [JsonProperty("y")]
        public int y => cid / 65536;

        //     [JsonProperty("p")]
        public float completion { get; set; }


        //    [JsonProperty("d")]
        public float distance { get; set; }

        public float loot => (type == (int)DungeonType.mountain ? mountainLoot[level - 1] : otherLoot[level - 1]) * (2 - completion * 0.01f);
        public string plan { get
            {
                var r = Raiding.ComputeIdealReps(this,city);
                return $"{r.reps}x{r.averageCarry:P1}% carry";
            }
        }
        public float GetScore( byte bestDungeonType)
        {
            // lower is better
            var rv = distance;
            if (bestDungeonType != type)
                rv += SettingsPage.penaltyForWrongDungeonType; // penalty of 4 spaces for wrong type
			if (!isValid)
				rv += 100;
			return rv;

        }
		public bool isValid;
		public void ComputeIsValid()
		{
				var d = Raiding.ComputeIdealReps(this, city);
			isValid=(d.reps != 0 && (d.averageCarry < SettingsPage.raidCarryMax) && d.averageCarry > SettingsPage.raidCarryMin && completion > SettingsPage.minDungeonProgress);

		}
		public static async Task<bool> ShowDungeonList(City city, JsonElement jse, bool autoRaid)
		{
			var rv = new List<Dungeon>();
		//	rv.Clear();
			var idealType = city.GetIdealDungeonType();
			foreach (var dung in jse.EnumerateArray())
			{
				var type = dung.GetAsByte("t");
				if (Views.SettingsPage.raidOffDungeons || (type == idealType))
				{
					var d = new Dungeon()
					{
						city = city,
						cid = dung.GetAsInt("c"),
						type = type,
						level = dung.GetAsByte("l"),
						completion = dung.GetAsFloat("p"),
						distance = dung.GetAsFloat("d")

					};
					d.ComputeIsValid();
					if (d.isValid || !autoRaid)
						rv.Add(d);
				
					
				}
			}

			rv.Sort((a, b) => a.GetScore(idealType).CompareTo(b.GetScore(idealType)));
			
			if (autoRaid)
			{
				if(rv.Count>0)
				{
					var success = false;
					foreach (var _i in rv)
					{
						if(!_i.isValid)
							continue;
						var i = _i;
						int counter = 0;
						for(; ; )
						{ 
							var good = await Raiding.SendRaids(i, false);
							if (good)
								break;
							if(++counter > 8)
							{
								Note.Show($"Giving up on {city.nameAndRemarks}, please try again in a few min");
								break;
							}
							await Task.Delay(500);	
						}
						success = true;
						break;
					}
					if(!success)
					{
						Note.Show($"No appropriate dungeons for {city.nameAndRemarks}");
					}
				}
			}
			else
			{
				
				// dont wait on this 
				//COTG.Views.MainPage.UpdateDungeonList(rv);
				await DungeonView.Show(city, rv);
			}
			return true;
		}
	}
}
