using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json.Serialization;
using static COTG.Debug;
using COTG.Services;
using COTG.Game;
using COTG.Helpers;

namespace COTG.JSON
{
    public class IncomingOverview : OverviewApi
    {
        public static IncomingOverview instance = new IncomingOverview();

        public IncomingOverview() : base("overview/incover.php") { }
        public static int DecodeCid(string s)
        {
            var x = int.Parse(s.Substring(5, 3));
            var y = int.Parse(s.Substring(9, 3));
            return x + y * 65536;
        }
        public override void ProcessJson(JsonDocument jsd)
        {
            var jse = jsd.RootElement.GetProperty("b");
            List<Attack> attacks = new List<Attack>();
            foreach (var b in jse.EnumerateArray())
            {
                Attack attack = new Attack();
                attack.sourceCid = DecodeCid(b.GetString("attacker_locatuin"));
                attack.targetCid = DecodeCid(b.GetString("defender_location"));
                var spotted = b.GetString("spotted");
                var arrival= b.GetString("arrival");

                attack.spotted = DateTime.Parse(spotted);
                attack.time = DateTime.Parse(arrival);
                attacks.Add(attack);
                Log(attack.ToString());
            }
            Attack.attacks = attacks.ToArray();


        }
       
    }

}
/*
 * 
 */
