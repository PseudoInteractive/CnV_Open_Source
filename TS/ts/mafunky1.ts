
let __base64Encode: Function=null;
let __base64Decode: Function = null;
var encryptStr = [""];
var decryptStr = [""];
// This gets replaced immediately
//function MakeGlobalGetter(a) {
//	return `window['get${a}'] = ()=> ${a};`;
//}
//function MakeGlobalCopy(a) {
//	return `window['__${a}'] = ${a};`;
//}
function encryptJs(req,k2v) {
//	console.log(req);
//	console.log(k2v);
	return a6.ccazzx.encrypt(JSON.stringify(k2v),ekeys[req],256);
};
interface stringCType {

}
var shrinec= [[]];//string,string,number,number,number,number,string, number,number,number]] = [[[]]];// = [[["castle", "", 0, 0,0,0,1, "0", 0, 0, 0]]];


/**
 * @param {!Date} date_2
 * @return {?}
 */
var errz_: number = 0;

function errorgo_(j_) {
	errz_ = errz_ + 1;

	let b_ = `errBR${errz_}`;

	let c_ = `#${b_}`;

	let d_ = `#${b_} div`;

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
	const wrapper = { error: j_ }
	window['external']['notify'](JSON.stringify(wrapper));
}


function addToAttackSender(tid)  {

let tempx = Number(tid % 65536);
let tempy = Number((tid - tempx) / 65536);

for (let i = 1; i <= 15; i++) {
	let tx = "#t" + i + "x";
	let ty = "#t" + i + "y";

	let vx = $(tx).val();
	let vy = $(ty).val();
	if (vx == tempx && vy == tempy) {

		errorgo_(tempx + ":" + tempy + " already added");
		return;
	}
}
for (let i = 1; i <= 15; i++) {
	let tx = "#t" + i + "x";
	let ty = "#t" + i + "y";
	let vx = $(tx).val();

	if (!vx) {

		$(tx).val(tempx);
		$(ty).val(tempy);
		errorgo_(tempx + ":" + tempy + " added");
		break;
	}
}
			
}

window['addtoattacksender'] = addToAttackSender;

//function betterBase64Decode() {
//	try {
//		//var me=arguments.callee.caller.caller.prototype;
//		//me.eval(MakeGlobalGetter("D6"));
//		//me.eval(MakeGlobalCopy("a6"));
//		//console.log(window['GetD6']());;

//		__a6.ccazzx.decrypt=arguments.callee.caller as (a: string,b: string,c: number) => string;


//		//console.log(window['__a6']);
//		// all done!
//		String.prototype['base64Decode']=__base64Decode;
//	}
//	catch(e) {
//		// not ready yet, try again later
//	}
//	let rv=__base64Decode.call(this);
//	//console.log(rv);
//	return rv;
//}

//function betterBase64Encode()   {
//	try {
//		//var me=arguments.callee.caller.caller.prototype;
//		//me.eval(MakeGlobalGetter("D6"));
//		//me.eval(MakeGlobalCopy("a6"));
//		//console.log(window['GetD6']());;

//		__a6.ccazzx.encrypt = arguments.callee.caller as (a: string,b: string,c: number) => string;
	
//		//console.log(this);
//		//console.log(window['__a6']);
//		// all done!
//		String.prototype['base64Encode']=__base64Encode; 
//	}
//	catch(e) {
//		// not ready yet, try again later
//	}
//	return __base64Encode.call(this);
//}
function GetCity(): jsonT.City {
	//	return window['getD6']();
	return D6;//
}
/*function DummyPromise(data:string) {
	return new Promise<Response>();
}
*/
//var __fetch=Window.prototype.fetch;
//var __debugMe: any;

//function sleep(time) {
//	return new Promise((resolve) => setTimeout(resolve,time));
//}

let defaultHeaders: Record<string,string>;

function SetupHeaders() {
	const cookie = (ppdt['opt'][67] as string).substring(0, 10);
	if (!cookie)
		throw "waiting";


	defaultHeaders = {
		'Content-Encoding': cookie,
		'pp-ss': ppss,
		'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
	};
	return cookie;
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

function Contains(a:string,b:string) {
	return a.indexOf(b)!=-1;
}

let updateTimeout=0;

function sendCityData(delayInMs) {

	clearTimeout(updateTimeout);
	updateTimeout=setTimeout(() => {

		const wrapper = {
			citydata:
			{
				cid: D6.cid,
				citn:D6.citn,
				comm: D6.comm,
				th: D6.th,
				tc: D6.tc,
				bd: D6.bd
			}
		};
		if (D6.hasOwnProperty('trin') && D6.trin.length > 0)
		{
			wrapper.citydata['trin'] = D6.trin;
		}
		if (D6.hasOwnProperty('trintr') && D6.trintr.length > 0) {
			wrapper.citydata['trintr'] = D6.trintr;
		}
		if (D6.hasOwnProperty('triin') && D6.triin.length > 0) {
			wrapper.citydata['triin'] = D6.triin;
		}

		window['external']['notify'](JSON.stringify(wrapper));
		clearTimeout(updateTimeout);
		let counc = document.getElementById("warcouncbox");
		if (counc !== null && counc.style !== null && counc.style.display === "") {
			updateattack();
			updatedef();
		}
	}, delayInMs);
}


function sendchat(channel:string,message:string)
{
	__c.sendchat(+channel,message)
}

function gCPosted()
{
	setAutoDemo(false); // clear auto demo flag
	sendCityData(100);
}

function gWrdPosted(data) {
	setTimeout(function () {
		
			wdata = JSON.parse(data);
			beentoworld = true;
			wdata = decwdata(wdata.a);
			getbossinfo();
		
	}, 1000);
}
function decwdata(data) {
	var DecData = { bosses: [], cities: [], ll: [], cavern: [], portals: [], shrines: [] },
		temp = data.split("|"),
		keys = temp[1].split("l"),
		ckey = keys[0],
		skey = keys[1],
		bkey = keys[2],
		lkey = keys[3],
		cavkey = keys[4],
		pkey = keys[5],
		cities = temp[0].split("l"),
		shrines = temp[2].split("l"),
		bosses = temp[3].split("l"),
		lawless = temp[4].split("l"),
		caverns = temp[5].split("l"),
		portals = temp[6].split("l"),
		dat = "";
	for (var i in bosses) {
		dat = (Number(bosses[i]) + Number(bkey)) + "";
		bkey = dat;
		DecData.bosses.push("1" + dat);
	}
	for (var i in cities) {
		dat = (Number(cities[i]) + Number(ckey)) + "";
		ckey = dat;
		DecData.cities.push("2" + dat);
	}
	for (var i in lawless) {
		dat = (Number(lawless[i]) + Number(lkey)) + "";
		lkey = dat;
		DecData.ll.push("3" + dat);
	}
	for (var i in caverns) {
		dat = (Number(caverns[i]) + Number(cavkey)) + "";
		cavkey = dat;
		DecData.cavern.push("7" + dat);
	}
	for (var i in portals) {
		dat = (Number(portals[i]) + Number(pkey)) + "";
		pkey = dat;
		DecData.portals.push("8" + dat);
	}
	for (var i in shrines) {
		dat = (Number(shrines[i]) + Number(skey)) + "";
		skey = dat;
		DecData.shrines.push("9" + dat);
	}
	return DecData;
}
//function __avatarAjaxDone(url: string,
//	data: string) {
//	//console.log("Change: " + this.readyState + " " + this.responseURL);

//	if (Contains(url, "gC.php")) {
		

//	}
//	else if (Contains(url, "gaLoy.php")) {
//		UpdateResearchAndFaith();
//	}
//	else if (Contains(url, "nBuu.php") || Contains(url, "UBBit.php")) {
//		sendCityData(1000);
//	}

//	else if (Contains(url, "gWrd.php")) {
//		setTimeout(function () {
//			/** @type {*} */
//			const wrapper = JSON.parse(data);
//			/** @type {boolean} */
//			beentoworld_ = true;
//			wdata_ = DecodeWorldData(wrapper.a);
//			UpdateResearchAndFaith();
//			getbossinfo_();
//		}, 1000);
//	}
//	//else if (Contains(url, "gPlA.php")) {
//	//	/** @type {*} */
//	//}
//	//// if(url.endsWith("pD.php")) {
//	//// 	pdata=JSON.parse(this.response);
//	//// }
//	//else if (Contains(url, "poll2.php")) {
//	//	setTimeout(function () {
//	//	/** @type {*} */
//	//		if (__c.hasOwnProperty('j71')) {

//	//			if (__c.j71.hasOwnProperty('OGA'))
//	//				OGA = __c.j71['OGA'];



//	//			if (__c.j71.hasOwnProperty('city')) {
//	//				{
//	//					sendCityData(4000);

//	//				}
//	//			}
//	//		}

//	//	}, 100);
//	//}
//}


function _pleaseNoMorePrefilters() { }

function OptimizeAjax()
{

//	priorPrefilter
	jQuery.ajaxPrefilter(  (A7U, n7U, xhr)=> {
	//	xhr.setRequestHeader("pp-ss", ppss);
		if (ppdt['opt'][67] !== undefined)
		{
			let cookie = (ppdt['opt'][67] as any as string).substring(0, 10);
			xhr.setRequestHeader("Content-Encoding" , cookie);
		}
	});
	//jQuery.ajaxSetup({dataType:"nada" } )
	jQuery.ajaxPrefilter = _pleaseNoMorePrefilters;;
	//setTimeout(function () {
	//	(function (open_2) {
	//		/**
	//		 * @param {string=} p0
	//		 * @param {string=} p1
	//		 * @param {(boolean|null)=} p2
	//		 * @param {(null|string)=} p3
	//		 * @param {(null|string)=} p4
	//		 * @return {void}
	//		 */
	//		XMLHttpRequest.prototype.open = function () {
	//			this.addEventListener("readystatechange", function () {
	//				//console.log("Change: " + this.readyState + " " + this.responseURL);
	//				if (this.readyState == 4) {
	//					__avatarAjaxDone(this.responseURL, this.response);

						
						
	//				}
	//			}, false);
	//			open_2.apply(this, arguments);
	//		};
	//	})(XMLHttpRequest.prototype.open);
	//}, 100);
	/*
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

function UpdateResearchAndFaith(): void {
	/**
	 * @param {?} ldata_
	 * @return {void}
	  */
	try {
		// may need to to wait, if this fails, we retry

		let faith=cotg.alliance.faith();
		let research=cotg.player.research();

		//ttres_[0]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[29]])/100;
		//ttres_[1]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[42]])/100;
		//ttres_[2]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[30]])/100;
		//ttres_[3]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[31]])/100;
		//ttres_[4]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[32]])/100;
		//ttres_[5]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[33]])/100;
		//ttres_[6]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[34]])/100;
		//ttres_[7]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[46]])/100;
		//ttres_[8]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[35]])/100;
		//ttres_[9]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[36]])/100;
		//ttres_[10]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[37]])/100;
		//ttres_[11]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[38]])/100;
		//ttres_[12]=1+AsNumber(faith.cyndros)*0.5/100+AsNumber(Res_[research[39]])/100;
		//ttres_[13]=1+AsNumber(faith.cyndros)*0.5/100+AsNumber(Res_[research[41]])/100;
		//ttres_[14]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[44]])/100;
		//ttres_[15]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[43]])/100;
		//ttres_[16] = 1 + AsNumber(faith.cyndros) * 0.5 / 100 + AsNumber(Res_[research[45]]) / 100;

		//	0 "guard",1 "ballista",2 "ranger",3 "triari", 
		//  4  "priestess",5 "vanquisher",6 "sorcerers",7 "scout", 
		/// 8  "arbalist",9 "praetor",10 "horseman",11 "druid",
		//   12 "ram",13 "scorpion",14 "galley",15 "stinger",
		///  16 "warship",17 "senator"


		ttSpeedBonus[0] = 1; // no speed reserach for guard
		ttSpeedBonus[1]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[12]])/100;
		ttSpeedBonus[2]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttSpeedBonus[3]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttSpeedBonus[4]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttSpeedBonus[5]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttSpeedBonus[6]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttSpeedBonus[7]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[11]])/100;
		ttSpeedBonus[8]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttSpeedBonus[9]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttSpeedBonus[10]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttSpeedBonus[11]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttSpeedBonus[12]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[12]])/100;
		ttSpeedBonus[13]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[12]])/100;
		ttSpeedBonus[14]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[13]])/100;
		ttSpeedBonus[15]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[13]])/100;
		ttSpeedBonus[16]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[13]])/100;
		ttSpeedBonus[17]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[14]])/100;


		ttCombatBonus[0] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[29]]) / 100;
		ttCombatBonus[1] = 1 + AsNumber(faith.naera) * 0.5 / 100 + AsNumber(Res_[research[42]]) / 100;
		ttCombatBonus[2]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[30]])/100;
		ttCombatBonus[3]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[31]])/100;
		ttCombatBonus[4]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[32]])/100;
		ttCombatBonus[5]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[33]])/100;
		ttCombatBonus[6]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[34]])/100;
		ttCombatBonus[7]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[46]])/100;
		ttCombatBonus[8]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[35]])/100;
		ttCombatBonus[9]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[36]])/100;
		ttCombatBonus[10]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[37]])/100;
		ttCombatBonus[11]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[38]])/100;
		ttCombatBonus[14]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[44]])/100;
		ttCombatBonus[15]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[43]])/100;
		ttCombatBonus[16] = 1 + AsNumber(faith.cyndros) * 0.5 / 100 + AsNumber(Res_[research[45]]) / 100;
		ttCombatBonus[17] = 1; // no combat research for senator

	//	let wrapper = { speedBonus: ttSpeedBonus, combatBonus: ttCombatBonus }
	//	window['external']['notify'](JSON.stringify(wrapper));

	}
	catch(e) {
		//setTimeout(UpdateResearchAndFaith,1000);
	//	return;

	}

}

//function getppdt() {
//	return JSON.stringify(ppdt);
//}
function jqclick( s) {
	$(s).click();
}

function getview() {
	if (regrender == 1)
		return "region";
	if (citrender == 1)
		return "city";
	return "world";
}

function avapost(url: string, args: string) {
	const k2D = $.post(url, args);
	k2D.done(s => {
		console.log(s);
	});
}

// returns for current city
function avagetts() {
	var rv = { tc: D6.tc, th: D6.th };
	return JSON.stringify(rv);
}


