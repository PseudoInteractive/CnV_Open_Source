
function AsNumber(a) { return a == undefined ? 0 : parseFloat(a.toString()); }


function LocalStoreAsInt( __s: string, __def: number = 0) {
	const rv = localStorage.getItem(__s);
	if (rv == null) {
		return __def;
	}
	return parseInt(rv);
}
function LocalStoreAsFloat(__s: string, __def: number= 0) {
	const rv = localStorage.getItem(__s);
	if (rv == null) {
		return __def;
	}
	return parseFloat(rv);
}
function GetContinent(x: number, y: number): number {
	return Math.floor(x / 100) + Math.floor(y / 100) * 10;
}

class Coord  {
	public static xy(x: number, y: number) {
		return new Coord(x + y * 65536);
	}
	constructor(public a: number) { }
	get cid(): number { return this.a ; }
	get x(): number {
		return this.a & 65535;
	}
	get y(): number {
		return this.a >> 16;
	}
	get text() {
		return `${this.x}:${this.y}`;
	}
	get cont(): number {
		return GetContinent(this.x, this.y);
	}

}
function GetCityContinent(_city: jsonT.City): number {
	return GetContinent(_city.x, _city.y);
}
let host = `https://w${window.worldidnumid}.crownofthegods.com`;
let hostOverview = `https://w${window.worldidnumid}.crownofthegods.com/overview.php?s=0`;

function OverviewPost(url: string, post?: object, onSuccess?: (a) => void ) {

	fetch(

		new Request(url, {
			method: "POST",
			headers: new Headers ({
				"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
				"pp-ss": ppss,
				referer: hostOverview, // may be redundant
				//  'X-Requested-With':'XMLHttpRequest',
			})
			,

			//  redirect: 'follow',
			referrer: hostOverview,
			mode: "cors",
			cache: "no-cache",
			body: (post ? $.param(post) : null),
		}))
		.then((response) => onSuccess != null ? response.json() : null)
		.then((data) => onSuccess != null ? onSuccess(data) : null)
		.catch((err) => console.error("Error:", err));

}
function OverviewFetch(url: string, post?: object): Promise<string> {

	return fetch(

		new Request(url, {
			method: "POST",
			headers: new Headers({
				"Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
				"pp-ss": ppss,
				referer: hostOverview, // may be redundant
				//  'X-Requested-With':'XMLHttpRequest',
			})
			,

			//  redirect: 'follow',
			referrer: hostOverview,
			mode: "cors",
			cache: "no-cache",
			body: (post ? $.param(post) : null),
		})). then((response) => response.json());

}
function ToFloat(__s: any): number {
	if (!__s) {
		return 0;
	}
	return parseFloat(__s.toString());
}
function ToInt(__s: any) {
	if (!__s) {
		return 0;
	}
	return parseInt(__s.toString());
}
function LocalStoreSet(__s: string, a: number) {
	 localStorage.setItem(__s, a.toString());
}
function LocalStoreSetBool(__s: string, a: boolean ) {
	 localStorage.setItem(__s, a ? "1" : "0");
}
function LocalStoreGetBool(__s: string, __def: boolean = false) {
	const rv = localStorage.getItem(__s);
	if (rv == null) {
		return __def;
	}
	return rv == "1";
}
// import from jQuery;
function TroopNameToId(__name: string): number { return ttname_.indexOf(__name); }

class Command {
	public cid: number;
	public dist: number;

	// only used to attacks
	public isReal: boolean;
}
class TroopTypeInfo {
	public id: number;
	public home: number;
	public total: number;
	public speed: number; // cached for convienience
	public used: boolean;
}
class CommandInfo {

		 public c: Command [];
		 public i: TroopTypeInfo[];
		 public perc: number = 100;
		// dep: 0,
		 public ret: number = 0;
		 public scouts: number = 0;

		 public date = new Date();
	}
let commandInfo = new CommandInfo();
function ResetCommandInfo() {
		commandInfo = new CommandInfo();
		return commandInfo;
	}
	/**
	 * @return {Date}
	 */
function GetAttackTime(source) {
		const strs = source.split(" ");
		if (strs.length == 1) {
			strs.push(ServerDate.now().toLocaleDateString());
		} else {
			strs[1] = strs[1] + "/" + ServerDate.now().getFullYear();
		}
		return new Date(strs[0] + " " + strs[1]);
	}
	/**
	 * @return {void}
	 */


function SendDef(defobj) {
	$("#commandsPopUpBox").show();
	var commandtabs = $("#commandsdiv").tabs();
	commandtabs.tabs("option", "active", 2);
	$("#reintabb").trigger({ type: "click", originalEvent: "1" });
	var targets = defobj.targets;
	var tarnumb = defobj.targets.numb;
	var t = defobj.t;
	var maxdist = Math.max.apply(Math, targets.dist);
	var time;
	//galley defend
	if (t.type.indexOf(14) > -1) {
		if (t.use[t.type.indexOf(14)] == 1) {
			time = ttspeed[14] / ttSpeedBonus[14] * maxdist;
			//console.log(time);
			var gali = t.type.indexOf(14);
			if (defobj.dep == 0) {
				var galnumb = Math.floor(t.home[gali] / tarnumb);
			} else {
				var galnumb = Math.floor(t.tot[gali] / tarnumb);
			}
			var maxts = 0;
			for (let i in t.home) {
				if (i != gali) {
					if (t.use[i] == 1) {
						if (t.type[i] != 15) {
							if (defobj.dep == 0) {
								maxts += Math.floor(t.home[i] * ttts_[t.type[i]] / tarnumb);
							} else {
								maxts += Math.floor(t.tot[i] * ttts_[t.type[i]] / tarnumb);
							}
						}
					}
				}
			}
			if (maxts <= galnumb * 500) {
				t.amount[gali] = Math.ceil(maxts / 500);
				for (let i in t.home) {
					if (i != gali) {
						if (t.use[i] == 1) {
							if (defobj.dep == 0) {
								t.amount[i] = Math.floor(t.home[i] / tarnumb);
							} else {
								t.amount[i] = Math.floor(t.tot[i] / tarnumb);
							}
						}
					}
				}
			} else {
				var rat = galnumb * 500 / maxts;
				t.amount[gali] = galnumb;
				for (let i in t.home) {
					if (i != gali) {
						if (t.use[i] == 1) {
							if (t.type[i] != 15) {
								if (defobj.dep == 0) {
									t.amount[i] = Math.floor(rat * t.home[i] / tarnumb);
								} else {
									t.amount[i] = Math.floor(rat * t.tot[i] / tarnumb);
								}
							} else {
								if (defobj.dep == 0) {
									t.amount[i] = Math.floor(t.home[i] / tarnumb);
								} else {
									t.amount[i] = Math.floor(t.tot[i] / tarnumb);
								}
							}
						}
					}
				}
			}
		}
		// normal def
	} else {
		time = Math.max.apply(Math, t.speed) * maxdist;
		for (let i in t.home) {
			if (t.use[i] == 1) {
				if (defobj.dep == 0) {
					t.amount[i] = Math.floor(t.home[i] / tarnumb);
				} else {
					t.amount[i] = Math.floor(t.tot[i] / tarnumb);
				}
			}
		}
	}
	// sending def
	//console.log(t);
	var l = 0; var end = targets.x.length;
	function dloop() {
		for (let i in t.home) {
			if (t.use[i] == 1) {
				$("#reiIP" + t.type[i]).val(t.amount[i]);
			}
		}
		$("#reinxcoord").val(targets.x[l]);
		$("#reinycoord").val(targets.y[l]);
		setTimeout(function () {
			$("#reinfcoordgo").trigger({ type: "click", originalEvent: "1" });
		}, 100);
		$("#reinforcetimingselect").val(Number(defobj.dep) + 1).change();
		if ($("#defdeparture").val() > 0) {
			//console.log(defobj.min,defobj.hr,defobj.sec);
			var date = defobj.date + " " + defobj.hr + ":" + defobj.min + ":" + defobj.sec;
			$("#reinfotimeinp").val(date);
		}

	//	var event = jQuery.Event("logged");
//		event.user = "foo";
		//$("#reinforceGo").trigger({type:"click",originalEvent:"1"});
		$("#reinforceGo").trigger({
			type: "click",
			originalEvent: "1"
		});
		l++;
		if (l < end) {
			setTimeout(dloop, 1500);
		} else {
			$("#commandsPopUpBox").hide();
			setTimeout(function () {
				art();
			}, 4000);
		}
	}
	dloop();
	function art() { //setting return time for raids according to city view outgoing list
		//console.log(OGA);
		$("#commandsPopUpBox").hide();
		if (defobj.ret == 1) {
			jQuery(".toptdinncommtbl1:first")[0].click();
			setTimeout(function () {
				$("#outgoingPopUpBox").hide();
			}, 500);
			var minddate = new Date(2050, 0, 0);
			for (let i in OGA) {

				//console.log(targets.cstr,OGA[i][5]);
				if (targets.cstr.indexOf(OGA[i][5]) > -1) {

						var d = GetAttackTime( OGA[i][6] );
						//console.log(d,minddate);
						if (d < minddate) {
							minddate = d;
						}
					
				}
			}
			minddate.setHours(minddate.getHours() - defobj.rettime);
			//console.log(minddate);
			var retdate = getFormattedTime(minddate);
			//console.log(retdate);
			$("#raidrettimesela").val(3).change();
			$("#raidrettimeselinp").val(retdate);
			jQuery("#doneOGAll")[0].click();
			alert("Defense set and troops returned");
		} else {
			alert("Defense set");
		}
	}
}	/**
	 * @return {void}
	 */
