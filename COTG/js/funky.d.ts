/// <reference types="jquery" />
/// <reference types="jqueryui" />
declare function AsNumber(a: any): number;
declare function LocalStoreAsInt(__s: string, __def?: number): number;
declare function LocalStoreAsFloat(__s: string, __def?: number): number;
declare function GetContinent(x: number, y: number): number;
declare class Coord {
    a: number;
    static xy(x: number, y: number): Coord;
    constructor(a: number);
    get cid(): number;
    get x(): number;
    get y(): number;
    get text(): string;
    get cont(): number;
}
declare function GetCityContinent(_city: jsonT.City): number;
declare let host: string;
declare let hostOverview: string;
declare function OverviewPost(url: string, post?: object, onSuccess?: (a: any) => void): void;
declare function OverviewFetch(url: string, post?: object): Promise<string>;
declare function ToFloat(__s: any): number;
declare function ToInt(__s: any): number;
declare function LocalStoreSet(__s: string, a: number): void;
declare function LocalStoreSetBool(__s: string, a: boolean): void;
declare function LocalStoreGetBool(__s: string, __def?: boolean): boolean;
declare function TroopNameToId(__name: string): number;
declare class Command {
    cid: number;
    dist: number;
    isReal: boolean;
}
declare class TroopTypeInfo {
    id: number;
    home: number;
    total: number;
    speed: number;
    used: boolean;
}
declare class CommandInfo {
    c: Command[];
    i: TroopTypeInfo[];
    perc: number;
    ret: number;
    scouts: number;
    date: Date;
}
declare let commandInfo: CommandInfo;
declare function ResetCommandInfo(): CommandInfo;
/**
 * @return {Date}
 */
declare function GetAttackTime(source: any): Date;
/**
 * @return {void}
 */
declare function IssueCommandsAndReturnTroops(): void;
/**
 * @param {!Object} defobj_
 * @return {void}
 */
/**
 * @return {void}
 */
declare function updateattack_(): void;
/**
 * @return {void}
 */
declare function updatedef_(): void;
/**
 * @return {void}
 */
declare function SendAttack_(): void;
/**
 * @return {void}
 */
declare function incomings_(): void;
/**
 * @return {void}
 */
declare function hidecities_(): void;
declare let defaultMru: {
    cid: number;
    name: string;
    pin: number;
    misc0: number;
    misc1: number;
    notes: string;
    player: string;
    alliance: string;
    plvl: number;
    score: number;
    castle: number;
    water: number;
    bless: number;
    last: Date;
};
declare let mru: any[];
/**
 * @return {void}
 */
declare function showcities_(): void;
/**
 * @return {void}
 */
declare function updateshrine_(): void;
declare function DecodeWorldData(data: any): {
    bosses: any[];
    cities: any[];
    ll: any[];
    cavern: any[];
    portals: any[];
    shrines: any[];
};
/**
 * @param {number} num_6
 * @return {?}
 */
declare function roundingto2_(num_6: any): number;
/**
 * @param {number} n_3
 * @return {String}
 */
