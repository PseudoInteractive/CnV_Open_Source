namespace jsonT {
    export interface Poll {
        lastct?: number;
        cssrsn?: number;
        cssrs?: number;
        player?: Player;
        alliance?: Alliance;
        server?: number;
        mail?: number;
        RI?: { [key: string]: number };
        OGT?: Array<OGTElement[]>;
        OGR?: any[];
		OGA?: Array<Command>;
        iNt?: { [key: string]: number };
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

	export type Command=[number,string,number,number,string,string,string,number,number,any,number,string,number];



  
    export type OGTElement=OGTClass|number|string;

    export interface OGTClass {
        crt?: number;
        shp?: number;
        a?: number;
        b?: number;
        c?: number;
        d?: number;
        g?: number;
    }

    export interface Alliance {
        id?: number;
        n?: string;
        ab?: string;
        fa?: { [key: string]: number };
        m?: { [key: string]: MValue };
        pr?: number;
        mc?: number;
        ic?: number;
        ip?: string;
        ep?: string;
        h?: H[];
        oi?: any[];
        d?: { [key: string]: D[] };
        r?: number[];
        st?: number;
    }

    export interface D {
        id?: string;
        n?: string;
    }

    export interface H {
        n?: string;
        i?: string;
        pn?: string;
        t?: number;
    }

 

   /* export enum N {
        AllianceCreated="Alliance Created",
        Allied="Allied",
        Booted="Booted",
        EditedPreferences="Edited Preferences",
        Enemy="Enemy",
        Invited="Invited",
        Joined="Joined",
        Left="Left",
        Nap="NAP",
        Promoted="Promoted",
    }*/

    export interface MValue {
        r?: number;
        n?: string;
        pid?: Lc;
        j?: number;
        lty?: number;
        lti?: number;
    }

    export type Lc=number|string;

   

    export interface Fg {
        t?: number;
        l?: number;
        n?: number;
    }

    export type Mo=number[]|number;

   


    export interface Player {
        _id?: number; // ?
        pn?: string; // name
        pid?: number; // player id
        lcit?: string;
        r?: number; // Rank, 11 is Caesar
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
        pr?: { [key: string]: number };
        cg?: any[];
        rs?: { [key: string]: PlayerR };
        rw?: Rw[];
        ts?: number;
        lc?: { [key: string]: Lc };
        cob?: { [key: string]: number };
        cobm?: { [key: string]: number };
        npp?: number;
        clc?: { [key: string]: number[] };
        ft?: number[];
        opt?: { [key: string]: Lc }; // options, including security options and token
        hlp?: number[]; // help strings seen
        arc?: number;
        lock?: number;
        nppf?: string;
        planame?: string;  // alliance
        cb?: number;
        prot?: PlayerProt;
        fa?: { [key: string]: number };
        paid?: number; // has ministers?  Or paid at least once?
        fwc?: number;
        sco?: number;
        mibt?: number;
        alatitties?: number; // tities?
        td?: Mvb;
        mvb?: Mvb;
        mats?: number;
        lrct?: number;
        repcnt?: number;
        crw?: number;
        pccount?: number;
        gr?: number;
        fec?: number;
        specse?: number; // 1 == tournament
        gra?: Gra;
        itc?: { [key: string]: number };
        tcps?: any[];
        acra?: number;
        acr?: number;
        suba?: number;
        subb?: number;
        subc?: number;
    }

    export interface G {
        t?: number;
        b?: number;
    }

    export interface Gra {
        t?: string;
    }

    export interface MsClass {
        t?: number;
        b?: number;
        r?: number;
        f?: number;
    }

    export interface Mvb {
        t?: number;
        l?: number;
    }

    export interface PlayerProt {
        s?: number;
        e?: number;
        sf?: number;
        ef?: number;
    }

    export interface PlayerR {
        n?: number;
        l?: number;
    }

    export interface Rw {
        p?: number;
        l?: number;
    }


    
}
