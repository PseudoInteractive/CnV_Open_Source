using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV.GameData
{
    class GetIO
    {
        public partial class Root
        {
            public object[] @out { get; set; }
            public Inc[] inc { get; set; }
            public El[] el { get; set; }
        }

        public partial class El
        {
            public string cn { get; set; }
            public long Cid { get; set; }
            public string Crd { get; set; }
            public string Pl { get; set; }
            public long Pt { get; set; }
            public long Plvl { get; set; }
            public string Ti { get; set; }
            public long Wm { get; set; }
            public long Sm { get; set; }
            public long Pr { get; set; }
            public long Dpr { get; set; }
            public string Pnot { get; set; }
            public long Pid { get; set; }
            public long C { get; set; }
            public long Tt { get; set; }
            public string Dat { get; set; }
        }

        public partial class Inc
        {
            public long mg { get; set; }
            public long Tco { get; set; }
            public string Ty { get; set; }
            public long Tpi { get; set; }
            public string Tpn { get; set; }
            public string Tcn { get; set; }
            public string Txy { get; set; }
            public string Spt { get; set; }
            public string Art { get; set; }
            public string Apn { get; set; }
            public string Aan { get; set; }
            public string Acn { get; set; }
            public string Axy { get; set; }
            public long Ats { get; set; }
            public string Dts { get; set; }
            public long B { get; set; }
            public long Tt { get; set; }
            public long St { get; set; }
        }

     
    }
}
/*
 {
    "out": [],
    "inc": [
        {
            "mg": 0,
            "tco": 22,
            "ty": "Sieging",
            "tpi": 57791,
            "tpn": "UndeadAlive",
            "tcn": "<span class=\"coordblink shcitt\" data=\"14024962\">1007<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"14024962\">C22 (258:214)<\/span>",
            "spt": "06:00:00 ",
            "art": "07:00:00 ",
            "apn": "Markulino",
            "aan": "Unidos-",
            "acn": "<span class=\"coordblink shcitt\" data=\"14942475\">C22 010<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"14942475\">C22 (267:228)<\/span>",
            "ats": 38433,
            "dts": "-",
            "b": 0,
            "tt": 1593586800,
            "st": 1593583200
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "Sieging",
            "tpi": 57791,
            "tpn": "UndeadAlive",
            "tcn": "<span class=\"coordblink shcitt\" data=\"14024962\">1007<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"14024962\">C22 (258:214)<\/span>",
            "spt": "06:00:00 ",
            "art": "07:00:00 ",
            "apn": "Markulino",
            "aan": "Unidos-",
            "acn": "<span class=\"coordblink shcitt\" data=\"15204623\">C22 017<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15204623\">C22 (271:232)<\/span>",
            "ats": 28,
            "dts": "-",
            "b": 0,
            "tt": 1593586800,
            "st": 1593583200
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "Sieging",
            "tpi": 57791,
            "tpn": "UndeadAlive",
            "tcn": "<span class=\"coordblink shcitt\" data=\"14024962\">1007<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"14024962\">C22 (258:214)<\/span>",
            "spt": "06:00:00 ",
            "art": "07:00:00 ",
            "apn": "Markulino",
            "aan": "Unidos-",
            "acn": "<span class=\"coordblink shcitt\" data=\"15073551\">C22 003<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15073551\">C22 (271:230)<\/span>",
            "ats": 20706,
            "dts": "-",
            "b": 0,
            "tt": 1593586800,
            "st": 1593583200
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "Sieging",
            "tpi": 63203,
            "tpn": "StoneRemnant",
            "tcn": "<span class=\"coordblink shcitt\" data=\"16580882\">22_01<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"16580882\">C22 (274:253)<\/span>",
            "spt": "06:22:25 ",
            "art": "07:09:33 ",
            "apn": "ck430017",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16515345\">C22_07<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16515345\">C22 (273:252)<\/span>",
            "ats": 4001,
            "dts": "-",
            "b": 36,
            "tt": 1593587373,
            "st": 1593584545
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "Internal Attack",
            "tpi": 61298,
            "tpn": "Falsestep",
            "tcn": "<span class=\"coordblink shcitt\" data=\"14614748\">Mayhem<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"14614748\">C22 (220:223)<\/span>",
            "spt": "03:54:13 ",
            "art": "09:19:51 ",
            "apn": "arve",
            "aan": "Phoenix",
            "acn": "<span class=\"coordblink shcitt\" data=\"14483687\">ah_05<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"14483687\">C22 (231:221)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593595191,
            "st": 1593575653
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 44771,
            "tpn": "GateKeeper",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17957133\">*New City<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17957133\">C22 (269:274)<\/span>",
            "spt": "03:36:46 ",
            "art": "10:00:00 ",
            "apn": "majmajor",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"17170694\">C22-010<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"17170694\">C22 (262:262)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593574606
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629447\">C226-1<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629447\">C22 (263:269)<\/span>",
            "spt": "05:24:09 ",
            "art": "10:00:00 ",
            "apn": "majmajor",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16974087\">C22-030<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16974087\">C22 (263:259)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593581049
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17694989\">C228-1<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17694989\">C22 (269:270)<\/span>",
            "spt": "04:35:44 ",
            "art": "10:00:00 ",
            "apn": "LostSoul",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"15335673\">C22 019<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15335673\">C22 (249:234)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593578144
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629451\">C223-1<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629451\">C22 (267:269)<\/span>",
            "spt": "23:51:20 06\/30",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16318720\">22_026<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16318720\">C22 (256:249)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593561080
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17694990\">Lena<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17694990\">C22 (270:270)<\/span>",
            "spt": "23:35:45 06\/30",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16253190\">22_002<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16253190\">C22 (262:248)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593560145
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629449\">C222-1<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629449\">C22 (265:269)<\/span>",
            "spt": "00:29:56 ",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16253189\">22_004<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16253189\">C22 (261:248)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593563396
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17694993\">Elena<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17694993\">C22 (273:270)<\/span>",
            "spt": "02:19:40 ",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16580878\">22_011<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16580878\">C22 (270:253)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593569980
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629455\">Brigitte<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629455\">C22 (271:269)<\/span>",
            "spt": "22:22:04 06\/30",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16384253\">22_027<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16384253\">C22 (253:250)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593555724
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17694991\">Alice<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17694991\">C22 (271:270)<\/span>",
            "spt": "23:56:37 06\/30",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16646399\">22_028<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16646399\">C22 (255:254)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593561397
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629448\">C224-2<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629448\">C22 (264:269)<\/span>",
            "spt": "00:29:56 ",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16253188\">22_008<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16253188\">C22 (260:248)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593563396
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17563922\">Ksantypa<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17563922\">C22 (274:268)<\/span>",
            "spt": "23:39:45 06\/30",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16187656\">22_007<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16187656\">C22 (264:247)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593560385
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629443\">C227-2<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629443\">C22 (259:269)<\/span>",
            "spt": "05:28:19 ",
            "art": "10:00:00 ",
            "apn": "majmajor",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"17039623\">C22-029<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"17039623\">C22 (263:260)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593581299
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629452\">C224-1<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629452\">C22 (268:269)<\/span>",
            "spt": "01:53:23 ",
            "art": "10:00:00 ",
            "apn": "Halebel",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"16449807\">22_012<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"16449807\">C22 (271:251)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593597600,
            "st": 1593568403
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17760517\">C223-2<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17760517\">C22 (261:271)<\/span>",
            "spt": "05:56:13 ",
            "art": "11:43:54 ",
            "apn": "Osbourne",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"17694994\">Fanta<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"17694994\">C22 (274:270)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593603834,
            "st": 1593582973
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 58189,
            "tpn": "MackDaddy",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17563911\">C225-1<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17563911\">C22 (263:268)<\/span>",
            "spt": "06:33:47 ",
            "art": "11:45:54 ",
            "apn": "Osbourne",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"17826066\">Canteloupe<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"17826066\">C22 (274:272)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593603954,
            "st": 1593585227
        },
        {
            "mg": 0,
            "tco": 12,
            "ty": "-",
            "tpi": 21410,
            "tpn": "Mantzikert",
            "tcn": "<span class=\"coordblink shcitt\" data=\"10682642\">C12 024<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"10682642\">C12 (274:163)<\/span>",
            "spt": "04:06:27 ",
            "art": "12:09:00 ",
            "apn": "Angel2302",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"10092833\">DOCE004<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"10092833\">C12 (289:154)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593605340,
            "st": 1593576387
        },
        {
            "mg": 0,
            "tco": 12,
            "ty": "-",
            "tpi": 21410,
            "tpn": "Mantzikert",
            "tcn": "<span class=\"coordblink shcitt\" data=\"10354961\">C12 038<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"10354961\">C12 (273:158)<\/span>",
            "spt": "04:34:03 ",
            "art": "12:09:00 ",
            "apn": "Angel2302",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"10092833\">DOCE004<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"10092833\">C12 (289:154)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593605340,
            "st": 1593578043
        },
        {
            "mg": 0,
            "tco": 12,
            "ty": "-",
            "tpi": 21410,
            "tpn": "Mantzikert",
            "tcn": "<span class=\"coordblink shcitt\" data=\"10551575\">C12 051<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"10551575\">C12 (279:161)<\/span>",
            "spt": "05:57:52 ",
            "art": "12:09:00 ",
            "apn": "Angel2302",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"9896224\">DOCE006<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"9896224\">C12 (288:151)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593605340,
            "st": 1593583072
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17694991\">Alice<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17694991\">C22 (271:270)<\/span>",
            "spt": "04:55:58 ",
            "art": "14:00:00 ",
            "apn": "LostSoul",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"15335681\">C22 023<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15335681\">C22 (257:234)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593612000,
            "st": 1593579358
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17694993\">Elena<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17694993\">C22 (273:270)<\/span>",
            "spt": "04:51:26 ",
            "art": "14:00:00 ",
            "apn": "LostSoul",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"15466750\">C22 017<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15466750\">C22 (254:236)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593612000,
            "st": 1593579086
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17629455\">Brigitte<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17629455\">C22 (271:269)<\/span>",
            "spt": "05:16:36 ",
            "art": "14:00:00 ",
            "apn": "LostSoul",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"15401216\">C22 021<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15401216\">C22 (256:235)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593612000,
            "st": 1593580596
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17694990\">Lena<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17694990\">C22 (270:270)<\/span>",
            "spt": "04:37:09 ",
            "art": "14:00:00 ",
            "apn": "LostSoul",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"15466745\">C22 012<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15466745\">C22 (249:236)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593612000,
            "st": 1593578229
        },
        {
            "mg": 0,
            "tco": 22,
            "ty": "-",
            "tpi": 2138,
            "tpn": "Scenarion",
            "tcn": "<span class=\"coordblink shcitt\" data=\"17563922\">Ksantypa<\/span>",
            "txy": "<span class=\"coordblink shcitt\" data=\"17563922\">C22 (274:268)<\/span>",
            "spt": "05:26:41 ",
            "art": "15:00:00 ",
            "apn": "LostSoul",
            "aan": "Horizon",
            "acn": "<span class=\"coordblink shcitt\" data=\"15204607\">C22 025<\/span>",
            "axy": "<span class=\"coordblink shcitt\" data=\"15204607\">C22 (255:232)<\/span>",
            "ats": 0,
            "dts": "-",
            "b": 0,
            "tt": 1593615600,
            "st": 1593581201
        }
    ],
    "el": [
        {
            "cn": "<span class=\"coordblink shcitt\" data=\"22086070\">*34New City<\/span>",
            "cid": 22086070,
            "crd": "<span class=\"coordblink shcitt\" data=\"22086070\">C34 (438:337)<\/span>",
            "pl": "brooklynotss",
            "pt": 7,
            "plvl": 0,
            "ti": "12:01:03 07\/03",
            "wm": 0,
            "sm": 0,
            "pr": 1,
            "dpr": 1,
            "pnot": "faith only, do not overfill",
            "pid": 21837,
            "c": 34,
            "tt": 1593777663,
            "dat": "0=10000000-0-11589770.5;"
        }
    ]
}
 */
