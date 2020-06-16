function AsNumber(a) { return a == undefined ? 0 : parseFloat(a.toString()); }
function LocalStoreAsInt(__s, __def = 0) {
    const rv = localStorage.getItem(__s);
    if (rv == null) {
        return __def;
    }
    return parseInt(rv);
}
function LocalStoreAsFloat(__s, __def = 0) {
    const rv = localStorage.getItem(__s);
    if (rv == null) {
        return __def;
    }
    return parseFloat(rv);
}
function GetContinent(x, y) {
    return Math.floor(x / 100) + Math.floor(y / 100) * 10;
}
class Coord {
    constructor(a) {
        this.a = a;
    }
    static xy(x, y) {
        return new Coord(x + y * 65536);
    }
    get cid() { return this.a; }
    get x() {
        return this.a & 65535;
    }
    get y() {
        return this.a >> 16;
    }
    get text() {
        return `${this.x}:${this.y}`;
    }
    get cont() {
        return GetContinent(this.x, this.y);
    }
}
function GetCityContinent(_city) {
    return GetContinent(_city.x, _city.y);
}
let host = `https://w${window.worldidnumid}.crownofthegods.com`;
let hostOverview = `https://w${window.worldidnumid}.crownofthegods.com/overview.php?s=0`;
function OverviewPost(url, post, onSuccess) {
    fetch(new Request(url, {
        method: "POST",
        headers: new Headers({
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
            "pp-ss": ppss,
            referer: hostOverview,
        }),
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
function OverviewFetch(url, post) {
    return fetch(new Request(url, {
        method: "POST",
        headers: new Headers({
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
            "pp-ss": ppss,
            referer: hostOverview,
        }),
        //  redirect: 'follow',
        referrer: hostOverview,
        mode: "cors",
        cache: "no-cache",
        body: (post ? $.param(post) : null),
    })).then((response) => response.json());
}
function ToFloat(__s) {
    if (!__s) {
        return 0;
    }
    return parseFloat(__s.toString());
}
function ToInt(__s) {
    if (!__s) {
        return 0;
    }
    return parseInt(__s.toString());
}
function LocalStoreSet(__s, a) {
    localStorage.setItem(__s, a.toString());
}
function LocalStoreSetBool(__s, a) {
    localStorage.setItem(__s, a ? "1" : "0");
}
function LocalStoreGetBool(__s, __def = false) {
    const rv = localStorage.getItem(__s);
    if (rv == null) {
        return __def;
    }
    return rv == "1";
}
// import from jQuery;
function TroopNameToId(__name) { return ttname_.indexOf(__name); }
class Command {
}
class TroopTypeInfo {
}
class CommandInfo {
    constructor() {
        this.perc = 100;
        // dep: 0,
        this.ret = 0;
        this.scouts = 0;
        this.date = new Date();
    }
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
    }
    else {
        strs[1] = strs[1] + "/" + ServerDate.now().getFullYear();
    }
    return new Date(strs[0] + " " + strs[1]);
}
/**
 * @return {void}
 */
function IssueCommandsAndReturnTroops() { }
// function IssueCommandsAndReturnTroops() {
// 	$("#commandsPopUpBox").hide();
// 	if(commandInfo.ret==1) {
// 		jQuery(".toptdinncommtbl1:first")[0].click();
// 		setTimeout(() => {
// 			$("#outgoingPopUpBox").hide();
// 		},500);
// 		/** @type {!Date} */
// 		var minddate_=new Date(2050,0,0);
// 		var i_10;
// 		for(i_10 in OGA) {
// 			if(commandInfo.cstr.indexOf(OGA[i_10][5])>-1) {
// 				let d_1=GetAttackTime(OGA[i_10][6]);
// 				if(d_1<minddate_) {
// 					/** @type {!Date} */
// 					minddate_=d_1;
// 				}
// 			}
// 		}
// 		minddate_=new Date(minddate_.getTime()-AsNumber(localStorage.getItem("retHr"))*60*60*10000);
// 		/** @type {string} */
// 		var retdate_=getFormattedTime(minddate_);
// 		$("#raidrettimesela").val(3).change();
// 		$("#raidrettimeselinp").val(retdate_);
// 		jQuery("#doneOGAll")[0].click();
// 		alert("Commands set and troops returned");
// 	} else {
// 		alert("Commands set");
// 	}
// }
/// @todo
/**
 * @param {!Object} defobj_
 * @return {void}
 */
// function SendDef_() {
// 	/**
// 	 * @return {void}
// 	 */
// 	function dloop_() {
// 		var i_9;
// 		for(i_9 in commandInfo.home) {
// 			if(commandInfo.use[i_9]==1) {
// 				$(`#reiIP${commandInfo.type[i_9]}`).val(commandInfo.amount[i_9]);
// 			}
// 		}
// 		$("#reinxcoord").val(commandInfo.x[l_]);
// 		$("#reinycoord").val(commandInfo.y[l_]);
// 		setTimeout(() => {
// 			$("#reinfcoordgo").trigger({
// 				type: "click",
// 				originalEvent: "1"
// 			});
// 		},100);
// 		$("#reinforcetimingselect").val(AsNumber(commandInfo.dep)+1).change();
// 		if($("#defdeparture").val()>0) {
// 			/** @type {string} */
// 			var date_3=`${commandInfo.date} ${commandInfo.hr}:${commandInfo.min}:${commandInfo.sec}`;
// 			$("#reinfotimeinp").val(date_3);
// 		}
// 		var event_1=jQuery.Event("logged");
// 		/** @type {string} */
// 		event_1.user="foo";
// 		$("#reinforceGo").trigger({
// 			type: "click",
// 			originalEvent: "1"
// 		});
// 		l_++;
// 		if(l_<end_4) {
// 			setTimeout(dloop_,1500);
// 		} else {
// 			$("#commandsPopUpBox").hide();
// 			setTimeout(IssueCommandsAndReturnTroops,4000);
// 		}
// 	}
// 	$("#commandsPopUpBox").show();
// 	var commandtabs_=$("#commandsdiv").tabs();
// 	commandtabs_.tabs("option","active",2);
// 	$("#reintabb").trigger({
// 		type: "click",
// 		originalEvent: "1"
// 	});
// 	/** @type {number} */
// 	var maxdist_=Math.max(...commandInfo.dist);
// 	var time_;
// 	if(commandInfo.type.indexOf(14)>-1) {
// 		if(commandInfo.use[commandInfo.type.indexOf(14)]==1) {
// 			/** @type {number} */
// 			time_=ttspeed_[14]/ttspeedres_[14]*maxdist_;
// 			var gali_=commandInfo.type.indexOf(14);
// 			if(commandInfo.dep==0) {
// 				/** @type {number} */
// 				var galnumb_=Math.floor(commandInfo.home[gali_]/commandInfo.numb);
// 			} else {
// 				/** @type {number} */
// 				galnumb_=Math.floor(commandInfo.tot[gali_]/commandInfo.numb);
// 			}
// 			/** @type {number} */
// 			var maxts_=0;
// 			var i_8;
// 			for(i_8 in commandInfo.home) {
// 				if(i_8!=gali_) {
// 					if(commandInfo.use[i_8]==1) {
// 						if(commandInfo.type[i_8]!=15) {
// 							if(commandInfo.dep==0) {
// 								/** @type {number} */
// 								maxts_=maxts_+Math.floor(commandInfo.home[i_8]*ttts_[commandInfo.type[i_8]]/commandInfo.numb);
// 							} else {
// 								/** @type {number} */
// 								maxts_=maxts_+Math.floor(commandInfo.tot[i_8]*ttts_[commandInfo.type[i_8]]/commandInfo.numb);
// 							}
// 						}
// 					}
// 				}
// 			}
// 			if(maxts_<=galnumb_*500) {
// 				/** @type {number} */
// 				commandInfo.amount[gali_]=Math.ceil(maxts_/500);
// 				for(i_8 in commandInfo.home) {
// 					if(i_8!=gali_) {
// 						if(commandInfo.use[i_8]==1) {
// 							if(commandInfo.dep==0) {
// 								/** @type {number} */
// 								commandInfo.amount[i_8]=Math.floor(commandInfo.home[i_8]/commandInfo.numb);
// 							} else {
// 								/** @type {number} */
// 								commandInfo.amount[i_8]=Math.floor(commandInfo.tot[i_8]/commandInfo.numb);
// 							}
// 						}
// 					}
// 				}
// 			} else {
// 				/** @type {number} */
// 				var rat_=galnumb_*500/maxts_;
// 				/** @type {number} */
// 				commandInfo.amount[gali_]=galnumb_;
// 				for(i_8 in commandInfo.home) {
// 					if(i_8!=gali_) {
// 						if(commandInfo.use[i_8]==1) {
// 							if(commandInfo.type[i_8]!=15) {
// 								if(commandInfo.dep==0) {
// 									/** @type {number} */
// 									commandInfo.amount[i_8]=Math.floor(rat_*commandInfo.home[i_8]/commandInfo.numb);
// 								} else {
// 									/** @type {number} */
// 									commandInfo.amount[i_8]=Math.floor(rat_*commandInfo.tot[i_8]/commandInfo.numb);
// 								}
// 							} else {
// 								if(commandInfo.dep==0) {
// 									/** @type {number} */
// 									commandInfo.amount[i_8]=Math.floor(commandInfo.home[i_8]/commandInfo.numb);
// 								} else {
// 									/** @type {number} */
// 									commandInfo.amount[i_8]=Math.floor(commandInfo.tot[i_8]/commandInfo.numb);
// 								}
// 							}
// 						}
// 					}
// 				}
// 			}
// 		}
// 	} else {
// 		/** @type {number} */
// 		time_=Math.max(...commandInfo.speed)*maxdist_;
// 		for(i_8 in commandInfo.home) {
// 			if(commandInfo.use[i_8]==1) {
// 				if(commandInfo.dep==0) {
// 					/** @type {number} */
// 					commandInfo.amount[i_8]=Math.floor(commandInfo.home[i_8]/commandInfo.numb);
// 				} else {
// 					/** @type {number} */
// 					commandInfo.amount[i_8]=Math.floor(commandInfo.tot[i_8]/commandInfo.numb);
// 				}
// 			}
// 		}
// 	}
// 	/** @type {number} */
// 	var l_=0;
// 	var end_4=commandInfo.x.length;
// 	dloop_();
// }
// function EnumerateTroops(predicate) {
// 	var troops=cotg.city.troops();
// 	commandInfo.scouts=0;
// 	for(let i in troops) {
// 		let total=troops[i].total;
// 		if(total>0) {
// 			let id=TroopNameToId(i);
// 			if(id==7)
// 				commandInfo.scouts=total;
// 			commandInfo.total.push(Math.ceil(total*AsNumber($("#perc").val())/100));
// 			commandInfo.home.push(Math.ceil(troops[i].home*AsNumber($("#perc").val())/100));
// 			commandInfo.type.push(id);
// 			if(predicate(id)) {
// 				commandInfo.speed.push(ttspeed_[id]/ttspeedres_[id]);
// 				commandInfo.use.push(true);
// 			} else {
// 				commandInfo.speed.push(0);
// 				commandInfo.use.push(false);
// 			}
// 		}
// 	}
// 	return commandInfo;
// }
/**
 * @return {void}
 */
function updateattack_() {
    /** @type {string} */
    let ttseltab_ = "<table><thead><th>Troop Type</th><th>Use for real</th><th>Use for fake</th></thead><tbody>";
    const troops = cotg.city.troops();
    for (const id in cotg.city.troops()) {
        const t = troops[id];
        if (t.total == 0) {
            continue;
        }
        const i = TroopNameToId(id);
        ttseltab_ = `${ttseltab_}<tr><td style='height:40px;'><div class='${tpicdiv_[i]}'></div></td><td style='text-align: center;'><input id='usereal${i}' class='clsubopti' type='checkbox' checked></td>`;
        /** @type {string} */
        ttseltab_ = `${ttseltab_}<td style='text-align: center;'><input id='usefake${1}' class='clsubopti' type='checkbox' checked></td></tr>`;
    }
    /** @type {string} */
    ttseltab_ = `${ttseltab_}</tbody></table>`;
    $("#picktype").html(ttseltab_);
}
/**
 * @return {void}
 */
function updatedef_() {
    const t_4 = {
        home: [],
        type: [],
    };
    /// @todo
    // var i_12;
    // for(i_12 in cdata_.tc) {
    // 	if(cdata_.tc[i_12]) {
    // 		t_4.home.push(cdata_.tc[i_12]);
    // 		t_4.type.push(i_12);
    // 	}
    // }
    /// ** @type {string} */
    // var ttseltab_1="<table><thead><th>Troop Type</th><th>Use</th></thead><tbody>";
    // for(i_12 in t_4.home) {
    // 	/** @type {string} */
    // 	ttseltab_1=`${ttseltab_1}<tr><td style='height:40px;'><div class='${tpicdiv_[t_4.type[i_12]]}'></div></td><td style='text-align: center;'><input id='usedef${t_4.type[i_12]}' class='clsubopti' type='checkbox' checked></td></tr>`;
    // }
    /// ** @type {string} */
    // ttseltab_1=`${ttseltab_1}</tbody></table>`;
    // $("#dpicktype").html(ttseltab_1);
}
/**
 * @return {void}
 */
function SendAttack_() {
    // 	var l_1=0;
    // 	var end_5=0;
    // 	/**
    // 	 * @return {void}
    // 	 */
    // 	function GetAttackTime() {
    // 		var date=new Date($("#attackDat").val());
    // 		return getFormattedTime(date);
    // 	}
    // 	function loop_() {
    // 		if(commandInfo.real[l_1]==1) {
    // 			if($("#realtype").val()==0) {
    // 				pvptabs_.tabs("option","active",0);
    // 				var i_14;
    // 				for(i_14 in t_5.real) {
    // 					$(`#assIP${t_5.type[i_14]}`).val(t_5.real[i_14]);
    // 				}
    // 				$("#assaultxcoord").val(commandInfo.x[l_1]);
    // 				$("#assaultycoord").val(commandInfo.y[l_1]);
    // 				setTimeout(() => {
    // 					jQuery("#assaultcoordgo")[0].click();
    // 				},100);
    // 				$("#assaulttimingselect").val(3).change();
    // 				var date_4=GetAttackTime();
    // 				$("#assaulttimeinp").val(date_4);
    // 				alltimes_.a.push($("#assaulttraveltime").text());
    // 				jQuery("#assaultGo")[0].click();
    // 			}
    // 			if($("#realtype").val()==1) {
    // 				pvptabs_.tabs("option","active",1);
    // 				for(i_14 in t_5.real) {
    // 					$(`#sieIP${t_5.type[i_14]}`).val(t_5.real[i_14]);
    // 				}
    // 				$("#siexcoord").val(commandInfo.x[l_1]);
    // 				$("#sieycoord").val(commandInfo.y[l_1]);
    // 				setTimeout(() => {
    // 					jQuery("#siegecoordgo")[0].click();
    // 				},100);
    // 				$("#siegetimingselect").val(3).change();
    // 				var date_4=GetAttackTime();
    // 				$("#siegetimeinp").val(date_4);
    // 				alltimes_.a.push($("#siegetraveltime").text());
    // 				jQuery("#siegeGo")[0].click();
    // 			}
    // 			if($("#realtype").val()==2) {
    // 				pvptabs_.tabs("option","active",2);
    // 				for(i_14 in t_5.real) {
    // 					$(`#pluIP${t_5.type[i_14]}`).val(t_5.real[i_14]);
    // 				}
    // 				$("#pluxcoord").val(commandInfo.x[l_1]);
    // 				$("#pluycoord").val(commandInfo.y[l_1]);
    // 				setTimeout(() => {
    // 					jQuery("#plundercoordgo")[0].click();
    // 				},100);
    // 				$("#plundertimingselect").val(3).change();
    // 				var date_4=GetAttackTime();
    // 				$("#plundtimeinp").val(date_4);
    // 				alltimes_.a.push($("#plundtraveltime").text());
    // 				$("#plunderGo").prop("disabled",false);
    // 				jQuery("#plunderGo")[0].click();
    // 			}
    // 			if($("#realtype").val()==3) {
    // 				pvptabs_.tabs("option","active",3);
    // 				for(i_14 in t_5.real) {
    // 					$(`#scoIP${t_5.type[i_14]}`).val(t_5.real[i_14]);
    // 				}
    // 				$("#scoIP7").val(t_5.scoutreal[0]);
    // 				$("#scoxcoord").val(commandInfo.x[l_1]);
    // 				$("#scoycoord").val(commandInfo.y[l_1]);
    // 				setTimeout(() => {
    // 					$("#scoutcoordgo")[0].click();
    // 				},100);
    // 				$("#scouttimingselect").val(3).change();
    // 				var date_4=GetAttackTime();
    // 				$("#scouttimeinp").val(date_4);
    // 				$("#scoutGo")[0].click();
    // 			}
    // 		}
    // 		if(commandInfo.real[l_1]==0) {
    // 			if($("#faketype").val()==0) {
    // 				pvptabs_.tabs("option","active",0);
    // 				for(i_14 in t_5.real) {
    // 					$(`#assIP${t_5.type[i_14]}`).val(t_5.fake[i_14]);
    // 				}
    // 				$("#assaultxcoord").val(commandInfo.x[l_1]);
    // 				$("#assaultycoord").val(commandInfo.y[l_1]);
    // 				setTimeout(() => {
    // 					jQuery("#assaultcoordgo")[0].click();
    // 				},100);
    // 				$("#assaulttimingselect").val(3).change();
    // 				var date_4=GetAttackTime();
    // 				$("#assaulttimeinp").val(date_4);
    // 				alltimes_.a.push($("#assaulttraveltime").text());
    // 				jQuery("#assaultGo")[0].click();
    // 			}
    // 			if($("#faketype").val()==1) {
    // 				pvptabs_.tabs("option","active",1);
    // 				for(i_14 in t_5.real) {
    // 					$(`#sieIP${t_5.type[i_14]}`).val(t_5.fake[i_14]);
    // 				}
    // 				$("#siexcoord").val(commandInfo.x[l_1]);
    // 				$("#sieycoord").val(commandInfo.y[l_1]);
    // 				setTimeout(() => {
    // 					jQuery("#siegecoordgo")[0].click();
    // 				},100);
    // 				$("#siegetimingselect").val(3).change();
    // 				var date_4=GetAttackTime();
    // 				$("#siegetimeinp").val(date_4);
    // 				alltimes_.a.push($("#siegetraveltime").text());
    // 				jQuery("#siegeGo")[0].click();
    // 			}
    // 			if($("#faketype").val()==2) {
    // 				pvptabs_.tabs("option","active",2);
    // 				for(i_14 in t_5.real) {
    // 					$(`#pluIP${t_5.type[i_14]}`).val(t_5.fake[i_14]);
    // 				}
    // 				$("#pluxcoord").val(commandInfo.x[l_1]);
    // 				$("#pluycoord").val(commandInfo.y[l_1]);
    // 				setTimeout(() => {
    // 					jQuery("#plundercoordgo")[0].click();
    // 				},100);
    // 				$("#plundertimingselect").val(3).change();
    // 				var date_4=GetAttackTime();
    // 				$("#plundtimeinp").val(date_4);
    // 				alltimes_.a.push($("#plundtraveltime").text());
    // 				$("#plunderGo").prop("disabled",false);
    // 				jQuery("#plunderGo")[0].click();
    // 			}
    // 			if($("#faketype").val()==3) {
    // 				if($("#scoutick").prop("checked")===true) {
    // 					pvptabs_.tabs("option","active",3);
    // 					$("#scoIP7").val(1);
    // 					$("#scoIP14").val(faketss_/100);
    // 					$("#scoxcoord").val(commandInfo.x[l_1]);
    // 					$("#scoycoord").val(commandInfo.y[l_1]);
    // 					setTimeout(() => {
    // 						$("#scoutcoordgo")[0].click();
    // 					},100);
    // 					$("#scouttimingselect").val(3).change();
    // 					var date_4=GetAttackTime();
    // 					$("#scouttimeinp").val(date_4);
    // 					$("#scoutGo")[0].click();
    // 				} else {
    // 					pvptabs_.tabs("option","active",3);
    // 					for(i_14 in t_5.real) {
    // 						$(`#scoIP${t_5.type[i_14]}`).val(t_5.fake[i_14]);
    // 					}
    // 					$("#scoIP7").val(t_5.scoutfake[0]);
    // 					$("#scoxcoord").val(commandInfo.x[l_1]);
    // 					$("#scoycoord").val(commandInfo.y[l_1]);
    // 					setTimeout(() => {
    // 						$("#scoutcoordgo")[0].click();
    // 					},100);
    // 					$("#scouttimingselect").val(3).change();
    // 					var date_4=GetAttackTime();
    // 					$("#scouttimeinp").val(date_4);
    // 					$("#scoutGo")[0].click();
    // 				}
    // 			}
    // 		}
    // 		l_1++;
    // 		if(l_1<end_5) {
    // 			setTimeout(loop_,1000);
    // 		} else {
    // 			setTimeout(IssueCommandsAndReturnTroops,4000);
    // 		}
    // 	}
    // 	$("#commandsPopUpBox").show();
    // 	var commandtabs_1=$("#commandsdiv").tabs();
    // 	var pvptabs_=$("#pvpTab").tabs();
    // 	jQuery("#pvptabb")[0].click();
    // 	commandtabs_1.tabs("option","active",1);
    // 	/** @type {number} */
    // 	let fakenumb_=0;
    // 	/** @type {number} */
    // 	let realnumb_=0;
    // 	let tempx_2;
    // 	let tempy_2;
    // 	/** @type {number} */
    // 	ResetTargets();
    // 	var i_13=1;
    // 	for(; i_13<16; i_13++) {
    // 		if($(`#t${i_13}x`).val()) {
    // 			tempx_2=$(`#t${i_13}x`).val();
    // 			tempy_2=$(`#t${i_13}y`).val();
    // 			commandInfo.x.push(tempx_2);
    // 			commandInfo.y.push(tempy_2);
    // 			commandInfo.cstr.push(`${tempx_2}:${tempy_2}`);
    // 			commandInfo.real.push($(`#type${i_13}`).val());
    // 			if($(`#type${i_13}`).val()==1) {
    // 				/** @type {number} */
    // 				realnumb_=realnumb_+1;
    // 			} else {
    // 				/** @type {number} */
    // 				fakenumb_=fakenumb_+1;
    // 			}
    // 			commandInfo.dist.push(Math.sqrt((tempx_2-cdata_.x)*(tempx_2-cdata_.x)+(tempy_2-cdata_.y)*(tempy_2-cdata_.y)));
    // 		}
    // 	}
    // 	EnumerateTroops((id) => ($(`#usereal${id}`).prop("checked")));
    // 	/** @type {number} */
    // 	var maxdist_1=Math.max(...commandInfo.dist);
    // 	var time_2;
    // 	var faketss_;
    // 	var fakeg_;
    // 	var tscbr_=cdata_.tt;
    // 	if(tscbr_<20000) {
    // 		/** @type {number} */
    // 		faketss_=1;
    // 		/** @type {number} */
    // 		fakeg_=1;
    // 	} else {
    // 		if(tscbr_<40000) {
    // 			/** @type {number} */
    // 			faketss_=200;
    // 			/** @type {number} */
    // 			fakeg_=1;
    // 		} else {
    // 			if(tscbr_<60000) {
    // 				/** @type {number} */
    // 				faketss_=500;
    // 				/** @type {number} */
    // 				fakeg_=1;
    // 			} else {
    // 				if(tscbr_<80000) {
    // 					/** @type {number} */
    // 					faketss_=800;
    // 					/** @type {number} */
    // 					fakeg_=2;
    // 				} else {
    // 					if(tscbr_<100000) {
    // 						/** @type {number} */
    // 						faketss_=1000;
    // 						/** @type {number} */
    // 						fakeg_=2;
    // 					} else {
    // 						if(tscbr_<120000) {
    // 							/** @type {number} */
    // 							faketss_=1200;
    // 							/** @type {number} */
    // 							fakeg_=2;
    // 						} else {
    // 							if(tscbr_<160000) {
    // 								/** @type {number} */
    // 								faketss_=1600;
    // 								/** @type {number} */
    // 								fakeg_=3;
    // 							} else {
    // 								if(tscbr_<200000) {
    // 									/** @type {number} */
    // 									faketss_=2000;
    // 									/** @type {number} */
    // 									fakeg_=4;
    // 								} else {
    // 									if(tscbr_<240000) {
    // 										/** @type {number} */
    // 										faketss_=2500;
    // 										/** @type {number} */
    // 										fakeg_=5;
    // 									} else {
    // 										/** @type {number} */
    // 										faketss_=3000;
    // 										/** @type {number} */
    // 										fakeg_=5;
    // 									}
    // 								}
    // 							}
    // 						}
    // 					}
    // 				}
    // 			}
    // 		}
    // 	}
    // 	if(scouts>0) {
    // 		if($("#realtype").val()==3&&$("#faketype").val()==3) {
    // 			if($("#usereal14").prop("checked")===true) {
    // 				if($("#usefake14").prop("checked")===true) {
    // 					/** @type {number} */
    // 					t_5.scoutfake[0]=fakeg_*250;
    // 					/** @type {number} */
    // 					t_5.scoutreal[0]=Math.floor((scouts-fakeg_*250*fakenumb_)/realnumb_);
    // 				} else {
    // 					/** @type {number} */
    // 					t_5.scoutfake[0]=faketss_/2;
    // 					/** @type {number} */
    // 					t_5.scoutreal[0]=Math.floor((scouts-faketss_/2*fakenumb_)/realnumb_);
    // 				}
    // 			} else {
    // 				if($("#usereal14").prop("checked")!==true) {
    // 					if($("#usefake14").prop("checked")===true) {
    // 						/** @type {number} */
    // 						t_5.scoutfake[0]=fakeg_*250;
    // 						/** @type {number} */
    // 						t_5.scoutreal[0]=Math.floor((scouts-fakeg_*250*fakenumb_)/realnumb_);
    // 					} else {
    // 						/** @type {number} */
    // 						t_5.scoutfake[0]=faketss_/2;
    // 						/** @type {number} */
    // 						t_5.scoutreal[0]=Math.floor((scouts-faketss_/2*fakenumb_)/realnumb_);
    // 					}
    // 				}
    // 			}
    // 		}
    // 		if($("#realtype").val()==3&&$("#faketype").val()!=3) {
    // 			if($("#usereal14").prop("checked")===true) {
    // 				if($("#usefake14").prop("checked")===true) {
    // 					/** @type {number} */
    // 					t_5.scoutreal[0]=Math.floor(scouts/realnumb_);
    // 				} else {
    // 					/** @type {number} */
    // 					t_5.scoutreal[0]=Math.floor(scouts/realnumb_);
    // 				}
    // 			} else {
    // 				/** @type {number} */
    // 				t_5.scoutreal[0]=Math.floor(scouts/realnumb_);
    // 			}
    // 		}
    // 		if($("#realtype").val()!=3&&$("#faketype").val()==3) {
    // 			if($("#usereal14").prop("checked")===true) {
    // 				if($("#usefake14").prop("checked")===true) {
    // 					/** @type {number} */
    // 					t_5.scoutfake[0]=fakeg_*250;
    // 				} else {
    // 					/** @type {number} */
    // 					t_5.scoutfake[0]=faketss_/2;
    // 				}
    // 			} else {
    // 				if($("#usereal14").prop("checked")!==true) {
    // 					if($("#usefake14").prop("checked")===true) {
    // 						/** @type {number} */
    // 						t_5.scoutfake[0]=fakeg_*250;
    // 					} else {
    // 						/** @type {number} */
    // 						t_5.scoutfake[0]=faketss_/2;
    // 					}
    // 				}
    // 			}
    // 		}
    // 	}
    // 	if(t_5.type.indexOf(14)>-1&&$("#usereal14").prop("checked")===true) {
    // 		/** @type {number} */
    // 		time_2=ttspeed_[14]/ttspeedres_[14]*maxdist_1;
    // 		/** @type {number} */
    // 		var gali_1=t_5.type.indexOf(14);
    // 		/** @type {number} */
    // 		var galnumb_1=Math.floor((t_5.home[gali_1]-fakeg_*fakenumb_)/realnumb_);
    // 		/** @type {number} */
    // 		t_5.real[gali_1]=galnumb_1;
    // 		/** @type {number} */
    // 		t_5.fake[gali_1]=fakeg_;
    // 		/** @type {number} */
    // 		var galcap_=500*galnumb_1;
    // 		/** @type {number} */
    // 		var nongalts_=0;
    // 		for(i_13 in t_5.home) {
    // 			if(t_5.type[i_13]==14&&t_5.type[i_13]==17&&t_5.type[i_13]==16) {
    // 				if(t_5.type[i_13]==14) {
    // 					if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 						if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 							t_5.real[i_13]==1;
    // 							t_5.fake[i_13]==1;
    // 						} else {
    // 							t_5.real[i_13]==1;
    // 							t_5.fake[i_13]==0;
    // 						}
    // 					}
    // 				}
    // 				if(t_5.type[i_13]==17) {
    // 					if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 						if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 							if(t_5.home[i_13]>=fakenumb_+realnumb_) {
    // 								/** @type {number} */
    // 								t_5.fake[i_13]=1;
    // 								/** @type {number} */
    // 								t_5.real[i_13]=1;
    // 							} else {
    // 								/** @type {number} */
    // 								t_5.fake[i_13]=0;
    // 								/** @type {number} */
    // 								t_5.real[i_13]=1;
    // 							}
    // 						} else {
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=0;
    // 							/** @type {number} */
    // 							t_5.real[i_13]=1;
    // 						}
    // 					} else {
    // 						if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 							/** @type {number} */
    // 							t_5.real[i_13]=0;
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=1;
    // 						} else {
    // 							/** @type {number} */
    // 							t_5.real[i_13]=0;
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=0;
    // 						}
    // 					}
    // 				}
    // 				if(t_5.type[i_13]==16) {
    // 					if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 						if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=Math.ceil(faketss_*t_5.home[i_13]);
    // 							/** @type {number} */
    // 							t_5.real[i_13]=Math.floor((t_5.home[i_13]-t_5.fake[i_13]*fakenumb_)/realnumb_);
    // 						} else {
    // 							/** @type {number} */
    // 							t_5.real[i_13]=Math.floor(t_5.home[i_13]/realnumb_);
    // 						}
    // 					}
    // 				}
    // 			}
    // 		}
    // 		for(i_13 in t_5.home) {
    // 			if(i_13!=gali_1&&t_5.type[i_13]!=17) {
    // 				if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						/** @type {number} */
    // 						nongalts_=nongalts_+ttts_[t_5.type[i_13]]*(t_5.home[i_13]-Math.ceil(fakeg_*500/ttts_[t_5.type[i_13]])*fakenumb_)/realnumb_;
    // 					} else {
    // 						/** @type {number} */
    // 						nongalts_=nongalts_+ttts_[t_5.type[i_13]]*t_5.home[i_13]/realnumb_;
    // 					}
    // 				}
    // 			}
    // 			if(t_5.type[i_13]==17) {
    // 				if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						if(t_5.home[i_13]>=fakenumb_+realnumb_) {
    // 							/** @type {number} */
    // 							nongalts_=nongalts_+1;
    // 						} else {
    // 							/** @type {number} */
    // 							nongalts_=nongalts_+1;
    // 						}
    // 					} else {
    // 						/** @type {number} */
    // 						nongalts_=nongalts_+1;
    // 					}
    // 				}
    // 			}
    // 		}
    // 		/** @type {number} */
    // 		var fakerat_=0;
    // 		for(i_13 in t_5.home) {
    // 			if(i_13!=gali_1) {
    // 				if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 					/** @type {number} */
    // 					fakerat_=fakerat_+ttts_[t_5.type[i_13]]*t_5.home[i_13];
    // 				}
    // 			}
    // 		}
    // 		for(i_13 in t_5.home) {
    // 			if(i_13!=gali_1&&t_5.type[i_13]!=17) {
    // 				if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 					/** @type {number} */
    // 					t_5.fake[i_13]=Math.ceil(fakeg_*500*t_5.home[i_13]/fakerat_);
    // 				}
    // 			}
    // 			if(t_5.type[i_13]==17) {
    // 				if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						if(t_5.home[i_13]>=fakenumb_+realnumb_) {
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=1;
    // 							/** @type {number} */
    // 							t_5.real[i_13]=1;
    // 						} else {
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=0;
    // 							/** @type {number} */
    // 							t_5.real[i_13]=1;
    // 						}
    // 					} else {
    // 						/** @type {number} */
    // 						t_5.fake[i_13]=0;
    // 						/** @type {number} */
    // 						t_5.real[i_13]=1;
    // 					}
    // 				} else {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						/** @type {number} */
    // 						t_5.real[i_13]=0;
    // 						/** @type {number} */
    // 						t_5.fake[i_13]=1;
    // 					} else {
    // 						/** @type {number} */
    // 						t_5.real[i_13]=0;
    // 						/** @type {number} */
    // 						t_5.fake[i_13]=0;
    // 					}
    // 				}
    // 			}
    // 		}
    // 		for(i_13 in t_5.home) {
    // 			if(i_13!=gali_1&&t_5.type[i_13]!=17) {
    // 				if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						if(nongalts_>galcap_) {
    // 							/** @type {number} */
    // 							t_5.real[i_13]=Math.floor(galcap_/nongalts_*(t_5.home[i_13]-t_5.fake[i_13]*fakenumb_)/realnumb_);
    // 						} else {
    // 							/** @type {number} */
    // 							t_5.real[i_13]=Math.floor((t_5.home[i_13]-t_5.fake[i_13]*fakenumb_)/realnumb_);
    // 						}
    // 					} else {
    // 						if(nongalts_>galcap_) {
    // 							/** @type {number} */
    // 							t_5.real[i_13]=Math.floor(galcap_/nongalts_*t_5.home[i_13]/realnumb_);
    // 						} else {
    // 							/** @type {number} */
    // 							t_5.real[i_13]=Math.floor(t_5.home[i_13]/realnumb_);
    // 						}
    // 						/** @type {number} */
    // 						t_5.fake[i_13]=0;
    // 					}
    // 				}
    // 			}
    // 		}
    // 	} else {
    // 		/** @type {number} */
    // 		fakerat_=0;
    // 		/** @type {number} */
    // 		time_2=Math.max(...t_5.speed)*maxdist_1;
    // 		for(i_13 in t_5.home) {
    // 			if(t_5.type[i_13]!==17) {
    // 				if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 					/** @type {number} */
    // 					fakerat_=fakerat_+ttts_[t_5.type[i_13]]*t_5.home[i_13];
    // 				}
    // 			}
    // 		}
    // 		for(i_13 in t_5.home) {
    // 			if(t_5.type[i_13]!=17) {
    // 				if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 					/** @type {number} */
    // 					t_5.fake[i_13]=Math.ceil(faketss_*t_5.home[i_13]/fakerat_);
    // 				}
    // 			}
    // 		}
    // 		for(i_13 in t_5.home) {
    // 			if(t_5.type[i_13]!=17) {
    // 				if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						/** @type {number} */
    // 						t_5.real[i_13]=Math.floor((t_5.home[i_13]-t_5.fake[i_13]*fakenumb_)/realnumb_);
    // 					} else {
    // 						/** @type {number} */
    // 						t_5.real[i_13]=Math.floor(t_5.home[i_13]/realnumb_);
    // 					}
    // 				} else {
    // 					/** @type {number} */
    // 					t_5.real[i_13]=0;
    // 				}
    // 			}
    // 			if(t_5.type[i_13]==17) {
    // 				if($(`#usereal${t_5.type[i_13]}`).prop("checked")===true) {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						if(t_5.home[i_13]>=fakenumb_+realnumb_) {
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=1;
    // 							/** @type {number} */
    // 							t_5.real[i_13]=1;
    // 						} else {
    // 							/** @type {number} */
    // 							t_5.fake[i_13]=0;
    // 							/** @type {number} */
    // 							t_5.real[i_13]=1;
    // 						}
    // 					} else {
    // 						/** @type {number} */
    // 						t_5.fake[i_13]=0;
    // 						/** @type {number} */
    // 						t_5.real[i_13]=1;
    // 					}
    // 				} else {
    // 					if($(`#usefake${t_5.type[i_13]}`).prop("checked")===true) {
    // 						/** @type {number} */
    // 						t_5.real[i_13]=0;
    // 						/** @type {number} */
    // 						t_5.fake[i_13]=1;
    // 					} else {
    // 						/** @type {number} */
    // 						t_5.real[i_13]=0;
    // 						/** @type {number} */
    // 						t_5.fake[i_13]=0;
    // 					}
    // 				}
    // 			}
    // 		}
    // 	}
    // 	var alltimes_={
    // 		a: [],
    // 		b: [],
    // 		c: [],
    // 		d: []
    // 	};
    // 	/** @type {number} */
    // 	l_1=0;
    // 	/** @type {number} */
    // 	end_5=commandInfo.x.length;
    // 	loop_();
}
var lastUpdate = 0;
function makebuildcount_() {
    lastUpdate = Date.now();
    $("#bdtable").remove();
    const currentbd_ = {
        name: [],
        bid: [],
        count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    };
    let j_4;
    /** @type {number} */
    let bdtypecount_ = -1;
    /** @type {number} */
    let bdNumber_ = -1;
    let i_16;
    for (i_16 in cdata_.bd) {
        if (buildings_.bid.indexOf(cdata_.bd[i_16].bid) > -1) {
            if (currentbd_.bid.indexOf(cdata_.bd[i_16].bid) > -1) {
                /** @type {number} */
                j_4 = currentbd_.bid.indexOf(cdata_.bd[i_16].bid);
                currentbd_.count[j_4] += 1;
                /** @type {number} */
                bdNumber_ = bdNumber_ + 1;
            }
            else {
                /** @type {number} */
                bdtypecount_ = bdtypecount_ + 1;
                /** @type {number} */
                j_4 = buildings_.bid.indexOf(cdata_.bd[i_16].bid);
                currentbd_.name[bdtypecount_] = buildings_.name[j_4];
                currentbd_.bid[bdtypecount_] = buildings_.bid[j_4];
                currentbd_.count[bdtypecount_] += 1;
                /** @type {number} */
                bdNumber_ = bdNumber_ + 1;
            }
        }
    }
    /** @type {string} */
    let bdtable_ = "<table id='bdtable'><tbody><tr>";
    for (i_16 in currentbd_.bid) {
        if (i_16 < 9 || i_16 > 9 && i_16 < 19 || i_16 > 19 && i_16 < 29) {
            /** @type {string} */
            bdtable_ = `${bdtable_}<td style='text-align:center; width:30px; height:30px;'><div style='background-image: url(/images/city/buildings/icons/${currentbd_.name[i_16]}.png); background-size:contain;background-repeat:no-repeat;width:30px; height:30px;'></div>${AsNumber(currentbd_.count[i_16])}</td>`;
        }
        if (i_16 == 9 || i_16 == 19) {
            /** @type {string} */
            bdtable_ = `${bdtable_}</tr><tr>`;
            /** @type {string} */
            bdtable_ = `${bdtable_}<td style='text-align:center; width:30px; height:30px;'><div style='background-image: url(/images/city/buildings/icons/${currentbd_.name[i_16]}.png); background-size:contain;background-repeat:no-repeat;width:30px; height:30px;'></div>${AsNumber(currentbd_.count[i_16])}</td>`;
        }
    }
    /** @type {string} */
    bdtable_ = `${bdtable_}</tr></tbody></table>`;
    $("#bdcountwin").html(bdtable_);
    $("#numbdleft").html(bdNumber_.toLocaleString());
}
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
    $("#iaBody tr").each(function () {
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
        }
        else {
            /** @type {number} */
            arrivaltimemonth_ = d_3.getMonth() + 1;
            /** @type {number} */
            arrivaltimedate_ = d_3.getDate();
        }
        let time_4;
        if (hdiff_ >= 0) {
            /** @type {number} */
            time_4 = 60 * hdiff_;
        }
        else {
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
        }
        else {
            /** @type {number} */
            locks_ = 60 + sdiff_;
            /** @type {number} */
            mdiff_ = mdiff_ - 1;
        }
        if (mdiff_ >= 0) {
            /** @type {number} */
            lockm_ = mdiff_;
        }
        else {
            /** @type {number} */
            lockm_ = 60 + mdiff_;
            /** @type {number} */
            hdiff_ = hdiff_ - 1;
        }
        if (hdiff_ >= 0) {
            /** @type {number} */
            lockh_ = hdiff_;
        }
        else {
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
                    }
                    else {
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
                    }
                    else {
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
                    }
                    else {
                        $(":nth-child(2)", this).text(`Navy ${speeeed_[zns_]}%`);
                    }
                }
            }
            else {
                if ($(":nth-child(1)", this).html()) {
                    $(":nth-child(2)", this).text("Portal");
                }
                else {
                    if (zns_ != -1) {
                        $(":nth-child(2)", this).text(`Navy ${speeeed_[zns_]}%`);
                    }
                    else {
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
    $("#shrineTab tr").each(function () {
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
    last: new Date(),
};
let mru = [];
/**
 * @return {void}
 */
function showcities_() {
    $("#shrineTab tr").each(function () {
        if ($(this).attr("data") == "city") {
            $(this).show();
        }
    });
}
/**
 * @return {void}
 */
/// @todo
function updateshrine_() { }
// function updateshrine_() {
// 	/** @type {string} */
// 	var shrinetab_="<table id='shrineTab'><thead><th style='width:115px'>Change</th><th style='width:50px'>Chances</th><th>Distance</th><th>Player</th><th>City</th><th>Coords</th><th style='width:100px'>Alliance</th><th>score</th><th>Type</th></thead><tbody>";
// 	/** @type {number} */
// 	var ccounter_=0;
// 	/** @type {!Array} */
// 	var w_7=[];
// 	/** @type {number} */
// 	var wtot_=0;
// 	for(var i_30 in shrinec_) {
// 		if(i_30>0) {
// 			var k_2=splayers_.name.indexOf(shrinec_[i_30][1]);
// 			var j_8;
// 			for(j_8 in splayers_.cities[k_2]) {
// 				if(shrinec_[i_30][3]==splayers_.cities[k_2][j_8].b&&shrinec_[i_30][4]==splayers_.cities[k_2][j_8].c) {
// 					shrinec_[i_30][2]=splayers_.cities[k_2][j_8].h;
// 					if(shrinec_[i_30][9]==0) {
// 						shrinec_[i_30][7]=splayers_.cities[k_2][j_8].a;
// 					}
// 					shrinec_[i_30][8]=splayers_.ally[k_2];
// 				}
// 			}
// 			if(shrinec_[i_30][0]=="castle") {
// 				ccounter_++;
// 				if(ccounter_<17) {
// 					/** @type {number} */
// 					w_7[ccounter_]=shrinec_[i_30][7]/shrinec_[i_30][5];
// 					/** @type {number} */
// 					wtot_=wtot_+shrinec_[i_30][7]/shrinec_[i_30][5];
// 				}
// 			}
// 		}
// 	}
// 	for(var i_30 in w_7) {
// 		/** @type {number} */
// 		w_7[i_30]=Math.round(w_7[i_30]/wtot_*100);
// 	}
// 	/** @type {number} */
// 	ccounter_=0;
// 	for(var i_30 in shrinec_) {
// 		if(i_30>0) {
// 			/** @type {number} */
// 			var cid_5=shrinec_[i_30][4]*65536+AsNumber(shrinec_[i_30][3]);
// 			if(shrinec_[i_30][0]=="castle") {
// 				ccounter_++;
// 				if(ccounter_<17) {
// 					if(shrinec_[i_30][6]=="0") {
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<tr style='color:purple;'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button id='${i_30}' data='castle' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding-top: 3px;border-radius: 4px;'>City</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td>${ccounter_} - ${w_7[ccounter_]}% </td>`;
// 					} else {
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<tr style='color:green;'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button id='${i_30}' data='castle' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding-top: 3px;border-radius: 4px;'>City</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td>${ccounter_} - ${w_7[ccounter_]}% </td>`;
// 					}
// 				} else {
// 					if(ccounter_>=17&&ccounter_<21) {
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<tr><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button id='${i_30}' data='castle' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding-top: 3px;border-radius: 4px;'>City</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
// 						/** @type {string} */
// 						shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td>${ccounter_}</td>`;
// 					}
// 				}
// 			} else {
// 				if(shrinec_[i_30][6]=="0") {
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<tr style='color:grey;' data='city'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<button id='${i_30}' data='city' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding: 3px;border-radius: 4px;width:37px;'>Castle</button>`;
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td></td>`;
// 				} else {
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<tr style='color:#74A274;'><td><button data='${i_30}' class='greenb shrineremove' style='font-size: 10px;height: 20px;padding: 3px;width: 15px;border-radius: 4px;'>x</button>`;
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<button id='${i_30}' data='city' class='greenb shrinechange' style='font-size: 10px;height: 20px;padding: 3px;border-radius: 4px;width:37px;'>Castle</button>`;
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine10k' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>10k</button>`;
// 					/** @type {string} */
// 					shrinetab_=`${shrinetab_}<button data='${i_30}' class='greenb shrine7pt' style='font-size: 10px;height: 20px;padding: 3px;width: 25px;border-radius: 4px;'>7pt</button></td><td></td>`;
// 				}
// 			}
// 			/** @type {string} */
// 			shrinetab_=`${shrinetab_}<td>${RoundTo2Digits(shrinec_[i_30][5])}</td><td class='playerblink'>${shrinec_[i_30][1]}</td><td>${shrinec_[i_30][2]}</td><td class='coordblink shcitt' data='${cid_5}'>${shrinec_[i_30][3]}:${shrinec_[i_30][4]}</td><td class='allyblink'>${shrinec_[i_30][8]}</td><td>${shrinec_[i_30][7]}</td><td>${shrinec_[i_30][0]}</td></tr>`;
// 			if(ccounter_==20) {
// 				break;
// 			}
// 		}
// 	}
// 	/** @type {string} */
// 	shrinetab_=`${shrinetab_}</tbody></table>`;
// 	$("#shrinediv").html(shrinetab_);
// 	$("#shrineTab td").css("text-align","center");
// 	if(localStorage.getItem("hidecities")=="1") {
// 		hidecities_();
// 	}
// 	$(".shrinechange").click(function() {
// 		if($(this).attr("data")=="castle") {
// 			/** @type {string} */
// 			shrinec_[$(this).attr("id")][0]="city";
// 		} else {
// 			/** @type {string} */
// 			shrinec_[$(this).attr("id")][0]="castle";
// 		}
// 		if(shrinec_[$(this).attr("id")][6]=="0") {
// 			/** @type {number} */
// 			shrinec_[$(this).attr("id")][6]=1;
// 		} else {
// 			/** @type {number} */
// 			shrinec_[$(this).attr("id")][6]=0;
// 		}
// 		updateshrine_();
// 	});
// 	$(".shrineremove").click(function() {
// 		shrinec_.splice(GetIntData($(this)),1);
// 		updateshrine_();
// 	});
// 	$(".shrine7pt").click(function() {
// 		if(shrinec_[$(this).attr("data")][7]!=7) {
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][7]=7;
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][9]=1;
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][6]=1;
// 		} else {
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][9]=0;
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][6]=0;
// 		}
// 		updateshrine_();
// 	});
// 	$(".shrine10k").click(function() {
// 		if(shrinec_[$(this).attr("data")][7]!=10000) {
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][7]=10000;
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][9]=1;
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][6]=1;
// 		} else {
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][9]=0;
// 			/** @type {number} */
// 			shrinec_[$(this).attr("data")][6]=0;
// 		}
// 		updateshrine_();
// 	});
// }
// * @param {string} data
// * @return {?}
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
function TwoDigitNum(n_3) {
    return n_3 > 9 ? `${n_3}` : `0${n_3}`;
}
function GetStringValue(a) {
    return a.val().toString();
}
function GetIntData(a) {
    return ToInt(a.attr("data"));
}
function GetFloatData(a) {
    return ToFloat(a.attr("data"));
}
function GetFloatValue(a) {
    return ToFloat(a.val());
}
function GetCidData(a) {
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
let tpicdiv_ = ["guard32 trooptdcm", "bally32 trooptdcm", "ranger32 trooptdcm", "triari32 trooptdcm", "priest32 trooptdcm", "vanq32 trooptdcm", "sorc32 trooptdcm", "scout32 trooptdcm", "arbal32 trooptdcm", "praet32 trooptdcm", "horsem32 trooptdcm", "druid32 trooptdcm", "ram32 trooptdcm", "scorp32 trooptdcm", "galley32 trooptdcm", "sting32 trooptdcm", "wship32 trooptdcm", "senat32 trooptdcm"];
/** @type {!Array} */
let tpicdiv20_ = ["guard20 trooptdcm", "bally20 trooptdcm", "ranger20 trooptdcm", "triari20 trooptdcm", "priest20 trooptdcm", "vanq20 trooptdcm", "sorc20 trooptdcm", "scout20 trooptdcm", "arbal20 trooptdcm", "praet20 trooptdcm", "horsem20 trooptdcm", "druid20 trooptdcm", "ram20 trooptdcm", "scorp20 trooptdcm", "galley20 trooptdcm", "sting20 trooptdcm", "wship20 trooptdcm", "senat20 trooptdcm"];
/** @type {!Array} */
let ttspeed_ = [0, 30, 20, 20, 20, 20, 20, 8, 10, 10, 10, 10, 30, 30, 5, 5, 5, 40, 40];
/** @type {!Array} */
let ttres_ = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
/** @type {number} */
let ttspeedres_ = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
/** @type {!Array} */
let TS_type_ = [0, 0, 1, 1, 1, 1, 1, 0, 2, 2, 2, 2, 0, 0, 0, 100, 400];
/** @type {!Array} */
let Total_Combat_Research_ = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1];
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
let cdata_;
let resl_ = [[]];
let OGA = [];
let wdata_;
let pldata_;
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
function InitCheckbox(v) {
    const i = $("#" + v);
    i.prop("checked", LocalStoreGetBool(v));
    i.change((newVal) => {
        LocalStoreSetBool(v, $(i).prop("checked"));
    });
}
function Checked(id) {
    return $(id).prop("checked");
}
function IsChecked(a) {
    return a.find(":input").prop("checked");
}
function RoundTo2Digits(num_5) {
    return (Math.round(num_5 * 100) / 100.0);
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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
let __base64Encode = null;
let __base64Decode = null;
let __a6 = {
    ccazzx: { encrypt: (a, b, c) => "", decrypt: (a, b, c) => "" }
};
function MakeGlobalGetter(a) {
    return `window['get${a}'] = ()=> ${a};`;
}
function MakeGlobalCopy(a) {
    return `window['__${a}'] = ${a};`;
}
function encryptJs(req, k2v) {
    console.log(req);
    console.log(k2v);
    return __a6.ccazzx.encrypt(JSON.stringify(k2v), ekeys[req], 256);
}
;
function betterBase64Decode() {
    try {
        //var me=arguments.callee.caller.caller.prototype;
        //me.eval(MakeGlobalGetter("D6"));
        //me.eval(MakeGlobalCopy("a6"));
        //console.log(window['GetD6']());;
        __a6.ccazzx.decrypt = arguments.callee.caller;
        //console.log(window['__a6']);
        // all done!
        String.prototype['base64Encode'] = __base64Decode;
    }
    catch (e) {
        // not ready yet, try again later
    }
    var rv = __base64Decode.call(this);
    //console.log(rv);
    return rv;
}
function betterBase64Encode() {
    try {
        //var me=arguments.callee.caller.caller.prototype;
        //me.eval(MakeGlobalGetter("D6"));
        //me.eval(MakeGlobalCopy("a6"));
        //console.log(window['GetD6']());;
        __a6.ccazzx.encrypt = arguments.callee.caller;
        //console.log(this);
        //console.log(window['__a6']);
        // all done!
        String.prototype['base64Encode'] = __base64Encode;
    }
    catch (e) {
        // not ready yet, try again later
    }
    return __base64Encode.call(this);
}
function GetCity() {
    //	return window['getD6']();
    return cdata_; //
}
/*function DummyPromise(data:string) {
    return new Promise<Response>();
}
*/
var __fetch = Window.prototype.fetch;
var __debugMe;
function sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time));
}
let defaultHeaders;
function SetupHeaders() {
    const cookie = ppdt['opt'][67].substring(0, 10);
    if (!cookie)
        throw "waiting";
    defaultHeaders = {
        'Content-Encoding': cookie,
        'pp-ss': ppss,
        'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
    };
    return;
}
//class DoneWrapper {
//	//public req: Promise<Response>;
//	dataResult: Promise<string>;
//	dataRequest: Promise<Response>;
//	onFail: (a) => void;
//	result: string;
//	reason: any;
//	that: this;
//	async done(cb: (a:string) =>void) {
//		await this.dataRequest;
//		let text=await this.dataResult;
//		cb(text);
//	}
//	fail(cb:any): this {
//		this.onFail=cb;
//		if(this.reason!=null)
//			this.onFail(this.reason);
//		return this;
//	}
//constructor(public url: string,public settings: JQueryAjaxSettings) { }
//async go() {
//		try {
//		let data=this.settings? this.settings.data:null;
//		console.log(data);
//			this.dataRequest= fetch(this.url,{
//			method: 'POST',
//			headers: new AvaHeaders(),
//			mode: 'cors',
//			cache: "no-cache",
//			body: data? (typeof data==="object"? $.param(data as object):(data as string)):""
//			});
//			let a= await this.dataRequest;
//			this.dataResult=a.text();
//			let dataText=await this.dataResult;
//				this.result=dataText;
//		//		if(this.onDone)
//			//			this.onDone(dataText);
//				if(this.settings&&this.settings.success) {
//					var suc=this.settings.success as JQuery.Ajax.SuccessCallback<any> ;
//					console.log(suc);
//					suc(dataText,null,null);
//					console.log("hope this works!");
//				}
//			await sleep(100);
//			__avatarAjaxDone(this.url,dataText);
//				//return cb(data);
//				//		_this.req.
//				//then(cb).
//				//catch(e => console.log(e));
//	} catch(reason) {
//				this.reason=reason;
//				console.log(reason);
//			if(this.onFail)
//				this.onFail(reason);
//			}
//	}
//}
//class AvaHeaders implements Headers {
//	a: Array<[string,string]>;
//	append(name: string,value: string): void {
//		throw new Error("Method not implemented.");
//	}
//	delete(name: string): void {
//		throw new Error("Method not implemented.");
//	}
//	get(name: string): string {
//		throw new Error("Method not implemented.");
//	}
//	has(name: string): boolean {
//		throw new Error("Method not implemented.");
//	}
//	set(name: string,value: string): void {
//		throw new Error("Method not implemented.");
//	}
//	forEach(callbackfn: (value: string,key: string,parent: Headers) => void,thisArg?: any): void {
//		throw new Error("Method not implemented.");
//	}
//	entries(): IterableIterator<[string,string]> {
//		throw new Error("Method not implemented.");
//	}
//	keys(): IterableIterator<string> {
//		throw new Error("Method not implemented.");
//	}
//	values(): IterableIterator<string> {
//		throw new Error("Method not implemented.");
//	}
//	[Symbol.iterator](): IterableIterator<[string,string]> {
//		return defaultHeaders[Symbol.iterator]();
//	}
//	return?(value?: any): IteratorResult<[string,string],any> {
//		throw new Error("Method not implemented.");
//	}
//	throw?(e?: any): IteratorResult<[string,string],any> {
//		throw new Error("Method not implemented.");
//	}
//	construtor() { this.a=defaultHeaders; }
//}
//function avatarPost(_url: string|JQuery.AjaxSettings,settings?: JQuery.AjaxSettings): DoneWrapper {
//	let url=_url as string;
//	if(typeof settings==='undefined') {
//		settings=_url as JQuery.AjaxSettings;
//		if(settings)
//			url=settings.url;
//	}
//	else if(!url) { url=settings.url; }
//	try {
//		let rv=new DoneWrapper(url,settings);
//		rv.go();
//		return rv;
//	} catch(e) {
//		console.log(e);
//	}
//	}
function Contains(a, b) {
    return a.indexOf(b) != -1;
}
function sendCityData() {
    console.log("sendCity");
    const wrapper = { citydata: cdata_ };
    window['external']['notify'](JSON.stringify(wrapper));
}
function __avatarAjaxDone(url, data) {
    //console.log("Change: " + this.readyState + " " + this.responseURL);
    let url_21 = url;
    if (Contains(url_21, "gC.php")) {
        cdata_ = JSON.parse(data);
        sendCityData();
        setTimeout(function () {
            /** @type {*} */
            updateattack_();
            updatedef_();
            makebuildcount_();
        }, 1000);
    }
    else if (Contains(url_21, "gaLoy.php")) {
        UpdateResearchAndFaith();
    }
    else if (Contains(url_21, "nBuu.php") || Contains(url_21, "UBBit.php")) {
        cdata_ = JSON.parse(data);
    }
    else if (Contains(url_21, "gWrd.php")) {
        setTimeout(function () {
            /** @type {*} */
            var wrapper = JSON.parse(data);
            /** @type {boolean} */
            beentoworld_ = true;
            wdata_ = DecodeWorldData(wrapper.a);
            UpdateResearchAndFaith();
            getbossinfo_();
        }, 500);
    }
    else if (Contains(url_21, "gPlA.php")) {
        /** @type {*} */
        pldata_ = JSON.parse(data);
    }
    // if(url_21.endsWith("pD.php")) {
    // 	pdata=JSON.parse(this.response);
    // }
    else if (Contains(url_21, "poll2.php")) {
        /** @type {*} */
        let poll2_ = JSON.parse(data);
        if ('OGA' in poll2_)
            OGA = poll2_.OGA;
        if ('city' in poll2_) {
            {
                cdata_ = Object.assign(cdata_, poll2_.city);
                if ('bd' in poll2_.city) {
                    let now = Date.now();
                    console.log("pollCity");
                    if (now > lastUpdate + 5000) {
                        lastUpdate = now;
                        sendCityData();
                        setTimeout(makebuildcount_, 400);
                    }
                }
            }
        }
    }
}
function _pleaseNoMorePrefilters() { }
function OptimizeAjax() {
    //	priorPrefilter
    jQuery.ajaxPrefilter((A7U, n7U, xhr) => {
        //	xhr.setRequestHeader("pp-ss", ppss);
        if (ppdt['opt'][67] !== undefined) {
            let cookie = ppdt['opt'][67].substring(0, 10);
            xhr.setRequestHeader("Content-Encoding", cookie);
        }
    });
    //jQuery.ajaxSetup({dataType:"nada" } )
    jQuery.ajaxPrefilter = _pleaseNoMorePrefilters;
    ;
    setTimeout(function () {
        (function (open_2) {
            /**
             * @param {string=} p0
             * @param {string=} p1
             * @param {(boolean|null)=} p2
             * @param {(null|string)=} p3
             * @param {(null|string)=} p4
             * @return {void}
             */
            XMLHttpRequest.prototype.open = function () {
                this.addEventListener("readystatechange", function () {
                    //console.log("Change: " + this.readyState + " " + this.responseURL);
                    if (this.readyState == 4) {
                        __avatarAjaxDone(this.responseURL, this.response);
                    }
                }, false);
                open_2.apply(this, arguments);
            };
        })(XMLHttpRequest.prototype.open);
    }, 100); /*
    __ajax=window['$']['ajax'];
        try {
            DoneWrapper.setup();
        
        } catch(e) {
            setTimeout(OptimizeAjax,1000);
            return;

        }
    window['$']['ajax']=avatarPost;
    */
    //	$.['post']=avatarPost;
    //jQuery.ajaxPrefilter=_ajaxPrefilter;
    //_ajaxPrefilter('text',avatarPrefilter)
    /*		function Inner() {
                try {
                    if(!ppdt) {
                        setTimeout(Inner,500);
                        return;
                    }
    
                    let encodingKey=(ppdt['opt'][67] as any as String).substring(0,10);
    
    
                    jQuery.ajaxSetup({
                        global: true,dataType: "text",enctype:'application/x-www-form-urlencoded; charset=UTF-8'
                        ,headers: { "pp-ss": "0","Content-Encoding": encodingKey } });
                }
                catch(e)
                {
                    console.log(e);
                    setTimeout(Inner,500);
                    return;
                }
            }
            Inner();
    */ 
}
function UpdateResearchAndFaith() {
    /**
     * @param {?} ldata_
     * @return {void}
      */
    try {
        // need to wait
        let faith = cotg.alliance.faith();
        let research = cotg.player.research();
        ttres_[0] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[29]]) / 100;
        ttres_[1] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[42]]) / 100;
        ttres_[2] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[30]]) / 100;
        ttres_[3] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[31]]) / 100;
        ttres_[4] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[32]]) / 100;
        ttres_[5] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[33]]) / 100;
        ttres_[6] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[34]]) / 100;
        ttres_[7] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[46]]) / 100;
        ttres_[8] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[35]]) / 100;
        ttres_[9] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[36]]) / 100;
        ttres_[10] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[37]]) / 100;
        ttres_[11] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[38]]) / 100;
        ttres_[12] = 1 + AsNumber(faith.cyndros) * 0.5 / 100 + AsNumber(Res_[research[39]]) / 100;
        ttres_[13] = 1 + AsNumber(faith.cyndros) * 0.5 / 100 + AsNumber(Res_[research[41]]) / 100;
        ttres_[14] = 1 + AsNumber(faith.ylanna) * 0.5 / 100 + AsNumber(Res_[research[44]]) / 100;
        ttres_[15] = 1 + AsNumber(faith.ylanna) * 0.5 / 100 + AsNumber(Res_[research[43]]) / 100;
        ttres_[16] = 1 + AsNumber(faith.cyndros) * 0.5 / 100 + AsNumber(Res_[research[45]]) / 100;
        ttspeedres_[1] = 1 + AsNumber(faith.domdis) * 0.5 / 100 + AsNumber(Res_[research[12]]) / 100;
        ttspeedres_[2] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[8]]) / 100;
        ttspeedres_[3] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[8]]) / 100;
        ttspeedres_[4] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[8]]) / 100;
        ttspeedres_[5] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[8]]) / 100;
        ttspeedres_[6] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[8]]) / 100;
        ttspeedres_[7] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[11]]) / 100;
        ttspeedres_[8] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[9]]) / 100;
        ttspeedres_[9] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[9]]) / 100;
        ttspeedres_[10] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[9]]) / 100;
        ttspeedres_[11] = 1 + AsNumber(faith.ibria) * 0.5 / 100 + AsNumber(Res_[research[9]]) / 100;
        ttspeedres_[12] = 1 + AsNumber(faith.domdis) * 0.5 / 100 + AsNumber(Res_[research[12]]) / 100;
        ttspeedres_[13] = 1 + AsNumber(faith.domdis) * 0.5 / 100 + AsNumber(Res_[research[12]]) / 100;
        ttspeedres_[14] = 1 + AsNumber(faith.domdis) * 0.5 / 100 + AsNumber(Res_[research[13]]) / 100;
        ttspeedres_[15] = 1 + AsNumber(faith.domdis) * 0.5 / 100 + AsNumber(Res_[research[13]]) / 100;
        ttspeedres_[16] = 1 + AsNumber(faith.domdis) * 0.5 / 100 + AsNumber(Res_[research[13]]) / 100;
        ttspeedres_[17] = 1 + AsNumber(faith.domdis) * 0.5 / 100 + AsNumber(Res_[research[14]]) / 100;
        Total_Combat_Research_[2] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[30]]) / 100;
        Total_Combat_Research_[3] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[31]]) / 100;
        Total_Combat_Research_[4] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[32]]) / 100;
        Total_Combat_Research_[5] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[33]]) / 100;
        Total_Combat_Research_[6] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[34]]) / 100;
        Total_Combat_Research_[7] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[46]]) / 100;
        Total_Combat_Research_[8] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[35]]) / 100;
        Total_Combat_Research_[9] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[36]]) / 100;
        Total_Combat_Research_[10] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[37]]) / 100;
        Total_Combat_Research_[11] = 1 + AsNumber(faith.vexemis) * 0.5 / 100 + AsNumber(Res_[research[38]]) / 100;
        Total_Combat_Research_[14] = 1 + AsNumber(faith.ylanna) * 0.5 / 100 + AsNumber(Res_[research[44]]) / 100;
        Total_Combat_Research_[15] = 1 + AsNumber(faith.ylanna) * 0.5 / 100 + AsNumber(Res_[research[43]]) / 100;
        Total_Combat_Research_[16] = 1 + AsNumber(faith.cyndros) * 0.5 / 100 + AsNumber(Res_[research[45]]) / 100;
    }
    catch (e) {
        setTimeout(UpdateResearchAndFaith, 1000);
        return;
    }
}
function getppdt() {
    return JSON.stringify(ppdt);
}
function jqclick(s) {
    $(s).click();
}
function getview() {
    if (regrender == 1)
        return "region";
    if (citrender = 1)
        return "city";
    return "world";
}
function avapost(url, args) {
    const k2D = $.post(url, args);
    k2D.done(s => {
        console.log(s);
    });
}
function avafetch(url, args) {
    return __awaiter(this, void 0, void 0, function* () {
        let req = fetch(url, {
            method: 'POST',
            headers: defaultHeaders,
            mode: 'cors',
            cache: "no-cache",
            body: args
        });
        let a = yield req;
        let txt = a.text();
        console.log(txt);
        return txt;
    });
}
function avactor() {
    //	var E3y="5894";
    var q7y = 15;
    var G5y = 128;
    var q3y = 16;
    var U7y = 256;
    var v1R = 192;
    var P2y = 1000;
    var l9p = 0xffff;
    var k9p = 0x100000000;
    console.log("here");
    //};
    function GetDate(jq) {
        return new Date($(jq).data().toString());
    }
    function sleep(time) {
        return new Promise((resolve) => setTimeout(resolve, time));
    }
    //var strings={};
    //async function Decode() {
    //	for(let i=0;i<10000;)
    //	{
    //		for(let j=0;j<100;++j,++i) {
    //			try {
    //				var x=_s(i);
    //				if(x)
    //				{
    //					strings[i]=x;
    //				}
    //			}
    //			catch(e)
    //			{
    //			}
    //			if((i%1000)==0)
    //				console.log(strings);
    //		}
    //		await sleep(10);
    //	}
    //	console.log("done");
    //	console.log(strings);
    //}
    /**
 * @return {void}
 */
    //  function AjaxPrefilterOverview() {
    //            jQuery.ajaxPrefilter(function (options, V7U, J7U) {
    //              console.log(options);
    //              if(options.requestHeaders != undefined)
    //              {
    //              console.log(options.requestHeaders[i011.S55('3885')]);
    //               delete options.requestHeaders[i011.S55('3885')];
    //               //J7U.setRequestHeader(i011.S55('3885'));
    //                console.log(options.requestHeaders[i011.S55('3885')]);
    //              }
    // 			  });
    //  }
    //function SetCityNotes(cid, notes) {
    //    includes / nnch.php
    //}
    /**
     * @param {!Date} date_2
     * @return {?}
     */
    function getFormattedTime(date_2) {
        var year_1 = date_2.getFullYear();
        var month_1 = (1 + date_2.getMonth()).toString();
        month_1 = month_1.length > 1 ? month_1 : `0${month_1}`;
        var day_ = date_2.getDate().toString();
        day_ = day_.length > 1 ? day_ : `0${day_}`;
        /** @type {string} */
        var hour_ = TwoDigitNum(date_2.getHours());
        /** @type {string} */
        var min_ = TwoDigitNum(date_2.getMinutes());
        /** @type {string} */
        var sec_ = TwoDigitNum(date_2.getSeconds());
        return `${month_1}/${day_}/${year_1} ${hour_}:${min_}:${sec_}`;
    }
    /**
     * @param {number} num_5
     * @return {number}
     */
    /**
     * @param {string} j_
     * @return {void}
     */
    function errorgo_(j_) {
        errz_ = errz_ + 1;
        /** @type {string} */
        let b_ = `errBR${errz_}`;
        /** @type {string} */
        let c_ = `#${b_}`;
        /** @type {string} */
        let d_ = `#${b_} div`;
        /** @type {string} */
        let errormsgs_ = `<tr ID = "${b_}"><td><div class = "errBR">${j_}<div></td></tr>`;
        $("#errorBRpopup").append(errormsgs_);
        $(c_).show();
        $(d_).animate({
            opacity: 1,
            bottom: "+10px"
        }, "slow");
        setTimeout(() => {
            $(d_).animate({
                opacity: 0,
                bottom: "-10px"
            }, "slow");
            $(c_).fadeOut("slow");
        }, 5000);
        setTimeout(() => {
            $(c_).remove();
        }, 6000);
    }
    setTimeout(() => {
        __base64Encode = String.prototype['base64Encode'];
        String.prototype['base64Encode'] = betterBase64Encode;
        __base64Decode = String.prototype['base64Decode'];
        String.prototype['base64Decode'] = betterBase64Decode;
        OptimizeAjax();
        $("<style type='text/css'> .ava{ width: auto; line-height:100%; table-layout: auto;text-align: center;padding-top:0px;padding-left:0px;border-width:1px;margin-left:0px } </style>").appendTo("head");
        $("<style type='text/css'> .ava td{ width: auto; line-height:100% table-layout: auto; text-align: center;padding-top:0px;padding-left:0px;border-width:1px;margin-left:0px} </style>").appendTo("head");
        $("<style type='text/css'> .ava table{table-layout: auto; } </style>").appendTo("head");
        //Decode();
        /** @type {string} */
        //  var popwin_ = `<div id='HelloWorld' style='width:400px;height:400px;background-color: #E2CBAC;-moz-border-radius: 10px;-webkit-border-radius: 10px;border-radius: 10px;border: 4px ridge #DAA520;position:absolute;right:40%;top:100px; z-index:1000000;'><div class=\"popUpBar\"> <span class=\"ppspan\">Welcome!</span><button id=\"cfunkyX\" onclick=\"$('#HelloWorld').remove();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div id='hellobody' class=\"popUpWindow\"><span style='margin-left: 5%;'> <h3 style='text-align:center;'>Welcome to Crown Of The Gods!</h3></span><br><br><span style='margin-left: 5%;'> <h4 style='text-align:center;'> MFunky(Cfunky + Dfunky + Mohnki's Additional Layouts + Avatar's nonsense)</h4></span><br><span style='margin-left: 5%;'> <h4 style='text-align:center;'>Updated Mar 1 2020</h4></span><br><br><span style='margin-left: 5%;'><h4>changes:</h4> <ul style='margin-left: 6%;'><li>Added 4 raiding carry percentages(100..125)</li><li>When you click on one, it will ensure that carry is at least that value and it will set it as the initial value for the next city that you go to</li></ul></span></div></div>`;
        //$("body").append(popwin_);
        window['alliancelink'] = gspotfunct.alliancelink;
        window['infoPlay'] = gspotfunct.infoPlay;
        window['shCit'] = gspotfunct.shCit;
        window['chcity'] = gspotfunct.chcity;
        String.prototype['utf8Encode'] = function () {
            console.log(this);
            return unescape(encodeURIComponent(this));
        };
        // if (typeof String.prototype.utf8Decode == i011.S55(h2R << 2061309088)) String.prototype.utf8Decode = function () {
        //   i011.R6();
        //   try {
        //     return decodeURIComponent(escape(this));
        //   } catch (g2v) {
        //     return this;
        //   }
        // };
        // if (typeof String.prototype.base64Decode == _s(h2R << 2140990016))
        //     String.prototype.base64Decode = function() {
        //       if (typeof atob != _s(h2R ^ 0))
        //         return atob(this);
        //       if (typeof Buffer != _s(h2R >> 1492823776))
        //         return new Buffer(this,i011.S55(+'\x31\x35\x32\x33')).toString(i011.S55("5503" ^ 0));
        //       throw new Error(_s("2559" >> 1791287200));
        //     }
        //     ;
        // if (typeof String.prototype.base64Encode == _s(+h2R))
        //     String.prototype.base64Encode = function() {
        //       if (typeof btoa != _s(+h2R))
        //         return btoa(this);
        //       if (typeof Buffer != _s(+h2R))
        //         return new Buffer(this,_s(+'\x35\x35\x30\x33')).toString(i011.S55('\x31\x35\x32\x33' | 1120));
        //       throw new Error(i011.S55(+"170"));
        //     }
        var a_5 = $("#organiser > option");
        var l_3 = a_5.length;
        /** @type {number} */
        var i_32 = 0;
        for (; i_32 < l_3; i_32++) {
            /** @type {string} */
            var temp_3 = String($(a_5[i_32]).attr("value"));
            //	$("#organiser").val(temp_3).change();
            /** @type {!Array} */
            //  console.log(ppdt.clc);
            //console.log(temp_3);
            //  if(ppdt.clc !== null && temp_3 !== null )
            //{
            //  ppdt.clc[temp_3] = [];
            //  var tempcl_ = $("#cityDropdownMenu > option");
            //  var ll_ = tempcl_.length;
            //  if (cdata_.cg,temp_3) > -1) {
            //    ppdt.clc[temp_3].push($(tempcl_[0]).attr("value"));
            //  }
            //  if (ll_ > 1) {
            //    /** @type {number} */
            //    var j_10 = 1;
            //    for (; j_10 < ll_; j_10++) {
            //      ppdt.clc[temp_3].push($(tempcl_[j_10]).attr("value"));
            //    }
            //  }
            //}
            //else
            //{
            //}
        }
        //	$("#organiser").val("all").change();
    }, 8000);
    //this	{"a":"worldButton","b":"block","c":true,"d":1591969039987,"e":"World"}
    /** @type {number} */
    var errz_ = 0;
    /**
     * @param {?} index_54
     * @param {string} char_
     * @return {?}
     * @this {!String}
     */
    function ReplaceAt(me, index_54, char_) {
        /** @type {!Array<string>} */
        var a_6 = me.split("");
        /** @type {string} */
        a_6[index_54] = char_;
        return a_6.join("");
    }
    /**
     * @return {?}
     * @this {!String}
     */
    // String.prototype.decrypt=function() {
    // 	/** @type {!String} */
    // 	var a_7=this;
    // 	var i_34;
    // 	for(i_34 in a_7) {
    // 		var j_11;
    // 		for(j_11 in key_35) {
    // 			if(a_7.charAt(i_34)==key_35.charAt(j_11)) {
    // 				a_7=a_7.replaceAt(i_34,j_11);
    // 			}
    // 		}
    // 	}
    // 	return a_7;
    // };
    /** @type {!Array} */
    var shrinec_ = [[]];
    var _mru = localStorage.getItem("mru");
    if (_mru != null)
        mru = JSON.parse(_mru);
    setTimeout(() => {
        UpdateResearchAndFaith();
        /** @type {string} */
        var returnAllbut_ = "<button id='returnAllb' style='right: 35.6%; margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Return All</button>";
        /** @type {string} */
        var raidbossbut_ = "<button id='raidbossGo' style='left: 65%;margin-left: 10px;margin-top: 15px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Locate Bosses</button>";
        /** @type {string} */
        var attackbut_ = "<button id='attackGo' style='margin-left: 25px;margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Attack Sender</button>";
        /** @type {string} */
        var defbut_ = "<button id='defGo' style='left: 65%;margin-left: 10px;margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Defense Sender</button>";
        /** @type {string} */
        var quickdefbut_ = "<button id='quickdefCityGo' style='width:96%; margin-top:2%; margin-left:2%;' class='regButton greenbuttonGo greenb'>@ Quick Reinforcements @</button>";
        /** @type {string} */
        var neardefbut_ = "<button id='ndefGo' style='left: 4%;margin-left: 3px;margin-top: 95px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'> Nearest Defense</button>";
        /** @type {string} */
        var nearoffbut_ = "<button id='noffGo' style='right: 35.6%; margin-top: 95px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'> Offensive list</button>";
        /** @type {string} */
        var addtoatt_ = "<button id='addtoAtt' style='margin-left: 7%;margin-top: -5%;width: 40%;height: 26px !important; font-size: 9px !important;' class='regButton greenb'>Add to Attack Sender</button>";
        /** @type {string} */
        var addtodef_ = "<button id='addtoDef' style='margin-left: 7%;width: 40%;height: 26px !important; font-size: 9px !important;' class='regButton greenb'>Add to Defense Sender</button>";
        /** @type {string} */
        var bosstab_ = "<li id='bosshuntab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warBossmanager'";
        /** @type {string} */
        bosstab_ = `${bosstab_}aria-labeledby='ui-id-20' aria-selected='false' aria-expanded='false'>`;
        /** @type {string} */
        bosstab_ = `${bosstab_}<a href='#warBossmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-20'>Find Bosses</a></li>`;
        /** @type {string} */
        var bosstabbody_ = "<div id='warBossmanager' aria-labeledby='ui-id-20' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        /** @type {string} */
        bosstabbody_ = `${bosstabbody_} role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >CFunky's Boss Raiding tool:</div>`;
        /** @type {string} */
        bosstabbody_ = `${bosstabbody_}<div id='bossbox' class='beigemenutable scroll-pane ava' style='width: 96%; height: 85%; margin-left: 2%;'></div>`;
        /** @type {string} */
        bosstabbody_ = `${bosstabbody_}<div id='idletroops'></div></div>`;
        /** @type {string} */
        var attacktab_ = "<li id='attacktab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warAttackmanager'";
        /** @type {string} */
        attacktab_ = `${attacktab_}aria-labeledby='ui-id-21' aria-selected='false' aria-expanded='false'>`;
        /** @type {string} */
        attacktab_ = `${attacktab_}<a href='#warAttackmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-21'>Attack</a></li>`;
        /** @type {string} */
        var attacktabbody_ = "<div id='warAttackmanager' aria-labeledby='ui-id-21' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_} role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Attack Sender:</div>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<div id='attackbox' class='beigemenutable scroll-pane ava' style='width: 53%; height: 50%; float:left; margin-left: 1%; margin-right: 1%;'>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<table><thead><th></th><th>X</th><th>Y</th><th>Type</th></thead><tbody>`;
        /** @type {number} */
        var i_35 = 1;
        for (; i_35 < 16; i_35++) {
            /** @type {string} */
            attacktabbody_ = `${attacktabbody_}<tr><td>Target ${i_35} </td><td><input id='t${i_35}x' type='number' style='width: 85%'></td><td><input id='t${i_35}y' type='number' style='width: 85%'></td>`;
            /** @type {string} */
            attacktabbody_ = `${attacktabbody_}<td><select id='type${i_35}' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'><option value='0'>Fake</option><option value='1'>Real</option></select></td></tr>`;
        }
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}</tbody></table></div>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<div id='picktype' class='beigemenutable scroll-pane ava' style='width: 43%; height: 50%;'></div>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<table><tr><td><span>Use percentage of troops:</span></td><td><input id='perc' type='number' style='width: 30px'>%</td><td></td></tr>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<tr><td><span>Send real as:</span></td><td><select id='realtype' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<option value='0'>Assault</option><option value='1'>Siege</option><option value='2'>Plunder</option><option value='3'>Scout</option></select></td><td></td></tr>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<tr><td><span>Send fake as:</span></td><td><select id='faketype' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<option value='0'>Assault</option><option value='1'>Siege</option><option value='2'>Plunder</option><option value='3'>Scout</option></select></td><td></td></tr>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<tr><td><input id='retcheck' class='clsubopti' type='checkbox' checked> Return all Troops</td><td colspan=2><input id='retHr' type='number' style='width: 20px' value='2'> Hours before attack</td></tr>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<tr><td><input id='scoutick' class='clsubopti' type='checkbox' checked>30galleys/1scout fake</td></tr></table>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<table style='width:96%;margin-left:2%'><thead><tr style='text-align:center;'><th colspan='5'>Date</th></tr>`;
        /** @type {string} */
        //    attacktabbody_ = attacktabbody_ + "<tr><td>Set Time: </td><td><input id='attackHr' type='number style='width: 35px;height: 22px;font-size: 10px;' value='10'></td><td><input id='attackMin' style='width: 35px;height: 22px;font-size: 10px;' type='number' value='00'></td>";
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<tr><td colspan='5'><input style='width:96%;' id='attackDat' type='datetime-local' step='1'></td></tr></tbody></table>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<table style='margin-left: 10%; margin-top:20px;'><tbody><tr><td style='width: 20%'><button id='Attack' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Attack!</button></td>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<td style='width: 20%'><button id='Aexport' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Export Order</button></td>`;
        /** @type {string} */
        attacktabbody_ = `${attacktabbody_}<td style='width: 20%'><button id='Aimport' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Import Order</button></td></tr></tbody></table>`;
        /** @type {string} */
        var deftab_ = "<li id='deftab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warDefmanager'";
        /** @type {string} */
        deftab_ = `${deftab_}aria-labeledby='ui-id-22' aria-selected='false' aria-expanded='false'>`;
        /** @type {string} */
        deftab_ = `${deftab_}<a href='#warDefmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-22'>Defend</a></li>`;
        /** @type {string} */
        var deftabbbody_ = "<div id='warDefmanager' aria-labeledby='ui-id-21' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_} role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Defense Sender:</div>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<div><p style='font-size: 10px;'>Defense sender will split all the troops you choose to send according to the number of targets you input.</p></div>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<div id='defbox' class='beigemenutable scroll-pane ava' style='width: 53%; height: 50%; float:left; margin-left: 1%; margin-right: 1%;'>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<table><thead><th></th><th>X</th><th>Y</th></thead><tbody>`;
        /** @type {number} */
        i_35 = 1;
        for (; i_35 <= 15; i_35++) {
            /** @type {string} */
            deftabbbody_ = `${deftabbbody_}<tr><td>Target ${i_35} </td><td><input id='d${i_35}x' type='number' style='width: 85%'></td><td><input id='d${i_35}y' type='number' style='width: 85%'></td></tr>`;
        }
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}</tbody></table></div>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<div id='dpicktype' class='beigemenutable scroll-pane ava' style='width: 43%; height: 50%;'></div>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<table><tr><td><span>Use percentage of troops:</span></td><td><input id='defperc' type='number' style='width: 30px'>%</td><td></td></tr>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<tr><td><span>Select Departure:</span></td><td><select id='defdeparture' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<option value='0'>Now</option><option value='1'>Departure time</option><option value='2'>Arrival time</option></select></td><td></td></tr>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<tr id='dret'><td><input id='dretcheck' class='clsubopti' type='checkbox' checked> Return all Troops</td><td colspan=2><input id='dretHr' type='number' style='width: 20px' value='2'> Hours before departure</td></tr></table>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<table id='deftime' style='width:96%;margin-left:2%'><thead><tr style='text-align:center;'><th></th><th>Hr</th><th>Min</th><th>Sec</th><th colspan='2'>Date</th></tr>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<tr><td>Set Time: </td><td><input id='defHr' type='number' style='width: 35px;height: 22px;font-size: 10px;' value='10'></td><td><input id='defMin' style='width: 35px;height: 22px;font-size: 10px;' type='number' value='00'></td>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<td><input style='width: 35px;height: 22px;font-size: 10px;' id='defSec' type='number' value='00'></td><td colspan='2'><input style='width:90px;' id='date' type='text' value='00/00/0000'></td></tr></tbody></table>`;
        /** @type {string} */
        deftabbbody_ = `${deftabbbody_}<button id='Defend' style='width: 35%;height: 30px; font-size: 12px; margin:10px;' class='regButton greenb'>Send Defense</button>`;
        /** @type {string} */
        var ndeftab_ = "<li id='neardeftab' class='ui-state-default ui-corner-top' role='tab'>";
        /** @type {string} */
        ndeftab_ = `${ndeftab_}<a href='#warNdefmanager' class='ui-tabs-anchor' role='presentation'>Near Def</a></li>`;
        /** @type {string} */
        var ndeftabbody_ = "<div id='warNdefmanager' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        /** @type {string} */
        ndeftabbody_ = `${ndeftabbody_} role='tabpanel' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Nearest defense:</div>`;
        /** @type {string} */
        ndeftabbody_ = ndeftabbody_ + '<table><td>Choose city:</td><td><input style=\'width: 30px;height: 22px;font-size: 10px;\' id=\'ndefx\' type=\'number\'> : <input style=\'width: 30px;height: 22px;font-size: 10px;\' id=\'ndefy\' type=\'number\'></td>';
        /** @type {string} */
        ndeftabbody_ = `${ndeftabbody_}<td>Showing For:</td><td id='asdfgh' class='coordblink shcitt'></td>`;
        /** @type {string} */
        ndeftabbody_ = `${ndeftabbody_}<td><button class='regButton greenb' id='ndefup' style='height:30px; width:70px;'>Update</button></td></table>`;
        /** @type {string} */
        ndeftabbody_ = `${ndeftabbody_}<div id='Ndefbox' class='beigemenutable scroll-pane ava' style='width: 96%; height: 85%; margin-left: 2%;'></div>`;
        /** @type {string} */
        var nofftab_ = "<li id='nearofftab' class='ui-state-default ui-corner-top' role='tab'>";
        /** @type {string} */
        nofftab_ = `${nofftab_}<a href='#warNoffmanager' class='ui-tabs-anchor' role='presentation'>Offensive TS</a></li>`;
        /** @type {string} */
        var nofftabbody_ = "<div id='warNoffmanager' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        /** @type {string} */
        nofftabbody_ = `${nofftabbody_} role='tabpanel' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >ALL Offensive TS:</div>`;
        /** @type {string} */
        nofftabbody_ = `${nofftabbody_}<table><td colspan='2'> Continent(99 for navy):</td><td><input style='width: 30px;height: 22px;font-size: 10px;' id='noffx' type='number' value='0'>`;
        /** @type {string} */
        nofftabbody_ = `${nofftabbody_}<td><button class='regButton greenb' id='noffup' style='height:30px; width:70px;'>Update</button></td>`;
        /** @type {string} */
        nofftabbody_ = `${nofftabbody_}<td id='asdfg' style='width:10% !important;'></td><td><button class='regButton greenb' id='mailoff' style='height:30px; width:50px;'>Mail</button></td><td><input style='width: 100px;height: 22px;font-size: 10px;' id='mailname' type='text' value='Name_here;'></table>`;
        /** @type {string} */
        nofftabbody_ = `${nofftabbody_}<div id='Noffbox' class='beigemenutable scroll-pane ava' style='width: 96%; height: 85%; margin-left: 2%;'></div>`;
        /** @type {string} */
        var expwin_ = "<div id='ExpImp' style='width:250px;height:200px;' class='popUpBox ui-draggable'><div class=\"popUpBar\"><span class=\"ppspan\">Import/Export attack orders</span>";
        /** @type {string} */
        expwin_ = `${expwin_}<button id="cfunkyX" onclick="$('#ExpImp').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div id='expbody' class="popUpWindow">`;
        /** @type {string} */
        expwin_ = `${expwin_}<textarea style='font-size:11px;width:300px;height:200px;' id='expstring'></textarea><button id='applyExp' style='margin-left: 15px; width: 100px;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Apply</button></div></div>`;
        var tabs_ = $("#warcouncTabs").tabs();
        var ul_ = tabs_.find("ul");
        $(bosstab_).appendTo(ul_);
        $(attacktab_).appendTo(ul_);
        $(deftab_).appendTo(ul_);
        $(ndeftab_).appendTo(ul_);
        $(nofftab_).appendTo(ul_);
        tabs_.tabs("refresh");
        $("#warCidlemanager").after(bosstabbody_);
        $("#warCidlemanager").after(attacktabbody_);
        $("#warAttackmanager").after(deftabbbody_);
        $("#warDefmanager").after(ndeftabbody_);
        $("#warNdefmanager").after(nofftabbody_);
        $("#senddefCityGo").after(quickdefbut_);
        $("#deftime").hide();
        $("#dret").hide();
        $("#warCounc").append(returnAllbut_);
        $("#warCounc").append(attackbut_);
        $("#warCounc").append(defbut_);
        $("#warCounc").append(neardefbut_);
        $("#warCounc").append(nearoffbut_);
        $("#coordstochatGo1").after(addtoatt_);
        $("#addtoAtt").after(addtodef_);
        $("#loccavwarconGo").css("right", "65%");
        $("#idluniwarconGo").css("left", "34%");
        $("#idluniwarconGo").after(raidbossbut_);
        $("#defdeparture").change(() => {
            if ($("#defdeparture").val() == 0) {
                $("#deftime").hide();
                $("#dret").hide();
            }
            else {
                $("#deftime").show();
                $("#dret").show();
            }
        });
        $("#troopperc").val(localStorage.getItem("troopperc") || 100);
        $("#retcheck").prop("checked", (LocalStoreAsInt("retcheck") == 1));
        $("#retHr").val(localStorage.getItem("retHr") || 0);
        // $("#attackDat").datepicker();
        $("#defDat").datepicker();
        $("#bosshuntab").click(() => {
            if (beentoworld_) {
                openbosswin_();
            }
            else {
                alert("Press World Button");
            }
        });
        $("#returnAllb").click(() => {
            jQuery.ajax({
                url: "includes/gIDl.php",
                type: "POST",
                // async false,
                success: function success_10(data_49) {
                    /** @type {*} */
                    var thdata_ = JSON.parse(data_49);
                    $("#returnAll").remove();
                    openreturnwin_(thdata_);
                }
            });
        });
        $("#raidbossGo").click(() => {
            if (beentoworld_) {
                $("#warcouncbox").show();
                tabs_.tabs("option", "active", 2);
                $("#bosshuntab").click();
            }
            else {
                alert("Press World Button");
            }
        });
        $("#Attack").click(() => {
            localStorage.setItem("troopperc", $("#troopperc").val().toString());
            localStorage.setItem("retHr", $("#retHr").toString());
            LocalStoreSet("retcheck", $("#retcheck").prop("checked") ? 1 : 0);
            SendAttack_();
        });
        //$("#Defend").click(() => {
        //	localStorage.setItem("troopperc",$("#troopperc").val().toString());
        //	localStorage.setItem("retHr",$("#retHr").val().toString());
        //	ResetCommandInfo();
        //	commandInfo.perc=$("#troopperc").val() as number;
        //	commandInfo.date=new Date($("#defdeparture").val() as string);
        //	commandInfo.ret=$("#dretcheck").prop("checked")==true? 1:0;
        //	LocalStoreSet("dretcheck",commandInfo.ret);
        //	commandInfo.date=$("#defDate").val();
        //	/** @type {number} */
        //	for(var i_36=1; i_36<=15; i_36++) {
        //		if($(`#d${i_36}x`).val()) {
        //			let tempx_6=$(`#d${i_36}x`).val();
        //			let tempy_6=$(`#d${i_36}y`).val();
        //			commandInfo.x.push(tempx_6);
        //			commandInfo.y.push(tempy_6);
        //			commandInfo.cstr.push(`${tempx_6}:${tempy_6}`);
        //			commandInfo.dist.push(Math.sqrt((tempx_6-cdata_.x)*(tempx_6-cdata_.x)+(tempy_6-cdata_.y)*(tempy_6-cdata_.y)));
        //			commandInfo.numb++;
        //		}
        //	}
        //	for(var i_36 in cdata_.tc) {
        //		if(cdata_.tc[i_36]) {
        //			commandInfo.tot.push(Math.ceil(cdata_.tc[i_36]*AsNumber($("#defperc").val())/100));
        //			commandInfo.home.push(Math.ceil(cdata_.th[i_36]*AsNumber($("#defperc").val())/100));
        //			commandInfo.type.push(AsNumber(i_36));
        //			if($(`#usedef${i_36}`).prop("checked")==true) {
        //				commandInfo.speed.push(ttspeed_[i_36]/ttspeedres_[i_36]);
        //				commandInfo.use.push(1);
        //			} else {
        //				commandInfo.speed.push(0);
        //				commandInfo.use.push(0);
        //			}
        //			commandInfo.amount.push(0);
        //		}
        //	}
        //	SendDef_();
        //});
        $("#attackGo").click(() => {
            $("#warcouncbox").show();
            tabs_.tabs("option", "active", 3);
            jQuery("#attacktab")[0].click();
        });
        $("#defGo").click(() => {
            $("#warcouncbox").show();
            tabs_.tabs("option", "active", 4);
            $("#deftab").click();
        });
        $("#ndefGo").click(() => {
            NearDefSubscribe();
            $("#warcouncbox").show();
            tabs_.tabs("option", "active", 5);
            //$("#neardeftab").trigger({
            //	type: "click",
            //	originalEvent: "1"
            //});
        });
        $("#neardeftab").click(() => {
            NearDefSubscribe();
        });
        $("#ui-id-115").click(() => {
            NearDefSubscribe();
        });
        $("#noffGo").click(() => {
            $("#warcouncbox").show();
            tabs_.tabs("option", "active", 6);
            //$("#nearofftab").trigger({
            //	type: "click",
            //	originalEvent: "1"
            //});
        });
        $("#addtoAtt").click(() => {
            /** @type {number} */
            var i_37 = 1;
            for (; i_37 < 16; i_37++) {
                if (!$(`#t${i_37}x`).val()) {
                    /** @type {number} */
                    var tid_4 = AsNumber($("#showReportsGo").attr("data"));
                    var tempx_7;
                    var tempy_7;
                    /** @type {number} */
                    tempx_7 = AsNumber(tid_4 % 65536);
                    /** @type {number} */
                    tempy_7 = AsNumber((tid_4 - tempx_7) / 65536);
                    $(`#t${i_37}x`).val(tempx_7);
                    $(`#t${i_37}y`).val(tempy_7);
                    break;
                }
            }
        });
        $("#addtoDef").click(() => {
            /** @type {number} */
            var i_38 = 1;
            for (; i_38 < 16; i_38++) {
                if (!$(`#d${i_38}x`).val()) {
                    /** @type {number} */
                    var tid_5 = AsNumber($("#showReportsGo").attr("data"));
                    var tempx_8;
                    var tempy_8;
                    /** @type {number} */
                    tempx_8 = AsNumber(tid_5 % 65536);
                    /** @type {number} */
                    tempy_8 = AsNumber((tid_5 - tempx_8) / 65536);
                    $(`#d${i_38}x`).val(tempx_8);
                    $(`#d${i_38}y`).val(tempy_8);
                    break;
                }
            }
        });
        $("#quickdefCityGo").click(() => {
            ///@todo
            ///** @type {number} */
            //var tid_6=AsNumber($("#showReportsGo").attr("data"));
            //var tempx_9;
            //var tempy_9;
            ///** @type {number} */
            //tempx_9=AsNumber(tid_6%65536);
            ///** @type {number} */
            //tempy_9=AsNumber((tid_6-tempx_9)/65536);
            //commandInfo.t.targets.dist.push(Math.sqrt((tempx_9-cdata_.x)*(tempx_9-cdata_.x)+(tempy_9-cdata_.y)*(tempy_9-cdata_.y)));
            //var i_39;
            //for(i_39 in cdata_.th) {
            //	if(cdata_.th[i_39]) {
            //		defobj_2.t.home.push(Math.ceil(cdata_.th[i_39]*AsNumber($("#defperc").val())/100));
            //		defobj_2.t.type.push(AsNumber(i_39));
            //		defobj_2.t.speed.push(ttspeed_[i_39]/ttspeedres_[i_39]);
            //		defobj_2.t.use.push(1);
            //		defobj_2.t.amount.push(0);
            //	}
            //}
            //SendDef_(defobj_2);
        });
        $("#ndefup").click(() => {
            /** @type {number} */
            var tempxs_ = AsNumber($("#ndefx").val());
            /** @type {number} */
            var tempys_ = AsNumber($("#ndefy").val());
            /** @type {number} */
            var tids_ = tempxs_ + tempys_ * 65536;
            $("#asdfgh").data('data', tids_);
            $("#asdfgh").text(`${tempxs_}:${tempys_}`);
            ///@todo
            //jQuery.ajax({
            //	url: "overview/trpover.php",
            //	type: "POST",
            //	async: true,
            //	success: function success_11(data_53) {
            //		/** @type {*} */
            //		var t_6=JSON.parse(data_53);
            //		neardeftable_(t_6);
            //	}
            //});
        });
        $("#noffup").click(() => {
            ///@todo
            //jQuery.ajax({
            //	url: "overview/trpover.php",
            //	type: "POST",
            //	async: true,
            //	success: function success_12(data_54) {
            //		/** @type {*} */
            //		var t_7=JSON.parse(data_54);
            //		nearofftable_(t_7);
            //	}
            //});
        });
        $("#Aexport").click(() => {
            var Aexp_1 = {
                x: [],
                y: [],
                type: [],
                time: []
            };
            /** @type {number} */
            var i_40 = 1;
            for (; i_40 < 16; i_40++) {
                if ($(`#t${i_40}x`).val()) {
                    Aexp_1.x.push($(`#t${i_40}x`).val());
                    Aexp_1.y.push($(`#t${i_40}y`).val());
                    Aexp_1.type.push($(`#type${i_40}`).val());
                }
            }
            /** @type {Date} */
            var date = GetDate("#attackDat");
            Aexp_1.time[0] = date.getHours();
            Aexp_1.time[1] = date.getMinutes();
            Aexp_1.time[2] = date.getSeconds();
            Aexp_1.time[3] = date.toLocaleDateString();
            prompt("Attack Orders Expot", JSON.stringify(Aexp_1));
        });
        $("#Aimport").click(() => {
            $("body").append(expwin_);
            $("#ExpImp").draggable({
                handle: ".popUpBar",
                containment: "window",
                scroll: false
            });
            document.addEventListener("paste", evt_27 => {
                $("#expstring").val(evt_27.clipboardData.getData("text/plain"));
            });
            $("#applyExp").click(() => {
                Aimp_($("#expstring").val());
                $("#ExpImp").remove();
            });
        });
        /** @type {string} */
        var fourbutton_ = "<div id='fourbuttons' class='commandinndiv'><div><button id='fb1' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>TBA</button><button id='fb2' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Refine</button><button id='fb3' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Raid</button><button id='fb4' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Demolish</button></div></div>";
        /** @type {string} */
        var bdcountbox_ = "<div id='currentBd'><div id='bdcountbar' class='queueBar'>";
        /** @type {string} */
        bdcountbox_ = `${bdcountbox_}<div id='bdcountbut' class='tradeqarr2'><div></div></div><span class='qbspan'>Current Buildings</span>`;
        /** @type {string} */
        bdcountbox_ = `${bdcountbox_}<div id='numbdleft' class='barRightFloat tooltipstered'>0</div>`;
        /** @type {string} */
        bdcountbox_ = `${bdcountbox_}</div><div id='bdcountwin' class='queueWindow' style='display: block;'></div></div>`;
        $("#buildQueue").before(fourbutton_);
        /** @type {string} */
        var fillbut_ = '<button id="fillque" class="greenb tooltipstered" style="height:18px; width:40px; margin-left:7px; margin-top:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;">Fill</button>';
        $("#sortbut").after(fillbut_);
        $("#fillque").click(() => {
            OverviewPost('overview/fillq.php', { a: cotg.city.id() });
            event.stopPropagation();
        });
        /** @type {string} */
        var convbut_ = '<button id="convque" class="greenb tooltipstered" style="height:18px; width:60px; margin-left:7px; margin-top:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;">Convert</button>';
        $("#sortbut").after(convbut_);
        $("#convque").click(() => {
            OverviewPost('overview/mconv.php', { a: cotg.city.id() });
            event.stopPropagation();
        });
        //$("#fb1").click(function() {
        //  $("#councillorPopUpBox").show();
        //  jQuery("#ui-id-11")[0].click();
        //  jQuery("#couonoffdv")[0].click();
        //  setTimeout(() => {
        //	jQuery("#councillorXbutton")[0].click();
        //  }, 100);
        //  if (coon_ == 0) {
        //	/** @type {number} */
        //	coon_ = 1;
        //	$(this).removeClass("greenb");
        //	$(this).addClass("redb");
        //  } else {
        //	/** @type {number} */
        //	coon_ = 0;
        //	$(this).removeClass("redb");
        //	$(this).addClass("greenb");
        //  }
        //});
        $("#fb2").click(() => {
            $("#tradePopUpBox").show();
            setTimeout(() => {
                jQuery("#ui-id-27")[0].click();
            }, 100);
        });
        $("#fb3").click(() => {
            $("#warcouncbox").show();
            jQuery("#ui-id-19")[0].click();
        });
        /** @type {number} */
        var autodemoon_ = 0;
        $("#fb4").click(function () {
            if (autodemoon_ == 0) {
                /** @type {number} */
                autodemoon_ = 1;
                $(this).removeClass("greenb");
                $(this).addClass("redb");
            }
            else {
                /** @type {number} */
                autodemoon_ = 0;
                $(this).removeClass("redb");
                $(this).addClass("greenb");
            }
        });
        $("#centarrowNextDiv").click(() => {
            /** @type {number} */
            autodemoon_ = 0;
            $("#fb4").removeClass("redb").addClass("greenb");
        });
        $("#centarrowPrevDiv").click(() => {
            /** @type {number} */
            autodemoon_ = 0;
            $("#fb4").removeClass("redb").addClass("greenb");
        });
        $("#ddctd").click(() => {
            /** @type {number} */
            autodemoon_ = 0;
            $("#fb4").removeClass("redb").addClass("greenb");
        });
        $("#qbuildtbButton").click(() => {
            /** @type {number} */
            autodemoon_ = 0;
            $("#fb4").removeClass("redb").addClass("greenb");
        });
        $("#city_map").click(() => {
            if (autodemoon_ == 1) {
                $("#buildingDemolishButton").trigger("click", "1");
            }
        });
        /** @type {string} */
        var sumbut_ = "<button class='tabButton' id='Sum'>Summary</button>";
        $("#items").after(sumbut_);
        $("#Sum").click(() => {
            if (sum_) {
                opensumwin_();
            }
            else {
                $("#sumWin").show();
            }
        });
        $("#sumWin").click(() => {
            console.log("popsum");
        });
        $("#recruitmentQueue").before(bdcountbox_);
        $("#bdcountbut").click(() => {
            if (bdcountshow_) {
                $("#bdcountwin").hide();
                $("#bdcountbut").removeClass("tradeqarr2").addClass("tradeqarr1");
                /** @type {boolean} */
                bdcountshow_ = false;
            }
            else {
                $("#bdcountwin").show();
                $("#bdcountbut").removeClass("tradeqarr1").addClass("tradeqarr2");
                /** @type {boolean} */
                bdcountshow_ = true;
            }
        });
        /** @type {string} */
        var wood50_ = "<td><button class='brownb' id='wood50'>Add 50%</button></td>";
        $("#woodmaxbutton").parent().after(wood50_);
        $("#wood50").click(() => {
            /** @type {number} */
            var res_3 = AsNumber($("#maxwoodsend").text().replace(/,/g, ""));
            if ($("#landseasendres").val() == "1") {
                /** @type {number} */
                var carts_ = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
            }
            else {
                /** @type {number} */
                carts_ = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
            }
            if (res_3 > carts_) {
                /** @type {number} */
                res_3 = carts_;
            }
            $("#woodsendamt").val(res_3);
        });
        /** @type {string} */
        var stone50_ = "<td><button class='brownb' id='stone50'>Add 50%</button></td>";
        $("#stonemaxbutton").parent().after(stone50_);
        $("#stone50").click(() => {
            if ($("#landseasendres").val() == "1") {
                /** @type {number} */
                var carts_1 = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
            }
            else {
                /** @type {number} */
                carts_1 = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
            }
            /** @type {number} */
            var res_4 = AsNumber($("#maxstonesend").text().replace(/,/g, ""));
            if (res_4 > carts_1) {
                /** @type {number} */
                res_4 = carts_1;
            }
            $("#stonesendamt").val(res_4);
        });
        /** @type {string} */
        var iron50_ = "<td><button class='brownb' id='iron50'>Add 50%</button></td>";
        $("#ironmaxbutton").parent().after(iron50_);
        $("#iron50").click(() => {
            /** @type {number} */
            var res_5 = AsNumber($("#maxironsend").text().replace(/,/g, ""));
            if ($("#landseasendres").val() == "1") {
                /** @type {number} */
                var carts_2 = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
            }
            else {
                /** @type {number} */
                carts_2 = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
            }
            if (res_5 > carts_2) {
                /** @type {number} */
                res_5 = carts_2;
            }
            $("#ironsendamt").val(res_5);
        });
        /** @type {string} */
        var food50_ = "<td><button class='brownb' id='food50'>Add 50%</button></td>";
        $("#foodmaxbutton").parent().after(food50_);
        $("#food50").click(() => {
            /** @type {number} */
            var res_6 = AsNumber($("#maxfoodsend").text().replace(/,/g, ""));
            if ($("#landseasendres").val() == "1") {
                /** @type {number} */
                var carts_3 = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
            }
            else {
                /** @type {number} */
                carts_3 = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
            }
            if (res_6 > carts_3) {
                /** @type {number} */
                res_6 = carts_3;
            }
            $("#foodsendamt").val(res_6);
        });
        /** @type {string} */
        var shrinebut_ = "<button class='regButton greenb' id='shrineP' style='width: 98%;margins: 1%;'>Shrine Planner</button>";
        $("#inactiveshrineInfo").before(shrinebut_);
        $("#shrineP").click(() => {
            if (beentoworld_) {
                /** @type {!Array} */
                shrinec_ = [[]];
                splayers_ = {
                    name: [],
                    ally: [],
                    cities: []
                };
                /** @type {!Array} */
                var players_ = [];
                var coords_ = $("#coordstochatGo3").attr("data");
                /** @type {number} */
                var shrinex_ = parseInt(coords_);
                /** @type {number} */
                var shriney_ = AsNumber(coords_.match(/\d+$/)[0]);
                /** @type {number} */
                var shrinecont_ = AsNumber(Math.floor(shrinex_ / 100) + 10 * Math.floor(shriney_ / 100));
                var i_41;
                for (i_41 in wdata_.cities) {
                    /** @type {number} */
                    var tempx_10 = AsNumber(wdata_.cities[i_41].substr(8, 3)) - 100;
                    /** @type {number} */
                    var tempy_10 = AsNumber(wdata_.cities[i_41].substr(5, 3)) - 100;
                    /** @type {number} */
                    var cont_2 = AsNumber(Math.floor(tempx_10 / 100) + 10 * Math.floor(tempy_10 / 100));
                    if (cont_2 == shrinecont_) {
                        /** @type {number} */
                        var dist_1 = Math.sqrt((tempx_10 - shrinex_) * (tempx_10 - shrinex_) + (tempy_10 - shriney_) * (tempy_10 - shriney_));
                        if (dist_1 < 10) {
                            /** @type {number} */
                            var l_4 = AsNumber(wdata_.cities[i_41].substr(11, 1));
                            /** @type {number} */
                            var pid_ = AsNumber(wdata_.cities[i_41].substr(12, l_4));
                            var pname_12 = pldata_[pid_];
                            /** @type {!Array} */
                            var csn_ = [3, 4, 7, 8];
                            if (csn_.indexOf(AsNumber(wdata_.cities[i_41].charAt(4))) > -1) {
                                shrinec_.push(["castle", pname_12, 0, tempx_10, tempy_10, dist_1, "0", 0, 0, 0]);
                            }
                            else {
                                shrinec_.push(["city", pname_12, 0, tempx_10, tempy_10, dist_1, "0", 0, 0, 0]);
                            }
                        }
                    }
                }
                shrinec_.sort((a, b) => {
                    return a[5] - b[5];
                });
                /** @type {string} */
                var planwin_ = "<div id='shrinePopup' style='width:40%;height:50%;left: 360px; z-index: 3000;' class='popUpBox'><div class='popUpBar'><span class=\"ppspan\">Shrine Planner</span><button id='hidec' class='greenb' style='margin-left:10px;border-radius: 7px;margin-top: 2px;height: 28px;'>Hide Cities</button>";
                /** @type {string} */
                planwin_ = `${planwin_}<button id='addcity' class='greenb' style='margin-left:10px;border-radius: 7px;margin-top: 2px;height: 28px;'>Add City</button><button id="sumX" onclick="$('#shrinePopup').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div class="popUpWindow" style='height:100%'>`;
                /** @type {string} */
                planwin_ = `${planwin_}<div id='shrinediv' class='beigemenutable scroll-pane ava' style='background:none;border: none;padding: 0px;height:90%;'></div></div>`;
                for (i_41 in shrinec_) {
                    if (i_41 < 101) {
                        pname_12 = shrinec_[i_41][1];
                        if (players_.indexOf(pname_12) == -1) {
                            players_.push(pname_12);
                            jQuery.ajax({
                                url: "includes/gPi.php", type: "POST",
                                async: true,
                                data: {
                                    a: pname_12
                                },
                                success: function success_13(_data) {
                                    var pinfo_ = JSON.parse(_data);
                                    splayers_.name.push(pinfo_.player);
                                    splayers_.ally.push(pinfo_.a);
                                    splayers_.cities.push(pinfo_.h);
                                }
                            });
                            //data: {
                            //	a: pname_12
                            //},
                            //success: function success_13(status: SuccessTextStatus,j: jqXHR) {
                            //	/** @type {*} */
                            //	var pinfo_=JSON.parse(data_55);
                            //	splayers_.name.push(pinfo_.player);
                            //	splayers_.ally.push(pinfo_.a);
                            //	splayers_.cities.push(pinfo_.h);
                            //}
                        }
                        ;
                    }
                }
                setTimeout(() => {
                    $("#reportsViewBox").after(planwin_);
                    $("#shrinePopup").draggable({
                        handle: ".popUpBar",
                        containment: "window",
                        scroll: false
                    });
                    $("#shrinePopup").resizable();
                    if (localStorage.getItem("hidecities")) {
                        1 == 1;
                    }
                    else {
                        localStorage.setItem("hidecities", "0");
                    }
                    if (localStorage.getItem("hidecities") == "1") {
                        $("#hidec").html("Show Cities");
                    }
                    $("#hidec").click(() => {
                        if (localStorage.getItem("hidecities") == "0") {
                            hidecities_();
                            localStorage.setItem("hidecities", "1");
                            $("#hidec").html("Show Cities");
                        }
                        else {
                            if (localStorage.getItem("hidecities") == "1") {
                                showcities_();
                                localStorage.setItem("hidecities", "0");
                                $("#hidec").html("Hide Cities");
                            }
                        }
                    });
                    updateshrine_();
                    /** @type {string} */
                    var addcitypop_ = "<div id='addcityPopup' style='width:500px;height:100px;left: 360px; z-index: 3000;' class='popUpBox'><div class='popUpBar'><span class=\"ppspan\">Add City</span>";
                    /** @type {string} */
                    addcitypop_ = `${addcitypop_}<button id="sumX" onclick="$('#addcityPopup').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div class="popUpWindow" style='height:100%'>`;
                    /** @type {string} */
                    addcitypop_ = `${addcitypop_}<div><table><td>X: <input id='addx' type='number' style='width: 35px;height: 22px;font-size: 10px;'></td><td>y: <input id='addy' type='number' style='width: 35px;height: 22px;font-size: 10px;'></td>`;
                    /** @type {string} */
                    addcitypop_ = `${addcitypop_}<td>score: <input id='addscore' type='number' style='width: 45px;height: 22px;font-size: 10px;'></td><td>Type: <select id='addtype' class='greensel' style='font-size: 15px !important;width:55%;height:30px;'>`;
                    /** @type {string} */
                    addcitypop_ = `${addcitypop_}<option value='city'>City</option><option value='castle'>Castle</option></select></td><td><button id='addadd' class='greenb'>Add</button></td></table></div></div>`;
                    $("#addcity").click(() => {
                        $("body").append(addcitypop_);
                        $("#addcityPopup").draggable({
                            handle: ".popUpBar",
                            containment: "window",
                            scroll: false
                        });
                        $("#addadd").click(() => {
                            let tempx_10 = $("#addx").val();
                            let tempy_10 = $("#addy").val();
                            /** @type {number} */
                            dist_1 = Math.sqrt((tempx_10 - shrinex_) * (tempx_10 - shrinex_) + (tempy_10 - shriney_) * (tempy_10 - shriney_));
                            /** @type {!Array} */
                            var temp_4 = [$("#addtype").val(), "Poseidon", "Atlantis", tempx_10, tempy_10, dist_1, "1", $("#addscore").val(), "Hellas", "1"];
                            shrinec_.push(temp_4);
                            shrinec_.sort((a_9, b_7) => {
                                return a_9[5] - b_7[5];
                            });
                            updateshrine_();
                            $("#addcityPopup").remove();
                        });
                    });
                }, 2000);
            }
            else {
                alert("Press World Button");
            }
        });
        var incomingtabledata_ = $("#incomingsAttacksTable").children().children().children();
        $("#incomingsAttacksTable table thead tr th:nth-child(2)").width(140);
        /** @type {string} */
        var Addth_ = "<th>Lock time</th>";
        incomingtabledata_.append(Addth_);
        /** @type {string} */
        var Addth1_ = "<th>Travel time</th>";
        incomingtabledata_.append(Addth1_);
        $("#allianceIncomings").parent().click(() => {
            setTimeout(() => {
                incomings_();
            }, 5000);
        });
        $("#incomingsPic").click(() => {
            setTimeout(() => {
                incomings_();
            }, 5000);
        });
        cotgsubscribe.subscribe("regional", clickInfo => {
            var dtype_ = clickInfo.type;
            var type_113 = clickInfo.info.type;
            var lvl_ = clickInfo.info.lvl;
            var prog_ = clickInfo.info.prog;
            var bossname_ = clickInfo.info.name;
            console.log(clickInfo);
            UpdateResearchAndFaith();
            let troops = cotg.city.troops();
            if (dtype_ === "dungeon") {
                if ($("#cityplayerInfo div table tbody tr").length >= 11) {
                    bossele_();
                }
                /** @type {number} */
                let home_loot_2 = 0;
                for (let i in troops) {
                    let d = troops[i];
                    /** @type {number} */
                    let home_1 = d.home;
                    /** @type {number} */
                    home_loot_2 = home_loot_2 + home_1 * ttloot_[TroopNameToId(i)];
                }
                if (type_113 === "Siren's Cove") {
                    /** @type {number} */
                    let optimalTS_ = Math.ceil(other_loot_[lvl_ - 1] / 10 * (1 - prog_ / 100 + 1) * 1.05);
                    /** @type {number} */
                    var galleyTS_ = Math.ceil(optimalTS_ / 100);
                    /** @type {number} */
                    var stingerTS_ = Math.ceil(optimalTS_ / 150);
                    /** @type {number} */
                    var warshipTS_ = Math.ceil(optimalTS_ / 300);
                    /**
                     * @return {void}
                     */
                    document.getElementById("raidDungGo").onclick = () => {
                        setTimeout(() => {
                            if (troops.warship.home > warshipTS_) {
                                $("#raidIP16").val(warshipTS_);
                            }
                            else {
                                if (troops.stinger.home > stingerTS_) {
                                    $("#raidIP15").val(stingerTS_);
                                }
                                else {
                                    if (troops.galley.home > galleyTS_) {
                                        $("#raidIP14").val(galleyTS_);
                                    }
                                    else {
                                        errorgo_(message_23);
                                    }
                                }
                            }
                        }, 1500);
                    };
                    $("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text(galleyTS_);
                    $("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text(stingerTS_);
                    $("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text(warshipTS_);
                }
                else {
                    let loot_;
                    if (type_113 === "Mountain Cavern")
                        loot_ = mountain_loot_;
                    else
                        loot_ = other_loot_;
                    /**
                     * @return {void}
                     */
                    const total_lootm_ = Math.ceil(loot_[AsNumber(lvl_) - 1] * (1 - AsNumber(prog_) / 100 + 1) * 1.05);
                    document.getElementById("raidDungGo").onclick = () => {
                        setTimeout(() => {
                            /** @type {number} */
                            if (home_loot_2 > total_lootm_) {
                                /** @type {number} */
                                const option_numbersm_ = Math.floor(home_loot_2 / total_lootm_);
                                /** @type {number} */
                                const templ1m_ = home_loot_2 / total_lootm_ * 100 / option_numbersm_;
                                /** @type {number} */
                                const templ2m_ = (templ1m_ - 100) / templ1m_ * 100;
                                /** @type {number} */
                                for (let i in troops) {
                                    const id = TroopNameToId(i);
                                    const th = troops[i].home;
                                    /** @type {number} */
                                    $(`#raidIP${id}`).val(th / option_numbersm_);
                                }
                            }
                        }, 1500);
                    };
                    /** @type {number} */
                    const optimalTSM_ = total_lootm_;
                    /** @type {number} */
                    var cavoptim_ = Math.ceil(optimalTSM_ * 2 / 3);
                    /** @type {number} */
                    var praoptim_ = Math.ceil(optimalTSM_ / 2);
                    /** @type {number} */
                    var sorcoptim_ = Math.ceil(optimalTSM_ * 2);
                    /** @type {number} */
                    var RToptim_ = Math.ceil(optimalTSM_ / 3);
                    $("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(optimalTSM_);
                    $("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text(`${RToptim_}/${RToptim_}`);
                    $("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(optimalTSM_);
                    $("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(praoptim_);
                    $("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(cavoptim_);
                    $("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(cavoptim_);
                    $("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(sorcoptim_);
                    $("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(optimalTSM_);
                    $("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(optimalTSM_);
                    $("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(praoptim_);
                    $("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
                    $("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
                }
            }
            if (dtype_ === "boss") {
                /// @todo
                //if($("#cityplayerInfo div table tbody tr").length===11) {
                //	bossele_();
                //}
                //if(clickInfo.info.active) {
                //	/** @type {string} */
                //	message_23="Inactive Boss";
                //	errorgo_(message_23);
                //}
                ///** @type {string} */
                //message_23="Not enough TS to kill this boss!";
                ///** @type {!Array} */
                //var attackres_=[];
                ///** @type {!Array} */
                //var attackwres_=[];
                //for(i_42 in ttattack_) {
                //	/** @type {number} */
                //	var bossTS_=Math.ceil(AsNumber(bossdef_[lvl_-1])*4/(AsNumber(ttattack_[i_42])*AsNumber(Total_Combat_Research_[i_42])));
                //	attackres_.push(bossTS_);
                //	/** @type {number} */
                //	var bosswTS_=Math.ceil(AsNumber(bossdefw_[lvl_-1])*4/(AsNumber(ttattack_[i_42])*AsNumber(Total_Combat_Research_[i_42])));
                //	attackwres_.push(bosswTS_);
                //}
                ///** @type {number} */
                //var home_strength_=0;
                ///** @type {number} */
                //home_loot_2=0;
                ///** @type {!Array} */
                //km_2=[];
                ///** @type {!Array} */
                //var bm_=[];
                ///** @type {!Array} */
                //var bmw_=[];
                ///** @type {!Array} */
                //var kg_=[];
                ///** @type {number} */
                //var home_TSw_=0;
                ///** @type {number} */
                //var boss_strength_=Math.ceil(AsNumber(bossdef_[lvl_-1])*4);
                ///** @type {number} */
                //var boss_strengthw_=Math.ceil(AsNumber(bossdefw_[lvl_-1])*4);
                ///** @type {number} */
                //i_42=0;
                //for(var x_85 in cdata_.th) {
                //	/** @type {number} */
                //	let home_1=AsNumber(cdata_.th[x_85]);
                //	if(i_42===0||i_42===1||i_42===7||i_42===12||i_42===13) {
                //		/** @type {number} */
                //		home_1=0;
                //	}
                //	kg_.push(home_1);
                //	if(i_42===14||i_42===15||i_42===16) {
                //		/** @type {number} */
                //		home_1=0;
                //	}
                //	/** @type {number} */
                //	home_strength_=home_strength_+AsNumber(ttattack_[i_42])*AsNumber(home_1)*AsNumber(Total_Combat_Research_[i_42]);
                //	/** @type {number} */
                //	home_TSw_=home_TSw_+home_1*TS_type_[i_42];
                //	km_2.push(home_1);
                //	/** @type {number} */
                //	i_42=i_42+1;
                //	if(i_42===17) {
                //		break;
                //	}
                //}
                //if(home_strength_>boss_strength_) {
                //	/** @type {number} */
                //	var proportion_=home_strength_/boss_strength_;
                //	for(i_42 in km_2) {
                //		/** @type {number} */
                //		bm_[i_42]=Math.ceil(AsNumber(km_2[i_42])/proportion_);
                //	}
                //}
                //if(home_strength_>boss_strengthw_) {
                //	/** @type {number} */
                //	var proportionw_=home_strength_/boss_strengthw_;
                //	for(i_42 in km_2) {
                //		/** @type {number} */
                //		bmw_[i_42]=Math.ceil(AsNumber(km_2[i_42])/proportionw_);
                //	}
                //}
                //if(bossname_==="Triton") {
                //	/** @type {!Array} */
                //	var bmz_=[];
                //	/** @type {number} */
                //	var home_strengthw_=0;
                //	/** @type {number} */
                //	var galleytroops_=0;
                //	/** @type {number} */
                //	var tempgalley_=0;
                //	/** @type {number} */
                //	var galley_TSneeded_=Math.ceil(home_TSw_/500);
                //	if(kg_[14]) {
                //		/** @type {number} */
                //		home_strengthw_=home_strength_+AsNumber(galley_TSneeded_)*3000*AsNumber(Total_Combat_Research_[14]);
                //		if(home_strengthw_>boss_strength_) {
                //			/** @type {number} */
                //			var proportionz_=home_strengthw_/boss_strength_;
                //			for(i_42 in km_2) {
                //				/** @type {number} */
                //				bmz_[i_42]=Math.ceil(AsNumber(km_2[i_42])/proportionz_);
                //				/** @type {number} */
                //				tempgalley_=tempgalley_+bmz_[i_42]*TS_type_[i_42];
                //			}
                //		}
                //		/** @type {number} */
                //		galleytroops_=Math.ceil(tempgalley_/500);
                //	}
                //	/**
                //	 * @return {void}
                //	 */
                //	document.getElementById("raidDungGo").onclick=() => {
                //		setTimeout(() => {
                //			if((kg_[14]||kg_[15]||kg_[16])&&!kg_[5]&&!kg_[6]&&!kg_[8]&&!kg_[9]&&!kg_[10]&&!kg_[11]&&!kg_[2]&&!kg_[3]&&!kg_[4]) {
                //				if(kg_[16]>attackwres_[16]) {
                //					$("#raidIP16").val(attackwres_[16]);
                //				} else {
                //					if(kg_[15]>attackwres_[15]) {
                //						$("#raidIP15").val(attackwres_[15]);
                //					} else {
                //						if(kg_[14]>attackwres_[14]) {
                //							$("#raidIP14").val(attackwres_[14]);
                //						} else {
                //							errorgo_(message_23);
                //						}
                //					}
                //				}
                //			} else {
                //				if(kg_[14]&&(kg_[5]||kg_[6]||kg_[8]||kg_[9]||kg_[10]||kg_[11]||kg_[2]||kg_[3]||kg_[4])) {
                //					if(kg_[14]>galleytroops_&&bmz_.length>0) {
                //						var i_46;
                //						for(i_46 in km_2) {
                //							$(`#raidIP${[i_46]}`).val(bmz_[i_46]);
                //						}
                //						$("#raidIP14").val(galleytroops_);
                //					} else {
                //						if(kg_[14]>attackwres_[14]) {
                //							$("#raidIP14").val(attackwres_[14]);
                //						} else {
                //							errorgo_(message_23);
                //						}
                //					}
                //				} else {
                //					errorgo_(message_23);
                //				}
                //			}
                //		},1500);
                //	};
                //	$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text(attackwres_[14]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text(attackwres_[15]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text(attackwres_[16]);
                //}
                //if(bossname_=="Cyclops") {
                //	/**
                //	 * @return {void}
                //	 */
                //	document.getElementById("raidDungGo").onclick=() => {
                //		setTimeout(() => {
                //			var i_47;
                //			for(i_47 in km_2) {
                //				if((km_2[8]||km_2[9]||km_2[10])&&!km_2[5]&&!km_2[6]&&!km_2[2]&&!km_2[3]&&!km_2[4]&&!km_2[11]) {
                //					$(`#raidIP${[i_47]}`).val(bmw_[i_47]);
                //					if(bmw_.length===0) {
                //						errorgo_(message_23);
                //						break;
                //					}
                //				} else {
                //					$(`#raidIP${[i_47]}`).val(bm_[i_47]);
                //					if(bm_.length===0) {
                //						errorgo_(message_23);
                //						break;
                //					}
                //				}
                //			}
                //		},1500);
                //	};
                //	$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackwres_[8]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackwres_[10]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackwres_[9]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackwres_[7]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
                //}
                //if(bossname_=="Andar's Colosseum Challenge") {
                //	/**
                //	 * @return {void}
                //	 */
                //	document.getElementById("raidDungGo").onclick=() => {
                //		setTimeout(() => {
                //			var i_48;
                //			for(i_48 in km_2) {
                //				if((km_2[8]||km_2[9]||km_2[10])&&!km_2[5]&&!km_2[6]&&!km_2[2]&&!km_2[3]&&!km_2[4]&&!km_2[11]) {
                //					$(`#raidIP${[i_48]}`).val(bmw_[i_48]);
                //					if(bmw_.length===0) {
                //						errorgo_(message_23);
                //						break;
                //					}
                //				} else {
                //					$(`#raidIP${[i_48]}`).val(bm_[i_48]);
                //				}
                //				if(bm_.length===0) {
                //					errorgo_(message_23);
                //					break;
                //				}
                //			}
                //		},1500);
                //	};
                //	$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackwres_[8]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackwres_[10]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackwres_[9]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackwres_[7]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
                //}
                //if(bossname_=="Romulus and Remus") {
                //	/**
                //	 * @return {void}
                //	 */
                //	document.getElementById("raidDungGo").onclick=() => {
                //		setTimeout(() => {
                //			var i_49;
                //			for(i_49 in km_2) {
                //				if((km_2[2]||km_2[3]||km_2[4]||km_2[5])&&!km_2[6]&&!km_2[8]&&!km_2[9]&&!km_2[10]&&!km_2[11]) {
                //					$(`#raidIP${[i_49]}`).val(bmw_[i_49]);
                //					if(bmw_.length===0) {
                //						errorgo_(message_23);
                //						break;
                //					}
                //				} else {
                //					$(`#raidIP${[i_49]}`).val(bm_[i_49]);
                //				}
                //				if(bm_.length===0) {
                //					errorgo_(message_23);
                //					break;
                //				}
                //			}
                //		},1500);
                //	};
                //	$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackwres_[5]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackwres_[2]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackwres_[3]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackwres_[4]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
                //}
                //if(bossname_=="Dragon") {
                //	/**
                //	 * @return {void}
                //	 */
                //	document.getElementById("raidDungGo").onclick=() => {
                //		setTimeout(() => {
                //			var i_50;
                //			for(i_50 in km_2) {
                //				if((km_2[2]||km_2[3]||km_2[4]||km_2[5])&&!km_2[6]&&!km_2[8]&&!km_2[9]&&!km_2[10]&&!km_2[11]) {
                //					$(`#raidIP${[i_50]}`).val(bmw_[i_50]);
                //					if(bmw_.length===0) {
                //						errorgo_(message_23);
                //						break;
                //					}
                //				} else {
                //					$(`#raidIP${[i_50]}`).val(bm_[i_50]);
                //				}
                //				if(bm_.length===0) {
                //					errorgo_(message_23);
                //					break;
                //				}
                //			}
                //		},1500);
                //	};
                //	$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackwres_[5]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackwres_[2]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackwres_[3]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackwres_[4]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
                //}
                //if(bossname_=="Gorgon") {
                //	/**
                //	 * @return {void}
                //	 */
                //	document.getElementById("raidDungGo").onclick=() => {
                //		setTimeout(() => {
                //			var i_51;
                //			for(i_51 in km_2) {
                //				if((km_2[6]||km_2[11])&&!km_2[4]&&!km_2[5]&&!km_2[3]&&!km_2[8]&&!km_2[9]&&!km_2[10]&&!km_2[2]) {
                //					$(`#raidIP${[i_51]}`).val(bmw_[i_51]);
                //					if(bmw_.length===0) {
                //						errorgo_(message_23);
                //						break;
                //					}
                //				} else {
                //					$(`#raidIP${[i_51]}`).val(bm_[i_51]);
                //				}
                //				if(bm_.length===0) {
                //					errorgo_(message_23);
                //					break;
                //				}
                //			}
                //		},1500);
                //	};
                //	$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackwres_[6]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackwres_[11]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
                //}
                //if(bossname_=="Gladiator") {
                //	/**
                //	 * @return {void}
                //	 */
                //	document.getElementById("raidDungGo").onclick=() => {
                //		setTimeout(() => {
                //			var i_52;
                //			for(i_52 in km_2) {
                //				if((km_2[6]||km_2[11])&&!km_2[4]&&!km_2[5]&&!km_2[3]&&!km_2[8]&&!km_2[9]&&!km_2[10]&&!km_2[2]) {
                //					$(`#raidIP${[i_52]}`).val(bmw_[i_52]);
                //					if(bmw_.length===0) {
                //						errorgo_(message_23);
                //						break;
                //					}
                //				} else {
                //					$(`#raidIP${[i_52]}`).val(bm_[i_52]);
                //				}
                //				if(bm_.length===0) {
                //					errorgo_(message_23);
                //					break;
                //				}
                //			}
                //		},1500);
                //	};
                //	$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackwres_[6]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackwres_[11]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
                //	$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
                //	$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
                //}
            }
            if (dtype_ === "city") {
                $("#cityplayerInfo div table tbody tr:gt(6)").remove();
                // var coords = $("#citycoords")[0].innerText.split(":");
                let _cid = AsNumber(clickInfo.x) + 65536 * AsNumber(clickInfo.y);
                var toAdd = Object.assign({}, defaultMru); // clone defaults
                var maxCount = 32;
                /** @type {AsNumber} */
                for (var i = 0; i < mru.length; ++i) {
                    var _i = mru[i];
                    if (_i.cid == _cid) {
                        toAdd = _i;
                        toAdd.last = new Date();
                        mru.splice(AsNumber(i), 1);
                        break;
                    }
                }
                if (mru.length > maxCount) {
                    /** @type {number} */
                    for (var i = mru.length; --i >= 0;) {
                        if (!mru[i].pin) {
                            mru.splice(i, 1);
                            break;
                        }
                    }
                }
                toAdd.player = clickInfo.info.player;
                toAdd.last = new Date();
                toAdd.alliance = clickInfo.info.alliance;
                toAdd.name = clickInfo.info.name;
                toAdd.notes = clickInfo.info.remarks;
                toAdd.cid = _cid;
                mru.push(toAdd);
                mru.sort((a, b) => { return b.last.valueOf() - a.last.valueOf(); });
                console.log(mru);
                localStorage.setItem("mru", JSON.stringify(mru));
                const wrapper = { cityclick: toAdd };
                window['external']['notify'](JSON.stringify(wrapper));
            }
        });
        /** @type {string} */
        var newbutz_ = "<div style='float: left; margin-left: 2%;'><button id='newbuttonu' style='font-size:8px; padding: 4px; border-radius: 8px;' class='greenb shRnTr'>Recall(<90%)</button></div>";
        $("#totalTS").before(newbutz_);
        $("#newbuttonu").click(() => {
            setTimeout(() => {
                recallraidl100_();
            }, 500);
        });
        $("#totalTS").click(() => {
            setTimeout(() => {
                carrycheck_();
            }, 500);
        });
        $("#loccavwarconGo").click(() => {
            setTimeout(() => {
                getDugRows_();
            }, 1000);
        });
        $("#raidmantab").click(() => {
            setTimeout(() => {
                getDugRows_();
            }, 1000);
        });
        $("#allianceIncomings").parent().click(() => {
            setTimeout(() => {
                incomings_();
            }, 4000);
        });
        $("#ui-id-37").click(() => {
            setTimeout(() => {
                incomings_();
            }, 1000);
        });
        if (localStorage.getItem("raidbox") != null) {
            /** @type {string} */
            var raidboxback_ = "<button class='regButton greenb' id='raidboxb' style='width:120px; margin-left: 2%;'>Return Raiding Box</button>";
            $("#squaredung td").find(".squarePlayerInfo").before(raidboxback_);
            $("#raidboxb").click(() => {
                localStorage.removeItem("raidbox");
                $("#raidboxb").remove();
            });
        }
        /** @type {string} */
        var cancelallya_ = "<input id='cancelAllya' type='checkbox' checked='checked'> Cancel attack if same alliance";
        /** @type {string} */
        var cancelallys_ = "<input id='cancelAllys' type='checkbox' checked='checked'> Cancel attack if same alliance";
        /** @type {string} */
        var cancelallyp_ = "<input id='cancelAllyp' type='checkbox' checked='checked'> Cancel attack if same alliance";
        /** @type {string} */
        var cancelallyc_ = "<input id='cancelAllyc' type='checkbox' checked='checked'> Cancel attack if same alliance";
        $("#assaulttraveltime").parent().next().html(cancelallya_);
        $("#siegetraveltime").parent().next().html(cancelallys_);
        $("#plundtraveltime").parent().next().html(cancelallyp_);
        $("#scouttraveltime").parent().next().html(cancelallyc_);
        $("#assaultGo").click(() => {
            if ($("#cancelAllya").prop("checked") == false) {
                setTimeout(() => {
                    $(".shAinf").each(function () {
                        let tid_7 = ToInt($(this).parent().next().find(".cityblink").attr("data"));
                        /** @type {number} */
                        var tx_1 = tid_7 % 65536;
                        /** @type {number} */
                        var ty_1 = (tid_7 - tx_1) / 65536;
                        if (tx_1 == $("#assaultxcoord").val() && ty_1 == $("#assaultycoord").val()) {
                            var aid_ = $(this).attr("data");
                            var dat_7 = {
                                a: aid_,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UaO.php",
                                type: "POST",
                                async: true,
                                data: dat_7
                            });
                        }
                    });
                    $(".shPinf").each(function () {
                        let a = $(this).parent().next().find(".cityblink");
                        let tid_8 = GetIntData(a);
                        /** @type {number} */
                        var tx_2 = tid_8 % 65536;
                        /** @type {number} */
                        var ty_2 = (tid_8 - tx_2) / 65536;
                        if (tx_2 == $("#assaultxcoord").val() && ty_2 == $("#assaultycoord").val()) {
                            var aid_1 = $(this).attr("data");
                            var dat_8 = {
                                a: aid_1,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UpO.php",
                                type: "POST",
                                async: true,
                                data: dat_8
                            });
                        }
                    });
                }, 4000);
            }
        });
        $("#plunderGo").click(() => {
            if ($("#cancelAllyp").prop("checked") == false) {
                setTimeout(() => {
                    $(".shAinf").each(function () {
                        var tid_9 = GetIntData($(this).parent().next().find(".cityblink"));
                        /** @type {number} */
                        var tx_3 = tid_9 % 65536;
                        /** @type {number} */
                        var ty_3 = (tid_9 - tx_3) / 65536;
                        if (tx_3 == $("#pluxcoord").val() && ty_3 == $("#pluycoord").val()) {
                            var aid_2 = $(this).attr("data");
                            var dat_9 = {
                                a: aid_2,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UaO.php",
                                type: "POST",
                                async: true,
                                data: dat_9
                            });
                        }
                    });
                    $(".shPinf").each(function () {
                        var tid_10 = GetIntData($(this).parent().next().find(".cityblink"));
                        /** @type {number} */
                        var tx_4 = tid_10 % 65536;
                        /** @type {number} */
                        var ty_4 = (tid_10 - tx_4) / 65536;
                        if (tx_4 == $("#pluxcoord").val() && ty_4 == $("#pluycoord").val()) {
                            var aid_3 = $(this).attr("data");
                            var dat_10 = {
                                a: aid_3,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UpO.php",
                                type: "POST",
                                async: true,
                                data: dat_10
                            });
                        }
                    });
                }, 4000);
            }
        });
        $("#scoutGo").click(() => {
            if ($("#cancelAllyc").prop("checked") == false) {
                setTimeout(() => {
                    $(".shAinf").each(function () {
                        var tid_11 = GetIntData($(this).parent().next().find(".cityblink"));
                        /** @type {number} */
                        var tx_5 = tid_11 % 65536;
                        /** @type {number} */
                        var ty_5 = (tid_11 - tx_5) / 65536;
                        if (tx_5 == $("#scoxcoord").val() && ty_5 == $("#scoycoord").val()) {
                            var aid_4 = $(this).attr("data");
                            var dat_11 = {
                                a: aid_4,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UaO.php",
                                type: "POST",
                                // async false,
                                data: dat_11
                            });
                        }
                    });
                    $(".shPinf").each(function () {
                        var tid_12 = GetIntData($(this).parent().next().find(".cityblink"));
                        /** @type {number} */
                        var tx_6 = tid_12 % 65536;
                        /** @type {number} */
                        var ty_6 = (tid_12 - tx_6) / 65536;
                        if (tx_6 == $("#scoxcoord").val() && ty_6 == $("#scoycoord").val()) {
                            var aid_5 = $(this).attr("data");
                            var dat_12 = {
                                a: aid_5,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UpO.php",
                                type: "POST",
                                // async false,
                                data: dat_12
                            });
                        }
                    });
                }, 4000);
            }
        });
        $("#siegeGo").click(() => {
            if ($("#cancelAllys").prop("checked") == false) {
                setTimeout(() => {
                    $(".shAinf").each(function () {
                        var tid_13 = GetIntData($(this).parent().next().find(".cityblink"));
                        /** @type {number} */
                        var tx_7 = tid_13 % 65536;
                        /** @type {number} */
                        var ty_7 = (tid_13 - tx_7) / 65536;
                        if (tx_7 == $("#siexcoord").val() && ty_7 == $("#sieycoord").val()) {
                            var aid_6 = $(this).attr("data");
                            var dat_13 = {
                                a: aid_6,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UaO.php",
                                type: "POST",
                                // async false,
                                data: dat_13
                            });
                        }
                    });
                    $(".shPinf").each(function () {
                        let cid = GetCidData($(this).parent().next().find(".cityblink"));
                        /** @type {number} */
                        let tx_8 = cid.x;
                        /** @type {number} */
                        let ty_8 = cid.y;
                        if (tx_8 == $("#siexcoord").val() && ty_8 == $("#sieycoord").val()) {
                            var aid_7 = $(this).attr("data");
                            var dat_14 = {
                                a: aid_7,
                                b: 1
                            };
                            jQuery.ajax({
                                url: "includes/UpO.php",
                                type: "POST",
                                // async false,
                                data: dat_14
                            });
                        }
                    });
                }, 4000);
            }
        });
        $("#citynotes").draggable({
            handle: ".popUpBar",
            containment: "window",
            scroll: false
        });
        $("#citynotes").height("310px");
        $("#citynotes").width("495px");
        /** @type {string} */
        var layoutopttab_ = "<li id='layoutopt' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='layoutoptBody'";
        /** @type {string} */
        layoutopttab_ = `${layoutopttab_}aria-labeledby='ui-id-60' aria-selected='false' aria-expanded='false'>`;
        /** @type {string} */
        layoutopttab_ = `${layoutopttab_}<a href='#layoutoptBody' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-60'>Layout Options</a></li>`;
        /** @type {string} */
        var layoutoptbody_ = "<div id='layoutoptBody' aria-labeledby='ui-id-60' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_} role='tabpanel' aria-hidden='true' style='display: none;'><table><tbody><tr><td><input id='addnotes' class='clsubopti' type='checkbox'> Add Notes</td>`;
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_}<td><input id='addtroops' class='clsubopti' type='checkbox'> Add Troops</td></tr><tr><td><input id='addtowers' class='clsubopti' type='checkbox'> Add Towers</td><td><input id='addbuildings' class='clsubopti' type='checkbox'> Upgrade Cabins</td>`;
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_}<td> Cabin Lvl: <input id='cablev' type='number' style='width:22px;' value='7'></td></tr><tr><td><input id='addwalls' class='clsubopti' type='checkbox'> Add Walls</td>`;
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_}<td><input id='addhub' class='clsubopti' type='checkbox'> Set Nearest Hub With layout</td></tr><tr><td>Select Hubs list: </td><td id='selhublist'></td><td>`;
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_}<button id='nearhubAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Set Nearest Hub</button><button id='infantryAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Infantry setup</button></td></tr></tbody></table>`;
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_}<table><tbody><tr><td colspan='2'><input id='addres' class='clsubopti' type='checkbox'> Add Resources:</td><td id='buttd' colspan='2'></td></tr><tr><td>wood<input id='woodin' type='number' style='width:100px;' value='200000'></td><td>stone<input id='stonein' type='number' style='width:100px;' value='220000'></td>`;
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_}<td>iron<input id='ironin' type='number' style='width:100px;' value='200000'></td><td>food<input id='foodin' type='number' style='width:100px;' value='350000'></td></tr>`;
        /** @type {string} */
        layoutoptbody_ = `${layoutoptbody_}</tbody></table></div>`;
        /** @type {string} */
        var layoptbut_ = "<button id='layoptBut' class='regButton greenb' style='width:150px;'>Save Res Settings</button>";
        var tabs_1 = $("#CNtabs").tabs();
        var ul_1 = tabs_1.find("ul");
        $(layoutopttab_).appendTo(ul_1);
        tabs_1.tabs("refresh");
        $("#CNtabs").append(layoutoptbody_);
        $("#buttd").append(layoptbut_);
        $("#nearhubAp").click(() => {
            PostMMNIO(null);
        });
        $("#infantryAp").click(() => {
            setinfantry_();
        });
        $("#layoptBut").click(() => {
            localStorage.setItem("woodin", $('$woodin').val().toString());
            localStorage.setItem("foodin", $("#foodin").val().toString());
            localStorage.setItem("ironin", $("#ironin").val().toString());
            localStorage.setItem("stonein", $("#stonein").val().toString());
            localStorage.setItem("cablev", $("#cablev").val().toString());
        });
        if (localStorage.getItem("cablev")) {
            $("#cablev").val(LocalStoreAsInt("cablev"));
        }
        if (localStorage.getItem("woodin")) {
            $("#woodin").val(localStorage.getItem("woodin"));
        }
        if (localStorage.getItem("stonein")) {
            $("#stonein").val(localStorage.getItem("stonein"));
        }
        if (localStorage.getItem("ironin")) {
            $("#ironin").val(localStorage.getItem("ironin"));
        }
        if (localStorage.getItem("foodin")) {
            $("#foodin").val(localStorage.getItem("foodin"));
        }
        InitCheckbox('addres');
        InitCheckbox("addbuildings");
        InitCheckbox("addnotes");
        InitCheckbox('addwalls');
        InitCheckbox("addtowers");
        InitCheckbox("addhub");
        InitCheckbox("addtroops");
        $("#editspncn").click(() => {
            $("#selHub").remove();
            var selhub_ = $("#organiser").clone(false).attr({
                id: "selHub",
                style: "width:100%;height:28px;font-size:11;border-radius:6px;margin:7px"
            });
            $("#selhublist").append(selhub_);
            if (localStorage.getItem("hublist")) {
                $("#selHub").val(localStorage.getItem("hublist")).change();
            }
            $("#selHub").change(() => {
                localStorage.setItem("hublist", $("#selHub").val().toString());
            });
            $("#dfunkylayout").remove();
            $("#funkylayoutl").remove();
            $("#funkylayoutw").remove();
            setTimeout(() => {
                var currentlayout_ = $("#currentLOtextarea").text();
                /** @type {number} */
                var i_53 = 20;
                for (; i_53 < currentlayout_.length - 20; i_53++) {
                    var tmpchar_ = currentlayout_.charAt(i_53);
                    /** @type {!RegExp} */
                    var cmp_ = new RegExp(tmpchar_);
                    if (!cmp_.test(emptyspots_)) {
                        currentlayout_ = ReplaceAt(currentlayout_, i_53, "-");
                    }
                }
                /** @type {!Array} */
                var prefered_data_ = [{
                        name: "Guz 7s Prae 122k",
                        string: "[ShareString.1.3]:########################-------#-------#####BBBB----#--------###BZZZB----#---------##BBBBB----#---------##BZZZZ-#######------##BBBBB##BBBBB##-----##----##BZZZZZB##----##----#BBBBBBBBB#----##----#BZZZZZZZB#----#######BBBBTBBBB#######P--X#BZZZZZZZB#----##-SSJ#BBBBBBBBB#----##P---##BZZZZZB##----##P----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################",
                        remarks: "Landlocked Praetors",
                        notes: "122000 Praetors",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 4s Arbs 132k",
                        string: "[ShareString.1.3]:########################-------#-------#####BBBB----#--------###BEEEB----#---------##BBBBB----#---------##BEBEB-#######------##BBBBB##BBBBB##-----##----##BEEBEEB##----##----#BBBBBBBBB#----##----#BEEEBEEEB#----#######BBBBTBBBB#######----#EEEEBEEEB#----##----#BBBBBBBBB#----##----##BEEBEEB##----##-----##BBBBB##-----##------#######------##---------#J--------##-----SS--#X--------###----LM--#--------#####--PP---#-------########################",
                        remarks: "Arbs",
                        notes: "132000 Arbs",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 3s Rng 280k",
                        string: "[ShareString.1.3];########################-------#-------#####BBBB----#--------###BGBGB----#---------##BBBBB----#---------##BGBGB-#######------##BBBBB##BBBBB##-----##----##BGGBGGB##----##----#BBBBBBBBB#----##----#BGGBGBGGB#----#######BBBBTBBBB#######----#BGGBGBGGB#----##----#BBBBBBBBB#----##----##BGGBGGB##----##-----##BBBBB##-----##------#######--__--##---------#J---_##_-##-----SS--#X---_###_###----LM--#-----_#######--PP---#------_########################",
                        remarks: "Ranger",
                        notes: "280000 Ranger",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 3s Rng/Tri 256k",
                        string: "[ShareString.1.3]:########################BBBBB--#--,-,--#####-BGBGB-,#------,-###,-BGBBB--#-,-..-,--##--BGBGB-.#,-------.##--BBBB#######:-.---##----:##BBBBB##-.-,-##.-;-##GBGBGBG##----##----#BBBGBGBBB#--:-##...-#BGBGBGBGB#-::-#######BBBGTGBGB#######.SS.#BGBGBGBGB#---:##P--X#BBBGBGBBB#----##:-:J##GBGBGBG##--;-##P:---##BBBBB##,----##:--.--#######---,--##P-.--.-:-#--------,##P----.---#.--:-,-,-###,-,-.---#--------#####,----:-#.--;---########################",
                        remarks: "R/T",
                        notes: "128K Rng 128K Tri",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 3s R/T Ship 240K",
                        string: "[ShareString.1.3];########################-------#---BBBB#####--------#---BGBGB###---------#---BGBGB-##---------#---BBBBB-##------#######BGBGB-##-----##BBBBB##GBGB-##----##BGBGBGB##BB--##----#-BGBGBGB-#----##----#-BGBGBGB-#----#######-BGBTBGB-#######----#-BGBGBGB-#----##----#-BGBGBGB-#----##----##BGBGBGB##----##-----##BBBBB##-----##------#######--RR--##---------#SS--R##R-##---------#J---R###R###--------#X----R#######-------#------R########################",
                        remarks: "R/T Ship",
                        notes: "120K Rng 120K Tri",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 7s Arb Ship 124K",
                        string: "[ShareString.1.3];########################-------#-------#####BBB-----#--------###BEEE-----#---------##BBBBB----#---------##BEEE--#######------##BBBB-##BBBBB##-----##----##BEEBEEB##----##----#BBBBBBBBB#----##----#BEEEBEEEB#----#######BBBBTBBBB#######----#BEEEBEEEB#----##-SSX#BBBBBBBBB#----##---J##BEEBEEB##----##-----##BBBBB##-----##------#######--RR--##---------#----R##R-##---------#----R###R###--------#-----R#######-------#------R########################",
                        remarks: "Arb Ship",
                        notes: "124K Arb",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 7s Prae Ship 112K",
                        string: "[ShareString.1.3];########################-------#-------#####BBB-----#--------###ZZZZ-----#---------##BBBBB----#---------##ZZZZ--#######------##BBBB-##BBBBB##-----##----##BZZZZZB##----##----#BBBBBBBBB#----##----#BZZZZZZZB#----#######BBBBTBBBB#######----#BZZZZZZZB#----##-SSX#BBBBBBBBB#----##---J##BZZZZZB##----##-----##BBBBB##-----##------#######--RR--##---------#----R##R-##---------#----R###R###--------#-----R#######-------#------R########################",
                        remarks: "Prae Ship",
                        notes: "112K Arb",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 3s Rng Ship 260K",
                        string: "[ShareString.1.3];########################-------#-------#####BBB-----#--------###BGBGB----#---------##BBBBB----#---------##BGGG--#######------##-BBB-##BBBBB##-----##----##BGGBGGB##----##----#BBBBBBBBB#----##----#BGGBGBGGB#----#######BBBBTBBBB#######----#BGGBGBGGB#----##----#BBBBBBBBB#----##-SSX##BGGBGGB##----##-----##BBBBB##-----##---J--#######--RR--##---------#----R##R-##---------#----R###R###--------#-----R#######-------#------R########################",
                        remarks: "Rng Ship",
                        notes: "260K Arb",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 3s Vanq 300K",
                        string: "[ShareString.1.3]:########################-------#-------#####--------#BBBBBBB-###---------#BGBGBGB--##---------#BBBBBBB--##------#######-BGBB-##-----##BBBBB##BBB--##----##-BGBGBZ##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BGBBTBBBB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##-BGBGB-##----##-----##BBBBB##-----##------#######------##---------#-X-------##---------#JP-------###--------#SM------#####-------#SM-----########################",
                        remarks: "S - Vanq",
                        notes: "300K Vanq Senator Capable",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Guz 10s Druid 106K",
                        string: "[ShareString.1.3];########################BB-----#-------#####-JJ-----#--------###BBBBB----#---------##JJJJJ----#---------##BBBBBB#######------##JJJJJ##BBBBB##-----##BBBB##JJJJJJJ##----##----#BBBBBBBBB#----##----#JJJJJJJJJ#----#######BBBBTBBBB#######----#JJJJJJJJJ#----##----#BBBBBBBBB#----##----##JJJJJJJ##----##-----##BBBBB##-----##------#######--__--##--------M#X---_##_-##--------S#----_###_###--------#-----_#######-------#------_########################",
                        remarks: "Druid",
                        notes: "106K Druid",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }, {
                        name: "Tas 4sec Priests",
                        string: "[ShareString.1.3];########################-------#-----BB#####--------#----BBBB###---------#----BZZZB##---------#----BBBBB##------#######-BZZZB##-----##BZBZB##BBBBB##----##ZBZBZBZ##----##----#BZBZBZBZB#SP--##----#BZBZBZBZB#SP--#######BZBZTZBZB#######----#BZBZBZBZB#JX--##----#BZBZBZBZB#----##----##ZBZBZBZ##----##-----##BZBZB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################",
                        remarks: "Priests",
                        notes: "224000 Priests",
                        troop_count: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000]
                    }];
                /** @type {string} */
                var selectbuttsdf_ = '<select id="dfunkylayout" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:30%;" class="regButton greenb"><option value="0">Prefered build layout</option>';
                /** @type {number} */
                var ww_ = 1;
                var prefered_;
                for (prefered_ in prefered_data_) {
                    console.log(prefered_data_[prefered_]);
                    /** @type {string} */
                    selectbuttsdf_ = `${selectbuttsdf_}<option value="${ww_}">${prefered_data_[prefered_].name}</option>`;
                    layoutdf_.push(prefered_data_[prefered_].string);
                    remarkdf_.push(prefered_data_[prefered_].remarks);
                    notedf_.push(prefered_data_[prefered_].notes);
                    troopcound_.push(prefered_data_[prefered_].troop_count);
                    resd_.push(prefered_data_[prefered_].res_count);
                    ww_++;
                }
                /** @type {string} */
                selectbuttsdf_ = `${selectbuttsdf_}</select>`;
                /** @type {string} */
                var selectbuttsw_ = '<select id="funkylayoutw" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Select water layout</option>';
                /** @type {number} */
                var cww_ = 1;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">2 sec rang/galley</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##GBGBGBG##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##GBGBGBG##G---##-----##BGGGB##BBBBG##------#######BBVVBB##---------#--GBV##VB##---------#--GBV###V###--------#---BBV#######-------#----BBV########################");
                remarksw_.push("rangers/triari/galley");
                notesw_.push("166600 inf and 334 galley @ 10 days");
                troopcounw_.push([0, 0, 83300, 83300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 334, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">6 sec arbs/galley</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SPJX##----#BEBEBEBEB#MH--##----##EBEBEBE##----##-----##BEBEB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#--BBBVTT#####-------#--BEBBV########################");
                remarksw_.push("arbs/galley");
                notesw_.push("88300 inf and 354 galley @ 11.5 days");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 88300, 0, 0, 0, 0, 0, 354, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">3 sec priestess/galley</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#---H#######BZBZTZBZB#######----#BZBZBZBZB#JSPX##----#BZBZBZBZB#----##----##ZBZBZBZ##-Z--##-----##BZZZB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#--ZBV###V###--------#---BBV#######-------#---ZBBV########################");
                remarksw_.push("priestess/galley");
                notesw_.push("166600 inf and 334 galley @ 11 days");
                troopcounw_.push([0, 0, 0, 0, 166600, 0, 0, 0, 0, 0, 0, 0, 0, 0, 334, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">7 sec praetor/galley</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SPJX##----#BZBZBZBZB#MH--##----##ZBZBZBZ##----##-----##BZBZB##BBBBZ##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#--BZBBV########################");
                remarksw_.push("praetors/galley");
                notesw_.push("86650 praetors and 347 galley @ 12 days");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 86650, 0, 0, 0, 0, 347, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">2 sec vanq/galley+senator</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##BBGBGBB##---B##-----##BGBGB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#--BBBBV########################");
                remarksw_.push("vanq/galley+senator");
                notesw_.push("193300 inf and 387 galley @ 10 days");
                troopcounw_.push([0, 0, 0, 0, 0, 193300, 0, 0, 0, 0, 0, 0, 0, 0, 387, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">5 sec horses/galley</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#---H#######BEBETEBEB#######----#BEBEBEBEB#JSPX##----#BEBEBEBEB#-M--##----##EBEBEBB##----##-----##BEBEB##BBBB-##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#--BBBV#######-------#--BEBBV########################");
                remarksw_.push("horses/galley");
                notesw_.push("90000 cav and 360 galley @ 10.5 days");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90000, 0, 0, 0, 360, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">5 sec sorc/galley</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##JBJBJ##-----##----##BJBJBJB##----##----#JBJBJBJBJ#----##----#JBJBJBJBJ#---H#######JBJBTBJBJ#######----#JBJBJBJBJ#-S-X##----#JBJBJBJBJ#----##----##BJBJBJB##JJ--##-----##JBJBJ##BBBBJ##------#######BBVVBB##---------#--JBV##VB##---------#--JBV###V###--------#---BBV#######-------#---JBBV########################");
                remarksw_.push("sorc/galley");
                notesw_.push("156600 sorc and 314 galley @ 13.5 days");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 156600, 0, 0, 0, 0, 0, 0, 0, 314, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">vanqs+ports+senator</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######----#BGBBBBBGB#PPJX##----#BGBGBGBGB#BBBB##----##BBGBGBB##BBBB##-----##BBBBB##BBBBB##------#######-BRRBB##---------#----R##RZ##---------#----R###R###--------#----SR#######-------#----MSR########################");
                remarksw_.push("vanqs+senator+ports");
                notesw_.push("264k infantry @ 10 days");
                troopcounw_.push([0, 0, 0, 100000, 0, 164000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">main hub</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#---PPPPP###---------#---PPPPPP##---------#---PPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##SLSDSAS##PPPP##----#-SDSMSDS-#PPPP##----#-SLSMSAS-#PPPP#######-SDSTSDS-#######----#-SLSMSAS-#----##----#-SDSMSDS-#----##----##SLSDSAS##----##-----##-----##-----##------#######--RR--##---------#ZB--RTTR-##---------#PJ--RTTTR###--------#-----RTT#####-------#------R########################");
                remarksw_.push("main hub");
                notesw_.push("14 mil w/s 23 mil iron 15 mil food 8200 carts 240 boats");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">palace storage</option>`;
                layoutsw_.push("[ShareString.1.3]:########################-------#-----PP#####--------#-----PPP###---------#-----PPPP##---------#-----PPPP##------#######--PPPP##-----##SASLS##-PPPP##----##ASASLSL##PPPP##----#SASASLSLS#-PPP##----#SASASLSLS#JPPP#######SASA#LSLS#######----#SASASLSLS#----##----#SASASLSLS#----##----##ASASLSL##----##-----##SASLS##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksw_.push("palace storage");
                notesw_.push("40 mil w/s 6200 carts");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">palace feeder</option>`;
                layoutsw_.push("[ShareString.1.3];########################-PPPPPP#PPPPPPP#####--PPPPPP#PPPPPPPP###---PPPPPP#PPPPPPPPP##---PPPPPP#PPPPPPPPP##----PP#######PPPPPP##-----##----J##PPPPP##----##-A-----##PPPP##----#-SSS-----#PPPP##----#-AAA-----#PPPP#######-SSST----#######----#-LLL-----#----##----#-SSS-----#----##----##-L-----##----##-----##-----##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
                remarksw_.push("palace feeder");
                notesw_.push("8.75 mil w/s 16400 carts");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">palace Hub mixed</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#PPPPPPP#####--------#PPPPPPPP###---------#PPPPPPPPP##---------#PPPPPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##-------##PPPP##----#SLSASLSAS#PPPP##----#SASLSASLS#JPPP#######SLSATLSAS#######----#SASLSASLS#----##----#SLSASLSAS#----##----##-------##----##-----##-----##-----##------#######--__--##---------#----_TT_-##---------#----_TTT_###--------#-----_TT#####-------#------_########################");
                remarksw_.push("palace Hub mixed");
                notesw_.push("24.57 mil w/s 11000 carts");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">Stingers</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######----T----#######----#---------#SPHX##----#---------#-M--##----##-------##----##-----##-----##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
                remarksw_.push("stingers");
                notesw_.push("3480 stingers @ 84 days");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3480, 0]);
                resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
                cww_++;
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">Warships</option>`;
                layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######----T----#######----#---------#SPHX##----#---------#-M--##----##-------##----##-----##-----##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
                remarksw_.push("warships");
                notesw_.push("870 warships @ 42 days");
                troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 870]);
                resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
                /** @type {string} */
                selectbuttsw_ = `${selectbuttsw_}</select>`;
                /** @type {string} */
                var selectbuttsl_ = '<select id="funkylayoutl" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Select land layout</option>';
                /** @type {number} */
                var ll_1 = 1;
                /** @type {!Array} */
                var land_locked_data_ = [{
                        name: "1 sec vanqs",
                        string: "[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##GBGBG##-----##----##BGBGBGB##----##----#GBGBGBGBG#----##----#GBGBGBGBG#----#######GBGBTBGBG#######----#GBGBGBGBG#----##----#GBGBGBGBG#----##----##BGBGBGB##----##GGGGG##GBGBG##-----##BBBBB-#######------##GGGGGG--H#---------##BBBBBB--J#---------###GGGG---X#--------#####BB----S#-------########################",
                        remarks: "vanqs",
                        notes: "180000 vanqs @ 2 days",
                        troop_count: [0, 0, 0, 0, 0, 180000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]
                    }, {
                        name: "2 sec vanqs",
                        string: "[ShareString.1.3]:########################BBB--JX#-------#####BGBG--PP#--------###-BBBBB-MS#---------##-BGBGB--H#---------##-BGBGB#######------##-ZBB-##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBBBBBGB#----#######BGBGTGBGB#######----#BGBBBBBGB#----##----#BGBGBGBGB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################",
                        remarks: "vanqs",
                        notes: "264000 vanqs @ 6 days",
                        troop_count: [0, 0, 0, 0, 0, 264000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                        res_count: [0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]
                    }];
                var l_locked_;
                for (l_locked_ in land_locked_data_) {
                    /** @type {string} */
                    selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">${land_locked_data_[l_locked_].name}</option>`;
                    layoutsl_.push(land_locked_data_[l_locked_].string);
                    remarksl_.push(land_locked_data_[l_locked_].remarks);
                    notesl_.push(land_locked_data_[l_locked_].notes);
                    troopcounl_.push(land_locked_data_[l_locked_].troop_count);
                    resl_.push(land_locked_data_[l_locked_].res_count);
                    ll_1++;
                }
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">3 sec vanqs raiding</option>`;
                layoutsl_.push("[ShareString.1.3];########################----PJX#-------#####BB----PP#--------###BGBGB--SS#---------##BBBBB--MP#---------##BGBGB-#######------##BBBBB##BBBBB##-----##--G-##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBBB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
                remarksl_.push("vanqs");
                notesl_.push("296000 vanqs @ 10 days");
                troopcounl_.push([0, 0, 0, 0, 0, 296000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">2 sec rangers</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BGBGB-PP#--------###-BGBGB-MS#---------##-BGBGB--H#---------##-BGBGB#######------##--BBB##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######----#BGBGBGBGB#----##----#BGBGBGBGB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("rangers/triari");
                notesl_.push("236000 inf @ 6.5 days");
                troopcounl_.push([0, 0, 186000, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">3 sec priests</option>`;
                layoutsl_.push("[ShareString.1.3];########################-------#-----BB#####--------#----BBBB###---------#----BZZZB##---------#----BBBBB##------#######-BZZZB##-----##BZBZB##BBBBB##----##ZBZBZBZ##----##----#BZBZBZBZB#SP--##----#BZBZBZBZB#SP--#######BZBZTZBZB#######----#BZBZBZBZB#JX--##----#BZBZBZBZB#----##----##ZBZBZBZ##----##-----##BZBZB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
                remarksl_.push("priests");
                notesl_.push("224000 inf @ 7.7 days");
                troopcounl_.push([0, 0, 224000, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">6 sec praetors</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BZBZB-PP#--------###-BZBZB-MS#---------##-BZBZB--H#---------##-BZBZB#######------##--BBB##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#----##----#BZBZBZBZB#----##----##BBZBZBB##----##-----##BZBZB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("praetors");
                notesl_.push("110000 praetors @ 7.5 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 110000, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">4 sec horses</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--ZBB##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBE##----##-----##BEBEB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("horses");
                notesl_.push("106000 horses @ 5 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 106000, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">5 sec horses</option>`;
                layoutsl_.push("[ShareString.1.3]:########################-B---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB-PH#---------##-BEBEB#######------##--BBB##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBBTBBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("horses");
                notesl_.push("124000 horses @ 7 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 124000, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">5 sec arbs</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--BBB##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BEBEB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("arbs");
                notesl_.push("110000 arbs @ 6.5 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 110000, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">6 sec arbs</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BBBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--BBB##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("arbs");
                notesl_.push("124000 arbs @ 8.5 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 124000, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">4 sec sorc</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BJBJ--X#-------#####JBJBJ--S#--------###-JBJBJ--M#---------##-JBJBJ--H#---------##-JBJBJ#######------##-ZBJB##JBJBJ##-----##----##BJBJBJB##----##----#JBJBJBJBJ#----##----#JBJBJBJBJ#----#######JBJBTBJBJ#######----#JBJBJBJBJ#----##----#JBJBJBJBJ#----##----##BJBJBJB##----##-----##JBJBJ##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("sorc");
                notesl_.push("176000 sorc @ 8 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 176000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">5 sec sorc</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BBB---X#-------#####BJBJB--P#--------###-BJBJB-MS#---------##-BJBJB--H#---------##-BJBJB#######------##-ZBBB##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#----##----#BJBJBJBJB#----##----##BBJBJBB##----##-----##BJBJB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("sorc");
                notesl_.push("224000 sorc @ 13 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 224000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">10 sec druids</option>`;
                layoutsl_.push("[ShareString.1.3]:########################-J----X#-------#####JBJB--MS#--------###BJBJB---H#---------##BJBJB----#---------##BJBJB-#######------##BJBJB##BJBJB##-----##-JBJ##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#----##----#BJBJBJBJB#----##----##JBJBJBJ##----##-----##BJBJB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("druids");
                notesl_.push("102000 druids @ 12 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 102000, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">scorp/rams</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BBYB--X#-------#####BYBYB---#--------###-BYBYB-MS#---------##-BYBYB--H#---------##-BYBYB#######------##-BYBB##BYBYB##-----##----##YBYBYBY##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#----##----#BYBYBYBYB#----##----##YBYBYBY##----##-----##BYBYB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("scorp/rams");
                notesl_.push("21600 siege engines @ 7.5 days");
                troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5500, 16100, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                ll_1++;
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">ballista</option>`;
                layoutsl_.push("[ShareString.1.3]:########################BBBB--X#-------#####BYBYB---#--------###-BYBYB-MS#---------##-BYBYB--H#---------##-BYBYB#######------##-BBBB##BBBBB##-----##----##BBYBYBB##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#----##----#BYBYBYBYB#----##----##BBYBYBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarksl_.push("ballista");
                notesl_.push("25600 siege engines @ 10.5 days");
                troopcounl_.push([0, 25600, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
                resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
                /** @type {string} */
                selectbuttsl_ = `${selectbuttsl_}</select>`;
                $("#removeoverlayGo").after(selectbuttsdf_);
                $("#dfunkylayout").after(selectbuttsl_);
                $("#funkylayoutl").after(selectbuttsw_);
                $("#funkylayoutl").change(() => {
                    var newlayout_ = currentlayout_;
                    /** @type {number} */
                    var j_12 = 1;
                    for (; j_12 < layoutsl_.length; j_12++) {
                        if ($("#funkylayoutl").val() == j_12) {
                            /** @type {number} */
                            var i_54 = 20;
                            for (; i_54 < currentlayout_.length; i_54++) {
                                var tmpchar_1 = layoutsl_[j_12].charAt(i_54);
                                /** @type {!RegExp} */
                                var cmp_1 = new RegExp(tmpchar_1);
                                if (!cmp_1.test(emptyspots_)) {
                                    newlayout_ = ReplaceAt(newlayout_, i_54, tmpchar_1);
                                }
                            }
                            $("#overlaytextarea").val(newlayout_);
                            setTimeout(() => {
                                jQuery("#applyoverlayGo")[0].click();
                            }, 300);
                            PostMMNIO(j_12);
                        }
                    }
                });
                $("#funkylayoutw").change(() => {
                    var newlayout_1 = currentlayout_;
                    /** @type {number} */
                    var j_13 = 1;
                    for (; j_13 < layoutsw_.length; j_13++) {
                        if ($("#funkylayoutw").val() == j_13) {
                            /** @type {number} */
                            for (let i_55 = 20; i_55 < currentlayout_.length; i_55++) {
                                var tmpchar_2 = layoutsw_[j_13].charAt(i_55);
                                /** @type {!RegExp} */
                                var cmp_2 = new RegExp(tmpchar_2);
                                if (!cmp_2.test(emptyspots_)) {
                                    newlayout_1 = ReplaceAt(newlayout_1, i_55, tmpchar_2);
                                }
                            }
                            $("#overlaytextarea").val(newlayout_1);
                            setTimeout(() => {
                                jQuery("#applyoverlayGo")[0].click();
                            }, 300);
                            if ($("#addnotes").prop("checked") == true) {
                                $("#CNremarks").val(remarksw_[j_13]);
                                $("#citynotestextarea").val(notesw_[j_13]);
                                setTimeout(() => {
                                    jQuery("#citnotesaveb")[0].click();
                                }, 100);
                            }
                            var aa_8 = cdata_.mo;
                            if ($("#addtroops").prop("checked") == true) {
                                var k_4;
                                for (k_4 in troopcounw_[j_13]) {
                                    aa_8[9 + AsNumber(k_4)] = troopcounw_[j_13][k_4];
                                }
                            }
                            if ($("#addwalls").prop("checked") == true) {
                                /** @type {number} */
                                aa_8[26] = 1;
                            }
                            if ($("#addtowers").prop("checked") == true) {
                                /** @type {number} */
                                aa_8[27] = 1;
                            }
                            if ($("#addhub").prop("checked") == true) {
                                var hubs_3 = {
                                    cid: [],
                                    distance: []
                                };
                                $.each(ppdt.clc, (key_57, value_105) => {
                                    if (key_57 == $("#selHub").val()) {
                                        /** @type {number} */
                                        hubs_3.cid = value_105;
                                    }
                                });
                                for (let i_55 in hubs_3.cid) {
                                    /** @type {number} */
                                    var tempx_12 = AsNumber(hubs_3.cid[i_55] % 65536);
                                    /** @type {number} */
                                    var tempy_12 = AsNumber((hubs_3.cid[i_55] - tempx_12) / 65536);
                                    hubs_3.distance.push(Math.sqrt((tempx_12 - cdata_.x) * (tempx_12 - cdata_.x) + (tempy_12 - cdata_.y) * (tempy_12 - cdata_.y)));
                                }
                                /** @type {number} */
                                var mindist_3 = Math.min(...hubs_3.distance);
                                var nearesthub_3 = hubs_3.cid[hubs_3.distance.indexOf(mindist_3)];
                                resw_[j_13][14] = nearesthub_3;
                                resw_[j_13][15] = nearesthub_3;
                            }
                            else {
                                /** @type {number} */
                                resw_[j_13][14] = 0;
                                /** @type {number} */
                                resw_[j_13][15] = 0;
                            }
                            if ($("#addres").prop("checked") == true) {
                                resw_[j_13][5] = $("#woodin").val();
                                resw_[j_13][6] = $("#stonein").val();
                                resw_[j_13][7] = $("#ironin").val();
                                resw_[j_13][8] = $("#foodin").val();
                                resw_[j_13][19] = $("#woodin").val();
                                resw_[j_13][20] = $("#stonein").val();
                                resw_[j_13][21] = $("#ironin").val();
                                resw_[j_13][22] = $("#foodin").val();
                                for (k_4 in resw_[j_13]) {
                                    aa_8[28 + AsNumber(k_4)] = resw_[j_13][k_4];
                                }
                            }
                            if ($("#addbuildings").prop("checked") == true) {
                                /** @type {!Array} */
                                aa_8[51] = [1, GetFloatValue($("#cablev"))];
                                /** @type {number} */
                                aa_8[1] = 1;
                            }
                            var dat_16 = {
                                a: JSON.stringify(aa_8),
                                b: cotg.city.id()
                            };
                            jQuery.ajax({
                                url: "includes/mnio.php",
                                type: "POST",
                                // async false,
                                data: dat_16
                            });
                        }
                    }
                });
                $("#dfunkylayout").change(() => {
                    var newlayout_2 = currentlayout_;
                    /** @type {number} */
                    var j_14 = 1;
                    for (; j_14 < layoutdf_.length; j_14++) {
                        if ($("#dfunkylayout").val() == j_14) {
                            /** @type {number} */
                            for (let i_56 = 20; i_56 < currentlayout_.length; i_56++) {
                                var tmpchar_3 = layoutdf_[j_14].charAt(i_56);
                                /** @type {!RegExp} */
                                var cmp_3 = new RegExp(tmpchar_3);
                                if (!cmp_3.test(emptyspots_)) {
                                    newlayout_2 = ReplaceAt(newlayout_2, i_56, tmpchar_3);
                                }
                            }
                            $("#overlaytextarea").val(newlayout_2);
                            setTimeout(() => {
                                jQuery("#applyoverlayGo")[0].click();
                            }, 300);
                            if ($("#addnotes").prop("checked") == true) {
                                $("#CNremarks").val(remarkdf_[j_14]);
                                $("#citynotestextarea").val(notedf_[j_14]);
                                setTimeout(() => {
                                    jQuery("#citnotesaveb")[0].click();
                                }, 100);
                            }
                            var aa_9 = cdata_.mo;
                            if ($("#addtroops").prop("checked") == true) {
                                var k_5;
                                for (k_5 in troopcound_[j_14]) {
                                    aa_9[9 + AsNumber(k_5)] = troopcound_[j_14][k_5];
                                }
                            }
                            if ($("#addwalls").prop("checked") == true) {
                                /** @type {number} */
                                aa_9[26] = 1;
                            }
                            if ($("#addtowers").prop("checked") == true) {
                                /** @type {number} */
                                aa_9[27] = 1;
                            }
                            if ($("#addhub").prop("checked") == true) {
                                var hubs_4 = {
                                    cid: [],
                                    distance: []
                                };
                                $.each(ppdt.clc, (key_58, value_106) => {
                                    if (key_58 == $("#selHub").val()) {
                                        /** @type {number} */
                                        hubs_4.cid = value_106;
                                    }
                                });
                                for (let i_56 in hubs_4.cid) {
                                    /** @type {number} */
                                    var tempx_13 = AsNumber(hubs_4.cid[i_56] % 65536);
                                    /** @type {number} */
                                    var tempy_13 = AsNumber((hubs_4.cid[i_56] - tempx_13) / 65536);
                                    hubs_4.distance.push(Math.sqrt((tempx_13 - cdata_.x) * (tempx_13 - cdata_.x) + (tempy_13 - cdata_.y) * (tempy_13 - cdata_.y)));
                                }
                                /** @type {number} */
                                var mindist_4 = Math.min(...hubs_4.distance);
                                var nearesthub_4 = hubs_4.cid[hubs_4.distance.indexOf(mindist_4)];
                                resd_[j_14][14] = nearesthub_4;
                                resd_[j_14][15] = nearesthub_4;
                            }
                            else {
                                /** @type {number} */
                                resd_[j_14][14] = 0;
                                /** @type {number} */
                                resd_[j_14][15] = 0;
                            }
                            if ($("#addres").prop("checked") == true) {
                                resd_[j_14][5] = $("#woodin").val();
                                resd_[j_14][6] = $("#stonein").val();
                                resd_[j_14][7] = $("#ironin").val();
                                resd_[j_14][8] = $("#foodin").val();
                                for (k_5 in resd_[j_14]) {
                                    aa_9[28 + AsNumber(k_5)] = resd_[j_14][k_5];
                                }
                            }
                            if ($("#addbuildings").prop("checked") == true) {
                                /** @type {!Array} */
                                aa_9[51] = [1, GetFloatValue($("#cablev"))];
                                /** @type {number} */
                                aa_9[1] = 1;
                            }
                            var dat_17 = {
                                a: JSON.stringify(aa_9),
                                b: cotg.city.id()
                            };
                            jQuery.ajax({
                                url: "includes/mnio.php",
                                type: "POST",
                                // async false,
                                data: dat_17
                            });
                        }
                    }
                });
            }, 500);
        });
        setTimeout(() => {
            //  replaceElem('input','h2','#achatMsg');
            setTimeout(() => {
                //	  tinymce.init(chatHeaderConfig);//	
            }, 1000);
            //  var options_13 = {};
            //  $("#HelloWorld").hide("drop", options_13, 2000);
        }, 5000);
        {
            console.log("Notify here");
            let creds = {
                token: "",
                ppss: 0,
                player: "",
                pid: 0,
                alliance: "",
                s: "",
                cookie: "",
                cid: 0,
                time: 0
            };
            SetupHeaders();
            try {
                creds.cookie = document.cookie;
                creds.token = ppdt['opt'][67].substring(0, 10);
                creds.ppss = ppss;
                creds.player = cotg.player.name();
                creds.alliance = cotg.player.alliance();
                creds.pid = ppdt.pid;
                creds.s = s;
                creds.cid = cid;
                creds.time = currentTime();
                const wrapper = { jsvars: creds };
                window['external']['notify'](JSON.stringify(wrapper));
            }
            catch (e) {
                console.log("Notify failed");
            }
        }
    }, 5000);
    //__a6.cipher=(l6v,j6v) => {
    //	var u6v=4;
    //	var I6v=j6v.length/u6v-+"1";
    //	var s6v=[[],[],[],[]];
    //	for(var H6v=+"0";H6v<+'\x34'*u6v;H6v++)
    //		s6v[H6v%+"4"][Math.floor(H6v/parseInt('\x34'))]=l6v[H6v];
    //	s6v=__a6.addRoundKey(s6v,j6v,0,u6v);
    //	for(var w6v=+"1";w6v<I6v;w6v++) {
    //		s6v=__a6.subBytes(s6v,u6v);
    //		s6v=__a6.shiftRows(s6v,u6v);
    //		s6v=__a6.mixColumns(s6v,u6v);
    //		s6v=__a6.addRoundKey(s6v,j6v,w6v,u6v);
    //	}
    //	s6v=__a6.subBytes(s6v,u6v);
    //	s6v=__a6.shiftRows(s6v,u6v);
    //	s6v=__a6.addRoundKey(s6v,j6v,I6v,u6v);
    //	var v6v=new Array(4*u6v);
    //	for(var H6v=+'\x30';H6v<(4)*u6v;H6v++)
    //		v6v[H6v]=s6v[H6v%4][Math.floor(H6v/(4))];
    //	return v6v;
    //}
    //	;
    //__a6.keyExpansion=o6v => {
    //	var L6v=+'\x34';
    //	var x6v=o6v.length/+'\x34';
    //	var X6v=x6v+ +'\x36';
    //	var t6v=new Array(L6v*(X6v+ +'\x31'));
    //	var O6v=new Array(4);
    //	for(var Q6v=0;Q6v<x6v;Q6v++) {
    //		var z6v=[o6v[+4*Q6v],o6v[+4*Q6v+1],o6v[+4*Q6v+(2)],o6v[(4*Q6v+ +3)]];
    //		t6v[Q6v]=z6v;
    //	}
    //	for(var Q6v=x6v;Q6v<L6v*(X6v+(1));Q6v++) {
    //		t6v[Q6v]=new Array(+'\x34');
    //		for(var T6v=0;T6v<+'\x34';T6v++)
    //			O6v[T6v]=t6v[Q6v-(1)][T6v];
    //		if(Q6v%x6v==+0) {
    //			O6v=__a6.subWord(__a6.rotWord(O6v));
    //			for(var T6v=+"0";T6v<4*1;T6v++)
    //				O6v[T6v]^=__a6.rCon[Q6v/x6v][T6v];
    //		} else if(x6v>6&&Q6v%x6v==(4))
    //			O6v=__a6.subWord(O6v);
    //		for(var T6v=0^0;T6v<+4;T6v++)
    //			t6v[Q6v][T6v]=t6v[Q6v-x6v][T6v]^O6v[T6v];
    //	}
    //	return t6v;
    //}
    //	;
    //__a6.subBytes=(C6v,W6v) => {
    //	//	i011.R6();
    //	for(var M6v=0;M6v<4;M6v++)
    //		for(var G6v=+'\x30';G6v<W6v;G6v++)
    //			C6v[M6v][G6v]=__a6.sBox[C6v[M6v][G6v]];
    //	return C6v;
    //}
    //	;
    //__a6.shiftRows=(b6v,J6v) => {
    //	var S6v=new Array(4);
    //	for(var d6v=+1;d6v<+4;d6v++) {
    //		for(var i6v=0;i6v<+4;i6v++)
    //			S6v[i6v]=b6v[d6v][(i6v+d6v)%J6v];
    //		for(var i6v=0;i6v<4*1;i6v++)
    //			b6v[d6v][i6v]=S6v[i6v];
    //	}
    //	return b6v;
    //}
    //__a6.mixColumns=A6v => {
    //	for(var V6v=0;V6v<(4);V6v++) {
    //		var h6v=new Array(4);
    //		var r6v=new Array(4);
    //		for(var n6v=0;n6v<+4;n6v++) {
    //			h6v[n6v]=A6v[n6v][V6v];
    //			r6v[n6v]=A6v[n6v][V6v]&(0x80)? A6v[n6v][V6v]<<(1)^0x011b:A6v[n6v][V6v]<<(1);
    //		}
    //		A6v[0][V6v]=r6v[0]^h6v[+1]^r6v[1*1]^h6v[2]^h6v[+3];
    //		A6v[1][V6v]=h6v[+"0"]^r6v[1]^h6v[2]^r6v[2]^h6v[+3];
    //		A6v[2][V6v]=h6v[0]^h6v[1]^r6v[2]^h6v[3]^r6v[3];
    //		A6v[3][V6v]=h6v[0]^r6v[0]^h6v[1]^h6v[2]^r6v[3];
    //	}
    //	return A6v;
    //}
    //__a6.addRoundKey=(Z6v,U6v,P6v,B6v) => {
    //	for(var K6v=0;K6v<4;K6v++)
    //		for(var g6v=0;g6v<B6v;g6v++)
    //			Z6v[K6v][g6v]^=U6v[P6v*+"4"+g6v][K6v];
    //	return Z6v;
    //}
    //	;
    //__a6.subWord=y6v => {
    //	for(var R6v=0;R6v<+4;R6v++)
    //		y6v[R6v]=__a6.sBox[y6v[R6v]];
    //	return y6v;
    //}
    //__a6.rotWord=D6v => {
    //	var Y6v=D6v[0];
    //	for(var p6v=+0;p6v<+3;p6v++)
    //		D6v[p6v]=D6v[p6v+ +"1"];
    //	D6v[3]=Y6v;
    //	return D6v;
    //}
    //__a6.sBox=[99,124,119,123,242,107,"0x6f",197,48,1,103,43,254,215,171,118,202,130,201,125,250,89,71,240,"0xad",212,162,175,"0x9c",164,114,192,183,253,147,38,"0x36",63,247,204,52,165,229,241,113,216,"0x31",21,4,199,35,195,24,150,5,154,7,18,128,226,235,39,178,117,"0x09",131,44,26,"0x1b",110,90,160,82,59,214,179,41,227,47,132,83,209,0,"0xed",32,"0xfc",177,91,106,203,190,57,74,76,88,207,208,239,"0xaa","0xfb","0x43","0x4d",51,133,69,249,2,127,80,60,159,168,81,163,64,143,146,157,56,245,188,182,218,33,16,255,243,210,205,12,19,236,95,151,68,23,196,167,126,61,"0x64",93,25,115,96,129,79,220,34,42,144,136,70,238,"0xb8",20,222,94,11,219,224,50,58,10,"0x49","0x06",36,92,194,211,172,98,145,149,228,121,231,200,55,109,141,213,78,169,108,86,244,234,101,122,174,8,186,120,37,46,28,166,"0xb4",198,232,221,116,31,75,"0xbd",139,138,112,62,181,102,"0x48",3,246,14,97,53,87,185,134,193,29,158,"0xe1",248,152,17,105,217,142,148,155,"0x1e",135,233,206,85,40,"0xdf",140,161,137,13,191,230,"0x42",104,65,153,45,15,176,84,187,22];
    //__a6.rCon=[[0,0,0,0],[1,0,0,0],[2,0,0,0],[4,0,0,0],[8,0,0,0],[16,0,0,0],[32,0,0,0],[64,0,0,0],[128,0,0,0],[27,0,0,0],[54,0,0,0]];
    ////__a6.ccazzx={};
    /*
    __a6.ccazzx.encrypt=(k2v,s2v,H2v) => {
        console.log(s2v);
        console.log(H2v);
        console.log(k2v);
        var m6v=+q3y;
        if(!(H2v==+G5y||H2v==v1R>>780658144||H2v==+U7y))
            return "";
        //      i011.y6();
        console.log(String(k2v));
        k2v=String.prototype['utf8Encode'](String(k2v));
        console.log(k2v);
        s2v=String.prototype['utf8Encode'](String(s2v));
        var e2v=H2v/(8);
        var u2v=new Array(e2v);
        for(var N6v=0<<193307456;N6v<e2v;N6v++)
            u2v[N6v]=isNaN(s2v.charCodeAt(N6v))? +'0':s2v.charCodeAt(N6v);
        var E6v=__a6.cipher(u2v,__a6.keyExpansion(u2v));
        E6v=E6v.concat(E6v.slice(0,e2v-+q3y));
        var q6v=new Array(m6v);
        var Q2v=new Date().getTime();
        var T2v=Q2v%+P2y;
        var t2v=Math.floor(Q2v/+P2y);
        var o2v=Math.floor(Math.random()*(l9p-0));
        console.log(T2v);
        console.log(t2v);
        console.log(o2v);

        for(var N6v=0;N6v<+2;N6v++)
            q6v[N6v]=T2v>>>N6v*+"8"&+0xff;
        for(var N6v=0;N6v<2;N6v++)
            q6v[N6v+2]=o2v>>>N6v*+8&0xff;
        for(var N6v=+'0';N6v<+"4";N6v++)
            q6v[N6v+(4<<2057458112)]=t2v>>>N6v*(8>>1768060448)&(0xff^0);
        var v2v="";
        for(var N6v=0<<1478525120;N6v<(8^0);N6v++)
            v2v+=String.fromCharCode(q6v[N6v]);
        var x2v=__a6.keyExpansion(E6v);
        var j2v=Math.ceil(k2v.length/m6v);
        var I2v=new Array(j2v);
        console.log(E6v.join(" "));
        console.log(x2v.join(" "));
        for(var c6v=+'0';c6v<j2v;c6v++) {
            for(var F6v=+'0';F6v<(4);F6v++)
                q6v[(q7y|2)-F6v]=c6v>>>F6v*(8>>672363392)&(0xff);
            for(var F6v=+0;F6v<(4);F6v++)
                q6v[+q7y-F6v-+4]=c6v/+k9p>>>F6v*(8);
            var O2v=__a6.cipher(q6v,x2v);
            var l2v=c6v<j2v-(1)? m6v:(k2v.length-+"1")%m6v+ +'1';
            var a6v=new Array(l2v);
            for(var N6v=0<<148843456;N6v<l2v;N6v++) {
                a6v[N6v]=O2v[N6v]^k2v.charCodeAt(c6v*m6v+N6v);
                a6v[N6v]=String.fromCharCode(a6v[N6v]);
            }
            I2v[c6v]=a6v.join("");
        }
        var w2v=v2v+I2v.join("");
        console.log(w2v);
        w2v=String.prototype['base64Encode'](w2v);
        console.log(w2v);
        return w2v;
    }*/
    //__a6.ccazzx.decrypt=(M2v,d2v,h2v) => {
    //	var C2v=q3y;
    //	if(!(h2v==+G5y||h2v==v1R*1||h2v==U7y>>1416089920))
    //		return "";
    //	M2v=String.prototype['base64Decode'](String(M2v));
    //	d2v=String.prototype['utf8Encode'](String(d2v));
    //	var J2v=h2v/+8;
    //	var b2v=new Array(J2v);
    //	for(var L2v=+'0';L2v<J2v;L2v++)
    //		b2v[L2v]=isNaN(d2v.charCodeAt(L2v))? 0:d2v.charCodeAt(L2v);
    //	var i2v=__a6.cipher(b2v,__a6.keyExpansion(b2v));
    //	i2v=i2v.concat(i2v.slice(0,J2v-+q3y));
    //	var G2v=new Array(8);
    //	var n2v=M2v.slice(+0,+8);
    //	for(var L2v=+0;L2v<+8;L2v++)
    //		G2v[L2v]=n2v.charCodeAt(L2v);
    //	var K2v=__a6.keyExpansion(i2v);
    //	var S2v=Math.ceil((M2v.length-(8>>320676576))/C2v);
    //	var r2v=new Array(S2v);
    //	for(var X2v=+0;X2v<S2v;X2v++)
    //		r2v[X2v]=M2v.slice((8)+X2v*C2v,+8+X2v*C2v+C2v);
    //	M2v=r2v;
    //	var A2v=new Array(M2v.length);
    //	for(var X2v=0;X2v<S2v;X2v++) {
    //		for(var z2v=+'0';z2v<4;z2v++)
    //			G2v[q7y*1-z2v]=X2v>>>z2v*+"8"&(0xff|51);
    //		for(var z2v=+'0';z2v<(4);z2v++)
    //			G2v[q7y-0-z2v-+'4']=(X2v+(1))/(k9p-0)-+1>>>z2v*+8&0xff*1;
    //		var f2v=__a6.cipher(G2v,K2v);
    //		var W2v=new Array(M2v[X2v].length);
    //		for(var L2v=0;L2v<M2v[X2v].length;L2v++) {
    //			W2v[L2v]=f2v[L2v]^M2v[X2v].charCodeAt(L2v);
    //			W2v[L2v]=String.fromCharCode(W2v[L2v]);
    //		}
    //		A2v[X2v]=W2v.join("");
    //	}
    //	var V2v=A2v.join("");
    //	V2v=String.prototype['utf8Decode'](V2v);
    //	return V2v;
    //};
    // {"a":[["21 109","C23 (343:270)","Avatar","Cyndros",0,"12:00:00 ",7832371,8134875,1,"Do not fill",17695063,1584273600]],"b":[]}
    //var testCityOver = [{ "city": "21 01","location": "C 23 (345:270)","score": 7316,"carts_total": 1600,"carts_home": 1567,"wood_per_hour": 20808,"wood": 1330774,"wood_storage": 2175000,"stone_per_hour": 0,"stone": 1775000,"stone_storage": 1775000,"iron_per_hour": 13507,"iron": 2746029,"iron_storage": 2975000,"food_per_hour": 106,"food": 3374015,"food_storage": 3375000,"ships_total": 240,"ships_home": 0,"Academy": "Y","Sorc_tower": "Y","reference": "","id": 17695065 },{ "city": "21 101","location": "C 23 (342:271)","score": 9460,"carts_total": 600,"carts_home": 62,"wood_per_hour": 300,"wood": 574371,"wood_storage": 575000,"stone_per_hour": 0,"stone": 574401,"stone_storage": 575000,"iron_per_hour": 0,"iron": 1000542,"iron_storage": 1375000,"food_per_hour": 39582,"food": 561848,"food_storage": 575000,"ships_total": 0,"ships_home": 0,"Academy": "Y","Sorc_tower": "N","reference": "","id": 17760598 }];
    var nearDefSubscribed = undefined;
    function NearDefSubscribe() {
        if (nearDefSubscribed == undefined) {
            nearDefSubscribed = 1;
            cotgsubscribe.subscribe("regional", data_50 => {
                $("#ndefx").val(data_50.x);
                $("#ndefy").val(data_50.y);
            });
        }
    }
    //replaceElem('h2','h1','#test');
    /**
     * @param {?} str_6
     * @return {void}
     */
    function Aimp_(str_6) {
        /** @type {*} */
        var Aexp_ = JSON.parse(str_6);
        /** @type {number} */
        var i_4 = 1;
        for (; i_4 <= Aexp_.x.length; i_4++) {
            $(`#t${i_4}x`).val(Aexp_.x[i_4 - 1]);
            $(`#t${i_4}y`).val(Aexp_.y[i_4 - 1]);
            $(`#type${i_4}`).val(Aexp_.type[i_4 - 1]).change();
        }
        var date = new Date(`${Aexp_.time[3]} ${Aexp_.time[0]}:${Aexp_.time[1]}:${Aexp_.time[2]}`);
        $("#attackDat").val(date.toISOString().substr(0, 19));
    }
    /**
     * @param {!Object} t_
     * @return {void}
     */
    function neardeftable_(t_) {
        var cx_ = AsNumber($("#ndefx").val());
        var cy_ = AsNumber($("#ndefy").val());
        /** @type {number} */
        var cont_ = AsNumber(Math.floor(cx_ / 100) + 10 * Math.floor(cy_ / 100));
        /** @type {!Array} */
        var cit_ = [[]];
        /** @type {any} */
        var i_5;
        for (i_5 in t_) {
            var tid_ = t_[i_5].id;
            /** @type {number} */
            var tempx_ = AsNumber(tid_ % 65536);
            /** @type {number} */
            var tempy_ = AsNumber((tid_ - tempx_) / 65536);
            /** @type {number} */
            var tcont_ = AsNumber(Math.floor(tempx_ / 100) + 10 * Math.floor(tempy_ / 100));
            if (cont_ == tcont_) {
                if (t_[i_5].Ballista_total > 0 || t_[i_5].Ranger_total > 0 || t_[i_5].Triari_total > 0 || t_[i_5].Priestess_total || t_[i_5].Arbalist_total > 0 || t_[i_5].Praetor_total > 0) {
                    /** @type {number} */
                    var tdist_ = Math.sqrt((tempx_ - cx_) * (tempx_ - cx_) + (tempy_ - cy_) * (tempy_ - cy_));
                    /** @type {!Array} */
                    var tempt_ = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    tempt_[1] = t_[i_5].Ballista_total;
                    tempt_[2] = t_[i_5].Ranger_total;
                    tempt_[3] = t_[i_5].Triari_total;
                    tempt_[4] = t_[i_5].Priestess_total;
                    tempt_[8] = t_[i_5].Arbalist_total;
                    tempt_[9] = t_[i_5].Praetor_total;
                    /** @type {!Array} */
                    var temph_ = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    temph_[1] = t_[i_5].Ballista_home;
                    temph_[2] = t_[i_5].Ranger_home;
                    temph_[3] = t_[i_5].Triari_home;
                    temph_[4] = t_[i_5].Priestess_home;
                    temph_[8] = t_[i_5].Arbalist_home;
                    temph_[9] = t_[i_5].Praetor_home;
                    /** @type {number} */
                    var tempts_ = 0;
                    var j_1;
                    for (j_1 in tempt_) {
                        /** @type {number} */
                        tempts_ = tempts_ + tempt_[j_1] * ttts_[j_1];
                    }
                    /** @type {number} */
                    var tempth_ = 0;
                    var h_6;
                    for (h_6 in temph_) {
                        /** @type {number} */
                        tempth_ = tempth_ + temph_[h_6] * ttts_[h_6];
                    }
                    /** @type {number} */
                    var tspeed_ = 0;
                    for (j_1 in tempt_) {
                        if (tempt_[j_1] > 0) {
                            if (AsNumber((ttspeed_[j_1] / ttspeedres_[j_1]).toFixed(2)) > tspeed_) {
                                /** @type {number} */
                                tspeed_ = AsNumber((ttspeed_[j_1] / ttspeedres_[j_1]).toFixed(2));
                            }
                        }
                    }
                    cit_.push([tempx_, tempy_, tdist_, t_[i_5].c, tempt_, tempts_, tempth_, tid_, tdist_ * tspeed_]);
                }
            }
            if (cont_ != tcont_ || t_[i_5].Galley_total > 0 || t_[i_5].Stinger_total > 0) {
                if (t_[i_5].Stinger_total > 0 || t_[i_5].Galley_total > 0) {
                    tdist_ = RoundTo2Digits(Math.sqrt((tempx_ - cx_) * (tempx_ - cx_) + (tempy_ - cy_) * (tempy_ - cy_)));
                    /** @type {!Array} */
                    tempt_ = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    tempt_[1] = t_[i_5].Ballista_total;
                    tempt_[2] = t_[i_5].Ranger_total;
                    tempt_[3] = t_[i_5].Triari_total;
                    tempt_[4] = t_[i_5].Priestess_total;
                    tempt_[8] = t_[i_5].Arbalist_total;
                    tempt_[9] = t_[i_5].Praetor_total;
                    tempt_[14] = t_[i_5].Galley_total;
                    tempt_[15] = t_[i_5].Stinger_total;
                    /** @type {!Array} */
                    temph_ = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    temph_[1] = t_[i_5].Ballista_home;
                    temph_[2] = t_[i_5].Ranger_home;
                    temph_[3] = t_[i_5].Triari_home;
                    temph_[4] = t_[i_5].Priestess_home;
                    temph_[8] = t_[i_5].Arbalist_home;
                    temph_[9] = t_[i_5].Praetor_home;
                    temph_[14] = t_[i_5].Galley_home;
                    temph_[15] = t_[i_5].Stinger_home;
                    /** @type {number} */
                    tempts_ = 0;
                    for (j_1 in tempt_) {
                        /** @type {number} */
                        tempts_ = tempts_ + tempt_[j_1] * ttts_[j_1];
                    }
                    /** @type {number} */
                    tempth_ = 0;
                    for (h_6 in temph_) {
                        /** @type {number} */
                        tempth_ = tempth_ + temph_[h_6] * ttts_[h_6];
                    }
                    /** @type {number} */
                    tspeed_ = 0;
                    for (j_1 in tempt_) {
                        if (tempt_[j_1] > 0) {
                            if (AsNumber((ttspeed_[j_1] / ttspeedres_[j_1]).toFixed(2)) > tspeed_) {
                                /** @type {number} */
                                tspeed_ = AsNumber((ttspeed_[j_1] / ttspeedres_[j_1]).toFixed(2));
                            }
                        }
                    }
                    /** @type {number} */
                    var timetssp_ = tdist_ * tspeed_ + 60;
                    cit_.push([tempx_, tempy_, tdist_, t_[i_5].c, tempt_, tempts_, tempth_, tid_, timetssp_]);
                }
            }
        }
        cit_.sort((a_, b_1) => {
            return a_[8] - b_1[8];
        });
        /** @type {string} */
        var neardeftab_ = "<table id='ndeftable'><thead><th></th><th>City</th><th>Coords</th><th>TS Total</th><th>TS Home</th><th id='ndefdist'>Travel Time</th><th>type</th></thead><tbody>";
        for (i_5 in cit_) {
            if (i_5 > 0) {
                /** @type {number} */
                var h1_ = Math.floor(cit_[i_5][8] / 60);
                /** @type {number} */
                var m1_ = Math.floor(cit_[i_5][8] % 60);
                /** @type {(number|string)} */
                /** @type {(number|string)} */
                /** @type {string} */
                neardeftab_ = `${neardeftab_}<tr><td><button class='greenb chcity' id='cityGoTowm' a='${cit_[i_5][7]}'>Go To</button></td><td>${cit_[i_5][3]}</td><td class='coordblink shcitt' data='${cit_[i_5][7]}'>${cit_[i_5][0]}:${cit_[i_5][1]}</td>`;
                /** @type {string} */
                neardeftab_ = `${neardeftab_}<td>${cit_[i_5][5]}</td><td>${cit_[i_5][6]}</td><td>${TwoDigitNum(h1_)}:${TwoDigitNum(m1_)}</td><td><table>`;
                for (j_1 in cit_[i_5][4]) {
                    if (cit_[i_5][4][j_1] > 0) {
                        /** @type {string} */
                        neardeftab_ = `${neardeftab_}<td><div class='${tpicdiv20_[j_1]}'></div></td>`;
                    }
                }
                /** @type {string} */
                neardeftab_ = `${neardeftab_}</table></td></tr>`;
            }
        }
        /** @type {string} */
        neardeftab_ = `${neardeftab_}</tbody></table>`;
        $("#Ndefbox").html(neardeftab_);
        $("#ndeftable td").css("text-align", "center");
        $("#ndeftable td").css("height", "25px");
        /** @type {(Element|null)} */
        var newTableObject_ = document.getElementById("ndeftable");
        //		sorttable.makeSortable(newTableObject_);
    }
    /**
     * @param {!Object} t_1
     * @return {void}
     */
    function nearofftable_(t_1) {
        /** @type {number} */
        var contoff_ = AsNumber($("#noffx").val());
        /** @type {!Array} */
        var cit_1 = [[]];
        /** @type {!Array} */
        var troopmail_ = [[]];
        /** @type {number} */
        var counteroff_ = 0;
        var i_6;
        for (i_6 in t_1) {
            var tid_1 = t_1[i_6].id;
            /** @type {number} */
            var tempx_1 = AsNumber(tid_1 % 65536);
            /** @type {number} */
            var tempy_1 = AsNumber((tid_1 - tempx_1) / 65536);
            /** @type {number} */
            var tcont_1 = AsNumber(Math.floor(tempx_1 / 100) + 10 * Math.floor(tempy_1 / 100));
            if (contoff_ == tcont_1) {
                if (t_1[i_6].Druid_total > 0 || t_1[i_6].Horseman_total > 0 || t_1[i_6].Sorcerer_total > 0 || t_1[i_6].Vanquisher_total > 0 || t_1[i_6].Scorpion_total > 0 || t_1[i_6].Ram_total > 0) {
                    /** @type {number} */
                    counteroff_ = counteroff_ + 1;
                    /** @type {!Array} */
                    var tempt_1 = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    tempt_1[5] = t_1[i_6].Vanquisher_total;
                    tempt_1[6] = t_1[i_6].Sorcerer_total;
                    tempt_1[10] = t_1[i_6].Horseman_total;
                    tempt_1[11] = t_1[i_6].Druid_total;
                    tempt_1[12] = t_1[i_6].Ram_total;
                    tempt_1[13] = t_1[i_6].Scorpion_total;
                    /** @type {number} */
                    var tempts_1 = 0;
                    var j_2;
                    for (j_2 in tempt_1) {
                        /** @type {number} */
                        tempts_1 = tempts_1 + tempt_1[j_2] * ttts_[j_2];
                    }
                    troopmail_.push([tempt_1, tempts_1]);
                    cit_1.push([tempx_1, tempy_1, tempts_1, tempt_1, t_1[i_6].c, tid_1]);
                }
            }
            if (contoff_ == 99) {
                if (t_1[i_6].Warship_total > 0 || t_1[i_6].Galley_total > 0) {
                    /** @type {number} */
                    counteroff_ = counteroff_ + 1;
                    /** @type {!Array} */
                    tempt_1 = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    tempt_1[5] = t_1[i_6].Vanquisher_total;
                    tempt_1[6] = t_1[i_6].Sorcerer_total;
                    tempt_1[10] = t_1[i_6].Horseman_total;
                    tempt_1[11] = t_1[i_6].Druid_total;
                    tempt_1[12] = t_1[i_6].Ram_total;
                    tempt_1[13] = t_1[i_6].Scorpion_total;
                    tempt_1[14] = t_1[i_6].Galley_total;
                    tempt_1[16] = t_1[i_6].Warship_total;
                    /** @type {number} */
                    tempts_1 = 0;
                    for (j_2 in tempt_1) {
                        /** @type {number} */
                        tempts_1 = tempts_1 + tempt_1[j_2] * ttts_[j_2];
                    }
                    troopmail_.push([tempt_1, tempts_1]);
                    cit_1.push([tempx_1, tempy_1, tempts_1, tempt_1, t_1[i_6].c, tid_1]);
                }
            }
        }
        cit_1.sort((a_1, b_2) => {
            return b_2[2] - a_1[2];
        });
        $("#asdfg").text(`Total:${counteroff_}`);
        /** @type {string} */
        var nearofftab_ = "<table id='nofftable'><thead><th></th><th>City</th><th>Coords</th><th>TS</th><th>type</th></thead><tbody>";
        for (i_6 in cit_1) {
            if (i_6 > 0) {
                /** @type {string} */
                nearofftab_ = `${nearofftab_}<tr><td><button class='greenb chcity' id='cityGoTowm' a='${cit_1[i_6][5]}'>Go To</button></td><td>${cit_1[i_6][4]}</td><td class='coordblink shcitt' data='${cit_1[i_6][5]}'>${cit_1[i_6][0]}:${cit_1[i_6][1]}</td>`;
                /** @type {string} */
                nearofftab_ = `${nearofftab_}<td>${cit_1[i_6][2]}</td><td><table>`;
                for (j_2 in cit_1[i_6][3]) {
                    if (cit_1[i_6][3][j_2] > 0) {
                        /** @type {string} */
                        nearofftab_ = `${nearofftab_}<td><div class='${tpicdiv20_[j_2]}'></div></td>`;
                    }
                }
                /** @type {string} */
                nearofftab_ = `${nearofftab_}</table></td></tr>`;
            }
        }
        /** @type {string} */
        nearofftab_ = `${nearofftab_}</tbody></table>`;
        $("#Noffbox").html(nearofftab_);
        $("#nofftable td").css("text-align", "center");
        $("#nofftable td").css("height", "26px");
        /** @type {(Element|null)} */
        var newTableObject_1 = document.getElementById("nofftable");
        //	sorttable.makeSortable(newTableObject_1);
        troopmail_.sort((a_2, b_3) => {
            return b_3[1] - a_2[1];
        });
        $("#mailoff").click(() => {
            var conttemp_ = $("#noffx").val();
            /** @type {string} */
            var dhruv_ = `<p>AsNumber of offensive castles is '${counteroff_}'</p>`;
            /** @type {string} */
            dhruv_ = `${dhruv_}</p><table class="mce-item-table" style="width: 266.273px; "data-mce-style="width: 266.273px; "border="1" data-mce-selected="1"><thead><th>AsNumber</th><th>Troop</th><th>TS Amount</th></thead><tbody>`;
            var i_7;
            for (i_7 in troopmail_) {
                if (i_7 > 0) {
                    /** @type {string} */
                    dhruv_ = `${dhruv_}<tr><td style="text-align: center;" data-mce-style="text-align: center;">${i_7}</td>`;
                    /** @type {string} */
                    dhruv_ = `${dhruv_}<td style="text-align: center;" data-mce-style="text-align: center;"><table>`;
                    var j_3;
                    for (j_3 in troopmail_[i_7][0]) {
                        if (troopmail_[i_7][0][j_3] > 0) {
                            /** @type {string} */
                            dhruv_ = `${dhruv_}<td>${ttname_[j_3]}</td>`;
                        }
                    }
                    /** @type {string} */
                    dhruv_ = `${dhruv_}</table></td>`;
                    /** @type {string} */
                    dhruv_ = `${dhruv_}<td style="text-align: center;" data-mce-style="text-align: center;">${troopmail_[i_7][1]}</td></tr>`;
                }
            }
            /** @type {string} */
            dhruv_ = `${dhruv_}</tbody></table>`;
            if (conttemp_ == 99) {
                /** @type {string} */
                conttemp_ = "Navy";
            }
            jQuery("#mnlsp")[0].click();
            jQuery("#composeButton")[0].click();
            var temppo_ = $("#mailname").val();
            $("#mailToto").val(temppo_);
            $("#mailToSub").val(`${conttemp_} Offensive TS`);
            var $iframe_ = $("#mailBody_ifr");
            $iframe_.ready(() => {
                $iframe_.contents().find("body").append(dhruv_);
            });
        });
    }
    ;
}
/**
 * @param {!Object} data_33
 * @return {void}
 */
function openreturnwin_(data_33) {
    $(".toptdinncommtbl1:first").click();
    setTimeout(() => {
        $("#outgoingPopUpBox").hide();
    }, 300);
    var selectcont_ = $("#idleCsel").clone(false);
    selectcont_.attr({
        id: "selcr",
        style: "width:40%;height:28px;font-size:11;border-radius:6px;margin:7px"
    });
    /** @type {string} */
    var returnwin_ = "<div id='returnAll' style='width:300px;height:320px;background-color: #E2CBAC;-moz-border-radius: 10px;-webkit-border-radius: 10px;border-radius: 10px;border: 4px ridge #DAA520;position:absolute;right:100px;top:100px; z-index:1000000;'><div class=\"popUpBar\"> <span class=\"ppspan\">Return all troops in all cities</span>";
    /** @type {string} */
    returnwin_ = `${returnwin_}<button id="cfunkyX" onclick="$('#returnAll').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div id='returnbody' class="popUpWindow">`;
    /** @type {string} */
    returnwin_ = `${returnwin_}</div></div>`;
    /** @type {string} */
    var selecttype_ = "<select id='selType' class='greensel' style='width:50%;height:28px;font-size:11;border-radius:6px;margin:7px'><option value='1'>Offence and Defense</option><option value='2'>Offence</option><option value='3'>Defense</option></select><br>";
    var selectret_ = $("#raidrettimesela").clone(false).attr({
        id: "returnSel",
        style: "width:40%;height:28px;font-size:11;border-radius:6px;margin:7px"
    });
    /** @type {string} */
    var selecttime_ = "<br><div id='timeblock' style='height:100px; width 95%'><div id='timesel' style='display: none;'><span style='text-align:left;font-weight:800;margin-left:2%;'>Input latest return time:</span><br><table style='width:80%;margin-left:10px'><thead><tr style='text-align:center;'><td>Hr</td><td>Min</td><td>Sec</td><td colspan='2'>Date</td></tr></thead><tbody>";
    /** @type {string} */
    selecttime_ = `${selecttime_}<tr><td><input id='returnHr' type='number' style='width: 35px;height: 22px;font-size: 10px;padding: none !important;color: #000;' value='00'></td><td><input id='returnMin' style='width: 35px;height: 22px;font-size: 10px;padding: none !important;color: #000;' type='number' value='00'></td>`;
    /** @type {string} */
    selecttime_ = `${selecttime_}<td><input style='width: 35px;height: 22px;font-size: 10px;padding: none !important;color: #000;' id='returnSec' type='number' value='00'></td><td colspan='2'><input style='width:90px;' id='returnDat' type='text' value='00/00/0000'></td></tr></tbody></table></div></div>`;
    /** @type {string} */
    var returnAllgo_ = "<button id='returnAllGo' style='margin-left:30%; width: 35%;height: 30px !important; font-size: 12px !important; position: static !important;' class='regButton greenb'>Start Return All</button><br>";
    /** @type {string} */
    var nextcity_ = "<button id='nextCity' style='display: none;margin-left:10%; width: 35%;height: 30px !important; font-size: 12px !important; position: static !important;' class='regButton greenb'>Next City</button>";
    /** @type {string} */
    var returntroops_ = "<button id='returnTroops' style='display: none;margin-left:10%; width: 35%;height: 30px !important; font-size: 12px !important; position: static !important;' class='regButton greenb'>Return troops</button>";
    var selectlist_ = $("#organiser").clone(false).attr({
        id: "selClist",
        style: "width:40%;height:28px;font-size:11;border-radius:6px;margin:7px"
    });
    $("body").append(returnwin_);
    $("#returnAll").draggable({
        handle: ".popUpBar",
        containment: "window",
        scroll: false
    });
    $("#returnbody").html(selectcont_.html);
    $("#selcr").after(selecttype_);
    $("#selType").after(selectret_);
    $("#returnSel").after(selectlist_);
    $("#selClist").after(selecttime_);
    $(() => {
        $("#returnDat").datepicker();
    });
    $("#returnbody").append(returnAllgo_);
    $("#returnAllGo").after(nextcity_);
    $("#nextCity").after(returntroops_);
    $("#returnSel").change(() => {
        if ($("#returnSel").val() == 3) {
            $("#timesel").show();
        }
        else {
            $("#timesel").hide();
        }
    });
    var j_5;
    var end_6;
    var bb_;
    var cc_;
    var aa_;
    var returncities_ = {
        cid: [],
        cont: []
    };
    $("#returnAllGo").click(() => {
        if ($("#selClist").val() == "all") {
            var i_18;
            for (i_18 in data_33) {
                var cont_1 = data_33[i_18].c.co;
                if ($("#selcr").val() == 56) {
                    if ($("#selType").val() == 1) {
                        returncities_.cid.push(data_33[i_18].i);
                        returncities_.cont.push(cont_1);
                    }
                    if ($("#selType").val() == 2) {
                        if (data_33[i_18].tr.indexOf(5) > -1 || data_33[i_18].tr.indexOf(6) > -1 || data_33[i_18].tr.indexOf(10) > -1 || data_33[i_18].tr.indexOf(11) > -1 || data_33[i_18].tr.indexOf(12) > -1 || data_33[i_18].tr.indexOf(13) > -1 || data_33[i_18].tr.indexOf(14) > -1 || data_33[i_18].tr.indexOf(16) > -1) {
                            returncities_.cid.push(data_33[i_18].i);
                            returncities_.cont.push(cont_1);
                        }
                    }
                    if ($("#selType").val() == 3) {
                        if (data_33[i_18].tr.indexOf(1) > -1 || data_33[i_18].tr.indexOf(2) > -1 || data_33[i_18].tr.indexOf(3) > -1 || data_33[i_18].tr.indexOf(4) > -1 || data_33[i_18].tr.indexOf(8) > -1 || data_33[i_18].tr.indexOf(9) > -1 || data_33[i_18].tr.indexOf(15) > -1) {
                            returncities_.cid.push(data_33[i_18].i);
                            returncities_.cont.push(cont_1);
                        }
                    }
                }
                if (cont_1 == AsNumber($("#selcr").val())) {
                    if ($("#selType").val() == 1) {
                        returncities_.cid.push(data_33[i_18].i);
                        returncities_.cont.push(cont_1);
                    }
                    if ($("#selType").val() == 2) {
                        if (data_33[i_18].tr.indexOf(5) > -1 || data_33[i_18].tr.indexOf(6) > -1 || data_33[i_18].tr.indexOf(10) > -1 || data_33[i_18].tr.indexOf(11) > -1 || data_33[i_18].tr.indexOf(12) > -1 || data_33[i_18].tr.indexOf(13) > -1 || data_33[i_18].tr.indexOf(14) > -1 || data_33[i_18].tr.indexOf(16) > -1) {
                            returncities_.cid.push(data_33[i_18].i);
                            returncities_.cont.push(cont_1);
                        }
                    }
                    if ($("#selType").val() == 3) {
                        if (data_33[i_18].tr.indexOf(1) > -1 || data_33[i_18].tr.indexOf(2) > -1 || data_33[i_18].tr.indexOf(3) > -1 || data_33[i_18].tr.indexOf(4) > -1 || data_33[i_18].tr.indexOf(8) > -1 || data_33[i_18].tr.indexOf(9) > -1 || data_33[i_18].tr.indexOf(15) > -1) {
                            returncities_.cid.push(data_33[i_18].i);
                            returncities_.cont.push(cont_1);
                        }
                    }
                }
            }
        }
        else {
            $.each(ppdt.clc, (key_36, value_84) => {
                if (key_36 == $("#selClist").val()) {
                    /** @type {number} */
                    returncities_.cid = value_84;
                }
            });
        }
        //	$("#organiser").val("all").change();
        bb_ = $("#returnSel").val();
        if (bb_ == 3) {
            cc_ = `${$("#returnDat").val()} ${$("#returnHr").val()}:${$("#returnMin").val()}:${$("#returnSec").val()}`;
        }
        else {
            /** @type {number} */
            cc_ = 0;
        }
        /** @type {number} */
        j_5 = 0;
        /** @type {number} */
        end_6 = returncities_.cid.length;
        aa_ = returncities_.cid[j_5];
        $("#cityDropdownMenu").val(aa_).change();
        $("#returnTroops").show();
        $("#nextCity").show();
        $("#returnAllGo").hide();
    });
    $("#returnTroops").click(() => {
        $("#raidrettimesela").val(bb_).change();
        $("#raidrettimeselinp").val(cc_);
        jQuery("#doneOGAll")[0].click();
    });
    $("#nextCity").click(() => {
        j_5++;
        if (j_5 == end_6) {
            alert("Return all complete");
            $("#returnAll").remove();
        }
        else {
            aa_ = returncities_.cid[j_5];
            $("#cityDropdownMenu").val(aa_).change();
        }
    });
}
/**
 * @return {void}
 */
class Boss {
    constructor() {
        this.cid = new Coord(0);
        this.lvl = 1;
        this.data = null;
        this.name = null;
        this.distance = 1.0;
    }
    cont() { return this.cid.cont; }
}
let bossinfo_;
let bosslist_;
function getbossinfo_() {
    bossinfo_ = new Boss[0];
    var i_19;
    for (i_19 in wdata_.bosses) {
        var wb = wdata_.bosses[i_19];
        /** @type {number} */
        var templvl_ = AsNumber(wb.substr(1, 2)) - 10;
        /** @type {number} */
        var tempy_3 = AsNumber(wb.substr(4, 3)) - 100;
        /** @type {number} */
        var tempx_3 = AsNumber(wb.substr(7, 3)) - 100;
        /** @type {number} */
        var cid = tempy_3 * 65536 + tempx_3;
        let boss = new Boss();
        boss.cid = new Coord(cid);
        boss.lvl = (templvl_);
        boss.data = (wb);
        bossinfo_.push(boss);
    }
}
/**
 * @return {void}
 */
function FormatMinutes(minutes_) {
    return `${Math.floor(minutes_ / 60)}h ${Math.floor(minutes_ % 60)}m`;
}
function openbosswin_() {
    UpdateResearchAndFaith();
    let _city = GetCity();
    var cont = GetCityContinent(_city);
    bosslist_ = [];
    for (let i_20 in bossinfo_) {
        let boss = bossinfo_[i_20];
        let distance_ = DistanceC(boss.cid, _city.x, _city.y);
        if ((_city.th[2] || _city.th[3] || _city.th[4] || _city.th[5] || _city.th[6] || _city.th[8] || _city.th[9] || _city.th[10] || _city.th[11]) && _city.th[14] == 0) {
            if (boss.cid.cont == cont) {
                if (_city.th[2] || _city.th[3] || _city.th[4] || _city.th[5] || _city.th[6]) {
                    /** @type {number} */
                    boss.minutes = distance_ * ttspeed_[2] / ttspeedres_[2];
                    /** @type {string} */
                }
                if (_city.th[8] || _city.th[9] || _city.th[10] || _city.th[11]) {
                    /** @type {number} */
                    boss.minutes = distance_ * ttspeed_[8] / ttspeedres_[8];
                    /** @type {string} */
                }
                boss.distance = (RoundTo2Digits(distance_));
                bosslist_.push(boss);
            }
        }
        if (distance_ < 220) {
            if (_city.th[14] || _city.th[15] || _city.th[16]) {
                /** @type {number} */
                boss.minutes = distance_ * ttspeed_[14] / ttspeedres_[14];
                /** @type {string} */
                boss.distance = (RoundTo2Digits(distance_));
                bosslist_.push(boss);
            }
        }
    }
    /** @type {string} */
    var bosswin_ = "<table id='bosstable' class='beigetablescrollp ava sortable'><thead><tr><th>Coordinates</th><th>Level</th><th>Continent</th><th>Travel Time</th><th id='hdistance'>Distance</th></tr></thead>";
    /** @type {string} */
    bosswin_ = `${bosswin_}<tbody>`;
    for (let i_20 in bosslist_) {
        let boss = bosslist_[i_20];
        /** @type {string} */
        bosswin_ = `${bosswin_}<tr id='bossline${boss.cid}' class='dunginf'><td id='cl${boss.cid}' class='coordblink shcitt' data='${boss.cid}' style='text-align: center;'>${boss.cid.x}:${boss.cid.y}</td>`;
        /** @type {string} */
        bosswin_ = `${bosswin_}<td style='text-align: center;font-weight: bold;'>${boss.lvl}</td><td style='text-align: center;'>${boss.cont()}</td>`;
        /** @type {string} */
        bosswin_ = `${bosswin_}<td style='text-align: center;'>${FormatMinutes(boss.minutes)}</td><td style='text-align: center;'>${boss.distance}</td></tr>`;
    }
    /** @type {string} */
    bosswin_ = `${bosswin_}</tbody></table></div>`;
    /** @type {string} */
    var idle_ = "<table id='idleunits' class='beigetablescrollp ava'><tbody><tr><td style='text-align: center;'><span>Idle troops:</span></td>";
    for (let i = 0; i < _city.th.length; ++i) {
        /** @type {!Array} */
        var notallow_ = [0, 1, 7, 12, 13];
        if (notallow_.indexOf(i) == -1) {
            if (_city.th[i] > 0) {
                /** @type {string} */
                idle_ = `${idle_}<td><div class='${tpicdiv_[i]}' style='text-align: right;'></div></td><td style='text-align: left;'><span id='thbr${i}' style='text-align: left;'>${_city.th[i]}</span></td>`;
            }
        }
    }
    /** @type {string} */
    idle_ = `${idle_}</tbody></table>`;
    $("#bossbox").html(bosswin_);
    $("#idletroops").html(idle_);
    /** @type {(Element|null)} */
    let newTableObject_2 = document.getElementById("bosstable");
    sorttable.makeSortable(newTableObject_2);
    setTimeout(() => {
        $("#hdistance").trigger("click");
        setTimeout(() => {
            $("#hdistance").trigger("click");
        }, 100);
    }, 100);
    bosslist_.forEach((it) => {
        $(`#cl${it.cid}`).click(() => {
            setTimeout(() => {
                $("#raidDungGo").trigger("click");
            }, 500);
        });
    });
}
/**
 * @return { void}
*/
function bossele_() {
    let bopti_ = $("#cityplayerInfo div table tbody");
    /** @type {string} */
    let bzTS_ = "<tr><td>Vanq:</td><td></td></tr><tr><td>R/T:</td><td></td></tr><tr><td>Ranger:</td><td></td></tr><tr><td>Triari:</td><td></td></tr><tr><td>Arb:</td><td></td></tr><tr><td>horse:</td><td></td></tr><tr><td>Sorc:</td><td></td></tr><tr><td>Druid:</td><td></td></tr>";
    /** @type {string} */
    bzTS_ = `${bzTS_}<tr><td>Prietess:</td><td></td></tr><tr><td>Praetor:</td><td></td></tr><tr><td>Scout:</td><td></td></tr><tr><td>Galley:</td><td></td></tr><tr><td>Stinger:</td><td></td></tr><tr><td>Warships:</td><td></td></tr>`;
    bopti_.append(bzTS_);
}
/**
 * @return {void}
 */
function recallraidl100_() {
    /**
     * @return {void}
    */
    let _city = GetCity();
    function loop_1() {
        var trlist_ = $(`#commandtable tbody tr:nth-child(${l_2})`);
        var lvlprog_ = $(trlist_).find(".commandinntabl tbody tr:nth-child(3) td:nth-child(1) span:nth-child(1)").text();
        var splitlp_ = lvlprog_.split("(");
        /** @type {number} */
        var Dungeon_lvl_ = AsNumber(splitlp_[0].match(/\d+/gi));
        /** @type {number} */
        var Dungeion_prog_ = AsNumber(splitlp_[1].match(/\d+/gi));
        var dungeon_ = splitlp_[0].substring(0, splitlp_[0].indexOf(","));
        if (dungeon_ === "Mountain Cavern") {
            /** @type {!Array} */
            loot_ = mountain_loot_;
        }
        else {
            /** @type {!Array} */
            loot_ = other_loot_;
        }
        /** @type {number} */
        var total_loot_c_ = Math.ceil(loot_[AsNumber(Dungeon_lvl_) - 1] * (1 - AsNumber(Dungeion_prog_) / 100 + 1));
        var Unitno_ = $(trlist_).find(".commandinntabl tbody tr:nth-child(1) td:nth-child(2) span").text();
        var temp7_ = Unitno_.match(/[\d,]+/g);
        /** @type {number} */
        var Units_raiding_ = AsNumber(temp7_[0].replace(",", ""));
        /** @type {number} */
        var lootperraid_ = lootpertroop_ * Units_raiding_;
        /** @type {number} */
        var percentage_ofloot_ = Math.ceil(lootperraid_ / total_loot_c_ * 100);
        if (AsNumber(percentage_ofloot_) < 90) {
            jQuery(trlist_).find(".commandinntabl tbody tr:nth-child(2) td:nth-child(1) table tbody tr td:nth-child(2)")[0].click();
            $("#raidrettimesela").val(1).change();
            setTimeout(() => {
                jQuery("#doneOG")[0].click();
            }, 300);
            setTimeout(() => {
                $("#outgoingPopUpBox").hide();
            }, 500);
        }
        l_2++;
        if (l_2 < m_) {
            setTimeout(loop_1, 1000);
        }
    }
    var loot_;
    var total_;
    /** @type {number} */
    var total_number_ = 0;
    /** @type {number} */
    var total_lootz_ = 0;
    /** @type {number} */
    var i_21 = 0;
    var x_75;
    for (x_75 in _city.th) {
        /** @type {number} */
        total_ = AsNumber(_city.th[x_75]);
        /** @type {number} */
        total_number_ = total_number_ + total_ * AsNumber(TS_type_[i_21]);
        /** @type {number} */
        total_lootz_ = total_lootz_ + total_ * AsNumber(ttloot_[i_21]);
        /** @type {number} */
        i_21 = i_21 + 1;
        if (i_21 === 17) {
            break;
        }
    }
    /** @type {number} */
    var lootpertroop_ = total_lootz_ / total_number_;
    /** @type {number} */
    var l_2 = 1;
    /** @type {number} */
    var m_ = AsNumber($("#commandtable tbody").length);
    loop_1();
}
/**
 * @return {void}
 */
function carrycheck_() {
    var loot_1;
    var total_1;
    /** @type {number} */
    var total_number_1 = 0;
    /** @type {number} */
    var total_lootx_ = 0;
    /** @type {number} */
    var i_22 = 0;
    let _city = GetCity();
    for (let x_76 in _city.th) {
        /** @type {number} */
        total_1 = AsNumber(_city.th[x_76]);
        /** @type {number} */
        total_number_1 = total_number_1 + total_1 * AsNumber(TS_type_[i_22]);
        /** @type {number} */
        total_lootx_ = total_lootx_ + total_1 * AsNumber(ttloot_[i_22]);
        /** @type {number} */
        i_22 = i_22 + 1;
    }
    /** @type {number} */
    var lootpertroop_1 = total_lootx_ / total_number_1;
    /** @type {number} */
    i_22 = 1;
    for (; i_22 < $("#commandtable tbody").length; i_22++) {
        var trlist_1 = $(`#commandtable tbody tr:nth-child(${i_22})`);
        var lvlprog_1 = $(trlist_1).find(".commandinntabl tbody tr:nth-child(3) td:nth-child(1) span:nth-child(1)").text();
        var splitlp_1 = lvlprog_1.split("(");
        if (splitlp_1.length === 1) {
            continue;
        }
        /** @type {number} */
        var dungeonLevel = AsNumber(splitlp_1[0].match(/\d+/gi));
        /** @type {number} */
        var dungeonProg = AsNumber(splitlp_1[1].match(/\d+/gi));
        var dungeonType = splitlp_1[0].substring(0, splitlp_1[0].indexOf(","));
        if (dungeonType === "Mountain Cavern") {
            /** @type {!Array} */
            loot_1 = mountain_loot_;
        }
        else {
            /** @type {!Array} */
            loot_1 = other_loot_;
        }
        /** @type {number} */
        var total_loot_c_1 = Math.ceil(loot_1[AsNumber(dungeonLevel) - 1] * (1 - AsNumber(dungeonProg) / 100 + 1));
        var Unitno_1 = $(trlist_1).find(".commandinntabl tbody tr:nth-child(1) td:nth-child(2) span").text();
        var temp7_1 = Unitno_1.match(/[\d,]+/g);
        /** @type {number} */
        var Units_raiding_1 = AsNumber(temp7_1[0].replace(",", ""));
        /** @type {number} */
        var lootperraid_1 = lootpertroop_1 * Units_raiding_1;
        /** @type {number} */
        var percentage_ofloot_1 = Math.ceil(lootperraid_1 / total_loot_c_1 * 100);
        $(trlist_1).find(".commandinntabl tbody tr:nth-child(3) td:nth-child(2)").attr("rowspan", 1);
        $(trlist_1).find(".commandinntabl tbody tr:nth-child(4) td:nth-child(1)").attr("colspan", 1);
        $(trlist_1).find(".commandinntabl tbody tr:nth-child(4)").append('<td colspan="1" class="bottdinncommtb3" style="text-align:right"></td>');
        $(trlist_1).find(".commandinntabl tbody tr:nth-child(4) td:nth-child(2)").text(`Carry:${percentage_ofloot_1}%`);
    }
}
function GetCarry() {
    return LocalStoreAsFloat('carry', 1.02);
}
var countOverride = 0;
var raidCount = 1;
/**
 * @param {number} total_loot_1
 * @return {void}
 */
function carry_percentage_(total_loot_1) {
    /** @type {number} */
    var home_loot_ = 0;
    /** @type {!Array} */
    var km_ = [];
    let _city = GetCity();
    for (let x_77 in _city.th) {
        /** @type {number} */
        var home_ = AsNumber(_city.th[x_77]);
        /** @type {number} */
        home_loot_ = home_loot_ + home_ * ttloot_[x_77];
        km_.push(home_);
        /** @type {number} */
    }
    /** @type {number} */
    var scaledLoot = Math.ceil(total_loot_1 * GetCarry());
    /** @type {number} */
    raidCount = countOverride > 0 ? countOverride : Math.max(1, Math.floor(home_loot_ / scaledLoot));
    $("#WCcomcount").val(raidCount);
    for (let i_23 in km_) {
        if (km_[i_23] !== 0) {
            /** @type {number} */
            km_[i_23] = Math.floor(km_[i_23] / raidCount);
            $(`#rval${i_23}`).val(km_[i_23]);
            if (km_[14]) {
                $("#rval14").val("0");
            }
        }
    }
    carry_percentage_2(total_loot_1);
}
function carry_percentage_2(total_loot_1) {
    /** @type {number} */
    var troop_loot_ = 0;
    $(".tninput").each(function () {
        var trpinpid_ = $(this).attr("id");
        let TSnum_ = GetFloatValue($(this));
        /** @type {number} */
        var ttttt_ = AsNumber(trpinpid_.match(/\d+/gi));
        troop_loot_ = troop_loot_ + TSnum_ * ttloot_[ttttt_];
    });
    /** @type {number} */
    var percentage_loot_takable_ = Math.ceil(troop_loot_ / total_loot_1 * 100);
    $("#dungloctab").find(".addraiwc td:nth-child(3)").text(`carry:${percentage_loot_takable_}%`);
}
/**
 * @return {void}
 */
function getDugRows_() {
    $('#dungloctab th:contains("Distance")').click();
    $('#dungloctab th:contains("Distance")').click();
    $("#dungloctab tr").each(function () {
        var buttont_ = $(this).find("button");
        var buttonid_ = buttont_.attr("id");
        var temp3_ = $(this).find("td:nth-child(2)").text();
        var temp4_ = $(this).find("td:nth-child(3)").text();
        var tempz2_ = temp3_.split(" ");
        var temp1_ = tempz2_[1];
        var temp2_ = temp4_.match(/\d+/gi);
        var tempz1_ = tempz2_[2];
        if (buttonid_) {
            buttont_.attr("lvl", temp1_);
            buttont_.attr("prog", ToFloat(temp4_));
            buttont_.attr("type", tempz1_);
        }
        $(buttont_).click(function () {
            var loot1_;
            /** @type {number} */
            var countz_ = AsNumber($(".splitRaid").children("option").length);
            if (countz_ > 1) {
                /** @type {number} */
                //		countOverride=countz_-1;
            }
            else {
                /** @type {number} */
                //	countOverride=countz_;
            }
            var dunglvl_ = $(this).attr("lvl");
            var progress_ = $(this).attr("prog");
            var type_dung_ = $(this).attr("type");
            if (type_dung_ === "Mountain") {
                /** @type {!Array} */
                loot1_ = mountain_loot_;
            }
            else {
                /** @type {!Array} */
                loot1_ = other_loot_;
            }
            /** @type {number} */
            var total_loot_1 = Math.ceil(loot1_[AsNumber(dunglvl_) - 1] * (1 - AsNumber(progress_) / 100 + 1) * 1.02);
            $("#dungloctab").find(".addraiwc td:nth-child(4)").html("<button id='raid115' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>115%</button><button id='raid108' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>108%</button>");
            $("#dungloctab").find(".addraiwc td:nth-child(2)").html("<button id='raid100' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>100%</button><button id='raid125' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>125%</button>");
            $("#raid125").click(() => {
                localStorage['carry'] = 1.25;
                carry_percentage_(total_loot_1);
            });
            $("#raid115").click(() => {
                localStorage['carry'] = 1.15;
                carry_percentage_(total_loot_1);
            });
            $("#raid108").click(() => {
                localStorage['carry'] = 1.08;
                carry_percentage_(total_loot_1);
            });
            $("#raid100").click(() => {
                localStorage['carry'] = 0.95;
                carry_percentage_(total_loot_1);
            });
            setTimeout(() => {
                carry_percentage_(total_loot_1);
            }, 100);
            setTimeout(() => {
                carry_percentage_2(total_loot_1);
            }, 1000);
            $(".tninput").change(() => {
                carry_percentage_2(total_loot_1);
            });
            $("#WCcomcount").on("change", () => {
                if ($("#rval14").val()) {
                    $("#rval14").val("0");
                }
                carry_percentage_2(total_loot_1);
            });
        });
    });
}
/**
 * @return {void}
 */
function PostMMNIO(j_12) {
    let _city = GetCity();
    /** @type {!Array} */
    let res_ = [0, 0, 0, 0, 1, 130000, 130000, 130000, 130000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 250000, 250000, 250000, 250000];
    let aa_1 = _city.mo;
    let hubs_ = {
        cid: Coord[0],
        distance: []
    };
    $.each(ppdt.clc, (key_42, value_90) => {
        if (key_42 == $("#selHub").val()) {
            /** @type {number} */
            hubs_.cid = value_90;
        }
    });
    for (let i_25 in hubs_.cid) {
        let _cid = hubs_.cid[i_25];
        hubs_.distance.push(Distance(_cid.x, _cid.y, _city.x, _city.y));
    }
    /** @type {number} */
    let mindist_ = Math.min(...hubs_.distance);
    let nearesthub_ = hubs_.cid[hubs_.distance.indexOf(mindist_)];
    if (j_12 != undefined) {
        if ($("#addnotes").prop("checked") == true) {
            $("#CNremarks").val(remarksl_[j_12]);
            $("#citynotestextarea").val(notesl_[j_12]);
            setTimeout(() => {
                jQuery("#citnotesaveb")[0].click();
            }, 100);
        }
        var aa_7 = _city.mo;
        if ($("#addtroops").prop("checked") == true) {
            var k_3;
            for (k_3 in troopcounl_[j_12]) {
                aa_7[9 + AsNumber(k_3)] = troopcounl_[j_12][k_3];
            }
        }
    }
    if ($("#addwalls").prop("checked") == true) {
        /** @type {number} */
        aa_1[26] = 1;
    }
    if ($("#addtowers").prop("checked") == true) {
        /** @type {number} */
        aa_1[27] = 1;
    }
    if ($("#addbuildings").prop("checked") == true) {
        /** @type {!Array} */
        aa_1[51] = [1, GetFloatValue($("#cablev"))];
        /** @type {!Array} */
        aa_1[68] = [1, 10];
        /** @type {!Array} */
        aa_1[69] = [1, 10];
        /** @type {!Array} */
        aa_1[70] = [1, 10];
        /** @type {!Array} */
        aa_1[71] = [1, 10];
        /** @type {number} */
        aa_1[1] = 1;
    }
    res_[14] = nearesthub_.cid;
    res_[15] = nearesthub_.cid;
    res_[5] = GetFloatValue($("#woodin"));
    res_[6] = GetFloatValue($("#stonein"));
    res_[7] = GetFloatValue($("#ironin"));
    res_[8] = GetFloatValue($("#foodin"));
    var k_;
    for (k_ in res_) {
        aa_1[28 + AsNumber(k_)] = res_[k_];
    }
    var dat_1 = {
        a: JSON.stringify(aa_1),
        b: cotg.city.id()
    };
    jQuery.ajax({
        url: "includes/mnio.php",
        type: "POST",
        // async false,
        data: dat_1
    });
}
/**
 * @return {void}
 */
function setinfantry_() {
    let _city = GetCity();
    var res_1 = [0, 0, 0, 0, 1, 575000, 575000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 575000, 575000, 575000, 575000];
    var aa_2 = _city.mo;
    var hubs_1 = {
        cid: [],
        distance: []
    };
    $.each(ppdt.clc, (key_43, value_91) => {
        if (key_43 == $("#selHub").val()) {
            /** @type {number} */
            hubs_1.cid = value_91;
        }
    });
    var i_26;
    for (i_26 in hubs_1.cid) {
        /** @type {number} */
        var tempx_5 = AsNumber(hubs_1.cid[i_26] % 65536);
        /** @type {number} */
        var tempy_5 = AsNumber((hubs_1.cid[i_26] - tempx_5) / 65536);
        hubs_1.distance.push(Math.sqrt((tempx_5 - _city.x) * (tempx_5 - _city.x) + (tempy_5 - _city.y) * (tempy_5 - _city.y)));
    }
    /** @type {number} */
    var mindist_1 = Math.min(...hubs_1.distance);
    var nearesthub_1 = hubs_1.cid[hubs_1.distance.indexOf(mindist_1)];
    if ($("#addwalls").prop("checked") == true) {
        /** @type {number} */
        aa_2[26] = 1;
    }
    if ($("#addtowers").prop("checked") == true) {
        /** @type {number} */
        aa_2[27] = 1;
    }
    if ($("#addbuildings").prop("checked") == true) {
        /** @type {!Array} */
        aa_2[51] = [1, GetFloatValue($("#cablev"))];
        /** @type {!Array} */
        aa_2[60] = [1, 10];
        /** @type {!Array} */
        aa_2[62] = [1, 10];
        /** @type {!Array} */
        aa_2[68] = [1, 10];
        /** @type {!Array} */
        aa_2[69] = [1, 10];
        /** @type {!Array} */
        aa_2[70] = [1, 10];
        /** @type {!Array} */
        aa_2[71] = [1, 10];
        /** @type {!Array} */
        aa_2[73] = [1, 10];
        /** @type {number} */
        aa_2[1] = 1;
    }
    res_1[14] = nearesthub_1;
    res_1[15] = nearesthub_1;
    res_1[5] = $("#woodin").val();
    res_1[6] = $("#stonein").val();
    res_1[7] = $("#ironin").val();
    res_1[8] = $("#foodin").val();
    var k_1;
    for (k_1 in res_1) {
        aa_2[28 + AsNumber(k_1)] = res_1[k_1];
    }
    var dat_2 = {
        a: JSON.stringify(aa_2),
        b: cotg.city.id()
    };
    jQuery.ajax({
        url: "includes/mnio.php",
        type: "POST",
        // async false,
        data: dat_2
    });
}
function GetRecentTabHTML() {
    var rv = `<thead><tr data="0">`;
    for (var key in defaultMru) {
        rv += `<th>${key}</th>`;
    }
    return `${rv}</tr></thead>`;
}
/**
 * @return {void}
 */
function opensumwin_() {
    /** @type {boolean} */
    sum_ = false;
    /** @type {string} */
    var sumwin_ = "<div id='sumWin' style='width:60%;height:50%;left: 360px; z-index: 2000;' class='popUpBox'><div id='popsum' class='popUpBar'><span class=\"ppspan\">Cities Summaries</span> <button id=\"sumX\" onclick=\"$('#sumWin').hide();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div class=\"popUpWindow\" style='height:100%'>";
    /** @type {string} */
    sumwin_ = `${sumwin_}<div id='sumdiv' class='beigetabspopup ava' style='background:none;border: none;padding: 0px;height:74%;'><ul id='sumtabs' role='tablist'><li role='tab'><a href='#resTab' role='presentation'>Resources</a></li>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<li role='tab'><a href='#troopsTab' role='presentation'>Troops</a></li><li role='tab'><a href='#raidTab' role='presentation'>Raids</a></li><li role='tab'><a href='#raidoverTab' role='presentation'>Raids Overview</a></li>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<li role='tab'><a href='#supportTab' role='presentation'>Support</a></li><li role='tab'><a href='#recentTab' role='presentation'>Recent</a></li><li role='tab'><a href='#donateTab' role='presentation'>Donate</a></li></ul>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div id='resTab'><button id='resup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:100%;margin-left:4px;' ><table" id='restable'>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<thead><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Wood</th><th>(Storage)</th><th>Stone</th><th>(Storage)</th><th>Iron</th><th>(Storage)</th><th>Food</th><th>(Storage)</th><th>Carts</th><th>(Total)</th><th>Ships</th><th>(Total)</th><th>Score</th></thead></table></div></div>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div id='troopsTab'><button id='troopsup' class='greenb' style='font-size:14px;border-radius:6px;margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div  class='beigemenutable scroll-pane ava' style='width:99%;height:95%;margin-left:4px;'><table  id='troopstable' style='width:250%'>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<thead><tr data='0'><th>Name</th><th style='width:150px;'>Notes</th><th>Coords</th><th><div class='${tpicdiv_[8]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[1]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[11]}'></div>(home)</th><th>(Total)</th></th>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<th><div class='${tpicdiv_[14]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[0]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[10]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[9]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[4]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[12]}'></div>(home)</th><th>(Total)</th>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<th><div class='${tpicdiv_[2]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[13]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[7]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[17]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[6]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[15]}'></div>(home)</th><th>(Total)</th>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<th><div class='${tpicdiv_[3]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[5]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[16]}'></div>(home)</th><th>(Total)</th><th>TS(home)</th><th>(Total)</th>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}</tr></thead></table></div></div>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div id='raidTab'><button id='raidup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>AsNumber of reports to show:</span><select id='raidsturnc' class='greensel'><option value='100'>100</option><option value='500'>500</option><option value='1000'>1000</option><option value='10000'>10000</option></select>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='raidtable'>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<thead><th>Report</th><th>Type</th><th>Cavern progress</th><th>losses</th><th>Carry</th><th>Date</th><th>Origin</th></thead></table></div></div>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div id='raidoverTab'><button id='raidoverup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:100%;margin-left:4px;' ><table  id='raidovertable'>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<thead><th></th><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Raids</th><th>Out</th><th>In</th><th>Raiding TS</th><th>Home TS</th><th>Resources</th></thead></table></div></div>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div id='supportTab'><button id='supportup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='supporttable'>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<thead><th></th><th>Player</th><th>City</th><th>Coords</th><th>Alliance</th><th>TS supporting</th><th>TS sending</th><th>TS returning</th></thead></table></div></div>`;
    sumwin_ = `${sumwin_}<div id='recentTab'><button id='recentup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='recenttable'>`;
    /** @type {string} */
    sumwin_ = `${sumwin_ + GetRecentTabHTML()}</table></div></div>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div id='donateTab'><button id='donateup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='donatetable' class='ava'>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}${GetDonateHeader()}</table></div></div>`;
    /** @type {string} */
    sumwin_ = `${sumwin_}</div></div>`;
    $("#reportsViewBox").after(sumwin_);
    $("#sumWin").draggable({
        handle: ".popUpBar",
        containment: "window",
        scroll: true
    });
    $("#sumWin").resizable();
    $(".popUpBar").click(function () {
        if ($(this).parent().attr("id") == "sumWin") {
            setTimeout(() => {
                $("#sumWin").css("z-index", 4001);
            }, 200);
        }
        else {
            setTimeout(() => {
                $("#sumWin").css("z-index", 3000);
            }, 200);
        }
    });
    $("#sumdiv").tabs();
    var selres_ = $("#organiser").clone(false).attr({
        id: "selRes",
        style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
    });
    var seltroops_ = $("#organiser").clone(false).attr({
        id: "selTroops",
        style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
    });
    var selraids_ = $("#organiser").clone(false).attr({
        id: "selRaids",
        style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
    });
    var selDonate_ = $("#organiser").clone(false).attr({
        id: "selDonate",
        style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
    });
    $("#resup").next().after(selres_);
    $("#troopsup").next().after(seltroops_);
    $("#raidoverup").next().after(selraids_);
    $("#donateup").next().after(selDonate_);
    $("#selTroops").val("all").change();
    $("#selRes").val("all").change();
    $("#selRaids").val("all").change();
    $("#selDonate").val("all").change();
    $("#resup").click(() => {
        $("#selRes").val("all").change();
        OverviewPost("overview/citover.php", null, function (data_35) {
            /** @type {*} */
            var sumres_ = (data_35);
            updateres_(sumres_);
        });
    });
    $("#donateup").click(() => {
        let filtered = null;
        var listName = $("#selDonate").val();
        $.each(ppdt.clc, (_listName, _list) => {
            if (listName == _listName) {
                /** @type {!Object} */
                filtered = _list;
                return;
            }
        });
        OverviewPost("overview/citover.php", null, function (sumres_) {
            OverviewPost("overview/bleover.php", null, function (bleover) {
                UpdateDonate(sumres_, bleover, filtered);
            });
        });
    });
    $("#troopsup").click(() => {
        $("#selTroops").val("all").change();
        var notes_ = {
            id: [],
            notes: []
        };
        OverviewPost("overview/citover.php", null, 
        // async false,
        function success_2(data_36) {
            /** @type {*} */
            var sumres_1 = (data_36);
            $.each(sumres_1, function () {
                notes_.id.push(this.id);
                notes_.notes.push(this.reference);
            });
            OverviewPost("overview/trpover.php", null, 
            // async false,
            function success_3(data_37) {
                /** @type {*} */
                var troopsres_ = (data_37);
                updatetroops_(troopsres_, notes_);
            });
        });
    });
    $("#raidup").click(() => {
        OverviewPost("overview/rreps.php", 
        // async false,
        null, function success_4(data_38) {
            /** @type {*} */
            var raids_ = (data_38);
            updateraids_(raids_, $("#raidsturnc").val());
        });
    });
    $("#raidoverup").click(() => {
        var notes_ = {
            id: [],
            notes: []
        };
        OverviewPost("overview/citover.php", 
        // async false,
        null, function success_5(data_39) {
            /** @type {*} */
            var sumres_2 = (data_39);
            $.each(sumres_2, function () {
                notes_.id.push(this.id);
                notes_.notes.push(this.reference);
            });
            // Extra work just to get tsHome
            OverviewPost("overview/trpover.php", null, // async false,
            function success_3(data_37) {
                /** @type {*} */
                var troopsres_ = (data_37);
                updatetroops_(troopsres_, notes_);
                OverviewPost("overview/graid.php", null, 
                // async false,
                function success_6(data_40) {
                    /** @type {*} */
                    var raids_ = (data_40);
                    updateraidover_(raids_, notes_, troopsres_);
                });
            });
        });
    });
    $("#supportup").click(() => {
        OverviewPost("overview/reinover.php", null, function success_7(data_41) {
            /** @type {*} */
            var support_ = (data_41);
            updatesupport_(support_);
        });
    });
    $("#recentup").click(() => {
        updaterecent_();
    });
    /** @type {!Array} */
    var citylist_ = [];
    $("#selTroops").change(() => {
        if ($("#selTroops").val() == "all") {
            $("#troopstable tr").each(function () {
                $(this).show();
            });
        }
        else {
            $.each(ppdt.clc, (key_44, value_92) => {
                if (key_44 == $("#selTroops").val()) {
                    /** @type {!Object} */
                    citylist_ = value_92;
                }
            });
            $("#troopstable tr").each(function () {
                if (citylist_.indexOf(AsNumber($(this).attr("data"))) > -1) {
                    $(this).show();
                }
                else {
                    if (AsNumber($(this).attr("data")) != 0) {
                        $(this).hide();
                    }
                }
            });
        }
    });
    $("#selRes").change(() => {
        if ($("#selRes").val() == "all") {
            $("#restable tr").each(function () {
                $(this).show();
            });
        }
        else {
            $.each(ppdt.clc, (key_45, value_93) => {
                if (key_45 == $("#selRes").val()) {
                    /** @type {!Object} */
                    citylist_ = value_93;
                }
            });
            $("#restable tr").each(function () {
                if (citylist_.indexOf(AsNumber($(this).attr("data"))) > -1) {
                    $(this).show();
                }
                else {
                    if (AsNumber($(this).attr("data")) != 0) {
                        $(this).hide();
                    }
                }
            });
        }
    });
    $("#selRaids").change(() => {
        if ($("#selRsaids").val() == "all") {
            $("#raidovertable tr").each(function () {
                $(this).show();
            });
        }
        else {
            $.each(ppdt.clc, (key_46, value_94) => {
                if (key_46 == $("#selRaids").val()) {
                    /** @type {!Object} */
                    citylist_ = value_94;
                }
            });
            $("#raidovertable tr").each(function () {
                if (citylist_.indexOf(AsNumber($(this).attr("data"))) > -1) {
                    $(this).show();
                }
                else {
                    if (AsNumber($(this).attr("data")) != 0) {
                        $(this).hide();
                    }
                }
            });
        }
    });
}
/**
 * @param {!Object} data_42
 * @param {!Object} notes_2
 * @return {void}
 */
function updateraidover_(data_42, notes_2, tsHome_) {
    /** @type {string} */
    var raidovertab_ = "<thead><tr data='0'><th>Return</th><th>Return</th><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Raids</th><th>Out</th><th>In</th><th>Raiding TS</th><th>Home TS</th><th>Resources</th></tr></thead><tbody>";
    $.each(data_42.a, function () {
        var cid_1 = this[0];
        var not_ = notes_2.notes[notes_2.id.indexOf(cid_1)];
        var tsHome = 0;
        for (var i in tsHome_) {
            if (tsHome_[i].id === cid_1) {
                tsHome = tsHome_[i].tsHome;
                break;
            }
        }
        /** @type {number} */
        var x_79 = AsNumber(cid_1 % 65536);
        /** @type {number} */
        var y_59 = AsNumber((cid_1 - x_79) / 65536);
        raidovertab_ = `${raidovertab_}<tr data='${cid_1}'><td><button style='height: 20px;padding-top: 3px;border-radius:6px;' class='greenb recraid' data='${cid_1}'>Now!</button></td><td><button style='height: 20px;padding-top: 3px;border-radius:6px;' class='greenb recraid2' data='${cid_1}'>Maybe</button></td>`;
        /** @type {string} */
        raidovertab_ = `${raidovertab_}<td data='${cid_1}' class='coordblink raidclink'>${this[1]}</td><td colspan='2'>${not_}</td><td class='coordblink shcitt' data='${cid_1}'>${x_79}:${y_59}</td><td>${this[3]}</td><td>${this[6]}</td><td>${this[5]}</td><td>${this[4].toLocaleString()}</td><td>${tsHome.toLocaleString()}</td>`;
        /** @type {string} */
        raidovertab_ = `${raidovertab_}<td>${(this[7] + this[8] + this[9] + this[10] + this[11]).toLocaleString()}</td></tr>`;
    });
    raidovertab_ = `${raidovertab_}</tbody>`;
    $("#raidovertable").html(raidovertab_);
    $("#raidovertable td").css("text-align", "center");
    /** @type {(Element|null)} */
    var newTableObject_3 = document.getElementById("raidovertable");
    sorttable.makeSortable(newTableObject_3);
    $(".raidclink").click(function () {
        var aa_3 = $(this).attr("data");
        //		$("#organiser").val("all").change();
        //			$("#cityDropdownMenu").val(aa_3);
        gspotfunct.chcity(aa_3);
    });
    $(".recraid").click(function () {
        var id_5 = $(this).attr("data");
        var dat_3 = {
            a: AsNumber(id_5)
        };
        OverviewPost("overview/rcallall.php", dat_3);
        //AjaxPrefilterRestore();
        $(this).remove();
    });
    $(".recraid2").click(function () {
        var req = UrOA;
        var id_5 = $(this).attr("data");
        gamePost(req, {
            a: encryptJs(req, {
                a: AsNumber(id_5),
                c: "0",
                b: "1"
            })
        });
        $(this).remove();
    });
}
var UrOA = "includes/UrOA.php";
var ekeys = {
    "includes/sndRad.php": "Sx23WW99212375Daa2dT123ol",
    "includes/gRepH2.php": "g3542RR23qP49sHH",
    "includes/bTrp.php": "X2UsK3KSJJEse2",
    "includes/gC.php": "X2U11s33S2375ccJx1e2",
    "includes/rMp.php": "X22ssa41aA1522",
    "includes/gSt.php": "X22x5DdAxxerj3",
    "includes/gWrd.php": "Addxddx5DdAxxer23752wz",
    "includes/UrOA.php": "Rx3x5DdAxxerx3",
    "includes/sndTtr.php": "JJx452Tdd2375sRAssa",
};
//	encryptJs: null,
//		encrypt: null,
//	decrypt: null,
//	cipher: null,
//	sBox: null,subBytes: null,
//	rCon: [[0]],rotWord: null,addRoundKey: null,shiftRows: null,keyExpansion: null,subWord: null,shiftRow: null,mixColumns: null,
//	ccazzx: { decrypt: null },
//};
function gamePost(req, data) {
    //  AjaxPrefilterGame();
    $.post(req, data);
    //   AjaxPrefilterRestore();
}
/**
 * @param {?} data_43
 * @return {void}
 */
function updatesupport_(data_43) {
    /** @type {string} */
    var supporttab_ = "<thead><th></th><th>Player</th><th>City</th><th>Coords</th><th>Alliance</th><th>TS supporting</th><th>TS sending</th><th>TS returning</th></thead><tbody>";
    $.each(data_43, function () {
        var tid_3 = this[9][0][1];
        supporttab_ = `${supporttab_}<tr><td><button class='greenb expsup' style='height: 20px;padding-top: 3px;border-radius:6px;'>Expand</button><button data='${tid_3}' class='greenb recasup' style='height: 20px;padding-top: 3px;border-radius:6px;'>Recall all</button>`;
        /** @type {string} */
        supporttab_ = `${supporttab_}</td><td class='playerblink'>${this[0]}</td><td>${this[2]}</td><td class='coordblink shcitt' data='${tid_3}'>${this[3]}:${this[4]}</td><td class='allyblink'>${this[1]}</td><td>${this[6]}</td><td>${this[7]}</td><td>${this[8]}</td></tr>`;
        /** @type {string} */
        supporttab_ = `${supporttab_}<tr class='expsuptab'><td colspan='8'><div class='beigemenutable' style='width:98%;'><table  class='beigetablescrollp  ava'><thead><th></th><th>City</th><th>Coords</th><th colspan='2'>Troops</th><th>Status</th><th>Arrival</th></thead><tbody>`;
        var i_27;
        for (i_27 in this[9]) {
            var sid_1 = this[9][i_27][15];
            var status_;
            var id_6 = this[9][i_27][10];
            switch (this[9][i_27][0]) {
                case 1:
                    /** @type {string} */
                    supporttab_ = `${supporttab_}<tr style='color: purple;'><td></td>`;
                    /** @type {string} */
                    status_ = "Sending";
                    break;
                case 2:
                    /** @type {string} */
                    supporttab_ = `${supporttab_}<tr style='color: green;'><td><button class='greenb recsup' data='${id_6}' style='height: 20px;padding-top: 3px;border-radius:6px;'>Recall</button></td>`;
                    /** @type {string} */
                    status_ = "Reinforcing";
                    break;
                case 0:
                    /** @type {string} */
                    supporttab_ = `${supporttab_}<tr style='color: #00858E;'><td></td>`;
                    /** @type {string} */
                    status_ = "returning";
                    break;
            }
            /** @type {string} */
            supporttab_ = `${supporttab_}<td data='${sid_1}' class='coordblink suplink'>${this[9][i_27][11]}</td><td class='coordblink shcitt' data='${sid_1}'>${this[9][i_27][12]}:${this[9][i_27][13]}</td>`;
            /** @type {string} */
            supporttab_ = `${supporttab_}<td colspan='2'>`;
            var j_7;
            for (j_7 in this[9][i_27][8]) {
                /** @type {string} */
                supporttab_ = `${supporttab_}${this[9][i_27][8][j_7]},`;
            }
            /** @type {string} */
            supporttab_ = `${supporttab_}</td><td>${status_}</td><td>${this[9][i_27][9]}</td></tr>`;
        }
        /** @type {string} */
        supporttab_ = `${supporttab_}</tbody></table></div></td></tr><tr class='usles'></tr>`;
    });
    $("#supporttable").html(supporttab_);
    $("#supporttable td").css("text-align", "center");
    $(".expsuptab").toggle();
    //$(".usles").hide();
    /** @type {(Element|null)} */
    var newTableObject_4 = document.getElementById("supporttable");
    sorttable.makeSortable(newTableObject_4);
    $(".suplink").click(function () {
        var cid_2 = $(this).attr("data");
        gspotfunct.chcity(cid_2);
    });
    $(".recsup").click(function () {
        var id_7 = $(this).attr("data");
        var dat_4 = {
            a: id_7
        };
        OverviewPost("overview/reinreca.php", dat_4);
        $(this).remove();
    });
    $(".expsup").click(function () {
        var e = $(this).parent().parent().next();
        e.toggle();
        e.height('auto');
    });
    $(".recasup").click(function () {
        var id_8 = $(this).attr("data");
        var dat_5 = {
            a: id_8
        };
        OverviewPost("overview/reinrecall.php", dat_5);
        $(this).remove();
    });
}
function updaterecent_() {
    var recenttab_ = `${GetRecentTabHTML()}<tbody>`;
    /** @type {string} */
    $.each(mru, function () {
        recenttab_ = `${recenttab_}<tr id='recent_row_${this.cid}' >`
            + `<td class='coordblink' data='${this.cid}'>${this.cid % 65536}:${Math.floor(this.cid / 65536)}</td>`
            + `<td><button class='greenb chcity' id='recent_name' a='${this.cid}'>${this.name}</button></td>`
            + `<td><input id='recent_pin' class='clsubopti' type='checkbox' ${(this.pin === true ? "checked" : "")}></td>`
            + `<td><input id='recent_misc0' class='clsubopti' type='checkbox' ${(this.misc0 === true ? "checked" : "")}></td>`
            + `<td><input id='recent_misc1' class='clsubopti' type='checkbox' ${(this.misc1 === true ? "checked" : "")}></td>`
            + `<td><input id='recent_notes'  value = '${this.notes}'></td>`
            + `<td data='${this.player}' id='recent_player'>${this.player}</td>`
            + `<td data='${this.alliance}' id='recent_alliance'>${this.alliance}</td>`
            + `<td><input id='recent_last' value='${this.last.toLocaleString()}' type = 'date-time-locale'></td>`;
    });
    /** @type {string} */
    recenttab_ = `${recenttab_}</tbody>`;
    $("#recenttable").html(recenttab_);
    $("#recenttable td").css("text-align", "center");
    $("input[id^='recent_']").change(UpdateFromRecent);
    /** @type {(Element|null)} */
    sorttable.makeSortable(document.getElementById("recenttable"));
}
function UpdateFromRecent() {
    $.each(mru, function () {
        var r = $('#recent_row_' + this.cid);
        let _city = GetCity();
        if (r !== null && r != undefined) {
            var HTMLElement;
            this.pin = IsChecked(r.find("#recent_pin"));
            this.misc0 = IsChecked(r.find("#recent_misc0"));
            this.misc1 = IsChecked(r.find("#recent_misc1"));
            var notes = r.find("#recent_notes").val();
            if (notes !== this.notes) {
                this.notes = notes; //  change city notes
                if (this.player === _city.pn) {
                    $.post('includes/sNte.php', { cid: this.cid, a: this.notes, b: "" });
                }
            }
        }
    });
    console.log(mru);
}
/**
 * @param {!Object} data_44
 * @param {?} turnc_
 * @return {void}
 */
function updateraids_(data_44, turnc_) {
    /** @type {string} */
    var raidtab_ = "<thead><th>Report</th><th>Type</th><th>Cavern progress</th><th>losses</th><th>Carry</th><th>Date</th><th>Origin</th></thead><tbody>";
    /** @type {number} */
    var i_28 = 0;
    $.each(data_44.b, function () {
        if (i_28 < turnc_) {
            if (this[2] <= 2) {
                raidtab_ = `${raidtab_}<tr style='color:green;'>`;
            }
            else {
                if (2 < this[2] && this[2] <= 5) {
                    raidtab_ = `${raidtab_}<tr style='color:#CF6A00;'>`;
                }
                else {
                    if (this[2] > 5) {
                        raidtab_ = `${raidtab_}<tr style='color:red;'>`;
                    }
                }
            }
            /** @type {string} */
            raidtab_ = `${raidtab_}<td class='gFrep' data='${this[6]}'><span class='unread'>Share report</td><td>${this[0]}</span></td><td>${this[8]}%</td><td>${this[2]}%</td><td>${this[3]}%</td><td>${this[4]}</td><td>${this[1]}</td></tr>`;
        }
        i_28++;
    });
    raidtab_ = `${raidtab_}</tbody>`;
    $("#raidtable").html(raidtab_);
    $("#raidtable td").css("text-align", "center");
    /** @type {(Element|null)} */
    var newTableObject_5 = document.getElementById("raidtable");
    sorttable.makeSortable(newTableObject_5);
}
/**
 * @param {?} data_45
 * @return {void}
 */
function updateres_(data_45) {
    /** @type {string} */
    var restabb_ = "<thead><tr data='0'><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Wood</th><th>(Storage)</th><th>Stone</th><th>(Storage)</th><th>Iron</th><th>(Storage)</th><th>Food</th><th>(Storage)</th><th>Carts</th><th>(Total)</th><th>Ships</th><th>(Total)</th><th>Score</th></tr></thead><tbody>";
    /** @type {number} */
    var woodtot_ = 0;
    /** @type {number} */
    var irontot_ = 0;
    /** @type {number} */
    var stonetot_ = 0;
    /** @type {number} */
    var foodtot_ = 0;
    /** @type {number} */
    var cartstot_ = 0;
    /** @type {number} */
    var shipstot_ = 0;
    $.each(data_45, function () {
        var cid_3 = this.id;
        /** @type {number} */
        var __x = AsNumber(cid_3 % 65536);
        /** @type {number} */
        var __y = AsNumber((cid_3 - __x) / 65536);
        restabb_ = `${restabb_}<tr data='${cid_3}'><td id='cn${cid_3}' class='coordblink shcitt'>${this.city}</td><td colspan='2'>${this.reference}</td><td class='coordblink' data='${cid_3}'>${__x}:${__y}</td>`;
        var res_2;
        var sto_;
        cartstot_ = cartstot_ + this.carts_total;
        shipstot_ = shipstot_ + this.ships_total;
        /** @type {number} */
        for (var i_29 = 0; i_29 < 4; i_29++) {
            switch (i_29) {
                case 0:
                    res_2 = this.wood;
                    woodtot_ = woodtot_ + res_2;
                    sto_ = this.wood_storage;
                    break;
                case 1:
                    res_2 = this.stone;
                    stonetot_ = stonetot_ + res_2;
                    sto_ = this.stone_storage;
                    break;
                case 2:
                    res_2 = this.iron;
                    irontot_ = irontot_ + res_2;
                    sto_ = this.iron_storage;
                    break;
                case 3:
                    res_2 = this.food;
                    foodtot_ = foodtot_ + res_2;
                    sto_ = this.food_storage;
                    break;
            }
            if (res_2 / sto_ < 0.9) {
                /** @type {string} */
                restabb_ = `${restabb_}<td style='color:green;'>${res_2.toLocaleString()}</td><td>${sto_.toLocaleString()}</td>`;
            }
            else {
                if (res_2 / sto_ < 1 && res_2 / sto_ >= 0.9) {
                    /** @type {string} */
                    restabb_ = `${restabb_}<td style='color:#CF6A00;'>${res_2.toLocaleString()}</td><td>${sto_.toLocaleString()}</td>`;
                }
                else {
                    if (res_2 == sto_) {
                        /** @type {string} */
                        restabb_ = `${restabb_}<td style='color:red;'>${res_2.toLocaleString()}</td><td>${sto_.toLocaleString()}</td>`;
                    }
                }
            }
        }
        /** @type {string} */
        restabb_ = `${restabb_}<td>${this.carts_home.toLocaleString()}</td><td>${this.carts_total.toLocaleString()}</td><td>${this.ships_home}</td><td>${this.ships_total}</td><td>${this.score.toLocaleString()}</td></tr>`;
    });
    restabb_ = `${restabb_}</tbody>`;
    $("#restable").html(restabb_);
    $("#restable td").css("text-align", "center");
    /** @type {(Element|null)} */
    var newTableObject_6 = document.getElementById("restable");
    sorttable.makeSortable(newTableObject_6);
    /** @type {string} */
    var tottab_ = `<div id='rsum' class='beigemenutable scroll-pane ava' style='width: 99%;margin-left: 4px;'><table><td>Total wood: </td><td>${woodtot_.toLocaleString()}</td><td>Total stone: </td><td>${stonetot_.toLocaleString()}</td><td>Total iron: </td><td>${irontot_.toLocaleString()}</td><td>Total food: </td><td>${foodtot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_ = `${tottab_}<td>Total carts: </td><td>${cartstot_.toLocaleString()}</td><td>Total ships: </td><td>${shipstot_.toLocaleString()}</td></table></div>`;
    $("#rsum").remove();
    $("#resTab").append(tottab_);
    $("#rsum td").css("text-align", "center");
    $.each(data_45, function () {
        var aa_4 = this.id;
        //	$(`#cn${aa_4}`).click(() => {
        //		$("#organiser").val("all").change();
        //		$("#cityDropdownMenu").val(aa_4).change();
        //	});
    });
}
function GetDonateHeader() {
    return "<thead><tr data='0' class = 'ava'>"
        + "<th>Send</th>"
        + "<th>Name</th><th>Notes</th><th>Coords</th>"
        + "<th>Coords</th>"
        + "<th>Dist</th>"
        + "<th>W needed</th>"
        + "<th>S needed</th>"
        + "<th>Priority</th>"
        + "<th>notes</th>"
        + "<th>Wood</th>"
        + "<th>Stone</th>"
        + "<th>Carts</th>"
        + "<th>Total</th>"
        + "</tr></thead>";
}
/**
 * @param {?} data_45
 * @return {void}
 */
function Distance(__x0, __y0, __x1, __y1) {
    let dx = __x0 - __x1;
    let dy = __y0 - __y1;
    return Math.sqrt(dx * dx + dy * dy);
}
function DistanceC(__a, __x1, __y1) {
    return Distance(__a.x, __a.y, __x1, __y1);
}
function DistanceCC(__a, __b) {
    return Distance(__a.x, __a.y, __b.x, __b.y);
}
function SendDonation() {
    let me = $(this);
    let parent = me.parent().parent();
    let wood = GetFloatValue(parent.find("[id^='donate_wood']"));
    let stone = GetFloatValue(parent.find("[id^='donate_stone']"));
    const carts = GetFloatValue(parent.find("[id^='donate_carts']"));
    const desiredRes = wood + stone;
    let resCap = carts * 1000;
    if (desiredRes > resCap) {
        const fract = resCap / desiredRes;
        stone = Math.floor(stone * fract);
        wood = Math.floor(wood * fract);
    }
    let _cid = parent.data();
    var args = {
        b: stone.toString(), "d": 0, "cid": _cid, "rcid": me.val(), a: wood.toString(), "t": "1", "c": 0
    };
    var req = 'includes/sndTtr.php';
    gamePost(req, {
        cid: _cid, f: encryptJs(req, args)
    });
    $(this).remove();
    //	cid: 17695065
    //	f: pgOAHrJYbl4LLkiRPpgYUBBEvLjIL0l5KrX9w5ayFfHnMetN9rW1bP3aiihA4jTuYtFk+ibyNwK6nSy1Oo6r20ITiFpLB8PDiyr324xc
}
function UpdateDonate(resData, blessData, filter) {
    /** @type {string} */
    let restabb_ = GetDonateHeader() + "<tbody class = 'ava'>";
    /** @type {number} */
    let woodtot_ = 0;
    /** @type {number} */
    let stonetot_ = 0;
    /** @type {number} */
    var cartstot_ = 0;
    $.each(resData, function () {
        let cid_3 = AsNumber(this.id);
        if (filter != null && filter.indexOf(cid_3) == -1) {
            return;
        }
        /** @type {number} */
        let __x = AsNumber(cid_3 % 65536);
        /** @type {number} */
        let __y = AsNumber((cid_3 - __x) / 65536);
        // find closest blessed city
        let closest = null; //["None","None","Avatar","Cyndros",0,"12: 00: 00 ",0,0,0,"None on continent",cid_3,0];
        let closestD = 256 * 256;
        let cont = GetContinent(__x, __y);
        for (let i = 0; i < blessData.a.length; ++i) {
            let bcid = blessData.a[i][10];
            /** @type {number} */
            let tempx = AsNumber(bcid % 65536);
            /** @type {number} */
            let tempy = AsNumber((bcid - tempx) / 65536);
            if (GetContinent(tempx, tempy) !== cont)
                continue;
            let distance = Distance(tempx, tempy, __x, __y);
            if (distance < closestD) {
                closestD = distance;
                closest = blessData.a[i];
            }
        }
        if (closest === null)
            return;
        restabb_ = `${restabb_}<tr data='${cid_3}'><td><button id='donate_rcid' data='${closest[10]}' class='ava' style='color:white;background-color: #3f0896;font-weight:800' >${closest[0]}</button></td>`
            + `<td id='donate_cid' class='chcity ava' data='${cid_3}'>${this.city}</td>`
            + `<td>${this.reference}</td><td class='coordblink' data='${cid_3}'>${__x}:${__y}</td>`
            + `<td class='shcitt ava' data='${closest[10]}' >${closest[10]}</td>`
            + `<td>${closestD}</td>`
            + `<td><input value='${closest[6]}'></input></td>`
            + `<td><input value='${closest[7]}'></input></td>`
            + `<td>${closest[8]}</td>`
            + `<td>${closest[9]}</td>`;
        let res_2;
        let sto_;
        cartstot_ = cartstot_ + this.carts_total;
        /** @type {number} */
        for (let i_29 = 0; i_29 < 2; i_29++) {
            switch (i_29) {
                case 0:
                    res_2 = this.wood;
                    woodtot_ = woodtot_ + res_2;
                    sto_ = this.wood_storage;
                    break;
                case 1:
                    res_2 = this.stone;
                    stonetot_ = stonetot_ + res_2;
                    sto_ = this.stone_storage;
                    break;
            }
            /** @type {string} */
            restabb_ = `${restabb_}<td><input id = 'donate_${(i_29 === 0 ? 'wood' : 'stone')}_${cid_3}' style='color:green;' value='${res_2.toLocaleString()}'></input></td>`;
        }
        /** @type {string} */
        restabb_ = `${restabb_}<td><input id = 'donate_carts_${cid_3}' value='${this.carts_home.toLocaleString()}'></input></td><td>${this.carts_total.toLocaleString()}</td></tr>`;
    });
    restabb_ = `${restabb_}</tbody>`;
    let jqTable = $("#donatetable");
    jqTable.html(restabb_);
    //$("#donatetable td").css("text-align","center");
    /** @type {(Element|null)} */
    let newTableObject_6 = document.getElementById("donatetable");
    sorttable.makeSortable(newTableObject_6);
    /** @type {string} */
    let tottab_ = `<div id='rsum' class='beigemenutable scroll-pane ava'><table><td>Total wood: </td><td>${woodtot_.toLocaleString()}</td><td>Total stone: </td><td>${stonetot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_ = `${tottab_}<td>Total carts: </td><td>${cartstot_.toLocaleString()}</td></table></div>`;
    $("#rsum").remove();
    $("#donateTab").append(tottab_);
    var temp = jqTable.find("button");
    console.log(temp);
    temp.click(this, SendDonation);
    var temp2 = jqTable.find(".chcity");
    console.log(temp2);
    //temp2.click(gspotfunct.chcity);
    //$.each(resData,function() {
    //	let aa_4=this.id;
    //	$(`#cn${aa_4}`).click(() => {
    //		$("#organiser").val("all").change();
    //		$("#cityDropdownMenu").val(aa_4).change();
    //	});
    //});
}
/**
 * @param {?} data_46
 * @param {!Object} notes_3
 * @return {void}
 */
function updatetroops_(data_46, notes_3) {
    /** @type {string} */
    var troopstab_ = `<thead><tr data='0'><th>Name</th><th style='width:150px;'>Notes</th><th>Coords</th><th><div class='${tpicdiv_[8]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[1]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[11]}'></div>(home)</th><th>(Total)</th></th>`;
    troopstab_ = `${troopstab_}<th><div class='${tpicdiv_[14]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[0]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[10]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[9]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[4]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[12]}'></div>(home)</th><th>(Total)</th>`;
    troopstab_ = `${troopstab_}<th><div class='${tpicdiv_[2]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[13]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[7]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[17]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[6]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[15]}'></div>(home)</th><th>(Total)</th>`;
    troopstab_ = `${troopstab_}<th><div class='${tpicdiv_[3]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[5]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv_[16]}'></div>(home)</th><th>(Total)</th><th>TS(home)</th><th>(Total)</th>`;
    troopstab_ = `${troopstab_}</tr></thead><tbody>`;
    /** @type {number} */
    var arbstot_ = 0;
    /** @type {number} */
    var balltot_ = 0;
    /** @type {number} */
    var druidstot_ = 0;
    /** @type {number} */
    var galltot_ = 0;
    /** @type {number} */
    var guardstot_ = 0;
    /** @type {number} */
    var horsetot_ = 0;
    /** @type {number} */
    var praetorstot_ = 0;
    /** @type {number} */
    var priesttot_ = 0;
    /** @type {number} */
    var ramstot_ = 0;
    /** @type {number} */
    var rangerstot_ = 0;
    /** @type {number} */
    var scorptot_ = 0;
    /** @type {number} */
    var scoutstot_ = 0;
    /** @type {number} */
    var senatortot_ = 0;
    /** @type {number} */
    var sorctot_ = 0;
    /** @type {number} */
    var stingerstot_ = 0;
    /** @type {number} */
    var triaritot_ = 0;
    /** @type {number} */
    var vanqstot_ = 0;
    /** @type {number} */
    var warshipstot_ = 0;
    var tshome_;
    var tstot_;
    var thome_;
    var ttot_;
    $.each(data_46, function () {
        /** @type {number} */
        tshome_ = 0;
        /** @type {number} */
        tstot_ = 0;
        var cid_4 = this.id;
        var not_1 = notes_3.notes[notes_3.id.indexOf(cid_4)];
        /** @type {number} */
        var x_81 = AsNumber(cid_4 % 65536);
        /** @type {number} */
        var y_61 = AsNumber((cid_4 - x_81) / 65536);
        troopstab_ = `${troopstab_}<tr data='${cid_4}'><td id='cnt${cid_4}' class='coordblink'>${this.c}</td><td style='width:150px;'>${not_1}</td><td class='coordblink shcitt' data='${cid_4}'>${x_81}:${y_61}</td>`;
        thome_ = this.Arbalist_home;
        ttot_ = this.Arbalist_total;
        arbstot_ = arbstot_ + ttot_;
        /** @type {number} */
        tshome_ = tshome_ + 2 * thome_;
        /** @type {number} */
        tstot_ = tstot_ + 2 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Ballista_home;
        ttot_ = this.Ballista_total;
        balltot_ = balltot_ + ttot_;
        /** @type {number} */
        tshome_ = tshome_ + 10 * thome_;
        /** @type {number} */
        tstot_ = tstot_ + 10 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Druid_home;
        ttot_ = this.Druid_total;
        druidstot_ = druidstot_ + ttot_;
        /** @type {number} */
        tshome_ = tshome_ + 2 * thome_;
        /** @type {number} */
        tstot_ = tstot_ + 2 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Galley_home;
        ttot_ = this.Galley_total;
        galltot_ = galltot_ + ttot_;
        /** @type {number} */
        tshome_ = tshome_ + 100 * thome_;
        /** @type {number} */
        tstot_ = tstot_ + 100 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Guard_home;
        ttot_ = this.Guard_total;
        guardstot_ = guardstot_ + ttot_;
        tshome_ = tshome_ + thome_;
        tstot_ = tstot_ + ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Horseman_home;
        ttot_ = this.Horseman_total;
        horsetot_ = horsetot_ + ttot_;
        tshome_ = tshome_ + 2 * thome_;
        tstot_ = tstot_ + 2 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Praetor_home;
        ttot_ = this.Praetor_total;
        praetorstot_ = praetorstot_ + ttot_;
        tshome_ = tshome_ + 2 * thome_;
        tstot_ = tstot_ + 2 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Priestess_home;
        ttot_ = this.Priestess_total;
        priesttot_ = priesttot_ + ttot_;
        tshome_ = tshome_ + thome_;
        tstot_ = tstot_ + ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Ram_home;
        ttot_ = this.Ram_total;
        ramstot_ = ramstot_ + ttot_;
        tshome_ = tshome_ + 10 * thome_;
        tstot_ = tstot_ + 10 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Ranger_home;
        ttot_ = this.Ranger_total;
        rangerstot_ = rangerstot_ + ttot_;
        tshome_ = tshome_ + thome_;
        tstot_ = tstot_ + ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Scorpion_home;
        ttot_ = this.Scorpion_total;
        scorptot_ = scorptot_ + ttot_;
        tshome_ = tshome_ + 10 * thome_;
        tstot_ = tstot_ + 10 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Scout_home;
        ttot_ = this.Scout_total;
        scoutstot_ = scoutstot_ + ttot_;
        tshome_ = tshome_ + 2 * thome_;
        tstot_ = tstot_ + 2 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Senator_home;
        ttot_ = this.Senator_total;
        senatortot_ = senatortot_ + ttot_;
        tshome_ = tshome_ + thome_;
        tstot_ = tstot_ + ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Sorcerer_home;
        ttot_ = this.Sorcerer_total;
        sorctot_ = sorctot_ + ttot_;
        tshome_ = tshome_ + thome_;
        tstot_ = tstot_ + ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Stinger_home;
        ttot_ = this.Stinger_total;
        stingerstot_ = stingerstot_ + ttot_;
        tshome_ = tshome_ + 100 * thome_;
        tstot_ = tstot_ + 100 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Triari_home;
        ttot_ = this.Triari_total;
        triaritot_ = triaritot_ + ttot_;
        tshome_ = tshome_ + thome_;
        tstot_ = tstot_ + ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Vanquisher_home;
        ttot_ = this.Vanquisher_total;
        vanqstot_ = vanqstot_ + ttot_;
        tshome_ = tshome_ + thome_;
        tstot_ = tstot_ + ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        thome_ = this.Warship_home;
        ttot_ = this.Warship_total;
        warshipstot_ = warshipstot_ + ttot_;
        tshome_ = tshome_ + 400 * thome_;
        tstot_ = tstot_ + 400 * ttot_;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
        /** @type {string} */
        troopstab_ = `${troopstab_}<td>${tshome_.toLocaleString()}</td><td>${tstot_.toLocaleString()}</td></tr>`;
        this['tsHome'] = tshome_;
    });
    troopstab_ = `${troopstab_}</tbody>`;
    $("#troopstable").html(troopstab_);
    $("#troopstable td").css("text-align", "center");
    $("#troopstable td").css("padding-left", "0%");
    /** @type {(Element|null)} */
    var newTableObject_7 = document.getElementById("troopstable");
    sorttable.makeSortable(newTableObject_7);
    /** @type {string} */
    var tottab_1 = "<div id='tsum' class='beigemenutable scroll-pane ava' style='width: 99%;margin-left: 4px;'><table style='font-size: 14px;width: 120%;'><tr><td>Total arbs: </td><td>Total balli: </td><td>Total druids: </td><td>Total galley: </td><td>Total guards: </td><td>Total horses: </td><td>Total praetor: </td><td>Total priest: </td><td>Total rams: </td><td>Total rangers: </td>";
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>Total scorp: </td><td>Total scouts: </td><td>Total senator: </td><td>Total sorc: </td><td>Total stingers: </td><td>Total triari: </td><td>Total vanqs: </td><td>Total warship: </td></tr>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<tr><td>${arbstot_.toLocaleString()}</td><td>${balltot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${druidstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${galltot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${guardstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${horsetot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${praetorstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${priesttot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${ramstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${rangerstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${scorptot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${scoutstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${senatortot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${sorctot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${stingerstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${triaritot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${vanqstot_.toLocaleString()}</td>`;
    /** @type {string} */
    tottab_1 = `${tottab_1}<td>${warshipstot_.toLocaleString()}</td></tr></table></div>`;
    $("#tsum").remove();
    $("#troopsTab").append(tottab_1);
    $.each(data_46, function () {
        var aa_5 = this.id;
        //$(`#cnt${aa_5}`).click(() => {
        //	$("#organiser").val("all").change();
        //	$("#cityDropdownMenu").val(aa_5).change();
        //});
    });
}
//# sourceMappingURL=funky.js.map