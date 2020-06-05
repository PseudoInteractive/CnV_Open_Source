
let __base64Encode: Function=null;
let __base64Decode: Function=null;
let __a6={
	ccazzx: { encrypt: (a: string,b: string,c: number) => "",decrypt: (a: string,b: string,c: number) => "" } };
function MakeGlobalGetter(a) {
	return `window['get${a}'] = ()=> ${a};`;
}
function MakeGlobalCopy(a) {
	return `window['__${a}'] = ${a};`;
}
function encryptJs(req,k2v) {
	console.log(req);
	console.log(k2v);
	return __a6.ccazzx.encrypt(JSON.stringify(k2v),ekeys[req],256);
};

function betterBase64Decode() {
	try {
		//var me=arguments.callee.caller.caller.prototype;
		//me.eval(MakeGlobalGetter("D6"));
		//me.eval(MakeGlobalCopy("a6"));
		//console.log(window['GetD6']());;

		__a6.ccazzx.decrypt=arguments.callee.caller as (a: string,b: string,c: number) => string;


		//console.log(window['__a6']);
		// all done!
		String.prototype['base64Encode']=__base64Decode;
	}
	catch(e) {
		// not ready yet, try again later
	}
	var rv=__base64Decode.call(this);
	//console.log(rv);
	return rv;
}

function betterBase64Encode()   {
	try {
		//var me=arguments.callee.caller.caller.prototype;
		//me.eval(MakeGlobalGetter("D6"));
		//me.eval(MakeGlobalCopy("a6"));
		//console.log(window['GetD6']());;

		__a6.ccazzx.encrypt = arguments.callee.caller as (a:string,b:string,c:number)=>string;
	
		//console.log(this);
		//console.log(window['__a6']);
		// all done!
		String.prototype['base64Encode']=__base64Encode; 
	}
	catch(e) {
		// not ready yet, try again later
	}
	return __base64Encode.call(this);
}
function GetCity(): jsonT.City {
	//	return window['getD6']();
	return cdata_;//
}
/*function DummyPromise(data:string) {
	return new Promise<Response>();
}
*/
var __fetch=Window.prototype.fetch;
var __debugMe: any;

function sleep(time) {
	return new Promise((resolve) => setTimeout(resolve,time));
}

var defaultHeaders: [string,string][]=null;

class DoneWrapper {
	//public req: Promise<Response>;
	dataResult: Promise<string>;
	dataRequest: Promise<Response>;
	onFail: (a) => void;
	result: string;
	reason: any;
	that: this;

	static setup() {
		if(defaultHeaders!=null)
			return;
		var cookie=(ppdt['opt'][67] as any as String).substring(0,10);
		if(!cookie)
			throw "waiting";


		defaultHeaders=[
			["Content-Encoding",cookie],
			['pp-ss', ppss],
			['Content-Type','application/x-www-form-urlencoded; charset=UTF-8'],
			['X-Requested-With','XMLHttpRequest']];
		return;
	}
	async done(cb: (a:string) =>void) {
		await this.dataRequest;
		let text=await this.dataResult;
		cb(text);
	}

	fail(cb:any): this {
		this.onFail=cb;
		if(this.reason!=null)
			this.onFail(this.reason);
		return this;
	}

constructor(public url: string,public settings: JQueryAjaxSettings) { }
	
async go() {
	
		try {

		let data=this.settings? this.settings.data:null;
		console.log(data);
			this.dataRequest= fetch(this.url,{
			method: 'POST',
			headers: new AvaHeaders(),

			mode: 'cors',
			cache: "no-cache",
			body: data? (typeof data==="object"? $.param(data as object):(data as string)):""
			});
			let a= await this.dataRequest;
			this.dataResult=a.text();
			let dataText=await this.dataResult;
				this.result=dataText;
		//		if(this.onDone)
			//			this.onDone(dataText);
				if(this.settings&&this.settings.success) {
					var suc=this.settings.success as JQuery.Ajax.SuccessCallback<any> ;
					console.log(suc);
					suc(dataText,null,null);
					console.log("hope this works!");
				}
			await sleep(100);
			__avatarAjaxDone(this.url,dataText);
				//return cb(data);
				//		_this.req.
				//then(cb).
				//catch(e => console.log(e));
		
	} catch(reason) {

				this.reason=reason;

				console.log(reason);
			if(this.onFail)
				this.onFail(reason);
			}
			
	
	}

}


class AvaHeaders implements Headers {
	a: Array<[string,string]>;

	append(name: string,value: string): void {
		throw new Error("Method not implemented.");
	}
	delete(name: string): void {
		throw new Error("Method not implemented.");
	}
	get(name: string): string {
		throw new Error("Method not implemented.");
	}
	has(name: string): boolean {
		throw new Error("Method not implemented.");
	}
	set(name: string,value: string): void {
		throw new Error("Method not implemented.");
	}
	forEach(callbackfn: (value: string,key: string,parent: Headers) => void,thisArg?: any): void {
		throw new Error("Method not implemented.");
	}
	entries(): IterableIterator<[string,string]> {
		throw new Error("Method not implemented.");
	}
	keys(): IterableIterator<string> {
		throw new Error("Method not implemented.");
	}
	values(): IterableIterator<string> {
		throw new Error("Method not implemented.");
	}
	[Symbol.iterator](): IterableIterator<[string,string]> {
		return defaultHeaders[Symbol.iterator]();
	}
	return?(value?: any): IteratorResult<[string,string],any> {
		throw new Error("Method not implemented.");
	}
	throw?(e?: any): IteratorResult<[string,string],any> {
		throw new Error("Method not implemented.");
	}
	construtor() { this.a=defaultHeaders; }




}

function avatarPost(_url: string|JQuery.AjaxSettings,settings?: JQuery.AjaxSettings): DoneWrapper {
	let url=_url as string;
	if(typeof settings==='undefined') {

		settings=_url as JQuery.AjaxSettings;
		if(settings)
			url=settings.url;
	}
	else if(!url) { url=settings.url; }

	try {
		let rv=new DoneWrapper(url,settings);
		rv.go();
		return rv;
	} catch(e) {
		console.log(e);

	}

	}

function Contains(a:string,b:string) {
	return a.indexOf(b)!=-1;
}


function __avatarAjaxDone(url: string,
	data: string) {
	//console.log("Change: " + this.readyState + " " + this.responseURL);
	let url_21=url;

	if(Contains(url_21,"gC.php")) {
		setTimeout(function() {
			/** @type {*} */
			cdata_=JSON.parse(data);
			updateattack_();
			updatedef_();
			makebuildcount_();

		},1000);

	}
	else if(Contains(url_21,"gaLoy.php")) {
		UpdateResearchAndFaith();
	}
	else if(Contains(url_21,"nBuu.php")||Contains(url_21,"UBBit.php")) {
		cdata_=JSON.parse(data);
	}

	else if(Contains(url_21,"gWrd.php")) {
		setTimeout(function() {
			/** @type {*} */
			wdata_=JSON.parse(data);
			/** @type {boolean} */
			beentoworld_=true;
			wdata_=DecodeWorldData(wdata_.a);
			UpdateResearchAndFaith();
			getbossinfo_();
		},500);
	} else	if(Contains(url_21,"gPlA.php")) {
		/** @type {*} */
		pldata_=JSON.parse(data);
	}
	// if(url_21.endsWith("pD.php")) {
	// 	pdata=JSON.parse(this.response);
	// }
	else if(Contains(url_21,"poll2.php")) {

		/** @type {*} */
		var poll2_=JSON.parse(data) as jsonT.Poll;
		if('OGA' in poll2_)
			OGA=poll2_.OGA;



		if('city' in poll2_) {
			{
				cdata_={ ...cdata_,...poll2_.city };
				if('bd' in poll2_.city) {
					let now=Date.now();
					if(now>lastUpdate+5000) {
						lastUpdate=now;
						setTimeout(makebuildcount_,400);
					}
				}
			}


		}
	}
	
}


function _pleaseNoMorePrefilters() { }

function OptimizeAjax() {

//	priorPrefilter
	jQuery.ajaxPrefilter("nada", function avatarPrefilter(A7U, n7U, xhr) {
		xhr.setRequestHeader("pp-ss", ppss);
		if (ppdt['opt'][67] !== undefined)
		{
			var cookie = (ppdt['opt'][67] as any as String).substring(0, 10);
			xhr.setRequestHeader("Content-Encoding" , cookie);
		}
	});
	jQuery.ajaxPrefilter = _pleaseNoMorePrefilters;;

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
*/	}
function UpdateResearchAndFaith(): void {
	/**
	 * @param {?} ldata_
	 * @return {void}
	  */
	try {
		// need to wait

		let faith=cotg.alliance.faith();
		let research=cotg.player.research();

		ttres_[0]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[29]])/100;
		ttres_[1]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[42]])/100;
		ttres_[2]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[30]])/100;
		ttres_[3]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[31]])/100;
		ttres_[4]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[32]])/100;
		ttres_[5]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[33]])/100;
		ttres_[6]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[34]])/100;
		ttres_[7]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[46]])/100;
		ttres_[8]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[35]])/100;
		ttres_[9]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[36]])/100;
		ttres_[10]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[37]])/100;
		ttres_[11]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[38]])/100;
		ttres_[12]=1+AsNumber(faith.cyndros)*0.5/100+AsNumber(Res_[research[39]])/100;
		ttres_[13]=1+AsNumber(faith.cyndros)*0.5/100+AsNumber(Res_[research[41]])/100;
		ttres_[14]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[44]])/100;
		ttres_[15]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[43]])/100;
		ttres_[16]=1+AsNumber(faith.cyndros)*0.5/100+AsNumber(Res_[research[45]])/100;
		ttspeedres_[1]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[12]])/100;
		ttspeedres_[2]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttspeedres_[3]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttspeedres_[4]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttspeedres_[5]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttspeedres_[6]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[8]])/100;
		ttspeedres_[7]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[11]])/100;
		ttspeedres_[8]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttspeedres_[9]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttspeedres_[10]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttspeedres_[11]=1+AsNumber(faith.ibria)*0.5/100+AsNumber(Res_[research[9]])/100;
		ttspeedres_[12]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[12]])/100;
		ttspeedres_[13]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[12]])/100;
		ttspeedres_[14]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[13]])/100;
		ttspeedres_[15]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[13]])/100;
		ttspeedres_[16]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[13]])/100;
		ttspeedres_[17]=1+AsNumber(faith.domdis)*0.5/100+AsNumber(Res_[research[14]])/100;


		Total_Combat_Research_[2]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[30]])/100;
		Total_Combat_Research_[3]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[31]])/100;
		Total_Combat_Research_[4]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[32]])/100;
		Total_Combat_Research_[5]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[33]])/100;
		Total_Combat_Research_[6]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[34]])/100;
		Total_Combat_Research_[7]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[46]])/100;
		Total_Combat_Research_[8]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[35]])/100;
		Total_Combat_Research_[9]=1+AsNumber(faith.naera)*0.5/100+AsNumber(Res_[research[36]])/100;
		Total_Combat_Research_[10]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[37]])/100;
		Total_Combat_Research_[11]=1+AsNumber(faith.vexemis)*0.5/100+AsNumber(Res_[research[38]])/100;
		Total_Combat_Research_[14]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[44]])/100;
		Total_Combat_Research_[15]=1+AsNumber(faith.ylanna)*0.5/100+AsNumber(Res_[research[43]])/100;
		Total_Combat_Research_[16]=1+AsNumber(faith.cyndros)*0.5/100+AsNumber(Res_[research[45]])/100;
	}
	catch(e) {
		setTimeout(UpdateResearchAndFaith,1000);
		return;

	}

}


var creds = {
	header: null,
	cookies: {},
}