//async function avafetch(url: string, args: string) {
//	let req = fetch(url, {
//		method: 'POST',
//		headers: defaultHeaders,

//		mode: 'cors',
//		cache: "no-cache",
//		body: args
//	});
//	let a = await req;
//	let txt = a.text();
//	console.log(txt);
//	return txt;
//}
var raidSecret: string;

function onKeyDown(ev: KeyboardEvent) {

	if (ev.keyCode == 9) {
		ev.preventDefault();
		ev.stopPropagation();
		window.departFocus('up', { originLeft: 0, originTop: 0, originWidth: 0, originHeight: 0 });
	}

	if (ev.key !== "Control" && ev.key !== "Shift")
		return;

	let wrapper = {
		keyDown: { key: ev.key, alt: ev.altKey, shift: ev.shiftKey, control: ev.ctrlKey }
	};
	let str = JSON.stringify(wrapper);
	setTimeout(()=> window['external']['notify'](str),0);
}

function onKeyUp(ev: KeyboardEvent) {

	if (ev.key !== "Control" && ev.key !== "Shift")
		return;

	let wrapper = {
		keyUp: { key: ev.key, alt: ev.altKey, shift: ev.shiftKey, control: ev.ctrlKey }
	};
	let str = JSON.stringify(wrapper);
	setTimeout(() => window['external']['notify'](str), 0);
}


function canvasMouseDown(ev: MouseEvent ) {

	let wrapper = {
		mouseDown: {
			button: ev.button, alt: ev.altKey, shift: ev.shiftKey, control: ev.ctrlKey, x: ev.clientX, y: ev.clientY
		}
	}
	window['external']['notify'](JSON.stringify(wrapper));
}

// i.e. "click"
let underMouse : Element = null;
function postMouseEvent(sx: string,sy:string, eventName:string,button : string, dx:string, dy:string)
{
	let x = parseInt(sx);
	let y = parseInt(sy);
	//if(eventName === "mousedown")
		underMouse = document.elementFromPoint(x,y);

	if(underMouse != null)
	{
		let buttons = parseInt(button);
		let evt = new MouseEvent(eventName, {
			bubbles: true,
			cancelable: true,
			view: window,
			clientX:x,
			clientY:y,
			button:buttons,
			buttons:1,
			movementX: dx!==null? parseInt(dx):null,
			movementY: dy!==null? parseInt(dy):null,
		  });
	  let canceled = !underMouse.dispatchEvent(evt);
	}
}

function postppdt()
{

	try {

		console.log("Notify here");
		let options: AddEventListenerOptions = { capture: true, passive:true,once:false } 
		document.addEventListener("keydown", onKeyDown, options);
		document.addEventListener("keyup", onKeyUp, options);
	//	window.addEventListener("click", onMouseDown, options);
	//	document.addEventListener("mousedown", onMouseDown, options);
		document.getElementById("mainMapDiv").addEventListener("mousedown",canvasMouseDown,options);
		 // this needs to be white in region mode 
	

		let creds = {
			token: SetupHeaders(),
			raid:raidSecret,
			ppss: ppss,
			player: ppdt.pn,
			pid: ppdt.pid,
			s: s,
			cookie: document.cookie,
			cid: ppdt.lcit,
			time: currentTime(),
			spanX: mainMapDiv.clientWidth,
			spanY: mainMapDiv.clientHeight,
			left: mainMapDiv.clientLeft,
			top: mainMapDiv.clientTop,
			timeoffset: (ServerDate.getTime() - Date.now()),
			agent: navigator.userAgent,
		};

		
		let wrapper = { jsvars: creds, ppdt:ppdt }
//		if (D6 != null)
//			wrapper['citydata'] = D6;

		window['external']['notify'](JSON.stringify(wrapper));
		//OptimizeAjax();
	} catch (e) {
		console.log("Notify failed");
//		setTimeout(SendCreds, 1000); // vars are probably not ready try again in 1s
	}
	document.getElementById("tbbuttons").style.display = "none"; // these no longer work
	document.getElementById("canvasborders").style.display = "none";
	 document.getElementById("container").style.background =  "none";
	let cityMap = document.getElementById("city_map");
	cityMap.className = ""; // remove backgroud
	cityMap.style.background = "rgba(255,255,255,1)";
	cityMap.style.pointerEvents = "none";
//	 mainMapDiv.style.display = "none";//(_viewMode!==viewModeCity) ? "none" : null;
	setTimeout(avactor, 3000);

}

function GetDate(jq: string) {
	return new Date($(jq).data().toString());
}

//var stringTable = [];

//function BuildStringTable(idStart)
//{
//	let i = idStart;
//	let idEnd = i + 200;
	
//	for (; i < idEnd;++i) {

//		let x = "";
//			try {
//				x = i011.o55(i) || x;
//			}
//			catch (e) {
//			}
//			stringTable.push(x);
//	}
//	if (i >= 10000) {
//		console.log("done");
//		const wrapper = { stable: stringTable }
//		window['external']['notify'](JSON.stringify(wrapper));
//	} else {
//		console.log(i);
//		setTimeout(() => BuildStringTable(i), 100);
//	}

//}

//(function () {
//	let stringTable = [];
//	for (let i = 0; i < 1000; ++i) {

//		let x = '';
//		try {
//			x = o0FF.y5u(i) || x;
//		}
//		catch (e) {
//		}
//		stringTable.push(x);
//	}
//	return JSON.stringify(stringTable);

//})();



function viewcity(cid: string)
{
	gspotfunct.chcity(Number(cid))
	$("#cityButton").click();
	_viewModeCache =_viewMode = viewModeCity; // 

}
function setviewmode(mode: string) {
	if (mode == 'c') {
		$("#cityButton").click();
		_viewModeCache = _viewMode = viewModeCity; //
		callSyncViewMode();

	}
	else {
		$("#worldButton").click();
		_viewModeCache = _viewMode = viewModeWorld; // 
		callSyncViewMode();

	}
}

