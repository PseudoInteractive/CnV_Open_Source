/// <reference types="jquery" />
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
declare function GetAttackTime(source: string): Date;
declare function SendDef(defobj: any): void;
declare function updateattack(): void;
declare function updatedef(): void;
declare function SendAttack(): void;
declare function incomings(): void;
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
    type: number;
};
declare let mru: any[];
declare function hidecities(): void;
declare function showcities(): void;
declare function updateshrine(): void;
declare function DecodeWorldData(data: any): {
    bosses: any[];
    cities: any[];
    ll: any[];
    cavern: any[];
    portals: any[];
    shrines: any[];
};
declare function roundingto2_(num_6: any): number;
declare function TwoDigitNum(n_3: number): string;
declare function GetStringValue(a: JQuery<HTMLElement>): string;
declare function GetIntData(a: JQuery<HTMLElement>): number;
declare function GetFloatData(a: JQuery<HTMLElement>): number;
declare function GetFloatValue(a: JQuery<HTMLElement>): number;
declare function GetCidData(a: JQuery<HTMLElement>): Coord;
declare let ttts: number[];
declare let message_23: string;
declare var other_loot: number[];
declare var mountain_loot: number[];
declare let tpicdiv: string[];
declare let tpicdiv20: string[];
declare let ttspeed: number[];
declare let ttSpeedBonus: number[];
declare let TS_type_: number[];
declare let ttCombatBonus: number[];
declare let buildings_: {
    name: string[];
    bid: (string | number)[];
};
declare let sum_: boolean;
declare let bdcountshow_: boolean;
declare let bossdef_: number[];
declare let bossdefw_: number[];
declare let ttloot_: number[];
declare let ttattack_: number[];
declare let Res_: number[];
declare let ttname: string[];
declare let OGA: jsonT.Command[];
declare let wdata: any;
declare let PostgWrd: () => void;
declare let emptyspots_: string;
declare let beentoworld: boolean;
declare let pldata: any[];
declare let splayers: {
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
declare let testFlag: boolean;
declare var _cid: number;
declare var _viewMode: number;
declare let disablePoll: boolean;
declare let onResumePoll: {
    (): void;
}[];
declare var _viewModeCache: number;
declare const viewModeCity = 0;
declare const viewModeRegion = 1;
declare const viewModeWorld = 2;
declare const viewModeInvalid = 8;
declare var pollthis: any;
declare let city: jsonT.City;
declare let pollJ: jsonT.Poll;
declare var P8: number;
declare function SetViewMode(mode: any): void;
declare function setTestFlag(flag: any): void;
declare let xcoord: number;
declare let ycoord: number;
declare function debounceArgs(func: () => void, timeout: number): (...args: any[]) => void;
declare function debounce(func: () => void, timeout: number): () => void;
declare let _zoom: number;
declare let _popupCountCache: number;
declare let _popupCount: number;
declare let _lastTooltip: any;
declare let popupSizeDirty: boolean;
declare function PopupOnDrag(): void;
declare function PopupMouseMove(ev: MouseEvent): void;
declare let mouseMoveOptions: AddEventListenerOptions;
declare const observer: IntersectionObserver;
declare function registerPopup(pop: Element): void;
declare function AppendPopup(html: string): void;
declare function setupSyncView(): void;
declare function RemovePopup(id: string): void;
declare function BuildPopup(node: Element): any;
declare let buildingRemap: {};
declare let tooltipped: any;
declare let popupIds: string[];
declare let popupClasses: string[];
declare var callSyncViewMode: () => void;
declare function DoSyncViewMode(): void;
declare var cid: number;
declare var gStCid: number;
declare let buildingInfo: (l6U: number, T6U: number, O6U: number, t6U: number, L6U: number) => void;
declare var a6: {
    ccazzx: {
        encrypt: (a: string, b: string, c: number) => string;
        decrypt: (a: string, b: string, c: number) => string;
    };
};
declare let __c: {
    sendchat: (n: any, m: any) => void;
    showreport: (reportId: any) => void;
};
declare var mainMapDiv: HTMLElement;
declare var gStphp: any;
declare var gStQuery: any;
declare var gStQueryCB: any;
declare let stayAlive: boolean;
declare let updateBuildQueue: any;
declare let _camera: {
    x: number;
    y: number;
};
declare var _cameraX: number;
declare var _cameraY: number;
declare const bspotHall = 220;
declare function AppendHtml(parent: string, html: string): JQuery;
declare function setCameraC(a: any, b: any): void;
declare function setStayAlive(on: any): void;
declare function PostChatIn(c: any): void;
declare let lastTroopsHome: number;
declare let lastCid: number;
declare function showNotification(W3V: string, i3V: string): void;
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
declare var gspotfunct: {
    shCit: (a: any) => void;
    infoPlay: (a: any) => void;
    chcity: (a: any) => void;
    alliancelink: (a: any) => void;
};
declare var setcitybind: any;
declare let wantFa: boolean;
declare let wantRs: boolean;
declare let lastSendMoveSlots: any;
declare function truncateToken(): void;
declare function ppdtChanged(__ppdt: any): void;
declare function reportAway(): void;
declare function gaFrep(E7c: any): void;
declare var cotgsubscribe: any;
declare function outer(): void;
declare namespace jsonT {
    interface Bq {
        bid: number;
        btype: number;
        bspot: number;
        slvl: number;
        elvl: number;
        ds: number;
        de: number;
        brep: number;
        btime?: number;
        pa?: number;
    }
    interface Tq {
        tid: number | string;
        ttype: number;
        bt: number;
        tc: number;
        ds: number;
        de: number;
        tl: number;
        tm: number;
        pa: number;
    }
    interface CityWrapper {
        citydata: City;
    }
    interface City {
        _id?: number;
        cid?: number;
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
        bq?: Bq[];
        tq?: Tq[];
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
        sts?: string;
    }
    interface Bd {
        bid?: number;
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
        clc?: {
            [key: string]: number[];
        };
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
declare var emptyspots: string;
declare var layoutsl: string[];
declare var layoutlol: string[];
declare var layoutldl: string[];
declare var layoutwol: string[];
declare var layoutwdl: string[];
declare var layouttsg: string[];
declare var layoutpol: string[];
declare var layoutsw: string[];
declare var layoutdf: string[];
declare var layouthul: string[];
declare var layoutshl: string[];
declare var remarksl: string[];
declare var remarksw: string[];
declare var remarkdf: string[];
declare var remarklol: string[];
declare var remarkldl: string[];
declare var remarkwol: string[];
declare var remarkwdl: string[];
declare var remarkhul: string[];
declare var remarkshl: string[];
declare var remarkpol: string[];
declare var remarktsg: string[];
declare var troopcounw: any[][];
declare var troopcound: any[][];
declare var troopcounl: any[][];
declare var troopcounlol: any[][];
declare var troopcounldl: any[][];
declare var troopcounwol: any[][];
declare var troopcounwdl: any[][];
declare var troopcounhul: any[][];
declare var troopcounshl: any[][];
declare var troopcounpol: any[][];
declare var troopcountsg: any[][];
declare var resw: any[][];
declare var resd: any[][];
declare var resl: any[][];
declare var reslol: any[][];
declare var resldl: any[][];
declare var reswol: any[][];
declare var reswdl: any[][];
declare var reshul: any[][];
declare var resshl: any[][];
declare var respol: any[][];
declare var restsg: any[][];
declare var notesl: string[];
declare var notesw: string[];
declare var notedf: string[];
declare var notelol: string[];
declare var noteldl: string[];
declare var notewol: string[];
declare var notewdl: string[];
declare var notehul: string[];
declare var noteshl: string[];
declare var notepol: string[];
declare var notetsg: string[];
declare function replaceAt(_this: string, index: number, char: any): string;
declare function setnearhub(): void;
declare function setinfantry(): void;
declare let __base64Encode: Function;
declare let __base64Decode: Function;
declare var encryptStr: string[];
declare var decryptStr: string[];
declare function encryptJs(req: any, k2v: any): string;
interface stringCType {
}
declare var shrinec: any[][];
declare var errz_: number;
declare function buildQTouch(__cid: number): void;
declare function errorgo_(j_: any): void;
declare function addToAttackSender(tid: any): void;
declare function GetCity(): jsonT.City;
declare let defaultHeaders: Record<string, string>;
declare function SetupHeaders(): string;
declare function Contains(a: string, b: string): boolean;
declare let updateTimeout: number;
declare let lastSentBq: number;
declare let bqInFlight: number;
declare let lastSentSts: string;
declare let lastSentBD: number;
declare function compareSts(): boolean;
declare function bdChecksum(bd: jsonT.Bd[]): number;
declare function bqChecksum(bd: jsonT.Bq[]): number;
declare function compareBD(): boolean;
declare function compareBq(): boolean;
declare function sendCityData(delayInMs: any): void;
declare function sendBuildingData(): void;
declare function sendchat(channel: string, message: string): void;
declare function gCPosted(): void;
declare function gWrdPosted(data: any): void;
declare function decwdata(data: any): {
    bosses: any[];
    cities: any[];
    ll: any[];
    cavern: any[];
    portals: any[];
    shrines: any[];
};
declare function UpdateResearchAndFaith(): void;
declare function jqclick(s: any): void;
declare function getview(): "city" | "world" | "region";
declare function avapost(url: string, args: string): void;
declare function avagetts(): string;
declare var raidSecret: string;
declare function onKeyDown(ev: KeyboardEvent): void;
declare function onKeyUp(ev: KeyboardEvent): void;
declare function canvasMouseDown(ev: MouseEvent): void;
declare let underMouse: Element;
declare function postMouseEvent(sx: string, sy: string, eventName: string, button: string, dx: string, dy: string): void;
declare function postppdt(): void;
declare function GetDate(jq: string): Date;
declare function viewcity(cid: string): void;
declare function setviewmode(mode: string): void;
declare function avactor(): void;
declare var __autodemoon_: boolean;
declare function callDemo(): void;
declare function setAutoDemo(_autodemoon: boolean): void;
declare function getFormattedTime(date_2: any): string;
declare function openreturnwin_(data_33: any): void;
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
declare function getbossinfo(): void;
declare function FormatMinutes(minutes_: number): string;
declare function openbosswin_(): void;
declare function bossele_(): void;
declare function recallraidl100_(): void;
declare function carrycheck_(): void;
declare function GetCarry(): number;
declare var countOverride: number;
declare var raidCount: number;
declare function carry_percentage_(total_loot_1: any): void;
declare function carry_percentage_2(total_loot_1: any): void;
declare function getDugRows_(): void;
declare function opensumwin_(): void;
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
declare function updatesupport_(data_43: any): void;
declare function updateraids_(data_44: any, turnc_: any): void;
declare function updateres_(data_45: any): void;
declare function Distance(__x0: number, __y0: number, __x1: number, __y1: number): number;
declare function DistanceC(__a: Coord, __x1: number, __y1: number): number;
declare function DistanceCC(__a: Coord, __b: Coord): number;
declare function updatetroops_(data_46: any, notes_3: any): void;