function updateattack() {
	let counc = document.getElementById("warcouncbox");
	if (counc == null || counc.style === null || counc.style.display !== "")
		return;

	var t = { home: [], type: [] };
	var scouttts;
	for (let i in D6.tc) {
		if (D6.tc[i]) {
			if (Number(i) == 7) {
				scouttts = D6.tc[7];
			}
			else {
				t.home.push(D6.tc[i]);
				t.type.push(i);
			}
		}
	}
	var ttseltab = "<table><thead><th>Troop Type</th><th>Use for real</th><th>Use for fake</th></thead><tbody>";
	for (let i in t.home) {
		ttseltab += "<tr><td style='height:40px;'><div class='" + tpicdiv[t.type[i]] + "'></div></td><td style='text-align: center;'><input id='usereal" + t.type[i] + "' class='clsubopti' type='checkbox' checked></td>";
		ttseltab += "<td style='text-align: center;'><input id='usefake" + t.type[i] + "' class='clsubopti' type='checkbox' checked></td></tr>";
	}
	ttseltab += "</tbody></table>";
	$("#picktype").html(ttseltab);
}
function updatedef() {
	var t = { home: [], type: [] };
	for (let i in D6.tc) {
		if (D6.tc[i]) {
			t.home.push(D6.tc[i]);
			t.type.push(i);
		}
	}
	var ttseltab = "<table><thead><th>Troop Type</th><th>Use</th></thead><tbody>";
	for (let i in t.home) {
		//ttseltab+="<tr><td>"+ttname[t.type[i]]+"</td><td><input id='usereal"+t.type[i]+"' class='clsubopti' type='checkbox' checked></td>";
		ttseltab += "<tr><td style='height:40px;'><div class='" + tpicdiv[t.type[i]] + "'></div></td><td style='text-align: center;'><input id='usedef" + t.type[i] + "' class='clsubopti' type='checkbox' checked></td></tr>";
	}
	ttseltab += "</tbody></table>";
	$("#dpicktype").html(ttseltab);
}
function SendAttack() {
	$("#commandsPopUpBox").show();
	var commandtabs = $("#commandsdiv").tabs();
	var pvptabs = $("#pvpTab").tabs();
	jQuery("#pvptabb")[0].click();
	commandtabs.tabs("option", "active", 1);
	var targets = { x: [], y: [], real: [], dist: [], cstr: [] };
	var fakenumb = 0;
	var realnumb = 0;
	var tempx;
	var tempy;
	for (let i = 1; i < 16; i++) {
		if ($("#t" + i + "x").val()) {
			tempx = $("#t" + i + "x").val();
			tempy = $("#t" + i + "y").val();
			targets.x.push(tempx);
			targets.y.push(tempy);
			targets.cstr.push(tempx + ":" + tempy);
			targets.real.push($("#type" + i).val());
			if ($("#type" + i).val() == 1) { realnumb += 1; }
			else { fakenumb += 1; }
			targets.dist.push(Math.sqrt((tempx - D6.x) * (tempx - D6.x) + (tempy - D6.y) * (tempy - D6.y)));
		}
	}
	var scouttts;
	var t = { home: [], type: [], real: [], fake: [], speed: [], scoutfake: [], scoutreal: [] };
	for (let i in D6.tc) {
		if (D6.tc[i]) {
			if (Number(i) == 7) { scouttts = D6.tc[7]; }
			else {
				t.home.push(Math.ceil(D6.tc[i] * Number($("#perc").val()) / 100));
				t.type.push(Number(i));
				if ($("#usereal" + i).prop("checked") === true) {
					t.speed.push(ttspeed[i] / ttSpeedBonus[i]);
				} else { t.speed.push(0); }
			}
		}
	}
	var maxdist = Math.max.apply(Math, targets.dist);
	var time;
	var faketss;
	var fakeg;
	var tscbr = D6.tt;
	/*    if(tscbr<20000){faketss=200;fakeg=1;}
		else if(tscbr<40000){faketss=500;fakeg=1;}
		else if(tscbr<60000){faketss=800;fakeg=2;}
		else if(tscbr<80000){faketss=1000;fakeg=2;}
		else if(tscbr<100000){faketss=1200;fakeg=3;}
		else if(tscbr<120000){faketss=1600;fakeg=4;}
		else if(tscbr<160000){faketss=2000;fakeg=4;}
		else if(tscbr<200000){faketss=2500;fakeg=5;}
		else if(tscbr<240000){faketss=3000;fakeg=6;}
		else{faketss=3000;fakeg=6;}*/
	if (tscbr < 20000) { faketss = 1; fakeg = 1; }
	else if (tscbr < 40000) { faketss = 200; fakeg = 1; }//1 600, 2 1200, 3 1800, 4 2400, 5 3000
	else if (tscbr < 60000) { faketss = 500; fakeg = 1; }
	else if (tscbr < 80000) { faketss = 800; fakeg = 2; }
	else if (tscbr < 100000) { faketss = 1000; fakeg = 2; }
	else if (tscbr < 120000) { faketss = 1200; fakeg = 2; }
	else if (tscbr < 160000) { faketss = 1600; fakeg = 3; }
	else if (tscbr < 200000) { faketss = 2000; fakeg = 4; }
	else if (tscbr < 240000) { faketss = 2500; fakeg = 5; }
	else { faketss = 3000; fakeg = 5; }
	if (scouttts > 0) {
		if ($("#realtype").val() == 3 && $("#faketype").val() == 3) {
			if ($("#usereal14").prop("checked") === true) {
				if ($("#usefake14").prop("checked") === true) {
					t.scoutfake[0] = fakeg * 250;
					t.scoutreal[0] = Math.floor((scouttts - ((fakeg * 250) * fakenumb)) / realnumb);
				} else {
					t.scoutfake[0] = faketss / 2;
					t.scoutreal[0] = Math.floor((scouttts - ((faketss / 2) * fakenumb)) / realnumb);
				}
			} else if ($("#usereal14").prop("checked") !== true) {
				if ($("#usefake14").prop("checked") === true) {
					t.scoutfake[0] = fakeg * 250;
					t.scoutreal[0] = Math.floor((scouttts - ((fakeg * 250) * fakenumb)) / realnumb);
				} else {
					t.scoutfake[0] = faketss / 2;
					t.scoutreal[0] = Math.floor((scouttts - ((faketss / 2) * fakenumb)) / realnumb);
				}
			}
		}
		if ($("#realtype").val() == 3 && $("#faketype").val() != 3) {
			if ($("#usereal14").prop("checked") === true) {
				if ($("#usefake14").prop("checked") === true) {
					t.scoutreal[0] = Math.floor(scouttts / realnumb);
				} else {
					t.scoutreal[0] = Math.floor(scouttts / realnumb);
				}
			} else {
				t.scoutreal[0] = Math.floor(scouttts / realnumb);
			}
		}
		if ($("#realtype").val() != 3 && $("#faketype").val() == 3) {
			if ($("#usereal14").prop("checked") === true) {
				if ($("#usefake14").prop("checked") === true) {
					t.scoutfake[0] = fakeg * 250;
				} else {
					t.scoutfake[0] = faketss / 2;
				}
			} else if ($("#usereal14").prop("checked") !== true) {
				if ($("#usefake14").prop("checked") === true) {
					t.scoutfake[0] = fakeg * 250;
				} else {
					t.scoutfake[0] = faketss / 2;
				}
			}
		}
	}
	//galley attack
	if (t.type.indexOf(14) > -1 && $("#usereal14").prop("checked") === true) {
		time = ttspeed[14] / ttSpeedBonus[14] * maxdist;
		var gali = t.type.indexOf(14);
		var galnumb = Math.floor((t.home[gali] - fakeg * fakenumb) / realnumb);
		t.real[gali] = galnumb;
		t.fake[gali] = fakeg;
		var galcap = 500 * galnumb;
		var nongalts = 0;
		for (let i in t.home) {
			if (t.type[i] == 14 && t.type[i] == 17 && t.type[i] == 16) {
				if (t.type[i] == 14) {
					if ($("#usereal" + t.type[i]).prop("checked") === true) {
						if ($("#usefake" + t.type[i]).prop("checked") === true) {
							t.real[i] == 1;
							t.fake[i] == 1;
						} else {
							t.real[i] == 1;
							t.fake[i] == 0;
						}
					}
				}
				if (t.type[i] == 17) {
					if ($("#usereal" + t.type[i]).prop("checked") === true) {
						if ($("#usefake" + t.type[i]).prop("checked") === true) {
							if (t.home[i] >= fakenumb + realnumb) {
								t.fake[i] = 1;
								t.real[i] = 1;
							} else {
								t.fake[i] = 0;
								t.real[i] = 1;
							}
						} else {
							t.fake[i] = 0;
							t.real[i] = 1;
						}
					} else if ($("#usefake" + t.type[i]).prop("checked") === true) {
						t.real[i] = 0;
						t.fake[i] = 1;
					} else {
						t.real[i] = 0;
						t.fake[i] = 0;
					}
				}
				if (t.type[i] == 16) {
					if ($("#usereal" + t.type[i]).prop("checked") === true) {
						if ($("#usefake" + t.type[i]).prop("checked") === true) {
							t.fake[i] = Math.ceil(faketss * t.home[i]);
							t.real[i] = Math.floor((t.home[i] - t.fake[i] * fakenumb) / realnumb);
						} else {
							t.real[i] = Math.floor((t.home[i]) / realnumb);
						}
					}
				}
			}
		}
		for (let i in t.home) {
			if (Number(i) != gali && t.type[i] != 17) {
				if ($("#usereal" + t.type[i]).prop("checked") === true) {
					if ($("#usefake" + t.type[i]).prop("checked") === true) {
						nongalts += ttts_[t.type[i]] * (t.home[i] - Math.ceil((fakeg * 500) / ttts_[t.type[i]]) * fakenumb) / realnumb;
					} else { nongalts += ttts_[t.type[i]] * (t.home[i]) / realnumb; }
				}
			}
			if (t.type[i] == 17) {
				if ($("#usereal" + t.type[i]).prop("checked") === true) {
					if ($("#usefake" + t.type[i]).prop("checked") === true) {
						if (t.home[i] >= fakenumb + realnumb) {
							nongalts += 1;
						} else {
							nongalts += 1;
						}
					} else {
						nongalts += 1;
					}
				}
			}
		}
		var fakerat = 0;
		for (let i in t.home) {
			if (Number(i) != gali) {
				if ($("#usefake" + t.type[i]).prop("checked") === true) {
					fakerat += ttts_[t.type[i]] * t.home[i];
				}
			}
		}
		for (let i in t.home) {
			if (Number(i) != gali && t.type[i] != 17) {
				if ($("#usefake" + t.type[i]).prop("checked") === true) {
					t.fake[i] = Math.ceil((fakeg * 500) * t.home[i] / fakerat);
				}
			}
			// number of senators to send on each fake/reak attack
			if (t.type[i] == 17) {
				if ($("#usereal" + t.type[i]).prop("checked") === true) {
					if ($("#usefake" + t.type[i]).prop("checked") === true) {
						if (t.home[i] >= fakenumb + realnumb) {
							t.fake[i] = 1;
							t.real[i] = 1;
						} else {
							t.fake[i] = 0;
							t.real[i] = 1;
						}
					} else {
						t.fake[i] = 0;
						t.real[i] = 1;
					}
				} else if ($("#usefake" + t.type[i]).prop("checked") === true) {
					t.real[i] = 0;
					t.fake[i] = 1;
				} else {
					t.real[i] = 0;
					t.fake[i] = 0;
				}
			}
		}
		for (let i in t.home) {
			if (Number(i) != gali && t.type[i] != 17) {
				if ($("#usereal" + t.type[i]).prop("checked") === true) {
					if ($("#usefake" + t.type[i]).prop("checked") === true) {
						if (nongalts > galcap) {
							t.real[i] = Math.floor(galcap / nongalts * (t.home[i] - t.fake[i] * fakenumb) / realnumb);
						} else {
							t.real[i] = Math.floor((t.home[i] - t.fake[i] * fakenumb) / realnumb);
						}
					} else {
						if (nongalts > galcap) {
							t.real[i] = Math.floor(galcap / nongalts * (t.home[i]) / realnumb);
						} else {
							t.real[i] = Math.floor((t.home[i]) / realnumb);
						}
						t.fake[i] = 0;
					}
				}
			}
		}
	}
	else {
		var fakerat = 0;
		time = Math.max.apply(Math, t.speed) * maxdist;
		//Adding all the TS standing home with fake ticked into fakerat
		for (let i in t.home) {
			if (t.type[i] !== 17) {
				if ($("#usefake" + t.type[i]).prop("checked") === true) {
					fakerat += ttts_[t.type[i]] * t.home[i];
				}
			}
		}
		//getting fake TS proportions of each troop type in a fake attack and moving them into t.fake[i]
		for (let i in t.home) {
			if (t.type[i] != 17) {
				if ($("#usefake" + t.type[i]).prop("checked") === true) {
					t.fake[i] = Math.ceil(faketss * t.home[i] / fakerat);
				}
			}
		}
		//subracting fake numbers to get real numbers and distributing senator too
		for (let i in t.home) {
			if (t.type[i] != 17) {
				if ($("#usereal" + t.type[i]).prop("checked") === true) {
					if ($("#usefake" + t.type[i]).prop("checked") === true) {
						t.real[i] = Math.floor((t.home[i] - t.fake[i] * fakenumb) / realnumb);
					} else {
						t.real[i] = Math.floor((t.home[i]) / realnumb);
					}
				} else { t.real[i] = 0; }
			}
			//senator numbers
			if (t.type[i] == 17) {
				if ($("#usereal" + t.type[i]).prop("checked") === true) {
					if ($("#usefake" + t.type[i]).prop("checked") === true) {
						if (t.home[i] >= fakenumb + realnumb) {
							t.fake[i] = 1;
							t.real[i] = 1;
						} else {
							t.fake[i] = 0;
							t.real[i] = 1;
						}
					} else {
						t.fake[i] = 0;
						t.real[i] = 1;
					}
				} else if ($("#usefake" + t.type[i]).prop("checked") === true) {
					t.real[i] = 0;
					t.fake[i] = 1;
				} else {
					t.real[i] = 0;
					t.fake[i] = 0;
				}
			}
		}
	}
	var alltimes = { a: [], b: [], c: [], d: [] };
	var l = 0; var end = targets.x.length;
	function loop() {
		if (targets.real[l] == 1) {
			if ($("#realtype").val() == 0) {
				pvptabs.tabs("option", "active", 0);
				for (let i in t.real) {
					$("#assIP" + t.type[i]).val(t.real[i]);
				}
				$("#assaultxcoord").val(targets.x[l]);
				$("#assaultycoord").val(targets.y[l]);
				setTimeout(function () {
					jQuery("#assaultcoordgo")[0].click();
				}, 100);
				$("#assaulttimingselect").val(3).change();
				var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
				$("#assaulttimeinp").val(date);
				alltimes.a.push($("#assaulttraveltime").text());
				jQuery("#assaultGo")[0].click();
			}
			if ($("#realtype").val() == 1) {
				pvptabs.tabs("option", "active", 1);
				for (let i in t.real) {
					$("#sieIP" + t.type[i]).val(t.real[i]);
				}
				$("#siexcoord").val(targets.x[l]);
				$("#sieycoord").val(targets.y[l]);
				setTimeout(function () {
					jQuery("#siegecoordgo")[0].click();
				}, 100);
				$("#siegetimingselect").val(3).change();
				var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
				$("#siegetimeinp").val(date);
				alltimes.a.push($("#siegetraveltime").text());
				jQuery("#siegeGo")[0].click();
			}
			if ($("#realtype").val() == 2) {
				pvptabs.tabs("option", "active", 2);
				for (let i in t.real) {
					$("#pluIP" + t.type[i]).val(t.real[i]);
				}
				$("#pluxcoord").val(targets.x[l]);
				$("#pluycoord").val(targets.y[l]);
				setTimeout(function () {
					jQuery("#plundercoordgo")[0].click();
				}, 100);
				$("#plundertimingselect").val(3).change();
				var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
				$("#plundtimeinp").val(date);
				alltimes.a.push($("#plundtraveltime").text());
				$("#plunderGo").prop('disabled', false);
				jQuery("#plunderGo")[0].click();
			}
			if ($("#realtype").val() == 3) {
				pvptabs.tabs("option", "active", 3);
				for (let i in t.real) {
					$("#scoIP" + t.type[i]).val(t.real[i]);
				}
				$("#scoIP7").val(t.scoutreal[0]);
				$("#scoxcoord").val(targets.x[l]);
				$("#scoycoord").val(targets.y[l]);
				setTimeout(function () {
					jQuery("#scoutcoordgo")[0].click();
				}, 100);
				$("#scouttimingselect").val(3).change();
				var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
				$("#scouttimeinp").val(date);
				jQuery("#scoutGo")[0].click();
			}
		}
		if (targets.real[l] == 0) {
			if ($("#faketype").val() == 0) {
				pvptabs.tabs("option", "active", 0);
				for (let i in t.real) {
					$("#assIP" + t.type[i]).val(t.fake[i]);
				}
				$("#assaultxcoord").val(targets.x[l]);
				$("#assaultycoord").val(targets.y[l]);
				setTimeout(function () {
					jQuery("#assaultcoordgo")[0].click();
				}, 100);
				$("#assaulttimingselect").val(3).change();
				var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
				$("#assaulttimeinp").val(date);
				alltimes.a.push($("#assaulttraveltime").text());
				jQuery("#assaultGo")[0].click();
			}
			if ($("#faketype").val() == 1) {
				pvptabs.tabs("option", "active", 1);
				for (let i in t.real) {
					$("#sieIP" + t.type[i]).val(t.fake[i]);
				}
				$("#siexcoord").val(targets.x[l]);
				$("#sieycoord").val(targets.y[l]);
				setTimeout(function () {
					jQuery("#siegecoordgo")[0].click();
				}, 100);
				$("#siegetimingselect").val(3).change();
				var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
				$("#siegetimeinp").val(date);
				alltimes.a.push($("#siegetraveltime").text());
				jQuery("#siegeGo")[0].click();
			}
			if ($("#faketype").val() == 2) {
				pvptabs.tabs("option", "active", 2);
				for (let i in t.real) {
					$("#pluIP" + t.type[i]).val(t.fake[i]);
				}
				$("#pluxcoord").val(targets.x[l]);
				$("#pluycoord").val(targets.y[l]);
				setTimeout(function () {
					jQuery("#plundercoordgo")[0].click();
				}, 100);
				$("#plundertimingselect").val(3).change();
				var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
				$("#plundtimeinp").val(date);
				alltimes.a.push($("#plundtraveltime").text());
				$("#plunderGo").prop('disabled', false);
				jQuery("#plunderGo")[0].click();
			}
			if ($("#faketype").val() == 3) {
				if ($("#scoutick").prop("checked") === true) {
					pvptabs.tabs("option", "active", 3);
					$("#scoIP7").val(1);
					$("#scoIP14").val(30);
					$("#scoxcoord").val(targets.x[l]);
					$("#scoycoord").val(targets.y[l]);
					setTimeout(function () {
						jQuery("#scoutcoordgo")[0].click();
					}, 100);
					$("#scouttimingselect").val(3).change();
					var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
					$("#scouttimeinp").val(date);
					jQuery("#scoutGo")[0].click();
				}
				else {
					pvptabs.tabs("option", "active", 3);
					for (let i in t.real) {
						$("#scoIP" + t.type[i]).val(t.fake[i]);
					}
					$("#scoIP7").val(t.scoutfake[0]);
					$("#scoxcoord").val(targets.x[l]);
					$("#scoycoord").val(targets.y[l]);
					setTimeout(function () {
						jQuery("#scoutcoordgo")[0].click();
					}, 100);
					$("#scouttimingselect").val(3).change();
					var date = $("#attackDat").val() + " " + $("#attackHr").val() + ":" + $("#attackMin").val() + ":" + $("#attackSec").val();
					$("#scouttimeinp").val(date);
					jQuery("#scoutGo")[0].click();
				}
			}
		}
		l++;
		if (l < end) {
			setTimeout(loop, 1000);
		} else {
			setTimeout(function () {
				art();
			}, 4000);
		}
	}
	loop();
	function art() { //setting return time for raids according to city view attacks list
		//console.log(OGA);
		$("#commandsPopUpBox").hide();
		if ($("#retcheck").prop("checked") == true) {
			jQuery(".toptdinncommtbl1:first")[0].click();
			setTimeout(function () {
				$("#outgoingPopUpBox").hide();
			}, 500);
			var minddate = new Date(2050, 0, 0);
			for (let i in OGA) {
				//console.log(targets.cstr,OGA[i][5]);
				if (targets.cstr.indexOf(OGA[i][5]) > -1) {
						var d = GetAttackTime(OGA[i][6]);
						if (d < minddate) {
							minddate = d;
						}
					
				}
			}
			minddate.setHours(minddate.getHours() - Number($("#retHr").val()));
			//console.log(minddate);
			var retdate = getFormattedTime(minddate);
			//console.log(retdate);
			$("#raidrettimesela").val(3).change();
			$("#raidrettimeselinp").val(retdate);
			jQuery("#doneOGAll")[0].click();
			alert("Attack set and troops returned");
		} else {
			alert("Attack set");
		}
	}
}