declare function TwoDigitNum(n_3: number): string;
declare function GetStringValue(a: JQuery<HTMLElement>): string;
declare function GetIntData(a: JQuery<HTMLElement>): number;
declare function GetFloatData(a: JQuery<HTMLElement>): number;
declare function GetFloatValue(a: JQuery<HTMLElement>): number;
declare function GetCidData(a: JQuery<HTMLElement>): Coord;
/** @type {!Array} */
declare let ttts_: number[];
/** @type {string} */
declare let message_23: string;
/** @type {!Array} */
declare let other_loot_: number[];
/** @type {!Array} */
declare let mountain_loot_: number[];
/** @type {!Array} */
declare let tpicdiv_: string[];
/** @type {!Array} */
declare let tpicdiv20_: string[];
/** @type {!Array} */
declare let ttspeed_: number[];
/** @type {!Array} */
declare let ttres_: number[];
/** @type {number} */
declare let ttspeedres_: number[];
/** @type {!Array} */
declare let TS_type_: number[];
/** @type {!Array} */
declare let Total_Combat_Research_: number[];
declare let buildings_: {
    name: string[];
    bid: (string | number)[];
};
/** @type {boolean} */
declare let sum_: boolean;
/** @type {boolean} */
declare let bdcountshow_: boolean;
/** @type {!Array} */
declare let bossdef_: number[];
/** @type {!Array} */
declare let bossdefw_: number[];
/** @type {!Array} */
declare let ttloot_: number[];
/** @type {!Array} */
declare let ttattack_: number[];
/** @type {!Array} */
declare let Res_: number[];
/** @type {!Array} */
declare let ttname_: string[];
/** @type {!Array} */
declare let layoutsl_: string[];
/** @type {!Array} */
declare let layoutsw_: string[];
/** @type {!Array} */
declare let layoutdf_: string[];
declare let cdata_: jsonT.City;
declare let resl_: any[][];
declare let OGA: jsonT.Command[];
declare let wdata_: any;
/** @type {!Array} */
declare let remarksl_: string[];
/** @type {!Array} */
declare let remarksw_: string[];
/** @type {!Array} */
declare let remarkdf_: string[];
/** @type {!Array} */
declare let troopcounw_: any[][];
/** @type {!Array} */
declare let troopcound_: any[][];
/** @type {!Array} */
declare let troopcounl_: any[][];
/** @type {!Array} */
declare let resw_: any[][];
/** @type {!Array} */
declare let resd_: any[][];
/** @type {!Array} */
declare let notesl_: string[];
/** @type {!Array} */
declare let notesw_: string[];
/** @type {!Array} */
declare let notedf_: string[];
/** @type {string} */
declare let emptyspots_: string;
/** @type {boolean} */
declare let beentoworld_: boolean;
declare let splayers_: {
    name: any[];
    ally: any[];
    cities: any[];
};
declare function InitCheckbox(v: string): void;
declare function Checked(id: string): boolean;
declare function IsChecked(a: JQuery<HTMLElement>): boolean;
declare function RoundTo2Digits(num_5: number): number;
declare const __s: string[];
declare function _s(id: any): string;
declare var __c: {
    D6: {};
    j71: {};
    F5F: {};
    sendchat: (n: any, m: any) => void;
    showreport: (reportId: any) => void;
};
declare var M8: HTMLElement;
declare var gStphp: any;
declare let _camera: {
    x: number;
    y: number;
};
declare var _cameraX: number;
declare var _cameraY: number;
declare function setCameraC(a: any, b: any): void;
declare function K5SS(): typeof globalThis;
declare function A0KK(): typeof globalThis;
declare function B0KK(...args: any[]): void;
declare function u0VV(): typeof globalThis;
declare function I0VV(...args: any[]): void;
declare function i011(): void;
declare namespace i011 {
    var J0EE: any;
    var J55: {
        b0q: (Y0q: any) => string;
    };
    var o55: (__stringId: any) => string;
    var S55: (__stringId: any) => string;
    var r3q: () => any;
    var Q0s: () => any;
    var T3q: () => any;
    var n5x: () => any;
    var y3q: {
        Q07: (n07: any, h07: any, c07: any) => any;
        H07: (v07: any, u07: any, e07: any) => any;
    };
    var T5x: () => any;
    var B6: IArguments;
    var s0s: {
        t0s: void;
    };
    var r5x: {
        N2Q: (X2Q: any, P2Q: any, E2Q: any) => any;
    };
    var C0s: () => any;
    var y6: () => any;
    var U3q: () => any;
    var d3q: () => any;
    var s6s: number;
    var R6: () => any;
}
declare function w5SS(...args: any[]): void;
declare function X0rr(): typeof globalThis;
declare function I0rr(...args: any[]): void;
declare function gspotfunct(): void;
declare var setcitybind: any;
declare function ppdtChanged(__ppdt: any): void;
declare function gaFrep(E7c: any): void;
declare var __a6: {};
declare var cotgsubscribe: any;
declare namespace jsonT {
    interface City {
        _id?: number;
        cid?: Coord;
        pn?: string;
        pid?: number;
        pan?: string;
        paid?: number;
        tt?: number;
        tu?: number;
        tc?: number[];
        th?: number[];
        te?: number[];
        w?: string;
        s?: number;
        x?: number;
        y?: number;
        c?: number;
        co?: number;
        cs?: number;
        tps?: Tp[];
        r?: {
            [key: string]: R;
        };
        st?: {
            [key: string]: number;
        };
        rb?: string;
        sb?: string;
        bd?: Bd[];
        bq?: any[];
        tq?: any[];
        lup?: number;
        citn?: string;
        lut?: number;
        cn?: string[];
        prot?: Prot;
        mBu?: number;
        coof?: number;
        fw?: number;
        fwarn?: number;
        trintr?: any[];
        trin?: any[];
        triin?: any[];
        fg?: Fg;
        gg?: number;
        hs?: number;
        crt?: number;
        shp?: number;
        ww?: number[];
        cbt?: number;
        mibt?: number;
        crth?: number;
        shph?: number;
        crtu?: number;
        shpu?: number;
        tr?: any[];
        oa?: any[];
        comm?: number;
        bab?: string;
        buldtimeleft?: number;
        lud?: number;
        mo?: Mo[];
        cartinfo?: string;
        lock?: number;
        ble?: {
            [key: string]: number;
        };
        ciupd?: number;
        trts?: number;
        trpl?: number;
        ctt?: number;
        ncs?: number;
        rcs?: number;
        ics?: number;
        fcs?: number;
        aatroopittest?: string;
        aispaid?: string;
        aaaaaa?: string;
        ot?: any[];
        msave?: number;
        musave?: number;
        buto?: number;
        butu?: number;
        thlvlcheck?: number;
        buildtype2?: string;
        itu?: {
            [key: string]: Itu;
        };
        foodwarning?: string;
        bmq?: any[];
        testbmini?: string;
    }
    interface Bd {
        bid?: Bid;
        bl?: number;
        bu?: number;
        bd?: number;
        rt?: number;
        rh?: number;
        rbb?: {
            [key: string]: Rbb;
        };
        ruh?: number;
        rdh?: number;
        rb?: Rb[];
        rbt?: {
            [key: string]: Rb;
        };
        rtt?: Rbb[];
    }
    type Bid = number | string;
    interface Rb {
        b?: number;
        i?: number;
        a?: number;
        bl?: number;
        au?: number;
        ad?: number;
        add?: number;
        bt?: number;
    }
    interface Rbb {
        rt?: number;
        rh?: number;
        ruh?: number;
        rdh?: number;
    }
    interface Fg {
        t?: number;
        l?: number;
        n?: number;
    }
    type Itu = any[] | number;
    interface Prot {
        s?: number;
        e?: number;
    }
    interface R {
        r?: number;
        g?: number;
    }
    interface Tp {
        t?: number;
        s?: number;
    }
}
declare namespace jsonT {
    interface Poll {
        lastct?: number;
        cssrsn?: number;
        cssrs?: number;
        player?: Player;
        alliance?: Alliance;
        server?: number;
        mail?: number;
        RI?: {
            [key: string]: number;
        };
        OGT?: Array<OGTElement[]>;
        OGR?: any[];
        OGA?: Array<Command>;
        iNt?: {
            [key: string]: number;
        };
        ICC?: any[];
        EL?: number;
        AL?: number;
        OC?: number;
        IC?: number;
        AIC?: number;
        AOC?: number;
        rep?: number;
        city?: City;
        cth?: number;
        chh?: number;
        cid?: number;
        resregion?: number;
    }
    type Command = [number, string, number, number, string, string, string, number, number, any, number, string, number];
    type OGTElement = OGTClass | number | string;
    interface OGTClass {
        crt?: number;
        shp?: number;
        a?: number;
        b?: number;
        c?: number;
        d?: number;
        g?: number;
    }
    interface Alliance {
        id?: number;
        n?: string;
        ab?: string;
        fa?: {
            [key: string]: number;
        };
        m?: {
            [key: string]: MValue;
        };
        pr?: number;
        mc?: number;
        ic?: number;
        ip?: string;
        ep?: string;
        h?: H[];
        oi?: any[];
        d?: {
            [key: string]: D[];
        };
        r?: number[];
        st?: number;
    }
    interface D {
        id?: string;
        n?: string;
    }
    interface H {
        n?: string;
        i?: string;
        pn?: string;
        t?: number;
    }
    interface MValue {
        r?: number;
        n?: string;
        pid?: Lc;
        j?: number;
        lty?: number;
        lti?: number;
    }
    type Lc = number | string;
    interface Fg {
        t?: number;
        l?: number;
        n?: number;
    }
    type Mo = number[] | number;
    interface Player {
        _id?: number;
        pn?: string;
        pid?: number;
        lcit?: string;
        r?: number;
        t?: string;
        sc?: number;
        g?: G;
        m?: MsClass;
        ms?: MsClass;
        cac?: number;
        cc?: number;
        bc?: number;
        b?: number;
        bi?: number;
        bq?: number;
        pr?: {
            [key: string]: number;
        };
        cg?: any[];
        rs?: {
            [key: string]: PlayerR;
        };
        rw?: Rw[];
        ts?: number;
        lc?: {
            [key: string]: Lc;
        };
        cob?: {
            [key: string]: number;
        };
        cobm?: {
            [key: string]: number;
        };
        npp?: number;
        ft?: number[];
        opt?: {
            [key: string]: Lc;
        };
        hlp?: number[];
        arc?: number;
        lock?: number;
        nppf?: string;
        planame?: string;
        cb?: number;
        prot?: PlayerProt;
        fa?: {
            [key: string]: number;
        };
        paid?: number;
        fwc?: number;
        sco?: number;
        mibt?: number;
        alatitties?: number;
        td?: Mvb;
        mvb?: Mvb;
        mats?: number;
        lrct?: number;
        repcnt?: number;
        crw?: number;
        pccount?: number;
        gr?: number;
        fec?: number;
        specse?: number;
        gra?: Gra;
        itc?: {
            [key: string]: number;
        };
        tcps?: any[];
        acra?: number;
        acr?: number;
        suba?: number;
        subb?: number;
        subc?: number;
    }
    interface G {
        t?: number;
        b?: number;
    }
    interface Gra {
        t?: string;
    }
    interface MsClass {
        t?: number;
        b?: number;
        r?: number;
        f?: number;
    }
    interface Mvb {
        t?: number;
        l?: number;
    }
    interface PlayerProt {
        s?: number;
        e?: number;
        sf?: number;
        ef?: number;
    }
    interface PlayerR {
        n?: number;
        l?: number;
    }
    interface Rw {
        p?: number;
        l?: number;
    }
}
declare let __base64Encode: Function;
declare let __base64Decode: Function;
declare var encryptStr: string[];
declare var decryptStr: string[];
declare var cid: number;
declare function encryptJs(req: any, k2v: any): any;
declare function GetCity(): jsonT.City;
declare var __fetch: (input: RequestInfo, init?: RequestInit) => Promise<Response>;
declare var __debugMe: any;
declare let defaultHeaders: Record<string, string>;
declare function SetupHeaders(): string;
declare function Contains(a: string, b: string): boolean;
declare let updateTimeout: any;
declare function sendCityData(delayInMs: any): void;
declare function sendchat(channel: string, message: string): void;
declare function gCPosted(): void;
declare function gWrdPosted(data: any): void;
declare function __avatarAjaxDone(url: string, data: string): void;
declare function _pleaseNoMorePrefilters(): void;
declare function OptimizeAjax(): void;
declare function UpdateResearchAndFaith(): void;
declare function getppdt(): string;
declare function jqclick(s: any): void;
declare function getview(): "city" | "world" | "region";
declare function avapost(url: string, args: string): void;
declare function avagetts(): string;
declare function avafetch(url: string, args: string): Promise<string>;
declare function postppdt(): void;
declare function GetDate(jq: string): Date;
declare function SendAllianceInfo(): void;
declare function viewcity(cid: string): void;
declare function setviewmode(mode: string): void;
declare function avactor(): void;
/** @type {number} */
declare var __autodemoon_: boolean;
declare function callDemo(): void;
declare function setAutoDemo(_autodemoon: boolean): void;
/**
 * @param {!Object} data_33
 * @return {void}
 */
