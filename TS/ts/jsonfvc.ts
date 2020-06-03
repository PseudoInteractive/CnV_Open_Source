namespace jsoonTypes {
    export interface FCV {
        t: number; // type:  0 is forest,1 hill, 2 mountain, 3 is water
        l: number; // level 1..19
        x: number; // position
        y: number; // position
        p: string; // pecent complete (should be number)
        c: number;  // cid
        d: number; // distance
    }
}
/*
 [
		[
		
			{
				"t": 3,
				"l": 7,
				"x": 314,
				"y": 287,
				"p": "74.380",
				"c": 18809146,
				"d": 54.56
			},
			{
				"t": 3,
				"l": 7,
				"x": 298,
				"y": 274,
				"p": "79.930",
				"c": 17957162,
				"d": 55.01
			}
		],
		21627160
	]
 */