//var pendingBuildUpdate = false;

//function makebuildcount_() {
//		pendingBuildUpdate =false;

//		$("#bdtable").remove();
//		const currentbd_ = {
//			name: [],
//			bid: [],
//			count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
//		};
//		let j_4;
//		/** @type {number} */
//		let bdtypecount_ = -1;
//		/** @type {number} */
//		let bdNumber_ = -1;
//		let i_16;
//		for (i_16 in D6.bd) {
//			if (buildings_.bid.indexOf(D6.bd[i_16].bid) > -1) {
//				if (currentbd_.bid.indexOf(D6.bd[i_16].bid) > -1) {
//					/** @type {number} */
//					j_4 = currentbd_.bid.indexOf(D6.bd[i_16].bid);
//					currentbd_.count[j_4] += 1;
//					/** @type {number} */
//					bdNumber_ = bdNumber_ + 1;
//				} else {
//					/** @type {number} */
//					bdtypecount_ = bdtypecount_ + 1;
//					/** @type {number} */
//					j_4 = buildings_.bid.indexOf(D6.bd[i_16].bid);
//					currentbd_.name[bdtypecount_] = buildings_.name[j_4];
//					currentbd_.bid[bdtypecount_] = buildings_.bid[j_4];
//					currentbd_.count[bdtypecount_] += 1;
//					/** @type {number} */
//					bdNumber_ = bdNumber_ + 1;
//				}
//			}
//		}
//		/** @type {string} */
//		let bdtable_ = "<table id='bdtable'><tbody><tr>";
//		for (i_16 in currentbd_.bid) {
//			if (i_16 < 9 || i_16 > 9 && i_16 < 19 || i_16 > 19 && i_16 < 29) {
//				/** @type {string} */
//				bdtable_ = `${bdtable_}<td style='text-align:center; width:30px; height:30px;'><div style='background-image: url(/images/city/buildings/icons/${currentbd_.name[i_16]}.png); background-size:contain;background-repeat:no-repeat;width:30px; height:30px;'></div>${AsNumber(currentbd_.count[i_16])}</td>`;
//			}
//			if (i_16 == 9 || i_16 == 19) {
//				/** @type {string} */
//				bdtable_ = `${bdtable_}</tr><tr>`;
//				/** @type {string} */
//				bdtable_ = `${bdtable_}<td style='text-align:center; width:30px; height:30px;'><div style='background-image: url(/images/city/buildings/icons/${currentbd_.name[i_16]}.png); background-size:contain;background-repeat:no-repeat;width:30px; height:30px;'></div>${AsNumber(currentbd_.count[i_16])}</td>`;
//			}
//		}
//		/** @type {string} */
//		bdtable_ = `${bdtable_}</tr></tbody></table>`;
//		$("#bdcountwin").html(bdtable_);
//		$("#numbdleft").html(bdNumber_.toLocaleString());
//	}

	/**
	 * @return {void}
	 */
