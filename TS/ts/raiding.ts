
/**
 * @param {!Object} data_33
 * @return {void}
 */
function openreturnwin_(data_33) {
	$(".toptdinncommtbl1:first").click();
	setTimeout(() => {
		$("#outgoingPopUpBox").hide();
	},300);
	var selectcont_=$("#idleCsel").clone(false);
	selectcont_.attr({
		id: "selcr",
		style: "width:40%;height:28px;font-size:11;border-radius:6px;margin:7px"
	});
	/** @type {string} */
	var returnwin_="<div id='returnAll' style='width:300px;height:320px;background-color: #eeE2CBAC;-moz-border-radius: 10px;-webkit-border-radius: 10px;border-radius: 10px;border: 4px ridge #eeDAA520;position:absolute;right:100px;top:100px; z-index:1000000;'><div class=\"popUpBar\"> <span class=\"ppspan\">Return all troops in all cities</span>";
	/** @type {string} */
	returnwin_=`${returnwin_}<button id="cfunkyX" onclick="$('#returnAll').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div id='returnbody' class="popUpWindow">`;
	/** @type {string} */
	returnwin_=`${returnwin_}</div></div>`;
	/** @type {string} */
	var selecttype_="<select id='selType' class='greensel' style='width:50%;height:28px;font-size:11;border-radius:6px;margin:7px'><option value='1'>Offence and Defense</option><option value='2'>Offence</option><option value='3'>Defense</option></select><br>";
	var selectret_=$("#raidrettimesela").clone(false).attr({
		id: "returnSel",
		style: "width:40%;height:28px;font-size:11;border-radius:6px;margin:7px"
	});
	/** @type {string} */
	var selecttime_="<br><div id='timeblock' style='height:100px; width 95%'><div id='timesel' style='display: none;'><span style='text-align:left;font-weight:800;margin-left:2%;'>Input latest return time:</span><br><table style='width:80%;margin-left:10px'><thead><tr style='text-align:center;'><td>Hr</td><td>Min</td><td>Sec</td><td colspan='2'>Date</td></tr></thead><tbody>";
	/** @type {string} */
	selecttime_=`${selecttime_}<tr><td><input id='returnHr' type='number' style='width: 35px;height: 22px;font-size: 10px;padding: none !important;color: #000;' value='00'></td><td><input id='returnMin' style='width: 35px;height: 22px;font-size: 10px;padding: none !important;color: #000;' type='number' value='00'></td>`;
	/** @type {string} */
	selecttime_=`${selecttime_}<td><input style='width: 35px;height: 22px;font-size: 10px;padding: none !important;color: #000;' id='returnSec' type='number' value='00'></td><td colspan='2'><input style='width:90px;' id='returnDat' type='text' value='00/00/0000'></td></tr></tbody></table></div></div>`;
	/** @type {string} */
	var returnAllgo_="<button id='returnAllGo' style='margin-left:30%; width: 35%;height: 30px !important; font-size: 12px !important; position: static !important;' class='regButton greenb'>Start Return All</button><br>";
	/** @type {string} */
	var nextcity_="<button id='nextCity' style='display: none;margin-left:10%; width: 35%;height: 30px !important; font-size: 12px !important; position: static !important;' class='regButton greenb'>Next City</button>";
	/** @type {string} */
	var returntroops_="<button id='returnTroops' style='display: none;margin-left:10%; width: 35%;height: 30px !important; font-size: 12px !important; position: static !important;' class='regButton greenb'>Return troops</button>";
	var selectlist_=$("#organiser").clone(false).attr({
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
//	$("#returnAllGo").after(nextcity_);
	$("#nextCity").after(returntroops_);
	$("#returnSel").change(() => {
		if($("#returnSel").val()==3) {
			$("#timesel").show();
		} else {
			$("#timesel").hide();
		}
	});
	var j_5;
	var end_6;
	var bb_;
	var cc_;
	var aa_;
	var returncities_={
		cid: [],
		cont: []
	};
	$("#returnAllGo").click(() => {
		if($("#selClist").val()=="all") {
			var i_18;
			for(i_18 in data_33) {
				var cont_1=data_33[i_18].c.co;
				if($("#selcr").val()==56) {
					if($("#selType").val()==1) {
						returncities_.cid.push(data_33[i_18].i);
						returncities_.cont.push(cont_1);
					}
					if($("#selType").val()==2) {
						if(data_33[i_18].tr.indexOf(5)>-1||data_33[i_18].tr.indexOf(6)>-1||data_33[i_18].tr.indexOf(10)>-1||data_33[i_18].tr.indexOf(11)>-1||data_33[i_18].tr.indexOf(12)>-1||data_33[i_18].tr.indexOf(13)>-1||data_33[i_18].tr.indexOf(14)>-1||data_33[i_18].tr.indexOf(16)>-1) {
							returncities_.cid.push(data_33[i_18].i);
							returncities_.cont.push(cont_1);
						}
					}
					if($("#selType").val()==3) {
						if(data_33[i_18].tr.indexOf(1)>-1||data_33[i_18].tr.indexOf(2)>-1||data_33[i_18].tr.indexOf(3)>-1||data_33[i_18].tr.indexOf(4)>-1||data_33[i_18].tr.indexOf(8)>-1||data_33[i_18].tr.indexOf(9)>-1||data_33[i_18].tr.indexOf(15)>-1) {
							returncities_.cid.push(data_33[i_18].i);
							returncities_.cont.push(cont_1);
						}
					}
				}
				if(cont_1==AsNumber($("#selcr").val())) {
					if($("#selType").val()==1) {
						returncities_.cid.push(data_33[i_18].i);
						returncities_.cont.push(cont_1);
					}
					if($("#selType").val()==2) {
						if(data_33[i_18].tr.indexOf(5)>-1||data_33[i_18].tr.indexOf(6)>-1||data_33[i_18].tr.indexOf(10)>-1||data_33[i_18].tr.indexOf(11)>-1||data_33[i_18].tr.indexOf(12)>-1||data_33[i_18].tr.indexOf(13)>-1||data_33[i_18].tr.indexOf(14)>-1||data_33[i_18].tr.indexOf(16)>-1) {
							returncities_.cid.push(data_33[i_18].i);
							returncities_.cont.push(cont_1);
						}
					}
					if($("#selType").val()==3) {
						if(data_33[i_18].tr.indexOf(1)>-1||data_33[i_18].tr.indexOf(2)>-1||data_33[i_18].tr.indexOf(3)>-1||data_33[i_18].tr.indexOf(4)>-1||data_33[i_18].tr.indexOf(8)>-1||data_33[i_18].tr.indexOf(9)>-1||data_33[i_18].tr.indexOf(15)>-1) {
							returncities_.cid.push(data_33[i_18].i);
							returncities_.cont.push(cont_1);
						}
					}
				}
			}
		} else {
			$.each(ppdt.clc,(key_36,value_84) => {
				if(key_36==$("#selClist").val()) {
					/** @type {number} */
					returncities_.cid=value_84;
				}
			});
		}
		//	$("#organiser").val("all").change();
		bb_=$("#returnSel").val();
		if(bb_==3) {
			cc_=`${$("#returnDat").val()} ${$("#returnHr").val()}:${$("#returnMin").val()}:${$("#returnSec").val()}`;
		} else {
			/** @type {number} */
			cc_=0;
		}
		/** @type {number} */
		j_5=0;
		/** @type {number} */
		end_6=returncities_.cid.length;
		aa_=returncities_.cid[j_5];
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
		if(j_5==end_6) {
			alert("Return all complete");
			$("#returnAll").remove();
		} else {
			aa_=returncities_.cid[j_5];
			$("#cityDropdownMenu").val(aa_).change();
		}
	});
}
/**
 * @return {void}
 */
class Boss {
	cid=0;
	lvl=1;
	data=null;
	name:string=null;
	distance=1.0;
	minutes: number;

}

let bossinfo_: any [];
let bosslist_: any[];

function getbossinfo() {

	bossinfo_= [];
	var i_19;
	for(i_19 in wdata.bosses) {
		var wb=wdata.bosses[i_19];
		/** @type {number} */
		var templvl_=AsNumber(wb.substr(1,2))-10;
		/** @type {number} */
		var tempy_3=AsNumber(wb.substr(4,3))-100;
		/** @type {number} */
		var tempx_3=AsNumber(wb.substr(7,3))-100;
		/** @type {number} */
		var cid=tempy_3*65536+tempx_3;
		let boss = {
			cid: (cid),
			lvl: (templvl_),
			data: (wb)
		}
		bossinfo_.push(boss);
	}
}
/**
 * @return {void}
 */
function FormatMinutes(minutes_ :number) {
	return `${Math.floor(minutes_/60)}h ${Math.floor(minutes_%60)}m`
}
function openbosswin_() {

	UpdateResearchAndFaith();
	let _city=GetCity();
	var cont=GetCityContinent(_city);
	bosslist_ = [];

	for(let i_20 in bossinfo_) {
		let boss=bossinfo_[i_20];
		let distance_: number=DistanceC(boss.cid,_city.x,_city.y);
		if((_city.th[2]||_city.th[3]||_city.th[4]||_city.th[5]||_city.th[6]||_city.th[8]||_city.th[9]||_city.th[10]||_city.th[11])&&_city.th[14]==0) {
			if(boss.cid.cont==cont) {
				if(_city.th[2]||_city.th[3]||_city.th[4]||_city.th[5]||_city.th[6]) {
			/** @type {number} */
boss.minutes=distance_*ttspeed[2]/ttSpeedBonus[2];
/** @type {string} */
				}
				if(_city.th[8]||_city.th[9]||_city.th[10]||_city.th[11]) {
/** @type {number} */
						boss.minutes=distance_*ttspeed[8]/ttSpeedBonus[8];
/** @type {string} */
				}
				boss.distance =(RoundTo2Digits(distance_));
				
				bosslist_.push(boss);
			}
		}
		if(distance_<220) {
			if(_city.th[14]||_city.th[15]||_city.th[16]) {
/** @type {number} */
					boss.minutes=distance_*ttspeed[14]/ttSpeedBonus[14];
/** @type {string} */
				
				boss.distance=(RoundTo2Digits(distance_));
				bosslist_.push(boss);
			}
		}
	}
/** @type {string} */
var bosswin_="<table id='bosstable' class='beigetablescrollp ava sortable'><thead><tr><th>Coordinates</th><th>Level</th><th>Continent</th><th>Travel Time</th><th id='hdistance'>Distance</th></tr></thead>";
/** @type {string} */
		bosswin_=`${bosswin_}<tbody>`;
	for(let i_20 in bosslist_) {
		let boss=bosslist_[i_20];
	/** @type {string} */
			bosswin_=`${bosswin_}<tr id='bossline${boss.cid}' class='dunginf'><td id='cl${boss.cid}' class='coordblink shcitt' data='${boss.cid}' style='text-align: center;'>${boss.cid.x}:${boss.cid.y}</td>`;
/** @type {string} */
			bosswin_=`${bosswin_}<td style='text-align: center;font-weight: bold;'>${boss.lvl}</td><td style='text-align: center;'>${boss.cont()}</td>`;
/** @type {string} */
			bosswin_=`${bosswin_}<td style='text-align: center;'>${FormatMinutes(boss.minutes)}</td><td style='text-align: center;'>${boss.distance}</td></tr>`;
	}
/** @type {string} */
		bosswin_=`${bosswin_}</tbody></table></div>`;
/** @type {string} */
	var idle_="<table id='idleunits' class='beigetablescrollp ava'><tbody><tr><td style='text-align: center;'><span>Idle troops:</span></td>";
	for(let i=0;i<_city.th.length;++i) {
	/** @type {!Array} */
var notallow_=[0,1,7,12,13];
		if(notallow_.indexOf(i)==-1) {
			if(_city.th[i]>0) {
		/** @type {string} */
					idle_=`${idle_}<td><div class='${tpicdiv[i]}' style='text-align: right;'></div></td><td style='text-align: left;'><span id='thbr${i}' style='text-align: left;'>${_city.th[i]}</span></td>`;
			}
		}
	}
/** @type {string} */
		idle_=`${idle_}</tbody></table>`;
	$("#bossbox").html(bosswin_);
	$("#idletroops").html(idle_);
/** @type {(Element|null)} */
let newTableObject_2=document.getElementById("bosstable");
	sorttable.makeSortable(newTableObject_2);
	setTimeout(() => {
		$("#hdistance").trigger("click");
		setTimeout(() => {
			$("#hdistance").trigger("click");
		},100);
	},100);
	bosslist_.forEach((it) => {
		$(`#cl${it.cid}`).click(() => {
			setTimeout(() => {
				$("#raidDungGo").trigger("click");
			},500);
		});
	});
		
}
/**
 * @return { void}
*/
function bossele_() {
	let bopti_=$("#cityplayerInfo div table tbody");
	/** @type {string} */
	let bzTS_="<tr><td>Vanq:</td><td></td></tr><tr><td>R/T:</td><td></td></tr><tr><td>Ranger:</td><td></td></tr><tr><td>Triari:</td><td></td></tr><tr><td>Arb:</td><td></td></tr><tr><td>horse:</td><td></td></tr><tr><td>Sorc:</td><td></td></tr><tr><td>Druid:</td><td></td></tr>";
	/** @type {string} */
	bzTS_=`${bzTS_}<tr><td>Prietess:</td><td></td></tr><tr><td>Praetor:</td><td></td></tr><tr><td>Scout:</td><td></td></tr><tr><td>Galley:</td><td></td></tr><tr><td>Stinger:</td><td></td></tr><tr><td>Warships:</td><td></td></tr>`;
	bopti_.append(bzTS_);
}
/**
 * @return {void}
 */
function recallraidl100_() {
	/**
	 * @return {void}
	*/
	let _city=GetCity();
	function loop_1() {
		var trlist_=$(`#commandtable tbody tr:nth-child(${l_2})`);
		var lvlprog_=$(trlist_).find(".commandinntabl tbody tr:nth-child(3) td:nth-child(1) span:nth-child(1)").text();
		var splitlp_=lvlprog_.split("(");
		/** @type {number} */
		var Dungeon_lvl_=AsNumber(splitlp_[0].match(/\d+/gi));
		/** @type {number} */
		var Dungeion_prog_=AsNumber(splitlp_[1].match(/\d+/gi));
		var dungeon_=splitlp_[0].substring(0,splitlp_[0].indexOf(","));
		if(dungeon_==="Mountain Cavern") {
			/** @type {!Array} */
			loot_=mountain_loot;
		} else {
			/** @type {!Array} */
			loot_=other_loot;
		}
		/** @type {number} */
		var total_loot_c_=Math.ceil(loot_[AsNumber(Dungeon_lvl_)-1]*(1-AsNumber(Dungeion_prog_)/100+1));
		var Unitno_=$(trlist_).find(".commandinntabl tbody tr:nth-child(1) td:nth-child(2) span").text();
		var temp7_=Unitno_.match(/[\d,]+/g);
		/** @type {number} */
		var Units_raiding_=AsNumber(temp7_[0].replace(",",""));
		/** @type {number} */
		var lootperraid_=lootpertroop_*Units_raiding_;
		/** @type {number} */
		var percentage_ofloot_=Math.ceil(lootperraid_/total_loot_c_*100);
		if(AsNumber(percentage_ofloot_)<90) {
			jQuery(trlist_).find(".commandinntabl tbody tr:nth-child(2) td:nth-child(1) table tbody tr td:nth-child(2)")[0].click();
			$("#raidrettimesela").val(1).change();
			setTimeout(() => {
				jQuery("#doneOG")[0].click();
			},300);
			setTimeout(() => {
				$("#outgoingPopUpBox").hide();
			},500);
		}
		l_2++;
		if(l_2<m_) {
			setTimeout(loop_1,1000);
		}
	}
	var loot_;
	var total_;
	/** @type {number} */
	var total_number_=0;
	/** @type {number} */
	var total_lootz_=0;
	/** @type {number} */
	var i_21=0;
	var x_75;
	for(x_75 in _city.th) {
		/** @type {number} */
		total_=AsNumber(_city.th[x_75]);
		/** @type {number} */
		total_number_=total_number_+total_*AsNumber(TS_type_[i_21]);
		/** @type {number} */
		total_lootz_=total_lootz_+total_*AsNumber(ttloot_[i_21]);
		/** @type {number} */
		i_21=i_21+1;
		if(i_21===17) {
			break;
		}
	}
	/** @type {number} */
	var lootpertroop_=total_lootz_/total_number_;
	/** @type {number} */
	var l_2=1;
	/** @type {number} */
	var m_=AsNumber($("#commandtable tbody").length);
	loop_1();
}
/**
 * @return {void}
 */
function carrycheck_() {
	var loot_1;
	var total_1;
	/** @type {number} */
	var total_number_1=0;
	/** @type {number} */
	var total_lootx_=0;
	/** @type {number} */
	var i_22=0;
	let _city=GetCity();
	for(let x_76 in _city.th) {
		/** @type {number} */
		total_1=AsNumber(_city.th[x_76]);
		/** @type {number} */
		total_number_1=total_number_1+total_1*AsNumber(TS_type_[i_22]);
		/** @type {number} */
		total_lootx_=total_lootx_+total_1*AsNumber(ttloot_[i_22]);
		/** @type {number} */
		i_22=i_22+1;

	}
	/** @type {number} */
	var lootpertroop_1=total_lootx_/total_number_1;
	/** @type {number} */
	i_22=1;
	for(;i_22<$("#commandtable tbody").length;i_22++) {
		var trlist_1=$(`#commandtable tbody tr:nth-child(${i_22})`);
		var lvlprog_1=$(trlist_1).find(".commandinntabl tbody tr:nth-child(3) td:nth-child(1) span:nth-child(1)").text();
		var splitlp_1=lvlprog_1.split("(");
		if(splitlp_1.length===1) {
			continue;
		}
		/** @type {number} */
		var dungeonLevel=AsNumber(splitlp_1[0].match(/\d+/gi));
		/** @type {number} */
		var dungeonProg=AsNumber(splitlp_1[1].match(/\d+/gi));
		var dungeonType=splitlp_1[0].substring(0,splitlp_1[0].indexOf(","));
		if(dungeonType==="Mountain Cavern") {
			/** @type {!Array} */
			loot_1=mountain_loot;
		} else {
			/** @type {!Array} */
			loot_1=other_loot;
		}
		/** @type {number} */
		var total_loot_c_1=Math.ceil(loot_1[AsNumber(dungeonLevel)-1]*(1-AsNumber(dungeonProg)/100+1));
		var Unitno_1=$(trlist_1).find(".commandinntabl tbody tr:nth-child(1) td:nth-child(2) span").text();
		var temp7_1=Unitno_1.match(/[\d,]+/g);
		/** @type {number} */
		var Units_raiding_1=AsNumber(temp7_1[0].replace(",",""));
		/** @type {number} */
		var lootperraid_1=lootpertroop_1*Units_raiding_1;
		/** @type {number} */
		var percentage_ofloot_1=Math.ceil(lootperraid_1/total_loot_c_1*100);
		$(trlist_1).find(".commandinntabl tbody tr:nth-child(3) td:nth-child(2)").attr("rowspan",1);
		$(trlist_1).find(".commandinntabl tbody tr:nth-child(4) td:nth-child(1)").attr("colspan",1);
		$(trlist_1).find(".commandinntabl tbody tr:nth-child(4)").append('<td colspan="1" class="bottdinncommtb3" style="text-align:right"></td>');
		$(trlist_1).find(".commandinntabl tbody tr:nth-child(4) td:nth-child(2)").text(`Carry:${percentage_ofloot_1}%`);
	}
}

function GetCarry() {
	return LocalStoreAsFloat('carry',1.02);
}
var countOverride=0;
var raidCount=1;
/**
 * @param {number} total_loot_1
 * @return {void}
 */
function carry_percentage_(total_loot_1) {
	/** @type {number} */
	var home_loot_=0;
	/** @type {!Array} */
	var km_: number[]=[];
	let _city=GetCity();
	for(let x_77 in _city.th) {
		/** @type {number} */
		var home_=AsNumber(_city.th[x_77]);
		/** @type {number} */
		home_loot_=home_loot_+home_*ttloot_[x_77];
		km_.push(home_);
		/** @type {number} */

	}
	/** @type {number} */
	var scaledLoot=Math.ceil(total_loot_1*GetCarry());
	/** @type {number} */
	raidCount=countOverride>0? countOverride:Math.max(1,Math.floor(home_loot_/scaledLoot));
	$("#WCcomcount").val(raidCount);
	for(let i_23 in km_) {
		if(km_[i_23]!==0) {
			/** @type {number} */
			km_[i_23]=Math.floor(km_[i_23]/raidCount);
			$(`#rval${i_23}`).val(km_[i_23]);
			if(km_[14]) {
				$("#rval14").val("0");
			}
		}
	}


	carry_percentage_2(total_loot_1);
}

function carry_percentage_2(total_loot_1) {
	/** @type {number} */
	var troop_loot_=0;
	$(".tninput").each(function() {
		var trpinpid_=$(this).attr("id");
		let TSnum_=GetFloatValue($(this));
		/** @type {number} */
		var ttttt_=AsNumber(trpinpid_.match(/\d+/gi));
		troop_loot_=troop_loot_+TSnum_*ttloot_[ttttt_];
	});
	/** @type {number} */
	var percentage_loot_takable_=Math.ceil(troop_loot_/total_loot_1*100);
	$("#dungloctab").find(".addraiwc td:nth-child(3)").text(`carry:${percentage_loot_takable_}%`);
}
/**
 * @return {void}
 */
function getDugRows_() {
	$('#dungloctab th:contains("Distance")').click();
	$('#dungloctab th:contains("Distance")').click();
	$("#dungloctab tr").each(function() {
		var buttont_=$(this).find("button");
		var buttonid_=buttont_.attr("id");
		var temp3_=$(this).find("td:nth-child(2)").text();
		var temp4_=$(this).find("td:nth-child(3)").text();
		var tempz2_=temp3_.split(" ");
		var temp1_=tempz2_[1];
		var temp2_=temp4_.match(/\d+/gi);
		var tempz1_=tempz2_[2];
		if(buttonid_) {
			buttont_.attr("lvl",temp1_);
			buttont_.attr("prog",ToFloat(temp4_));
			buttont_.attr("type",tempz1_);
		}
		$(buttont_).click(function() {
			var loot1_;
			/** @type {number} */
			var countz_=AsNumber($(".splitRaid").children("option").length);
			if(countz_>1) {
				/** @type {number} */
				//		countOverride=countz_-1;
			} else {
				/** @type {number} */
				//	countOverride=countz_;
			}

			var dunglvl_=$(this).attr("lvl");
			var progress_=$(this).attr("prog");
			var type_dung_=$(this).attr("type");
			if(type_dung_==="Mountain") {
				/** @type {!Array} */
				loot1_=mountain_loot;
			} else {
				/** @type {!Array} */
				loot1_=other_loot;
			}
			/** @type {number} */
			var total_loot_1=Math.ceil(loot1_[AsNumber(dunglvl_)-1]*(2-AsNumber(progress_)/100)*1.02);
			$("#dungloctab").find(".addraiwc td:nth-child(4)").html("<button id='raid115' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>115%</button><button id='raid108' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>108%</button>");
			$("#dungloctab").find(".addraiwc td:nth-child(2)").html("<button id='raid100' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>100%</button><button id='raid125' style='padding: 2px; border-radius: 4px;' class='greenb shRnTr'>125%</button>");

			$("#raid125").click(() => {
				localStorage['carry']=1.25;

				carry_percentage_(total_loot_1);
			}
			);
			$("#raid115").click(() => {
				localStorage['carry']=1.15;

				carry_percentage_(total_loot_1);
			}
			);
			$("#raid108").click(() => {
				localStorage['carry']=1.08;

				carry_percentage_(total_loot_1);
			}
			);
			$("#raid100").click(() => {
				localStorage['carry']=0.95;

				carry_percentage_(total_loot_1);
			}
			);

			setTimeout(() => {
				carry_percentage_(total_loot_1);
			},100);
			setTimeout(() => {
				carry_percentage_2(total_loot_1);
			},1000);
			$(".tninput").change(() => {
				carry_percentage_2(total_loot_1);
			});
			$("#WCcomcount").on("change",() => {
				if($("#rval14").val()) {
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
var nearesthub;


function setnearhub(j_12) {
	

	var res = [0, 0, 0, 0, 1, 130000, 130000, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 300000, 300000, 300000, 400000];
	var aa = D6.mo;
	if (j_12 != undefined) {
		if ($("#addnotes").prop("checked") == true) {
			$("#CNremarks").val(remarksl_[j_12]);
			$("#citynotestextarea").val(notesl_[j_12]);
			setTimeout(() => {
				jQuery("#citnotesaveb")[0].click();
			}, 100);
		}
		if ($("#addtroops").prop("checked") == true) {
			var k_3;
			for (k_3 in troopcounl_[j_12]) {
				aa[9 + AsNumber(k_3)] = troopcounl_[j_12][k_3];
			}
		}
	}

	var hubs = { cid: [], distance: [] };
	$.each(ppdt.clc, function (key, value) {
		if (key == $("#selHub").val()) {
			hubs.cid = value;
		}
	});
	for (var i in hubs.cid) {
		var tempx = Number(hubs.cid[i] % 65536);
		var tempy = Number((hubs.cid[i] - tempx) / 65536);
		hubs.distance.push(Math.sqrt((tempx - D6.x) * (tempx - D6.x) + (tempy - D6.y) * (tempy - D6.y)));
	}
	var mindist = Math.min.apply(Math, hubs.distance);
	nearesthub = hubs.cid[hubs.distance.indexOf(mindist)];
	if ($("#addwalls").prop("checked") == true) {
		aa[26] = 1;
	}
	if ($("#addtowers").prop("checked") == true) {
		aa[27] = 1;
	}
	if ($("#addbuildings").prop("checked") == true) {
		aa[51] = [1, $("#cablev").val() as number];
		aa[68] = [1, 10];
		aa[69] = [1, 10];
		aa[70] = [1, 10];
		aa[71] = [1, 10];
		aa[1] = 1;
	}
	res[14] = nearesthub;
	res[15] = nearesthub;
	res[5] = $("#woodin").val() as number;
	res[6] = $("#stonein").val() as number;
	res[7] = $("#ironin").val() as number;
	res[8] = $("#foodin").val() as number;
	for (var k in res) {
		aa[28 + Number(k)] = res[k];
	}
	var dat = { a: JSON.stringify(aa), b: D6.cid };
	jQuery.ajax( { url: 'includes/mnio.php', type: 'POST',  data: dat });
}
/**
 * @return {void}
 */
function setinfantry_() {
	let _city=GetCity();
	var res_1=[0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000];
	var aa_2=_city.mo;
	var hubs_1={
		cid: [],
		distance: []
	};
	$.each(ppdt.clc,(key_43,value_91) => {
		if(key_43==$("#selHub").val()) {
			/** @type {number} */
			hubs_1.cid=value_91;
		}
	});
	var i_26;
	for(i_26 in hubs_1.cid) {
		/** @type {number} */
		var tempx_5=AsNumber(hubs_1.cid[i_26]%65536);
		/** @type {number} */
		var tempy_5=AsNumber((hubs_1.cid[i_26]-tempx_5)/65536);
		hubs_1.distance.push(Math.sqrt((tempx_5-_city.x)*(tempx_5-_city.x)+(tempy_5-_city.y)*(tempy_5-_city.y)));
	}
	/** @type {number} */
	var mindist_1=Math.min(...hubs_1.distance);
	nearesthub=hubs_1.cid[hubs_1.distance.indexOf(mindist_1)];
	if($("#addwalls").prop("checked")==true) {
		/** @type {number} */
		aa_2[26]=1;
	}
	if($("#addtowers").prop("checked")==true) {
		/** @type {number} */
		aa_2[27]=1;
	}
	if($("#addbuildings").prop("checked")==true) {
		/** @type {!Array} */
		aa_2[51]=[1,GetFloatValue($("#cablev"))];
		/** @type {!Array} */
		aa_2[60]=[1,10];
		/** @type {!Array} */
		aa_2[62]=[1,10];
		/** @type {!Array} */
		aa_2[68]=[1,10];
		/** @type {!Array} */
		aa_2[69]=[1,10];
		/** @type {!Array} */
		aa_2[70]=[1,10];
		/** @type {!Array} */
		aa_2[71]=[1,10];
		/** @type {!Array} */
		aa_2[73]=[1,10];
		/** @type {number} */
		aa_2[1]=1;
	}
	res_1[14]=nearesthub;
	res_1[15]=nearesthub;
	res_1[5]=$("#woodin").val() as number;
	res_1[6]=$("#stonein").val() as number;
	res_1[7]=$("#ironin").val() as number;
	res_1[8]=$("#foodin").val() as number;
	var k_1;
	for(k_1 in res_1) {
		aa_2[28+AsNumber(k_1)]=res_1[k_1];
	}
	var dat_2={
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


//function GetRecentTabHTML() {
//	var rv=`<thead><tr data="0">`;
//	for(var key in defaultMru) {
//		rv+=`<th>${key}</th>`;
//	}
//	return `${rv}</tr></thead>`;
//}
/**
 * @return {void}
 */
function opensumwin_() {
	/** @type {boolean} */
	sum_=false;
	/** @type {string} */
	var sumwin_ ="<div id='sumWin' style='width:60%;height:50%;left: 360px; display: block; z-index: 2000;' class='obscuretop'><div id='popsum' class='popUpBar'><span class=\"ppspan\">Cities Summaries</span> <button id=\"sumX\" onclick=\"$('#sumWin').hide();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div class=\"popUpWindow\" style='height:100%'>";
	/** @type {string} */
	sumwin_=`${sumwin_}<div id='sumdiv' class='beigetabspopup ava' style='background:none;border: none;padding: 0px;height:74%;'><ul id='sumtabs' role='tablist'><li role='tab'><a href='#resTab' role='presentation'>Resources</a></li>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<li role='tab'><a href='#troopsTab' role='presentation'>Troops</a></li><li role='tab'><a href='#raidTab' role='presentation'>Raids</a></li><li role='tab'><a href='#raidoverTab' role='presentation'>Raids Overview</a></li>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<li role='tab'><a href='#supportTab' role='presentation'>Support</a></li></ul>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div id='resTab'><button id='resup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:100%;margin-left:4px;' ><table" id='restable'>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<thead><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Wood</th><th>(Storage)</th><th>Stone</th><th>(Storage)</th><th>Iron</th><th>(Storage)</th><th>Food</th><th>(Storage)</th><th>Carts</th><th>(Total)</th><th>Ships</th><th>(Total)</th><th>Score</th></thead></table></div></div>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div id='troopsTab'><button id='troopsup' class='greenb' style='font-size:14px;border-radius:6px;margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div  class='beigemenutable scroll-pane ava' style='width:99%;height:95%;margin-left:4px;'><table  id='troopstable' style='width:250%'>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<thead><tr data='0'><th>Name</th><th style='width:150px;'>Notes</th><th>Coords</th><th><div class='${tpicdiv[8]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[1]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[11]}'></div>(home)</th><th>(Total)</th></th>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<th><div class='${tpicdiv[14]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[0]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[10]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[9]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[4]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[12]}'></div>(home)</th><th>(Total)</th>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<th><div class='${tpicdiv[2]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[13]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[7]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[17]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[6]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[15]}'></div>(home)</th><th>(Total)</th>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<th><div class='${tpicdiv[3]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[5]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[16]}'></div>(home)</th><th>(Total)</th><th>TS(home)</th><th>(Total)</th>`;
	/** @type {string} */
	sumwin_=`${sumwin_}</tr></thead></table></div></div>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div id='raidTab'><button id='raidup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>AsNumber of reports to show:</span><select id='raidsturnc' class='greensel'><option value='100'>100</option><option value='500'>500</option><option value='1000'>1000</option><option value='10000'>10000</option></select>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='raidtable'>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<thead><th>Report</th><th>Type</th><th>Cavern progress</th><th>losses</th><th>Carry</th><th>Date</th><th>Origin</th></thead></table></div></div>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div id='raidoverTab'><button id='raidoverup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:100%;margin-left:4px;' ><table  id='raidovertable'>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<thead><th></th><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Raids</th><th>Out</th><th>In</th><th>Raiding TS</th><th>Home TS</th><th>Resources</th></thead></table></div></div>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div id='supportTab'><button id='supportup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='supporttable'>`;
	/** @type {string} */
	sumwin_=`${sumwin_}<thead><th></th><th>Player</th><th>City</th><th>Coords</th><th>Alliance</th><th>TS supporting</th><th>TS sending</th><th>TS returning</th></thead></table></div></div>`;

	//sumwin_=`${sumwin_}<div id='recentTab'><button id='recentup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button>`;
	///** @type {string} */
	//sumwin_=`${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='recenttable'>`;
	///** @type {string} */
	//sumwin_=`${sumwin_+GetRecentTabHTML()}</table></div></div>`;
	///** @type {string} */
	//sumwin_=`${sumwin_}<div id='donateTab'><button id='donateup' class='greenb' style='font-size:14px;border-radius:6px; margin:4px;'>Update</button><span style='margin-left:50px;'>Show cities from: </span>`;
	///** @type {string} */
	//sumwin_=`${sumwin_}<div class='beigemenutable scroll-pane ava' style='width:99%;height:110%;margin-left:4px;' ><table  id='donatetable' class='ava'>`;
	///** @type {string} */
	//sumwin_=`${sumwin_}${GetDonateHeader()}</table></div></div>`;
	///** @type {string} */
	sumwin_=`${sumwin_}</div></div>`;
	$("#reportsViewBox").after(sumwin_);
	$("#sumWin").draggable({
		handle: ".popUpBar",
		containment: "window",
		scroll: true
	});
	$("#sumWin").resizable();
	$(".popUpBar").click(function() {
		if($(this).parent().attr("id")=="sumWin") {
			setTimeout(() => {
				$("#sumWin").css("z-index",4001);
			},200);
		} else {
			setTimeout(() => {
				$("#sumWin").css("z-index",3000);
			},200);
		}
	});
	$("#sumdiv").tabs();
	var selres_=$("#organiser").clone(false).attr({
		id: "selRes",
		style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
	});


	var seltroops_=$("#organiser").clone(false).attr({
		id: "selTroops",
		style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
	});
	var selraids_=$("#organiser").clone(false).attr({
		id: "selRaids",
		style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
	});
	//var selDonate_=$("#organiser").clone(false).attr({
	//	id: "selDonate",
	//	style: "height: 30px;width:150px;font-size:14px;border-radius:6px;margin:7px"
	//});
	$("#resup").next().after(selres_);
	$("#troopsup").next().after(seltroops_);
	$("#raidoverup").next().after(selraids_);
//	$("#donateup").next().after(selDonate_);
	$("#selTroops").val("all").change();
	$("#selRes").val("all").change();
	$("#selRaids").val("all").change();
//	$("#selDonate").val("all").change();

	$("#resup").click(() => {
		$("#selRes").val("all").change();
		OverviewPost("overview/citover.php",null,
			function(data_35) {
				/** @type {*} */
				var sumres_=(data_35);
				updateres_(sumres_);
			});
	});
	//$("#donateup").click(() => {
	//	let filtered=null;
	//	var listName=$("#selDonate").val();
	//	$.each(ppdt.clc,(_listName,_list) => {
	//		if(listName==_listName) {
	//			/** @type {!Object} */
	//			filtered=_list;
	//			return;
	//		}
	//	});
	//	OverviewPost("overview/citover.php",null,
	//		function(sumres_) {
	//			OverviewPost("overview/bleover.php",null,
	//				function(bleover) {

	//					UpdateDonate(sumres_,bleover,filtered);
	//				});
	//		});
	//});
	$("#troopsup").click(() => {
		$("#selTroops").val("all").change();
		var notes_={
			id: [],
			notes: []
		};
		OverviewPost(
			"overview/citover.php",
			null,
			// async false,
			function success_2(data_36) {
				/** @type {*} */
				var sumres_1=(data_36);
				$.each(sumres_1,function() {
					notes_.id.push(this.id);
					notes_.notes.push(this.reference);
				});
				OverviewPost(
					"overview/trpover.php",
					null,
					// async false,
					function success_3(data_37) {
						/** @type {*} */
						var troopsres_=(data_37);
						updatetroops_(troopsres_,notes_);
					}
				);
			}
		);
	});
	$("#raidup").click(() => {
		OverviewPost(
			"overview/rreps.php",
			// async false,
			null,
			function success_4(data_38) {
				/** @type {*} */
				var raids_=(data_38);
				updateraids_(raids_,$("#raidsturnc").val());
			}
		);
	});
	$("#raidoverup").click(() => {
		var notes_={
			id: [],
			notes: []
		};
		OverviewPost(
			"overview/citover.php",
			// async false,
			null,
			function success_5(data_39) {
				/** @type {*} */
				var sumres_2=(data_39);
				$.each(sumres_2,function() {
					notes_.id.push(this.id);
					notes_.notes.push(this.reference);
				});
				// Extra work just to get tsHome
				OverviewPost(
					"overview/trpover.php",
					null,		// async false,
					function success_3(data_37) {
						/** @type {*} */
						var troopsres_=(data_37);
						updatetroops_(troopsres_,notes_);
						OverviewPost(
							"overview/graid.php",
							null,
							// async false,
							function success_6(data_40) {
								/** @type {*} */
								var raids_=(data_40);
								updateraidover_(raids_,notes_,troopsres_);
							}
						);

					}
				);
			}
		);
	});
	$("#supportup").click(() => {
		OverviewPost(
			"overview/reinover.php",
			null,
			function success_7(data_41) {
				/** @type {*} */
				var support_=(data_41);
				updatesupport_(support_);
			}
		);
	});
	//$("#recentup").click(() => {
	//	updaterecent_();
	//});
	/** @type {!Array} */
	var citylist_: number[]=[];
	$("#selTroops").change(() => {
		if($("#selTroops").val()=="all") {
			$("#troopstable tr").each(function() {
				$(this).show();
			});
		} else {
			$.each(ppdt.clc,(key_44,value_92) => {
				if(key_44==$("#selTroops").val()) {
					/** @type {!Object} */
					citylist_=value_92;
				}
			});
			$("#troopstable tr").each(function() {
				if(citylist_.indexOf(AsNumber($(this).attr("data")))>-1) {
					$(this).show();
				} else {
					if(AsNumber($(this).attr("data"))!=0) {
						$(this).hide();
					}
				}
			});
		}
	});
	$("#selRes").change(() => {
		if($("#selRes").val()=="all") {
			$("#restable tr").each(function() {
				$(this).show();
			});
		} else {
			$.each(ppdt.clc,(key_45,value_93) => {
				if(key_45==$("#selRes").val()) {
					/** @type {!Object} */
					citylist_=value_93;
				}
			});
			$("#restable tr").each(function() {
				if(citylist_.indexOf(AsNumber($(this).attr("data")))>-1) {
					$(this).show();
				} else {
					if(AsNumber($(this).attr("data"))!=0) {
						$(this).hide();
					}
				}
			});
		}
	});
	$("#selRaids").change(() => {
		if($("#selRsaids").val()=="all") {
			$("#raidovertable tr").each(function() {
				$(this).show();
			});
		} else {
			$.each(ppdt.clc,(key_46,value_94) => {
				if(key_46==$("#selRaids").val()) {
					/** @type {!Object} */
					citylist_=value_94;
				}
			});
			$("#raidovertable tr").each(function() {
				if(citylist_.indexOf(AsNumber($(this).attr("data")))>-1) {
					$(this).show();
				} else {
					if(AsNumber($(this).attr("data"))!=0) {
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
function updateraidover_(data_42,notes_2,tsHome_) {
	/** @type {string} */
	var raidovertab_="<thead><tr data='0'><th>Return</th><th>Return</th><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Raids</th><th>Out</th><th>In</th><th>Raiding TS</th><th>Home TS</th><th>Resources</th></tr></thead><tbody>";
	$.each(data_42.a,function() {
		var cid_1=this[0];
		var not_=notes_2.notes[notes_2.id.indexOf(cid_1)];
		var tsHome=0;
		for(var i in tsHome_) {
			if(tsHome_[i].id===cid_1) {
				tsHome=tsHome_[i].tsHome;
				break;

			}

		}

		/** @type {number} */
		var x_79=AsNumber(cid_1%65536);
		/** @type {number} */
		var y_59=AsNumber((cid_1-x_79)/65536);
		raidovertab_=`${raidovertab_}<tr data='${cid_1}'><td><button style='height: 20px;padding-top: 3px;border-radius:6px;' class='greenb recraid' data='${cid_1}'>Now!</button></td><td><button style='height: 20px;padding-top: 3px;border-radius:6px;' class='greenb recraid2' data='${cid_1}'>Maybe</button></td>`;
		/** @type {string} */
		raidovertab_=`${raidovertab_}<td data='${cid_1}' class='coordblink raidclink'>${this[1]}</td><td colspan='2'>${not_}</td><td class='coordblink shcitt' data='${cid_1}'>${x_79}:${y_59}</td><td>${this[3]}</td><td>${this[6]}</td><td>${this[5]}</td><td>${this[4].toLocaleString()}</td><td>${tsHome.toLocaleString()}</td>`;
		/** @type {string} */
		raidovertab_=`${raidovertab_}<td>${(this[7]+this[8]+this[9]+this[10]+this[11]).toLocaleString()}</td></tr>`;
	});
	raidovertab_=`${raidovertab_}</tbody>`;
	$("#raidovertable").html(raidovertab_);
	$("#raidovertable td").css("text-align","center");
	/** @type {(Element|null)} */
	var newTableObject_3=document.getElementById("raidovertable");
	sorttable.makeSortable(newTableObject_3);
	$(".raidclink").click(function() {
		var aa_3=$(this).attr("data");
		//		$("#organiser").val("all").change();
		//			$("#cityDropdownMenu").val(aa_3);
				gspotfunct.chcity(aa_3);
	});
	$(".recraid").click(function() {

		var id_5=$(this).attr("data");
		var dat_3={
			a: AsNumber(id_5)
		};
		OverviewPost("overview/rcallall.php",dat_3);
		//AjaxPrefilterRestore();
		$(this).remove();

	});
	$(".recraid2").click(function() {

		var req=UrOA;
		var id_5=$(this).attr("data");

		gamePost(req,{
			a: encryptJs(req,{
				a: AsNumber(id_5),
				c: "0",
				b: "1"
			})
		});

		$(this).remove();
	});
}
var UrOA="includes/UrOA.php";
var ekeys={
	"includes/sndRad.php": "Sx23WW99212375Daa2dT123ol",
	"includes/gRepH2.php": "g3542RR23qP49sHH",
	"includes/bTrp.php": "X2UsK3KSJJEse2",
	"includes/gC.php": "X2U11s33S2375ccJx1e2",
	"includes/rMp.php": "X22ssa41aA1522",
	"includes/gSt.php": "X22x5DdAxxerj3",
	"includes/gWrd.php": "Addxddx5DdAxxer23752wz",
	"includes/UrOA.php": "Rx3x5DdAxxerx3", // {"a":20971788,"c":0,"b":"1"}
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
function gamePost(req,data) {
	//  AjaxPrefilterGame();
	$.post(req,data);
	//   AjaxPrefilterRestore();
}


/**
 * @param {?} data_43
 * @return {void}
 */
function updatesupport_(data_43) {
	/** @type {string} */
	var supporttab_="<thead><th></th><th>Player</th><th>City</th><th>Coords</th><th>Alliance</th><th>TS supporting</th><th>TS sending</th><th>TS returning</th></thead><tbody>";

	$.each(data_43,function() {
		var tid_3=this[9][0][1];
		supporttab_=`${supporttab_}<tr><td><button class='greenb expsup' style='height: 20px;padding-top: 3px;border-radius:6px;'>Expand</button><button data='${tid_3}' class='greenb recasup' style='height: 20px;padding-top: 3px;border-radius:6px;'>Recall all</button>`;
		/** @type {string} */
		supporttab_=`${supporttab_}</td><td class='playerblink'>${this[0]}</td><td>${this[2]}</td><td class='coordblink shcitt' data='${tid_3}'>${this[3]}:${this[4]}</td><td class='allyblink'>${this[1]}</td><td>${this[6]}</td><td>${this[7]}</td><td>${this[8]}</td></tr>`;
		/** @type {string} */
		supporttab_=`${supporttab_}<tr class='expsuptab'><td colspan='8'><div class='beigemenutable' style='width:98%;'><table  class='beigetablescrollp  ava'><thead><th></th><th>City</th><th>Coords</th><th colspan='2'>Troops</th><th>Status</th><th>Arrival</th></thead><tbody>`;
		var i_27;
		for(i_27 in this[9]) {
			var sid_1=this[9][i_27][15];
			var status_;
			var id_6=this[9][i_27][10];
			switch(this[9][i_27][0]) {
				case 1:
					/** @type {string} */
					supporttab_=`${supporttab_}<tr style='color: purple;'><td></td>`;
					/** @type {string} */
					status_="Sending";
					break;
				case 2:
					/** @type {string} */
					supporttab_=`${supporttab_}<tr style='color: green;'><td><button class='greenb recsup' data='${id_6}' style='height: 20px;padding-top: 3px;border-radius:6px;'>Recall</button></td>`;
					/** @type {string} */
					status_="Reinforcing";
					break;
				case 0:
					/** @type {string} */
					supporttab_=`${supporttab_}<tr style='color: #ee00858E;'><td></td>`;
					/** @type {string} */
					status_="returning";
					break;
			}
			/** @type {string} */
			supporttab_=`${supporttab_}<td data='${sid_1}' class='coordblink suplink'>${this[9][i_27][11]}</td><td class='coordblink shcitt' data='${sid_1}'>${this[9][i_27][12]}:${this[9][i_27][13]}</td>`;
			/** @type {string} */
			supporttab_=`${supporttab_}<td colspan='2'>`;
			var j_7;
			for(j_7 in this[9][i_27][8]) {
				/** @type {string} */
				supporttab_=`${supporttab_}${this[9][i_27][8][j_7]},`;
			}
			/** @type {string} */
			supporttab_=`${supporttab_}</td><td>${status_}</td><td>${this[9][i_27][9]}</td></tr>`;
		}
		/** @type {string} */
		supporttab_=`${supporttab_}</tbody></table></div></td></tr><tr class='usles'></tr>`;
	});
	$("#supporttable").html(supporttab_);
	$("#supporttable td").css("text-align","center");
	$(".expsuptab").toggle();
	//$(".usles").hide();
	/** @type {(Element|null)} */
	var newTableObject_4=document.getElementById("supporttable");

	sorttable.makeSortable(newTableObject_4);
	$(".suplink").click(function() {
		var cid_2=$(this).attr("data");
			gspotfunct.chcity(cid_2);
	});
	$(".recsup").click(function() {
		var id_7=$(this).attr("data");
		var dat_4={
			a: id_7
		};
		OverviewPost("overview/reinreca.php",dat_4);
		$(this).remove();
	});
	$(".expsup").click(function() {
		var e=$(this).parent().parent().next();
		e.toggle();
		e.height('auto');
	});
	$(".recasup").click(function() {
		var id_8=$(this).attr("data");
		var dat_5={
			a: id_8
		};
		OverviewPost("overview/reinrecall.php",dat_5);
		$(this).remove();
	});
}
//function updaterecent_() {
//	var recenttab_=`${GetRecentTabHTML()}<tbody>`;
//	/** @type {string} */
//	$.each(mru,function() {

//		recenttab_=`${recenttab_}<tr id='recent_row_${this.cid}' >`
//			+`<td class='coordblink' data='${this.cid}'>${this.cid%65536}:${Math.floor(this.cid/65536)}</td>`
//			+`<td><button class='greenb chcity' id='recent_name' a='${this.cid}'>${this.name}</button></td>`
//			+`<td><input id='recent_pin' class='clsubopti' type='checkbox' ${(this.pin===true? "checked":"")}></td>`
//			+`<td><input id='recent_misc0' class='clsubopti' type='checkbox' ${(this.misc0===true? "checked":"")}></td>`
//			+`<td><input id='recent_misc1' class='clsubopti' type='checkbox' ${(this.misc1===true? "checked":"")}></td>`
//			+`<td><input id='recent_notes'  value = '${this.notes}'></td>`
//			+`<td data='${this.player}' id='recent_player'>${this.player}</td>`
//			+`<td data='${this.alliance}' id='recent_alliance'>${this.alliance}</td>`
//			+`<td><input id='recent_last' value='${this.last.toLocaleString()}' type = 'date-time-locale'></td>`;
//	});

//	/** @type {string} */
//	recenttab_=`${recenttab_}</tbody>`;
//	$("#recenttable").html(recenttab_);
//	$("#recenttable td").css("text-align","center");
//	$("input[id^='recent_']").change(UpdateFromRecent);

//	/** @type {(Element|null)} */
//	sorttable.makeSortable(document.getElementById("recenttable"));
//}
//function UpdateFromRecent() {
//	$.each(mru,function() {
//		var r=$('#recent_row_'+this.cid);
//		let _city=GetCity();
//		if(r!==null&&r!=undefined) {
//			var HTMLElement
//			this.pin=IsChecked(r.find("#recent_pin"));
//			this.misc0=IsChecked(r.find("#recent_misc0"));
//			this.misc1=IsChecked(r.find("#recent_misc1"));
//			var notes=r.find("#recent_notes").val();
//			if(notes!==this.notes) {
//				this.notes=notes; //  change city notes
//				if(this.player===_city.pn) {
//					$.post('includes/sNte.php',{ cid: this.cid,a: this.notes,b: "" });

//				}

//			}

//		}

//	});

////	console.log(mru);

//}
/**
 * @param {!Object} data_44
 * @param {?} turnc_
 * @return {void}
 */
function updateraids_(data_44,turnc_) {
	/** @type {string} */
	var raidtab_="<thead><th>Report</th><th>Type</th><th>Cavern progress</th><th>losses</th><th>Carry</th><th>Date</th><th>Origin</th></thead><tbody>";
	/** @type {number} */
	var i_28=0;
	$.each(data_44.b,function() {
		if(i_28<turnc_) {
			if(this[2]<=2) {
				raidtab_=`${raidtab_}<tr style='color:green;'>`;
			} else {
				if(2<this[2]&&this[2]<=5) {
					raidtab_=`${raidtab_}<tr style='color:#eeCF6A00;'>`;
				} else {
					if(this[2]>5) {
						raidtab_=`${raidtab_}<tr style='color:red;'>`;
					}
				}
			}
			/** @type {string} */
			raidtab_=`${raidtab_}<td class='gFrep' data='${this[6]}'><span class='unread'>Share report</td><td>${this[0]}</span></td><td>${this[8]}%</td><td>${this[2]}%</td><td>${this[3]}%</td><td>${this[4]}</td><td>${this[1]}</td></tr>`;
		}
		i_28++;
	});
	raidtab_=`${raidtab_}</tbody>`;
	$("#raidtable").html(raidtab_);
	$("#raidtable td").css("text-align","center");
	/** @type {(Element|null)} */
	var newTableObject_5=document.getElementById("raidtable");
	sorttable.makeSortable(newTableObject_5);
}
/**
 * @param {?} data_45
 * @return {void}
 */
function updateres_(data_45) {
	/** @type {string} */
	var restabb_="<thead><tr data='0'><th>Name</th><th colspan='2'>Notes</th><th>Coords</th><th>Wood</th><th>(Storage)</th><th>Stone</th><th>(Storage)</th><th>Iron</th><th>(Storage)</th><th>Food</th><th>(Storage)</th><th>Carts</th><th>(Total)</th><th>Ships</th><th>(Total)</th><th>Score</th></tr></thead><tbody>";
	/** @type {number} */
	var woodtot_=0;
	/** @type {number} */
	var irontot_=0;
	/** @type {number} */
	var stonetot_=0;
	/** @type {number} */
	var foodtot_=0;
	/** @type {number} */
	var cartstot_=0;
	/** @type {number} */
	var shipstot_=0;
	$.each(data_45,function() {
		var cid_3=this.id;
		/** @type {number} */
		var __x=AsNumber(cid_3%65536);
		/** @type {number} */
		var __y=AsNumber((cid_3-__x)/65536);
		restabb_=`${restabb_}<tr data='${cid_3}'><td id='cn${cid_3}' class='coordblink shcitt'>${this.city}</td><td colspan='2'>${this.reference}</td><td class='coordblink' data='${cid_3}'>${__x}:${__y}</td>`;
		var res_2;
		var sto_;
		cartstot_=cartstot_+this.carts_total;
		shipstot_=shipstot_+this.ships_total;
		/** @type {number} */

		for(var i_29=0;i_29<4;i_29++) {
			switch(i_29) {
				case 0:
					res_2=this.wood;
					woodtot_=woodtot_+res_2;
					sto_=this.wood_storage;
					break;
				case 1:
					res_2=this.stone;
					stonetot_=stonetot_+res_2;
					sto_=this.stone_storage;
					break;
				case 2:
					res_2=this.iron;
					irontot_=irontot_+res_2;
					sto_=this.iron_storage;
					break;
				case 3:
					res_2=this.food;
					foodtot_=foodtot_+res_2;
					sto_=this.food_storage;
					break;
			}
			if(res_2/sto_<0.9) {
				/** @type {string} */
				restabb_=`${restabb_}<td style='color:green;'>${res_2.toLocaleString()}</td><td>${sto_.toLocaleString()}</td>`;
			} else {
				if(res_2/sto_<1&&res_2/sto_>=0.9) {
					/** @type {string} */
					restabb_=`${restabb_}<td style='color:#eeCF6A00;'>${res_2.toLocaleString()}</td><td>${sto_.toLocaleString()}</td>`;
				} else {
					if(res_2==sto_) {
						/** @type {string} */
						restabb_=`${restabb_}<td style='color:red;'>${res_2.toLocaleString()}</td><td>${sto_.toLocaleString()}</td>`;
					}
				}
			}
		}
		/** @type {string} */
		restabb_=`${restabb_}<td>${this.carts_home.toLocaleString()}</td><td>${this.carts_total.toLocaleString()}</td><td>${this.ships_home}</td><td>${this.ships_total}</td><td>${this.score.toLocaleString()}</td></tr>`;
	});
	restabb_=`${restabb_}</tbody>`;
	$("#restable").html(restabb_);
	$("#restable td").css("text-align","center");
	/** @type {(Element|null)} */
	var newTableObject_6=document.getElementById("restable");
	sorttable.makeSortable(newTableObject_6);
	/** @type {string} */
	var tottab_=`<div id='rsum' class='beigemenutable scroll-pane ava' style='width: 99%;margin-left: 4px;'><table><td>Total wood: </td><td>${woodtot_.toLocaleString()}</td><td>Total stone: </td><td>${stonetot_.toLocaleString()}</td><td>Total iron: </td><td>${irontot_.toLocaleString()}</td><td>Total food: </td><td>${foodtot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_=`${tottab_}<td>Total carts: </td><td>${cartstot_.toLocaleString()}</td><td>Total ships: </td><td>${shipstot_.toLocaleString()}</td></table></div>`;
	$("#rsum").remove();
	$("#resTab").append(tottab_);
	$("#rsum td").css("text-align","center");
	$.each(data_45,function() {
		var aa_4=this.id;
		//	$(`#cn${aa_4}`).click(() => {
		//		$("#organiser").val("all").change();
		//		$("#cityDropdownMenu").val(aa_4).change();
		//	});
	});
}

//function GetDonateHeader() {
//	return "<thead><tr data='0' class = 'ava'>"
//		+"<th>Send</th>"
//		+"<th>Name</th><th>Notes</th><th>Coords</th>"
//		+"<th>Coords</th>"
//		+"<th>Dist</th>"
//		+"<th>W needed</th>"
//		+"<th>S needed</th>"
//		+"<th>Priority</th>"
//		+"<th>notes</th>"
//		+"<th>Wood</th>"
//		+"<th>Stone</th>"
//		+"<th>Carts</th>"
//		+"<th>Total</th>"
//		+"</tr></thead>";
//}
/**
 * @param {?} data_45
 * @return {void}
 */
function Distance(__x0: number,__y0: number,__x1: number,__y1: number): number{
	let dx=__x0-__x1;
	let dy=__y0-__y1;
	return Math.sqrt(dx*dx+dy*dy);
}
function DistanceC(__a: Coord,__x1: number,__y1: number) : number {

	return Distance(__a.x,__a.y,__x1,__y1);
}
function DistanceCC(__a: Coord,__b: Coord): number{

	return Distance(__a.x,__a.y,__b.x,__b.y);
}


//function SendDonation() {
//	let me=$(this);
//	let parent=me.parent().parent();
//	let wood=GetFloatValue(parent.find("[id^='donate_wood']"));
//	let stone=GetFloatValue(parent.find("[id^='donate_stone']"));
//	const carts=GetFloatValue(parent.find("[id^='donate_carts']"));
//	const desiredRes=wood+stone;
//	let resCap=carts*1000;
//	if(desiredRes>resCap) {
//		const fract=resCap/desiredRes;
//		stone=Math.floor(stone*fract);
//		wood=Math.floor(wood*fract);
//	}
//	let _cid=parent.data();
//	var args={
//		b: stone.toString(),"d": 0,"cid": _cid,"rcid": me.val(),a: wood.toString(),"t": "1","c": 0
//	};
//	var req='includes/sndTtr.php';

//	gamePost(req,{
//		cid: _cid,f: encryptJs(req,args)
//	});
//	$(this).remove();
//	//	cid: 17695065
//	//	f: pgOAHrJYbl4LLkiRPpgYUBBEvLjIL0l5KrX9w5ayFfHnMetN9rW1bP3aiihA4jTuYtFk+ibyNwK6nSy1Oo6r20ITiFpLB8PDiyr324xc


//}

//function UpdateDonate(resData,blessData,filter) {
//	/** @type {string} */
//	let restabb_=GetDonateHeader()+"<tbody class = 'ava'>";
//	/** @type {number} */
//	let woodtot_=0;
//	/** @type {number} */
//	let stonetot_=0;
//	/** @type {number} */
//	var cartstot_=0;
//	$.each(resData,function() {
//		let cid_3=AsNumber(this.id);
//		if(filter!=null&&filter.indexOf(cid_3)==-1) {
//			return;
//		}

//		/** @type {number} */
//		let __x=AsNumber(cid_3%65536);
//		/** @type {number} */
//		let __y=AsNumber((cid_3-__x)/65536);

//		// find closest blessed city
//		let closest=null;//["None","None","Avatar","Cyndros",0,"12: 00: 00 ",0,0,0,"None on continent",cid_3,0];
//		let closestD=256*256;
//		let cont=GetContinent(__x,__y);

//		for(let i=0;i<blessData.a.length;++i) {
//			let bcid=blessData.a[i][10];

//			/** @type {number} */
//			let tempx=AsNumber(bcid%65536);
//			/** @type {number} */
//			let tempy=AsNumber((bcid-tempx)/65536);
//			if(GetContinent(tempx,tempy)!==cont)
//				continue;
//			let distance=Distance(tempx,tempy,__x,__y);
//			if(distance<closestD) {
//				closestD=distance;
//				closest=blessData.a[i];
//			}
//		}
//		if(closest===null)
//			return;
//		restabb_=`${restabb_}<tr data='${cid_3}'><td><button id='donate_rcid' data='${closest[10]}' class='ava' style='color:white;background-color: #ee3f0896;font-weight:800' >${closest[0]}</button></td>`
//			+`<td id='donate_cid' class='chcity ava' data='${cid_3}'>${this.city}</td>`
//			+`<td>${this.reference}</td><td class='coordblink' data='${cid_3}'>${__x}:${__y}</td>`
//			+`<td class='shcitt ava' data='${closest[10]}' >${closest[10]}</td>`
//			+`<td>${closestD}</td>`
//			+`<td><input value='${closest[6]}'></input></td>`
//			+`<td><input value='${closest[7]}'></input></td>`
//			+`<td>${closest[8]}</td>`
//			+`<td>${closest[9]}</td>`;

//		let res_2;
//		let sto_;
//		cartstot_=cartstot_+this.carts_total;
//		/** @type {number} */

//		for(let i_29=0;i_29<2;i_29++) {
//			switch(i_29) {
//				case 0:
//					res_2=this.wood;
//					woodtot_=woodtot_+res_2;
//					sto_=this.wood_storage;
//					break;
//				case 1:
//					res_2=this.stone;
//					stonetot_=stonetot_+res_2;
//					sto_=this.stone_storage;
//					break;

//			}
//			/** @type {string} */
//			restabb_=`${restabb_}<td><input id = 'donate_${(i_29===0? 'wood':'stone')}_${cid_3}' style='color:green;' value='${res_2.toLocaleString()}'></input></td>`;

//		}
//		/** @type {string} */
//		restabb_=`${restabb_}<td><input id = 'donate_carts_${cid_3}' value='${this.carts_home.toLocaleString()}'></input></td><td>${this.carts_total.toLocaleString()}</td></tr>`;
//	});
//	restabb_=`${restabb_}</tbody>`;
//	let jqTable=$("#donatetable");
//	jqTable.html(restabb_);
//	//$("#donatetable td").css("text-align","center");
//	/** @type {(Element|null)} */
//	let newTableObject_6=document.getElementById("donatetable");
//	sorttable.makeSortable(newTableObject_6);
//	/** @type {string} */
//	let tottab_=`<div id='rsum' class='beigemenutable scroll-pane ava'><table><td>Total wood: </td><td>${woodtot_.toLocaleString()}</td><td>Total stone: </td><td>${stonetot_.toLocaleString()}</td>`;
//	/** @type {string} */
//	tottab_=`${tottab_}<td>Total carts: </td><td>${cartstot_.toLocaleString()}</td></table></div>`;
//	$("#rsum").remove();
//	$("#donateTab").append(tottab_);

//	var temp=jqTable.find("button");
//	console.log(temp);
//	temp.click(this,SendDonation);





//	var temp2=jqTable.find(".chcity");
//	console.log(temp2);

//	//temp2.click(gspotfunct.chcity);
//	//$.each(resData,function() {
//	//	let aa_4=this.id;
//	//	$(`#cn${aa_4}`).click(() => {
//	//		$("#organiser").val("all").change();
//	//		$("#cityDropdownMenu").val(aa_4).change();
//	//	});
//	//});
//}
/**
 * @param {?} data_46
 * @param {!Object} notes_3
 * @return {void}
 */
function updatetroops_(data_46,notes_3) {
	/** @type {string} */
	var troopstab_=`<thead><tr data='0'><th>Name</th><th style='width:150px;'>Notes</th><th>Coords</th><th><div class='${tpicdiv[8]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[1]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[11]}'></div>(home)</th><th>(Total)</th></th>`;
	troopstab_=`${troopstab_}<th><div class='${tpicdiv[14]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[0]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[10]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[9]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[4]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[12]}'></div>(home)</th><th>(Total)</th>`;
	troopstab_=`${troopstab_}<th><div class='${tpicdiv[2]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[13]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[7]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[17]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[6]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[15]}'></div>(home)</th><th>(Total)</th>`;
	troopstab_=`${troopstab_}<th><div class='${tpicdiv[3]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[5]}'></div>(home)</th><th>(Total)</th><th><div class='${tpicdiv[16]}'></div>(home)</th><th>(Total)</th><th>TS(home)</th><th>(Total)</th>`;
	troopstab_=`${troopstab_}</tr></thead><tbody>`;
	/** @type {number} */
	var arbstot_=0;
	/** @type {number} */
	var balltot_=0;
	/** @type {number} */
	var druidstot_=0;
	/** @type {number} */
	var galltot_=0;
	/** @type {number} */
	var guardstot_=0;
	/** @type {number} */
	var horsetot_=0;
	/** @type {number} */
	var praetorstot_=0;
	/** @type {number} */
	var priesttot_=0;
	/** @type {number} */
	var ramstot_=0;
	/** @type {number} */
	var rangerstot_=0;
	/** @type {number} */
	var scorptot_=0;
	/** @type {number} */
	var scoutstot_=0;
	/** @type {number} */
	var senatortot_=0;
	/** @type {number} */
	var sorctot_=0;
	/** @type {number} */
	var stingerstot_=0;
	/** @type {number} */
	var triaritot_=0;
	/** @type {number} */
	var vanqstot_=0;
	/** @type {number} */
	var warshipstot_=0;
	var tshome_;
	var tstot_;
	var thome_;
	var ttot_;
	$.each(data_46,function() {
		/** @type {number} */
		tshome_=0;
		/** @type {number} */
		tstot_=0;
		var cid_4=this.id;
		var not_1=notes_3.notes[notes_3.id.indexOf(cid_4)];
		/** @type {number} */
		var x_81=AsNumber(cid_4%65536);
		/** @type {number} */
		var y_61=AsNumber((cid_4-x_81)/65536);
		troopstab_=`${troopstab_}<tr data='${cid_4}'><td id='cnt${cid_4}' class='coordblink'>${this.c}</td><td style='width:150px;'>${not_1}</td><td class='coordblink shcitt' data='${cid_4}'>${x_81}:${y_61}</td>`;
		thome_=this.Arbalist_home;
		ttot_=this.Arbalist_total;
		arbstot_=arbstot_+ttot_;
		/** @type {number} */
		tshome_=tshome_+2*thome_;
		/** @type {number} */
		tstot_=tstot_+2*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Ballista_home;
		ttot_=this.Ballista_total;
		balltot_=balltot_+ttot_;
		/** @type {number} */
		tshome_=tshome_+10*thome_;
		/** @type {number} */
		tstot_=tstot_+10*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Druid_home;
		ttot_=this.Druid_total;
		druidstot_=druidstot_+ttot_;
		/** @type {number} */
		tshome_=tshome_+2*thome_;
		/** @type {number} */
		tstot_=tstot_+2*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Galley_home;
		ttot_=this.Galley_total;
		galltot_=galltot_+ttot_;
		/** @type {number} */
		tshome_=tshome_+100*thome_;
		/** @type {number} */
		tstot_=tstot_+100*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Guard_home;
		ttot_=this.Guard_total;
		guardstot_=guardstot_+ttot_;
		tshome_=tshome_+thome_;
		tstot_=tstot_+ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Horseman_home;
		ttot_=this.Horseman_total;
		horsetot_=horsetot_+ttot_;
		tshome_=tshome_+2*thome_;
		tstot_=tstot_+2*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Praetor_home;
		ttot_=this.Praetor_total;
		praetorstot_=praetorstot_+ttot_;
		tshome_=tshome_+2*thome_;
		tstot_=tstot_+2*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Priestess_home;
		ttot_=this.Priestess_total;
		priesttot_=priesttot_+ttot_;
		tshome_=tshome_+thome_;
		tstot_=tstot_+ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Ram_home;
		ttot_=this.Ram_total;
		ramstot_=ramstot_+ttot_;
		tshome_=tshome_+10*thome_;
		tstot_=tstot_+10*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Ranger_home;
		ttot_=this.Ranger_total;
		rangerstot_=rangerstot_+ttot_;
		tshome_=tshome_+thome_;
		tstot_=tstot_+ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Scorpion_home;
		ttot_=this.Scorpion_total;
		scorptot_=scorptot_+ttot_;
		tshome_=tshome_+10*thome_;
		tstot_=tstot_+10*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Scout_home;
		ttot_=this.Scout_total;
		scoutstot_=scoutstot_+ttot_;
		tshome_=tshome_+2*thome_;
		tstot_=tstot_+2*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Senator_home;
		ttot_=this.Senator_total;
		senatortot_=senatortot_+ttot_;
		tshome_=tshome_+thome_;
		tstot_=tstot_+ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Sorcerer_home;
		ttot_=this.Sorcerer_total;
		sorctot_=sorctot_+ttot_;
		tshome_=tshome_+thome_;
		tstot_=tstot_+ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Stinger_home;
		ttot_=this.Stinger_total;
		stingerstot_=stingerstot_+ttot_;
		tshome_=tshome_+100*thome_;
		tstot_=tstot_+100*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Triari_home;
		ttot_=this.Triari_total;
		triaritot_=triaritot_+ttot_;
		tshome_=tshome_+thome_;
		tstot_=tstot_+ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Vanquisher_home;
		ttot_=this.Vanquisher_total;
		vanqstot_=vanqstot_+ttot_;
		tshome_=tshome_+thome_;
		tstot_=tstot_+ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		thome_=this.Warship_home;
		ttot_=this.Warship_total;
		warshipstot_=warshipstot_+ttot_;
		tshome_=tshome_+400*thome_;
		tstot_=tstot_+400*ttot_;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${thome_.toLocaleString()}</td><td>${ttot_.toLocaleString()}</td>`;
		/** @type {string} */
		troopstab_=`${troopstab_}<td>${tshome_.toLocaleString()}</td><td>${tstot_.toLocaleString()}</td></tr>`;
		this['tsHome']=tshome_;
	});
	troopstab_=`${troopstab_}</tbody>`;
	$("#troopstable").html(troopstab_);
	$("#troopstable td").css("text-align","center");
	$("#troopstable td").css("padding-left","0%");
	/** @type {(Element|null)} */
	var newTableObject_7=document.getElementById("troopstable");
	sorttable.makeSortable(newTableObject_7);
	/** @type {string} */
	var tottab_1="<div id='tsum' class='beigemenutable scroll-pane ava' style='width: 99%;margin-left: 4px;'><table style='font-size: 14px;width: 120%;'><tr><td>Total arbs: </td><td>Total balli: </td><td>Total druids: </td><td>Total galley: </td><td>Total guards: </td><td>Total horses: </td><td>Total praetor: </td><td>Total priest: </td><td>Total rams: </td><td>Total rangers: </td>";
	/** @type {string} */
	tottab_1=`${tottab_1}<td>Total scorp: </td><td>Total scouts: </td><td>Total senator: </td><td>Total sorc: </td><td>Total stingers: </td><td>Total triari: </td><td>Total vanqs: </td><td>Total warship: </td></tr>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<tr><td>${arbstot_.toLocaleString()}</td><td>${balltot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${druidstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${galltot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${guardstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${horsetot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${praetorstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${priesttot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${ramstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${rangerstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${scorptot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${scoutstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${senatortot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${sorctot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${stingerstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${triaritot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${vanqstot_.toLocaleString()}</td>`;
	/** @type {string} */
	tottab_1=`${tottab_1}<td>${warshipstot_.toLocaleString()}</td></tr></table></div>`;
	$("#tsum").remove();
	$("#troopsTab").append(tottab_1);
	$.each(data_46,function() {
		var aa_5=this.id;
		//$(`#cnt${aa_5}`).click(() => {
		//	$("#organiser").val("all").change();
		//	$("#cityDropdownMenu").val(aa_5).change();
		//});
	});
}