function avactor() {

	//	var E3y="5894";
	var q7y=15;
	var G5y=128;
	var q3y=16;
	var U7y=256;
	var v1R=192;
	var P2y=1000;
	var l9p=0xffff;
	var k9p=0x100000000;
	console.log("here");
	
	//};

	function GetDate(jq: string) {
		return new Date($(jq).data().toString());
	}

	function sleep(time) {
		return new Promise((resolve) => setTimeout(resolve,time));
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
		var year_1=date_2.getFullYear();
		var month_1=(1+date_2.getMonth()).toString();
		month_1=month_1.length>1? month_1:`0${month_1}`;
		var day_=date_2.getDate().toString();
		day_=day_.length>1? day_:`0${day_}`;
		/** @type {string} */
		var hour_=TwoDigitNum(date_2.getHours());

		/** @type {string} */
		var min_=TwoDigitNum(date_2.getMinutes());
		/** @type {string} */
		var sec_=TwoDigitNum(date_2.getSeconds());
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
		errz_=errz_+1;
		/** @type {string} */
		let b_=`errBR${errz_}`;
		/** @type {string} */
		let c_=`#${b_}`;
		/** @type {string} */
		let d_=`#${b_} div`;
		/** @type {string} */
		let errormsgs_=`<tr ID = "${b_}"><td><div class = "errBR">${j_}<div></td></tr>`;
		$("#errorBRpopup").append(errormsgs_);
		$(c_).show();
		$(d_).animate({
			opacity: 1,
			bottom: "+10px"
		},"slow");
		setTimeout(() => {
			$(d_).animate({
				opacity: 0,
				bottom: "-10px"
			},"slow");
			$(c_).fadeOut("slow");
		},5000);
		setTimeout(() => {
			$(c_).remove();
		},6000);
	}

	setTimeout(() => {
		__base64Encode=String.prototype['base64Encode'];
		String.prototype['base64Encode']=betterBase64Encode;

		__base64Decode=String.prototype['base64Decode'];
		String.prototype['base64Decode']=betterBase64Decode;

		OptimizeAjax();

		$("<style type='text/css'> .ava{ width: auto; line-height:100%; table-layout: auto;text-align: center;padding-top:0px;padding-left:0px;border-width:1px;margin-left:0px } </style>").appendTo("head");
		$("<style type='text/css'> .ava td{ width: auto; line-height:100% table-layout: auto; text-align: center;padding-top:0px;padding-left:0px;border-width:1px;margin-left:0px} </style>").appendTo("head");
		$("<style type='text/css'> .ava table{table-layout: auto; } </style>").appendTo("head");

		//Decode();
		/** @type {string} */
		//  var popwin_ = `<div id='HelloWorld' style='width:400px;height:400px;background-color: #E2CBAC;-moz-border-radius: 10px;-webkit-border-radius: 10px;border-radius: 10px;border: 4px ridge #DAA520;position:absolute;right:40%;top:100px; z-index:1000000;'><div class=\"popUpBar\"> <span class=\"ppspan\">Welcome!</span><button id=\"cfunkyX\" onclick=\"$('#HelloWorld').remove();\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div id='hellobody' class=\"popUpWindow\"><span style='margin-left: 5%;'> <h3 style='text-align:center;'>Welcome to Crown Of The Gods!</h3></span><br><br><span style='margin-left: 5%;'> <h4 style='text-align:center;'> MFunky(Cfunky + Dfunky + Mohnki's Additional Layouts + Avatar's nonsense)</h4></span><br><span style='margin-left: 5%;'> <h4 style='text-align:center;'>Updated Mar 1 2020</h4></span><br><br><span style='margin-left: 5%;'><h4>changes:</h4> <ul style='margin-left: 6%;'><li>Added 4 raiding carry percentages(100..125)</li><li>When you click on one, it will ensure that carry is at least that value and it will set it as the initial value for the next city that you go to</li></ul></span></div></div>`;

		//$("body").append(popwin_);

		window['alliancelink']=gspotfunct.alliancelink;
		window['infoPlay']=gspotfunct.infoPlay;
		window['shCit']=gspotfunct.shCit;
		window['chcity']=gspotfunct.chcity;



		// if (typeof String.prototype.utf8Encode == _s(h2R << 1513184672)) String.prototype.utf8Encode = function () {
		//   i011.R6();
		//   return unescape(encodeURIComponent(this));
		// };
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

		var a_5=$("#organiser > option");
		var l_3=a_5.length;
		/** @type {number} */
		var i_32=0;
		for(;i_32<l_3;i_32++) {
			/** @type {string} */
			var temp_3=String($(a_5[i_32]).attr("value"));
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
	},8000);

	/** @type {number} */
	var errz_: number=0;
	/**
	 * @param {?} index_54
	 * @param {string} char_
	 * @return {?}
	 * @this {!String}
	 */
	function ReplaceAt(me: string,index_54: number,char_: string) {
		/** @type {!Array<string>} */
		var a_6=me.split("");
		/** @type {string} */
		a_6[index_54]=char_;
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
	var shrinec_=[[]];


	var _mru=localStorage.getItem("mru");
	if(_mru!=null)
		mru=JSON.parse(_mru);


	setTimeout(() => {

		UpdateResearchAndFaith();
		/** @type {string} */
		var returnAllbut_="<button id='returnAllb' style='right: 35.6%; margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Return All</button>";
		/** @type {string} */
		var raidbossbut_="<button id='raidbossGo' style='left: 65%;margin-left: 10px;margin-top: 15px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Locate Bosses</button>";
		/** @type {string} */
		var attackbut_="<button id='attackGo' style='margin-left: 25px;margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Attack Sender</button>";
		/** @type {string} */
		var defbut_="<button id='defGo' style='left: 65%;margin-left: 10px;margin-top: 55px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'>Defense Sender</button>";
		/** @type {string} */
		var quickdefbut_="<button id='quickdefCityGo' style='width:96%; margin-top:2%; margin-left:2%;' class='regButton greenbuttonGo greenb'>@ Quick Reinforcements @</button>";
		/** @type {string} */
		var neardefbut_="<button id='ndefGo' style='left: 4%;margin-left: 3px;margin-top: 95px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'> Nearest Defense</button>";
		/** @type {string} */
		var nearoffbut_="<button id='noffGo' style='right: 35.6%; margin-top: 95px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;' class='regButton greenb'> Offensive list</button>";
		/** @type {string} */
		var addtoatt_="<button id='addtoAtt' style='margin-left: 7%;margin-top: -5%;width: 40%;height: 26px !important; font-size: 9px !important;' class='regButton greenb'>Add to Attack Sender</button>";
		/** @type {string} */
		var addtodef_="<button id='addtoDef' style='margin-left: 7%;width: 40%;height: 26px !important; font-size: 9px !important;' class='regButton greenb'>Add to Defense Sender</button>";
		/** @type {string} */
		var bosstab_="<li id='bosshuntab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warBossmanager'";
		/** @type {string} */
		bosstab_=`${bosstab_}aria-labeledby='ui-id-20' aria-selected='false' aria-expanded='false'>`;
		/** @type {string} */
		bosstab_=`${bosstab_}<a href='#warBossmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-20'>Find Bosses</a></li>`;
		/** @type {string} */
		var bosstabbody_="<div id='warBossmanager' aria-labeledby='ui-id-20' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		/** @type {string} */
		bosstabbody_=`${bosstabbody_} role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >CFunky's Boss Raiding tool:</div>`;
		/** @type {string} */
		bosstabbody_=`${bosstabbody_}<div id='bossbox' class='beigemenutable scroll-pane ava' style='width: 96%; height: 85%; margin-left: 2%;'></div>`;
		/** @type {string} */
		bosstabbody_=`${bosstabbody_}<div id='idletroops'></div></div>`;
		/** @type {string} */
		var attacktab_="<li id='attacktab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warAttackmanager'";
		/** @type {string} */
		attacktab_=`${attacktab_}aria-labeledby='ui-id-21' aria-selected='false' aria-expanded='false'>`;
		/** @type {string} */
		attacktab_=`${attacktab_}<a href='#warAttackmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-21'>Attack</a></li>`;
		/** @type {string} */
		var attacktabbody_="<div id='warAttackmanager' aria-labeledby='ui-id-21' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		/** @type {string} */
		attacktabbody_=`${attacktabbody_} role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Attack Sender:</div>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<div id='attackbox' class='beigemenutable scroll-pane ava' style='width: 53%; height: 50%; float:left; margin-left: 1%; margin-right: 1%;'>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<table><thead><th></th><th>X</th><th>Y</th><th>Type</th></thead><tbody>`;
		/** @type {number} */
		var i_35=1;
		for(;i_35<16;i_35++) {
			/** @type {string} */
			attacktabbody_=`${attacktabbody_}<tr><td>Target ${i_35} </td><td><input id='t${i_35}x' type='number' style='width: 85%'></td><td><input id='t${i_35}y' type='number' style='width: 85%'></td>`;
			/** @type {string} */
			attacktabbody_=`${attacktabbody_}<td><select id='type${i_35}' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'><option value='0'>Fake</option><option value='1'>Real</option></select></td></tr>`;
		}
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}</tbody></table></div>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<div id='picktype' class='beigemenutable scroll-pane ava' style='width: 43%; height: 50%;'></div>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<table><tr><td><span>Use percentage of troops:</span></td><td><input id='perc' type='number' style='width: 30px'>%</td><td></td></tr>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<tr><td><span>Send real as:</span></td><td><select id='realtype' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<option value='0'>Assault</option><option value='1'>Siege</option><option value='2'>Plunder</option><option value='3'>Scout</option></select></td><td></td></tr>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<tr><td><span>Send fake as:</span></td><td><select id='faketype' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<option value='0'>Assault</option><option value='1'>Siege</option><option value='2'>Plunder</option><option value='3'>Scout</option></select></td><td></td></tr>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<tr><td><input id='retcheck' class='clsubopti' type='checkbox' checked> Return all Troops</td><td colspan=2><input id='retHr' type='number' style='width: 20px' value='2'> Hours before attack</td></tr>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<tr><td><input id='scoutick' class='clsubopti' type='checkbox' checked>30galleys/1scout fake</td></tr></table>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<table style='width:96%;margin-left:2%'><thead><tr style='text-align:center;'><th colspan='5'>Date</th></tr>`;
		/** @type {string} */
		//    attacktabbody_ = attacktabbody_ + "<tr><td>Set Time: </td><td><input id='attackHr' type='number style='width: 35px;height: 22px;font-size: 10px;' value='10'></td><td><input id='attackMin' style='width: 35px;height: 22px;font-size: 10px;' type='number' value='00'></td>";
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<tr><td colspan='5'><input style='width:96%;' id='attackDat' type='datetime-local' step='1'></td></tr></tbody></table>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<table style='margin-left: 10%; margin-top:20px;'><tbody><tr><td style='width: 20%'><button id='Attack' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Attack!</button></td>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<td style='width: 20%'><button id='Aexport' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Export Order</button></td>`;
		/** @type {string} */
		attacktabbody_=`${attacktabbody_}<td style='width: 20%'><button id='Aimport' style='width: 95%;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Import Order</button></td></tr></tbody></table>`;
		/** @type {string} */
		var deftab_="<li id='deftab' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='warDefmanager'";
		/** @type {string} */
		deftab_=`${deftab_}aria-labeledby='ui-id-22' aria-selected='false' aria-expanded='false'>`;
		/** @type {string} */
		deftab_=`${deftab_}<a href='#warDefmanager' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-22'>Defend</a></li>`;
		/** @type {string} */
		var deftabbbody_="<div id='warDefmanager' aria-labeledby='ui-id-21' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		/** @type {string} */
		deftabbbody_=`${deftabbbody_} role='tabpanel' aria-hidden='true' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Defense Sender:</div>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<div><p style='font-size: 10px;'>Defense sender will split all the troops you choose to send according to the number of targets you input.</p></div>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<div id='defbox' class='beigemenutable scroll-pane ava' style='width: 53%; height: 50%; float:left; margin-left: 1%; margin-right: 1%;'>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<table><thead><th></th><th>X</th><th>Y</th></thead><tbody>`;
		/** @type {number} */
		i_35=1;
		for(;i_35<=15;i_35++) {
			/** @type {string} */
			deftabbbody_=`${deftabbbody_}<tr><td>Target ${i_35} </td><td><input id='d${i_35}x' type='number' style='width: 85%'></td><td><input id='d${i_35}y' type='number' style='width: 85%'></td></tr>`;
		}
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}</tbody></table></div>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<div id='dpicktype' class='beigemenutable scroll-pane ava' style='width: 43%; height: 50%;'></div>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<table><tr><td><span>Use percentage of troops:</span></td><td><input id='defperc' type='number' style='width: 30px'>%</td><td></td></tr>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<tr><td><span>Select Departure:</span></td><td><select id='defdeparture' class='greensel' style='font-size: 15px !important;width:95%;height:30px;'>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<option value='0'>Now</option><option value='1'>Departure time</option><option value='2'>Arrival time</option></select></td><td></td></tr>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<tr id='dret'><td><input id='dretcheck' class='clsubopti' type='checkbox' checked> Return all Troops</td><td colspan=2><input id='dretHr' type='number' style='width: 20px' value='2'> Hours before departure</td></tr></table>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<table id='deftime' style='width:96%;margin-left:2%'><thead><tr style='text-align:center;'><th></th><th>Hr</th><th>Min</th><th>Sec</th><th colspan='2'>Date</th></tr>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<tr><td>Set Time: </td><td><input id='defHr' type='number' style='width: 35px;height: 22px;font-size: 10px;' value='10'></td><td><input id='defMin' style='width: 35px;height: 22px;font-size: 10px;' type='number' value='00'></td>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<td><input style='width: 35px;height: 22px;font-size: 10px;' id='defSec' type='number' value='00'></td><td colspan='2'><input style='width:90px;' id='date' type='text' value='00/00/0000'></td></tr></tbody></table>`;
		/** @type {string} */
		deftabbbody_=`${deftabbbody_}<button id='Defend' style='width: 35%;height: 30px; font-size: 12px; margin:10px;' class='regButton greenb'>Send Defense</button>`;
		/** @type {string} */
		var ndeftab_="<li id='neardeftab' class='ui-state-default ui-corner-top' role='tab'>";
		/** @type {string} */
		ndeftab_=`${ndeftab_}<a href='#warNdefmanager' class='ui-tabs-anchor' role='presentation'>Near Def</a></li>`;
		/** @type {string} */
		var ndeftabbody_="<div id='warNdefmanager' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		/** @type {string} */
		ndeftabbody_=`${ndeftabbody_} role='tabpanel' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >Nearest defense:</div>`;
		/** @type {string} */
		ndeftabbody_=ndeftabbody_+'<table><td>Choose city:</td><td><input style=\'width: 30px;height: 22px;font-size: 10px;\' id=\'ndefx\' type=\'number\'> : <input style=\'width: 30px;height: 22px;font-size: 10px;\' id=\'ndefy\' type=\'number\'></td>';
		/** @type {string} */
		ndeftabbody_=`${ndeftabbody_}<td>Showing For:</td><td id='asdfgh' class='coordblink shcitt'></td>`;
		/** @type {string} */
		ndeftabbody_=`${ndeftabbody_}<td><button class='regButton greenb' id='ndefup' style='height:30px; width:70px;'>Update</button></td></table>`;
		/** @type {string} */
		ndeftabbody_=`${ndeftabbody_}<div id='Ndefbox' class='beigemenutable scroll-pane ava' style='width: 96%; height: 85%; margin-left: 2%;'></div>`;
		/** @type {string} */
		var nofftab_="<li id='nearofftab' class='ui-state-default ui-corner-top' role='tab'>";
		/** @type {string} */
		nofftab_=`${nofftab_}<a href='#warNoffmanager' class='ui-tabs-anchor' role='presentation'>Offensive TS</a></li>`;
		/** @type {string} */
		var nofftabbody_="<div id='warNoffmanager' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		/** @type {string} */
		nofftabbody_=`${nofftabbody_} role='tabpanel' style='display: none;'><div id='fpdcdiv3' class='redheading' style='margin-left: 2%;' >ALL Offensive TS:</div>`;
		/** @type {string} */
		nofftabbody_=`${nofftabbody_}<table><td colspan='2'> Continent(99 for navy):</td><td><input style='width: 30px;height: 22px;font-size: 10px;' id='noffx' type='number' value='0'>`;
		/** @type {string} */
		nofftabbody_=`${nofftabbody_}<td><button class='regButton greenb' id='noffup' style='height:30px; width:70px;'>Update</button></td>`;
		/** @type {string} */
		nofftabbody_=`${nofftabbody_}<td id='asdfg' style='width:10% !important;'></td><td><button class='regButton greenb' id='mailoff' style='height:30px; width:50px;'>Mail</button></td><td><input style='width: 100px;height: 22px;font-size: 10px;' id='mailname' type='text' value='Name_here;'></table>`;
		/** @type {string} */
		nofftabbody_=`${nofftabbody_}<div id='Noffbox' class='beigemenutable scroll-pane ava' style='width: 96%; height: 85%; margin-left: 2%;'></div>`;
		/** @type {string} */
		var expwin_="<div id='ExpImp' style='width:250px;height:200px;' class='popUpBox ui-draggable'><div class=\"popUpBar\"><span class=\"ppspan\">Import/Export attack orders</span>";
		/** @type {string} */
		expwin_=`${expwin_}<button id="cfunkyX" onclick="$('#ExpImp').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div id='expbody' class="popUpWindow">`;
		/** @type {string} */
		expwin_=`${expwin_}<textarea style='font-size:11px;width:300px;height:200px;' id='expstring'></textarea><button id='applyExp' style='margin-left: 15px; width: 100px;height: 30px !important; font-size: 12px !important;' class='regButton greenb'>Apply</button></div></div>`;
		var tabs_=$("#warcouncTabs").tabs();
		var ul_=tabs_.find("ul");
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
		$("#loccavwarconGo").css("right","65%");
		$("#idluniwarconGo").css("left","34%");
		$("#idluniwarconGo").after(raidbossbut_);
		$("#defdeparture").change(() => {
			if($("#defdeparture").val()==0) {
				$("#deftime").hide();
				$("#dret").hide();
			} else {
				$("#deftime").show();
				$("#dret").show();
			}
		});
		$("#troopperc").val(localStorage.getItem("troopperc")||100);
		$("#retcheck").prop("checked",(LocalStoreAsInt("retcheck")==1));

		$("#retHr").val(localStorage.getItem("retHr")||0);
		// $("#attackDat").datepicker();
		$("#defDat").datepicker();
		$("#bosshuntab").click(() => {
			if(beentoworld_) {
				openbosswin_();
			} else {
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
					var thdata_=JSON.parse(data_49);
					$("#returnAll").remove();
					openreturnwin_(thdata_);
				}
			});
		});
		$("#raidbossGo").click(() => {
			if(beentoworld_) {
				$("#warcouncbox").show();
				tabs_.tabs("option","active",2);
				$("#bosshuntab").click();
			} else {
				alert("Press World Button");
			}
		});
		$("#Attack").click(() => {
			localStorage.setItem("troopperc",$("#troopperc").val().toString());
			localStorage.setItem("retHr",$("#retHr").toString());
			LocalStoreSet("retcheck",$("#retcheck").prop("checked")? 1:0);
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
			tabs_.tabs("option","active",3);
			jQuery("#attacktab")[0].click();
		});
		$("#defGo").click(() => {
			$("#warcouncbox").show();
			tabs_.tabs("option","active",4);
			$("#deftab").click();
		});
		$("#ndefGo").click(() => {
			NearDefSubscribe();
			$("#warcouncbox").show();
			tabs_.tabs("option","active",5);
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
			tabs_.tabs("option","active",6);
			//$("#nearofftab").trigger({
			//	type: "click",
			//	originalEvent: "1"
			//});
		});
		$("#addtoAtt").click(() => {
			/** @type {number} */
			var i_37=1;
			for(;i_37<16;i_37++) {
				if(!$(`#t${i_37}x`).val()) {
					/** @type {number} */
					var tid_4=AsNumber($("#showReportsGo").attr("data"));
					var tempx_7;
					var tempy_7;
					/** @type {number} */
					tempx_7=AsNumber(tid_4%65536);
					/** @type {number} */
					tempy_7=AsNumber((tid_4-tempx_7)/65536);
					$(`#t${i_37}x`).val(tempx_7);
					$(`#t${i_37}y`).val(tempy_7);
					break;
				}
			}
		});
		$("#addtoDef").click(() => {
			/** @type {number} */
			var i_38=1;
			for(;i_38<16;i_38++) {
				if(!$(`#d${i_38}x`).val()) {
					/** @type {number} */
					var tid_5=AsNumber($("#showReportsGo").attr("data"));
					var tempx_8;
					var tempy_8;
					/** @type {number} */
					tempx_8=AsNumber(tid_5%65536);
					/** @type {number} */
					tempy_8=AsNumber((tid_5-tempx_8)/65536);
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
			var tempxs_=AsNumber($("#ndefx").val());
			/** @type {number} */
			var tempys_=AsNumber($("#ndefy").val());
			/** @type {number} */
			var tids_=tempxs_+tempys_*65536;
			$("#asdfgh").data('data',tids_);
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
			var Aexp_1={
				x: [],
				y: [],
				type: [],
				time: []
			};
			/** @type {number} */
			var i_40=1;
			for(;i_40<16;i_40++) {
				if($(`#t${i_40}x`).val()) {
					Aexp_1.x.push($(`#t${i_40}x`).val());
					Aexp_1.y.push($(`#t${i_40}y`).val());
					Aexp_1.type.push($(`#type${i_40}`).val());
				}
			}
			/** @type {Date} */
			var date=GetDate("#attackDat");
			Aexp_1.time[0]=date.getHours();
			Aexp_1.time[1]=date.getMinutes();
			Aexp_1.time[2]=date.getSeconds();
			Aexp_1.time[3]=date.toLocaleDateString();
			prompt("Attack Orders Expot",JSON.stringify(Aexp_1));

		});
		$("#Aimport").click(() => {
			$("body").append(expwin_);
			$("#ExpImp").draggable({
				handle: ".popUpBar",
				containment: "window",
				scroll: false
			});
			document.addEventListener("paste",evt_27 => {
				$("#expstring").val(evt_27.clipboardData.getData("text/plain"));
			});
			$("#applyExp").click(() => {
				Aimp_($("#expstring").val());
				$("#ExpImp").remove();
			});
		});
		/** @type {string} */
		var fourbutton_="<div id='fourbuttons' class='commandinndiv'><div><button id='fb1' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>TBA</button><button id='fb2' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Refine</button><button id='fb3' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Raid</button><button id='fb4' style='height:28px; width:65px; margin-left:7px; margin-bottom:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;' class='regButton greenb'>Demolish</button></div></div>";
		/** @type {string} */
		var bdcountbox_="<div id='currentBd'><div id='bdcountbar' class='queueBar'>";
		/** @type {string} */
		bdcountbox_=`${bdcountbox_}<div id='bdcountbut' class='tradeqarr2'><div></div></div><span class='qbspan'>Current Buildings</span>`;
		/** @type {string} */
		bdcountbox_=`${bdcountbox_}<div id='numbdleft' class='barRightFloat tooltipstered'>0</div>`;
		/** @type {string} */
		bdcountbox_=`${bdcountbox_}</div><div id='bdcountwin' class='queueWindow' style='display: block;'></div></div>`;
		$("#buildQueue").before(fourbutton_);
		/** @type {string} */
		var fillbut_='<button id="fillque" class="greenb tooltipstered" style="height:18px; width:40px; margin-left:7px; margin-top:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;">Fill</button>';
		$("#sortbut").after(fillbut_);
		$("#fillque").click(() => {
			OverviewPost('overview/fillq.php',{ a: cotg.city.id() });
			event.stopPropagation();
		});
		/** @type {string} */
		var convbut_='<button id="convque" class="greenb tooltipstered" style="height:18px; width:60px; margin-left:7px; margin-top:5px ; border-radius:4px ; font-size: 10px !important; padding: 0px;">Convert</button>';
		$("#sortbut").after(convbut_);
		$("#convque").click(() => {
			OverviewPost('overview/mconv.php',{ a: cotg.city.id() });
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
			},100);
		});
		$("#fb3").click(() => {
			$("#warcouncbox").show();
			jQuery("#ui-id-19")[0].click();
		});
		/** @type {number} */
		var autodemoon_=0;
		$("#fb4").click(function() {
			if(autodemoon_==0) {
				/** @type {number} */
				autodemoon_=1;
				$(this).removeClass("greenb");
				$(this).addClass("redb");
			} else {
				/** @type {number} */
				autodemoon_=0;
				$(this).removeClass("redb");
				$(this).addClass("greenb");
			}
		});
		$("#centarrowNextDiv").click(() => {
			/** @type {number} */
			autodemoon_=0;
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		$("#centarrowPrevDiv").click(() => {
			/** @type {number} */
			autodemoon_=0;
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		$("#ddctd").click(() => {
			/** @type {number} */
			autodemoon_=0;
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		$("#qbuildtbButton").click(() => {
			/** @type {number} */
			autodemoon_=0;
			$("#fb4").removeClass("redb").addClass("greenb");
		});
		$("#city_map").click(() => {
			if(autodemoon_==1) {
				$("#buildingDemolishButton").trigger(
					"click",
					"1"
				);
			}
		});
		/** @type {string} */
		var sumbut_="<button class='tabButton' id='Sum'>Summary</button>";
		$("#items").after(sumbut_);
		$("#Sum").click(() => {
			if(sum_) {
				opensumwin_();
			} else {
				$("#sumWin").show();
			}
		});
		$("#sumWin").click(() => {
			console.log("popsum");
		});
		$("#recruitmentQueue").before(bdcountbox_);
		$("#bdcountbut").click(() => {
			if(bdcountshow_) {
				$("#bdcountwin").hide();
				$("#bdcountbut").removeClass("tradeqarr2").addClass("tradeqarr1");
				/** @type {boolean} */
				bdcountshow_=false;
			} else {
				$("#bdcountwin").show();
				$("#bdcountbut").removeClass("tradeqarr1").addClass("tradeqarr2");
				/** @type {boolean} */
				bdcountshow_=true;
			}
		});
		/** @type {string} */
		var wood50_="<td><button class='brownb' id='wood50'>Add 50%</button></td>";
		$("#woodmaxbutton").parent().after(wood50_);
		$("#wood50").click(() => {
			/** @type {number} */
			var res_3=AsNumber($("#maxwoodsend").text().replace(/,/g,""));
			if($("#landseasendres").val()=="1") {
				/** @type {number} */
				var carts_=Math.floor(AsNumber($("#availcartscity").text())/2)*1000;
			} else {
				/** @type {number} */
				carts_=Math.floor(AsNumber($("#availshipscity").text())/2)*10000;
			}
			if(res_3>carts_) {
				/** @type {number} */
				res_3=carts_;
			}
			$("#woodsendamt").val(res_3);
		});
		/** @type {string} */
		var stone50_="<td><button class='brownb' id='stone50'>Add 50%</button></td>";
		$("#stonemaxbutton").parent().after(stone50_);
		$("#stone50").click(() => {
			if($("#landseasendres").val()=="1") {
				/** @type {number} */
				var carts_1=Math.floor(AsNumber($("#availcartscity").text())/2)*1000;
			} else {
				/** @type {number} */
				carts_1=Math.floor(AsNumber($("#availshipscity").text())/2)*10000;
			}
			/** @type {number} */
			var res_4=AsNumber($("#maxstonesend").text().replace(/,/g,""));
			if(res_4>carts_1) {
				/** @type {number} */
				res_4=carts_1;
			}
			$("#stonesendamt").val(res_4);
		});
		/** @type {string} */
		var iron50_="<td><button class='brownb' id='iron50'>Add 50%</button></td>";
		$("#ironmaxbutton").parent().after(iron50_);
		$("#iron50").click(() => {
			/** @type {number} */
			var res_5=AsNumber($("#maxironsend").text().replace(/,/g,""));
			if($("#landseasendres").val()=="1") {
				/** @type {number} */
				var carts_2=Math.floor(AsNumber($("#availcartscity").text())/2)*1000;
			} else {
				/** @type {number} */
				carts_2=Math.floor(AsNumber($("#availshipscity").text())/2)*10000;
			}
			if(res_5>carts_2) {
				/** @type {number} */
				res_5=carts_2;
			}
			$("#ironsendamt").val(res_5);
		});
		/** @type {string} */
		var food50_="<td><button class='brownb' id='food50'>Add 50%</button></td>";
		$("#foodmaxbutton").parent().after(food50_);
		$("#food50").click(() => {
			/** @type {number} */
			var res_6=AsNumber($("#maxfoodsend").text().replace(/,/g,""));
			if($("#landseasendres").val()=="1") {
				/** @type {number} */
				var carts_3=Math.floor(AsNumber($("#availcartscity").text())/2)*1000;
			} else {
				/** @type {number} */
				carts_3=Math.floor(AsNumber($("#availshipscity").text())/2)*10000;
			}
			if(res_6>carts_3) {
				/** @type {number} */
				res_6=carts_3;
			}
			$("#foodsendamt").val(res_6);
		});
		/** @type {string} */
		var shrinebut_="<button class='regButton greenb' id='shrineP' style='width: 98%;margins: 1%;'>Shrine Planner</button>";
		$("#inactiveshrineInfo").before(shrinebut_);
		$("#shrineP").click(() => {
			if(beentoworld_) {
				/** @type {!Array} */
				shrinec_=[[]];
				splayers_={
					name: [],
					ally: [],
					cities: []
				};
				/** @type {!Array} */
				var players_=[];
				var coords_=$("#coordstochatGo3").attr("data");
				/** @type {number} */
				var shrinex_=parseInt(coords_);
				/** @type {number} */
				var shriney_=AsNumber(coords_.match(/\d+$/)[0]);
				/** @type {number} */
				var shrinecont_=AsNumber(Math.floor(shrinex_/100)+10*Math.floor(shriney_/100));
				var i_41;
				for(i_41 in wdata_.cities) {
					/** @type {number} */
					var tempx_10=AsNumber(wdata_.cities[i_41].substr(8,3))-100;
					/** @type {number} */
					var tempy_10=AsNumber(wdata_.cities[i_41].substr(5,3))-100;
					/** @type {number} */
					var cont_2=AsNumber(Math.floor(tempx_10/100)+10*Math.floor(tempy_10/100));
					if(cont_2==shrinecont_) {
						/** @type {number} */
						var dist_1=Math.sqrt((tempx_10-shrinex_)*(tempx_10-shrinex_)+(tempy_10-shriney_)*(tempy_10-shriney_));
						if(dist_1<10) {
							/** @type {number} */
							var l_4=AsNumber(wdata_.cities[i_41].substr(11,1));
							/** @type {number} */
							var pid_=AsNumber(wdata_.cities[i_41].substr(12,l_4));
							var pname_12=pldata_[pid_];
							/** @type {!Array} */
							var csn_=[3,4,7,8];
							if(csn_.indexOf(AsNumber(wdata_.cities[i_41].charAt(4)))>-1) {
								shrinec_.push(["castle",pname_12,0,tempx_10,tempy_10,dist_1,"0",0,0,0]);
							} else {
								shrinec_.push(["city",pname_12,0,tempx_10,tempy_10,dist_1,"0",0,0,0]);
							}
						}
					}
				}
				shrinec_.sort((a,b) => {
					return a[5]-b[5];
				});
				/** @type {string} */
				var planwin_="<div id='shrinePopup' style='width:40%;height:50%;left: 360px; z-index: 3000;' class='popUpBox'><div class='popUpBar'><span class=\"ppspan\">Shrine Planner</span><button id='hidec' class='greenb' style='margin-left:10px;border-radius: 7px;margin-top: 2px;height: 28px;'>Hide Cities</button>";
				/** @type {string} */
				planwin_=`${planwin_}<button id='addcity' class='greenb' style='margin-left:10px;border-radius: 7px;margin-top: 2px;height: 28px;'>Add City</button><button id="sumX" onclick="$('#shrinePopup').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div class="popUpWindow" style='height:100%'>`;
				/** @type {string} */
				planwin_=`${planwin_}<div id='shrinediv' class='beigemenutable scroll-pane ava' style='background:none;border: none;padding: 0px;height:90%;'></div></div>`;
				for(i_41 in shrinec_) {
					if(i_41<101) {
						pname_12=shrinec_[i_41][1];
						if(players_.indexOf(pname_12)==-1) {
							players_.push(pname_12);
							jQuery.ajax(
								{
									url: "includes/gPi.php",type: "POST",
									async: true,
									data: {
										a: pname_12
									},
									success: function success_13(_data) {
										var pinfo_=JSON.parse(_data);
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
						};
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
					if(localStorage.getItem("hidecities")) {
						1==1;
					} else {
						localStorage.setItem("hidecities","0");
					}
					if(localStorage.getItem("hidecities")=="1") {
						$("#hidec").html("Show Cities");
					}
					$("#hidec").click(() => {
						if(localStorage.getItem("hidecities")=="0") {
							hidecities_();
							localStorage.setItem("hidecities","1");
							$("#hidec").html("Show Cities");
						} else {
							if(localStorage.getItem("hidecities")=="1") {
								showcities_();
								localStorage.setItem("hidecities","0");
								$("#hidec").html("Hide Cities");
							}
						}
					});
					updateshrine_();
					/** @type {string} */
					var addcitypop_="<div id='addcityPopup' style='width:500px;height:100px;left: 360px; z-index: 3000;' class='popUpBox'><div class='popUpBar'><span class=\"ppspan\">Add City</span>";
					/** @type {string} */
					addcitypop_=`${addcitypop_}<button id="sumX" onclick="$('#addcityPopup').remove();" class="xbutton greenb"><div id="xbuttondiv"><div><div id="centxbuttondiv"></div></div></div></button></div><div class="popUpWindow" style='height:100%'>`;
					/** @type {string} */
					addcitypop_=`${addcitypop_}<div><table><td>X: <input id='addx' type='number' style='width: 35px;height: 22px;font-size: 10px;'></td><td>y: <input id='addy' type='number' style='width: 35px;height: 22px;font-size: 10px;'></td>`;
					/** @type {string} */
					addcitypop_=`${addcitypop_}<td>score: <input id='addscore' type='number' style='width: 45px;height: 22px;font-size: 10px;'></td><td>Type: <select id='addtype' class='greensel' style='font-size: 15px !important;width:55%;height:30px;'>`;
					/** @type {string} */
					addcitypop_=`${addcitypop_}<option value='city'>City</option><option value='castle'>Castle</option></select></td><td><button id='addadd' class='greenb'>Add</button></td></table></div></div>`;
					$("#addcity").click(() => {
						$("body").append(addcitypop_);
						$("#addcityPopup").draggable({
							handle: ".popUpBar",
							containment: "window",
							scroll: false
						});
						$("#addadd").click(() => {
							let tempx_10=$("#addx").val() as number;
							let tempy_10=$("#addy").val() as number;
							/** @type {number} */
							dist_1=Math.sqrt((tempx_10-shrinex_)*(tempx_10-shrinex_)+(tempy_10-shriney_)*(tempy_10-shriney_));
							/** @type {!Array} */
							var temp_4=[$("#addtype").val(),"Poseidon","Atlantis",tempx_10,tempy_10,dist_1,"1",$("#addscore").val(),"Hellas","1"];
							shrinec_.push(temp_4);
							shrinec_.sort((a_9,b_7) => {
								return a_9[5]-b_7[5];
							});
							updateshrine_();
							$("#addcityPopup").remove();
						});
					});
				},2000);
			} else {
				alert("Press World Button");
			}
		});

		var incomingtabledata_=$("#incomingsAttacksTable").children().children().children();
		$("#incomingsAttacksTable table thead tr th:nth-child(2)").width(140);
		/** @type {string} */
		var Addth_="<th>Lock time</th>";
		incomingtabledata_.append(Addth_);
		/** @type {string} */
		var Addth1_="<th>Travel time</th>";
		incomingtabledata_.append(Addth1_);
		$("#allianceIncomings").parent().click(() => {
			setTimeout(() => {
				incomings_();
			},5000);
		});
		$("#incomingsPic").click(() => {
			setTimeout(() => {
				incomings_();
			},5000);
		});

		cotgsubscribe.subscribe("regional",clickInfo => {
			var dtype_=clickInfo.type;
			var type_113=clickInfo.info.type;
			var lvl_=clickInfo.info.lvl as number;
			var prog_=clickInfo.info.prog;
			var bossname_=clickInfo.info.name;
			console.log(clickInfo);
			UpdateResearchAndFaith();
			let troops=cotg.city.troops();

			if(dtype_==="dungeon") {
				if($("#cityplayerInfo div table tbody tr").length>=11) {
					bossele_();
				}
				/** @type {number} */
				let home_loot_2=0;

				for(let i in troops) {
					let d=troops[i];
					/** @type {number} */
					let home_1=d.home;
					/** @type {number} */
					home_loot_2=home_loot_2+home_1*ttloot_[TroopNameToId(i)];


				}
				if(type_113==="Siren's Cove") {
					/** @type {number} */
					let optimalTS_=Math.ceil(other_loot_[lvl_-1]/10*(1-prog_/100+1)*1.05);

					/** @type {number} */
					var galleyTS_=Math.ceil(optimalTS_/100);
					/** @type {number} */
					var stingerTS_=Math.ceil(optimalTS_/150);
					/** @type {number} */
					var warshipTS_=Math.ceil(optimalTS_/300);
					/**
					 * @return {void}
					 */
					document.getElementById("raidDungGo").onclick=() => {
						setTimeout(() => {
							if(troops.warship.home>warshipTS_) {
								$("#raidIP16").val(warshipTS_);
							} else {
								if(troops.stinger.home>stingerTS_) {
									$("#raidIP15").val(stingerTS_);
								} else {
									if(troops.galley.home>galleyTS_) {
										$("#raidIP14").val(galleyTS_);
									} else {
										errorgo_(message_23);
									}
								}

							}

						},1500);
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
					if(type_113==="Mountain Cavern")
						loot_=mountain_loot_;
					else
						loot_=other_loot_;
					/**
					 * @return {void}
					 */
					const total_lootm_=Math.ceil(loot_[AsNumber(lvl_)-1]*(1-AsNumber(prog_)/100+1)*1.05);
					document.getElementById("raidDungGo").onclick=() => {
						setTimeout(() => {
							/** @type {number} */
							if(home_loot_2>total_lootm_) {
								/** @type {number} */
								const option_numbersm_=Math.floor(home_loot_2/total_lootm_);
								/** @type {number} */
								const templ1m_=home_loot_2/total_lootm_*100/option_numbersm_;
								/** @type {number} */
								const templ2m_=(templ1m_-100)/templ1m_*100;
								/** @type {number} */
								for(let i in troops) {
									const id=TroopNameToId(i);
									const th=troops[i].home;
									/** @type {number} */
									$(`#raidIP${id}`).val(th/option_numbersm_);
								}
							}
						},1500);
					};
					/** @type {number} */
					const optimalTSM_=total_lootm_;
					/** @type {number} */
					var cavoptim_=Math.ceil(optimalTSM_*2/3);
					/** @type {number} */
					var praoptim_=Math.ceil(optimalTSM_/2);
					/** @type {number} */
					var sorcoptim_=Math.ceil(optimalTSM_*2);
					/** @type {number} */
					var RToptim_=Math.ceil(optimalTSM_/3);
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
			if(dtype_==="boss") {
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
			if(dtype_==="city") {
				$("#cityplayerInfo div table tbody tr:gt(6)").remove();
				// var coords = $("#citycoords")[0].innerText.split(":");
				let _cid=AsNumber(clickInfo.x)+65536*AsNumber(clickInfo.y);
				var toAdd={ ...defaultMru }; // clone defaults


				var maxCount=32;

				/** @type {AsNumber} */
				for(var i=0;i<mru.length;++i) {
					var _i=mru[i];
					if(_i.cid==_cid) {
						toAdd=_i;
						toAdd.last=new Date();
						mru.splice(AsNumber(i),1);
						break;
					}
				}
				if(mru.length>maxCount) {
					/** @type {number} */
					for(var i=mru.length;--i>=0;) {
						if(!mru[i].pin) {
							mru.splice(i,1);
							break;
						}
					}
				}
				toAdd.player=clickInfo.info.player;
				toAdd.last=new Date();
				toAdd.alliance=clickInfo.info.alliance;
				toAdd.name=clickInfo.info.name;
				toAdd.notes=clickInfo.info.remarks;
				toAdd.cid=_cid;
				mru.push(toAdd);
				mru.sort((a,b) => { return b.last.valueOf()-a.last.valueOf() });
				console.log(mru);
				localStorage.setItem("mru",JSON.stringify(mru));

			}
		});
		/** @type {string} */
		var newbutz_="<div style='float: left; margin-left: 2%;'><button id='newbuttonu' style='font-size:8px; padding: 4px; border-radius: 8px;' class='greenb shRnTr'>Recall(<90%)</button></div>";
		$("#totalTS").before(newbutz_);
		$("#newbuttonu").click(() => {
			setTimeout(() => {
				recallraidl100_();
			},500);
		});
		$("#totalTS").click(() => {
			setTimeout(() => {
				carrycheck_();
			},500);
		});
		$("#loccavwarconGo").click(() => {
			setTimeout(() => {
				getDugRows_();
			},1000);
		});
		$("#raidmantab").click(() => {
			setTimeout(() => {
				getDugRows_();
			},1000);
		});
		$("#allianceIncomings").parent().click(() => {
			setTimeout(() => {
				incomings_();
			},4000);
		});
		$("#ui-id-37").click(() => {
			setTimeout(() => {
				incomings_();
			},1000);
		});

		if(localStorage.getItem("raidbox")!=null) {
			/** @type {string} */
			var raidboxback_="<button class='regButton greenb' id='raidboxb' style='width:120px; margin-left: 2%;'>Return Raiding Box</button>";
			$("#squaredung td").find(".squarePlayerInfo").before(raidboxback_);
			$("#raidboxb").click(() => {
				localStorage.removeItem("raidbox");
				$("#raidboxb").remove();
			});
		}
		/** @type {string} */
		var cancelallya_="<input id='cancelAllya' type='checkbox' checked='checked'> Cancel attack if same alliance";
		/** @type {string} */
		var cancelallys_="<input id='cancelAllys' type='checkbox' checked='checked'> Cancel attack if same alliance";
		/** @type {string} */
		var cancelallyp_="<input id='cancelAllyp' type='checkbox' checked='checked'> Cancel attack if same alliance";
		/** @type {string} */
		var cancelallyc_="<input id='cancelAllyc' type='checkbox' checked='checked'> Cancel attack if same alliance";
		$("#assaulttraveltime").parent().next().html(cancelallya_);
		$("#siegetraveltime").parent().next().html(cancelallys_);
		$("#plundtraveltime").parent().next().html(cancelallyp_);
		$("#scouttraveltime").parent().next().html(cancelallyc_);
		$("#assaultGo").click(() => {
			if($("#cancelAllya").prop("checked")==false) {
				setTimeout(() => {
					$(".shAinf").each(function() {
						let tid_7=ToInt($(this).parent().next().find(".cityblink").attr("data"));
						/** @type {number} */
						var tx_1=tid_7%65536;
						/** @type {number} */
						var ty_1=(tid_7-tx_1)/65536;
						if(tx_1==$("#assaultxcoord").val()&&ty_1==$("#assaultycoord").val()) {
							var aid_=$(this).attr("data");
							var dat_7={
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
					$(".shPinf").each(function() {
						let a=$(this).parent().next().find(".cityblink");
						let tid_8=GetIntData(a);
						/** @type {number} */
						var tx_2=tid_8%65536;
						/** @type {number} */
						var ty_2=(tid_8-tx_2)/65536;
						if(tx_2==$("#assaultxcoord").val()&&ty_2==$("#assaultycoord").val()) {
							var aid_1=$(this).attr("data");
							var dat_8={
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
				},4000);
			}
		});
		$("#plunderGo").click(() => {
			if($("#cancelAllyp").prop("checked")==false) {
				setTimeout(() => {
					$(".shAinf").each(function() {
						var tid_9=GetIntData($(this).parent().next().find(".cityblink"));
						/** @type {number} */
						var tx_3=tid_9%65536;
						/** @type {number} */
						var ty_3=(tid_9-tx_3)/65536;
						if(tx_3==$("#pluxcoord").val()&&ty_3==$("#pluycoord").val()) {
							var aid_2=$(this).attr("data");
							var dat_9={
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
					$(".shPinf").each(function() {
						var tid_10=GetIntData($(this).parent().next().find(".cityblink"));
						/** @type {number} */
						var tx_4=tid_10%65536;
						/** @type {number} */
						var ty_4=(tid_10-tx_4)/65536;
						if(tx_4==$("#pluxcoord").val()&&ty_4==$("#pluycoord").val()) {
							var aid_3=$(this).attr("data");
							var dat_10={
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
				},4000);
			}
		});
		$("#scoutGo").click(() => {
			if($("#cancelAllyc").prop("checked")==false) {
				setTimeout(() => {
					$(".shAinf").each(function() {
						var tid_11=GetIntData($(this).parent().next().find(".cityblink"));
						/** @type {number} */
						var tx_5=tid_11%65536;
						/** @type {number} */
						var ty_5=(tid_11-tx_5)/65536;
						if(tx_5==$("#scoxcoord").val()&&ty_5==$("#scoycoord").val()) {
							var aid_4=$(this).attr("data");
							var dat_11={
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
					$(".shPinf").each(function() {
						var tid_12=GetIntData($(this).parent().next().find(".cityblink"));
						/** @type {number} */
						var tx_6=tid_12%65536;
						/** @type {number} */
						var ty_6=(tid_12-tx_6)/65536;
						if(tx_6==$("#scoxcoord").val()&&ty_6==$("#scoycoord").val()) {
							var aid_5=$(this).attr("data");
							var dat_12={
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
				},4000);
			}
		});
		$("#siegeGo").click(() => {
			if($("#cancelAllys").prop("checked")==false) {
				setTimeout(() => {
					$(".shAinf").each(function() {
						var tid_13=GetIntData($(this).parent().next().find(".cityblink"));
						/** @type {number} */
						var tx_7=tid_13%65536;
						/** @type {number} */
						var ty_7=(tid_13-tx_7)/65536;
						if(tx_7==$("#siexcoord").val()&&ty_7==$("#sieycoord").val()) {
							var aid_6=$(this).attr("data");
							var dat_13={
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
					$(".shPinf").each(function() {
						let cid=GetCidData($(this).parent().next().find(".cityblink"));
						/** @type {number} */
						let tx_8=cid.x;
						/** @type {number} */
						let ty_8=cid.y;
						if(tx_8==$("#siexcoord").val()&&ty_8==$("#sieycoord").val()) {
							var aid_7=$(this).attr("data");
							var dat_14={
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
				},4000);
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
		var layoutopttab_="<li id='layoutopt' class='ui-state-default ui-corner-top' role='tab' tabindex='-1' aria-controls='layoutoptBody'";
		/** @type {string} */
		layoutopttab_=`${layoutopttab_}aria-labeledby='ui-id-60' aria-selected='false' aria-expanded='false'>`;
		/** @type {string} */
		layoutopttab_=`${layoutopttab_}<a href='#layoutoptBody' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-60'>Layout Options</a></li>`;
		/** @type {string} */
		var layoutoptbody_="<div id='layoutoptBody' aria-labeledby='ui-id-60' class='ui-tabs-panel ui-widget-content ui-corner-bottom' ";
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_} role='tabpanel' aria-hidden='true' style='display: none;'><table><tbody><tr><td><input id='addnotes' class='clsubopti' type='checkbox'> Add Notes</td>`;
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_}<td><input id='addtroops' class='clsubopti' type='checkbox'> Add Troops</td></tr><tr><td><input id='addtowers' class='clsubopti' type='checkbox'> Add Towers</td><td><input id='addbuildings' class='clsubopti' type='checkbox'> Upgrade Cabins</td>`;
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_}<td> Cabin Lvl: <input id='cablev' type='number' style='width:22px;' value='7'></td></tr><tr><td><input id='addwalls' class='clsubopti' type='checkbox'> Add Walls</td>`;
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_}<td><input id='addhub' class='clsubopti' type='checkbox'> Set Nearest Hub With layout</td></tr><tr><td>Select Hubs list: </td><td id='selhublist'></td><td>`;
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_}<button id='nearhubAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Set Nearest Hub</button><button id='infantryAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Infantry setup</button></td></tr></tbody></table>`;
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_}<table><tbody><tr><td colspan='2'><input id='addres' class='clsubopti' type='checkbox'> Add Resources:</td><td id='buttd' colspan='2'></td></tr><tr><td>wood<input id='woodin' type='number' style='width:100px;' value='200000'></td><td>stone<input id='stonein' type='number' style='width:100px;' value='220000'></td>`;
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_}<td>iron<input id='ironin' type='number' style='width:100px;' value='200000'></td><td>food<input id='foodin' type='number' style='width:100px;' value='350000'></td></tr>`;
		/** @type {string} */
		layoutoptbody_=`${layoutoptbody_}</tbody></table></div>`;
		/** @type {string} */
		var layoptbut_="<button id='layoptBut' class='regButton greenb' style='width:150px;'>Save Res Settings</button>";
		var tabs_1=$("#CNtabs").tabs();
		var ul_1=tabs_1.find("ul");
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
			localStorage.setItem("woodin",$('$woodin').val().toString());
			localStorage.setItem("foodin",$("#foodin").val().toString());
			localStorage.setItem("ironin",$("#ironin").val().toString());
			localStorage.setItem("stonein",$("#stonein").val().toString());
			localStorage.setItem("cablev",$("#cablev").val().toString());
		});
		if(localStorage.getItem("cablev")) {
			$("#cablev").val(LocalStoreAsInt("cablev"));
		}
		if(localStorage.getItem("woodin")) {
			$("#woodin").val(localStorage.getItem("woodin"));
		}
		if(localStorage.getItem("stonein")) {
			$("#stonein").val(localStorage.getItem("stonein"));
		}
		if(localStorage.getItem("ironin")) {
			$("#ironin").val(localStorage.getItem("ironin"));
		}
		if(localStorage.getItem("foodin")) {
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
			var selhub_=$("#organiser").clone(false).attr({
				id: "selHub",
				style: "width:100%;height:28px;font-size:11;border-radius:6px;margin:7px"
			});
			$("#selhublist").append(selhub_);
			if(localStorage.getItem("hublist")) {
				$("#selHub").val(localStorage.getItem("hublist")).change();
			}
			$("#selHub").change(() => {
				localStorage.setItem("hublist",$("#selHub").val().toString());
			});
			$("#dfunkylayout").remove();
			$("#funkylayoutl").remove();
			$("#funkylayoutw").remove();
			setTimeout(() => {
				var currentlayout_=$("#currentLOtextarea").text();
				/** @type {number} */
				var i_53=20;
				for(;i_53<currentlayout_.length-20;i_53++) {
					var tmpchar_=currentlayout_.charAt(i_53);
					/** @type {!RegExp} */
					var cmp_=new RegExp(tmpchar_);
					if(!cmp_.test(emptyspots_)) {
						currentlayout_=ReplaceAt(currentlayout_,i_53,"-");
					}
				}
				/** @type {!Array} */
				var prefered_data_=[{
					name: "Guz 7s Prae 122k",
					string: "[ShareString.1.3]:########################-------#-------#####BBBB----#--------###BZZZB----#---------##BBBBB----#---------##BZZZZ-#######------##BBBBB##BBBBB##-----##----##BZZZZZB##----##----#BBBBBBBBB#----##----#BZZZZZZZB#----#######BBBBTBBBB#######P--X#BZZZZZZZB#----##-SSJ#BBBBBBBBB#----##P---##BZZZZZB##----##P----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################",
					remarks: "Landlocked Praetors",
					notes: "122000 Praetors",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 4s Arbs 132k",
					string: "[ShareString.1.3]:########################-------#-------#####BBBB----#--------###BEEEB----#---------##BBBBB----#---------##BEBEB-#######------##BBBBB##BBBBB##-----##----##BEEBEEB##----##----#BBBBBBBBB#----##----#BEEEBEEEB#----#######BBBBTBBBB#######----#EEEEBEEEB#----##----#BBBBBBBBB#----##----##BEEBEEB##----##-----##BBBBB##-----##------#######------##---------#J--------##-----SS--#X--------###----LM--#--------#####--PP---#-------########################",
					remarks: "Arbs",
					notes: "132000 Arbs",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 3s Rng 280k",
					string: "[ShareString.1.3];########################-------#-------#####BBBB----#--------###BGBGB----#---------##BBBBB----#---------##BGBGB-#######------##BBBBB##BBBBB##-----##----##BGGBGGB##----##----#BBBBBBBBB#----##----#BGGBGBGGB#----#######BBBBTBBBB#######----#BGGBGBGGB#----##----#BBBBBBBBB#----##----##BGGBGGB##----##-----##BBBBB##-----##------#######--__--##---------#J---_##_-##-----SS--#X---_###_###----LM--#-----_#######--PP---#------_########################",
					remarks: "Ranger",
					notes: "280000 Ranger",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 3s Rng/Tri 256k",
					string: "[ShareString.1.3]:########################BBBBB--#--,-,--#####-BGBGB-,#------,-###,-BGBBB--#-,-..-,--##--BGBGB-.#,-------.##--BBBB#######:-.---##----:##BBBBB##-.-,-##.-;-##GBGBGBG##----##----#BBBGBGBBB#--:-##...-#BGBGBGBGB#-::-#######BBBGTGBGB#######.SS.#BGBGBGBGB#---:##P--X#BBBGBGBBB#----##:-:J##GBGBGBG##--;-##P:---##BBBBB##,----##:--.--#######---,--##P-.--.-:-#--------,##P----.---#.--:-,-,-###,-,-.---#--------#####,----:-#.--;---########################",
					remarks: "R/T",
					notes: "128K Rng 128K Tri",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 3s R/T Ship 240K",
					string: "[ShareString.1.3];########################-------#---BBBB#####--------#---BGBGB###---------#---BGBGB-##---------#---BBBBB-##------#######BGBGB-##-----##BBBBB##GBGB-##----##BGBGBGB##BB--##----#-BGBGBGB-#----##----#-BGBGBGB-#----#######-BGBTBGB-#######----#-BGBGBGB-#----##----#-BGBGBGB-#----##----##BGBGBGB##----##-----##BBBBB##-----##------#######--RR--##---------#SS--R##R-##---------#J---R###R###--------#X----R#######-------#------R########################",
					remarks: "R/T Ship",
					notes: "120K Rng 120K Tri",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 7s Arb Ship 124K",
					string: "[ShareString.1.3];########################-------#-------#####BBB-----#--------###BEEE-----#---------##BBBBB----#---------##BEEE--#######------##BBBB-##BBBBB##-----##----##BEEBEEB##----##----#BBBBBBBBB#----##----#BEEEBEEEB#----#######BBBBTBBBB#######----#BEEEBEEEB#----##-SSX#BBBBBBBBB#----##---J##BEEBEEB##----##-----##BBBBB##-----##------#######--RR--##---------#----R##R-##---------#----R###R###--------#-----R#######-------#------R########################",
					remarks: "Arb Ship",
					notes: "124K Arb",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 7s Prae Ship 112K",
					string: "[ShareString.1.3];########################-------#-------#####BBB-----#--------###ZZZZ-----#---------##BBBBB----#---------##ZZZZ--#######------##BBBB-##BBBBB##-----##----##BZZZZZB##----##----#BBBBBBBBB#----##----#BZZZZZZZB#----#######BBBBTBBBB#######----#BZZZZZZZB#----##-SSX#BBBBBBBBB#----##---J##BZZZZZB##----##-----##BBBBB##-----##------#######--RR--##---------#----R##R-##---------#----R###R###--------#-----R#######-------#------R########################",
					remarks: "Prae Ship",
					notes: "112K Arb",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 3s Rng Ship 260K",
					string: "[ShareString.1.3];########################-------#-------#####BBB-----#--------###BGBGB----#---------##BBBBB----#---------##BGGG--#######------##-BBB-##BBBBB##-----##----##BGGBGGB##----##----#BBBBBBBBB#----##----#BGGBGBGGB#----#######BBBBTBBBB#######----#BGGBGBGGB#----##----#BBBBBBBBB#----##-SSX##BGGBGGB##----##-----##BBBBB##-----##---J--#######--RR--##---------#----R##R-##---------#----R###R###--------#-----R#######-------#------R########################",
					remarks: "Rng Ship",
					notes: "260K Arb",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 3s Vanq 300K",
					string: "[ShareString.1.3]:########################-------#-------#####--------#BBBBBBB-###---------#BGBGBGB--##---------#BBBBBBB--##------#######-BGBB-##-----##BBBBB##BBB--##----##-BGBGBZ##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BGBBTBBBB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##-BGBGB-##----##-----##BBBBB##-----##------#######------##---------#-X-------##---------#JP-------###--------#SM------#####-------#SM-----########################",
					remarks: "S - Vanq",
					notes: "300K Vanq Senator Capable",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Guz 10s Druid 106K",
					string: "[ShareString.1.3];########################BB-----#-------#####-JJ-----#--------###BBBBB----#---------##JJJJJ----#---------##BBBBBB#######------##JJJJJ##BBBBB##-----##BBBB##JJJJJJJ##----##----#BBBBBBBBB#----##----#JJJJJJJJJ#----#######BBBBTBBBB#######----#JJJJJJJJJ#----##----#BBBBBBBBB#----##----##JJJJJJJ##----##-----##BBBBB##-----##------#######--__--##--------M#X---_##_-##--------S#----_###_###--------#-----_#######-------#------_########################",
					remarks: "Druid",
					notes: "106K Druid",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				},{
					name: "Tas 4sec Priests",
					string: "[ShareString.1.3];########################-------#-----BB#####--------#----BBBB###---------#----BZZZB##---------#----BBBBB##------#######-BZZZB##-----##BZBZB##BBBBB##----##ZBZBZBZ##----##----#BZBZBZBZB#SP--##----#BZBZBZBZB#SP--#######BZBZTZBZB#######----#BZBZBZBZB#JX--##----#BZBZBZBZB#----##----##ZBZBZBZ##----##-----##BZBZB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################",
					remarks: "Priests",
					notes: "224000 Priests",
					troop_count: [0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,575000,575000,575000,575000,0,0,0,0,1,0,0,0,0,0,575000,575000,575000,575000]
				}];
				/** @type {string} */
				var selectbuttsdf_='<select id="dfunkylayout" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:30%;" class="regButton greenb"><option value="0">Prefered build layout</option>';
				/** @type {number} */
				var ww_=1;
				var prefered_;
				for(prefered_ in prefered_data_) {
					console.log(prefered_data_[prefered_]);
					/** @type {string} */
					selectbuttsdf_=`${selectbuttsdf_}<option value="${ww_}">${prefered_data_[prefered_].name}</option>`;
					layoutdf_.push(prefered_data_[prefered_].string);
					remarkdf_.push(prefered_data_[prefered_].remarks);
					notedf_.push(prefered_data_[prefered_].notes);
					troopcound_.push(prefered_data_[prefered_].troop_count);
					resd_.push(prefered_data_[prefered_].res_count);
					ww_++;
				}
				/** @type {string} */
				selectbuttsdf_=`${selectbuttsdf_}</select>`;
				/** @type {string} */
				var selectbuttsw_='<select id="funkylayoutw" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Select water layout</option>';
				/** @type {number} */
				var cww_=1;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">2 sec rang/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##GBGBGBG##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##GBGBGBG##G---##-----##BGGGB##BBBBG##------#######BBVVBB##---------#--GBV##VB##---------#--GBV###V###--------#---BBV#######-------#----BBV########################");
				remarksw_.push("rangers/triari/galley");
				notesw_.push("166600 inf and 334 galley @ 10 days");
				troopcounw_.push([0,0,83300,83300,0,0,0,0,0,0,0,0,0,0,334,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">6 sec arbs/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SPJX##----#BEBEBEBEB#MH--##----##EBEBEBE##----##-----##BEBEB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#--BBBVTT#####-------#--BEBBV########################");
				remarksw_.push("arbs/galley");
				notesw_.push("88300 inf and 354 galley @ 11.5 days");
				troopcounw_.push([0,0,0,0,0,0,0,0,88300,0,0,0,0,0,354,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">3 sec priestess/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#---H#######BZBZTZBZB#######----#BZBZBZBZB#JSPX##----#BZBZBZBZB#----##----##ZBZBZBZ##-Z--##-----##BZZZB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#--ZBV###V###--------#---BBV#######-------#---ZBBV########################");
				remarksw_.push("priestess/galley");
				notesw_.push("166600 inf and 334 galley @ 11 days");
				troopcounw_.push([0,0,0,0,166600,0,0,0,0,0,0,0,0,0,334,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">7 sec praetor/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SPJX##----#BZBZBZBZB#MH--##----##ZBZBZBZ##----##-----##BZBZB##BBBBZ##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#--BZBBV########################");
				remarksw_.push("praetors/galley");
				notesw_.push("86650 praetors and 347 galley @ 12 days");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,86650,0,0,0,0,347,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">2 sec vanq/galley+senator</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##BBGBGBB##---B##-----##BGBGB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#--BBBBV########################");
				remarksw_.push("vanq/galley+senator");
				notesw_.push("193300 inf and 387 galley @ 10 days");
				troopcounw_.push([0,0,0,0,0,193300,0,0,0,0,0,0,0,0,387,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">5 sec horses/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#---H#######BEBETEBEB#######----#BEBEBEBEB#JSPX##----#BEBEBEBEB#-M--##----##EBEBEBB##----##-----##BEBEB##BBBB-##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#--BBBV#######-------#--BEBBV########################");
				remarksw_.push("horses/galley");
				notesw_.push("90000 cav and 360 galley @ 10.5 days");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,0,90000,0,0,0,360,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">5 sec sorc/galley</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##JBJBJ##-----##----##BJBJBJB##----##----#JBJBJBJBJ#----##----#JBJBJBJBJ#---H#######JBJBTBJBJ#######----#JBJBJBJBJ#-S-X##----#JBJBJBJBJ#----##----##BJBJBJB##JJ--##-----##JBJBJ##BBBBJ##------#######BBVVBB##---------#--JBV##VB##---------#--JBV###V###--------#---BBV#######-------#---JBBV########################");
				remarksw_.push("sorc/galley");
				notesw_.push("156600 sorc and 314 galley @ 13.5 days");
				troopcounw_.push([0,0,0,0,0,0,156600,0,0,0,0,0,0,0,314,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">vanqs+ports+senator</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######----#BGBBBBBGB#PPJX##----#BGBGBGBGB#BBBB##----##BBGBGBB##BBBB##-----##BBBBB##BBBBB##------#######-BRRBB##---------#----R##RZ##---------#----R###R###--------#----SR#######-------#----MSR########################");
				remarksw_.push("vanqs+senator+ports");
				notesw_.push("264k infantry @ 10 days");
				troopcounw_.push([0,0,0,100000,0,164000,0,0,0,0,0,0,0,0,0,0,0]);
				resw_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">main hub</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#---PPPPP###---------#---PPPPPP##---------#---PPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##SLSDSAS##PPPP##----#-SDSMSDS-#PPPP##----#-SLSMSAS-#PPPP#######-SDSTSDS-#######----#-SLSMSAS-#----##----#-SDSMSDS-#----##----##SLSDSAS##----##-----##-----##-----##------#######--RR--##---------#ZB--RTTR-##---------#PJ--RTTTR###--------#-----RTT#####-------#------R########################");
				remarksw_.push("main hub");
				notesw_.push("14 mil w/s 23 mil iron 15 mil food 8200 carts 240 boats");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
				resw_.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">palace storage</option>`;
				layoutsw_.push("[ShareString.1.3]:########################-------#-----PP#####--------#-----PPP###---------#-----PPPP##---------#-----PPPP##------#######--PPPP##-----##SASLS##-PPPP##----##ASASLSL##PPPP##----#SASASLSLS#-PPP##----#SASASLSLS#JPPP#######SASA#LSLS#######----#SASASLSLS#----##----#SASASLSLS#----##----##ASASLSL##----##-----##SASLS##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksw_.push("palace storage");
				notesw_.push("40 mil w/s 6200 carts");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
				resw_.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">palace feeder</option>`;
				layoutsw_.push("[ShareString.1.3];########################-PPPPPP#PPPPPPP#####--PPPPPP#PPPPPPPP###---PPPPPP#PPPPPPPPP##---PPPPPP#PPPPPPPPP##----PP#######PPPPPP##-----##----J##PPPPP##----##-A-----##PPPP##----#-SSS-----#PPPP##----#-AAA-----#PPPP#######-SSST----#######----#-LLL-----#----##----#-SSS-----#----##----##-L-----##----##-----##-----##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
				remarksw_.push("palace feeder");
				notesw_.push("8.75 mil w/s 16400 carts");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
				resw_.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">palace Hub mixed</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#PPPPPPP#####--------#PPPPPPPP###---------#PPPPPPPPP##---------#PPPPPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##-------##PPPP##----#SLSASLSAS#PPPP##----#SASLSASLS#JPPP#######SLSATLSAS#######----#SASLSASLS#----##----#SLSASLSAS#----##----##-------##----##-----##-----##-----##------#######--__--##---------#----_TT_-##---------#----_TTT_###--------#-----_TT#####-------#------_########################");
				remarksw_.push("palace Hub mixed");
				notesw_.push("24.57 mil w/s 11000 carts");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
				resw_.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">Stingers</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######----T----#######----#---------#SPHX##----#---------#-M--##----##-------##----##-----##-----##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
				remarksw_.push("stingers");
				notesw_.push("3480 stingers @ 84 days");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3480,0]);
				resw_.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
				cww_++;
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}<option value="${cww_}">Warships</option>`;
				layoutsw_.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######----T----#######----#---------#SPHX##----#---------#-M--##----##-------##----##-----##-----##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
				remarksw_.push("warships");
				notesw_.push("870 warships @ 42 days");
				troopcounw_.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,870]);
				resw_.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
				/** @type {string} */
				selectbuttsw_=`${selectbuttsw_}</select>`;
				/** @type {string} */
				var selectbuttsl_='<select id="funkylayoutl" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Select land layout</option>';
				/** @type {number} */
				var ll_1=1;
				/** @type {!Array} */
				var land_locked_data_=[{
					name: "1 sec vanqs",
					string: "[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##GBGBG##-----##----##BGBGBGB##----##----#GBGBGBGBG#----##----#GBGBGBGBG#----#######GBGBTBGBG#######----#GBGBGBGBG#----##----#GBGBGBGBG#----##----##BGBGBGB##----##GGGGG##GBGBG##-----##BBBBB-#######------##GGGGGG--H#---------##BBBBBB--J#---------###GGGG---X#--------#####BB----S#-------########################",
					remarks: "vanqs",
					notes: "180000 vanqs @ 2 days",
					troop_count: [0,0,0,0,0,180000,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]
				},{
					name: "2 sec vanqs",
					string: "[ShareString.1.3]:########################BBB--JX#-------#####BGBG--PP#--------###-BBBBB-MS#---------##-BGBGB--H#---------##-BGBGB#######------##-ZBB-##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBBBBBGB#----#######BGBGTGBGB#######----#BGBBBBBGB#----##----#BGBGBGBGB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################",
					remarks: "vanqs",
					notes: "264000 vanqs @ 6 days",
					troop_count: [0,0,0,0,0,264000,0,0,0,0,0,0,0,0,0,0,0],
					res_count: [0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]
				}];
				var l_locked_;
				for(l_locked_ in land_locked_data_) {
					/** @type {string} */
					selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">${land_locked_data_[l_locked_].name}</option>`;
					layoutsl_.push(land_locked_data_[l_locked_].string);
					remarksl_.push(land_locked_data_[l_locked_].remarks);
					notesl_.push(land_locked_data_[l_locked_].notes);
					troopcounl_.push(land_locked_data_[l_locked_].troop_count);
					resl_.push(land_locked_data_[l_locked_].res_count);
					ll_1++;
				}
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">3 sec vanqs raiding</option>`;
				layoutsl_.push("[ShareString.1.3];########################----PJX#-------#####BB----PP#--------###BGBGB--SS#---------##BBBBB--MP#---------##BGBGB-#######------##BBBBB##BBBBB##-----##--G-##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBBB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
				remarksl_.push("vanqs");
				notesl_.push("296000 vanqs @ 10 days");
				troopcounl_.push([0,0,0,0,0,296000,0,0,0,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">2 sec rangers</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BGBGB-PP#--------###-BGBGB-MS#---------##-BGBGB--H#---------##-BGBGB#######------##--BBB##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######----#BGBGBGBGB#----##----#BGBGBGBGB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("rangers/triari");
				notesl_.push("236000 inf @ 6.5 days");
				troopcounl_.push([0,0,186000,50000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">3 sec priests</option>`;
				layoutsl_.push("[ShareString.1.3];########################-------#-----BB#####--------#----BBBB###---------#----BZZZB##---------#----BBBBB##------#######-BZZZB##-----##BZBZB##BBBBB##----##ZBZBZBZ##----##----#BZBZBZBZB#SP--##----#BZBZBZBZB#SP--#######BZBZTZBZB#######----#BZBZBZBZB#JX--##----#BZBZBZBZB#----##----##ZBZBZBZ##----##-----##BZBZB##-----##------#######--__--##---------#----_##_-##---------#----_###_###--------#-----_#######-------#------_########################");
				remarksl_.push("priests");
				notesl_.push("224000 inf @ 7.7 days");
				troopcounl_.push([0,0,224000,50000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">6 sec praetors</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BZBZB-PP#--------###-BZBZB-MS#---------##-BZBZB--H#---------##-BZBZB#######------##--BBB##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#----##----#BZBZBZBZB#----##----##BBZBZBB##----##-----##BZBZB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("praetors");
				notesl_.push("110000 praetors @ 7.5 days");
				troopcounl_.push([0,0,0,0,0,0,0,0,0,110000,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">4 sec horses</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--ZBB##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBE##----##-----##BEBEB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("horses");
				notesl_.push("106000 horses @ 5 days");
				troopcounl_.push([0,0,0,0,0,0,0,0,0,0,106000,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">5 sec horses</option>`;
				layoutsl_.push("[ShareString.1.3]:########################-B---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB-PH#---------##-BEBEB#######------##--BBB##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBBTBBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("horses");
				notesl_.push("124000 horses @ 7 days");
				troopcounl_.push([0,0,0,0,0,0,0,0,0,0,124000,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">5 sec arbs</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BEBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--BBB##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BEBEB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("arbs");
				notesl_.push("110000 arbs @ 6.5 days");
				troopcounl_.push([0,0,0,0,0,0,0,0,110000,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">6 sec arbs</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BB---JX#-------#####BEBEB-PP#--------###-BBBEB-MS#---------##-BEBEB--H#---------##-BEBEB#######------##--BBB##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#----##----#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("arbs");
				notesl_.push("124000 arbs @ 8.5 days");
				troopcounl_.push([0,0,0,0,0,0,0,0,124000,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">4 sec sorc</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BJBJ--X#-------#####JBJBJ--S#--------###-JBJBJ--M#---------##-JBJBJ--H#---------##-JBJBJ#######------##-ZBJB##JBJBJ##-----##----##BJBJBJB##----##----#JBJBJBJBJ#----##----#JBJBJBJBJ#----#######JBJBTBJBJ#######----#JBJBJBJBJ#----##----#JBJBJBJBJ#----##----##BJBJBJB##----##-----##JBJBJ##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("sorc");
				notesl_.push("176000 sorc @ 8 days");
				troopcounl_.push([0,0,0,0,0,0,176000,0,0,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">5 sec sorc</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BBB---X#-------#####BJBJB--P#--------###-BJBJB-MS#---------##-BJBJB--H#---------##-BJBJB#######------##-ZBBB##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#----##----#BJBJBJBJB#----##----##BBJBJBB##----##-----##BJBJB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("sorc");
				notesl_.push("224000 sorc @ 13 days");
				troopcounl_.push([0,0,0,0,0,0,224000,0,0,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">10 sec druids</option>`;
				layoutsl_.push("[ShareString.1.3]:########################-J----X#-------#####JBJB--MS#--------###BJBJB---H#---------##BJBJB----#---------##BJBJB-#######------##BJBJB##BJBJB##-----##-JBJ##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#----##----#BJBJBJBJB#----##----##JBJBJBJ##----##-----##BJBJB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("druids");
				notesl_.push("102000 druids @ 12 days");
				troopcounl_.push([0,0,0,0,0,0,0,0,0,0,0,102000,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">scorp/rams</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BBYB--X#-------#####BYBYB---#--------###-BYBYB-MS#---------##-BYBYB--H#---------##-BYBYB#######------##-BYBB##BYBYB##-----##----##YBYBYBY##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#----##----#BYBYBYBYB#----##----##YBYBYBY##----##-----##BYBYB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("scorp/rams");
				notesl_.push("21600 siege engines @ 7.5 days");
				troopcounl_.push([0,0,0,0,0,0,0,0,0,0,0,0,5500,16100,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				ll_1++;
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}<option value="${ll_1}">ballista</option>`;
				layoutsl_.push("[ShareString.1.3]:########################BBBB--X#-------#####BYBYB---#--------###-BYBYB-MS#---------##-BYBYB--H#---------##-BYBYB#######------##-BBBB##BBBBB##-----##----##BBYBYBB##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#----##----#BYBYBYBYB#----##----##BBYBYBB##----##-----##BBBBB##-----##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
				remarksl_.push("ballista");
				notesl_.push("25600 siege engines @ 10.5 days");
				troopcounl_.push([0,25600,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
				resl_.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
				/** @type {string} */
				selectbuttsl_=`${selectbuttsl_}</select>`;
				$("#removeoverlayGo").after(selectbuttsdf_);
				$("#dfunkylayout").after(selectbuttsl_);
				$("#funkylayoutl").after(selectbuttsw_);
				$("#funkylayoutl").change(() => {
					var newlayout_=currentlayout_;
					/** @type {number} */
					var j_12=1;
					for(;j_12<layoutsl_.length;j_12++) {
						if($("#funkylayoutl").val()==j_12) {
							/** @type {number} */
							var i_54=20;
							for(;i_54<currentlayout_.length;i_54++) {
								var tmpchar_1=layoutsl_[j_12].charAt(i_54);
								/** @type {!RegExp} */
								var cmp_1=new RegExp(tmpchar_1);
								if(!cmp_1.test(emptyspots_)) {
									newlayout_=ReplaceAt(newlayout_,i_54,tmpchar_1);
								}
							}
							$("#overlaytextarea").val(newlayout_);
							setTimeout(() => {
								jQuery("#applyoverlayGo")[0].click();
							},300);
							PostMMNIO(j_12);
						}
					}
				});
				$("#funkylayoutw").change(() => {
					var newlayout_1=currentlayout_;
					/** @type {number} */
					var j_13=1;
					for(;j_13<layoutsw_.length;j_13++) {
						if($("#funkylayoutw").val()==j_13) {
							/** @type {number} */
							for(let i_55=20;i_55<currentlayout_.length;i_55++) {
								var tmpchar_2=layoutsw_[j_13].charAt(i_55);
								/** @type {!RegExp} */
								var cmp_2=new RegExp(tmpchar_2);
								if(!cmp_2.test(emptyspots_)) {
									newlayout_1=ReplaceAt(newlayout_1,i_55,tmpchar_2);
								}
							}
							$("#overlaytextarea").val(newlayout_1);
							setTimeout(() => {
								jQuery("#applyoverlayGo")[0].click();
							},300);
							if($("#addnotes").prop("checked")==true) {
								$("#CNremarks").val(remarksw_[j_13]);
								$("#citynotestextarea").val(notesw_[j_13]);
								setTimeout(() => {
									jQuery("#citnotesaveb")[0].click();
								},100);
							}
							var aa_8=cdata_.mo;
							if($("#addtroops").prop("checked")==true) {
								var k_4;
								for(k_4 in troopcounw_[j_13]) {
									aa_8[9+AsNumber(k_4)]=troopcounw_[j_13][k_4];
								}
							}
							if($("#addwalls").prop("checked")==true) {
								/** @type {number} */
								aa_8[26]=1;
							}
							if($("#addtowers").prop("checked")==true) {
								/** @type {number} */
								aa_8[27]=1;
							}
							if($("#addhub").prop("checked")==true) {
								var hubs_3={
									cid: [],
									distance: []
								};
								$.each(ppdt.clc,(key_57,value_105) => {
									if(key_57==$("#selHub").val()) {
										/** @type {number} */
										hubs_3.cid=value_105;
									}
								});
								for(let i_55 in hubs_3.cid) {
									/** @type {number} */
									var tempx_12=AsNumber(hubs_3.cid[i_55]%65536);
									/** @type {number} */
									var tempy_12=AsNumber((hubs_3.cid[i_55]-tempx_12)/65536);
									hubs_3.distance.push(Math.sqrt((tempx_12-cdata_.x)*(tempx_12-cdata_.x)+(tempy_12-cdata_.y)*(tempy_12-cdata_.y)));
								}
								/** @type {number} */
								var mindist_3=Math.min(...hubs_3.distance);
								var nearesthub_3=hubs_3.cid[hubs_3.distance.indexOf(mindist_3)];
								resw_[j_13][14]=nearesthub_3;
								resw_[j_13][15]=nearesthub_3;
							} else {
								/** @type {number} */
								resw_[j_13][14]=0;
								/** @type {number} */
								resw_[j_13][15]=0;
							}
							if($("#addres").prop("checked")==true) {
								resw_[j_13][5]=$("#woodin").val();
								resw_[j_13][6]=$("#stonein").val();
								resw_[j_13][7]=$("#ironin").val();
								resw_[j_13][8]=$("#foodin").val();
								resw_[j_13][19]=$("#woodin").val();
								resw_[j_13][20]=$("#stonein").val();
								resw_[j_13][21]=$("#ironin").val();
								resw_[j_13][22]=$("#foodin").val();
								for(k_4 in resw_[j_13]) {
									aa_8[28+AsNumber(k_4)]=resw_[j_13][k_4];
								}
							}
							if($("#addbuildings").prop("checked")==true) {
								/** @type {!Array} */
								aa_8[51]=[1,GetFloatValue($("#cablev"))];
								/** @type {number} */
								aa_8[1]=1;
							}
							var dat_16={
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
					var newlayout_2=currentlayout_;
					/** @type {number} */
					var j_14=1;
					for(;j_14<layoutdf_.length;j_14++) {
						if($("#dfunkylayout").val()==j_14) {
							/** @type {number} */

							for(let i_56=20;i_56<currentlayout_.length;i_56++) {
								var tmpchar_3=layoutdf_[j_14].charAt(i_56);
								/** @type {!RegExp} */
								var cmp_3=new RegExp(tmpchar_3);
								if(!cmp_3.test(emptyspots_)) {
									newlayout_2=ReplaceAt(newlayout_2,i_56,tmpchar_3);
								}
							}
							$("#overlaytextarea").val(newlayout_2);
							setTimeout(() => {
								jQuery("#applyoverlayGo")[0].click();
							},300);
							if($("#addnotes").prop("checked")==true) {
								$("#CNremarks").val(remarkdf_[j_14]);
								$("#citynotestextarea").val(notedf_[j_14]);
								setTimeout(() => {
									jQuery("#citnotesaveb")[0].click();
								},100);
							}
							var aa_9=cdata_.mo;
							if($("#addtroops").prop("checked")==true) {
								var k_5;
								for(k_5 in troopcound_[j_14]) {
									aa_9[9+AsNumber(k_5)]=troopcound_[j_14][k_5];
								}
							}
							if($("#addwalls").prop("checked")==true) {
								/** @type {number} */
								aa_9[26]=1;
							}
							if($("#addtowers").prop("checked")==true) {
								/** @type {number} */
								aa_9[27]=1;
							}
							if($("#addhub").prop("checked")==true) {
								var hubs_4={
									cid: [],
									distance: []
								};
								$.each(ppdt.clc,(key_58,value_106) => {
									if(key_58==$("#selHub").val()) {
										/** @type {number} */
										hubs_4.cid=value_106;
									}
								});
								for(let i_56 in hubs_4.cid) {
									/** @type {number} */
									var tempx_13=AsNumber(hubs_4.cid[i_56]%65536);
									/** @type {number} */
									var tempy_13=AsNumber((hubs_4.cid[i_56]-tempx_13)/65536);
									hubs_4.distance.push(Math.sqrt((tempx_13-cdata_.x)*(tempx_13-cdata_.x)+(tempy_13-cdata_.y)*(tempy_13-cdata_.y)));
								}
								/** @type {number} */
								var mindist_4=Math.min(...hubs_4.distance);
								var nearesthub_4=hubs_4.cid[hubs_4.distance.indexOf(mindist_4)];
								resd_[j_14][14]=nearesthub_4;
								resd_[j_14][15]=nearesthub_4;
							} else {
								/** @type {number} */
								resd_[j_14][14]=0;
								/** @type {number} */
								resd_[j_14][15]=0;
							}
							if($("#addres").prop("checked")==true) {
								resd_[j_14][5]=$("#woodin").val();
								resd_[j_14][6]=$("#stonein").val();
								resd_[j_14][7]=$("#ironin").val();
								resd_[j_14][8]=$("#foodin").val();
								for(k_5 in resd_[j_14]) {
									aa_9[28+AsNumber(k_5)]=resd_[j_14][k_5];
								}
							}
							if($("#addbuildings").prop("checked")==true) {
								/** @type {!Array} */
								aa_9[51]=[1,GetFloatValue($("#cablev"))];
								/** @type {number} */
								aa_9[1]=1;
							}
							var dat_17={
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
			},500);
		});
		setTimeout(() => {
			//  replaceElem('input','h2','#achatMsg');
			setTimeout(() => {
				//	  tinymce.init(chatHeaderConfig);//	
			},1000);
			//  var options_13 = {};
			//  $("#HelloWorld").hide("drop", options_13, 2000);
		}, 5000);
		{
			

			console.log("Notify here");
			try {
				let pairs = document.cookie.split(";");
				creds.cookies = {};
				for (var i = 0; i < pairs.length; i++) {
					var pair = pairs[i].split("=");
					creds.cookies[(pair[0] + '').trim()] = unescape(pair.slice(1).join('='));
				}
				creds.header = ppdt['opt'][67].substring(0, 10);

				window['external']['notify'](JSON.stringify(creds));

			} catch (e) {
				console.log("Notify failed");
			}

		}
	},5000);


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
	var nearDefSubscribed=undefined;
	function NearDefSubscribe() {
		if(nearDefSubscribed==undefined) {
			nearDefSubscribed=1;
			cotgsubscribe.subscribe("regional",data_50 => {
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
		var Aexp_=JSON.parse(str_6);
		/** @type {number} */
		var i_4=1;
		for(;i_4<=Aexp_.x.length;i_4++) {
			$(`#t${i_4}x`).val(Aexp_.x[i_4-1]);
			$(`#t${i_4}y`).val(Aexp_.y[i_4-1]);
			$(`#type${i_4}`).val(Aexp_.type[i_4-1]).change();
		}
		var date=new Date(`${Aexp_.time[3]} ${Aexp_.time[0]}:${Aexp_.time[1]}:${Aexp_.time[2]}`);

		$("#attackDat").val(date.toISOString().substr(0,19));

	}
	/**
	 * @param {!Object} t_
	 * @return {void}
	 */
	function neardeftable_(t_) {
		var cx_=AsNumber($("#ndefx").val());
		var cy_=AsNumber($("#ndefy").val());
		/** @type {number} */
		var cont_=AsNumber(Math.floor(cx_/100)+10*Math.floor(cy_/100));
		/** @type {!Array} */
		var cit_=[[]];
		/** @type {any} */
		var i_5;
		for(i_5 in t_) {
			var tid_=t_[i_5].id;
			/** @type {number} */
			var tempx_=AsNumber(tid_%65536);
			/** @type {number} */
			var tempy_=AsNumber((tid_-tempx_)/65536);
			/** @type {number} */
			var tcont_=AsNumber(Math.floor(tempx_/100)+10*Math.floor(tempy_/100));
			if(cont_==tcont_) {
				if(t_[i_5].Ballista_total>0||t_[i_5].Ranger_total>0||t_[i_5].Triari_total>0||t_[i_5].Priestess_total||t_[i_5].Arbalist_total>0||t_[i_5].Praetor_total>0) {
					/** @type {number} */
					var tdist_=Math.sqrt((tempx_-cx_)*(tempx_-cx_)+(tempy_-cy_)*(tempy_-cy_));
					/** @type {!Array} */
					var tempt_=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
					tempt_[1]=t_[i_5].Ballista_total;
					tempt_[2]=t_[i_5].Ranger_total;
					tempt_[3]=t_[i_5].Triari_total;
					tempt_[4]=t_[i_5].Priestess_total;
					tempt_[8]=t_[i_5].Arbalist_total;
					tempt_[9]=t_[i_5].Praetor_total;
					/** @type {!Array} */
					var temph_=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
					temph_[1]=t_[i_5].Ballista_home;
					temph_[2]=t_[i_5].Ranger_home;
					temph_[3]=t_[i_5].Triari_home;
					temph_[4]=t_[i_5].Priestess_home;
					temph_[8]=t_[i_5].Arbalist_home;
					temph_[9]=t_[i_5].Praetor_home;
					/** @type {number} */
					var tempts_=0;
					var j_1;
					for(j_1 in tempt_) {
						/** @type {number} */
						tempts_=tempts_+tempt_[j_1]*ttts_[j_1];
					}
					/** @type {number} */
					var tempth_=0;
					var h_6;
					for(h_6 in temph_) {
						/** @type {number} */
						tempth_=tempth_+temph_[h_6]*ttts_[h_6];
					}
					/** @type {number} */
					var tspeed_=0;
					for(j_1 in tempt_) {
						if(tempt_[j_1]>0) {
							if(AsNumber((ttspeed_[j_1]/ttspeedres_[j_1]).toFixed(2))>tspeed_) {
								/** @type {number} */
								tspeed_=AsNumber((ttspeed_[j_1]/ttspeedres_[j_1]).toFixed(2));
							}
						}
					}
					cit_.push([tempx_,tempy_,tdist_,t_[i_5].c,tempt_,tempts_,tempth_,tid_,tdist_*tspeed_]);
				}
			}
			if(cont_!=tcont_||t_[i_5].Galley_total>0||t_[i_5].Stinger_total>0) {
				if(t_[i_5].Stinger_total>0||t_[i_5].Galley_total>0) {
					tdist_=RoundTo2Digits(Math.sqrt((tempx_-cx_)*(tempx_-cx_)+(tempy_-cy_)*(tempy_-cy_)));
					/** @type {!Array} */
					tempt_=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
					tempt_[1]=t_[i_5].Ballista_total;
					tempt_[2]=t_[i_5].Ranger_total;
					tempt_[3]=t_[i_5].Triari_total;
					tempt_[4]=t_[i_5].Priestess_total;
					tempt_[8]=t_[i_5].Arbalist_total;
					tempt_[9]=t_[i_5].Praetor_total;
					tempt_[14]=t_[i_5].Galley_total;
					tempt_[15]=t_[i_5].Stinger_total;
					/** @type {!Array} */
					temph_=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
					temph_[1]=t_[i_5].Ballista_home;
					temph_[2]=t_[i_5].Ranger_home;
					temph_[3]=t_[i_5].Triari_home;
					temph_[4]=t_[i_5].Priestess_home;
					temph_[8]=t_[i_5].Arbalist_home;
					temph_[9]=t_[i_5].Praetor_home;
					temph_[14]=t_[i_5].Galley_home;
					temph_[15]=t_[i_5].Stinger_home;
					/** @type {number} */
					tempts_=0;
					for(j_1 in tempt_) {
						/** @type {number} */
						tempts_=tempts_+tempt_[j_1]*ttts_[j_1];
					}
					/** @type {number} */
					tempth_=0;
					for(h_6 in temph_) {
						/** @type {number} */
						tempth_=tempth_+temph_[h_6]*ttts_[h_6];
					}
					/** @type {number} */
					tspeed_=0;
					for(j_1 in tempt_) {
						if(tempt_[j_1]>0) {
							if(AsNumber((ttspeed_[j_1]/ttspeedres_[j_1]).toFixed(2))>tspeed_) {
								/** @type {number} */
								tspeed_=AsNumber((ttspeed_[j_1]/ttspeedres_[j_1]).toFixed(2));
							}
						}
					}
					/** @type {number} */
					var timetssp_=tdist_*tspeed_+60;
					cit_.push([tempx_,tempy_,tdist_,t_[i_5].c,tempt_,tempts_,tempth_,tid_,timetssp_]);
				}
			}
		}
		cit_.sort((a_,b_1) => {
			return a_[8]-b_1[8];
		});
		/** @type {string} */
		var neardeftab_="<table id='ndeftable'><thead><th></th><th>City</th><th>Coords</th><th>TS Total</th><th>TS Home</th><th id='ndefdist'>Travel Time</th><th>type</th></thead><tbody>";
		for(i_5 in cit_) {
			if(i_5>0) {
				/** @type {number} */
				var h1_=Math.floor(cit_[i_5][8]/60);
				/** @type {number} */
				var m1_=Math.floor(cit_[i_5][8]%60);
				/** @type {(number|string)} */

				/** @type {(number|string)} */
				/** @type {string} */
				neardeftab_=`${neardeftab_}<tr><td><button class='greenb chcity' id='cityGoTowm' a='${cit_[i_5][7]}'>Go To</button></td><td>${cit_[i_5][3]}</td><td class='coordblink shcitt' data='${cit_[i_5][7]}'>${cit_[i_5][0]}:${cit_[i_5][1]}</td>`;
				/** @type {string} */
				neardeftab_=`${neardeftab_}<td>${cit_[i_5][5]}</td><td>${cit_[i_5][6]}</td><td>${TwoDigitNum(h1_)}:${TwoDigitNum(m1_)}</td><td><table>`;
				for(j_1 in cit_[i_5][4]) {
					if(cit_[i_5][4][j_1]>0) {
						/** @type {string} */
						neardeftab_=`${neardeftab_}<td><div class='${tpicdiv20_[j_1]}'></div></td>`;
					}
				}
				/** @type {string} */
				neardeftab_=`${neardeftab_}</table></td></tr>`;
			}
		}
		/** @type {string} */
		neardeftab_=`${neardeftab_}</tbody></table>`;
		$("#Ndefbox").html(neardeftab_);
		$("#ndeftable td").css("text-align","center");
		$("#ndeftable td").css("height","25px");
		/** @type {(Element|null)} */
		var newTableObject_=document.getElementById("ndeftable");
//		sorttable.makeSortable(newTableObject_);
	}
	/**
	 * @param {!Object} t_1
	 * @return {void}
	 */
	function nearofftable_(t_1) {
		/** @type {number} */
		var contoff_=AsNumber($("#noffx").val());
		/** @type {!Array} */
		var cit_1=[[]];
		/** @type {!Array} */
		var troopmail_=[[]];
		/** @type {number} */
		var counteroff_=0;
		var i_6;
		for(i_6 in t_1) {
			var tid_1=t_1[i_6].id;
			/** @type {number} */
			var tempx_1=AsNumber(tid_1%65536);
			/** @type {number} */
			var tempy_1=AsNumber((tid_1-tempx_1)/65536);
			/** @type {number} */
			var tcont_1=AsNumber(Math.floor(tempx_1/100)+10*Math.floor(tempy_1/100));
			if(contoff_==tcont_1) {
				if(t_1[i_6].Druid_total>0||t_1[i_6].Horseman_total>0||t_1[i_6].Sorcerer_total>0||t_1[i_6].Vanquisher_total>0||t_1[i_6].Scorpion_total>0||t_1[i_6].Ram_total>0) {
					/** @type {number} */
					counteroff_=counteroff_+1;
					/** @type {!Array} */
					var tempt_1=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
					tempt_1[5]=t_1[i_6].Vanquisher_total;
					tempt_1[6]=t_1[i_6].Sorcerer_total;
					tempt_1[10]=t_1[i_6].Horseman_total;
					tempt_1[11]=t_1[i_6].Druid_total;
					tempt_1[12]=t_1[i_6].Ram_total;
					tempt_1[13]=t_1[i_6].Scorpion_total;
					/** @type {number} */
					var tempts_1=0;
					var j_2;
					for(j_2 in tempt_1) {
						/** @type {number} */
						tempts_1=tempts_1+tempt_1[j_2]*ttts_[j_2];
					}
					troopmail_.push([tempt_1,tempts_1]);
					cit_1.push([tempx_1,tempy_1,tempts_1,tempt_1,t_1[i_6].c,tid_1]);
				}
			}
			if(contoff_==99) {
				if(t_1[i_6].Warship_total>0||t_1[i_6].Galley_total>0) {
					/** @type {number} */
					counteroff_=counteroff_+1;
					/** @type {!Array} */
					tempt_1=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
					tempt_1[5]=t_1[i_6].Vanquisher_total;
					tempt_1[6]=t_1[i_6].Sorcerer_total;
					tempt_1[10]=t_1[i_6].Horseman_total;
					tempt_1[11]=t_1[i_6].Druid_total;
					tempt_1[12]=t_1[i_6].Ram_total;
					tempt_1[13]=t_1[i_6].Scorpion_total;
					tempt_1[14]=t_1[i_6].Galley_total;
					tempt_1[16]=t_1[i_6].Warship_total;
					/** @type {number} */
					tempts_1=0;
					for(j_2 in tempt_1) {
						/** @type {number} */
						tempts_1=tempts_1+tempt_1[j_2]*ttts_[j_2];
					}
					troopmail_.push([tempt_1,tempts_1]);
					cit_1.push([tempx_1,tempy_1,tempts_1,tempt_1,t_1[i_6].c,tid_1]);
				}
			}
		}
		cit_1.sort((a_1,b_2) => {
			return b_2[2]-a_1[2];
		});
		$("#asdfg").text(`Total:${counteroff_}`);
		/** @type {string} */
		var nearofftab_="<table id='nofftable'><thead><th></th><th>City</th><th>Coords</th><th>TS</th><th>type</th></thead><tbody>";
		for(i_6 in cit_1) {
			if(i_6>0) {
				/** @type {string} */
				nearofftab_=`${nearofftab_}<tr><td><button class='greenb chcity' id='cityGoTowm' a='${cit_1[i_6][5]}'>Go To</button></td><td>${cit_1[i_6][4]}</td><td class='coordblink shcitt' data='${cit_1[i_6][5]}'>${cit_1[i_6][0]}:${cit_1[i_6][1]}</td>`;
				/** @type {string} */
				nearofftab_=`${nearofftab_}<td>${cit_1[i_6][2]}</td><td><table>`;
				for(j_2 in cit_1[i_6][3]) {
					if(cit_1[i_6][3][j_2]>0) {
						/** @type {string} */
						nearofftab_=`${nearofftab_}<td><div class='${tpicdiv20_[j_2]}'></div></td>`;
					}
				}
				/** @type {string} */
				nearofftab_=`${nearofftab_}</table></td></tr>`;
			}
		}
		/** @type {string} */
		nearofftab_=`${nearofftab_}</tbody></table>`;
		$("#Noffbox").html(nearofftab_);
		$("#nofftable td").css("text-align","center");
		$("#nofftable td").css("height","26px");
		/** @type {(Element|null)} */
		var newTableObject_1=document.getElementById("nofftable");
	//	sorttable.makeSortable(newTableObject_1);
		troopmail_.sort((a_2,b_3) => {
			return b_3[1]-a_2[1];
		});
		$("#mailoff").click(() => {
			var conttemp_=$("#noffx").val();
			/** @type {string} */
			var dhruv_=`<p>AsNumber of offensive castles is '${counteroff_}'</p>`;
			/** @type {string} */
			dhruv_=`${dhruv_}</p><table class="mce-item-table" style="width: 266.273px; "data-mce-style="width: 266.273px; "border="1" data-mce-selected="1"><thead><th>AsNumber</th><th>Troop</th><th>TS Amount</th></thead><tbody>`;
			var i_7;
			for(i_7 in troopmail_) {
				if(i_7>0) {
					/** @type {string} */
					dhruv_=`${dhruv_}<tr><td style="text-align: center;" data-mce-style="text-align: center;">${i_7}</td>`;
					/** @type {string} */
					dhruv_=`${dhruv_}<td style="text-align: center;" data-mce-style="text-align: center;"><table>`;
					var j_3;
					for(j_3 in troopmail_[i_7][0]) {
						if(troopmail_[i_7][0][j_3]>0) {
							/** @type {string} */
							dhruv_=`${dhruv_}<td>${ttname_[j_3]}</td>`;
						}
					}
					/** @type {string} */
					dhruv_=`${dhruv_}</table></td>`;
					/** @type {string} */
					dhruv_=`${dhruv_}<td style="text-align: center;" data-mce-style="text-align: center;">${troopmail_[i_7][1]}</td></tr>`;
				}
			}
			/** @type {string} */
			dhruv_=`${dhruv_}</tbody></table>`;
			if(conttemp_==99) {
				/** @type {string} */
				conttemp_="Navy";
			}
			jQuery("#mnlsp")[0].click();
			jQuery("#composeButton")[0].click();
			var temppo_=$("#mailname").val();
			$("#mailToto").val(temppo_);
			$("#mailToSub").val(`${conttemp_} Offensive TS`);
			var $iframe_=$("#mailBody_ifr");
			$iframe_.ready(() => {
				$iframe_.contents().find("body").append(dhruv_);
			});
		});
	};
}
