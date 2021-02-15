

String.prototype.replaceAt=function(index: number, char: any) : string {
        var a = this;
        return a.substring(0,index) + char +   a.substring(index+1);
    };
var emptyspots=",.;:#-T";
 var layoutsl=[""];
	var layoutlol=[""];
	var layoutldl=[""];
	var layoutwol=[""];
	var layoutwdl=[""];
	var layouttsg=[""];
	var layoutpol=[""];
   	var layoutsw=[""];
    var layoutdf=[""];
	var layouthul=[""];
	var layoutshl=[""];
var remarksl=[""];
    var remarksw=[""];
    var remarkdf=[""];
	var remarklol=[""];
	var remarkldl=[""];
	var remarkwol=[""];
	var remarkwdl=[""];
	var remarkhul=[""];
	var remarkshl=[""];
	var remarkpol=[""];
	var remarktsg=[""];
    var troopcounw=[[]];
    var troopcound=[[]];
    var troopcounl=[[]];
	var troopcounlol=[[]];
	var troopcounldl=[[]];
	var troopcounwol=[[]];
	var troopcounwdl=[[]];
	var troopcounhul=[[]];
	var troopcounshl=[[]];
	var troopcounpol=[[]];
	var troopcountsg=[[]];
    var resw=[[]];
    var resd=[[]];
   	var resl=[[]];
	var reslol=[[]];
	var resldl=[[]];
	var reswol=[[]];
	var reswdl=[[]];
	var reshul=[[]];
	var resshl=[[]];
	var respol=[[]];
	var restsg=[[]];
   	var notesl=[""];
    var notesw=[""];
    var notedf=[""];
	var notelol=[""];
	var noteldl=[""];
	var notewol=[""];
	var notewdl=[""];
	var notehul=[""];
	var noteshl=[""];
	var notepol=[""];
	var notetsg=[""];

function replaceAt(_this:string,index: number, char: any) {
        
        var a = _this;
        return a.substring(0,index) + char +   a.substring(index+1);
    };

