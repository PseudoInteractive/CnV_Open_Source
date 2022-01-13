using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TShockAPI.CLI;

namespace CnVDiscord
{
    public class ConfigFile
    {
        public string DiscordBotToken = "NzgzNTU3MDAzNTYwNjE1OTc2.X8ceUw.My9T4SLadfgdVFLHUOosxnuu4Oo";
        public string Prefix = "/";
		public ulong ChatID = 0;// 863918043388969012;
        public ulong LogID = 0;
		public ulong CommandID = 0;


        public bool Chat = true;
        public bool Commands = false;


        //public List<ulong> OffRoles = new List<ulong>();

        //public List<ulong> BanRoles = new List<ulong>();

        //public List<ulong> KickRoles = new List<ulong>();
        //public List<ulong> MuteRoles = new List<ulong>();

        //public List<ulong> ListRoles = new List<ulong>();
        //public List<ulong> InfoRoles = new List<ulong>();
        //public List<ulong> SafeRoles = new List<ulong>();

        public string DiscordToCnVFormat = "{0}: {1}";
        public string CnVToDiscordFormat = "{0}: {1}";
        public int[] Messagecolor = { 0, 102, 204 };


        public static ConfigFile Read(string path)
        {
            if (!File.Exists(path))
            {
                ConfigFile config = new ConfigFile();

                File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
                return config;
            }
            return JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(path));
        }
    }
}
