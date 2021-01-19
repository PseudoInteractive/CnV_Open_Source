namespace jsonT {
	export interface Bq {
		bid: number | string;
		btype: number;
		bspot: number;
		slvl: number;
		elvl: number;
		ds: number;
		de: number;
		brep: number;
		btime: number;
		pa: number;
	}

	export interface Tq {
		tid: number |string;
		ttype: number;
		bt: number;
		tc: number;
		ds: number;
		de: number;
		tl: number;
		tm: number;
		pa: number;
	}


	export interface City {
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
		r?: { [key: string]: R };
		st?: { [key: string]: number };
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
		ble?: { [key: string]: number };
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
		itu?: { [key: string]: Itu };
		foodwarning?: string;
		bmq?: any[];
		testbmini?: string;
		sts?:string;
	}

	export interface Bd {
		bid?: Bid;
		bl?: number;
		bu?: number;
		bd?: number;
		rt?: number;
		rh?: number;
		rbb?: { [key: string]: Rbb };
		ruh?: number;
		rdh?: number;
		rb?: Rb[];
		rbt?: { [key: string]: Rb };
		rtt?: Rbb[];
	}

	export type Bid=number|string;

	export interface Rb {
		b?: number;
		i?: number;
		a?: number;
		bl?: number;
		au?: number;
		ad?: number;
		add?: number;
		bt?: number;
	}

	export interface Rbb {
		rt?: number;
		rh?: number;
		ruh?: number;
		rdh?: number;
	}

	export interface Fg {
		t?: number;
		l?: number;
		n?: number;
	}

	export type Itu=any[]|number;


	export interface Prot {
		s?: number;
		e?: number;
	}

	export interface R {
		r?: number;
		g?: number;
	}

	export interface Tp {
		t?: number;
		s?: number;
	}
}