declare function openreturnwin_(data_33: any): void;
/**
 * @return {void}
 */
declare class Boss {
    cid: number;
    lvl: number;
    data: any;
    name: string;
    distance: number;
    minutes: number;
}
declare let bossinfo_: any[];
declare let bosslist_: any[];
declare function getbossinfo_(): void;
/**
 * @return {void}
 */
declare function FormatMinutes(minutes_: number): string;
declare function openbosswin_(): void;
/**
 * @return { void}
*/
declare function bossele_(): void;
/**
 * @return {void}
 */
declare function recallraidl100_(): void;
/**
 * @return {void}
 */
declare function carrycheck_(): void;
declare function GetCarry(): number;
declare var countOverride: number;
declare var raidCount: number;
/**
 * @param {number} total_loot_1
 * @return {void}
 */
declare function carry_percentage_(total_loot_1: any): void;
declare function carry_percentage_2(total_loot_1: any): void;
/**
 * @return {void}
 */
declare function getDugRows_(): void;
/**
 * @return {void}
 */
declare function PostMMNIO(j_12: any): void;
/**
 * @return {void}
 */
declare function setinfantry_(): void;
declare function GetRecentTabHTML(): string;
/**
 * @return {void}
 */
declare function opensumwin_(): void;
/**
 * @param {!Object} data_42
 * @param {!Object} notes_2
 * @return {void}
 */
