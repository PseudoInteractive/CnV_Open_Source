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
        public static byte[] ttLandRaiders = { 2, 3, 4, 5, 6, 8, 9, 10, 11 };
        public static string[] ttName = { "guard", "ballista", "ranger", "triari", "priestess", "vanquisher", "sorcerers", "scout", "arbalist", "praetor", "horseman", "druid", "ram", "scorpion", "galley", "stinger", "warship", "senator" };
        public static string[] ttNameWithCaps = { "Guard", "Ballista", "Ranger", "Triari", "Priestess", "Vanquisher", "Sorcerer", "Scout", "Arbalist", "Praetor", "Horseman", "Druid", "Battering Ram", "Scorpion", "Galley", "Stinger", "Warship", "Senator" };

        public static int[] otherLoot = { 350, 1000, 4270, 15500, 32300, 56900, 117200, 198500, 297500, 441600 };
        public static int[] mountainLoot = { 350, 960, 4100, 14900, 31000, 54500, 112500, 190500, 285500, 423500 };

        public static byte[] ttspeed = { 0, 30, 20, 20, 20, 20, 20, 8, 10, 10, 10, 10, 30, 30, 5, 5, 5, 40, 40 };
        public static short[] ttCarry = { 0, 0, 10, 20, 10, 10, 5, 0, 15, 20, 15, 10, 0, 0, 0, 1500, 3000, 1};
        public static short[]  ttattack = { 10, 50, 30, 10, 25, 50, 70, 10, 40, 60, 90, 120, 50, 150, 3000, 1200, 12000, 1 };

        //
        // Templates not working for me
        //
      

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
    }
}