function incomings_() {
		/** @type {!Array} */
		const speeeed_ = [];
		/** @type {number} */
		speeeed_[0] = 0;
		/** @type {number} */
		for (let i_17 = 1; i_17 < 201; i_17++) {
			speeeed_[i_17] = speeeed_[i_17 - 1] + 0.5;
		}
		/** @type {!Array} */
		const navyspeed_ = [];
		/** @type {!Array} */
		const scoutspeed_ = [];
		/** @type {!Array} */
		const cavspeed_ = [];
		/** @type {!Array} */
		const infspeed_ = [];
		/** @type {!Array} */
		const artspeed_ = [];
		/** @type {!Array} */
		const senspeed_ = [];
		let temp_1;
		for (let i_17 = 0; i_17 < speeeed_.length; ++i_17) {
			/** @type {number} */
			temp_1 = 5 / (1 + speeeed_[i_17] * 1.0 / 100);
			navyspeed_[i_17] = roundingto2_(temp_1);
			/** @type {number} */
			temp_1 = 8 / (1 + speeeed_[i_17] * 1.0 / 100);
			scoutspeed_[i_17] = roundingto2_(temp_1);
			/** @type {number} */
			temp_1 = 10 / (1 + speeeed_[i_17] * 1.0 / 100);
			cavspeed_[i_17] = roundingto2_(temp_1);
			/** @type {number} */
			temp_1 = 20 / (1 + speeeed_[i_17] * 1.0 / 100);
			infspeed_[i_17] = roundingto2_(temp_1);
			/** @type {number} */
			temp_1 = 30 / (1 + speeeed_[i_17] * 1.0 / 100);
			artspeed_[i_17] = roundingto2_(temp_1);
			/** @type {number} */
			temp_1 = 40 / (1 + speeeed_[i_17] * 1.0 / 100);
			senspeed_[i_17] = roundingto2_(temp_1);
		}
		$("#iaBody tr").each(function() {
			const tid_2 = GetIntData($(":nth-child(5)", this).children().children());
		 const sid_ = GetIntData($(":nth-child(10)", this).children());
			/** @type {number} */
			const tx_ = tid_2 % 65536;
			/** @type {number} */
			const sx_1 = sid_ % 65536;
			/** @type {number} */
			const ty_ = (tid_2 - tx_) / 65536;
			/** @type {number} */
			const sy_1 = (sid_ - sx_1) / 65536;
			/** @type {number} */
			const tcont_2 = Math.floor(tx_ / 100) + Math.floor(ty_ / 100) * 10;
			/** @type {number} */
			const scont_ = Math.floor(sx_1 / 100) + Math.floor(sy_1 / 100) * 10;
			/** @type {number} */
			const dist_ = Math.sqrt((ty_ - sy_1) * (ty_ - sy_1) + (tx_ - sx_1) * (tx_ - sx_1));
			const atime_ = $(":nth-child(6)", this).text();
			const stime_ = $(":nth-child(11)", this).text();
			/** @type {number} */
			let hdiff_ = parseInt(atime_.substring(0, 2)) - parseInt(stime_.substring(0, 2));
			/** @type {number} */
			let mdiff_ = parseInt(atime_.substring(3, 5)) - parseInt(stime_.substring(3, 5));
			/** @type {number} */
			const sdiff_ = parseInt(atime_.substring(6, 8)) - parseInt(stime_.substring(6, 8));
			/** @type {!Date} */
			const d_3 = new Date;
			let arrivaltimemonth_;
			let arrivaltimedate_;
			if (atime_.length === 14) {
				/** @type {number} */
				arrivaltimemonth_ = AsNumber(atime_.substring(9, 11));
				/** @type {number} */
				arrivaltimedate_ = AsNumber(atime_.substring(12, 14));
			} else {
				/** @type {number} */
				arrivaltimemonth_ = d_3.getMonth() + 1;
				/** @type {number} */
				arrivaltimedate_ = d_3.getDate();
			}
			let time_4;
			if (hdiff_ >= 0) {
				/** @type {number} */
				time_4 = 60 * hdiff_;
			} else {
				/** @type {number} */
				time_4 = (24 + hdiff_) * 60;
			}
			if ((atime_.length === 14 || stime_.length === 14) && hdiff_ > 0) {
				/** @type {number} */
				time_4 = time_4 + +1440;
				/** @type {number} */
				hdiff_ = hdiff_ + 24;
			}
			/** @type {number} */
			time_4 = time_4 + mdiff_;
			/** @type {number} */
			time_4 = time_4 + sdiff_ / 60;
			const ispeed_ = roundingto2_(time_4 / dist_);
			const nspeed_ = roundingto2_((time_4 - 60) / dist_);
			let locks_;
			let lockm_;
			let lockh_;
			if (sdiff_ >= 0) {
				/** @type {number} */
				locks_ = sdiff_;
			} else {
				/** @type {number} */
				locks_ = 60 + sdiff_;
				/** @type {number} */
				mdiff_ = mdiff_ - 1;
			}
			if (mdiff_ >= 0) {
				/** @type {number} */
				lockm_ = mdiff_;
			} else {
				/** @type {number} */
				lockm_ = 60 + mdiff_;
				/** @type {number} */
				hdiff_ = hdiff_ - 1;
			}
			if (hdiff_ >= 0) {
				/** @type {number} */
				lockh_ = hdiff_;
			} else {
				/** @type {number} */
				lockh_ = hdiff_ + 24;
			}
			const travelingts_ = TwoDigitNum(locks_);
			const travelingtm_ = TwoDigitNum(lockm_);
			const travelingth_ = TwoDigitNum(lockh_);
			/** @type {number} */
			let locktimeh_ = AsNumber(lockh_) + AsNumber(atime_.substring(0, 2));
			/** @type {number} */
			let locktimem_ = AsNumber(lockm_) + AsNumber(atime_.substring(3, 5));
			/** @type {number} */
			let locktimes_ = AsNumber(locks_) + AsNumber(atime_.substring(6, 8));
			if (locktimes_ > 59) {
				/** @type {number} */
				locktimes_ = locktimes_ - 60;
				/** @type {number} */
				locktimem_ = locktimem_ + 1;
			}
			if (locktimem_ > 59) {
				/** @type {number} */
				locktimem_ = locktimem_ - 60;
				/** @type {number} */
				locktimeh_ = locktimeh_ + 1;
			}
			if (locktimeh_ > 23) {
				/** @type {number} */
				locktimeh_ = locktimeh_ - 24;
				/** @type {number} */
				arrivaltimedate_ = arrivaltimedate_ + 1;
			}
			/** @type {!Array} */
			const atm1_ = [1, 3, 5, 7, 8, 10, 12];
			/** @type {!Array} */
			const atm2_ = [4, 6, 9, 11];
			if (atm1_.indexOf(arrivaltimemonth_) > 0) {
				if (arrivaltimedate_ > 31) {
					/** @type {number} */
					arrivaltimedate_ = 1;
				}
			}
			if (atm2_.indexOf(arrivaltimemonth_) > 0) {
				if (arrivaltimedate_ > 30) {
					/** @type {number} */
					arrivaltimedate_ = 1;
				}
			}
			if (arrivaltimemonth_ === 2) {
				if (arrivaltimedate_ > 28) {
					/** @type {number} */
					arrivaltimedate_ = 1;
				}
			}
			const addt_ = $(this);
			arrivaltimemonth_ = TwoDigitNum(arrivaltimemonth_);
			arrivaltimedate_ = TwoDigitNum(arrivaltimedate_);
			/** @type {string} */
			const newtd_ = "<td></td>";
			if (addt_.children().length === 14) {
				$(this).append(newtd_);
				$(":nth-child(15)", this).text(`${TwoDigitNum(locktimeh_)}:${TwoDigitNum(locktimem_)}:${TwoDigitNum(locktimes_)} ${arrivaltimemonth_}/${arrivaltimedate_}`);
				if ($(":nth-child(2)", this).text() == "Sieging") {
					$(":nth-child(15)", this).css("color", "red");
				}
			}
			if (addt_.children().length === 15) {
				$(this).append(newtd_);
				$(":nth-child(16)", this).text(`${travelingth_}:${travelingtm_}:${travelingts_}`);
				if ($(":nth-child(2)", this).text() == "Sieging") {
					$(":nth-child(16)", this).css("color", "red");
				}
			}
			if ($(":nth-child(2)", this).text() == "-") {
				/** @type {number} */
				const zns_ = navyspeed_.indexOf(nspeed_);
				/** @type {number} */
				const zss_ = scoutspeed_.indexOf(ispeed_);
				/** @type {number} */
				const zcs_ = cavspeed_.indexOf(ispeed_);
				/** @type {number} */
				const zis_ = infspeed_.indexOf(ispeed_);
				/** @type {number} */
				const zas_ = artspeed_.indexOf(ispeed_);
				/** @type {number} */
				const zsn_ = senspeed_.indexOf(ispeed_);
				if (tcont_2 == scont_) {
					if (ispeed_ > 30) {
						if (zsn_ == -1) {
							$(":nth-child(2)", this).text("Tower?/Sen");
						} else {
							$(":nth-child(2)", this).text(`senator ${speeeed_[zsn_]}%`);
						}
					}
					if (ispeed_ > 20 && ispeed_ <= 30) {
						if (zsn_ == -1 && zas_ == -1) {
							$(":nth-child(2)", this).text("Tower?/Art/Sen");
						}
						if (zsn_ == -1 && zas_ != -1) {
							$(":nth-child(2)", this).text(`Artillery ${speeeed_[zas_]}%`);
						}
						if (zsn_ != -1 && zas_ == -1) {
							$(":nth-child(2)", this).text(`Senator ${speeeed_[zsn_]}%`);
						}
						if (zsn_ != -1 && zas_ != -1) {
							$(":nth-child(2)", this).text(`Art ${speeeed_[zas_]}%/Sen ${speeeed_[zsn_]}%`);
						}
					}
					if (ispeed_ == 20) {
						$(":nth-child(2)", this).text("Inf 0%/Art 50%/Sen 100%");
					}
					if (ispeed_ >= 15 && ispeed_ < 20) {
						if (zis_ == -1 && zas_ == -1) {
							$(":nth-child(2)", this).text("Tower?/Inf &above");
						}
						if (zis_ == -1 && zas_ != -1) {
							$(":nth-child(2)", this).text(`Artillery ${speeeed_[zas_]}%`);
						}
						if (zis_ != -1 && zas_ == -1) {
							$(":nth-child(2)", this).text(`Infantry ${speeeed_[zis_]}%`);
						}
						if (zis_ != -1 && zas_ != -1) {
							$(":nth-child(2)", this).text(`Inf ${speeeed_[zis_]}%/Art ${speeeed_[zas_]}%`);
						}
					}
					if (ispeed_ >= 10 && ispeed_ < 15) {
						if (zis_ == -1 && zcs_ == -1) {
							$(":nth-child(2)", this).text("Tower?/Cav &above");
						}
						if (zis_ == -1 && zcs_ != -1) {
							$(":nth-child(2)", this).text(`Cav ${speeeed_[zcs_]}%`);
						}
						if (zis_ != -1 && zcs_ == -1) {
							$(":nth-child(2)", this).text(`Inf ${speeeed_[zis_]}%`);
						}
						if (zis_ != -1 && zcs_ != -1) {
							$(":nth-child(2)", this).text(`Cav ${speeeed_[zcs_]}%/Inf ${speeeed_[zis_]}%`);
						}
					}
					if (ispeed_ > 8 && ispeed_ < 10) {
						if (zcs_ == -1) {
							$(":nth-child(2)", this).text("Tower?/Cav &above");
						} else {
							$(":nth-child(2)", this).text(`Cav ${speeeed_[zcs_]}%`);
						}
					}
					if (ispeed_ > 5 && ispeed_ <= 8) {
						if (zss_ == -1 && zcs_ == -1) {
							$(":nth-child(2)", this).text("Tower?/Scout &above");
						}
						if (zss_ == -1 && zcs_ != -1) {
							$(":nth-child(2)", this).text(`Cav ${speeeed_[zcs_]}%`);
						}
						if (zss_ != -1 && zcs_ == -1) {
							$(":nth-child(2)", this).text(`Scout ${speeeed_[zss_]}%`);
						}
						if (zss_ != -1 && zcs_ != -1) {
							$(":nth-child(2)", this).text(`Scout ${speeeed_[zss_]}%/Cav ${speeeed_[zcs_]}%`);
						}
					}
					if (ispeed_ == 5) {
						$(":nth-child(2)", this).text("Navy 0%/Scout 60%/Cav 100%");
					}
					if (ispeed_ >= 4 && ispeed_ < 5) {
						if (zss_ == -1 && zns_ == -1) {
							$(":nth-child(2)", this).text("Tower?/scout &above");
						}
						if (zss_ == -1 && zns_ != -1) {
							$(":nth-child(2)", this).text(`Navy ${speeeed_[zns_]}%`);
						}
						if (zss_ != -1 && zns_ == -1) {
							$(":nth-child(2)", this).text(`Scout ${speeeed_[zss_]}%`);
						}
						if (zss_ != -1 && zns_ != -1) {
							$(":nth-child(2)", this).text(`Navy ${speeeed_[zns_]}%/Scout ${speeeed_[zss_]}%`);
						}
					}
					if (ispeed_ < 4) {
						if (zns_ == -1) {
							$(":nth-child(2)", this).text("Tower?/Navy &above");
						} else {
							$(":nth-child(2)", this).text(`Navy ${speeeed_[zns_]}%`);
						}
					}
				} else {
					if ($(":nth-child(1)", this).html()) {
						$(":nth-child(2)", this).text("Portal");
					} else {
						if (zns_ != -1) {
							$(":nth-child(2)", this).text(`Navy ${speeeed_[zns_]}%`);
						} else {
							$(":nth-child(2)", this).text("Tower?/Navy");
						}
					}
				}
			}
		});
	}
	/**
	 * @return {void}
	 */
