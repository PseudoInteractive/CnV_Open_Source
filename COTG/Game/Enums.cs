using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
    public static class Enum
    {

        public const int ttCount = 18;
        public const byte ttGuard = 0;
        public const byte ttBallista = 1;
        public const byte ttRanger = 2;
        public const byte ttTriari = 3;
        public const byte ttPriestess = 4;
        public const byte ttVanquisher = 5;
        public const byte ttSorcerer = 6;
        public const byte ttScout = 7;
        public const byte ttArbalist = 8;
        public const byte ttPraetor = 9;
        public const byte ttHorseman = 10;
        public const byte ttDruid = 11;
        public const byte ttRam = 12;
        public const byte ttScorpion = 13;
        public const byte ttGalley = 14;
        public const byte ttStinger = 15;
        public const byte ttWarship = 16;
        public const byte ttSenator = 17;

        public static bool IsRaider(int type) => ttBestDungeonType[type] != (byte)DungeonType.invalid;
        public static bool IsLandRaider(int type) => ttBestDungeonType[type] < (byte)DungeonType.water;
        public static bool IsWaterRaider(int type) => ttBestDungeonType[type] == (byte)DungeonType.water;


        // 255 means none,
        public readonly static string[] dungeonTypes = { "forest", "hill", "mountain", "water" };

        //  0 "guard",1 "ballista",2 "ranger",3 "triari", 
        //  4  "priestess",5 "vanquisher",6 "sorcerers",7 "scout", 
        //  8  "arbalist",9 "praetor",10 "horseman",11 "druid",
        //  12 "ram",13 "scorpion",14 "galley",15 "stinger",
        //  16 "warship",17 "senator"

        // gets filled in 
        public static float[] ttSpeedBonus = {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1,1,1,1,1 };
        public static float[] ttCombatBonus = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public static float cartTravel=10.0f;
        public static float shipTravel =5.0f;

        public readonly static byte[] ttBestDungeonType = { 255, 255, 2, 2, 2, 2, 1, 255, 0, 0, 0, 1, 255, 255, 3, 3, 3, 255 };
        public readonly static ushort[] ttTs = { 1, 10, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 10, 10, 100, 100, 400, 1 };
        public readonly static string[] ttName = { "guard", "ballista", "ranger", "triari", "priestess", "vanquisher", "sorcerers", "scout", "arbalist", "praetor", "horseman", "druid", "ram", "scorpion", "galley", "stinger", "warship", "senator" };
        public readonly static string[] ttNameWithCaps = { "Guard", "Ballista", "Ranger", "Triari", "Priestess", "Vanquisher", "Sorcerer", "Scout", "Arbalist", "Praetor", "Horseman", "Druid", "Ram", "Scorpion", "Galley", "Stinger", "Warship", "Senator" };

        public readonly static string[] ttNameWithCapsAndBatteringRam = { "Guard", "Ballista", "Ranger", "Triari", "Priestess", "Vanquisher", "Sorcerer", "Scout", "Arbalist", "Praetor", "Horseman", "Druid", "Battering Ram", "Scorpion", "Galley", "Stinger", "Warship", "Senator" };

        public readonly static string[] ttCategory = { null, "Art",
                                    "Inf", "Inf", "Inf", "Inf", "Inf",
                                    "Scout",
                                    "Cav","Cav","Cav","Cav",
                                    "Art","Art",
                                    "Nav","Nav","Nav",
                                    "Sen" };



        public readonly static bool[] ttArtillery =  { false, false, false, false, false, false, false, false, false, false, false, false, true, true, false, false, true, false };
        public readonly static bool[] ttNavy =       { false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, false };
        public readonly static int[] otherLoot = { 350, 1000, 4270, 15500, 32300, 56900, 117200, 198500, 297500, 441600 };
        public readonly static int[] mountainLoot = { 350, 960, 4100, 14900, 31000, 54500, 112500, 190500, 285500, 423500 };
        public readonly static bool[] ttBossKiller =  { false, false, false, false,
                                                        false, true, true, false,
                                                        false, true,true,true,
                                                        false,false,true,false,
                                                        true,false }; 

        public readonly static byte[] ttTravel = {   0, 30, 20, 20,
                                                    20, 20, 20, 8,
                                                    10, 10, 10, 10,
                                                    30, 30, 5, 5,
                                                    5, 40 };
        public readonly static short[] ttCarry = { 0, 0, 10, 20, 10, 10, 5, 0, 15, 20, 15, 10, 0, 0, 0, 1500, 3000, 1 };
        public readonly static short[] ttAttack = { 10, 50, 30, 10, 25, 50, 70, 10, 40, 60, 90, 120, 50, 150, 3000, 1200, 12000, 1 };

        public static float TTTravel(int type) { return ttTravel[type] / (ttSpeedBonus[type]); }

        //
        // Templates not working for me
        //
        public enum DungeonType : byte
        {
            forest=0,
            hill=1,
            mountain=2,
            water=3,
            invalid=255
        }

        public static int IndexOf(this short[] a, short b)
        {
            for (int i = 0; i < a.Length; ++i)
                if (a[i] == b)
                    return i;
            return -1;
        }
        public static int IndexOf(this string[] a, string b)
        {
            for (int i = 0; i < a.Length; ++i)
                if (a[i] == b)
                    return i;
            return -1;
        }
        public static int FindIndex(this byte[] a, byte b)
        {
            for (int i = 0; i < a.Length; ++i)
                if (a[i] == b)
                    return i;
            return -1;
        }
        public static int FindIndex(this int[] a, int b)
        {
            for (int i = 0; i < a.Length; ++i)
                if (a[i] == b)
                    return i;
            return -1;
        }

        public const byte reportAssault = 0;
        public const byte reportSiege = 1; // siege in history
        public const byte reportPlunder = 2;
        public const byte reportScout = 3;
        public const byte reportSieging = 4; // siege in progress
        public const byte reportPending = 5; 

        public const byte reportArt = 6;
        public const byte reportSen = 7;
        public const byte reportInf = 8;
        public const byte reportCav = 9;
        public const byte reportNavy = 10;
        public const byte reportAttackCount = 11;
        public const byte reportDefenseStart = 11;
        public const byte reportDefensePending = reportDefenseStart;
        public const byte reportDefenseStationed = reportDefenseStart+1;

        public static readonly string[] reportStrings =
        {
             "Assault",
             "Siege",
             "Plunder",
             "Scout",
             "Sieging",
             "Pending", // is this right?
             "Art",
             "Sen",
             "Inf",
             "Cav",
             "Nav",
             "Def Pending",
             "Def"
        };
        public static int GetReportType(string s)
        {
            return reportStrings.IndexOf(s);
        }

    }
}
