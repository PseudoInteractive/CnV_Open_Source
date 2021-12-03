using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CnV.Helpers;
using static CnV.Debug;
using static CnV.Game.Troops;
using System.Text.Json;
using CnV.Views;

namespace CnV.Game
{
	using Views;

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

        public bool isWater => type == (int)Troops.DungeonType.water;
        

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

		public float carry;
		public byte reps;
        public float loot => (type == (int)Troops.DungeonType.mountain ? mountainLoot[level - 1] : otherLoot[level - 1]) * (2 - completion * 0.01f);
        public string plan { get
            {
            //    var r = Raiding.ComputeIdealReps(this,city);
                return $"{(isValid?"":"[bad] ")}{reps}x{carry:P1} carry";
            }
        }
        public float GetScore( byte bestDungeonType)
        {
            // lower is better
            var rv = distance+2;
			rv *= .75f + 4*SettingsPage.raidCarryVsDistance*(carry - SettingsPage.raidCarryTarget).Abs();

            if (bestDungeonType != type)
                rv += SettingsPage.penaltyForWrongDungeonType; // penalty of 4 spaces for wrong type
			if (!isValid)
				rv += 100;
			return rv;

        }
		public bool isValid;
		public static async Task<bool> ShowDungeonList(City city, JsonElement jse, bool autoRaid)
		{
			var rv = new List<Dungeon>();
		//	rv.Clear();
			var idealType = city.GetIdealDungeonType();
			foreach (var dung in jse.EnumerateArray())
			{
				var type = dung.GetAsByte("t");
				if (Views.SettingsPage.raidOffDungeons || (type == idealType) || type == (byte)Troops.DungeonType.water )
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
					var r = Raiding.ComputeIdealReps(d, city);
					d.isValid = r.isValid;
					d.carry = r.averageCarry;
					d.reps = (byte)r.reps;
					if (d.isValid || !autoRaid)
						rv.Add(d);
				
					
				}
			}

			rv.Sort((a, b) => a.GetScore(idealType).CompareTo(b.GetScore(idealType)));
			
			if (autoRaid)
			{
				var sent = false;
				if (rv.Count>0)
				{
					foreach (var _i in rv)
					{
						if(!_i.isValid)
							continue;
						var i = _i;
						int counter = 0;

						var good = await Raiding.SendRaids(i);
						if (!good)
						{
							Note.Show($"Raid send failed for {city.nameMarkdown}, will try again");
						}
						sent = true;
						break;
					}
				}
				if (!sent)
				{
					Note.Show($"No appropriate dungeons for {city.nameMarkdown}");
				}
				return sent;
			}
			else
			{
				
				// dont wait on this 
				//COTG.Views.MainPage.UpdateDungeonList(rv);
				await DungeonView.Show(city, rv);
				return true;
			}
		}
	}
}