declare function updateraidover_(data_42: any, notes_2: any, tsHome_: any): void;
declare var UrOA: string;
declare var ekeys: {
    "includes/sndRad.php": string;
    "includes/gRepH2.php": string;
    "includes/bTrp.php": string;
    "includes/gC.php": string;
    "includes/rMp.php": string;
    "includes/gSt.php": string;
    "includes/gWrd.php": string;
    "includes/UrOA.php": string;
    "includes/sndTtr.php": string;
};
declare function gamePost(req: any, data: any): void;
/**
 * @param {?} data_43
 * @return {void}
 */
declare function updatesupport_(data_43: any): void;
declare function updaterecent_(): void;
declare function UpdateFromRecent(): void;
/**
 * @param {!Object} data_44
 * @param {?} turnc_
 * @return {void}
 */
declare function updateraids_(data_44: any, turnc_: any): void;
/**
 * @param {?} data_45
 * @return {void}
 */
declare function updateres_(data_45: any): void;
declare function GetDonateHeader(): string;
/**
 * @param {?} data_45
 * @return {void}
 */
declare function Distance(__x0: number, __y0: number, __x1: number, __y1: number): number;
declare function DistanceC(__a: Coord, __x1: number, __y1: number): number;
declare function DistanceCC(__a: Coord, __b: Coord): number;
declare function SendDonation(): void;
declare function UpdateDonate(resData: any, blessData: any, filter: any): void;
/**
 * @param {?} data_46
 * @param {!Object} notes_3
 * @return {void}
 */
declare function updatetroops_(data_46: any, notes_3: any): void;