// setting layouts
    $(document).ready(function() {
		$('#openLOinplanner').remove();
        $("#citynotes").draggable({ handle: ".popUpBar" , containment: "window", scroll: false});
        $('#citynotes').height('420px');
        $('#citynotes').width('500px');
        var layoutopttab="<li id='layoutopt' class='ui-state-default ' role='tab' tabindex='-1' aria-controls='layoutoptBody'";
        layoutopttab+="aria-labeledby='ui-id-60' aria-selected='false' aria-expanded='false'>";
        layoutopttab+="<a href='#layoutoptBody' class='ui-tabs-anchor' role='presentation' tabindex='-1' id='ui-id-60'>Layout Options</a></li>";
        var layoutoptbody="<div id='layoutoptBody' aria-labeledby='ui-id-60' class='ui-tabs-panel ' ";
        layoutoptbody+=" role='tabpanel' aria-hidden='true' style='display: none;'><table><tbody><tr><td><input id='addnotes' class='clsubopti' type='checkbox' checked> Add Notes</td>";
        layoutoptbody+="<td><input id='addtroops' class='clsubopti' type='checkbox'> Add Troops</td></tr><tr><td><input id='addtowers' class='clsubopti' type='checkbox'> Add Towers</td><td><input id='addbuildings' class='clsubopti' type='checkbox' checked> Upgrade Cabins</td>";
        layoutoptbody+="<td> Cabin Lvl: <input id='cablev' type='number' style='width:22px;' value='8' checked></td></tr><tr><td><input id='addwalls' class='clsubopti' type='checkbox' checked> Add Walls</td>";
        layoutoptbody+="<td><input id='addhub' class='clsubopti' type='checkbox' checked> Set Nearest Hub With layout</td></tr><tr><td>Select Hubs list: </td><td id='selhublist'></td><td>";
        layoutoptbody+="<button id='nearhubAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Set Nearest Hub</button><button id='infantryAp' class='regButton greenb' style='width:130px; margin-left: 10%'>Infantry setup</button></td></tr></tbody></table>";
        layoutoptbody+="<table><tbody><tr><td colspan='2'><input id='addres' class='clsubopti' type='checkbox'> Add Resources:</td><td id='buttd' colspan='2'></td></tr><tr><td>wood<input id='woodin' type='number' style='width:100px;' value='250000'></td><td>stones<input id='stonein' type='number' style='width:100px;' value='250000'></td>";
        layoutoptbody+="<td>iron<input id='ironin' type='number' style='width:100px;' value='200000'></td><td>food<input id='foodin' type='number' style='width:100px;' value='350000'></td></tr>";
        layoutoptbody+="</tbody></table></div>";
        var layoptbut="<button id='layoptBut' class='regButton greenb' style='width:150px;'>Save Res Settings</button>";
        var tabs = $( "#CNtabs" ).tabs();
        var ul = tabs.find( "ul" );
        $( layoutopttab ).appendTo( ul );
        tabs.tabs( "refresh" );
        $("#CNtabs").append(layoutoptbody);
        $("#buttd").append(layoptbut);
        $("#nearhubAp").click(function() {
            setnearhub();
        });
        $("#infantryAp").click(function() {
            setinfantry();
        });
        $("#layoptBut").click(function() {
            localStorage.setItem('woodin',$("#woodin").val());
            localStorage.setItem('foodin',$("#foodin").val());
            localStorage.setItem('ironin',$("#ironin").val());
            localStorage.setItem('stonein',$("#stonein").val());
            localStorage.setItem('cablev',$("#cablev").val());
        });
        if (localStorage.getItem('cablev')) {
            $("#cablev").val(localStorage.getItem('cablev'));
        }
        if (localStorage.getItem('woodin')) {
            $("#woodin").val(localStorage.getItem('woodin'));
        }
        if (localStorage.getItem('stonein')) {
            $("#stonein").val(localStorage.getItem('stonein'));
        }
        if (localStorage.getItem('ironin')) {
            $("#ironin").val(localStorage.getItem('ironin'));
        }
        if (localStorage.getItem('foodin')) {
            $("#foodin").val(localStorage.getItem('foodin'));
        }
        if (localStorage.getItem('atroops')) {
            if (localStorage.getItem('atroops')==1) {
                $("#addtroops").prop( "checked", true );
            }
        }
        if (localStorage.getItem('ares')) {
            if (localStorage.getItem('ares')==1) {
                $("#addres").prop( "checked", true );
            }
        }
        if (localStorage.getItem('abuildings')) {
            if (localStorage.getItem('abuildings')==1) {
                $("#addbuildings").prop( "checked", true );
            }
        }
        if (localStorage.getItem('anotes')) {
            if (localStorage.getItem('anotes')==1) {
                $("#addnotes").prop( "checked", true );
            }
        }
        if (localStorage.getItem('awalls')) {
            if (localStorage.getItem('awalls')==1) {
                $("#addwalls").prop( "checked", true );
            }
        }if (localStorage.getItem('atowers')) {
            if (localStorage.getItem('atowers')==1) {
                $("#addtowers").prop( "checked", true );
            }
        }
        if (localStorage.getItem('ahub')) {
            if (localStorage.getItem('ahub')==1) {
                $("#addhub").prop( "checked", true );
            }
        }
        $("#addnotes").change(function() {
            if ($("#addnotes").prop( "checked")==true) {
                localStorage.setItem('anotes',1);
            } else {localStorage.setItem('anotes',0);}
        });
        $("#addres").change(function() {
            if ($("#addres").prop( "checked")==true) {
                localStorage.setItem('ares',1);
            } else {localStorage.setItem('ares',0);}
        });
        $("#addtroops").change(function() {
            if ($("#addtroops").prop( "checked")==true) {
                localStorage.setItem('atroops',1);
            } else {localStorage.setItem('atroops',0);}
        });
        $("#addbuildings").change(function() {
            if ($("#addbuildings").prop( "checked")==true) {
                localStorage.setItem('abuildings',1);
            } else {localStorage.setItem('abuildings',0);}
        });
        $("#addwalls").change(function() {
            if ($("#addwalls").prop( "checked")==true) {
                localStorage.setItem('awalls',1);
            } else {localStorage.setItem('awalls',0);}
        });
        $("#addtowers").change(function() {
            if ($("#addtowers").prop( "checked")==true) {
                localStorage.setItem('atowers',1);
            } else {localStorage.setItem('atowers',0);}
        });
        $("#addhub").change(function() {
            if ($("#addhub").prop( "checked")==true) {
                localStorage.setItem('ahub',1);
            } else {localStorage.setItem('ahub',0);}
        });

        $("#editspncn").click(function() {
            $("#selHub").remove();
            var selhub=$("#organiser").clone(false).attr({id:"selHub",style:"width:100%;height:28px;font-size:11;border-radius:6px;margin:7px"});
            $("#selhublist").append(selhub);
            if (localStorage.getItem('hublist')) {
                $("#selHub").val(localStorage.getItem('hublist')).change();
            }
            $("#selHub").change(function() {
                localStorage.setItem('hublist',$("#selHub").val());
            });

            $('#landbuildlayouts').remove();
            $('#waterbuildlayouts').remove();
			$('#landoffenselayouts').remove();
			$('#landdefenselayouts').remove();
			$('#wateroffenselayouts').remove();
			$('#waterdefenselayouts').remove();
			$('#shipperlayouts').remove();
			$('#hublayouts').remove();
			$('#portallayouts').remove();
			$('#troopscoutgalleylayouts').remove();

            setTimeout(function(){
                var currentlayout=$('#currentLOtextarea').text();
                for (let i=20; i<currentlayout.length-20;i++) {
                    var tmpchar=currentlayout.charAt(i);
                    var cmp=new RegExp(tmpchar);
                    if (!(cmp.test(emptyspots))) {
                        currentlayout=replaceAt(currentlayout,i,"-");
                    }
                }


				var selectbuttlandoff='<select id="landoffenselayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Land Offense Layouts</option>';
                var lol=1;

               
                selectbuttlandoff+='<option value="'+lol+'">Ava 2s Vanq Starter</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#----J--#####--------#-------.###---------#---------##---------#-.------,##------#######,-.---##-----##BGBGB##-----##----##GBGBGBG##-..-##----#BGBGBGBGB#-.--##:--.#BGBGBGBGB#--S,#######BGBGTGBGB#######---:#BGBGBGBGB#,-:-##-;-:#-GBGBGBG-#----##----##-BGBGB-##,--,##-,---##-----##-----##------#######::-:--##--P------#-,-:--:--##.-----X--#---------###------,,#----:-,,#####.-,--,-#-.-----########################");
                remarklol.push("Vanqs");
				notelol.push("Use this for the intial layout.");
                troopcounlol.push([0,0,0,0,0,264000,0,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0, 0, 0, 0, 1, 130000, 210000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 150000, 220000, 750000, 750000]);
                lol++;


            selectbuttlandoff+='<option value="'+lol+'">Ava 2s Vanq Finisher</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-;-,J--#####BBBBB---#-------.###-BGBGB---#----::---##ZBBBBB-,-#-.------,##-BGBGB#######,-.---##-BBBB##BBBBB##-----##;---##BBGBGBB##-..-##----#BGBGBGBGB#-.SS##:--.#BGBBBBBGB#--S,#######BGBGTGBGB#######---:#BGBGBGBGB#,-:-##-;-:#BGBGBGBGB#----##----##BBGBGBB##,--,##-,-;-##BBBBB##-----##-----:#######::-:--##--PP-----#-,-:--:--##.---.-X--#---------###------,,#----:-,,#####.-,--,-#-.-----##############v##########");
                remarklol.push("Vanqs");
				notelol.push("Use this layout once your main buildings are complete.  At this stage you increase TS, carts and storage");
                troopcounlol.push([0,0,0,0,0,264000,0,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0, 0, 0, 0, 1, 80000, 80000, 575000, 575000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 120000, 120000, 750000, 750000]);
                lol++;



                selectbuttlandoff+='<option value="'+lol+'">1 sec vanqs</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##GBGBG##-----##----##BGBGBGB##----##----#GBGBGBGBG#----##----#GBGBGBGBG#----#######GBGBTBGBG#######S--X#GBGBGBGBG#----##----#GBGBGBGBG#----##----##BGBGBGB##----##GGGGG##GBGBG##-----##BBBBBB#######------##GGGGGGJ--#---------##BBBBBB---#---------###GGGGZ---#--------#####B------#-------########################");
                remarklol.push("Vanqs");
				notelol.push("180000 Vanqs @ 2 days");
                troopcounlol.push([0,0,0,0,0,179999,0,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;


                selectbuttlandoff+='<option value="'+lol+'">2 sec vanqs</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBBTBBGB#######SSPX#BGBGBGBGB#----##MDPJ#BGBGBGBGB#----##S---##BBGBGBB##----##-----##BBBBB##-----##-BBBBB#######------##-ZBGGB---#---------##-BBBBB---#---------###-BGGB---#--------#####BBBB---#-------########################");
                remarklol.push("vanqs");
				notelol.push("256000 vanqs @ 6 days");
                troopcounlol.push([0,0,0,0,0,255999,0,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">3 sec vanqs raiding</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBGBGBBB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######SSPX#BGBBBBBGB#----##MDP-#BBBGBGBBB#----##S---##BBGBGBB##----##-----##BBBBB##-----##------#######------##---BBBBBB#---------##---BGGBGB#---------###--BBBBBB#--------#####-JBZBBB#-------########################");
                remarklol.push("vanqs");
				notelol.push("292000 vanqs @ 10 days");
                troopcounlol.push([0,0,0,0,0,291999,0,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">4 sec vanqs Portal</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBGB#######SMSX#BGBGBGBGB#----##SDPP#BBBBBBBBB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######------##---BBBBBB#---------##---BBBBBB#---------###--BBBBBB#--------#####-BBJBZB#-------########################");
                remarklol.push("vanqs");
				notelol.push("308000 vanqs @ 14.5 days");
                troopcounlol.push([0,0,0,0,0,307999,0,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
				selectbuttlandoff+='<option value="'+lol+'">4 sec horses</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##-BEBEB-##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######SM-X#BEBEBEBEB#----##S---#BEBEBEBEB#----##----##-BEBEB-##----##-BBBB##BEBEB##-----##ZEEEE-#######------##BBBBBBBB-#---------##JEEEEEEE-#---------###BBBBBBB-#--------#####-------#-------########################");
                remarklol.push("horses");
				notelol.push("110000 horses @ 5 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,109999,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">5 sec horses</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BEBEBEB##----##----#-BEBEBEB-#----##----#-BEBEBEB-#----#######-BEBTBEB-#######SSPX#-BEBEBEB-#----##MDP-#-BEBEBEB-#----##S---##BEBEBEB##----##-----##BBBBB##-----##--BBBB#######------##--BEEEEEB#---------##-ZBBBBBBB#---------###JBEEEEEB#--------#####BBBBBBB#-------########################");
                remarklol.push("horses");
				notelol.push("120000 horses @ 7 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,119999,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">6 sec horses Portal</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBBBBBEB#----#######BEBETEBEB#######SSPX#BEBBBBBEB#----##MDPJ#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##--BBBZ#######------##--BEBBB--#---------##--BBBEB--#---------###-BEBEB--#--------#####BBBBB--#-------########################");
                remarklol.push("horses");
				notelol.push("134000 horses @ 9.5 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,133999,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">4 sec sorc</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#-JBJBJBJ-#----##----#-JBJBJBJ-#----#######-JBJTJBJ-#######S--X#-JBJBJBJ-#----##M---#-JBJBJBJ-#----##----##JBJBJBJ##----##-JBJB##BJBJB##-----##BJBJBJ#######------##BJBJBJBJ-#---------##BJBJBJBJ-#---------###JBJBJBJ-#--------#####BJBJBZ-#-------########################");
                remarklol.push("sorc");
				notelol.push("180000 sorc @ 8.3 days");
                troopcounlol.push([0,0,0,0,0,0,179999,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">5 sec sorc</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######SMPX#BJBJBJBJB#----##----#BJBJBJBJB#----##----##BBJBJBB##----##-BBBB##BJBJB##-----##-BJBJB#######------##-BJBJB---#---------##ZBJBJB---#---------###BJBJB---#--------#####BBB----#-------########################");
                remarklol.push("sorc");
				notelol.push("224000 sorc @ 12.5 days");
                troopcounlol.push([0,0,0,0,0,0,223999,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">6 sec sorc Portal</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BJBJBJB##----##----#-BJBJBJB-#----##----#-BJBJBJB-#----#######-BJBTBJB-#######SMSX#-BJBJBJB-#----##SDPP#-BJBJBJB-#----##----##BJBJBJB##----##BBBBB##BBBBB##-----##BJJJJZ#######------##BBBBBBBB-#---------##BJJJJJJJ-#---------###BBBBBBB-#--------#####-------#-------########################");
                remarklol.push("sorc");
				notelol.push("240000 sorc @ 17 days");
                troopcounlol.push([0,0,0,0,0,0,239999,0,0,0,0,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">10 sec druids</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######SM-X#BJBJBJBJB#----##----#BJBJBJBJB#----##BBBB##JBJBJBJ##----##JJJJJ##BJBJB##-----##BBBBBB#######------##JJJJJJ---#---------##BBBBB----#---------###JJZ-----#--------#####-------#-------########################");
                remarklol.push("druids");
				notelol.push("102000 druids @ 11.5 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,101999,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">11 sec druids</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######SDPX#BJBJBJBJB#----##M---#BJBJBJBJB#----##----##JBJBJBJ##----##-----##BJBJB##-----##-----Z#######------##BBBBBBBBB#---------##BJJJJJJJJ#---------###BBBBBBBB#--------#####-------#-------########################");
                remarklol.push("druids");
				notelol.push("108000 druids @ 13.8 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,107999,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">14 sec druids Portal</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BJBJBJB##----##----#-BJBJBJB-#----##----#-BJBJBJB-#----#######-BBBTBJB-#######SMPX#-BJBJBJB-#----##SDP-#-BJBJBJB-#----##----##BJBJBJB##----##BBBBB##BBBBB##-----##BJJJJZ#######------##BBBBBBBB-#---------##BJJJJJJJB#---------###BBBBBBB-#--------#####-------#-------########################");
                remarklol.push("druids");
				notelol.push("124000 druids @ 20 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,123999,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">18 sec druids Portal</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BJBJBJB##----##----#-BJBJBJB-#----##----#-BJBJBJB-#----#######-BBBTBJB-#######SMPX#-BJBJBJB-#----##SDP-#-BJBJBJB-#----##----##BJBJBJB##----##BBBBB##BBBBB##-----##BJJJJZ#######------##BBBBBBBB-#---------##BJJJJJJJB#---------###BBBBBBB-#--------#####-------#-------########################");
                remarklol.push("druids");
				notelol.push("150000 druids @ 31.25 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,149999,0,0,0,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">scorp/rams 20s/32s</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##YBYBY##-----##----##BYBYBYB##----##----#-BYBYBYB-#----##----#YBYBYBYBY#----#######YBYBTBYBY#######SS-X#YBYBYBYBY#----##MD--#YBYBYBYBY#----##-BBB##BYBYBYB##----##-YYY-##YBYB-##-----##BBBBBB#######------##YYYYYY---#---------##BBBBBB---#---------###YYYZ----#--------#####BB-----#-------########################");
                remarklol.push("scorp/rams");
				notelol.push("3920 rams / 15680 scorps @ 6.7 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,0,3920,15680,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">scorp/rams 24s/39s</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBYB##-----##----##BBYBYBY##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######SMSX#BYBYBYBYB#----##SD--#BYBYBYBYB#----##----##BBYBYBY##----##-BBBB##BBBBB##-----##BYYYYB#######------##BBBBBB---#---------##BYYYYB---#---------###BBBB----#--------#####-------#-------########################");
                remarklol.push("scorp/rams");
				notelol.push("4720 rams / 18880 scorps @ 9.8 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,0,4720,18880,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">scorp/rams 28s/44s</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBYB##-----##----##BBYBYBY##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######SMSX#BYBYBYBYB#----##SD--#BYBYBYBYB#----##----##BBYBYBY##----##-BBBB##BBBBB##-----##BYYYYB#######------##BBBBBB---#---------##BYYYYB---#---------###BBBB----#--------#####-------#-------########################");
                remarklol.push("scorp/rams");
				notelol.push("5200 rams / 20800 scorps @ 12.3 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,0,5200,20800,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                lol++;
                selectbuttlandoff+='<option value="'+lol+'">scorps Portal 44s</option>';
                layoutlol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBYB##-----##----##BBYBYBY##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######SMSX#BYBYBYBYB#----##SD--#BYBYBYBYB#----##----##BBYBYBY##----##-BBBB##BBBBB##-----##BYYYYB#######------##BBBBBB---#---------##BYYYYB---#---------###BBBB----#--------#####-------#-------########################");
                remarklol.push("scorpions");
				notelol.push("26000 scorps @ 13.25 days");
                troopcounlol.push([0,0,0,0,0,0,0,0,0,0,0,0,0,26000,0,0,0]);
                reslol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                selectbuttlandoff+='</select>';


				var selectbuttlanddef='<select id="landdefenselayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Land Defense Layouts</option>';
                var ldl=1;

				selectbuttlanddef+='<option value="'+ldl+'">2 sec rangers</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######SSPX#BGBGBGBGB#----##MLPJ#BGBGBGBGB#----##S---##BBGBGBG##----##-----##BBBGB##-----##--BBBB#######------##-BGGGG---#---------##-BBBBBB--#---------###-GGGG---#--------#####BBBB---#-------########################");
                remarkldl.push("Rangers"); noteldl.push("228000 inf @ 5.2 days");
                troopcounldl.push([0,0,228000,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">3 sec rangers</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##GBGBGBG##----##----#BBBGBGBBB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######SMSX#BGBBBBBGB#----##SLPP#BBBGBGBBB#----##----##GBGBGBG##----##-----##BBBBB##-----##------#######------##----BBBBB#---------##---BBGGGB#---------###--JBGBGB#--------#####-BBBBBB#-------########################");
                remarkldl.push("Rangers");
				noteldl.push("268000 inf @ 9.3 days");
                troopcounldl.push([0,0,268000,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">2s/3s Rangers/Triari</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######SSPX#BGBGBGBGB#----##MLPJ#BGBGBGBGB#----##S---##BBGBGBG##----##-----##BBBGB##-----##--BBBB#######------##-BGGGG---#---------##-BBBBBB--#---------###-GGGG---#--------#####BBBB---#-------########################");
                remarkldl.push("rangers/triari");
				noteldl.push("228000 inf @ 6.2 days");
                troopcounldl.push([0,0,152000,76000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">3s/3s Rangers/Triari</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##-BGBGBB##----##----#BBBGBGBGB#----##----#BGBGBGBGB#----#######BGBBTGBGB#######SLSX#BGBGBGBGB#----##SMPP#BGBGBGBGB#----##----##BBGBGBB##----##-----##BBBBB##-----##--BBBB#######------##--BGBGB--#---------##--BGBGB--#---------###JBGBGB--#--------#####BBBBB--#-------########################");
                remarkldl.push("rangers/triari");
				noteldl.push("252000 inf @ 6.2 days");
                troopcounldl.push([0,0,168000,84000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">3 sec Priestess</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######SMSX#BZBZBZBZB#----##SDPP#BZBZBZBZB#----##----##BBZBZBB##----##-BBBB##BBBBB##-----##-ZZZZ-#######------##BBBBBB---#---------##JZZZZB---#---------###BBBB----#--------#####-------#-------########################");
                remarkldl.push("priests");
				noteldl.push("228000 Priestess @ 7.9 days");
                troopcounldl.push([0,0,0,0,228000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">4 sec Priestess</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##ZBZBZBZ##----##----#BBBZBZBBB#----##----#BZBZBZBZB#----#######BZBBTBBZB#######SMSX#BZBZBZBZB#----##SDPP#BBBZBZBBB#----##--PP##ZBZBZBZ##----##-----##BBBBB##-----##----BB#######------##----BJBBB#---------##----BBBZB#---------###---BZBZB#--------#####--BBBBB#-------########################");
                remarkldl.push("priests");
				noteldl.push("256000 Priestess @ 12.2 days");
                troopcounldl.push([0,0,0,0,256000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">5 sec Priestess</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BZBZBZB##----##----#-BBBBBBB-#----##----#-BZBZBZB-#----#######-BBBTBZB-#######SMSX#-BZBZBZB-#----##SDPP#-BBBBBZB-#----##----##BZBZBZB##----##-----##BBBBB##-----##BBBBB-#######------##BZBZB----#---------##BBBBBBBBB#---------###JBZBZBZB#--------#####BBBBBBB#-------########################");
                remarkldl.push("priests");
				noteldl.push("288000 Priestess @ 16.7 days");
                troopcounldl.push([0,0,0,0,288000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">6 sec praetors</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-BZB-##-----##----##BZBZBZB##----##----#-BZBZBZB-#----##----#-BZBZBZB-#----#######-BZBTBZB-#######SMPX#-BZBZBZB-#----##S-P-#-BZBZBZB-#----##----##BZBZBZB##----##-----##BBZB-##-----##BBBBBB#######------##ZZZZZZZBJ#---------##BBBBBBBBB#---------###ZZZZZZZZ#--------#####BBBBBBB#-------########################");
                remarkldl.push("praetors");
				noteldl.push("112000 praetors @ 7.75 days");
                troopcounldl.push([0,0,0,0,0,0,0,0,0,112000,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">7 sec praetors</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBZBB##-----##----##BZBZBZB##----##----#-BZBZBZB-#----##----#-BZBZBZB-#----#######-BZBTBZB-#######SSPX#-BZBZBZB-#----##MDPJ#-BZBZBZB-#----##S---##BZBZBZB##----##-----##BBZBB##-----##-BBBBB#######------##-BZBZBZB-#---------##BBZBZBZB-#---------###BZBZBZB-#--------#####BBBBBB-#-------########################");
                remarkldl.push("praetors");
				noteldl.push("120000 praetors @ 9.7 days");
                troopcounldl.push([0,0,0,0,0,0,0,0,0,120000,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,150000,220000,150000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,150000,350000]);
                ldl++;
				selectbuttlanddef+='<option value="'+ldl+'">5 sec arbs</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######MSLX#BEBEBEBEB#----##--PP#BEBEBEBEB#----##----##BBEBEBB##----##-JBBB##BEBEB##-----##-BEBEB#######------##-BEBEB---#---------##-BEBEB---#---------###BEBEB---#--------#####BB-----#-------########################");
                remarkldl.push("arbs");
				noteldl.push("110000 arbs @ 6.5 days");
                troopcounldl.push([0,0,0,0,0,0,0,0,110000,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,250000,250000,150000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">6 sec arbs</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BEBEBEB##----##----#-BEBEBEB-#----##----#-BEBEBEB-#----#######-BEBTBEB-#######SMSX#-BEBEBEB-#----##SLPP#-BEBEBEB-#----##----##BEBEBEB##----##-BBBB##BBBBB##-----##-EEEEJ#######------##BBBBBBBBB#---------##BEEEEEEEB#---------###BBBBBBB-#--------#####-------#-------########################");
                remarkldl.push("arbs");
				noteldl.push("120000 arbs @ 8.3 days");
                troopcounldl.push([0,0,0,0,0,0,0,0,124000,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,250000,250000,150000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">7 sec arbs</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBBBBBEB#----#######BEBETEBEB#######SMSX#BEBBBBBEB#----##SLPP#BEBEBEBEB#----##----##BBEBEBB##----##-----##BBBBB##-----##------#######------##---BBBBBJ#---------##--BEBEBEB#---------###-BEBEBEB#--------#####--BBBB-#-------########################");
                remarkldl.push("arbs");
				noteldl.push("130000 arbs @ 10.5 days");
                troopcounldl.push([0,0,0,0,0,0,0,0,124000,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,250000,250000,150000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,150000,350000]);
                ldl++;
                selectbuttlanddef+='<option value="'+ldl+'">8 sec arbs</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##-BEBEBJ##----##----#BBBEBEBBB#----##----#BEBBBBBEB#----#######BEBETEBEB#######SMSX#BEBBBBBEB#----##SLPP#BBBEBEBBB#----##----##-BEBEB-##----##-----##BBBBB##-----##------#######------##---BBBBBB#---------##--BEBEBEB#---------###-BEBEBEB#--------#####BBBBBBB#-------########################");
                remarkldl.push("arbs");
				noteldl.push("138000 arbs @ 12.8 days");
                troopcounldl.push([0,0,0,0,0,0,0,0,124000,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,250000,250000,150000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,150000,350000]);
                ldl++;
				selectbuttlanddef+='<option value="'+ldl+'">ballista</option>';
                layoutldl.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBYBYBB##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######MSDX#BYBYBYBYB#----##----#BYBYBYBYB#----##----##BBYBYBB##----##-BBBB##BBBBB##-----##-BYBYB#######------##-BYBYB---#---------##-BYBYB---#---------###BYBYB---#--------#####BBBB---#-------########################");
                remarkldl.push("baldlista");
				noteldl.push("25600 siege engines @ 10.5 days");
                troopcounldl.push([0,25600,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resldl.push([0,0,0,0,1,150000,220000,250000,350000,0,0,0,0,1,0,0,0,0,0,150000,220000,250000,350000]);
                selectbuttlanddef+='</select>';


				var selectbuttwateroff='<select id="wateroffenselayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Water Offense Layouts</option>';
                var wol=1;


				selectbuttwateroff+='<option value="'+wol+'">2 sec vanq/galley+senator</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##BBGBGBB##---B##-----##BGBGB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#--BBBBV########################");
                remarkwol.push("vanq/galley+senator");
				notewol.push("193299 inf and 387 galley @ 14 days");
                troopcounwol.push([0,0,0,0,0,1932990,0,0,0,0,0,0,0,0,387,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wol++;
				selectbuttwateroff+='<option value="'+wol+'">3 sec vanq/galley+senator</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##BBGBGBB##---B##-----##BGBGB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#--BBBBV########################");
                remarkwol.push("vanq/galley+senator");
				notewol.push("219999 inf and 440 galley @ 16 days");
                troopcounwol.push([0,0,0,0,0,219999,0,0,0,0,0,0,0,0,440,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wol++;
				selectbuttwateroff+='<option value="'+wol+'">4 sec vanq/galley+senator</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBGB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##BBGBGB-##----##-----##BBBBB##BBBBB##------#######BBVVBB##---------#SS-BV##VB##---------#DM-BV###V###--------#SP-BBV#######-------#XP-ZBBV########################");
                remarkwol.push("vanq/galley+senator");
				notewol.push("232999 inf and 440 galley @ 22 days");
                troopcounwol.push([0,0,0,0,0,232999,0,0,0,0,0,0,0,0,466,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wol++;
                selectbuttwateroff+='<option value="'+wol+'">5 sec horses/galley</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#---H#######BEBETEBEB#######----#BEBEBEBEB#JSPX##----#BEBEBEBEB#-M--##----##EBEBEBB##----##-----##BEBEB##BBBB-##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#--BBBV#######-------#--BEBBV########################");
                remarkwol.push("horses/galley");
				notewol.push("90000 cav and 360 galley @ 10.5 days");
                troopcounwol.push([0,0,0,0,0,0,0,0,0,0,90000,0,0,0,360,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,350000]);
                wol++;
				selectbuttwateroff+='<option value="'+wol+'">6 sec horses/galley</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##EBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SMSX##----#BEBEBEBEB#SDPP##----##EBEBEBB##----##-----##BBBBB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---JBBV########################");
                remarkwol.push("horses/galley");
				notewol.push("95000 cav and 380 galley @ 16 days");
                troopcounwol.push([0,0,0,0,0,0,0,0,0,0,95000,0,0,0,380,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,350000]);
                wol++;
				selectbuttwateroff+='<option value="'+wol+'">7 sec horses/galley</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##BBBBBBB##----##----#BEEEEEEEB#----##----#BBBBBBBBB#----#######EEEETEEEB#######----#BBBBBBBBB#BBZ-##----#BEEEEEEEB#BBBB##----##BBBBBBB##BEBB##-----##-----##BBBBB##------#######BBVVBB##---------#SS-BV##VB##---------#M--BV###V###--------#PP-BBV#######-------#X--JBBV########################");
                remarkwol.push("horses/galley");
				notewol.push("103299 cav and 414 galley @ 18.5 days");
                troopcounwol.push([0,0,0,0,0,0,0,0,0,0,103299,0,0,0,414,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,350000]);
                wol++;
                selectbuttwateroff+='<option value="'+wol+'">5 sec sorc/galley</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##JBJBJ##-----##----##BJBJBJB##----##----#JBJBJBJBJ#----##----#JBJBJBJBJ#---H#######JBJBTBJBJ#######----#JBJBJBJBJ#-S-X##----#JBJBJBJBJ#----##----##BJBJBJB##JJ--##-----##JBJBJ##BBBBJ##------#######BBVVBB##---------#--JBV##VB##---------#--JBV###V###--------#---BBV#######-------#---JBBV########################");
                remarkwol.push("sorc/galley");
				notewol.push("156600 sorc and 314 galley @ 13.5 days");
                troopcounwol.push([0,0,0,0,0,0,156600,0,0,0,0,0,0,0,314,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,350000]);
                wol++;
				selectbuttwateroff+='<option value="'+wol+'">6 sec sorc/galley</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##JJJJJJJ##----##----#BBBBBBBBB#----##----#JJJJJJJJJ#----#######BBBBTBBBB#######----#JJJJJJJJJ#----##----#BBBBBBBBB#----##----##JJJJJJJ##BJ--##-----##BBBBB##BBBBE##------#######BBVVBB##---------#SS-BV##VB##---------#M--BV###V###--------#P--BBV#######-------#X--ZBBV########################");
                remarkwol.push("sorc/galley");
				notewol.push("173299 sorc and 387 galley @ 25 days");
                troopcounwol.push([0,0,0,0,0,0,173299,0,0,0,0,0,0,0,387,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,350000]);
                wol++;
				selectbuttwateroff+='<option value="'+wol+'">7 sec sorc/galley</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##JJJJJJJ##----##----#BBBBBBBBB#----##----#JJJJJJJJJ#----#######BBBBTBBBB#######----#JJJJJJJJJ#----##----#BBBBBBBBB#----##----##JJJJJJJ##BJ--##-----##BBBBB##BBBBE##------#######BBVVBB##---------#SS-BV##VB##---------#M--BV###V###--------#P--BBV#######-------#X--ZBBV########################");
                remarkwol.push("sorc/galley");
				notewol.push("193299 sorc and 347 galley @ 20.5 days");
                troopcounwol.push([0,0,0,0,0,0,193299,0,0,0,0,0,0,0,347,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,350000]);
                wol++;
                selectbuttwateroff+='<option value="'+wol+'">vanqs+ports+senator</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######----#BGBBBBBGB#PPJX##----#BGBGBGBGB#BBBB##----##BBGBGBB##BBBB##-----##BBBBB##BBBBB##------#######-BRRBB##---------#----R##RZ##---------#----R###R###--------#----SR#######-------#----MSR########################");
                remarkwol.push("vanqs+senator+ports");
				notewol.push("264k infantry @ 10 days");
                troopcounwol.push([0,0,0,100000,0,164000,0,0,0,0,0,0,0,0,0,0,0]);
                reswol.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,350000]);
                wol++;
                selectbuttwateroff+='<option value="'+wol+'">Warships</option>';
                layoutwol.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######-SS-TP---#######-XBB#-ML--P---#BBBB##--BB#-S-------#BBBB##--BB##-------##BBBB##--BBB##-----##BBBBB##--BBBB#######BBVVBB##--BBBBBBB#BBBBV##VB##-BBJBZBBB#BBBBV###V###BBBBBBBB#BBBBBV#######BBBBBBB#BBBBBBV########################");
                remarkwol.push("warships"); notewol.push("819 warships @ 42 days");
                troopcounwol.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,819]);
                reswol.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
                selectbuttwateroff+='</select>';



				var selectbuttwaterdef='<select id="waterdefenselayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Water Defense Layouts</option>';
                var wdl=1;

				selectbuttwaterdef+='<option value="'+wdl+'">2/3 sec R/T/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##GBGBGBG##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##GBGBGBG##G---##-----##BGGGB##BBBBG##------#######BBVVBB##---------#--GBV##VB##---------#--GBV###V###--------#---BBV#######-------#----BBV########################");
                remarkwdl.push("R/T/galley");
				notewdl.push("166600 inf and 334 galley @ 10 days");
                troopcounwdl.push([0,0,83300,83300,0,0,0,0,0,0,0,0,0,0,334,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1450000]);
                wdl++;
                selectbuttwaterdef+='<option value="'+wdl+'">3/3 sec R/T/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##GBGBGBG##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##GBGBGBG##G---##-----##BGGGB##BBBBG##------#######BBVVBB##---------#--GBV##VB##---------#--GBV###V###--------#---BBV#######-------#----BBV########################");
                remarkwdl.push("R/T/galley");
				notewdl.push("180000 inf and 360 galley @ 15 days");
                troopcounwdl.push([0,0,90000,90000,0,0,0,0,0,0,0,0,0,0,360,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1450000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">2 sec Ranger/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BGBGB##-----##----##GBGBGBG##----##----#BGBGBGBGB#----##----#BGBGBGBGB#---H#######BGBGTGBGB#######----#BGBGBGBGB#JSPX##----#BGBGBGBGB#----##----##GBGBGBG##G---##-----##BGGGB##BBBBG##------#######BBVVBB##---------#--GBV##VB##---------#--GBV###V###--------#---BBV#######-------#----BBV########################");
                remarkwdl.push("Ranger/galley");
				notewdl.push("166600 inf and 334 galley @ 10 days");
                troopcounwdl.push([0,0,166000,0,0,0,0,0,0,0,0,0,0,0,334,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1450000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">3 sec Ranger/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-BBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######----#BGBGBGBGB#SMSX##----#BGBGBGBGB#SLPP##----##BBGBGBB##----##-----##BBBBB##BBBBJ##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---BBBV########################");
                remarkwdl.push("Ranger/galley");
				notewdl.push("196600 inf and 394 galley @ 16 days");
                troopcounwdl.push([0,0,196600,0,0,0,0,0,0,0,0,0,0,0,394,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1450000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">4 sec Ranger/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBBBBBGB#----##----#BBBGBGBBB#----#######BGBBTBBGB#######----#BGBGBGBGB#SMSX##----#BGBGBGBGB#SLPP##----##BBGBGBB##----##-----##BBBBB##BBBBJ##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
                remarkwdl.push("Ranger/galley");
				notewdl.push("216600 inf and 434 galley @ 20.5 days");
                troopcounwdl.push([0,0,216600,0,0,0,0,0,0,0,0,0,0,0,434,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1450000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">3 sec priestess/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#---H#######BZBZTZBZB#######----#BZBZBZBZB#JSPX##----#BZBZBZBZB#----##----##ZBZBZBZ##-Z--##-----##BZZZB##BBBBZ##------#######BBVVBB##---------#---BV##VB##---------#--ZBV###V###--------#---BBV#######-------#---ZBBV########################");
                remarkwdl.push("priestess/galley");
				notewdl.push("166600 inf and 334 galley @ 11 days");
                troopcounwdl.push([0,0,0,0,166600,0,0,0,0,0,0,0,0,0,334,0,0]);
                reswdl.push([0,0,0,0,1,250000,220000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">4 sec priestess/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SMPX##----#BZBZBZBZB#SDPJ##----##BBZBZBB##----##-----##BZBBB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---BBBV########################");
                remarkwdl.push("priestess/galley");
				notewdl.push("189999 inf and 380 galley @ 18 days");
                troopcounwdl.push([0,0,0,0,189999,0,0,0,0,0,0,0,0,0,380,0,0]);
                reswdl.push([0,0,0,0,1,250000,220000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">5 sec priestess/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BBBZBZBBB#----#######BZBZTZBZB#######----#BBBZBZBBB#SMPX##----#BZBZBZBZB#SDPJ##----##BBZBZBB##----##-----##BZBBB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---BBBV########################");
                remarkwdl.push("priestess/galley");
				notewdl.push("209999 inf and 420 galley @ 22 days");
                troopcounwdl.push([0,0,0,0,209999,0,0,0,0,0,0,0,0,0,420,0,0]);
                reswdl.push([0,0,0,0,1,250000,220000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">6 sec priestess/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBZBZBB##----##----#BZBBBBBZB#----##----#BBBZBZBBB#----#######BZBBTBBZB#######----#BZBZBZBBB#SMSX##----#BZBZBZBZB#SDPP##----##BBZBZBB##----##-----##BBBBB##BBBBJ##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#----BBV########################");
                remarkwdl.push("priestess/galley");
				notewdl.push("219999 inf and 440 galley @ 22 days");
                troopcounwdl.push([0,0,0,0,219999,0,0,0,0,0,0,0,0,0,440,0,0]);
                reswdl.push([0,0,0,0,1,250000,220000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">6 sec arbs/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BEBEB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SMSX##----#BEBEBEBEB#SLPP##----##EBEBEBE##----##-----##-EBE-##BBBBE##------#######BBVVBB##---------#--JBVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---EBBV########################");
                remarkwdl.push("arbs/galley");
				notewdl.push("81650 inf and 327 galley @ 13.5 days");
                troopcounwdl.push([0,0,0,0,0,0,0,0,81650,0,0,0,0,0,327,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,150000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,150000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">7 sec arbs/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##EBEBEBE##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SMSX##----#BEBEBEBEB#SLPP##----##EBEBEBE##----##-----##BBBBB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---JBBV########################");
                remarkwdl.push("arbs/galley");
				notewdl.push("91650 inf and 367 galley @ 16.5 days");
                troopcounwdl.push([0,0,0,0,0,0,0,0,91650,0,0,0,0,0,367,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,150000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,150000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">8 sec arbs/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SMSX##----#BEBEBEBEB#SLPP##----##BBEBEBB##----##-----##BBBBB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---JBBV########################");
                remarkwdl.push("arbs/galley");
				notewdl.push("98300 inf and 394 galley @ 16.5 days");
                troopcounwdl.push([0,0,0,0,0,0,0,0,98300,0,0,0,0,0,394,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,150000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,150000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">7 sec praetor/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SPJX##----#BZBZBZBZB#MH--##----##ZBZBZBZ##----##-----##BZBZB##BBBBZ##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#--BZBBV########################");
                remarkwdl.push("praetors/galley");
				notewdl.push("86650 praetors and 347 galley @ 12 days");
                troopcounwdl.push([0,0,0,0,0,0,0,0,0,86650,0,0,0,0,347,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">8 sec praetor/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBZB##-----##----##ZBZBZBZ##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SMSX##----#BZBZBZBZB#SDPP##----##ZBZBZBZ##----##-----##BBBBB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---JBBV########################");
                remarkwdl.push("praetors/galley");
				notewdl.push("89999 praetors and 360 galley @ 17 days");
                troopcounwdl.push([0,0,0,0,0,0,0,0,0,89999,0,0,0,0,360,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wdl++;
				selectbuttwaterdef+='<option value="'+wdl+'">9 sec praetor/galley</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SMSX##----#BZBZBZBZB#SDPP##----##ZBZBZBB##----##-----##BBBBB##BBBB-##------#######BBVVBB##---------#---BVTTVB##---------#---BVTTTV###--------#---BBVTT#####-------#---JBBV########################");
                remarkwdl.push("praetors/galley");
				notewdl.push("96649 praetors and 387 galley @ 19.5 days");
                troopcounwdl.push([0,0,0,0,0,0,0,0,0,96649,0,0,0,0,387,0,0]);
                reswdl.push([0,0,0,0,1,250000,250000,250000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,350000,1350000]);
                wdl++;
                selectbuttwaterdef+='<option value="'+wdl+'">Stingers</option>';
                layoutwdl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######-SS-TPP--#######-XBB#-ML--PP--#BBBB##--BB#-S-------#BBBB##--BB##-------##BBBB##--BBB##-----##BBBBB##--BBBB#######BBVVBB##--BBBBBBB#BBBBV##VB##--BJBZBBB#BBBBV###V###-BBBBBBB#BBBBBV#######BBBBBBB#BBBBBBV########################");
                remarkwdl.push("stingers"); notewdl.push("3198 stingers @ 49days");
                troopcounwdl.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,3198,0]);
                reswdl.push([0,0,0,0,1,500000,500000,500000,500000,0,0,0,0,1,0,0,0,0,0,500000,500000,500000,500000]);
                selectbuttwaterdef+='</select>';


				var selectbutthub='<select id="hublayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Hubs Layouts</option>';
                var hul=1;

				selectbutthub+='<option value="'+hul+'">Cluster Hub 9K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#SASMSDSLS#----##----#SASMSDSLS#----#######SASMTDSLS#######----#SASMSDSLS#PPPP##----#SASMSDSLS#PPPP##----##-------##PPPP##-----##-----##PPPPP##------#######PPRRPP##--------P#PPPPR##RP##-------ZP#PPPPR###R###------BP#PPPPPR#######-----JP#PPPPPPR########################");
                remarkhul.push("Hub");
				notehul.push("9K Carts, 15375000 W/S, 14175000 I/F ");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,15375000,15375000,14175000,14175000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Cluster Hub 6.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##MSMSM##-----##----##SSSSSSS##----##----#-AAAAAAM-#----##----#-SSSSSSS-#----#######-MMMTDDD-#######----#-SSSSSSS-#PPPP##----#-LLLLLLD-#PPPP##----##SSSSSSS##PPPP##-----##DSDSD##PPPPP##------#######PPRRPP##---------#J-PPR##RP##---------#B-PPR###R###--------#Z--PPR#######-------#---PPPR########################");
                remarkhul.push("Hub");
		notehul.push("6200K Carts, 20,175,000 W/S, 19,175,000 I/F ");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,20175000,20175000,19175000,19175000]);
                hul++;

		selectbutthub+='<option value="'+hul+'">Cluster Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SASLS##-----##----##DSASLSM##----##----#SDSASLSMS#----##----#SDSASLSMS#----#######SDSATLSMS#######----#SDSASLSMS#PPPP##----#SDSASLSMS#PPPP##----##DSASLSM##PPPP##-----##SASLS##PPPPP##------#######-PRRPP##---------#Z---R##RP##---------#B---R###R###--------#J----R#######-------#------R########################");
                remarkhul.push("Hub");
		notehul.push("4200K Carts, 26,175,000 W/S, 21,175,000 I/F use cluster transport with this ");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,26175000,26175000,21175000,21175000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Maj Hub Land 14.2K</option>';
                layouthul.push("[ShareString.1.3]:########################-------#PPPPPPP#####--------#PPPPPPPP###---------#PPPPPPPPP##---------#PPPPPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##-------##PPPP##----#---SLS---#PPPP##----#-SLSMSAS-#PPPP#######-SDSTSDS-#######----#-SLSMSAS-#PPPP##----#---SAS---#PPPP##----##-------##PPPP##-----##ZBJ--##--PPP##------#######------##---------#---------##---------#---------###--------#--------#####-------#-------########################");
                remarkhul.push("Hub");
		notehul.push("14200K Carts, 8,975,000 W/S, 8,175,000");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,8975000,8975000,8175000,8175000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Maj Hub Water 12.6K</option>';
                layouthul.push("[ShareString.1.3];########################-------#PPPPPPP#####--------#PPPPPPPP###---------#PPPPPPPPP##---------#PPPPPPPPP##------#######PPPPPP##-----##-----##PPPPP##----##-------##PPPP##----#---SLS---#PPPP##----#-SLSMSAS-#PPPP#######-SDSTSDS-#######----#-SLSMSAS-#PPPP##----#---SAS---#-PPP##----##-------##----##-----##ZBJ--##-----##------#######--RR--##---------#----R##R-##---------#----R###R###--------#-----R#######-------#------R########################");
                remarkhul.push("Hub");
		notehul.push("12600K Carts, 8,975,000 W/S, 8,175,000 I/F");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,8975000,8975000,8175000,8175000]);
                hul++;


		selectbutthub+='<option value="'+hul+'">Cluster transport 12.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######--PPPP##-----##-----##-PPPP##----##MMM----##PPPP##----#SSSS-----#PPPP##----#AAAAM-J--#PPPP#######SSSST-B--#######----#LLLLD-Z--#PPPP##----#SSSS-----#PPPP##----##DDD----##PPPP##-----##-----##PPPPP##------#######PPRRPP##---------#PPPPR##RP##---------#PPPPR###R###--------#PPPPPR#######-------#PPPPPPR########################");
                remarkhul.push("Hub");
		notehul.push("12,200K Carts, 10,575,000 W/S, 6,575,000 I/F");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,10575000,10575000,6575000,6575000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Temple Storage</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SLSAS##-----##----##LSLSASA##----##----#SLSLSASAS#----##----#SLSLSASAS#----#######SLSLTASAS#######----#SLSLSASAS#JBZ-##----#SLSLSASAS#----##----##LSLSASA##----##-----##SLSAS##PPPPP##------#######PPRRPP##---------#--PPRTTRP##---------#--PPRTTTR###--------#--PPPRTT#####-------#--PPPPR########################");
                remarkhul.push("Temple Storage");
		notehul.push("4200K Carts, 40,575,000 W/S, 7,375,000 I/F");
               troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,40575000,40575000,7375000,7375000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Temple Hub</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#SASASLSLS#----##----#SASASLSLS#----#######SASATLSLS#######----#SASASLSLS#PPPP##----#SASASLSLS#PPPP##----##-------##PPPP##-----##-----##PPPPP##------#######PPRRPP##--------P#PPPPRTTRP##-------ZP#PPPPRTTTR###------BP#PPPPPRTT#####-----JP#PPPPPPR########################");
                remarkhul.push("Temple Hub");
		notehul.push("9000K Carts, 24,575,000 W/S, 4,975,000 I/F");
               troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,24575000,24575000,4975000,4975000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Temple Transport</option>';
                layouthul.push("[ShareString.1.3];########################-------#------P#####--------#-----PPP###---------#-----PPPP##---------#-----PPPP##------#######--PPPP##-----##-----##-PPPP##----##-------##PPPP##----#-SS------#PPPP##----#AAAA--J--#PPPP#######SSSST-B--#######----#LLLL--Z--#PPPP##----#-SS------#PPPP##----##-------##PPPP##-----##-----##PPPPP##------#######PPRRPP##---------#PPPPR##RP##---------#PPPPR###R###--------#PPPPPR#######-------#PPPPPPR########################");
                remarkhul.push("Temple Transport");
		notehul.push("14,600 Carts, 8,175,000 W/S, 1,775,000 I/F");
               troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,8175000,8175000,1775000,1775000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Food Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SMSMS##-----##----##MSMSMSM##----##----#SMSMSMSMS#----##----#SMSMSMSMS#----#######SMSMTMSMS#######----#SMSMSMSMS#JBZ-##----#SMSMSMSMS#----##----##MSMSMSM##----##-----##SMSMS##PPPPP##------#######PPRRPP##---------#--PPRTTRP##---------#--PPRTTTR###--------#--PPPRTT#####-------#--PPPPR########################");
                remarkhul.push("Food Hub");
		notehul.push("4200K Carts, 7,375,000 W/S/I, 73,775,000 Food");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,7375000,7375000,7375000,73775000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Iron Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SDSDS##-----##----##DSDSDSD##----##----#SDSDSDSDS#----##----#SDSDSDSDS#----#######SDSDTDSDS#######----#SDSDSDSDS#JBZ-##----#SDSDSDSDS#----##----##DSDSDSD##----##-----##SDSDS##PPPPP##------#######PPRRPP##---------#--PPRTTRP##---------#--PPRTTTR###--------#--PPPRTT#####-------#--PPPPR########################");
                remarkhul.push("Iron Hub");
		notehul.push("4200K Carts, 7,375,000 W/S/F, 73,775,000 Iron");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,7375000,7375000,73775000,7375000]);
                hul++;

              selectbutthub+='<option value="'+hul+'">Stone Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SASAS##-----##----##ASASASA##----##----#SASASASAS#----##----#SASASASAS#----#######SASATASAS#######----#SASASASAS#JBZ-##----#SASASASAS#----##----##ASASASA##----##-----##SASAS##PPPPP##------#######PPRRPP##---------#--PPRTTRP##---------#--PPRTTTR###--------#--PPPRTT#####-------#--PPPPR########################");
                remarkhul.push("Stone Hub");
		notehul.push("4200K Carts, 7,375,000 W/I/F, 73,775,000 Stone");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,7375000,73775000,7375000,7375000]);
                hul++;

                 selectbutthub+='<option value="'+hul+'">Wood Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SLSLS##-----##----##LSLSLSL##----##----#SLSLSLSLS#----##----#SLSLSLSLS#----#######SLSLTLSLS#######----#SLSLSLSLS#JBZ-##----#SLSLSLSLS#----##----##LSLSLSL##----##-----##SLSLS##PPPPP##------#######PPRRPP##---------#--PPRTTRP##---------#--PPRTTTR###--------#--PPPRTT#####-------#--PPPPR########################");
                remarkhul.push("Wood Hub");
		notehul.push("4200K Carts, 7,375,000 S/I/F, 73,775,000 Wood");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,73775000,7375000,7375000,7375000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Wood/Stone Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SASLS##-----##----##ASASLSL##----##----#SASASLSLS#----##----#SASASLSLS#----#######SASATLSLS#######----#SASASLSLS#----##----#SASASLSLS#----##----##ASASLSL##PPPP##-----##SASLS##PPPPP##------#######PPRRPP##---------#---PR##RP##---------#Z--PR###R###--------#B--PPR#######-------#J--PPPR########################");
                remarkhul.push("Wood/Stone Hub");
		notehul.push("4200K Carts, 40,575,000 W/S, 7,375,000 I/F");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,40575000,40575000,7375000,7375000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Stone/Food Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SASMS##-----##----##ASASMSM##----##----#SASASMSMS#----##----#SASASMSMS#----#######SASATMSMS#######----#SASASMSMS#PPPP##----#SASASMSMS#PPPP##----##ASASMSM##PPPP##-----##SASMS##PPPPP##------#######-PRRPP##---------#----R##RP##-------Z-#----R###R###------B-#-----R#######-----J-#------R########################");
                remarkhul.push("Stone/Food Hub");
		notehul.push("4200K Carts, 40,575,000 S/F, 7,375,000 W/I");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,7375000,40575000,7375000,40575000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Stone/Iron Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SASDS##-----##----##ASASDSD##----##----#SASASDSDS#----##----#SASASDSDS#----#######SASATDSDS#######----#SASASDSDS#PPPP##----#SASASDSDS#PPPP##----##ASASDSD##PPPP##-----##SASDS##PPPPP##------#######-PRRPP##---------#----R##RP##---------#Z---R###R###--------#B----R#######-------#J-----R########################");
                remarkhul.push("Stone/Iron Hub");
		notehul.push("4200K Carts, 40,575,000 S/I, 7,375,000 W/F");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,7375000,40575000,40575000,7375000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Iron/Wood Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SASMS##-----##----##ASASMSM##----##----#SASASMSMS#----##----#SASASMSMS#----#######SASATMSMS#######----#SASASMSMS#PPPP##----#SASASMSMS#PPPP##----##ASASMSM##PPPP##-----##SASMS##PPPPP##------#######-PRRPP##---------#----R##RP##-------Z-#----R###R###------B-#-----R#######-----J-#------R########################");
                remarkhul.push("Iron/Wood Hub");
		notehul.push("4200K Carts, 40,575,000 I/W, 7,375,000 S/F");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,40575000,7375000,40575000,7375000]);
                hul++;

                selectbutthub+='<option value="'+hul+'">Food/Wood Hub 4.2K</option>';
                layouthul.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##SLSMS##-----##----##LSLSMSM##----##----#SLSLSMSMS#----##----#SLSLSMSMS#----#######SLSLTMSMS#######----#SLSLSMSMS#PPPP##----#SLSLSMSMS#PPPP##----##LSLSMSM##PPPP##-----##SLSMS##PPPPP##------#######-PRRPP##---------#----R##RP##---------#Z---R###R###--------#B----R#######-------#J-----R########################");
                remarkhul.push("Food/Wood Hub");
		notehul.push("4200K Carts, 40,575,000" + "\n" + "I/W 7,375,000 S/F");
                troopcounhul.push([0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                reshul.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,40575000,7375000,7375000,40575000]);
                selectbutthub+='</select>';

				var selectbuttshipper='<select id="shipperlayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Shipper Layouts</option>';
                var shl=1;

                selectbuttshipper+='<option value="'+shl+'">252K 3/4 sec R/T Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBGBGBGB#----##----#BGBGBGBBB#----#######BGBGTGBGB#######----#BGBGBGBBB#SMSX##----#BGBGBGBGB#S---##----##BBGBGBB##----##-----##BBBBB##-----##---BBB#######--RR--##---JBBBBB#----RTTR-##----BGBGB#----RTTTR###---BBBBB#-----RTT#####-------#------R########################");
                remarkshl.push("252K 3/4s R/T Shipper");
		noteshl.push("252KTS 126K Rangers 126K Triari @ 10 days  Substitute sawmill, masons hut, smelter or grain mill, as appropriate");
                troopcounshl.push([0,0,126000,126000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">240K 3/4 sec R/T +Sen Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBBB#----##----#BGBBBBBGB#----#######BGBGTGBGB#######----#BGBGBGBGB#SLPX##----#BGBGBGBGB#SDPJ##----##BBGBGBB##----##-----##BBBBB##-----##------#######--RR--##---------#BBBBRTTR-##---------#BGBZRTTTR###--------#BGBB-RTT#####-------#BBBB--R########################");
                remarkshl.push("240K 3/4s R/T +Sen Shipper");
		noteshl.push("240KTS 120K Rangers 120K Triari @ 10 days  Substitute sawmill, masons hut, smelter or grain mill, as appropriate");
                troopcounshl.push([0,0,120000,120000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">224K 3/3 sec R/T Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######----#BGBGBGBGB#SLPX##----#BGBGBGBGB#S-P-##----##BBGBGBG##----##-----##BBBGB##-----##--BBBB#######--RR--##--BGBJ---#----RTTR-##--BGBB---#----RTTTR###-BGB----#-----RTT#####BBB----#------R########################");
                remarkshl.push("224K 3/3s R/T Shipper");
		noteshl.push("224KTS 112K Rangers 112K Triari @ 8 days  Substitute sawmill, masons hut, smelter or grain mill, as appropriate");
                troopcounshl.push([0,0,112000,112000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">248K 3sec Ranger Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BGGGGGB##----##----#BBBBBBBBB#----##----#BGGGBGGGB#----#######BBBBTBBBB#######----#BGGGBGGGB#SLPX##----#BBBBBBBBB#S-PJ##----##BGGBGGB##----##-----##BBBBB##-----##------#######--RR--##---------#BBBBRTTR-##---------#BGBBRTTTR###--------#BGBB-RTT#####-------#BBBBB-R########################");
                remarkshl.push("248K 3s Ranger Shipper");
		noteshl.push("248KTS 248K Rangers @ 9 days  Substitute sawmill, masons hut, smelter or grain mill, as appropriate");
                troopcounshl.push([0,0,248000,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">264K 4sec Ranger Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBGBGBBB#----##----#BGBBBBBGB#----#######BGB-TGBBB#######----#BGBBBBBGB#SSPJ##----#BBBGBGBBB#MLPX##----##BBGBGBB##S---##-----##BBBBB##-----##------#######--RR--##---BBBBB-#----R##R-##---BGGGBB#----R###R###--BBBBB-#-----R#######-------#------R########################");
                remarkshl.push("264K 3s Ranger Shipper");
		noteshl.push("264KTS 264K Rangers @ 12 days  Substitute sawmill, masons hut, smelter or grain mill, as appropriate");
                troopcounshl.push([0,0,264000,0,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">260K 6sec Priest Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BZBZBZB##----##----#BBZBBBZBB#----##----#BBZBZBZBB#----#######BBZBTBZBB#######----#BBZBZBZBB#DAPX##----#BBBBBBZBB#SSP-##----##BZBZBZB##ML--##-----##BBBBB##-----##------#######--RR--##---------#BBB-RTTR-##---------#BBBJRTTTR###--------#BBBB-RTT#####-------#BBBB--R########################");
                remarkshl.push("260K 6s Priestess Shipper");
		noteshl.push("260KTS 260K Priestess @ 18 days ");
                troopcounshl.push([0,0,0,0,260000,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">248K 5sec Priest Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BZBZBZB##----##----#-BBBBBBB-#----##----#-BZBZBZB-#----#######-BBBTBZB-#######----#-BZBZBZB-#SSPX##----#-BBBBBZB-#MDP-##----##BZBZBZB##S---##-----##BBBBB##-----##BBBBB-#######--RR--##BZZZB----#----R##R-##BBBBBB---#----R###R###JBZZZ---#-----R#######BBBB---#------R########################");
                remarkshl.push("248K 5s Priestess Shipper");
		noteshl.push("248KTS 248K Priestess @ 14.5 days ");
                troopcounshl.push([0,0,0,0,248000,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">236K 4sec Priest Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BZBZBZB##----##----#-BBBBBBB-#----##----#-BZBZBZB-#----#######-BBBTBZB-#######----#-BZBZBZB-#SSPX##----#-BBBBBZB-#MDP-##----##BZBZBZB##S---##-----##BBBBB##-----##BBBBB-#######--RR--##BZZZB----#----R##R-##BBBBBB---#----R###R###JBZZZ---#-----R#######BBBB---#------R########################");
                remarkshl.push("236K 4s Priestess Shipper");
		noteshl.push("236KTS 236K Priestess @ 11 days ");
                troopcounshl.push([0,0,0,0,236000,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">216K 7sec Praetor Ship</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BZBZBZB##----##----#-BBBBBBB-#----##----#-BZBZBZB-#----#######-BBBTBZB-#######----#-BZBZBZB-#SSPX##----#-BBBBBZB-#MDP-##----##BZBZBZB##S---##-----##BBBBB##-----##BBBBB-#######--RR--##BZZZB----#----R##R-##BBBBBB---#----R###R###JBZZZ---#-----R#######BBBB---#------R########################");
                remarkshl.push("216K 7s Praetor Shipper");
		noteshl.push("216KTS 108000 Praetor @ 8.75 days ");
                troopcounshl.push([0,0,0,0,0,0,0,0,0,108000,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">188K 5sec Priest Mini Hub</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBBTBBBB#######----#BZBZBZBZB#SASM##----#BZBZBZBZB#SLSD##----##BBZBZBB##----##-----##BBBBB##PPPP-##------#######PPRRPP##---------#X--PRTTRP##---------#---PRTTTR###--------#---PPRTT#####-------#----JPR########################");
                remarkshl.push("188K 5s Priest Mini Hub");
		noteshl.push("2800 carts, 188KTS 188000 Priest @ 11 days ");
                troopcounshl.push([0,0,0,0,188000,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                shl++;

                selectbuttshipper+='<option value="'+shl+'">176K 3/4sec R/T Mini Hub</option>';
                layoutshl.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBGBGBGB#----#######BGBGTGBGB#######----#BGBGBGBGB#----##----#BGBGBGBGB#----##----##BBGBGBB##----##-----##BBBBB##PPPP-##------#######PPRRPP##---------#AL-PRTTRP##---------#SS-PRTTTR###--------#MD-PPRTT#####-------#XJZ-PPR########################");
                remarkshl.push("176K 3/4sec R/T Mini Hub");
		noteshl.push("3000 carts, 176KTS 88K rangers, 88K Triari @ 7 days ");
                troopcounshl.push([0,0,88000,88000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                resshl.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,350000,350000,300000,1400000]);
                selectbuttshipper+='</select>';

				var selectbuttportal='<select id="portallayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Portal Layouts</option>';
                var pol=1;

                selectbuttportal+='<option value="'+pol+'">3 sec vanqs </option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBGBGBBB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######SSPX#BGBBBBBGB#----##MDP-#BBBGBGBBB#----##S---##BBGBGBB##----##-----##BBBBB##-----##------#######------##---BBBBBB#---------##---BGGBGB#---------###--BBBBBB#--------#####-JBZBBB#-------########################");
                remarkpol.push("vanqs");
				notepol.push("292000 vanqs @ 10 days");
                troopcounpol.push([0,0,0,0,0,291999,0,0,0,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">4 sec vanqs Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBGB#######SMSX#BGBGBGBGB#----##SDPP#BBBBBBBBB#----##----##BBGBGBB##----##-----##BBBBB##-----##------#######------##---BBBBBB#---------##---BBBBBB#---------###--BBBBBB#--------#####-BBJBZB#-------########################");
                remarkpol.push("vanqs");
				notepol.push("308000 vanqs @ 14.5 days");
                troopcounpol.push([0,0,0,0,0,307999,0,0,0,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">6 sec Sorcs Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBJBJBB##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#SPJX##----#BJBJBJBJB#SP--##----##BBJBJBB##BBBB##-----##BBBBB##BJBJB##------#######-BJBJB##---------#----BJBJB##---------#----BBBBB###--------#-----B-B#####-------#-------########################");
                remarkpol.push("Sorcs");
				notepol.push("256000 Sorcs @ 17.5 days");
                troopcounpol.push([0,0,0,0,0,0,256000,0,0,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">5 sec Horse Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BEEEEEB##----##----#BBBBBBBBB#----##----#BEEEEEEEB#----#######BBBBTBBBB#######----#BEEEEEEEB#SPJX##----#BBBBBBBBB#S---##----##BEEEEEB##BBB-##-----##BBBBB##BBBBB##------#######-BEEEB##---------#----BBBBB##---------#----BEEEB###--------#----BBBB#####-------#-------########################");
                remarkpol.push("Horse");
				notepol.push("260KTS 130,000 Horse @ 7.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,130000,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">6 sec Horse Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBBBBBEB#----#######BEBETEBEB#######----#BEBBBBBEB#SMPX##----#BEBEBEBEB#SDPJ##----##BBEBEBB##----##-----##BBBBB##-----##------#######BBBBB-##---------#---BEBEB-##---------#---BBBBB-###--------#---BEBEB#####-------#---BBBB########################");
                remarkpol.push("Horse");
				notepol.push("272KTS 136,000 Horse @ 9.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,136000,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">10 sec Druids Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#SS-X##----#BJBJBJBJB#----##----##JBJBJBJ##----##-----##BJBJB##BBBBB##------#######JJJJJJ##---------#--BBBBBBB##---------#--JJJJJJ-###--------#--BBBBB-#####-------#-------########################");
                remarkpol.push("Druids");
				notepol.push("212KTS 106,000 Druids @ 12.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,106000,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">11 sec Druids Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#---X##----#BJBJBJBJB#----##----##JBJBJBJ##----##-----##BJBJB##-----##------#######BBBBB-##---------#BBBBBBBBB##---------#JJJJJJJJB###--------#BBBBBBBB#####-------#-------########################");
                remarkpol.push("Druids");
				notepol.push("236KTS 118,000 Druids @ 15 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,118000,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">14 sec Druids Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#---X##----#BJBJBJBJB#----##----##JBJBJBJ##----##-----##BJBJB##-----##------#######BBBBB-##---------#BBBBBBBBB##---------#JJJJJJJJB###--------#BBBBBBBB#####-------#-------########################");
                remarkpol.push("Druids");
				notepol.push("280KTS 140,000 Druids @ 22.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,140000,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">18 sec Druids Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##-BJBJB-##----##----#BBBJBJBBB#----##----#BJBJBBBJB#----#######BBBJTJBJB#######----#BJBBBBBBB#S--X##----#BBBJBJB--#M---##----##-BJBJB-##----##-----##BBBBB##BBB--##------#######BBJB--##---------#BBBBBJB--##---------#BJBJBBB--###--------#BJBJBJB-#####-------#BBBBBBB########################");
                remarkpol.push("Druids");
				notepol.push("300KTS 150,000 Druids @ 31.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,150000,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">252K 40 sec Scorpions Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BYBYB##-----##----##BBYBYBB##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#SS-X##----#BYBYBYBYB#----##----##BBYBYBB##----##-----##BYBYB##-----##------#######------##---------#-BBBBBB--##---------#BYBYBYBB-###--------#BYBYBYBB#####-------#BBBBBBB########################");
                remarkpol.push("Scorps");
				notepol.push("252KTS 25,200 Scorps @ 11.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,0,0,25200,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">284K 48 sec Scorpions Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##YBYBYBY##----##----#BBBYBYBBB#----##----#BYBBBBBYB#----#######BYBYTYBYB#######----#BYBBBBBYB#SS-X##----#BBBYBYBBB#----##----##YBYBYBY##----##-----##BBBBB##BBB--##------#######-BYB--##---------#--BBBYB--##---------#-BBYBYB--###--------#-BBYBYB-#####-------#-BBBBBB########################");
                remarkpol.push("Scorps");
				notepol.push("284KTS 28,400 Scorps @ 16 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,0,0,28400,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">256K 25 sec Rams Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BYBYB##-----##----##BBYBYBB##----##----#BYBYBYBYB#----##----#BYBYBYBYB#----#######BYBYTYBYB#######----#BYBYBYBYB#SS-X##----#BYBYBYBYB#----##----##BBYBYBB##----##-----##BBBYB##-----##------#######------##---------#-BBBBBB--##---------#BYBYBYBB-###--------#BYBYBYBB#####-------#BBBBBBB########################");
                remarkpol.push("Battering Rams");
				notepol.push("256KTS 25,600 Scorps @ 7.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,0,25600,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">284K 30 sec Rams Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##YBYBYBY##----##----#BBBYBYBBB#----##----#BYBBBBBYB#----#######BYBYTYBYB#######----#BYBBBBBYB#SS-X##----#BBBYBYBBB#----##----##YBYBYBY##----##-----##BBBBB##BBB--##------#######-BYB--##---------#--BBBYB--##---------#-BBYBYB--###--------#-BBYBYB-#####-------#-BBBBBB########################");
                remarkpol.push("Battering Rams");
				notepol.push("284KTS 28,400 Rams @ 10 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,0,0,0,28400,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">3/3 R/T Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##GBGBGBG##----##----#BBBGBGBBB#----##----#BGBGBGBGB#----#######BBBGTGBGB#######----#BGBGBGBGB#S-PX##----#BBBGBGBBB#S-P-##----##GBGBGBG##----##-----##BBBBB##-----##------#######BBBB--##---------#--BGBGBB-##---------#--BBBGBJ-###--------#--BGBGBB#####-------#--BBBBB########################");
                remarkpol.push("R/T");
				notepol.push("264KTS 132000 Ranger, 132000 Triari @ 9 days");
                troopcounpol.push([0,0,132000,132000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">3/4 R/T Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##GBGBGBG##----##----#BBBGBGBBB#----##----#BGBBBBBGB#----#######BBBGTGBBB#######----#BGBBBBBGB#SSPJ##----#BBBGBGBBB#MLPX##----##GBGBGBG##----##-----##BBBBB##-----##------#######------##---------#BBBBBB---##---------#BGGGBB---###--------#BGGBBB--#####-------#BBBBBB-########################");
                remarkpol.push("R/T");
				notepol.push("276KTS 138000 Ranger, 138000 Triari @ 11 days");
                troopcounpol.push([0,0,138000,138000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">4/4 R/T Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGB-##----##----#BBBGBGBBB#----##----#BGBBBBBGB#----#######BGBBTBBBB#######----#BGBGBGBGB#SSPJ##----#BBBGBGBBB#MLPX##----##-BGBGB-##----##-----##BBBBB##-----##------#######-BBB--##---------#----BGB--##---------#BBBBBBB--###--------#BGGGGGB-#####-------#BBBBBBB########################");
                remarkpol.push("R/T");
				notepol.push("284KTS 142000 Ranger, 142000 Triari @ 13 days");
                troopcounpol.push([0,0,142000,142000,0,0,0,0,0,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">Scouts Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBEBEBEB#----#######BEBETEBEB#######----#BEBEBEBEB#SMJX##----#BEBEBEBEB#S---##----##BBEBEBB##----##-----##BBBBB##BBBBB##------#######-BEBEB##---------#----BBBBB##---------#----BEBEB###--------#----BBBB#####-------#----BBB########################");
                remarkpol.push("Scouts");
				notepol.push("268KTS 134000 Scouts @ 4.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,134000,0,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">6 sec Arbs Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BEBEBEB##----##----#-BEBEBEB-#----##----#-BEBEBEB-#----#######-BEBTBEB-#######----#-BEBEBEB-#SSPJ##----#-BEBEBEB-#MLPX##----##BEBEBEB##S---##-----##BBBBB##BBBB-##------#######-EEEE-##---------#BBBBBBBBB##---------#BEEEEEEEB###--------#-BBBBBBB#####-------#-------########################");
                remarkpol.push("Arbs");
				notepol.push("240KTS 120,000 Arbs @ 8.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,120000,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">7 sec Arbs Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBEBEBB##----##----#BEBEBEBEB#----##----#BEBBBBBEB#----#######BEBETEBEB#######----#BEBBBBBEB#SMPX##----#BEBEBEBEB#SDPJ##----##BBEBEBB##----##-----##BBBBB##-----##------#######BBBBB-##---------#---BEBEB-##---------#---BBBBB-###--------#---BEBEB#####-------#---BBBB########################");
                remarkpol.push("Arbs");
				notepol.push("260KTS 130,000 Arbs @ 10.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,130000,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">8 sec Arbs Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##-BEBEB-##----##----#BBBEBEBBB#----##----#BEBBBBBEB#----#######BEBETEBEB#######----#BEBBBBBEB#SSPJ##----#BBBEBEBBB#MLPX##----##-BEBEB-##----##-----##BBBBB##-----##------#######------##---------#BBBBBBB--##---------#BEBEBEB--###--------#BEBEBEB-#####-------#BBBBBBB########################");
                remarkpol.push("Arbs");
				notepol.push("280KTS 140,000 Arbs @ 13 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,140000,0,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                pol++;

                selectbuttportal+='<option value="'+pol+'">8 sec Praetor Portal</option>';
                layoutpol.push("[ShareString.1.3]:########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BZBBBBBZB#----#######BZBZTZBZB#######----#BZBBBBBZB#SPJX##----#BZBZBZBZB#SP--##----##BBZBZBB##----##-----##BBBBB##-----##------#######BBBBBB##---------#---BZZZZB##---------#---BBBBBB###--------#---BZZB-#####-------#---BBBB########################");
                remarkpol.push("Praetor");
				notepol.push("272KTS 136,000 Praetor @ 12.5 days");
                troopcounpol.push([0,0,0,0,0,0,0,0,0,136000,0,0,0,0,0,0,0]);
                respol.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                selectbuttportal+='</select>';

				var selectbutttroopscoutg='<select id="troopscoutgalleylayouts" style="font-size: 10px !important;margin-top:1%;margin-left:2%;width:45%;" class="regButton greenb"><option value="0">Troop Scout Galley Layouts</option>';
                var tsg=1;
				selectbutttroopscoutg+='<option value="'+tsg+'">3 sec vanq/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BGBGBGBGB#----##----#BGBBBBBGB#----#######BGBGTGBGB#######----#BGBBBBBGB#----##----#BGBGBGBGB#BBJ-##----##BBBBBBB##BBB-##-----##-----##BBBBZ##------#######BBVVBB##---------#SS-BVTTVB##---------#M--BVTTTV###--------#PP-BBVTT#####-------#X--EBBV########################");
                remarktsg.push("Vanqs");
				notetsg.push("260KTS Default for 10 Fakes - 191500 Vanqs, 15 Scouts, 684 galleys"+ "\n" + "for 4 fakes - 206500 Vanqs, 15 scouts, 533 galleys"+ "\n" + "for 5 fakes - 204000 Vanqs, 15 scouts, 558 galleys"+ "\n" + "for 6 fakes - 201500 Vanqs, 15 scouts, 583 galleys");
                troopcountsg.push([0,0,0,0,0,191500,0,15,0,0,0,0,0,0,684,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">4 sec vanq/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBGB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##BBGBGB-##----##-----##BBBBB##BBBBE##------#######BBVVBB##---------#SS-BV##VB##---------#DM-BV###V###--------#SP-BBV#######-------#XP-ZBBV########################");
                remarktsg.push("Vanqs");
				notetsg.push("276KTS Default for 10 Fakes - 204500 Vanqs, 15 Scouts, 709 galleys"+ "\n" + "for 4 fakes - 219500 Vanqs, 15 scouts, 560 galleys"+ "\n" + "for 5 fakes - 217000 Vanqs, 15 scouts, 585 galleys"+ "\n" + "for 6 fakes - 214500 Vanqs, 15 scouts, 610 galleys");
                troopcountsg.push([0,0,0,0,0,204500,0,15,0,0,0,0,0,0,709,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">6 sec Sorc/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BJBJB##-----##----##JBJBJBJ##----##----#BJBJBJBJB#----##----#BJBJBJBJB#----#######BJBJTJBJB#######----#BJBJBJBJB#----##----#BJBJBJBJB#----##----##JBJBJBJ##BJ--##-----##BJBJB##BBBBE##------#######BBVVBB##---------#SS-BVTTVB##---------#M--BVTTTV###--------#P--BBVTT#####-------#X--ZBBV########################");
                remarktsg.push("Sorcs");
				notetsg.push("208KTS Default for 10 Fakes - 148200 Sorcs, 15 Scouts, 597 galleys"+ "\n" + "for 4 fakes - 163200 Sorcs, 15 scouts, 447 galleys"+ "\n" + "for 5 fakes - 160700 Sorcs, 15 scouts, 472 galleys"+ "\n" + "for 6 fakes - 158200 Sorcs, 15 scouts, 497 galleys");
                troopcountsg.push([0,0,0,0,0,0,148200,15,0,0,0,0,0,0,597,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">7 sec Sorc/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBJBB##-----##----##BJBJBJB##----##----#-BJBJBJB-#----##----#-BJBJBJB-#----#######-BJBTBJB-#######----#-BJBJBJB-#---X##----#-BJBJBJB-#BBB-##----##BJBJBJB##JJJE##-----##BBJBB##BBBBB##------#######BBVVBB##---------#-BJBVTTVB##---------#PBJBVTTTV###--------#M-BBBVTT#####-------#SS-ZBBV########################");
                remarktsg.push("Sorcs");
				notetsg.push("232KTS Default for 10 Fakes - 168000 Sorcs, 15 Scouts, 637 galleys"+ "\n" + "for 4 fakes - 183000 Sorcs, 15 scouts, 487 galleys"+ "\n" + "for 5 fakes - 180500 Sorcs, 15 scouts, 512 galleys"+ "\n" + "for 6 fakes - 178000 Sorcs, 15 scouts, 537 galleys");
                troopcountsg.push([0,0,0,0,0,0,168000,15,0,0,0,0,0,0,637,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">7 sec Horse/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BEBEBEB##----##----#-BEBEBEB-#----##----#-BEBEBEB-#----#######-BEBTBEB-#######----#-BEBEBEB-#BBZ-##----#-BEBEBEB-#BBBB##----##BEBEBEB##BEBB##-----##BBEBB##BBBBB##------#######BBVVBB##---------#SS-BVTTVB##---------#M--BVTTTV###--------#PP-BBVTT#####-------#X--JBBV########################");
                remarktsg.push("Horses");
				notetsg.push("248KTS Default for 10 Fakes - 90750 Horses, 15 Scouts, 664 galleys"+ "\n" + "for 4 fakes - 98000 Horses, 15 scouts, 513 galleys"+ "\n" + "for 5 fakes - 97000 Horses, 15 scouts, 539 galleys"+ "\n" + "for 6 fakes - 95750 Sorcs, 15 scouts, 564 galleys");
                troopcountsg.push([0,0,0,0,0,0,0,15,0,0,90750,0,0,0,664,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">Warship/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##-----##-----##----##-------##----##----#---------#----##----#---------#----#######-SS-TP---#######-XBB#-ML--P---#BBBB##-JBB#-S---P---#BBBB##--BB##-------##BBBB##--BBB##-----##BBBBB##--BBBB#######BBVVBB##--BBBBBBB#BBBBVTTVB##--BEBZBBB#BBBBVTTTV###-BBBBBBB#BBBBBVTT#####BBBBBBB#BBBBBBV########################");
                remarktsg.push("Warships");
				notetsg.push("320KTS Default for 10 Fakes - 724 Warships, 15 Scouts, 301 galleys"+ "\n" + "for 4 fakes - 769 Warships, 15 scouts, 121 galleys"+ "\n" + "for 5 fakes - 762 Warships, 15 scouts, 151 galleys"+ "\n" + "for 6 fakes - 754 Warships, 15 scouts, 181 galleys");
                troopcountsg.push([0,0,0,0,0,0,0,15,0,0,0,0,0,0,301,0,724]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">3 sec Scout/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BEBEBEB##----##----#-BEBEBEB-#----##----#-BEBEBEB-#----#######-BEBTBEB-#######----#-BEBEBEB-#BBZ-##----#-BEBEBEB-#BBBB##----##BEBEBEB##BEBB##-----##BBEBB##BBBBB##------#######BBVVBB##---------#SS-BVTTVB##---------#M--BVTTTV###--------#PP-BBVTT#####-------#X--JBBV########################");
                remarktsg.push("Scouts");
				notetsg.push("224KTS 93300 Scouts, 374 galleys");
                troopcountsg.push([0,0,0,0,0,0,0,93300,0,0,0,0,0,0,374,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
                tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">3 sec ranger/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBGBGBB##----##----#BBBBBBBBB#----##----#BGBGBGBGB#----#######BBBBTBBGB#######----#BGBGBGBGB#----##----#BBBBBBBBB#----##----##BBGBGB-##----##-----##BBBBB##BBBBE##------#######BBVVBB##---------#SS-BV##VB##---------#DM-BV###V###--------#SP-BBV#######-------#XP-ZBBV########################");
                remarktsg.push("Rangers");
				notetsg.push("232KTS Default for 10 Fakes - 168000 Rangers, 15 Scouts, 637 galleys"+ "\n" + "for 4 fakes - 183000 Vanqs, 15 scouts, 487 galleys"+ "\n" + "for 5 fakes - 180500 Vanqs, 15 scouts, 513 galleys"+ "\n" + "for 6 fakes - 178000 Vanqs, 15 scouts, 538 galleys");
                troopcountsg.push([0,0,168000,0,0,0,0,15,0,0,0,0,0,0,637,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
               	tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">4 sec priest/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BZBZB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BZBZBZBZB#----#######BZBZTZBZB#######----#BZBZBZBZB#SMPX##----#BZBZBZBZB#SDPJ##----##BBZBZBB##----##-----##BZBBB##BBBB-##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#---BBBV########################");
                remarktsg.push("Priest");
				notetsg.push("228KTS Default for 10 Fakes - 164969 Priests, 15 Scouts, 630 galleys"+ "\n" + "for 4 fakes - 179500 priests, 15 scouts, 481 galleys"+ "\n" + "for 5 fakes - 177469 priests, 15 scouts, 505 galleys"+ "\n" + "for 6 fakes - 174869 Vanqs, 15 scouts, 531 galleys");
                troopcountsg.push([0,0,0,0,164969,0,0,15,0,0,0,0,0,0,630,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
               	tsg++;

                selectbutttroopscoutg+='<option value="'+tsg+'">5 sec priest/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBZBZBB##----##----#BZBZBZBZB#----##----#BBBZBZBBB#----#######BZBZTZBZB#######----#BBBZBZBBB#SMPX##----#BZBZBZBZB#SDPJ##----##BBZBZBB##----##-----##BZBBB##BBBB-##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#---BBBV########################");
                remarktsg.push("Priest");
				notetsg.push("252KTS Default for 10 Fakes - 184969 Priests, 15 Scouts, 670 galleys"+ "\n" + "for 4 fakes - 199969 priests, 15 scouts, 520 galleys"+ "\n" + "for 5 fakes - 197469 priests, 15 scouts, 545 galleys"+ "\n" + "for 6 fakes - 194969 Vanqs, 15 scouts, 570 galleys");
                troopcountsg.push([0,0,0,0,184969,0,0,15,0,0,0,0,0,0,670,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
               	tsg++;

				 selectbutttroopscoutg+='<option value="'+tsg+'">6 sec priest/galley</option>';
                layouttsg.push("[ShareString.1.3];########################-------#-------#####--------#--------###---------#---------##---------#---------##------#######------##-----##BBBBB##-----##----##BBZBZBB##----##----#BZBBBBBZB#----##----#BBBZBZBBB#----#######BZBBTBBZB#######----#BZBZBZBBB#SMPX##----#BZBZBZBZB#SDP-##----##BBZBZBB##----##-----##BBBBB##BBBBE##------#######BBVVBB##---------#---BV##VB##---------#---BV###V###--------#---BBV#######-------#---JBBV########################");
                remarktsg.push("Priest");
				notetsg.push("264KTS Default for 10 Fakes - 194969 Priests, 15 Scouts, 690 galleys"+ "\n" + "for 4 fakes - 209969 priests, 15 scouts, 540 galleys"+ "\n" + "for 5 fakes - 207469 priests, 15 scouts, 565 galleys"+ "\n" + "for 6 fakes - 204969 Vanqs, 15 scouts, 590 galleys");
                troopcountsg.push([0,0,0,0,194969,0,0,15,0,0,0,0,0,0,690,0,0]);
                restsg.push([0,0,0,0,1,250000,250000,200000,350000,0,0,0,0,1,0,0,0,0,0,250000,250000,200000,400000]);
               	selectbutttroopscoutg+='</select>';

				$('#clearresquad').after(selectbuttlandoff);

				$('#landoffenselayouts').after(selectbuttlanddef);
                $('#landdefenselayouts').after(selectbuttwateroff);

				$('#wateroffenselayouts').after(selectbuttwaterdef);
                $('#waterdefenselayouts').after(selectbutthub);
				$('#hublayouts').after(selectbuttshipper);
                $('#shipperlayouts').after(selectbuttportal);
				$('#portallayouts').after(selectbutttroopscoutg);

				//Land Offense Builds
                $('#landoffenselayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layoutlol.length; j++) {

                        if ($('#landoffenselayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layoutlol[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                    //currentlayout=currentlayout.replaceAt(i,tmpchar);
                                }
                            }
                            //$('#removeoverlayGo').click();
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},3000);
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcounlol[j]) {
                                    aa[9+Number(k)]=troopcounlol[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                reslol[j][14]=nearesthub;
                                reslol[j][15]=nearesthub;
                            } else {
                                reslol[j][14]=0;
                                reslol[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                reslol[j][5]=$("#woodin").val();
                                reslol[j][6]=$("#stonein").val();
                                reslol[j][7]=$("#ironin").val();
                                reslol[j][8]=$("#foodin").val();
                                reslol[j][19]=$("#woodin").val();
                                reslol[j][20]=$("#stonein").val();
                                reslol[j][21]=$("#ironin").val();
                                reslol[j][22]=$("#foodin").val();
                                for (let k in reslol[j]) {
                                    aa[28+Number(k)]=reslol[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }

                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});

                        }
                    }
                });
				//Land Defense Builds
				$('#landdefenselayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layoutldl.length; j++) {

                        if ($('#landdefenselayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layoutldl[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                    //currentlayout=currentlayout.replaceAt(i,tmpchar);
                                }
                            }
                            //$('#removeoverlayGo').click();
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},3000);
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcounldl[j]) {
                                    aa[9+Number(k)]=troopcounldl[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                resldl[j][14]=nearesthub;
                                resldl[j][15]=nearesthub;
                            } else {
                                resldl[j][14]=0;
                                resldl[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                resldl[j][5]=$("#woodin").val();
                                resldl[j][6]=$("#stonein").val();
                                resldl[j][7]=$("#ironin").val();
                                resldl[j][8]=$("#foodin").val();
                                resldl[j][19]=$("#woodin").val();
                                resldl[j][20]=$("#stonein").val();
                                resldl[j][21]=$("#ironin").val();
                                resldl[j][22]=$("#foodin").val();
                                for (let k in resldl[j]) {
                                    aa[28+Number(k)]=resldl[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }

                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});

                        }
                    }
                });


		//Water Offense  Layouts
                $('#wateroffenselayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layoutwol.length; j++) {
                        if ($('#wateroffenselayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layoutwol[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                }
                            }
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},300);
                           
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcounwol[j]) {
                                    aa[9+Number(k)]=troopcounwol[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                reswol[j][14]=nearesthub;
                                reswol[j][15]=nearesthub;
                            } else {
                                reswol[j][14]=0;
                                reswol[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                reswol[j][5]=$("#woodin").val();
                                reswol[j][6]=$("#stonein").val();
                                reswol[j][7]=$("#ironin").val();
                                reswol[j][8]=$("#foodin").val();
                                reswol[j][19]=$("#woodin").val();
                                reswol[j][20]=$("#stonein").val();
                                reswol[j][21]=$("#ironin").val();
                                reswol[j][22]=$("#foodin").val();
                                for (let k in resw[j]) {
                                    aa[28+Number(k)]=reswol[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }
                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});
                        }
                    }
                });
				//Water Defense Layouts
				$('#waterdefenselayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layoutwdl.length; j++) {
                        if ($('#waterdefenselayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layoutwdl[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                }
                            }
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},300);
                           

                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcounwdl[j]) {
                                    aa[9+Number(k)]=troopcounwdl[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                reswdl[j][14]=nearesthub;
                                reswdl[j][15]=nearesthub;
                            } else {
                                reswdl[j][14]=0;
                                reswdl[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                reswdl[j][5]=$("#woodin").val();
                                reswdl[j][6]=$("#stonein").val();
                                reswdl[j][7]=$("#ironin").val();
                                reswdl[j][8]=$("#foodin").val();
                                reswdl[j][19]=$("#woodin").val();
                                reswdl[j][20]=$("#stonein").val();
                                reswdl[j][21]=$("#ironin").val();
                                reswdl[j][22]=$("#foodin").val();
                                for (let k in reswdl[j]) {
                                    aa[28+Number(k)]=reswdl[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }
                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});
                        }
                    }
                });
				//Hub Layouts
				$('#hublayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layouthul.length; j++) {

                        if ($('#hublayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layouthul[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                    //currentlayout=currentlayout.replaceAt(i,tmpchar);
                                }
                            }
                            //$('#removeoverlayGo').click();
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},3000);
                            
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcounhul[j]) {
                                    aa[9+Number(k)]=troopcounhul[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                reshul[j][14]=nearesthub;
                                reshul[j][15]=nearesthub;
                            } else {
                                reshul[j][14]=0;
                                reshul[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                reshul[j][5]=$("#woodin").val();
                                reshul[j][6]=$("#stonein").val();
                                reshul[j][7]=$("#ironin").val();
                                reshul[j][8]=$("#foodin").val();
                                reshul[j][19]=$("#woodin").val();
                                reshul[j][20]=$("#stonein").val();
                                reshul[j][21]=$("#ironin").val();
                                reshul[j][22]=$("#foodin").val();
                                for (let k in reshul[j]) {
                                    aa[28+Number(k)]=reshul[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }

                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});

                        }
                    }
                });
				$('#shipperlayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layoutshl.length; j++) {

                        if ($('#shipperlayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layoutshl[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                    //currentlayout=currentlayout.replaceAt(i,tmpchar);
                                }
                            }
                            //$('#removeoverlayGo').click();
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},3000);
                            
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcounshl[j]) {
                                    aa[9+Number(k)]=troopcounshl[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                resshl[j][14]=nearesthub;
                                resshl[j][15]=nearesthub;
                            } else {
                                resshl[j][14]=0;
                                resshl[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                resshl[j][5]=$("#woodin").val();
                                resshl[j][6]=$("#stonein").val();
                                resshl[j][7]=$("#ironin").val();
                                resshl[j][8]=$("#foodin").val();
                                resshl[j][19]=$("#woodin").val();
                                resshl[j][20]=$("#stonein").val();
                                resshl[j][21]=$("#ironin").val();
                                resshl[j][22]=$("#foodin").val();
                                for (let k in resshl[j]) {
                                    aa[28+Number(k)]=resshl[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }

                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});

                        }
                    }
                });
				$('#portallayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layoutpol.length; j++) {

                        if ($('#portallayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layoutpol[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                    //currentlayout=currentlayout.replaceAt(i,tmpchar);
                                }
                            }
                            //$('#removeoverlayGo').click();
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},3000);
                            
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcounpol[j]) {
                                    aa[9+Number(k)]=troopcounpol[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                respol[j][14]=nearesthub;
                                respol[j][15]=nearesthub;
                            } else {
                                respol[j][14]=0;
                                respol[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                respol[j][5]=$("#woodin").val();
                                respol[j][6]=$("#stonein").val();
                                respol[j][7]=$("#ironin").val();
                                respol[j][8]=$("#foodin").val();
                                respol[j][19]=$("#woodin").val();
                                respol[j][20]=$("#stonein").val();
                                respol[j][21]=$("#ironin").val();
                                respol[j][22]=$("#foodin").val();
                                for (let k in respol[j]) {
                                    aa[28+Number(k)]=respol[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }

                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});

                        }
                    }
                });
				$('#troopscoutgalleylayouts').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layouttsg.length; j++) {

                        if ($('#troopscoutgalleylayouts').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layouttsg[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                    //currentlayout=currentlayout.replaceAt(i,tmpchar);
                                }
                            }
                            //$('#removeoverlayGo').click();
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},3000);
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcountsg[j]) {
                                    aa[9+Number(k)]=troopcountsg[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                restsg[j][14]=nearesthub;
                                restsg[j][15]=nearesthub;
                            } else {
                                restsg[j][14]=0;
                                restsg[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                restsg[j][5]=$("#woodin").val();
                                restsg[j][6]=$("#stonein").val();
                                restsg[j][7]=$("#ironin").val();
                                restsg[j][8]=$("#foodin").val();
                                restsg[j][19]=$("#woodin").val();
                                restsg[j][20]=$("#stonein").val();
                                restsg[j][21]=$("#ironin").val();
                                restsg[j][22]=$("#foodin").val();
                                for (let k in restsg[j]) {
                                    aa[28+Number(k)]=restsg[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }

                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});

                        }
                    }
                });
		//Fast Build Layouts
                $('#fastbuildlayout').change(function() {
                    var newlayout=currentlayout;
                    for (let j=1; j<layoutdf.length; j++) {
                        if ($('#fastbuildlayout').val()==j) {
                            for (let i=20; i<currentlayout.length;i++) {
                                var tmpchar=layoutdf[j].charAt(i);
                                var cmp=new RegExp(tmpchar);
                                if (!(cmp.test(emptyspots))) {
                                    newlayout=newlayout.replaceAt(i,tmpchar);
                                }
                            }
                            //$('#removeoverlayGo').click();
                            $('#overlaytextarea').val(newlayout);
                            setTimeout(function(){$("#applyoverlayGo")[0].click();},300);
                            var aa=city.mo;
                            if ($("#addtroops").prop("checked")==true) {
                                for (let k in troopcound[j]) {
                                    aa[9+Number(k)]=troopcound[j][k];
                                }
                            }
                            if ($("#addwalls").prop("checked")==true) {
                                aa[26]=1;
                            }
                            if ($("#addtowers").prop("checked")==true) {
                                aa[27]=1;
                            }
                            if ($("#addhub").prop("checked")==true) {
                                var hubs={cid:[],distance:[]};
                                $.each(ppdt.clc, function(key, value) {
                                    if (key==$("#selHub").val()) {
                                        hubs.cid=value;
                                    }
                                });
                                for (let i in hubs.cid) {
                                    var tempx=Number(hubs.cid[i] % 65536);
                                    var tempy=Number((hubs.cid[i]-tempx)/65536);
                                    hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
                                }
                                var mindist = Math.min.apply(Math, hubs.distance);
                                var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
                                resd[j][14]=nearesthub;
                                resd[j][15]=nearesthub;
                            } else {
                                resd[j][14]=0;
                                resd[j][15]=0;
                            }
                            if ($("#addres").prop("checked")==true) {
                                resd[j][5]=$("#woodin").val();
                                resd[j][6]=$("#stonein").val();
                                resd[j][7]=$("#ironin").val();
                                resd[j][8]=$("#foodin").val();
                                for (let k in resd[j]) {
                                    aa[28+Number(k)]=resd[j][k];
                                }
                            }
                            if ($("#addbuildings").prop("checked")==true) {
                                aa[51]=[1,$("#cablev").val()];
                                aa[1]=1;
                            }
                           //var aaa=JSON.stringify(aa);
                            var dat={a:JSON.stringify(aa),b:city.cid};
                            $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});
                        }
                    }
                });
            },500);
        });
    });









//setting nearest hub to a city
    function setnearhub() {
        var res=[0,0,0,0,1,130000,130000,0,0,0,0,0,0,1,0,0,0,0,0,300000,300000,300000,400000];
        var aa=city.mo;
        var hubs={cid:[],distance:[]};
        $.each(ppdt.clc, function(key, value) {
            if (key==$("#selHub").val()) {
                hubs.cid=value;
            }
        });
        for (let i in hubs.cid) {
            var tempx=Number(hubs.cid[i] % 65536);
            var tempy=Number((hubs.cid[i]-tempx)/65536);
            hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
        }
        var mindist = Math.min.apply(Math, hubs.distance);
        var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
        if ($("#addwalls").prop("checked")==true) {
            aa[26]=1;
        }
        if ($("#addtowers").prop("checked")==true) {
            aa[27]=1;
        }
        if ($("#addbuildings").prop("checked")==true) {
            aa[51]=[1,$("#cablev").val()];
            aa[68]=[1,10];
            aa[69]=[1,10];
            aa[70]=[1,10];
            aa[71]=[1,10];
            aa[1]=1;
        }
        res[14]=nearesthub;
        res[15]=nearesthub;
        res[5]=$("#woodin").val();
        res[6]=$("#stonein").val();
        res[7]=$("#ironin").val();
        res[8]=$("#foodin").val();
        for (let k in res) {
            aa[28+Number(k)]=res[k];
        }
        var dat={a:JSON.stringify(aa),b:city.cid};
        $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});
    }








//infantry setup
    function setinfantry() {
        var res=[0,0,0,0,1,200000,220000,200000,350000,0,0,0,0,1,0,0,0,0,0,300000,300000,300000,400000];
        var aa=city.mo;
        var hubs={cid:[],distance:[]};
        $.each(ppdt.clc, function(key, value) {
            if (key==$("#selHub").val()) {
                hubs.cid=value;
            }
        });
        for (let i in hubs.cid) {
            var tempx=Number(hubs.cid[i] % 65536);
            var tempy=Number((hubs.cid[i]-tempx)/65536);
            hubs.distance.push(Math.sqrt((tempx-city.x)*(tempx-city.x)+(tempy-city.y)*(tempy-city.y)));
        }
        var mindist = Math.min.apply(Math, hubs.distance);
        var nearesthub=hubs.cid[hubs.distance.indexOf(mindist)];
        if ($("#addwalls").prop("checked")==true) {
            aa[26]=1;
        }
        if ($("#addtowers").prop("checked")==true) {
            aa[27]=1;
        }
        if ($("#addbuildings").prop("checked")==true) {
            aa[51]=[1,$("#cablev").val()];
            aa[60]=[1,10];
            aa[62]=[1,10];
            aa[68]=[1,10];
            aa[69]=[1,10];
            aa[70]=[1,10];
            aa[71]=[1,10];
            aa[73]=[1,10];
            aa[1]=1;
        }
        res[14]=nearesthub;
        res[15]=nearesthub;
        res[5]=$("#woodin").val();
        res[6]=$("#stonein").val();
        res[7]=$("#ironin").val();
        res[8]=$("#foodin").val();
        for (let k in res) {
            aa[28+Number(k)]=res[k];
        }
        var dat={a:JSON.stringify(aa),b:city.cid};
        $.ajax({url: 'includes/mnio.php',type: 'POST',async:true,data: dat});
    }