function hidecities_() {
		$("#shrineTab tr").each(function() {
			if ($(this).attr("data") == "city") {
				$(this).hide();
			}
		});
	}
	/*

	*/
let defaultMru = {
		
		cid: 0,
		name: "",
		pin: 0,
		misc0: 0,
		misc1: 0,
		notes: "",
		player: "tbd",
		alliance: "tbd",
		plvl: 0,
		score:0,
		castle:0,
		water:0,
		bless:0
	};
let mru = [];
	/**
	 * @return {void}
	 */
function showcities_() {
		$("#shrineTab tr").each(function() {
			if ($(this).attr("data") == "city") {
				$(this).show();
			}
		});
	}
	/**
	 * @return {void}
	 */
	/// @todo

	 function updateshrine_() {

	 	/** @type {string} */
	 	var shrinetab_="<table id='shrineTab'><thead><th style='width:115px'>Change</th><th style='width:50px'>Chances</th><th>Distance</th><th>Player</th><th>City</th><th>Coords</th><th style='width:100px'>Alliance</th><th>score</th><th>Type</th></thead><tbody>";
	 	/** @type {number} */
	 	var ccounter_=0;
	 	/** @type {!Array} */
	 	var w_7=[];
	 	/** @type {number} */
	 	var wtot_=0;
	 	for(let i_30 in shrinec_) {
	 		if(i_30) {
	 			var k_2=splayers_.name.indexOf(shrinec_[i_30][1]);
	 			var j_8;
	 			for(j_8 in splayers_.cities[k_2]) {
	 				if(shrinec_[i_30][3]==splayers_.cities[k_2][j_8].b&&shrinec_[i_30][4]==splayers_.cities[k_2][j_8].c) {
	 					shrinec_[i_30][2]=splayers_.cities[k_2][j_8].h;
	 					if(shrinec_[i_30][9]==0) {
	 						shrinec_[i_30][7]=splayers_.cities[k_2][j_8].a;
	 					}
	 					shrinec_[i_30][8]=splayers_.ally[k_2];
	 				}
	 			}
	 			if(shrinec_[i_30][0]=="castle") {
	 				ccounter_++;
	 				if(ccounter_<17) {
	 					/** @type {number} */
	 					w_7[ccounter_]=shrinec_[i_30][7]/shrinec_[i_30][5];
	 					/** @type {number} */
	 					wtot_=wtot_+shrinec_[i_30][7]/shrinec_[i_30][5];
	 				}
	 			}
	 		}
	 	}
	 	for(var i_30 in w_7) {
	 		/** @type {number} */
	 		w_7[i_30]=Math.round(w_7[i_30]/wtot_*100);
	 	}
	 	/** @type {number} */
	 	ccounter_=0;
	 	for(let i_30 in shrinec_) {
	 		if(i_30) {
	 			/** @type {number} */
	 			var cid_5=shrinec_[i_30][4]*65536+AsNumber(shrinec_[i_30][3]);
	 			if(shrinec_[i_30][0]=="castle") {
	 				ccounter_++;
	 				if(ccounter_<17) {
	 					if(shrinec_[i_30][6]=="0") {
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<tr style='color:purple;'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button id='${i_30}' data='castle' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding-top: 3px;border-radius: 4px;'>City</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td>${ccounter_} - ${w_7[ccounter_]}% </td>`;
	 					} else {
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<tr style='color:green;'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button id='${i_30}' data='castle' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding-top: 3px;border-radius: 4px;'>City</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td>${ccounter_} - ${w_7[ccounter_]}% </td>`;
	 					}
	 				} else {
	 					if(ccounter_>=17&&ccounter_<21) {
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<tr><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button id='${i_30}' data='castle' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding-top: 3px;border-radius: 4px;'>City</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
	 						/** @type {string} */
	 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td>${ccounter_}</td>`;
	 					}
	 				}
	 			} else {
	 				if(shrinec_[i_30][6]=="0") {
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<tr style='color:grey;' data='city'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<button id='${i_30}' data='city' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding: 3px;border-radius: 4px;width:37px;'>Castle</button>`;
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td></td>`;
	 				} else {
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<tr style='color:#74A274;'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<button id='${i_30}' data='city' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding: 3px;border-radius: 4px;width:37px;'>Castle</button>`;
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
	 					/** @type {string} */
	 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td></td>`;
	 				}
	 			}
	 			/** @type {string} */
	 			shrinetab_=`${shrinetab_}<td>${RoundTo2Digits(shrinec_[i_30][5])}</td><td class='playerblink'>${shrinec_[i_30][1]}</td><td>${shrinec_[i_30][2]}</td><td class='coordblink shcitt' data='${cid_5}'>${shrinec_[i_30][3]}:${shrinec_[i_30][4]}</td><td class='allyblink'>${shrinec_[i_30][8]}</td><td>${shrinec_[i_30][7]}</td><td>${shrinec_[i_30][0]}</td></tr>`;
	 			if(ccounter_==20) {
	 				break;
	 			}
	 		}
	 	}
	 	/** @type {string} */
	 	shrinetab_=`${shrinetab_}</tbody></table>`;
	 	$("#shrinediv").html(shrinetab_);
	 	$("#shrineTab td").css("text-align","center");
	 	if(localStorage.getItem("hidecities")=="1") {
	 		hidecities_();
	 	}
	 	$(".shrinechange").click(function() {
	 		if($(this).attr("data")=="castle") {
	 			/** @type {string} */
	 			shrinec_[$(this).attr("id")][0]="city";
	 		} else {
	 			/** @type {string} */
	 			shrinec_[$(this).attr("id")][0]="castle";
	 		}
	 		if(shrinec_[$(this).attr("id")][6]=="0") {
	 			/** @type {number} */
	 			shrinec_[$(this).attr("id")][6]=1;
	 		} else {
	 			/** @type {number} */
	 			shrinec_[$(this).attr("id")][6]=0;
	 		}
	 		updateshrine_();
	 	});
	 	$(".shrineremove").click(function() {
	 		shrinec_.splice(GetIntData($(this)),1);
	 		updateshrine_();
	 	});
	 	$(".shrine7pt").click(function() {
	 		if(shrinec_[$(this).attr("data")][7]!=7) {
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][7]=7;
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][9]=1;
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][6]=1;
	 		} else {
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][9]=0;
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][6]=0;
	 		}
	 		updateshrine_();
	 	});
	 	$(".shrine10k").click(function() {
	 		if(shrinec_[$(this).attr("data")][7]!=10000) {
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][7]=10000;
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][9]=1;
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][6]=1;
	 		} else {
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][9]=0;
	 			/** @type {number} */
	 			shrinec_[$(this).attr("data")][6]=0;
	 		}
	 		updateshrine_();
	 	});
	 }

//	 * @param {string} data
//	 * @return {?}

function DecodeWorldData(data) {
		const WorldData = {
			bosses: [],
			cities: [],
			ll: [],
			cavern: [],
			portals: [],
			shrines: [],
		};
		const temp = data.split("|");
		const keys = temp[1].split("l");
		let ckey = keys[0];
		let skey_ = keys[1];
		let bkey_ = keys[2];
		let lkey_ = keys[3];
		let cavkey_ = keys[4];
		let pkey_ = keys[5];
		const cities_ = temp[0].split("l");
		const shrines_ = temp[2].split("l");
		const bosses_1 = temp[3].split("l");
		const lawless_ = temp[4].split("l");
		const caverns_ = temp[5].split("l");
		const portals_ = temp[6].split("l");
		/** @type {string} */
		let dat_;
		let i_3;
		for (i_3 in bosses_1) {
			/** @type {string} */
			dat_ = `${AsNumber(bosses_1[i_3]) + AsNumber(bkey_)}`;
			/** @type {string} */
			bkey_ = dat_;
			WorldData.bosses.push(`1${dat_}`);
		}
		for (i_3 in cities_) {
			/** @type {string} */
			dat_ = `${AsNumber(cities_[i_3]) + AsNumber(ckey)}`;
			/** @type {string} */
			ckey = dat_;
			WorldData.cities.push(`2${dat_}`);
		}
		for (i_3 in lawless_) {
			/** @type {string} */
			dat_ = `${AsNumber(lawless_[i_3]) + AsNumber(lkey_)}`;
			/** @type {string} */
			lkey_ = dat_;
			WorldData.ll.push(`3${dat_}`);
		}
		for (i_3 in caverns_) {
			/** @type {string} */
			dat_ = `${AsNumber(caverns_[i_3]) + AsNumber(cavkey_)}`;
			/** @type {string} */
			cavkey_ = dat_;
			WorldData.cavern.push(`7${dat_}`);
		}
		for (i_3 in portals_) {
			/** @type {string} */
			dat_ = `${AsNumber(portals_[i_3]) + AsNumber(pkey_)}`;
			/** @type {string} */
			pkey_ = dat_;
			WorldData.portals.push(`8${dat_}`);
		}
		for (i_3 in shrines_) {
			/** @type {string} */
			dat_ = `${AsNumber(shrines_[i_3]) + AsNumber(skey_)}`;
			/** @type {string} */
			skey_ = dat_;
			WorldData.shrines.push(`9${dat_}`);
		}
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

		return WorldData;
	}

	/**
	 * @param {number} num_6
	 * @return {?}
	 */
function roundingto2_(num_6) {
		return Math.round(num_6 * 100) / 100;
	}
	/**
	 * @param {number} n_3
	 * @return {String}
	 */
function TwoDigitNum(n_3: number): string {
		return n_3 > 9 ? `${n_3}` : `0${n_3}`;
	}
function GetStringValue(a: JQuery<HTMLElement>): string {
    return a.val().toString();
}

function GetIntData(a: JQuery<HTMLElement>) {
    return ToInt(a.attr("data"));
}
function GetFloatData(a: JQuery<HTMLElement>) {
	return ToFloat(a.attr("data"));
}
function GetFloatValue(a: JQuery<HTMLElement>) {
	return ToFloat(a.val());
}

function GetCidData(a: JQuery<HTMLElement>) {
	return new Coord(ToInt(a.attr("data")));
}
/** @type {!Array} */
let ttts_ = [1, 10, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 10, 10, 100, 100, 400, 1];

/** @type {string} */
let message_23 = "Not enough TS to kill this boss!";
/** @type {!Array} */
let other_loot_ = [350, 1000, 4270, 15500, 32300, 56900, 117200, 198500, 297500, 441600];
/** @type {!Array} */
let mountain_loot_ = [350, 960, 4100, 14900, 31000, 54500, 112500, 190500, 285500, 423500];
/** @type {!Array} */
let tpicdiv = ["guard32 trooptdcm", "bally32 trooptdcm", "ranger32 trooptdcm", "triari32 trooptdcm", "priest32 trooptdcm", "vanq32 trooptdcm", "sorc32 trooptdcm", "scout32 trooptdcm", "arbal32 trooptdcm", "praet32 trooptdcm", "horsem32 trooptdcm", "druid32 trooptdcm", "ram32 trooptdcm", "scorp32 trooptdcm", "galley32 trooptdcm", "sting32 trooptdcm", "wship32 trooptdcm", "senat32 trooptdcm"];
/** @type {!Array} */
let tpicdiv20_ = ["guard20 trooptdcm", "bally20 trooptdcm", "ranger20 trooptdcm", "triari20 trooptdcm", "priest20 trooptdcm", "vanq20 trooptdcm", "sorc20 trooptdcm", "scout20 trooptdcm", "arbal20 trooptdcm", "praet20 trooptdcm", "horsem20 trooptdcm", "druid20 trooptdcm", "ram20 trooptdcm", "scorp20 trooptdcm", "galley20 trooptdcm", "sting20 trooptdcm", "wship20 trooptdcm", "senat20 trooptdcm"];
/** @type {!Array} */
let ttspeed = [0, 30, 20, 20, 20, 20, 20, 8, 10, 10, 10, 10, 30, 30, 5, 5, 5, 40, 40];
/** @type {!Array} */
//let ttres_ = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
/** @type {number} */

let ttSpeedBonus = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
/** @type {!Array} */
let TS_type_ = [0, 0, 1, 1, 1, 1, 1, 0, 2, 2, 2, 2, 0, 0, 0, 100, 400];
/** @type {!Array} */
let ttCombatBonus = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];

let buildings_ = {
	name: ["forester", "cottage", "storehouse", "quarry", "hideaway", "farmhouse", "cityguardhouse", "barracks", "mine", "trainingground", "marketplace", "townhouse", "sawmill", "stable", "stonemason", "mage_tower", "windmill", "temple", "smelter", "blacksmith", "castle", "port", "port", "port", "shipyard", "shipyard", "shipyard", "townhall", "castle"],
	bid: [448, 446, 464, 461, 479, 447, 504, 445, 465, 483, 449, 481, 460, 466, 462, 500, 463, 482, 477, 502, "467", 488, 489, 490, 491, 496, 498, 455, 467],
};
/** @type {boolean} */
let sum_ = true;
/** @type {boolean} */
let bdcountshow_ = true;
/** @type {!Array} */
let bossdef_ = [625, 3750, 25000, 50000, 125000, 187500, 250000, 375000, 562500, 750000];
/** @type {!Array} */
let bossdefw_ = [425, 2500, 17000, 33000, 83000, 125000, 170000, 250000, 375000, 500000];
/** @type {!Array} */
let ttloot_ = [0, 0, 10, 20, 10, 10, 5, 0, 15, 20, 15, 10, 0, 0, 0, 1500, 3000, 1];
/** @type {!Array} */
let ttattack_ = [10, 50, 30, 10, 25, 50, 70, 10, 40, 60, 90, 120, 50, 150, 3000, 1200, 12000, 1];
/** @type {!Array} */
let Res_ = [0, 1, 3, 6, 10, 15, 20, 25, 30, 35, 40, 45, 50];
/** @type {!Array} */
let ttname_ = ["guard", "ballista", "ranger", "triari", "priestess", "vanquisher", "sorcerers", "scout", "arbalist", "praetor", "horseman", "druid", "ram", "scorpion", "galley", "stinger", "warship", "senator"];
/** @type {!Array} */
let layoutsl_ = [""];
/** @type {!Array} */
let layoutsw_ = [""];
/** @type {!Array} */
let layoutdf_ = [""];
let cdata_: jsonT.City = null;
let resl_ = [[]];
let OGA: jsonT.Command[] = [];
let wdata_;
/** @type {!Array} */
let remarksl_ = [""];
/** @type {!Array} */
let remarksw_ = [""];
/** @type {!Array} */
let remarkdf_ = [""];
/** @type {!Array} */
let troopcounw_ = [[]];
/** @type {!Array} */
let troopcound_ = [[]];
/** @type {!Array} */
let troopcounl_ = [[]];
/** @type {!Array} */
let resw_ = [[]];
/** @type {!Array} */
let resd_ = [[]];
/** @type {!Array} */
let notesl_ = [""];
/** @type {!Array} */
let notesw_ = [""];
/** @type {!Array} */
let notedf_ = [""];
/** @type {string} */
let emptyspots_ = ",.;:#-T";
/** @type {boolean} */
let beentoworld_ = false;
let splayers_ = {
	name: [],
	ally: [],
	cities: [],
};
function InitCheckbox(v: string) {
	const i = $("#" + v);
	i.prop("checked", LocalStoreGetBool(v));
	i.change((newVal) => {
		LocalStoreSetBool(v, $(i).prop("checked"));
	});
}
function Checked(id: string): boolean {
	return $(id).prop("checked");
}
function IsChecked(a: JQuery<HTMLElement>): boolean {
	return a.find(":input").prop("checked");
}
function RoundTo2Digits(num_5: number): number {
	return (Math.round(num_5 * 100) / 100.0);
}