function avactor() {

	//	var E3y="5894";
	var q7y = 15;
	var G5y = 128;
	var q3y = 16;

	var v1R = 192;
//	var P2y = 1000;
	var l9p = 0xffff;
	var k9p = 0x100000000;
	console.log("here");
	
	popupSizeDirty=true;
	callSyncViewMode();


	window['alliancelink'] = gspotfunct.alliancelink;
	window['infoPlay'] = gspotfunct.infoPlay;
	window['shCit'] = gspotfunct.shCit;
	window['chcity'] = gspotfunct.chcity;
	//{
	//	let cont = document.getElementById("mainMapDiv");
	//	cont.oncontextmenu = null;
	//	(cont.children[0] as HTMLBaseElement).oncontextmenu = null;
	//	(cont.children[0] as HTMLBaseElement).style.background = "rgba(0,0,0,0)";
	//	let cc = cont.children[0].childElementCount;
	//	for (let i = 0; i < cc; ++i) {
	//		(cont.children[0].children[i] as HTMLElement).oncontextmenu = null;
	//		(cont.children[0].children[i] as HTMLElement).style.background = "rgba(0,0,0,0)";
	//	}
	//}
	
	let date = new Date(ServerDate.getTime());
	console.log( date );
	console.log(date.getUTCHours());
	console.log(date.getUTCDate());
	console.log(date.getDate());
	let date2 = date;
	date.setUTCHours(1, 0, 0, 0);
	date2.setHours(1, 0, 0, 0);
	console.log(date);
	console.log(date2);
	String.prototype['utf8Encode'] = function () {
		//		//	if (encryptStr.length > 8)
		//		//		encryptStr.shift();
		//		//	encryptStr.push(this);
		//		//	console.log(this);
		return unescape(encodeURIComponent(this));
	};
	String.prototype['utf8Decode'] = function () {
		//		//	if (decryptStr.length > 8)
		//	//			decryptStr.shift();
		//	//		decryptStr.push(this);
		////			console.log(this);
		return decodeURIComponent(escape(this));
	};

	cotgsubscribe.subscribe("regional", clickInfo => {
		let _cid = AsNumber(clickInfo.x) + 65536 * AsNumber(clickInfo.y);
		var it = { ...defaultMru }; // clone defaults


		var dtype_ = clickInfo.type;
		it.cid = _cid;
		it.name = clickInfo.info.name;
		it.player = clickInfo.info.player;
	//	console.log(clickInfo);
		UpdateResearchAndFaith();

		if (dtype_ === "dungeon") {
			let prog_ = clickInfo.info.prog;
			let troops = cotg.city.troops();
			let type_113 = clickInfo.info.type;
			let lvl_ = clickInfo.info.lvl as number;
			it.score = lvl_;
			it.name = type_113 + " " + lvl_;
			if ($("#cityplayerInfo div table tbody tr").length === 11) {
				bossele_();
			}
			
			let home_loot_2 = 0;

			for (let i in troops) {
				let d = troops[i];
				
				let home_1 = d.home;
				
				home_loot_2 = home_loot_2 + home_1 * ttloot_[TroopNameToId(i)];


			}
			if (type_113 === "Siren's Cove") {
				
				let optimalTS_ = Math.ceil(other_loot[lvl_ - 1] / 10 * (1 - prog_ / 100 + 1) * 1.05);

				
				var galleyTS_ = Math.ceil(optimalTS_ / 100);
				
				var stingerTS_ = Math.ceil(optimalTS_ / 150);
				
				var warshipTS_ = Math.ceil(optimalTS_ / 300);
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						if (troops.warship.home > warshipTS_) {
							$("#raidIP16").val(warshipTS_);
						} else {
							if (troops.stinger.home > stingerTS_) {
								$("#raidIP15").val(stingerTS_);
							} else {
								if (troops.galley.home > galleyTS_) {
									$("#raidIP14").val(galleyTS_);
								} else {
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
				let loot_: number[];
				if (type_113 === "Mountain Cavern")
					loot_ = mountain_loot;
				else
					loot_ = other_loot;
				/**
				 * @return {void}
				 */
				let total_lootm_ = Math.ceil(loot_[AsNumber(lvl_) - 1] * (1 - AsNumber(prog_) / 100 + 1) * 1.05);
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						
						if (home_loot_2 > total_lootm_) {
							
							const option_numbersm_ = Math.floor(home_loot_2 / total_lootm_);
							
							const templ1m_ = home_loot_2 / total_lootm_ * 100 / option_numbersm_;
							
							const templ2m_ = (templ1m_ - 100) / templ1m_ * 100;
							
							for (let i in troops) {
								const id = TroopNameToId(i);
								const th = troops[i].home;
								
								$(`#raidIP${id}`).val(th / option_numbersm_);
							}
						}
					}, 1500);
				};
				
				const optimalTSM_ = total_lootm_;
				var infoptim_ = Math.ceil(optimalTSM_ / 10);
				
				var cavoptim_ = Math.ceil(optimalTSM_ / 15);
				
				var praoptim_ = Math.ceil(optimalTSM_ / 20);
				
				var sorcoptim_ = Math.ceil(optimalTSM_ / 5);
				
				var RToptim_ = cavoptim_;
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(infoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text(RToptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(infoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(praoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(cavoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(cavoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(sorcoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(infoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(infoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(praoptim_);
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
			}

		}
		else if (dtype_ === "boss") {
			//	let type_113 = clickInfo.info.type;
			let lvl_ = clickInfo.info.lvl as number;
			let bossname_ = clickInfo.info.name;
			it.score = lvl_;
			// @todo
			if ($("#cityplayerInfo div table tbody tr").length === 11) {
				bossele_();
			}
			if (clickInfo.info.active) {
				
				message_23 = "Inactive Boss";
				errorgo_(message_23);
			}
			
			message_23 = "Not enough TS to kill this boss!";
			
			var attackres_ = [];
			
			var attackwres_ = [];
			for (let i_42 in ttattack_) {
				
				var bossTS_ = Math.ceil(AsNumber(bossdef_[lvl_ - 1]) * 4 / (AsNumber(ttattack_[i_42]) * AsNumber(ttCombatBonus[i_42])));
				attackres_.push(bossTS_);
				
				var bosswTS_ = Math.ceil(AsNumber(bossdefw_[lvl_ - 1]) * 4 / (AsNumber(ttattack_[i_42]) * AsNumber(ttCombatBonus[i_42])));
				attackwres_.push(bosswTS_);
			}
			
			var home_strength_ = 0;
			
			let home_loot_2 = 0;
			
			let km_2 = [];
			
			var bm_ = [];
			
			var bmw_ = [];
			
			var kg_ = [];
			
			var home_TSw_ = 0;
			
			var boss_strength_ = Math.ceil(AsNumber(bossdef_[lvl_ - 1]) * 4);
			
			var boss_strengthw_ = Math.ceil(AsNumber(bossdefw_[lvl_ - 1]) * 4);
			
			let i_4X = 0;
			for (var x_85 in D6.th) {
				
				let home_1 = AsNumber(D6.th[x_85]);
				if (i_4X === 0 || i_4X === 1 || i_4X === 7 || i_4X === 12 || i_4X === 13) {
					
					home_1 = 0;
				}
				kg_.push(home_1);
				if (i_4X === 14 || i_4X === 15 || i_4X === 16) {
					
					home_1 = 0;
				}
				
				home_strength_ = home_strength_ + AsNumber(ttattack_[i_4X]) * AsNumber(home_1) * AsNumber(ttCombatBonus[i_4X]);
				
				home_TSw_ = home_TSw_ + home_1 * TS_type_[i_4X];
				km_2.push(home_1);
				
				i_4X = i_4X + 1;
				if (i_4X === 17) {
					break;
				}
			}
			if (home_strength_ > boss_strength_) {
				
				var proportion_ = home_strength_ / boss_strength_;
				for (let i_42 in km_2) {
					
					bm_[i_42] = Math.ceil(AsNumber(km_2[i_42]) / proportion_);
				}
			}
			if (home_strength_ > boss_strengthw_) {
				
				var proportionw_ = home_strength_ / boss_strengthw_;
				for (let i_42 in km_2) {
					
					bmw_[i_42] = Math.ceil(AsNumber(km_2[i_42]) / proportionw_);
				}
			}
			if (bossname_ === "Triton") {
				
				var bmz_ = [];
				
				var home_strengthw_ = 0;
				
				var galleytroops_ = 0;
				
				var tempgalley_ = 0;
				
				var galley_TSneeded_ = Math.ceil(home_TSw_ / 500);
				if (kg_[14]) {
					
					home_strengthw_ = home_strength_ + AsNumber(galley_TSneeded_) * 3000 * AsNumber(ttCombatBonus[14]);
					if (home_strengthw_ > boss_strength_) {
						
						var proportionz_ = home_strengthw_ / boss_strength_;
						for (let i_42 in km_2) {
							
							bmz_[i_42] = Math.ceil(AsNumber(km_2[i_42]) / proportionz_);
							
							tempgalley_ = tempgalley_ + bmz_[i_42] * TS_type_[i_42];
						}
					}
					
					galleytroops_ = Math.ceil(tempgalley_ / 500);
				}
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						if ((kg_[14] || kg_[15] || kg_[16]) && !kg_[5] && !kg_[6] && !kg_[8] && !kg_[9] && !kg_[10] && !kg_[11] && !kg_[2] && !kg_[3] && !kg_[4]) {
							if (kg_[16] > attackwres_[16]) {
								$("#raidIP16").val(attackwres_[16]);
							} else {
								if (kg_[15] > attackwres_[15]) {
									$("#raidIP15").val(attackwres_[15]);
								} else {
									if (kg_[14] > attackwres_[14]) {
										$("#raidIP14").val(attackwres_[14]);
									} else {
										errorgo_(message_23);
									}
								}
							}
						} else {
							if (kg_[14] && (kg_[5] || kg_[6] || kg_[8] || kg_[9] || kg_[10] || kg_[11] || kg_[2] || kg_[3] || kg_[4])) {
								if (kg_[14] > galleytroops_ && bmz_.length > 0) {
									var i_46;
									for (i_46 in km_2) {
										$(`#raidIP${[i_46]}`).val(bmz_[i_46]);
									}
									$("#raidIP14").val(galleytroops_);
								} else {
									if (kg_[14] > attackwres_[14]) {
										$("#raidIP14").val(attackwres_[14]);
									} else {
										errorgo_(message_23);
									}
								}
							} else {
								errorgo_(message_23);
							}
						}
					}, 1500);
				};
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text(attackwres_[14]);
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text(attackwres_[15]);
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text(attackwres_[16]);
			}
			if (bossname_ == "Cyclops") {
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						var i_47;
						for (i_47 in km_2) {
							if ((km_2[8] || km_2[9] || km_2[10]) && !km_2[5] && !km_2[6] && !km_2[2] && !km_2[3] && !km_2[4] && !km_2[11]) {
								$(`#raidIP${[i_47]}`).val(bmw_[i_47]);
								if (bmw_.length === 0) {
									errorgo_(message_23);
									break;
								}
							} else {
								$(`#raidIP${[i_47]}`).val(bm_[i_47]);
								if (bm_.length === 0) {
									errorgo_(message_23);
									break;
								}
							}
						}
					}, 1500);
				};
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackwres_[8]);
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackwres_[10]);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackwres_[9]);
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackwres_[7]);
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
			}
			if (bossname_ == "Andar's Colosseum Challenge") {
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						var i_48;
						for (i_48 in km_2) {
							if ((km_2[8] || km_2[9] || km_2[10]) && !km_2[5] && !km_2[6] && !km_2[2] && !km_2[3] && !km_2[4] && !km_2[11]) {
								$(`#raidIP${[i_48]}`).val(bmw_[i_48]);
								if (bmw_.length === 0) {
									errorgo_(message_23);
									break;
								}
							} else {
								$(`#raidIP${[i_48]}`).val(bm_[i_48]);
							}
							if (bm_.length === 0) {
								errorgo_(message_23);
								break;
							}
						}
					}, 1500);
				};
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackwres_[8]);
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackwres_[10]);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackwres_[9]);
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackwres_[7]);
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
			}
			if (bossname_ == "Romulus and Remus") {
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						var i_49;
						for (i_49 in km_2) {
							if ((km_2[2] || km_2[3] || km_2[4] || km_2[5]) && !km_2[6] && !km_2[8] && !km_2[9] && !km_2[10] && !km_2[11]) {
								$(`#raidIP${[i_49]}`).val(bmw_[i_49]);
								if (bmw_.length === 0) {
									errorgo_(message_23);
									break;
								}
							} else {
								$(`#raidIP${[i_49]}`).val(bm_[i_49]);
							}
							if (bm_.length === 0) {
								errorgo_(message_23);
								break;
							}
						}
					}, 1500);
				};
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackwres_[5]);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackwres_[2]);
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackwres_[3]);
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackwres_[4]);
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
			}
			if (bossname_ == "Dragon") {
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						var i_50;
						for (i_50 in km_2) {
							if ((km_2[2] || km_2[3] || km_2[4] || km_2[5]) && !km_2[6] && !km_2[8] && !km_2[9] && !km_2[10] && !km_2[11]) {
								$(`#raidIP${[i_50]}`).val(bmw_[i_50]);
								if (bmw_.length === 0) {
									errorgo_(message_23);
									break;
								}
							} else {
								$(`#raidIP${[i_50]}`).val(bm_[i_50]);
							}
							if (bm_.length === 0) {
								errorgo_(message_23);
								break;
							}
						}
					}, 1500);
				};
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackwres_[5]);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackwres_[2]);
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackwres_[3]);
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackres_[6]);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackres_[11]);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackwres_[4]);
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
			}
			if (bossname_ == "Gorgon") {
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						var i_51;
						for (i_51 in km_2) {
							if ((km_2[6] || km_2[11]) && !km_2[4] && !km_2[5] && !km_2[3] && !km_2[8] && !km_2[9] && !km_2[10] && !km_2[2]) {
								$(`#raidIP${[i_51]}`).val(bmw_[i_51]);
								if (bmw_.length === 0) {
									errorgo_(message_23);
									break;
								}
							} else {
								$(`#raidIP${[i_51]}`).val(bm_[i_51]);
							}
							if (bm_.length === 0) {
								errorgo_(message_23);
								break;
							}
						}
					}, 1500);
				};
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackwres_[6]);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackwres_[11]);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
			}
			if (bossname_ == "Gladiator") {
				/**
				 * @return {void}
				 */
				document.getElementById("raidDungGo").onclick = () => {
					setTimeout(() => {
						var i_52;
						for (i_52 in km_2) {
							if ((km_2[6] || km_2[11]) && !km_2[4] && !km_2[5] && !km_2[3] && !km_2[8] && !km_2[9] && !km_2[10] && !km_2[2]) {
								$(`#raidIP${[i_52]}`).val(bmw_[i_52]);
								if (bmw_.length === 0) {
									errorgo_(message_23);
									break;
								}
							} else {
								$(`#raidIP${[i_52]}`).val(bm_[i_52]);
							}
							if (bm_.length === 0) {
								errorgo_(message_23);
								break;
							}
						}
					}, 1500);
				};
				$("#cityplayerInfo div table tbody tr:nth-child(5) td:nth-child(2)").text(attackres_[5]);
				$("#cityplayerInfo div table tbody tr:nth-child(6) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(7) td:nth-child(2)").text(attackres_[2]);
				$("#cityplayerInfo div table tbody tr:nth-child(8) td:nth-child(2)").text(attackres_[3]);
				$("#cityplayerInfo div table tbody tr:nth-child(9) td:nth-child(2)").text(attackres_[8]);
				$("#cityplayerInfo div table tbody tr:nth-child(10) td:nth-child(2)").text(attackres_[10]);
				$("#cityplayerInfo div table tbody tr:nth-child(11) td:nth-child(2)").text(attackwres_[6]);
				$("#cityplayerInfo div table tbody tr:nth-child(12) td:nth-child(2)").text(attackwres_[11]);
				$("#cityplayerInfo div table tbody tr:nth-child(13) td:nth-child(2)").text(attackres_[4]);
				$("#cityplayerInfo div table tbody tr:nth-child(14) td:nth-child(2)").text(attackres_[9]);
				$("#cityplayerInfo div table tbody tr:nth-child(15) td:nth-child(2)").text(attackres_[7]);
				$("#cityplayerInfo div table tbody tr:nth-child(16) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(17) td:nth-child(2)").text("0");
				$("#cityplayerInfo div table tbody tr:nth-child(18) td:nth-child(2)").text("0");
			}
		}
		else if (dtype_ === "portal" || dtype_ === "shrine")
		{
			it.score = clickInfo.info.active;
		}
		else{
			$("#cityplayerInfo div table tbody tr:gt(6)").remove();
			// var coords = $("#citycoords")[0].innerText.split(":");
		}
			let isCity = (dtype_ === "city");
			let isLawless = dtype_ === "lawless"
			//var maxCount = 32;

			///** @type {AsNumber} */
			//for (var i = 0; i < mru.length; ++i) {
			//	var _i = mru[i];
			//	if (_i.cid == _cid) {
			//		toAdd = _i;
			//		toAdd.last = new Date();
			//		mru.splice(AsNumber(i), 1);
			//		break;
			//	}
			//}
			//if (mru.length > maxCount) {
			//	
			//	for (var i = mru.length; --i >= 0;) {
			//		if (!mru[i].pin) {
			//			mru.splice(i, 1);
			//			break;
			//		}
			//	}
			//}
			

		if (isCity || isLawless) {
				it.water = clickInfo.info.water;
				if (isCity) {
					it.notes = clickInfo.info.remarks;
					it.plvl = clickInfo.info.plvl;
				}
				else {
					it.plvl = 0;
				}
				//	toAdd.alliance = isCity ? clickInfo.info.alliance : 0;
				//toAdd.castle = clickInfo.info.castle;
				it.bless = clickInfo.info.blessed;
				it.score = clickInfo.info.score;
			}
			else {

			}
			//mru.push(toAdd);
			//mru.sort((a, b) => { return b.last.valueOf() - a.last.valueOf() });
			//console.log(mru);
			//localStorage.setItem("mru", JSON.stringify(mru));
			const wrapper = { cityclick: it }
			window['external']['notify'](JSON.stringify(wrapper));

		
	});
	//SendAllianceInfo();
	//setTimeout(SendCreds, 600);




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


	setTimeout(() => {

	//	__base64Encode = String.prototype['base64Encode'];
	//	String.prototype['base64Encode'] = betterBase64Encode;

	//	__base64Decode = String.prototype['base64Decode'];
	//	String.prototype['base64Decode'] = betterBase64Decode;


		/// 
		//BuildStringTable(0);

		$("<style type='text/css'> .ava{ width: auto; line-height:100%; table-layout: auto;text-align: center;padding-top:0px;padding-left:0px;border-width:1px;margin-left:0px } </style>").appendTo("head");
		$("<style type='text/css'> .ava td{ width: auto; line-height:100% table-layout: auto; text-align: center;padding-top:0px;padding-left:0px;border-width:1px;margin-left:0px} </style>").appendTo("head");
		$("<style type='text/css'> .ava table{table-layout: auto; } </style>").appendTo("head");

		//Decode();
		
		//  var popwin_ = `<div id='HelloWorld' style='width:400px;height:400px;background-color: #eeE2CBAC;-moz-border-radius: 10px;-webkit-border-radius: 10px;border-radius: 10px;border: 4px ridge #eeDAA520;position:absolute;right:40%;top:100px; z-index:1000000;'><div class=\"popUpBar\"> <span class=\"ppspan\">Welcome!</span><button id=\"cfunkyX\" onclick=\"$('#HelloWorld').remove();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div id='hellobody' class=\"popUpWindow\"><span style='margin-left: 5%;'> <h3 style='text-align:center;'>Welcome to Crown Of The Gods!</h3></span><br><br><span style='margin-left: 5%;'> <h4 style='text-align:center;'> MFunky(Cfunky + Dfunky + Mohnki's Additional Layouts + Avatar's nonsense)</h4></span><br><span style='margin-left: 5%;'> <h4 style='text-align:center;'>Updated Mar 1 2020</h4></span><br><br><span style='margin-left: 5%;'><h4>changes:</h4> <ul style='margin-left: 6%;'><li>Added 4 raiding carry percentages(100..125)</li><li>When you click on one, it will ensure that carry is at least that value and it will set it as the initial value for the next city that you go to</li></ul></span></div></div>`;

		//$("body").append(popwin_);

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
		
		var i_32 = 0;
		for (; i_32 < l_3; i_32++) {
			
			var temp_3 = String($(a_5[i_32]).attr("value"));
			//	$("#organiser").val(temp_3).change();
			
			//  console.log(ppdt.clc);
			//console.log(temp_3);
			//  if(ppdt.clc !== null && temp_3 !== null )
			//{
			//  ppdt.clc[temp_3] = [];
			//  var tempcl_ = $("#cityDropdownMenu > option");
			//  var ll_ = tempcl_.length;
			//  if (D6.cg,temp_3) > -1) {
			//    ppdt.clc[temp_3].push($(tempcl_[0]).attr("value"));
			//  }
			//  if (ll_ > 1) {
			//    
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

	}, 8000);

	//this	{"a":"worldButton","b":"block","c":true,"d":1591969039987,"e":"World"}

	
	/**
	 * @param {?} index_54
	 * @param {string} char_
	 * @return {?}
	 * @this {!String}
	 */
	function ReplaceAt(me: string, index_54: number, char_: string) {
		/** @type {!Array<string>} */
		var a_6 = me.split("");
		
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
	


	//var _mru = localStorage.getItem("mru");
	//if (_mru != null)
	//	mru = JSON.parse(_mru);

	setTimeout(() => {
		UpdateResearchAndFaith();
		
		var returnAllbut = "<button id='returnAllb' style='right: 35.6%; margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Return All</button>";
		
		var raidbossbut = "<button id='raidbossGo' style='left: 65%;margin-left: 10px;margin-top: 15px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Locate Bosses</button>";
		
		var attackbut = "<button id='attackGo' style='margin-left: 25px;margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Attack Sender</button>";
		
		var defbut = "<button id='defGo' style='left: 65%;margin-left: 10px;margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Defense Sender</button>";
		
		var quickdefbut = "<button id='quickdefCityGo' style='width:96%; margin-top:2%; margin-left:2%;' class='regButton greenbuttonGo greenb'>@ Quick Reinforcements @</button>";
		
		var neardefbut = "<button id='ndefGo' style='left: 4%;margin-left: 3px;margin-top: 95px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'> Nearest Defense</button>";
		
		var nearoffbut = "<button id='noffGo' style='right: 35.6%; margin-top: 95px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'> Offensive list</button>";
		
		var addtoatt = "<button id='addtoAtt' style='margin-left: 7%;margin-top: -5%;width: 40%;height: 26px !important; font-size: 9px !important;' class='regButton greenb'>Add to Attack Sender</button>";
		
		var addtodef = "<button id='addtoDef' style='margin-left: 7%;width: 40%;height: 26px !important; font-size: 9px !important;' class='regButton greenb'>Add to Defense Sender</button>";
	//bosstab
		var bosstab = "<li id='bosshuntab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warBossmanager'";
		
        bosstab+="aria-labeledby='ui-id-20' aria-selected='false' aria-expanded='false'>";
		
        bosstab+="<a href='#warBossmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-20'>Find Bosses</a></li>";
		
		var bosstabbody = "<div id='warBossmanager' aria-labeledby='ui-id-20' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		
        bosstabbody+=" role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >CFunky's Boss Raiding tool:</div>";
		
        bosstabbody+="<div id='bossbox' class='beigemenutable scroll-pane' style='width: 96%; height: 85%; margin-left: 2%;'></div>";
		
        bosstabbody+="<div id='idletroops'></div></div>";
        //attack tab
		var attacktab = "<li id='attacktab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warAttackmanager'";
		
        attacktab+="aria-labeledby='ui-id-21' aria-selected='false' aria-expanded='false'>";
		
        attacktab+="<a href='#warAttackmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-21'>Attack</a></li>";
        //attack body
		var attacktabbody = "<div id='warAttackmanager' aria-labeledby='ui-id-21' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		
        attacktabbody+=" role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Attack Sender:</div>";
		
        attacktabbody+="<div id='attackbox' class='beigemenutable scroll-pane' style='width: 53%; height: 50%; float:left; margin-left: 1%; margin-right: 1%;'>";
		
        attacktabbody+="<table><thead><th></th><th>X</th><th>Y</th><th>Type</th></thead><tbody>";
        for (let i=1;i<16;i++) {
			
            attacktabbody+="<tr><td>Target "+i+" </td><td><input id='t"+i+"x' type='number' style='width: 85%'></td><td><input id='t"+i+"y' type='number' style='width: 85%'></td>";
			
            attacktabbody+="<td><select id='type"+i+"' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'><option value='0'>Fake</option><option value='1'>Real</option></select></td></tr>";
		}
		
        attacktabbody+="</tbody></table></div>";
		
        attacktabbody+="<div id='picktype' class='beigemenutable scroll-pane' style='width: 43%; height: 50%;'></div>";
		
        attacktabbody+="<table><tr><td><span>Use percentage of troops:</span></td><td><input id='perc' type='number' style='width: 30px'>%</td><td></td></tr>";
		
        attacktabbody+="<tr><td><span>Send real as:</span></td><td><select id='realtype' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>";
		
        attacktabbody+="<option value='0'>Assault</option><option value='1'>Siege</option><option value='2'>Plunder</option><option value='3'>Scout</option></select></td><td></td></tr>";
		
        attacktabbody+="<tr><td><span>Send fake as:</span></td><td><select id='faketype' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>";
		
        attacktabbody+="<option value='0'>Assault</option><option value='1'>Siege</option><option value='2'>Plunder</option><option value='3'>Scout</option></select></td><td></td></tr>";
		
        attacktabbody+="<tr><td><input id='retcheck' class='clsubopti' type='checkbox' checked> Return all Troops</td><td colspan=2><input id='retHr' type='number' style='width: 20px' value='2'> Hours before attack</td></tr>";
		
        attacktabbody+="<tr><td><input id='scoutick' class='clsubopti' type='checkbox' checked>30galleys/1scout fake</td></tr></table>";
		
        attacktabbody+="<table style='width:96%;margin-left:2%'><thead><tr style='text-align:center;'><th></th><th>Hr</th><th>Min</th><th>Sec</th><th colspan='2'>Date</th></tr>";
		
        attacktabbody+="<tr><td>Set Time: </td><td><input id='attackHr' type='number' style='width: 35px;height: 22px;font-size: 10px;' value='10'></td><td><input id='attackMin' style='width: 35px;height: 22px;font-size: 10px;' type='number' value='00'></td>";
		
        attacktabbody+="<td><input style='width: 35px;height: 22px;font-size: 10px;' id='attackSec' type='number' value='00'></td><td colspan='2'><input style='width:90px;' id='attackDat' type='text' value='00/00/0000'></td></tr></tbody></table>";
		
        attacktabbody+="<table style='margin-left: 10%; margin-top:20px;'><tbody><tr><td style='width: 20%'><button id='Attack' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Attack!</button></td>";
		
        attacktabbody+="<td style='width: 20%'><button id='Aexport' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Export Order</button></td>";
		
        attacktabbody+="<td style='width: 20%'><button id='Aimport' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Import Order</button></td></tr></tbody></table>";
        // defend tab
		var deftab = "<li id='deftab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warDefmanager'";
		
        deftab+="aria-labeledby='ui-id-22' aria-selected='false' aria-expanded='false'>";
		
        deftab+="<a href='#warDefmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-22'>Defend</a></li>";
        //defense body
		var deftabbbody = "<div id='warDefmanager' aria-labeledby='ui-id-21' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		
        deftabbbody+=" role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Defense Sender:</div>";
		
        deftabbbody+="<div><p style='font-size: 10px;'>Defense sender will split all the troops you choose to send according to the number of targets you input.</p></div>";
		
        deftabbbody+="<div id='defbox' class='beigemenutable scroll-pane' style='width: 53%; height: 50%; float:left; margin-left: 1%; margin-right: 1%;'>";
		
        deftabbbody+="<table><thead><th></th><th>X</th><th>Y</th></thead><tbody>";
        for (let i=1;i<15;i++) {
			
            deftabbbody+="<tr><td>Target "+i+" </td><td><input id='d"+i+"x' type='number' style='width: 85%'></td><td><input id='d"+i+"y' type='number' style='width: 85%'></td></tr>";
		}
		
        deftabbbody+="</tbody></table></div>";
		
        deftabbbody+="<div id='dpicktype' class='beigemenutable scroll-pane' style='width: 43%; height: 50%;'></div>";
        deftabbbody+="<table><tr><td><span>Use percentage of troops:</span></td><td><input id='defperc' type='number' style='width: 30px'>%</td><td></td></tr>";
        deftabbbody+="<tr><td><span>Select Departure:</span></td><td><select id='defdeparture' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>";
        deftabbbody+="<option value='0'>Now</option><option value='1'>Departure time</option><option value='2'>Arrival time</option></select></td><td></td></tr>";
        deftabbbody+="<tr id='dret'><td><input id='dretcheck' class='clsubopti' type='checkbox' checked> Return all Troops</td><td colspan=2><input id='dretHr' type='number' style='width: 20px' value='2'> Hours before departure</td></tr></table>";
        deftabbbody+="<table id='deftime' style='width:96%;margin-left:2%'><thead><tr style='text-align:center;'><th></th><th>Hr</th><th>Min</th><th>Sec</th><th colspan='2'>Date</th></tr>";
        deftabbbody+="<tr><td>Set Time: </td><td><input id='defHr' type='number' style='width: 35px;height: 22px;font-size: 10px;' value='10'></td><td><input id='defMin' style='width: 35px;height: 22px;font-size: 10px;' type='number' value='00'></td>";
        deftabbbody+="<td><input style='width: 35px;height: 22px;font-size: 10px;' id='defSec' type='number' value='00'></td><td colspan='2'><input style='width:90px;' id='defDat' type='text' value='00/00/0000'></td></tr></tbody></table>";
        deftabbbody+="<button id='Defend' style='width: 35%;height: 30px; font-size: 12px; margin:10px;' class='regButton greenb'>Send Defense</button>";
	    var ndeftab="<li id='neardeftab' class='ui-state-default ui-corner-top' role='tab'>";
        ndeftab+="<a href='#warNdefmanager' class='ui-tabs-anchor' role='presentation'>Near Def</a></li>";
        var ndeftabbody="<div id='warNdefmanager' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        ndeftabbody+=" role='tabpanel' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Nearest defense:</div>";
        ndeftabbody+="<table><td>Choose city:</td><td><input style='width: 30px;height: 22px;font-size: 10px;' id='ndefx' type='number'> : <input style='width: 30px;height: 22px;font-size: 10px;' id='ndefy' type='number'></td>";
        ndeftabbody+="<td>Showing For:</td><td id='asdfgh' class='coordblink shcitt'></td>";        
        ndeftabbody+="<td><button class='regButton greenb' id='ndefup' style='height:30px; width:70px;'>Update</button></td></table>";
        ndeftabbody+="<div id='Ndefbox' class='beigemenutable scroll-pane' style='width: 96%; height: 85%; margin-left: 2%;'></div>";
        var nofftab="<li id='nearofftab' class='ui-state-default ui-corner-top' role='tab'>";
        nofftab+="<a href='#warNoffmanager' class='ui-tabs-anchor' role='presentation'>Offensive TS</a></li>";
        var nofftabbody="<div id='warNoffmanager' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
        nofftabbody+=" role='tabpanel' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >ALL Offensive TS:</div>";
        nofftabbody+="<table><td colspan='2'> Continent(99 for navy):</td><td><input style='width: 30px;height: 22px;font-size: 10px;' id='noffx' type='number' value='0'>";
        nofftabbody+="<td><button class='regButton greenb' id='noffup' style='height:30px; width:70px;'>Update</button></td>";
        nofftabbody+="<td id='asdfg' style='width:10% !important;'></td><td><button class='regButton greenb' id='mailoff' style='height:30px; width:50px;'>Mail</button></td><td><input style='width: 100px;height: 22px;font-size: 10px;' id='mailname' type='text' value='Name_here;'></table>"
        nofftabbody+="<div id='Noffbox' class='beigemenutable scroll-pane' style='width: 96%; height: 85%; margin-left: 2%;'></div>";
        var expwin="<div id='ExpImp' style='width:250px;height:200px;' class='popUpBox ui-draggable'><div class=\"popUpBar\"><span class=\"ppspan\">Import/Export attack orders</span>";
        expwin+="<button id=\"cfunkyX\" onclick=\"$('#ExpImp').remove();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div id='expbody' class=\"popUpWindow\">";
        expwin+="<textarea style='font-size:11px;width:97%;margin-left:1%;height:17%;' id='expstring' maxlength='200'></textarea><button id='applyExp' style='margin-left: 15px; width: 100px;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Apply</button></div></div>";

		var tabs = $("#warcouncTabs").tabs();
		var ul = tabs.find("ul");
		$(bosstab).appendTo(ul);
		$(attacktab).appendTo(ul);
		$(deftab).appendTo(ul);
		$(ndeftab).appendTo(ul);
		$(nofftab).appendTo(ul);
		tabs.tabs("refresh");
        $('#warCidlemanager').after(bosstabbody);
        $('#warCidlemanager').after(attacktabbody);
        $('#warAttackmanager').after(deftabbbody);
        $('#warDefmanager').after(ndeftabbody);
        $('#warNdefmanager').after(nofftabbody);
		$("#senddefCityGo").after(quickdefbut);
		$("#deftime").hide();
		$("#dret").hide();
		$("#warCounc").append(returnAllbut);
		$("#warCounc").append(attackbut);
		$("#warCounc").append(defbut);
		$("#warCounc").append(neardefbut);
		$("#warCounc").append(nearoffbut);
		$("#coordstochatGo1").after(addtoatt);
		$("#addtoAtt").after(addtodef);
		$("#loccavwarconGo").css("right", "65%");
		$("#idluniwarconGo").css("left", "34%");
		$("#idluniwarconGo").after(raidbossbut);
        $("#defdeparture").change(function() {
			if ($("#defdeparture").val() == 0) {
				$("#deftime").hide();
				$("#dret").hide();
			} else {
				$("#deftime").show();
				$("#dret").show();
			}
		});

        if (localStorage.getItem('attperc')) {
            $("#perc").val(localStorage.getItem('attperc'));
        } else {$("#perc").val(99);}
        if (localStorage.getItem('defperc')) {
            $("#defperc").val(localStorage.getItem('defperc'));
        } else {$("#defperc").val(99);}
        if (localStorage.getItem('retcheck')) {
			if (localStorage.getItem('retcheck') == '1') {
                $("#retcheck").prop( "checked", true );
            }
            if (localStorage.getItem('retcheck')=='0') {
                $("#retcheck").prop( "checked", false );
            }
        }
        if (localStorage.getItem('dretcheck')) {
			if (localStorage.getItem('rdetcheck') == '1') {
                $("#dretcheck").prop( "checked", true );
            }
			if (localStorage.getItem('dretcheck') == '0') {
                $("#dretcheck").prop( "checked", false );
            }
        }
        if (localStorage.getItem('retHr')) {
            $("#retHr").val(localStorage.getItem('retHr'));
        }
        if (localStorage.getItem('dretHr')) {
            $("#dretHr").val(localStorage.getItem('dretHr'));
        }
        $( "#attackDat" ).datepicker();
		$("#defDat").datepicker();
  //       $('#bosshuntab').click(function() {
  //          if (beentoworld)
  //          {
  //              openbosswin();
		//	} else {
		//		alert("Press World Button");
		//	}
		//});
  //      $('#returnAllb').click(function() {
  //          jQuery.ajax({url: 'includes/gIDl.php',type: 'POST',
  //                       success: function(data) {
  //                           var thdata=JSON.parse(data);
		//			$("#returnAll").remove();
  //                           openreturnwin(thdata);
		//		}
		//	});
		//});
  //      $('#raidbossGo').click(function() {
  //          if (beentoworld)
  //          {
		//		$("#warcouncbox").show();
		//		tabs.tabs("option", "active", 2);
		//		$("#bosshuntab").click();
		//	} else {
		//		alert("Press World Button");
		//	}
		//});
		$("#Attack").click(function () {
            localStorage.setItem('attperc',$("#perc").val() as string );
			localStorage.setItem('retHr', $("#retHr").val() as string);
            if ($("#retcheck").prop( "checked")==true) {
                localStorage.setItem('retcheck',1);
            }
            if ($("#retcheck").prop( "checked")==false) {
                localStorage.setItem('retcheck',0);
            }
            SendAttack();
        });
		$("#Defend").click(function () {
			localStorage.setItem('defperc', $("#defperc").val() as string);
			localStorage.setItem('dretHr', $("#dretHr").val() as string);
            var defobj={targets:{x:[],y:[],dist:[],numb:0,cstr:[]},t:{tot:[],home:[],type:[],use:[],speed:[],amount:[]},perc:$("#defperc").val(),dep:$("#defdeparture").val(),ret:1,rettime:$("#dretHr").val(),hr:$("#defHr").val(),min:$("#defMin").val(),sec:$("#defSec").val(),date:$("#defDat").val(),dat:$("#defDat").datepicker('getDate')};
            if ($("#dretcheck").prop( "checked")==true) {
                localStorage.setItem('dretcheck',1);
                defobj.ret=1;
            }
            if ($("#dretcheck").prop( "checked")==false) {
                localStorage.setItem('dretcheck',0);
                defobj.ret=0;
            }
            var tempx;
            var tempy;
            for (let i=1;i<15;i++) {
                if ($("#d"+i+"x").val()) {
                    tempx=$("#d"+i+"x").val();
                    tempy=$("#d"+i+"y").val();
                    defobj.targets.x.push(tempx);
                    defobj.targets.y.push(tempy);
                    defobj.targets.cstr.push(tempx+":"+tempy);
                    defobj.targets.dist.push(Math.sqrt((tempx-D6.x)*(tempx-D6.x)+(tempy-D6.y)*(tempy-D6.y)));
                    defobj.targets.numb++;
                }
            }
            for (let i in D6.tc) {
                if (D6.tc[i]) {
                    defobj.t.tot.push(Math.ceil(D6.tc[i]*Number($("#defperc").val())/100));
                    defobj.t.home.push(Math.ceil(D6.th[i]*Number($("#defperc").val())/100));
                    defobj.t.type.push(Number(i));
                    if ($("#usedef"+i).prop( "checked")==true) {
                        defobj.t.speed.push(ttspeed[i]/ttSpeedBonus[i]);
                        defobj.t.use.push(1);
                    } else {
                        defobj.t.speed.push(0)
                        defobj.t.use.push(0);
                    }
                    defobj.t.amount.push(0);
                }
            }
            SendDef(defobj);
        });
        $('#attackGo').click(function() {
			$("#warcouncbox").show();
			tabs.tabs("option", "active", 3);
			jQuery("#attacktab")[0].click();
			setTimeout(updateattack,200);
		});
        $('#defGo').click(function() {
			$("#warcouncbox").show();
			tabs.tabs("option", "active", 4);
			$("#deftab").click();
			setTimeout( updatedef,200);
		});
	    $('#ndefGo').click(function() {
			NearDefSubscribe();
			$("#warcouncbox").show();
			tabs.tabs("option", "active", 5);
            $("#neardeftab").trigger({type:"click",originalEvent:"1"});
        });
        $('#neardeftab').click(function() {
			NearDefSubscribe();

		});
        $('#ui-id-115').click(function() {
			NearDefSubscribe();

        });
        $('#noffGo').click(function() {
			$("#warcouncbox").show();
			tabs.tabs("option", "active", 6);
            $("#nearofftab").trigger({type:"click",originalEvent:"1"});
		});
		$("#addtoAtt").click(function () {
			let tid = Number($("#showReportsGo").attr("data"));
			addToAttackSender(tid);
		});
        $("#addtoDef").click(function() {
            for (let i=1;i<15;i++) {
                if (!$("#d"+i+"x").val()) {
                    var tid=Number($("#showReportsGo").attr("data"));
                    var tempx;
                    var tempy;
                    tempx=Number(tid % 65536);
                    tempy=Number((tid-tempx)/65536);
                    $("#d"+i+"x").val(tempx);
                    $("#d"+i+"y").val(tempy);
					break;
				}
			}
		});
		$("#quickdefCityGo").click(function () {
			var tid = Number($("#showReportsGo").attr("data"));
			var tempx;
			var tempy;
			tempx = Number(tid % 65536);
			tempy = Number((tid - tempx) / 65536);
			var defobj = { targets: { x: [tempx], y: [tempy], dist: [], numb: 1 }, t: { home: [], type: [], use: [], speed: [], amount: [] }, perc: 100, dep: 0, ret: 0, rettime: 0, hr: 0, min: 0, sec: 0, dat: 0 };
            defobj.targets.dist.push(Math.sqrt((tempx-D6.x)*(tempx-D6.x)+(tempy-D6.y)*(tempy-D6.y)));
            for (let i in D6.th) {
                if (D6.th[i]) {
                    defobj.t.home.push(Math.ceil(D6.th[i]*Number($("#defperc").val())/100));
					defobj.t.type.push(Number(i));
					defobj.t.speed.push(ttspeed[i] / ttSpeedBonus[i]);
					defobj.t.use.push(1);
					defobj.t.amount.push(0);
				}
			}
			SendDef(defobj);
		});
        $("#ndefup").click(function() {
            var tempxs=Number($("#ndefx").val());
            var tempys=Number($("#ndefy").val());
			var tids = tempxs + (tempys * 65536);
			$("#asdfgh").data(tids.toString());
            $("#asdfgh").text(tempxs+":"+tempys);
            jQuery.ajax({url: 'overview/trpover.php',type: 'POST',

                         success: function(data) {
                             var t=JSON.parse(data);
                             neardeftable(t);
				}
			});
		});
        $("#noffup").click(function() {
            jQuery.ajax({url: 'overview/trpover.php',type: 'POST',

                         success: function(data) {
                             var t=JSON.parse(data);
                             nearofftable(t);
				}
			});
		});
        $("#Aexport").click(function() {
            var Aexp={x:[],y:[],type:[],time:[]};
            for (let i=1;i<16;i++) {
                if ($("#t"+i+"x").val()) {
                    Aexp.x.push($("#t"+i+"x").val());
                    Aexp.y.push($("#t"+i+"y").val());
                    Aexp.type.push($("#type"+i).val());
				}
			}
            Aexp.time[0]=$("#attackHr").val();
            Aexp.time[1]=$("#attackMin").val();
            Aexp.time[2]=$("#attackSec").val();
			Aexp.time[3] = $("#attackDat").val();

			const wrapper = {
				aexp: Aexp
			};
			window['external']['notify'](JSON.stringify(wrapper));

    //        var aa=prompt("Attack Orders Expot", JSON.stringify(Aexp));

		});
        $("#Aimport").click(function() {
            $("body").append(expwin);
            $("#ExpImp").draggable({ handle: ".popUpBar" , containment: "window", scroll: false});
            document.addEventListener('paste', function (evt) {
                $("#expstring").val(evt.clipboardData.getData('text/plain'));
			});
            $("#applyExp").click(function() {
                Aimp($("#expstring").val());
				$("#ExpImp").remove();
			});
		});
		
		var fourbutton = "<div id='fourbuttons' class='commandinndiv'><div><button id='fb1' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>flip q</button><button id='fb2' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Refine</button><button id='fb3' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Raid</button><button id='fb4' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Demolish</button></div></div>";

	//
	//	var bdcountbox_ = "<div id='currentBd'><div id='bdcountbar' class='queueBar'>";
	//	
	//	bdcountbox_ = `${bdcountbox_}<div id='bdcountbut' class='tradeqarr2'><div></div></div><span class='qbspan'>Current Buildings</span>`;
	//
	//	bdcountbox_ = `${bdcountbox_}<div id='numbdleft' class='barRightFloat tooltipstered'>0</div>`;

	//
	//	bdcountbox_ = `${bdcountbox_}</div><div id='bdcountwin' class='queueWindow' style='display: block;'></div></div>`;
		//$("#recruitmentQueue").before(bdcountbox_);
		//$("#bdcountbut").click(() => {
		//	if (bdcountshow_) {
		//		$("#bdcountwin").hide();
		//		$("#bdcountbut").removeClass("tradeqarr2").addClass("tradeqarr1");
		//		/** @type {boolean} */
		//		bdcountshow_ = false;
		//	} else {
		//		$("#bdcountwin").show();
		//		$("#bdcountbut").removeClass("tradeqarr1").addClass("tradeqarr2");
		//		/** @type {boolean} */
		//		bdcountshow_ = true;
		//	}
		//});


		$("#buildQueue").before(fourbutton);
		
		var fillbut = '<button id="fillque" class="greenb tooltipstered" style="height:18px; width:40px; margin-left:7px; margin-top:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;">Fill</button>';
		$("#sortbut").after(fillbut);
		$("#fillque").click(() => {
			OverviewPost('overview/fillq.php', { a: cotg.city.id() });
			event.stopPropagation();
		});
		
		var convbut_ = '<button id="convque" class="greenb tooltipstered" style="height:18px; width:60px; margin-left:7px; margin-top:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;">Convert</button>';
		$("#sortbut").after(convbut_);
		$("#convque").click(() => {
			OverviewPost('overview/mconv.php', { a: cotg.city.id() });
			event.stopPropagation();
		});
		$("#fb1").click(function () {

			if ($("#buildQueueWindow").is(":visible")) {
				$("#commandWindow").hide();
				$("#recruitmentQueueWindow").hide();
				$("#favoredDiv").hide();
			//	$("#citycitadeldiv").hide();
				$("#buildQueueWindow").hide();
				$("#defensesWindow").hide();
				$("#reinforcementsWindow").hide();
				$("#tradesWindow").hide();
				//		$("#quecontent").children(".tradeqarr2").each(function () { $(this).removeClass("tradeqarr2").addClass("tradeqarr1") });
				$(".tradeqarr2").removeClass("tradeqarr2").addClass("tradeqarr1"); //
				$("#increstable").hide();

				//$("#topIRsecd2s").hide();
			}
			else {
					$("#commandWindow").show();
					$("#recruitmentQueueWindow").show();
					$("#favoredDiv").show();
				//	$("#citycitadeldiv").show();
					$("#buildQueueWindow").show();
					$("#defensesWindow").show();
					$("#reinforcementsWindow").show();
					$("#tradesWindow").show();
					//		$("#quecontent").children(".tradeqarr2").each(function () { $(this).removeClass("tradeqarr2").addClass("tradeqarr1") });
					$(".tradeqarr1").removeClass("tradeqarr1").addClass("tradeqarr2"); //
					$("#increstable").show();
				//	$("#topIRsecd2s").show();

			}
			//  $("#councillorPopUpBox").show();
		//  jQuery("#ui-id-11")[0].click();
		//  jQuery("#couonoffdv")[0].click();
		//  setTimeout(() => {
		//	jQuery("#councillorXbutton")[0].click();
		//  }, 100);
		//  if (coon_ == 0) {
		//	
		//	coon_ = 1;
		//	$(this).removeClass("greenb");
		//	$(this).addClass("redb");
		//  } else {
		//	
		//	coon_ = 0;
		//	$(this).removeClass("redb");
		//	$(this).addClass("greenb");
		//  }
		});
        $("#fb2").click(function() {
            $('#tradePopUpBox').show();
            setTimeout(function(){
				jQuery("#ui-id-27")[0].click();
			}, 100);
		});
        $("#fb3").click(function() {
            $('#warcouncbox').show();
			jQuery("#ui-id-19")[0].click();
		});
		
		$("#fb4").click(function () {
			setAutoDemo(!__autodemoon_);
		});

		$("#centarrowNextDiv").click(() => {
			
			setAutoDemo(false);
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		$("#centarrowPrevDiv").click(() => {
			
			setAutoDemo(false);
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		$("#ddctd").click(() => {
			
			setAutoDemo(false);
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		$("#qbuildtbButton").click(() => {
			
			setAutoDemo(false);
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		
		var sumbut = "<button class='tabButton' id='Sum'>Summary</button>";
		$("#items").after(sumbut);
		$("#Sum").click(() => {
			if (sum_) {
				opensumwin_();
			} else {
				$("#sumWin").show();
			}
		});
		$("#sumWin").click(() => {
			console.log("popsum");
		});
		
		var wood50_ = "<td><button class='brownb' id='wood50'>Add 50%</button></td>";
		$("#woodmaxbutton").parent().after(wood50_);
		$("#wood50").click(() => {
			
			var res_3 = AsNumber($("#maxwoodsend").text().replace(/,/g, ""));
			if ($("#landseasendres").val() == "1") {
				
				var carts_ = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
			} else {
				
				carts_ = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
			}
			if (res_3 > carts_) {
				
				res_3 = carts_;
			}
			$("#woodsendamt").val(res_3);
		});
		
		var stone50 = "<td><button class='brownb' id='stone50'>Add 50%</button></td>";
		$("#stonemaxbutton").parent().after(stone50);
		$("#stone50").click(() => {
			if ($("#landseasendres").val() == "1") {
				
				var carts_1 = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
			} else {
				
				carts_1 = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
			}
			
			var res_4 = AsNumber($("#maxstonesend").text().replace(/,/g, ""));
			if (res_4 > carts_1) {
				
				res_4 = carts_1;
			}
			$("#stonesendamt").val(res_4);
		});
		
		var iron50 = "<td><button class='brownb' id='iron50'>Add 50%</button></td>";
		$("#ironmaxbutton").parent().after(iron50);
		$("#iron50").click(() => {
			
			var res_5 = AsNumber($("#maxironsend").text().replace(/,/g, ""));
			if ($("#landseasendres").val() == "1") {
				
				var carts_2 = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
			} else {
				
				carts_2 = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
			}
			if (res_5 > carts_2) {
				
				res_5 = carts_2;
			}
			$("#ironsendamt").val(res_5);
		});
		
		var food50_ = "<td><button class='brownb' id='food50'>Add 50%</button></td>";
		$("#foodmaxbutton").parent().after(food50_);
		$("#food50").click(() => {
			
			var res_6 = AsNumber($("#maxfoodsend").text().replace(/,/g, ""));
			if ($("#landseasendres").val() == "1") {
				
				var carts_3 = Math.floor(AsNumber($("#availcartscity").text()) / 2) * 1000;
			} else {
				
				carts_3 = Math.floor(AsNumber($("#availshipscity").text()) / 2) * 10000;
			}
			if (res_6 > carts_3) {
				
				res_6 = carts_3;
			}
			$("#foodsendamt").val(res_6);
		});
		
		//shrine planer part
		var shrinebut = "<button class='regButton greenb' id='shrineP' style='width: 98%;margins: 1%;'>Shrine Planner</button>";
		$("#inactiveshrineInfo").before(shrinebut);
		$("#shrineP").click(function () {
			if (beentoworld) {
				shrinec = [[]];
				splayers = { name: [], ally: [], cities: [] };
				var players = [];
				var coords = $("#coordstochatGo3").attr("data");
				var shrinex = parseInt(coords);
				var shriney = Number(coords.match(/\d+$/)[0]);
				var shrinecont = Number(Math.floor(shrinex / 100) + 10 * Math.floor(shriney / 100));
				for (let i in wdata.cities) {
					var tempx = Number(wdata.cities[i].substr(8, 3)) - 100;
					var tempy = Number(wdata.cities[i].substr(5, 3)) - 100;
					var cont = Number(Math.floor(tempx / 100) + 10 * Math.floor(tempy / 100));
					if (cont == shrinecont) {
						var dist = Math.sqrt((tempx - shrinex) * (tempx - shrinex) + (tempy - shriney) * (tempy - shriney));
						//console.log("dist");
						if (dist < 10) {
							var l = Number(wdata.cities[i].substr(11, 1));
							var pid = Number(wdata.cities[i].substr(12, l));
							var pname = pldata[pid];
							//console.log(pname);
							//console.log(splayers.name.indexOf(pname),pname,splayers.name);
							var csn = [3, 4, 7, 8];
							if (csn.indexOf(Number(wdata.cities[i].charAt(4))) > -1) {
								shrinec.push(["castle", pname, 0, tempx, tempy, dist, "0", 0, 0, 0]);
							} else {
								shrinec.push(["city", pname, 0, tempx, tempy, dist, "0", 0, 0, 0]);
							}
						}
					}
				}
				shrinec.sort(function (a, b) { return a[5] - b[5]; });
				var planwin = "<div id='shrinePopup' style='width:40%;height:50%;left: 360px; z-index: 3000;' class='popUpBox'><div class='popUpBar'><span class=\"ppspan\">Shrine Planner</span><button id='hidec' class='greenb' style='margin-left:10px;border-radius: 7px;margin-top: 2px;height: 28px;'>Hide Cities</button>";
				planwin += "<button id='addcity' class='greenb' style='margin-left:10px;border-radius: 7px;margin-top: 2px;height: 28px;'>Add City</button><button id=\"sumX\" onclick=\"$('#shrinePopup').remove();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div class=\"popUpWindow\" style='height:100%'>";
				planwin += "<div id='shrinediv' class='beigemenutable scroll-pane' style='background:none;border: none;padding: 0px;height:90%;'></div></div>";
				for (let i in shrinec) {
					if (i < 101) {
						var pname = shrinec[i][1];
						if (players.indexOf(pname) == -1) {
							players.push(pname);
							jQuery.ajax({
								url: 'includes/gPi.php', type: 'POST', data: { a: pname },
								success: function (data) {
									var pinfo = JSON.parse(data);
									splayers.name.push(pinfo.player);
									splayers.ally.push(pinfo.a);
									splayers.cities.push(pinfo.h);
									//console.log(pinfo.a,pinfo.h,pinfo.player);
								}
							});
						}
					}
				}
				setTimeout(function () {
					$("#reportsViewBox").after(planwin);
					$("#shrinePopup").draggable({ handle: ".popUpBar", containment: "window", scroll: false });
					$("#shrinePopup").resizable();
					if (localStorage.getItem("hidecities")) {
						1 == 1;
					} else {
						//console.log("hideciies nonexists");
						localStorage.setItem("hidecities", "0");
					}
					if (localStorage.getItem("hidecities") == "1") {
						$("#hidec").html("Show Cities");
					}
					$("#hidec").click(function () {
						if (localStorage.getItem("hidecities") == "0") {
							hidecities();
							localStorage.setItem("hidecities", "1");
							$("#hidec").html("Show Cities");
						} else if (localStorage.getItem("hidecities") == "1") {
							showcities();
							localStorage.setItem("hidecities", "0");
							$("#hidec").html("Hide Cities");
						}
					});
					updateshrine();
					var addcitypop = "<div id='addcityPopup' style='width:500px;height:100px;left: 360px; z-index: 3000;' class='popUpBox'><div class='popUpBar'><span class=\"ppspan\">Add City</span>";
					addcitypop += "<button id=\"sumX\" onclick=\"$('#addcityPopup').remove();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div class=\"popUpWindow\" style='height:100%'>";
					addcitypop += "<div><table><td>X: <input id='addx' type='number' style='width: 35px;height: 22px;font-size: 10px;'></td><td>y: <input id='addy' type='number' style='width: 35px;height: 22px;font-size: 10px;'></td>";
					addcitypop += "<td>score: <input id='addscore' type='number' style='width: 45px;height: 22px;font-size: 10px;'></td><td>Type: <select id='addtype' class='greensel' style='font-size: 15px !important;width:55%;height:30px;'>";
					addcitypop += "<option value='city'>City</option><option value='castle'>Castle</option></select></td><td><button id='addadd' class='greenb'>Add</button></td></table></div></div>";
					$("#addcity").click(function () {
						$("body").append(addcitypop);
						$("#addcityPopup").draggable({ handle: ".popUpBar", containment: "window", scroll: false });
						$("#eeaddadd").click(function () {
							tempx = $("#addx").val();
							tempy = $("#addy").val();
							dist = Math.sqrt((tempx - shrinex) * (tempx - shrinex) + (tempy - shriney) * (tempy - shriney));
							var temp = [$("#addtype").val(), "Poseidon", "Atlantis", tempx, tempy, dist, "1", $("#addscore").val(), "Hellas", "1"];
							shrinec.push(temp);
							shrinec.sort(function (a, b) { return a[5] - b[5]; });
							updateshrine();
							$("#addcityPopup").remove();
						});
					});
				}, 2000);
			} else {
				PostgWrd();
				alert("Please try again in 5 seconds");
			}
		});
	
		var incomingtabledata_ = $("#incomingsAttacksTable").children().children().children();
		$("#incomingsAttacksTable table thead tr th:nth-child(2)").width(140);
		
		var Addth_ = "<th>Lock time</th>";
		incomingtabledata_.append(Addth_);
		
		var Addth1_ = "<th>Travel time</th>";
		incomingtabledata_.append(Addth1_);
		$("#allianceIncomings").parent().click(() => {
			setTimeout(() => {
				incomings();
			}, 5000);
		});
		$("#incomingsPic").click(() => {
			setTimeout(() => {
				incomings();
			}, 5000);
		});


		
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
				incomings();
			}, 4000);
		});
		$("#ui-id-37").click(() => {
			setTimeout(() => {
				incomings();
			}, 1000);
		});

		if (localStorage.getItem("raidbox") != null) {
			
			var raidboxback_ = "<button class='regButton greenb' id='raidboxb' style='width:120px; margin-left: 2%;'>Return Raiding Box</button>";
			$("#squaredung td").find(".squarePlayerInfo").before(raidboxback_);
			$("#raidboxb").click(() => {
				localStorage.removeItem("raidbox");
				$("#raidboxb").remove();
			});
		}
		
        // var cancelallya="<input id='cancelAllya' type='checkbox' checked='checked'> Cancel attack if same alliance";
		
        // var cancelallys="<input id='cancelAllys' type='checkbox' checked='checked'> Cancel attack if same alliance";
		
        // var cancelallyp="<input id='cancelAllyp' type='checkbox' checked='checked'> Cancel attack if same alliance";
		
        // var cancelallyc="<input id='cancelAllyc' type='checkbox' checked='checked'> Cancel attack if same alliance";
        // $("#assaulttraveltime").parent().next().html(cancelallya);
        // $("#siegetraveltime").parent().next().html(cancelallys);
        // $("#plundtraveltime").parent().next().html(cancelallyp);
        // $("#scouttraveltime").parent().next().html(cancelallyc);
        $("#assaultGo").click(function() {
			if ($("#cancelAllya").prop("checked") == false) {
				setTimeout(() => {
					$(".shAinf").each(function () {
						let tid_7 = ToInt($(this).parent().next().find(".cityblink").attr("data"));
						
						var tx_1 = tid_7 % 65536;
						
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
								data: dat_7
							});
						}
					});
					$(".shPinf").each(function () {
						let a = $(this).parent().next().find(".cityblink");
						let tid_8 = GetIntData(a);
						
						var tx_2 = tid_8 % 65536;
						
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
						
						var tx_3 = tid_9 % 65536;
						
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
						
						var tx_4 = tid_10 % 65536;
						
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
						
						var tx_5 = tid_11 % 65536;
						
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
						
						var tx_6 = tid_12 % 65536;
						
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
						
						var tx_7 = tid_13 % 65536;
						
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
						
						let tx_8 = cid.x;
						
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
		
		var layoutopttab_ = "<li id='layoutopt' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='layoutoptBody'";
		
		layoutopttab_ = `${layoutopttab_}aria-labeledby='ui-id-60' aria-selected='false' aria-expanded='false'>`;
		
		layoutopttab_ = `${layoutopttab_}<a href='#layoutoptBody' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-60'>Layout Options</a></li>`;
		
		var layoutoptbody_ = "<div id='layoutoptBody' aria-labeledby='ui-id-60' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		
		layoutoptbody_ = `${layoutoptbody_} role='tabpanel' aria-hidden='true' style='display: none;'><table><tbody><tr><td><input id='addnotes' class='clsubopti' type='checkbox'> Add Notes</td>`;
		
		layoutoptbody_ = `${layoutoptbody_}<td><input id='addtroops' class='clsubopti' type='checkbox'> Add Troops</td></tr><tr><td><input id='addtowers' class='clsubopti' type='checkbox'> Add Towers</td><td><input id='addbuildings' class='clsubopti' type='checkbox'> Upgrade Cabins</td>`;
		
		layoutoptbody_ = `${layoutoptbody_}<td> Cabin Lvl: <input id='cablev' type='number' style='width:22px;' value='7'></td></tr><tr><td><input id='addwalls' class='clsubopti' type='checkbox'> Add Walls</td>`;
		
		layoutoptbody_ = `${layoutoptbody_}<td><input id='addhub' class='clsubopti' type='checkbox'> Set Nearest Hub With layout</td></tr><tr><td>Select Hubs list: </td><td id='selhublist'></td><td>`;
		
		layoutoptbody_ = `${layoutoptbody_}<button id='nearhubAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Set Nearest Hub</button><button id='infantryAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Infantry setup</button></td></tr></tbody></table>`;
		
		layoutoptbody_ = `${layoutoptbody_}<table><tbody><tr><td colspan='2'><input id='addres' class='clsubopti' type='checkbox'> Add Resources:</td><td id='buttd' colspan='2'></td></tr><tr><td>wood<input id='woodin' type='number' style='width:100px;' value='200000'></td><td>stone<input id='stonein' type='number' style='width:100px;' value='220000'></td>`;
		
		layoutoptbody_ = `${layoutoptbody_}<td>iron<input id='ironin' type='number' style='width:100px;' value='200000'></td><td>food<input id='foodin' type='number' style='width:100px;' value='350000'></td></tr>`;
		
		layoutoptbody_ = `${layoutoptbody_}</tbody></table></div>`;
		
		var layoptbut_ = "<button id='layoptBut' class='regButton greenb' style='width:150px;'>Save Res Settings</button>";
		var tabs_1 = $("#CNtabs").tabs();
		var ul_1 = tabs_1.find("ul");
		$(layoutopttab_).appendTo(ul_1);
		tabs_1.tabs("refresh");
		$("#CNtabs").append(layoutoptbody_);
		$("#buttd").append(layoptbut_);
		$("#nearhubAp").click(() => {
			setnearhub(null);
		});
		$("#infantryAp").click(() => {
			setinfantry_();
		});
		$("#layoptBut").click(() => {
			localStorage.setItem("woodin", $("#woodin").val().toString());
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
				
				var i_53 = 20;
				for (; i_53 < currentlayout_.length - 20; i_53++) {
					var tmpchar_ = currentlayout_.charAt(i_53);
					/** @type {!RegExp} */
					var cmp_ = new RegExp(tmpchar_);
					if (!cmp_.test(emptyspots_)) {
						currentlayout_ = ReplaceAt(currentlayout_, i_53, "-");
					}
				}
				
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
					string: "[ShareString.1.3];########################BB-----#-------#####-JJ-----#--------###BBBBB----#---------##JJJJJ----#---------##eeBBBBBB#######------##JJJJJ##BBBBB##-----##BBBB##JJJJJJJ##----##----#BBBBBBBBB#----##----#JJJJJJJJJ#----#######BBBBTBBBB#######----#JJJJJJJJJ#----##----#BBBBBBBBB#----##----##JJJJJJJ##----##-----##BBBBB##-----##------#######--__--##--------M#X---_##_-##--------S#----_###_###--------#-----_#######-------#------_########################",
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
				
				var selectbuttsdf_ = '<select id="dfunkylayout" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:30%;" class="regButton greenb"><option value="0">Prefered build layout</option>';
				
				var ww_ = 1;
				var prefered_;
				for (prefered_ in prefered_data_) {
				//	console.log(prefered_data_[prefered_]);
					
					selectbuttsdf_ = `${selectbuttsdf_}<option value="${ww_}">${prefered_data_[prefered_].name}</option>`;
					layoutdf_.push(prefered_data_[prefered_].string);
					remarkdf_.push(prefered_data_[prefered_].remarks);
					notedf_.push(prefered_data_[prefered_].notes);
					troopcound_.push(prefered_data_[prefered_].troop_count);
					resd_.push(prefered_data_[prefered_].res_count);
					ww_++;
				}
				
				selectbuttsdf_ = `${selectbuttsdf_}</select>`;
				
				var selectbuttsw_ = '<select id="funkylayoutw" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Select water layout</option>';
				
				var cww_ = 1;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">2 sec rang/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##GBGBGBG##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##GBGBGBG##G---##-----##BGGGB##BBBBG##------#######BBVVBB##---------#--GBV##VB##---------#--GBV###V###--------#---BBV#######-------#----BBV########################");
				remarksw_.push("rangers/triari/galley");
				notesw_.push("166600 inf and 334 galley @ 10 days");
				troopcounw_.push([0, 0, 83300, 83300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 334, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">6 sec arbs/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SPJX##----#BEBEBEBEB#MH--##----##EBEBEBE##----##-----##BEBEB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#--BBBVTT#####-------#--BEBBV########################");
				remarksw_.push("arbs/galley");
				notesw_.push("88300 inf and 354 galley @ 11.5 days");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 88300, 0, 0, 0, 0, 0, 354, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">3 sec priestess/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#---H#######BZBZTZBZB#######----#BZBZBZBZB#JSPX##----#BZBZBZBZB#----##----##ZBZBZBZ##-Z--##-----##BZZZB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#--ZBV###V###--------#---BBV#######-------#---ZBBV########################");
				remarksw_.push("priestess/galley");
				notesw_.push("166600 inf and 334 galley @ 11 days");
				troopcounw_.push([0, 0, 0, 0, 166600, 0, 0, 0, 0, 0, 0, 0, 0, 0, 334, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">7 sec praetor/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SPJX##----#BZBZBZBZB#MH--##----##ZBZBZBZ##----##-----##BZBZB##BBBBZ##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#--BZBBV########################");
				remarksw_.push("praetors/galley");
				notesw_.push("86650 praetors and 347 galley @ 12 days");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 86650, 0, 0, 0, 0, 347, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">2 sec vanq/galley+senator</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##BBGBGBB##---B##-----##BGBGB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#--BBBBV########################");
				remarksw_.push("vanq/galley+senator");
				notesw_.push("193300 inf and 387 galley @ 10 days");
				troopcounw_.push([0, 0, 0, 0, 0, 193300, 0, 0, 0, 0, 0, 0, 0, 0, 387, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">5 sec horses/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#---H#######BEBETEBEB#######----#BEBEBEBEB#JSPX##----#BEBEBEBEB#-M--##----##EBEBEBB##----##-----##BEBEB##BBBB-##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#--BBBV#######-------#--BEBBV########################");
				remarksw_.push("horses/galley");
				notesw_.push("90000 cav and 360 galley @ 10.5 days");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90000, 0, 0, 0, 360, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">5 sec sorc/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##JBJBJ##-----##----##BJBJBJB##----##----#JBJBJBJBJ#----##----#JBJBJBJBJ#---H#######JBJBTBJBJ#######----#JBJBJBJBJ#-S-X##----#JBJBJBJBJ#----##----##BJBJBJB##JJ--##-----##JBJBJ##BBBBJ##------#######BBVVBB##---------#--JBV##VB##---------#--JBV###V###--------#---BBV#######-------#---JBBV########################");
				remarksw_.push("sorc/galley");
				notesw_.push("156600 sorc and 314 galley @ 13.5 days");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 156600, 0, 0, 0, 0, 0, 0, 0, 314, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">vanqs+ports+senator</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######----#BGBBBBBGB#PPJX##----#BGBGBGBGB#BBBB##----##BBGBGBB##BBBB##-----##BBBBB##BBBBB##------#######-BRRBB##---------#----R##RZ##---------#----R###R###--------#----SR#######-------#----MSR########################");
				remarksw_.push("vanqs+senator+ports");
				notesw_.push("264k infantry @ 10 days");
				troopcounw_.push([0, 0, 0, 100000, 0, 164000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">main hub</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#---PPPPP###---------#---PPPPPP##---------#---PPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##SLSDSAS##PPPP##----#-SDSMSDS-#PPPP##----#-SLSMSAS-#PPPP#######-SDSTSDS-#######----#-SLSMSAS-#----##----#-SDSMSDS-#----##----##SLSDSAS##----##-----##-----##-----##------#######--RR--##---------#ZB--RTTR-##---------#PJ--RTTTR###--------#-----RTT#####-------#------R########################");
				remarksw_.push("main hub");
				notesw_.push("14 mil w/s 23 mil iron 15 mil food 8200 carts 240 boats");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">palace storage</option>`;
				layoutsw_.push("[ShareString.1.3]:########################-------#-----PP#####--------#-----PPP###---------#-----PPPP##---------#-----PPPP##------#######--PPPP##-----##SASLS##-PPPP##----##ASASLSL##PPPP##----#SASASLSLS#-PPP##----#SASASLSLS#JPPP#######SASA#LSLS#######----#SASASLSLS#----##----#SASASLSLS#----##----##ASASLSL##----##-----##SASLS##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksw_.push("palace storage");
				notesw_.push("40 mil w/s 6200 carts");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">palace feeder</option>`;
				layoutsw_.push("[ShareString.1.3];########################-PPPPPP#PPPPPPP#####--PPPPPP#PPPPPPPP###---PPPPPP#PPPPPPPPP##---PPPPPP#PPPPPPPPP##----PP#######PPPPPP##-----##----J##PPPPP##----##-A-----##PPPP##----#-SSS-----#PPPP##----#-AAA-----#PPPP#######-SSST----#######----#-LLL-----#----##----#-SSS-----#----##----##-L-----##----##-----##-----##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
				remarksw_.push("palace feeder");
				notesw_.push("8.75 mil w/s 16400 carts");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">palace Hub mixed</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#PPPPPPP#####--------#PPPPPPPP###---------#PPPPPPPPP##---------#PPPPPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##-------##PPPP##----#SLSASLSAS#PPPP##----#SASLSASLS#JPPP#######SLSATLSAS#######----#SASLSASLS#----##----#SLSASLSAS#----##----##-------##----##-----##-----##-----##------#######--__--##---------#----_TT_-##---------#----_TTT_###--------#-----_TT#####-------#------_########################");
				remarksw_.push("palace Hub mixed");
				notesw_.push("24.57 mil w/s 11000 carts");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">Stingers</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######----T----#######----#---------#SPHX##----#---------#-M--##----##-------##----##-----##-----##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
				remarksw_.push("stingers");
				notesw_.push("3480 stingers @ 84 days");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3480, 0]);
				resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
				cww_++;
				
				selectbuttsw_ = `${selectbuttsw_}<option value="${cww_}">Warships</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######----T----#######----#---------#SPHX##----#---------#-M--##----##-------##----##-----##-----##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
				remarksw_.push("warships");
				notesw_.push("870 warships @ 42 days");
				troopcounw_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 870]);
				resw_.push([0, 0, 0, 0, 1, 500000, 500000, 500000, 500000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 500000, 500000, 500000, 500000]);
				
				selectbuttsw_ = `${selectbuttsw_}</select>`;
				
				var selectbuttsl_ = '<select id="funkylayoutl" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Select land layout</option>';
				
				var ll_1 = 1;
				
				var land_locked_data_ = [{
					name: "1 sec vanqs",
					string: "[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##GBGBG##-----##----##BGBGBGB##----##----#GBGBGBGBG#----##----#GBGBGBGBG#----#######GBGBTBGBG#######----#GBGBGBGBG#----##----#GBGBGBGBG#----##----##BGBGBGB##----##GGGGG##GBGBG##-----##BBBBB-#######------##GGGGGG--H#---------##eeBBBBBB--J#---------###GGGG---X#--------#####BB----S#-------########################",
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
					
					selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">${land_locked_data_[l_locked_].name}</option>`;
					layoutsl_.push(land_locked_data_[l_locked_].string);
					remarksl_.push(land_locked_data_[l_locked_].remarks);
					notesl_.push(land_locked_data_[l_locked_].notes);
					troopcounl_.push(land_locked_data_[l_locked_].troop_count);
					resl_.push(land_locked_data_[l_locked_].res_count);
					ll_1++;
				}
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">3 sec vanqs raiding</option>`;
				layoutsl_.push("[ShareString.1.3];########################----PJX#-------#####BB----PP#--------###BGBGB--SS#---------##BBBBB--MP#---------##BGBGB-#######------##BBBBB##BBBBB##-----##--G-##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBBB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
				remarksl_.push("vanqs");
				notesl_.push("296000 vanqs @ 10 days");
				troopcounl_.push([0, 0, 0, 0, 0, 296000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">2 sec rangers</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BGBGB-PP#--------###-BGBGB-MS#---------##-BGBGB--H#---------##-BGBGB#######------##--BBB##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######----#BGBGBGBGB#----##----#BGBGBGBGB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("rangers/triari");
				notesl_.push("236000 inf @ 6.5 days");
				troopcounl_.push([0, 0, 186000, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">3 sec priests</option>`;
				layoutsl_.push("[ShareString.1.3];########################-------#-----BB#####--------#----BBBB###---------#----BZZZB##---------#----BBBBB##------#######-BZZZB##-----##BZBZB##BBBBB##----##ZBZBZBZ##----##----#BZBZBZBZB#SP--##----#BZBZBZBZB#SP--#######BZBZTZBZB#######----#BZBZBZBZB#JX--##----#BZBZBZBZB#----##----##ZBZBZBZ##----##-----##BZBZB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
				remarksl_.push("priests");
				notesl_.push("224000 inf @ 7.7 days");
				troopcounl_.push([0, 0, 224000, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">6 sec praetors</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BZBZB-PP#--------###-BZBZB-MS#---------##-BZBZB--H#---------##-BZBZB#######------##--BBB##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#----##----#BZBZBZBZB#----##----##BBZBZBB##----##-----##BZBZB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("praetors");
				notesl_.push("110000 praetors @ 7.5 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 110000, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">4 sec horses</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--ZBB##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBE##----##-----##BEBEB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("horses");
				notesl_.push("106000 horses @ 5 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 106000, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">5 sec horses</option>`;
				layoutsl_.push("[ShareString.1.3]:########################-B---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB-PH#---------##-BEBEB#######------##--BBB##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBBTBBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("horses");
				notesl_.push("124000 horses @ 7 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 124000, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">5 sec arbs</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--BBB##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BEBEB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("arbs");
				notesl_.push("110000 arbs @ 6.5 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 110000, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">6 sec arbs</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BBBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--BBB##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("arbs");
				notesl_.push("124000 arbs @ 8.5 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 124000, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">4 sec sorc</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BJBJ--X#-------#####JBJBJ--S#--------###-JBJBJ--M#---------##-JBJBJ--H#---------##-JBJBJ#######------##-ZBJB##JBJBJ##-----##----##BJBJBJB##----##----#JBJBJBJBJ#----##----#JBJBJBJBJ#----#######JBJBTBJBJ#######----#JBJBJBJBJ#----##----#JBJBJBJBJ#----##----##BJBJBJB##----##-----##JBJBJ##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("sorc");
				notesl_.push("176000 sorc @ 8 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 176000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">5 sec sorc</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BBB---X#-------#####BJBJB--P#--------###-BJBJB-MS#---------##-BJBJB--H#---------##-BJBJB#######------##-ZBBB##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#----##----#BJBJBJBJB#----##----##BBJBJBB##----##-----##BJBJB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("sorc");
				notesl_.push("224000 sorc @ 13 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 224000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">10 sec druids</option>`;
				layoutsl_.push("[ShareString.1.3]:########################-J----X#-------#####JBJB--MS#--------###BJBJB---H#---------##BJBJB----#---------##BJBJB-#######------##BJBJB##BJBJB##-----##-JBJ##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#----##----#BJBJBJBJB#----##----##JBJBJBJ##----##-----##BJBJB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("druids");
				notesl_.push("102000 druids @ 12 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 102000, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">scorp/rams</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BBYB--X#-------#####BYBYB---#--------###-BYBYB-MS#---------##-BYBYB--H#---------##-BYBYB#######------##-BYBB##BYBYB##-----##----##YBYBYBY##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#----##----#BYBYBYBYB#----##----##YBYBYBY##----##-----##BYBYB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("scorp/rams");
				notesl_.push("21600 siege engines @ 7.5 days");
				troopcounl_.push([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5500, 16100, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				ll_1++;
				
				selectbuttsl_ = `${selectbuttsl_}<option value="${ll_1}">ballista</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BBBB--X#-------#####BYBYB---#--------###-BYBYB-MS#---------##-BYBYB--H#---------##-BYBYB#######------##-BBBB##BBBBB##-----##----##BBYBYBB##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#----##----#BYBYBYBYB#----##----##BBYBYBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("ballista");
				notesl_.push("25600 siege engines @ 10.5 days");
				troopcounl_.push([0, 25600, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
				resl_.push([0, 0, 0, 0, 1, 150000, 220000, 150000, 350000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 150000, 350000]);
				
				selectbuttsl_ = `${selectbuttsl_}</select>`;
				$("#removeoverlayGo").after(selectbuttsdf_);
				$("#dfunkylayout").after(selectbuttsl_);
				$("#funkylayoutl").after(selectbuttsw_);
				$("#funkylayoutl").change(() => {
					var newlayout_ = currentlayout_;
					
					var j_12 = 1;
					for (; j_12 < layoutsl_.length; j_12++) {
						if ($("#funkylayoutl").val() == j_12) {
							
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
							setnearhub(j_12);
						}
					}
				});
				$("#funkylayoutw").change(() => {
					var newlayout_1 = currentlayout_;
					
					var j_13 = 1;
					for (; j_13 < layoutsw_.length; j_13++) {
						if ($("#funkylayoutw").val() == j_13) {
							
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
							var aa_8 = D6.mo;
							if ($("#addtroops").prop("checked") == true) {
								var k_4;
								for (k_4 in troopcounw_[j_13]) {
									aa_8[9 + AsNumber(k_4)] = troopcounw_[j_13][k_4];
								}
							}
							if ($("#addwalls").prop("checked") == true) {
								
								aa_8[26] = 1;
							}
							if ($("#addtowers").prop("checked") == true) {
								
								aa_8[27] = 1;
							}
							if ($("#addhub").prop("checked") == true) {
								var hubs_3 = {
									cid: [],
									distance: []
								};
								$.each(ppdt.clc, (key_57, value_105) => {
									if (key_57 == $("#selHub").val()) {
										
										hubs_3.cid = value_105;
									}
								});
								for (let i_55 in hubs_3.cid) {
									
									var tempx_12 = AsNumber(hubs_3.cid[i_55] % 65536);
									
									var tempy_12 = AsNumber((hubs_3.cid[i_55] - tempx_12) / 65536);
									hubs_3.distance.push(Math.sqrt((tempx_12 - D6.x) * (tempx_12 - D6.x) + (tempy_12 - D6.y) * (tempy_12 - D6.y)));
								}
								
								var mindist_3 = Math.min(...hubs_3.distance);
								let nearesthub_3 = hubs_3.cid[hubs_3.distance.indexOf(mindist_3)];
								resw_[j_13][14] = nearesthub_3;
								resw_[j_13][15] = nearesthub_3;
							} else {
								
								resw_[j_13][14] = 0;
								
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
								
								aa_8[51] = [1, GetFloatValue($("#cablev"))];
								
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
					
					var j_14 = 1;
					for (; j_14 < layoutdf_.length; j_14++) {
						if ($("#dfunkylayout").val() == j_14) {
							

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
							var aa_9 = D6.mo;
							if ($("#addtroops").prop("checked") == true) {
								var k_5;
								for (k_5 in troopcound_[j_14]) {
									aa_9[9 + AsNumber(k_5)] = troopcound_[j_14][k_5];
								}
							}
							if ($("#addwalls").prop("checked") == true) {
								
								aa_9[26] = 1;
							}
							if ($("#addtowers").prop("checked") == true) {
								
								aa_9[27] = 1;
							}
							if ($("#addhub").prop("checked") == true) {
								var hubs_4 = {
									cid: [],
									distance: []
								};
								$.each(ppdt.clc, (key_58, value_106) => {
									if (key_58 == $("#selHub").val()) {
										
										hubs_4.cid = value_106;
									}
								});
								for (let i_56 in hubs_4.cid) {
									
									var tempx_13 = AsNumber(hubs_4.cid[i_56] % 65536);
									
									var tempy_13 = AsNumber((hubs_4.cid[i_56] - tempx_13) / 65536);
									hubs_4.distance.push(Math.sqrt((tempx_13 - D6.x) * (tempx_13 - D6.x) + (tempy_13 - D6.y) * (tempy_13 - D6.y)));
								}
								
								var mindist_4 = Math.min(...hubs_4.distance);
								let nearesthub_4 = hubs_4.cid[hubs_4.distance.indexOf(mindist_4)];
								resd_[j_14][14] = nearesthub_4;
								resd_[j_14][15] = nearesthub_4;
							} else {
								
								resd_[j_14][14] = 0;
								
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
								
								aa_9[51] = [1, GetFloatValue($("#cablev"))];
								
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

  	function Aimp(str) {
        var Aexp=JSON.parse(str);
        for (let i=1; i<=Aexp.x.length; i++) {
            $("#t"+i+"x").val(Aexp.x[i-1]);
            $("#t"+i+"y").val(Aexp.y[i-1]);
            $("#type"+i).val(Aexp.type[i-1]).change();
		}
        $("#attackHr").val(Aexp.time[0]);
        $("#attackMin").val(Aexp.time[1]);
        $("#attackSec").val(Aexp.time[2]);
        $("#attackDat").val(Aexp.time[3]);

	}
    
	function neardeftable(t) {
        var cx= Number($("#ndefx").val());
        var cy= Number($("#ndefy").val());
        var cont=Number(Math.floor(cx/100)+10*Math.floor(cy/100));
        var cit=[[]];
        for (let i in t) {
            var tid=t[i].id;
            var tempx=Number(tid % 65536);
            var tempy=Number((tid-tempx)/65536);
            var tcont=Number(Math.floor(tempx/100)+10*Math.floor(tempy/100));
            var ttspd=0;
            if (cont==tcont) {
                if (t[i].Ballista_total>0 || t[i].Ranger_total>0 || t[i].Triari_total>0 || t[i].Priestess_total || t[i].Arbalist_total>0 || t[i].Praetor_total>0 ) {
                    var tdist=(Math.sqrt((tempx-cx)*(tempx-cx)+(tempy-cy)*(tempy-cy)));
                    var tempt=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    tempt[1]=t[i].Ballista_total;
                    tempt[2]=t[i].Ranger_total;
                    tempt[3]=t[i].Triari_total;
                    tempt[4]=t[i].Priestess_total;
                    tempt[8]=t[i].Arbalist_total;
                    tempt[9]=t[i].Praetor_total;
                    var temph=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    temph[1]=t[i].Ballista_home;
                    temph[2]=t[i].Ranger_home;
                    temph[3]=t[i].Triari_home;
                    temph[4]=t[i].Priestess_home;
                    temph[8]=t[i].Arbalist_home;
                    temph[9]=t[i].Praetor_home;
                    var tempts=0; //TS total
                    for (var j in tempt) {
                        tempts+=tempt[j]*ttts[j];
					}
                    var tempth=0; //TS Home
                    for (var h in temph) {
                        tempth+=temph[h]*ttts[h];
					}
                    var tspeed=0;
                    for (var j in tempt) {
                        if (tempt[j]>0) {
                            if (Number((ttspeed[j]/ttSpeedBonus[j]).toFixed(2))>tspeed) {
                                tspeed=Number((ttspeed[j]/ttSpeedBonus[j]).toFixed(2));
							}
						}
					}
                    cit.push([tempx,tempy,tdist,t[i].c,tempt,tempts,tempth,tid,tdist*tspeed]);
				}
			}
            if (cont!=tcont || t[i].Galley_total>0 || t[i].Stinger_total>0) {
				if (t[i].Stinger_total > 0 || t[i].Galley_total > 0) {
					var tdist = RoundTo2Digits(Math.sqrt((tempx - cx) * (tempx - cx) + (tempy - cy) * (tempy - cy)));
                    var tempt=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    tempt[1]=t[i].Ballista_total;
                    tempt[2]=t[i].Ranger_total;
                    tempt[3]=t[i].Triari_total;
                    tempt[4]=t[i].Priestess_total;
                    tempt[8]=t[i].Arbalist_total;
                    tempt[9]=t[i].Praetor_total;
                    tempt[14]=t[i].Galley_total;
                    tempt[15]=t[i].Stinger_total;
                    var temph=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    temph[1]=t[i].Ballista_home;
                    temph[2]=t[i].Ranger_home;
                    temph[3]=t[i].Triari_home;
                    temph[4]=t[i].Priestess_home;
                    temph[8]=t[i].Arbalist_home;
                    temph[9]=t[i].Praetor_home;
                    temph[14]=t[i].Galley_home;
                    temph[15]=t[i].Stinger_home;
                    var tempts=0;
                    for (var j in tempt) {
                        tempts+=tempt[j]*ttts[j];
					}
                    var tempth=0; //TS Home
                    for (var h in temph) {
                        tempth+=temph[h]*ttts[h];
					}
                    var tspeed=0;
                    for (var j in tempt) {
                        if (tempt[j]>0) {
                            if (Number((ttspeed[j]/ttSpeedBonus[j]).toFixed(2))>tspeed) {
                                tspeed=Number((ttspeed[j]/ttSpeedBonus[j]).toFixed(2));
							}
						}
					}
                    var timetssp=(tdist*tspeed)+60;
                    cit.push([tempx,tempy,tdist,t[i].c,tempt,tempts,tempth,tid,timetssp]);
				}
			}
		}
        cit.sort(function(a,b) {return a[8]-b[8];});
		
        var neardeftab="<table id='ndeftable'><thead><th></th><th>City</th><th>Coords</th><th>TS Total</th><th>TS Home</th><th id='ndefdist'>Travel Time</th><th>type</th></thead><tbody>";
        for (let i in cit) {
            if(Number(i)>0){
            let h1=Math.floor(cit[i][8]/60);
            let m1=Math.floor(cit[i][8]%60);
            let _h1 = h1 < 10 ? '0' + h1 : h1;
            let _m1 = m1 < 10 ? '0' + m1 : m1; 
				
            neardeftab+="<tr><td><button class='greenb chcity' id='cityGoTowm' a='"+cit[i][7]+"'>Go To</button></td><td>"+cit[i][3]+"</td><td class='coordblink shcitt' data='"+cit[i][7]+"'>"+cit[i][0]+":"+cit[i][1]+"</td>";
            //style='font-size: 9px;border-radius: 10px;width: 85%;height: 22px;padding: 1;white-space: nowrap;'
            neardeftab+="<td>"+cit[i][5]+"</td><td>"+cit[i][6]+"</td><td>"+_h1+":"+_m1+"</td><td><table>";
            for (var j in cit[i][4]) {
                if (cit[i][4][j]>0) {
						
                    neardeftab+="<td><div class='"+tpicdiv20[j]+"'></div></td>";
					}
				}
				
            neardeftab+="</table></td></tr>";
			}
		}
		
        neardeftab+="</tbody></table>";
        $("#Ndefbox").html(neardeftab);
		$("#ndeftable td").css("text-align", "center");
		$("#ndeftable td").css("height", "25px");
        var newTableObject = document.getElementById('ndeftable');
        sorttable.makeSortable(newTableObject);
  //      $("#ndefdist").trigger({type:"click",originalEvent:"1"});
  //      $("#ndefdist").trigger({type:"click",originalEvent:"1"});
	}

    function nearofftable(t) {
        var contoff=Number($("#noffx").val());
        var cit=[[]];
        var troopmail=[[]];
        var counteroff=0;
        for (let i in t) {
            var tid=t[i].id;
            var tempx=Number(tid % 65536);
            var tempy=Number((tid-tempx)/65536);
            var tcont=Number(Math.floor(tempx/100)+10*Math.floor(tempy/100));
            if (contoff==tcont) {
                if (t[i].Druid_total>0 || t[i].Horseman_total>0 || t[i].Sorcerer_total>0 || t[i].Vanquisher_total>0 || t[i].Scorpion_total>0 || t[i].Ram_total>0) {
                    counteroff+=1;
                    var tempt=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    tempt[5]=t[i].Vanquisher_total;
                    tempt[6]=t[i].Sorcerer_total;
                    tempt[10]=t[i].Horseman_total;
                    tempt[11]=t[i].Druid_total;
                    tempt[12]=t[i].Ram_total;
                    tempt[13]=t[i].Scorpion_total;
                    var tempts=0;
                    for (var j in tempt) {
                        tempts+=tempt[j]*ttts[j];
					}
                    troopmail.push([tempt,tempts]);
                    cit.push([tempx,tempy,tempts,tempt,t[i].c,tid]);
				}
			}
            if(contoff==99){
                if (t[i].Warship_total>0  || t[i].Galley_total>0) {
                    counteroff+=1;
                    var tempt=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
                    tempt[5]=t[i].Vanquisher_total;
                    tempt[6]=t[i].Sorcerer_total;
                    tempt[10]=t[i].Horseman_total;
                    tempt[11]=t[i].Druid_total;
                    tempt[12]=t[i].Ram_total;
                    tempt[13]=t[i].Scorpion_total;
                    tempt[14]=t[i].Galley_total;
                    tempt[16]=t[i].Warship_total;
                    var tempts=0;
                    for (var j in tempt) {
                        tempts+=tempt[j]*ttts[j];
					}
                    troopmail.push([tempt,tempts]);
                    cit.push([tempx,tempy,tempts,tempt,t[i].c,tid]);
				}
			}
		}
        cit.sort(function(a,b) {return b[2]-a[2];});
        $("#asdfg").text("Total:"+counteroff);
		
        var nearofftab="<table id='nofftable'><thead><th></th><th>City</th><th>Coords</th><th>TS</th><th>type</th></thead><tbody>";
        for (let i in cit) {
            if( Number(i)>0){
				
                nearofftab+="<tr><td><button class='greenb chcity' id='cityGoTowm' a='"+cit[i][5]+"'>Go To</button></td><td>"+cit[i][4]+"</td><td class='coordblink shcitt' data='"+cit[i][5]+"'>"+cit[i][0]+":"+cit[i][1]+"</td>";
                //style='font-size: 9px;border-radius: 6px;width: 80%;height: 22px;padding: 0;white-space: nowrap;'
                nearofftab+="<td>"+cit[i][2]+"</td><td><table>";
                for (var j in cit[i][3]) {
                    if (cit[i][3][j]>0) {
						
                        nearofftab+="<td><div class='"+tpicdiv20[j]+"'></div></td>";
					}
				}
				
                nearofftab+="</table></td></tr>";
			}
		}
		
        nearofftab+="</tbody></table>";
        $("#Noffbox").html(nearofftab);
		$("#nofftable td").css("text-align", "center");
		$("#nofftable td").css("height", "26px");
        var newTableObject = document.getElementById('nofftable');
        sorttable.makeSortable(newTableObject);
        troopmail.sort(function(a,b) {return b[1]-a[1];});
        $("#mailoff").click(function() {
            //$("#mailComposeBox").show();
            var conttemp=$("#noffx").val();
			
            var dhruv="<p>Number of offensive castles is '"+counteroff+"'</p>";
			
            dhruv+='</p><table class="mce-item-table" style="width: 266.273px; "data-mce-style="width: 266.273px; "border="1" data-mce-selected="1"><thead><th>Number</th><th>Troop</th><th>TS Amount</th></thead><tbody>';
            for (let i in troopmail) {
                if(Number(i)>0){
					
                    dhruv+='<tr><td style="text-align: center;" data-mce-style="text-align: center;">'+i+'</td>';
					
                    dhruv+='<td style="text-align: center;" data-mce-style="text-align: center;"><table>';
                    for (var j in troopmail[i][0]) {
                        if (troopmail[i][0][j]>0) {
							
                            dhruv+='<td>'+ttname[j]+'</td>';
						}
					}
					
                    dhruv+='</table></td>';
					
                    dhruv+='<td style="text-align: center;" data-mce-style="text-align: center;">'+troopmail[i][1]+'</td></tr>';
				}
			}
			
            dhruv+="</tbody></table>";
            if(conttemp==99){conttemp="Navy";}
            jQuery("#mnlsp")[0].click();
            jQuery("#composeButton")[0].click();
            var temppo=$("#mailname").val();
            $("#mailToto").val(temppo);
            $("#mailToSub").val(conttemp+" Offensive TS");
            var $iframe = $('#mailBody_ifr');
            $iframe.ready(function() {
                $iframe.contents().find("body").append(dhruv);
			});
		});
	}

}

var __autodemoon_ = false;
function callDemo() {
	$("#buildingDemolishButton").trigger(
		"click",
		"1"
	);
}

function setAutoDemo(_autodemoon: boolean) {
	if (_autodemoon == __autodemoon_)
		return;
	__autodemoon_ = _autodemoon;
	if (__autodemoon_ === true) {
		$("#fb4").removeClass("greenb");
		$("#fb4").addClass("redb");
		$("#city_map").click(callDemo);
		}	
	 else {
		$("#fb4").removeClass("redb");
	$("#fb4").addClass("greenb");
	$("#city_map").off("click", callDemo);
	}
}

function getFormattedTime(date_2) {
	var year_1 = date_2.getFullYear();
	var month_1 = (1 + date_2.getMonth()).toString();
	month_1 = month_1.length > 1 ? month_1 : `0${month_1}`;
	var day_ = date_2.getDate().toString();
	day_ = day_.length > 1 ? day_ : `0${day_}`;
	
	var hour_ = TwoDigitNum(date_2.getHours());

	
	var min_ = TwoDigitNum(date_2.getMinutes());
	
	var sec_ = TwoDigitNum(date_2.getSeconds());
	return `${month_1}/${day_}/${year_1} ${hour_}:${min_}:${sec_}`;
}

//(function ()
//{
//	setTimeout(avactor, 100);
//})();
