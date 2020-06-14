using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using static COTG.Debug;

namespace COTG.Game
{
    static class WorldHelper
    {
        static internal uint SubStrAsInt(this string s, int start, int count)
        {
            uint rv = default;
            for (int i = start; i < start + count; ++i)
            {
                rv *= 10;
                var c = i < s.Length ? s[i] : '0';
                (c >= '0').Should().BeTrue();
                (c <= '9').Should().BeTrue();
                rv += (uint)(c - '0');
            }
            return rv;
        }
        static internal ushort SubStrAsShort(this string s, int start, int count)
        {
            return (ushort)SubStrAsInt(s, start, count);
        }
        static internal byte SubStrAsByte(this string s, int start, int count)
        {
            return (byte)SubStrAsInt(s, start, count);
        }

    }

    public class World
    {
        public static World current;

        public struct Boss
        {
            public byte level;
            public ushort x;
            public ushort y;
 

        }
        public Boss[] bosses;
        public struct City
        {
            public int playerId;
            public ushort x;
            public ushort y;
            public byte type;
            public bool isCastle() => type switch
            {
                3 => true,
                4 => true,
                7 => true,
                8 => true,
                _ => false

            };


        }
        public City[] cities;

        static ulong AsNumber(string s) => ulong.Parse(s);

        public static void UpdateCurrent(JsonDocument jsd)
        {
            current = Decode(jsd);
        }

        public static World Decode(JsonDocument jsd)
        {
            var data = jsd.RootElement.GetProperty("a").GetString(); // we do at least one utf16 <-> utf8 round trip here
            List<City> cities = new List<City>(1024);
            List<Boss> bosses = new List<Boss>(128);
            var temp = data.Split("|");
            var keys = temp[1].Split("l");
            var ckey = AsNumber(keys[0]);
            var skey_ = AsNumber(keys[1]);
            var bkey_ = AsNumber(keys[2]);
            var lkey_ = AsNumber(keys[3]);
            var cavkey_ = AsNumber(keys[4]);
            var pkey_ = AsNumber(keys[5]);
            var cities_ = temp[0].Split("l");
            var shrines_ = temp[2].Split("l");
            var bosses_1 = temp[3].Split("l");
            var lawless_ = temp[4].Split("l");
            var caverns_ = temp[5].Split("l");
            var portals_ = temp[6].Split("l");
            var rv = new World();
            /** @type {string} */
            foreach (var id in bosses_1)
            {

                try
                {
                    if (id == "")
                        continue;
                    /** @type {string} */
                    var dat_ = AsNumber(id) + (bkey_);
                    /** @type {string} */
                    bkey_ = dat_;
                    var _t = dat_.ToString();
                    var b = new Boss() { x = _t.SubStrAsShort(6, 3), y = _t.SubStrAsShort(3, 3), level = _t.SubStrAsByte(0, 2) };
                    LogJS(b);
                    bosses.Add(b);
                }
                catch (Exception e)
                {
                    Log(e);
                }


            }
            var rand = new Random();
            foreach (var id in cities_)
            {
                try
                {
                    if (id == "")
                        continue;
                    /** @type {string} */
                    var dat_ = AsNumber(id) + (ckey);
                    /** @type {string} */
                    ckey = dat_;
                    var _t =  dat_.ToString();

                    var digitCount = _t.SubStrAsInt(10, 1);
                    var pid = (int)_t.SubStrAsInt(11, (int)digitCount);
                    var c=(new City() { x = _t.SubStrAsShort(7, 3), y = _t.SubStrAsShort(4, 3), playerId = pid, type = _t.SubStrAsByte(3, 1) });
                    if (pid == JSClient.jsVars.pid || rand.Next(32) == 0)
                    {
                        LogJS(c);
                        Log(_t);
                    }
                    cities.Add(c);
                }
                catch (Exception e)
                {
                    Log(e);
                }


            }
            //foreach (var id in lawless_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(lawless_[i_3]) + (lkey_);
            //	/** @type {string} */
            //	lkey_ = dat_;
            //	WorldData.ll.push(`3${dat_);
            //}
            //foreach (var id in caverns_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(caverns_[i_3]) + (cavkey_);
            //	/** @type {string} */
            //	cavkey_ = dat_;
            //	WorldData.cavern.push(`7${dat_);
            //}
            //foreach (var id in portals_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(portals_[i_3]) + (pkey_);
            //	/** @type {string} */
            //	pkey_ = dat_;
            //	WorldData.portals.push(`8${dat_);
            //}
            //foreach (var id in shrines_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(shrines_[i_3]) + (skey_);
            //	/** @type {string} */
            //	skey_ = dat_;
            //	WorldData.shrines.push(`9${dat_);
            //}
            // var ckey = __a6.ccazzx.encrypt(currentTime(), '1QA64sa23511sJx1e2', 256);
            // console.log(ckey);
            // var cdat = {
            // 	a: ckey
            // };
            //  console.log(JSON.stringify(cdat));
            //  console.log(cdat);
            // jQuery.ajax({
            // 	url: 'includes/pD.php',
            // 	type: 'POST',
            // 	async: true,
            // 	data: cdat,
            // 	success: function (data) {
            //     console.log(data);
            // 		pdata = (data);
            // 	}
            // });

            rv.cities = cities.ToArray();
            rv.bosses = bosses.ToArray();
            return rv;
        }

    }
}
