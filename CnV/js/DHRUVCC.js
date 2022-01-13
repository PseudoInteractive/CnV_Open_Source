// ==UserScript==
// @name Cotg Combat Calculator
// @namespace https://github.com/Dkhub85/Cotg-Combat-Calculator
// @version 1.0.0
// @description Cotg Combat Calculator
// @author Dhruv
// @match https://*.crownofthegods.com
// @include https://*.crownofthegods.com/?s=*
// @grant none
// @updateURL https://raw.githubusercontent.com/DKhub85/Cotg-Combat-Calculator/master/COTG_CC.user.js
// @downloadURL https://raw.githubusercontent.com/DKhub85/Cotg-Combat-Calculator/master/COTG_CC.user.js
// ==/UserScript==

(function() {
    //    'use strict';
    var ttname=["Guards","Ballistas","Rangers","Triari","Priestess","Vanquishers","Sorcerers","Scouts","Arbalists","Praetors","Horsemans","Druids","Rams","Scorpions","Galleys","Stingers","Warships","senator"];
    var ttattack=[10,50,30,10,25,50,70,10,40,60,90,120,50,150,3000,1200,12000]; //troops attack value
    var ttinfdef=[10,200,40,30,20,15,15,10,40,50,40,30,20,100,4000,4500,5000];//infantry defence
    var ttcavdef=[10,100,10,50,30,12,10,10,90,20,30,20,20,100,4000,4500,5000];//cavalry defence
    var ttmystdef=[10,200,25,20,50,10,30,10,30,90,20,50,20,200,2000,2000,2500];//mystic defence
    var ttartdef=[10,400,15,15,15,15,15,10,40,40,40,40,50,50,2000,6000,6000];//artilery defence
    var ttinfdefz=[];
    var ttcavdefz=[];
    var ttmystdefz=[];
    var ttartdefz=[];
    var iscav=[0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0]; //which troop number is cav
    var isinf=[0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0]; //which troop number is inf
    var ismgc=[0,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0]; //which troop number is magic
    var isart=[0,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1]; //which troop number is artillery
    var sum=true;
    var attackerts=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
    var defenderts=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
    var zdefenderts=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
    var survivingdefTS=[];
    var dresearch=[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1];
    var aresearch=[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1];
    var tswalld=[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1];
    var lossdefTS=[];
    var inputnightp=1;
    var inputmorale=1;
    var inputportal=1;
    var infatk=0;
    var cavatk=0;
    var mystatk=0;
    var artatk=0;
    var sumofattk;
    var sumofdef;
    var percinfattk;
    var percavattk;
    var permystattk;
    var perartattk;
    var ratioinf;
    var ratiocav;
    var ratiomyst;
    var ratioart;
    var ratiosum;
    var attinflosses;
    var attcavlosses;
    var attmystlosses;
    var attartlosses;
    var definflosses;
    var defcavlosses;
    var defmystlosses;
    var defartlosses;
    var defloss;
    var survoffTS=[];
    var defintensity=5;
    var atkintenstity=5;
    var lossattTS=[];
    var survivingattTS=[];
    var norrang;
    var towerrang;
    var nortri;
    var towertri;
    var norpri;
    var towerpri;
    var norbali;
    var towerbali;
    var dkumar="<button class='regButton greenb' id='CoCa' style='margin-left: 355px;margin-top: 250px;width: 150px;height: 30px !important; font-size: 12px !important; position: absolute;'>Combat Calculator</button>";
    $("#warCounc").append(dkumar);
    $("#CoCa").click(function() {
        if(sum) {
            combcal();
        } else {
            document.getElementById("kumar").style.display = "block";
          
        }
    });
    function repeat() {
        var HasA = 0;
        for (var i=0;i<17;i++) {
            HasA = HasA + survivingattTS[i];
        }
        var HasD =0;
        for (var i=0;i<17;i++) {
            HasD = HasD + survivingdefTS[i];
        }
        if (HasA == 0) {
            alert("Atack force has already been obliterated!");
        }
        else if (HasD == 0) {
            alert("Defense force has already been obliterated!");
        }
        else {
            document.getElementById("vanqA").value = Math.floor(survivingattTS[5]);
            document.getElementById("sorcA").value = Math.floor(survivingattTS[6]);
            document.getElementById("rangA").value = Math.floor(survivingattTS[2]);
            document.getElementById("triA").value = Math.floor(survivingattTS[3]);
            document.getElementById("priA").value = Math.floor(survivingattTS[4]);
            document.getElementById("scoutA").value = Math.floor(survivingattTS[7]);
            document.getElementById("horseA").value = Math.floor(survivingattTS[10]);
            document.getElementById("druidA").value = Math.floor(survivingattTS[11]);
            document.getElementById("arbA").value = Math.floor(survivingattTS[8]);
            document.getElementById("praA").value = Math.floor(survivingattTS[9]);
            document.getElementById("ramA").value = Math.floor(survivingattTS[12]);
            document.getElementById("scorpA").value = Math.floor(survivingattTS[13]);
            document.getElementById("balisA").value = Math.floor(survivingattTS[1]);
            document.getElementById("galleyA").value = Math.floor(survivingattTS[14]);
            document.getElementById("stingA").value = Math.floor(survivingattTS[15]);
            document.getElementById("wgA").value = Math.floor(survivingattTS[16]);
            //Defenders
            document.getElementById("vanqD").value = Math.floor(survivingdefTS[5]);
            document.getElementById("cgD").value = Math.floor(survivingdefTS[0]);
            document.getElementById("sorcD").value = Math.floor(survivingdefTS[6]);
            document.getElementById("rangD").value = Math.floor(survivingdefTS[2]);
            document.getElementById("triD").value = Math.floor(survivingdefTS[3]);
            document.getElementById("priD").value = Math.floor(survivingdefTS[4]);
            document.getElementById("scoutD").value = Math.floor(survivingdefTS[7]);
            document.getElementById("horseD").value = Math.floor(survivingdefTS[10]);
            document.getElementById("druidD").value = Math.floor(survivingdefTS[11]);
            document.getElementById("arbD").value = Math.floor(survivingdefTS[8]);
            document.getElementById("praD").value = Math.floor(survivingdefTS[9]);
            document.getElementById("ramD").value = Math.floor(survivingdefTS[12]);
            document.getElementById("scorpD").value = Math.floor(survivingdefTS[13]);
            document.getElementById("balisD").value = Math.floor(survivingdefTS[1]);
            document.getElementById("galleyD").value = Math.floor(survivingdefTS[14]);
            document.getElementById("stingD").value = Math.floor(survivingdefTS[15]);
            document.getElementById("wgD").value = Math.floor(survivingdefTS[16]);
        }
        defenderTSF();
    }
    function reset(){
        document.getElementById("vanqA").value = 0;
        document.getElementById("sorcA").value = 0;
        document.getElementById("rangA").value = 0;
        document.getElementById("triA").value = 0;
        document.getElementById("priA").value = 0;
        document.getElementById("scoutA").value = 0;
        document.getElementById("horseA").value = 0;
        document.getElementById("druidA").value = 0;
        document.getElementById("arbA").value = 0;
        document.getElementById("praA").value = 0;
        document.getElementById("ramA").value = 0;
        document.getElementById("scorpA").value = 0;
        document.getElementById("balisA").value = 0;
        document.getElementById("galleyA").value = 0;
        document.getElementById("stingA").value = 0;
        document.getElementById("wgA").value = 0;
        //Defenders
        document.getElementById("vanqD").value = 0;
        document.getElementById("cgD").value = 0;
        document.getElementById("sorcD").value = 0;
        document.getElementById("rangD").value = 0;
        document.getElementById("triD").value = 0;
        document.getElementById("priD").value = 0;
        document.getElementById("scoutD").value = 0;
        document.getElementById("horseD").value = 0;
        document.getElementById("druidD").value = 0;
        document.getElementById("arbD").value = 0;
        document.getElementById("praD").value = 0;
        document.getElementById("ramD").value = 0;
        document.getElementById("scorpD").value = 0;
        document.getElementById("balisD").value = 0;
        document.getElementById("galleyD").value = 0;
        document.getElementById("stingD").value = 0;
        document.getElementById("wgD").value = 0;

        //research entries
        document.getElementById("vanqAR").value = 0;
        document.getElementById("sorcAR").value = 0;
        document.getElementById("rangAR").value = 0;
        document.getElementById("triAR").value = 0;
        document.getElementById("priAR").value = 0;
        document.getElementById("scoutAR").value = 0;
        document.getElementById("horseAR").value = 0;
        document.getElementById("druidAR").value = 0;
        document.getElementById("arbAR").value = 0;
        document.getElementById("praAR").value = 0;
        document.getElementById("ramAR").value = 0;
        document.getElementById("scorpAR").value = 0;
        document.getElementById("balisAR").value = 0;
        document.getElementById("galleyAR").value = 0;
        document.getElementById("stingAR").value = 0;
        document.getElementById("wgAR").value = 0;

        //Defenders
        document.getElementById("cgDR").value = 0;
        document.getElementById("vanqDR").value = 0;
        document.getElementById("sorcDR").value = 0;
        document.getElementById("rangDR").value = 0;
        document.getElementById("triDR").value = 0;
        document.getElementById("priDR").value = 0;
        document.getElementById("scoutDR").value = 0;
        document.getElementById("horseDR").value = 0;
        document.getElementById("druidDR").value = 0;
        document.getElementById("arbDR").value = 0;;
        document.getElementById("praDR").value = 0;
        document.getElementById("ramDR").value = 0;
        document.getElementById("scorpDR").value = 0;
        document.getElementById("balisDR").value = 0;
        document.getElementById("galleyDR").value = 0;
        document.getElementById("stingDR").value = 0;
        document.getElementById("wgDR").value =0;

        //modifiers

        document.getElementById("NightP").value = 0;
        document.getElementById("MGpen").value = 0;
        document.getElementById("Ascore").value =0;
        document.getElementById("Wall").value = 0;

        document.getElementById("rangt").value = 0;
        document.getElementById("trit").value = 0;
        document.getElementById("prit").value = 0;
        document.getElementById("balit").value = 0;

        defenderts=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
        zdefenderts=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
        attackerts=[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0];
        dresearch=[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1];
        aresearch=[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1];
        tswalld=[1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1];
        defenderTSF();
    }
    function defenderTSF(){
        defenderts[0] = parseInt( document.getElementById("cgD").value );
        if ( isNaN(defenderts[0])) {
            defenderts[0] = 0;
            document.getElementById("cgD").value = 0;
        }
        zdefenderts[0] = parseInt( document.getElementById("cgD").value );
        if ( isNaN(zdefenderts[0])) {
            zdefenderts[0] = 0;
        }
        //added balli tower
        norbali = parseInt( document.getElementById("balisD").value );
        if ( isNaN(norbali)) {
            norbali = 0;
            document.getElementById("balisD").value = 0;
        }
        towerbali = parseInt( document.getElementById("balit").value );
        if ( isNaN(towerbali)) {
            towerbali = 0;
            document.getElementById("balit").value = 0;
        }
        defenderts[1] = Number(norbali + Math.min(norbali,towerbali));
        zdefenderts[1] = parseInt( document.getElementById("balisD").value );
        if ( isNaN(zdefenderts[1])) {
            zdefenderts[1] = 0;
        }
        //added tower rang
        norrang = parseInt( document.getElementById("rangD").value );
        if ( isNaN(norrang)) {
            norrang = 0;
            document.getElementById("rangD").value = 0;
        }
        towerrang = parseInt( document.getElementById("rangt").value );
        if ( isNaN(towerrang)) {
            towerrang = 0;
            document.getElementById("rangt").value = 0;
        }
        defenderts[2] = Number(norrang + Math.min(norrang,towerrang));
        zdefenderts[2] = parseInt( document.getElementById("rangD").value );
        if ( isNaN(zdefenderts[2])) {
            zdefenderts[2] = 0;
        }
        // added tower tri
        nortri = parseInt( document.getElementById("triD").value );
        if ( isNaN(nortri)) {
            nortri = 0;
            document.getElementById("triD").value = 0;
        }
        towertri = parseInt( document.getElementById("trit").value );
        if ( isNaN(towertri)) {
            towertri = 0;
            document.getElementById("trit").value = 0;
        }
        defenderts[3] = Number(nortri + Math.min(nortri,towertri));
        zdefenderts[3] = parseInt( document.getElementById("triD").value );
        if ( isNaN(zdefenderts[3])) {
            zdefenderts[3] = 0;
        }
        //added tower pri
        norpri = parseInt( document.getElementById("priD").value );
        if ( isNaN(norpri)) {
            norpri = 0;
            document.getElementById("priD").value = 0;
        }
        towerpri = parseInt( document.getElementById("prit").value );
        if ( isNaN(towerpri)) {
            towerpri = 0;
            document.getElementById("prit").value = 0;
        }
        defenderts[4] = Number(norpri + Math.min(norpri,towerpri));
        zdefenderts[4] = parseInt( document.getElementById("priD").value );
        if ( isNaN(zdefenderts[4])) {
            zdefenderts[4] = 0;
        }

        defenderts[5] = parseInt( document.getElementById("vanqD").value );
        if ( isNaN(defenderts[5])) {
            defenderts[5] = 0;
            document.getElementById("vanqD").value = 0;
        }
        zdefenderts[5] = parseInt( document.getElementById("vanqD").value );
        if ( isNaN(zdefenderts[5])) {
            zdefenderts[5] = 0;
        }

        defenderts[6] = parseInt( document.getElementById("sorcD").value );
        if ( isNaN(defenderts[6])) {
            defenderts[6] = 0;
            document.getElementById("sorcD").value = 0;
        }
        zdefenderts[6] = parseInt( document.getElementById("sorcD").value );
        if ( isNaN(zdefenderts[6])) {
            zdefenderts[6] = 0;
        }
        defenderts[7] = parseInt( document.getElementById("scoutD").value );
        if ( isNaN(defenderts[7])) {
            defenderts[7] = 0;
            document.getElementById("scoutD").value = 0;
        }
        zdefenderts[7] = parseInt( document.getElementById("scoutD").value );
        if ( isNaN(zdefenderts[7])) {
            zdefenderts[7] = 0;
        }
        defenderts[8] = parseInt( document.getElementById("arbD").value );
        if ( isNaN(defenderts[8])) {
            defenderts[8] = 0;
            document.getElementById("arbD").value = 0;
        }
        zdefenderts[8] = parseInt( document.getElementById("arbD").value );
        if ( isNaN(zdefenderts[8])) {
            zdefenderts[8] = 0;
        }
        defenderts[9] = parseInt( document.getElementById("praD").value );
        if ( isNaN(defenderts[9])) {
            defenderts[9] = 0;
            document.getElementById("praD").value = 0;
        }
        zdefenderts[9] = parseInt( document.getElementById("praD").value );
        if ( isNaN(zdefenderts[9])) {
            zdefenderts[9] = 0;
        }
        defenderts[10] = parseInt( document.getElementById("horseD").value );
        if ( isNaN(defenderts[10])) {
            defenderts[10] = 0;
            document.getElementById("horseD").value = 0;
        }
        zdefenderts[10] = parseInt( document.getElementById("horseD").value );
        if ( isNaN(zdefenderts[10])) {
            zdefenderts[10] = 0;
        }
        defenderts[11] = parseInt( document.getElementById("druidD").value );
        if ( isNaN(defenderts[11])) {
            defenderts[11] = 0;
            document.getElementById("druidD").value = 0;
        }
        zdefenderts[11] = parseInt( document.getElementById("druidD").value );
        if ( isNaN(zdefenderts[11])) {
            zdefenderts[11] = 0;
        }
        defenderts[12] = parseInt( document.getElementById("ramD").value );
        if ( isNaN(defenderts[12])) {
            defenderts[12] = 0;
            document.getElementById("ramD").value = 0;
        }
        zdefenderts[12] = parseInt( document.getElementById("ramD").value );
        if ( isNaN(zdefenderts[12])) {
            zdefenderts[12] = 0;
        }
        defenderts[13] = parseInt( document.getElementById("scorpD").value );
        if ( isNaN(defenderts[13])) {
            defenderts[13] = 0;
            document.getElementById("scorpD").value = 0;
        }
        zdefenderts[13] = parseInt( document.getElementById("scorpD").value );
        if ( isNaN(zdefenderts[13])) {
            zdefenderts[13] = 0;
        }
        defenderts[14] = parseInt( document.getElementById("galleyD").value );
        if ( isNaN(defenderts[14])) {
            defenderts[14] = 0;
            document.getElementById("galleyD").value = 0;
        }
        zdefenderts[14] = parseInt( document.getElementById("galleyD").value );
        if ( isNaN(zdefenderts[14])) {
            zdefenderts[14] = 0;
        }
        defenderts[15] = parseInt( document.getElementById("stingD").value );
        if ( isNaN(defenderts[15])) {
            defenderts[15] = 0;
            document.getElementById("stingD").value = 0;
        }
        zdefenderts[15] = parseInt( document.getElementById("stingD").value );
        if ( isNaN(zdefenderts[15])) {
            zdefenderts[15] = 0;
        }
        defenderts[16] = parseInt( document.getElementById("wgD").value );
        if ( isNaN(defenderts[16])) {
            defenderts[16] = 0;
            document.getElementById("wgD").value = 0;
        }
        zdefenderts[16] = parseInt( document.getElementById("wgD").value );
        if ( isNaN(zdefenderts[16])) {
            zdefenderts[16] = 0;
        }
        attackerts[0] = 0;
        attackerts[1] = parseInt( document.getElementById("balisA").value );
        if ( isNaN(attackerts[1])) {
            attackerts[1] = 0;
            document.getElementById("balisA").value = 0;
        }
        attackerts[2] = parseInt( document.getElementById("rangA").value );
        if ( isNaN(attackerts[2])) {
            attackerts[2] = 0;
            document.getElementById("rangA").value = 0;
        }
        attackerts[3] = parseInt( document.getElementById("triA").value );
        if ( isNaN(attackerts[3])) {
            attackerts[3] = 0;
            document.getElementById("triA").value = 0;
        }
        attackerts[4] = parseInt( document.getElementById("priA").value );
        if ( isNaN(attackerts[4])) {
            attackerts[4] = 0;
            document.getElementById("priA").value = 0;
        }
        attackerts[5] = parseInt( document.getElementById("vanqA").value );
        if ( isNaN(attackerts[5])) {
            attackerts[5] = 0;
            document.getElementById("vanqA").value = 0;
        }
        attackerts[6] = parseInt( document.getElementById("sorcA").value );
        if ( isNaN(attackerts[6])) {
            attackerts[6] = 0;
            document.getElementById("sorcA").value = 0;
        }
        attackerts[7] = parseInt( document.getElementById("scoutA").value );
        if ( isNaN(attackerts[7])) {
            attackerts[7] = 0;
            document.getElementById("scoutA").value = 0;
        }
        attackerts[8] = parseInt( document.getElementById("arbA").value );
        if ( isNaN(attackerts[8])) {
            attackerts[8] = 0;
            document.getElementById("arbA").value = 0;
        }
        attackerts[9] = parseInt( document.getElementById("praA").value );
        if ( isNaN(attackerts[9])) {
            attackerts[9] = 0;
            document.getElementById("praA").value = 0;
        }
        attackerts[10] = parseInt( document.getElementById("horseA").value );
        if ( isNaN(attackerts[10])) {
            attackerts[10] = 0;
            document.getElementById("horseA").value = 0;
        }
        attackerts[11] = parseInt( document.getElementById("druidA").value );
        if ( isNaN(attackerts[11])) {
            attackerts[11] = 0;
            document.getElementById("druidA").value = 0;
        }
        attackerts[12] = parseInt( document.getElementById("ramA").value );
        if ( isNaN(attackerts[12])) {
            attackerts[12] = 0;
            document.getElementById("ramA").value = 0;
        }
        attackerts[13] = parseInt( document.getElementById("scorpA").value );
        if ( isNaN(attackerts[13])) {
            attackerts[13] = 0;
            document.getElementById("scorpA").value = 0;
        }
        attackerts[14] = parseInt( document.getElementById("galleyA").value );
        if ( isNaN(attackerts[14])) {
            attackerts[14] = 0;
            document.getElementById("galleyA").value = 0;
        }
        attackerts[15] = parseInt( document.getElementById("stingA").value );
        if ( isNaN(attackerts[15])) {
            attackerts[15] = 0;
            document.getElementById("stingA").value = 0;
        }
        attackerts[16] = parseInt( document.getElementById("wgA").value );
        if ( isNaN(attackerts[16])) {
            attackerts[16] = 0;
            document.getElementById("wgA").value = 0;
        }

        //Atackers
        aresearch[0]=1;

        var balisAR = document.getElementById("balisAR").value;
        if (balisAR > 100 || isNaN(balisAR) == 1) {
            balisAR = 0;
            document.getElementById("balisAR").value = 0;
        }
        var balisARz=1+balisAR/100;
        aresearch[1]=balisARz;

        var rangAR = document.getElementById("rangAR").value;
        if (rangAR > 100 || isNaN(rangAR) == 1) {
            rangAR = 0;
            document.getElementById("rangAR").value = 0;
        }
        var rangARz=1+rangAR/100;
        aresearch[2]=rangARz;

        var triAR = document.getElementById("triAR").value;
        if (triAR > 100 || isNaN(triAR) == 1) {
            triAR = 0;
            document.getElementById("triAR").value = 0;
        }
        var triARz=1+triAR/100;
        aresearch[3]=triARz;

        var priAR = document.getElementById("priAR").value;
        if (priAR > 100 || isNaN(priAR) == 1) {
            priAR = 0;
            document.getElementById("priAR").value = 0;
        }
        var priARz=1+priAR/100;
        aresearch[4]=priARz;

        var vanqAR = document.getElementById("vanqAR").value;
        if (vanqAR > 100 || isNaN(vanqAR) == 1) {
            vanqAR = 0;
            document.getElementById("vanqAR").value = 0;
        }
        var vanqARz=1+vanqAR/100;
        aresearch[5]=vanqARz;

        var sorcAR = document.getElementById("sorcAR").value;
        if (sorcAR > 100 || isNaN(sorcAR) == 1) {
            sorcAR = 0;
            document.getElementById("sorcAR").value = 0;
        }
        var sorcARz=1+sorcAR/100;
        aresearch[6]=sorcARz;

        var scoutAR = document.getElementById("scoutAR").value;
        if (scoutAR > 100 || isNaN(scoutAR) == 1) {
            scoutAR = 0;
            document.getElementById("scoutAR").value = 0;
        }
        var scoutARz=1+scoutAR/100;
        aresearch[7]=scoutARz;

        var arbAR = document.getElementById("arbAR").value;
        if (arbAR > 100 || isNaN(arbAR) == 1) {
            arbAR = 0;
            document.getElementById("arbAR").value = 0;
        }
        var arbARz=1+arbAR/100;
        aresearch[8]=arbARz;

        var praAR = document.getElementById("praAR").value;
        if (praAR > 100 || isNaN(praAR) == 1) {
            praAR = 0;
            document.getElementById("praAR").value = 0;
        }
        var praARz=1+praAR/100;
        aresearch[9]=praARz;

        var horseAR = document.getElementById("horseAR").value;
        if (horseAR > 100 || isNaN(horseAR) == 1) {
            horseAR = 0;
            document.getElementById("horseAR").value = 0;
        }
        var horseARz=1+horseAR/100;
        aresearch[10]=horseARz;

        var druidAR = document.getElementById("druidAR").value;
        if (druidAR > 100 || isNaN(druidAR) == 1) {
            druidAR = 0;
            document.getElementById("druidAR").value = 0;
        }
        var druidARz=1+druidAR/100;
        aresearch[11]=druidARz;

        var ramAR = document.getElementById("ramAR").value;
        if (ramAR > 100 || isNaN(ramAR) == 1) {
            ramAR = 0;
            document.getElementById("ramAR").value = 0;
        }
        var ramARz=1+ramAR/100;
        aresearch[12]=ramARz;

        var scorpAR = document.getElementById("scorpAR").value;
        if (scorpAR > 100 || isNaN(scorpAR) == 1) {
            scorpAR = 0;
            document.getElementById("scorpAR").value = 0;
        }
        var scorpARz=1+scorpAR/100;
        aresearch[13]=scorpARz;

        var galleyAR = document.getElementById("galleyAR").value;
        if (galleyAR > 100 || isNaN(galleyAR) == 1) {
            galleyAR = 0;
            document.getElementById("galleyAR").value = 0;
        }
        var galleyARz=1+galleyAR/100;
        aresearch[14]=galleyARz;

        var stingAR = document.getElementById("stingAR").value;
        if (stingAR > 100 || isNaN(stingAR) == 1) {
            stingAR = 0;
            document.getElementById("stingAR").value = 0;
        }
        var stingARz=1+stingAR/100;
        aresearch[15]=stingARz;

        var wgAR = document.getElementById("wgAR").value;
        if (wgAR > 100 || isNaN(wgAR) == 1) {
            wgAR = 0;
            document.getElementById("wgAR").value = 0;
        }
        var wgARz=1+wgAR/100;
        aresearch[16]=wgARz;

        //Defenders
        var cgDR = document.getElementById("cgDR").value;
        if (cgDR > 100 || isNaN(cgDR) == 1) {
            cgDR = 0;
            document.getElementById("cgDR").value = 0;
        }
        var cgDRz=1+cgDR/100;
        dresearch[0]=cgDRz;

        var balisDR = document.getElementById("balisDR").value;
        if (balisDR > 100 || isNaN(balisDR) == 1) {
            balisDR = 0;
            document.getElementById("balisDR").value = 0;
        }
        var balisDRz=1+balisDR/100;
        dresearch[1]=balisDRz;

        var rangDR = document.getElementById("rangDR").value;
        if (rangDR > 100 || isNaN(rangDR) == 1) {
            rangDR = 0;
            document.getElementById("rangDR").value = 0;
        }
        var rangDRz=1+rangDR/100;
        dresearch[2]=rangDRz;

        var triDR = document.getElementById("triDR").value;
        if (triDR > 100 || isNaN(triDR) == 1) {
            triDR = 0;
            document.getElementById("triDR").value = 0;
        }
        var triDRz=1+triDR/100;
        dresearch[3]=triDRz;

        var priDR = document.getElementById("priDR").value;
        if (priDR > 100 || isNaN(priDR) == 1) {
            priDR = 0;
            document.getElementById("priDR").value = 0;
        }
        var priDRz=1+priDR/100;
        dresearch[4]=priDRz;

        var vanqDR = document.getElementById("vanqDR").value;
        if (vanqDR > 100 || isNaN(vanqDR) == 1) {
            vanqDR = 0;
            document.getElementById("vanqDR").value = 0;
        }
        var vanqDRz=1+vanqDR/100;
        dresearch[5]=vanqDRz;

        var sorcDR = document.getElementById("sorcDR").value;
        if (sorcDR > 100 || isNaN(sorcDR) == 1) {
            sorcDR = 0;
            document.getElementById("sorcDR").value = 0;
        }
        var sorcDRz=1+sorcDR/100;
        dresearch[6]=sorcDRz;

        var scoutDR = document.getElementById("scoutDR").value;
        if (scoutDR > 100 || isNaN(scoutDR) == 1) {
            scoutDR = 0;
            document.getElementById("scoutDR").value = 0;
        }
        var scoutDRz=1+scoutDR/100;
        dresearch[7]=scoutDRz;

        var arbDR = document.getElementById("arbDR").value;
        if (arbDR > 100 || isNaN(arbDR) == 1) {
            arbDR = 0;
            document.getElementById("arbDR").value = 0;
        }
        var arbDRz=1+arbDR/100;
        dresearch[8]=arbDRz;

        var praDR = document.getElementById("praDR").value;
        if (praDR > 100 || isNaN(praDR) == 1) {
            praDR = 0;
            document.getElementById("praDR").value = 0;
        }
        var praDRz=1+praDR/100;
        dresearch[9]=praDRz;

        var horseDR = document.getElementById("horseDR").value;
        if (horseDR > 100 || isNaN(horseDR) == 1) {
            horseDR = 0;
            document.getElementById("horseDR").value = 0;
        }
        var horseDRz=1+horseDR/100;
        dresearch[10]=horseDRz;

        var druidDR = document.getElementById("druidDR").value;
        if (druidDR > 100 || isNaN(druidDR) == 1) {
            druidDR = 0;
            document.getElementById("druidDR").value = 0;
        }
        var druidDRz=1+druidDR/100;
        dresearch[11]=druidDRz;

        var ramDR = document.getElementById("ramDR").value;
        if (ramDR > 100 || isNaN(ramDR) == 1) {
            ramDR = 0;
            document.getElementById("ramDR").value = 0;
        }
        var ramDRz=1+ramDR/100;
        dresearch[12]=ramDRz;

        var scorpDR = document.getElementById("scorpDR").value;
        if (scorpDR > 100 || isNaN(scorpDR) == 1) {
            scorpDR = 0;
            document.getElementById("scorpDR").value = 0;
        }
        var scorpDRz=1+scorpDR/100;
        dresearch[13]=scorpDRz;

        var galleyDR = document.getElementById("galleyDR").value;
        if (galleyDR > 100 || isNaN(galleyDR) == 1) {
            galleyDR = 0;
            document.getElementById("galleyDR").value = 0;
        }
        var galleyDRz=1+galleyDR/100;
        dresearch[14]=galleyDRz;

        var stingDR = document.getElementById("stingDR").value;
        if (stingDR > 100 || isNaN(stingDR) == 1) {
            stingDR = 0;
            document.getElementById("stingDR").value = 0;
        }
        var stingDRz=1+stingDR/100;
        dresearch[15]=stingDRz;

        var wgDR = document.getElementById("wgDR").value;
        if (wgDR > 100 || isNaN(wgDR) == 1) {
            wgDR = 0;
            document.getElementById("wgDR").value = 0;
        }
        var wgDRz=1+wgDR/100;
        dresearch[16]=wgDRz;
        //wall
        var wall = parseInt( document.getElementById("Wall").value );
        if (wall > 10 || isNaN(wall) == 1) {
            wall = 0;
            document.getElementById("Wall").value = 0;
        }
        for(var i=0; i<17; i++) {
            if(i<14){
                tswalld[i]=wall*.05+1;
            }else{
                tswalld[i]=1
            }
        }
        //night protection
        var inputnp = parseInt( document.getElementById("NightP").value );
        inputnightp=Number(1-inputnp/100);
        //morale
        var inputmor = parseInt( document.getElementById("Ascore").value );
        inputmorale=Number(1-inputmor/100);
        //portal
        var inputport = parseInt( document.getElementById("MGpen").value );
        inputportal=Number(1-inputport/100);

        //input intensity, assult siege or plunder
        var tempintensity=document.getElementById("atackType").value;
        if(tempintensity==="assault"){
            defintensity=5;
            atkintenstity=5;
        }
        if(tempintensity==="siege"){
            defintensity=1;
            atkintenstity=1;
        }
        if(tempintensity==="plunder"){
            defintensity=0.1;
            atkintenstity=1;
        }
        calculationCC();
    }

    function combcal() {
        sum=false;
        var kumar = "<div id='kumar' style='width:538px;height:715px; display:block; left: 400px; z-index: 2000; background-color: #FFFFEB; -moz-border-radius: 10px;-webkit-border-radius: 10px;border-radius: 10px;border: 4px ridge #DAA520; position: relative; right: auto; bottom: auto; top: -90px;' class='popUpBox ui-draggable ui-resizable'><div id='popkumar' style='margin-top:0x;width: calc(100% + 4px);margin-left: -6px;margin-top: -4px;height: 32px;line-height: 32px;vertical-align: middle;border-radius: 10px;border: 4px ridge #DAA520;color: #fff;background: url(/images/background/bmarb.png); background-repeat: repeat;' class='popUpBar ui-draggable-handle'><span style='margin-left : 60px;'>Combat Calculator</span> <button id=\"sumX\" onclick=\"document.getElementById('kumar').style.display='none';\" class=\"xbutton greenb\"><div id=\"xbuttondiv\"><div><div id=\"centxbuttondiv\"></div></div></div></button></div><div class=\"popUpWindow\" style='height:100%'>";
        kumar+='<div id = "toptxt">NP:<input type="text" id="NightP" size="1" value="0">';
        kumar+='Portal: <input type="text" id="MGpen" size="1" value="0">';
        kumar+='Morale: <input type="text" id="Ascore" size="1" value="0">';
        kumar+='Type:<select name="atackType" id="atackType" onchange="inputIntensity()"><option selected value="assault">Assault</option><option value="siege">Siege</option><option value="plunder">Plunder</option></select>';
        kumar+='Ranger=<input type="text" id="rangt" size="1" value="0">Triari=<input type="text" id="trit" size="1" value="0">Priestess=<input type="text" id="prit" size="1" value="0">Balista=<input type="text" id="balit" size="1" value="0">';
        kumar+='<br>Walls lvl: <input type="text" id="Wall" size="1" value="0"></div>';

        kumar+='<div id="chart">';
        kumar+='<table id = "tab">';
        kumar+='<tr>';
        kumar+='<!--<th colspan="2">ATT</th>  <th> +% </th>   <th>  </th>  <th> +%  </th>   <th colspan="2">DEF</th>"cityguard"-->';
        kumar+='<th>left </th><th>ATT</th>  <th> +% </th>   <th>  </th>  <th> +%  </th>   <th>DEF</th> <th>left</th>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td></td><td>-</td><td>-</td>';
        kumar+='<td><div class="guard32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="cgDR" size="1" maxlength="4" value="0"></td>';
        kumar+='<td><input type="text" id="cgD" size="5" value="0" > </td><td><span id="survcgD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="survanqA" style="font-weight:bold;"></span> </td><td><input type="text" id="vanqA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="vanqAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="vanq32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="vanqDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="vanqD" size="5" value="0"> </td><td><span id="survanqD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="sursorcA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="sorcA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="sorcAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="sorc32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="sorcDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="sorcD" size="5" value="0"> </td><td> <span id="sursorcD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr class = "alt">';
        kumar+='<td><span id="surRangA" style="font-weight:bold;"></span> </td><td><input type="text" id="rangA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="rangAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="ranger32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="rangDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="rangD" size="5" value="0"> </td><td><span id="surRangD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="surtriA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="triA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="triAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="triari32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="triDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="triD" size="5" value="0"> </td><td> <span id="surtriD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr class = "alt">';
        kumar+='<td><span id="surpriA" style="font-weight:bold;"></span> </td><td><input type="text" id="priA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="priAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="priest32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="priDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="priD" size="5" value="0"> </td><td><span id="surpriD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="surScoutA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="scoutA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="scoutAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="scout32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="scoutDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="scoutD" size="5" value="0"> </td><td> <span id="surScoutD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr class = "alt">';
        kumar+='<td><span id="surhorseA" style="font-weight:bold;"></span> </td><td><input type="text" id="horseA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="horseAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="horsem32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="horseDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="horseD" size="5" value="0"> </td><td><span id="surhorseD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="surdruidA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="druidA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="druidAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="druid32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="druidDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="druidD" size="5" value="0"> </td><td> <span id="surdruidD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr class = "alt">';
        kumar+='<td><span id="surarbA" style="font-weight:bold;"></span> </td><td><input type="text" id="arbA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="arbAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="arbal32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="arbDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="arbD" size="5" value="0"> </td><td><span id="surarbD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="surpraA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="praA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="praAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="praet32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="praDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="praD" size="5" value="0"> </td><td> <span id="surpraD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr class = "alt">';
        kumar+='<td><span id="surramA" style="font-weight:bold;"></span> </td><td><input type="text" id="ramA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="ramAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="ram32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="ramDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="ramD" size="5" value="0"> </td><td><span id="surramD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="surscorpA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="scorpA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="scorpAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="scorp32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="scorpDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="scorpD" size="5" value="0"> </td><td> <span id="surscorpD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr class = "alt">';
        kumar+='<td><span id="surbalisA" style="font-weight:bold;"></span> </td><td><input type="text" id="balisA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="balisAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="bally32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="balisDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="balisD" size="5" value="0"> </td><td><span id="surbalisD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="surgalleyA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="galleyA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="galleyAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="galley32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="galleyDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="galleyD" size="5" value="0"> </td><td> <span id="surgalleyD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr class = "alt">';
        kumar+='<td><span id="surstingA" style="font-weight:bold;"></span> </td><td><input type="text" id="stingA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="stingAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="sting32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="stingDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="stingD" size="5" value="0"> </td><td><span id="surstingD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<tr>';
        kumar+='<td><span id="surWgA" style="font-weight:bold;"></span> </td> <td> <input type="text" id="wgA" size="5" value="0"></td>';
        kumar+='<td><input type="text" id="wgAR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><div class="wship32 trooptdcm" /></td>';
        kumar+='<td><input type="text" id="wgDR" size="1" maxlength="4" value="0"> </td>';
        kumar+='<td><input type="text" id="wgD" size="5" value="0"> </td><td> <span id="surWgD" style="font-weight:bold;"></span></td>';
        kumar+='</tr>';

        kumar+='<div id="buttonbox">';
        kumar+='<input type=button value="Repeat" id="repeatbutton">';
        kumar+='<input type=button value="Reset" id="resetbutton">';
        kumar+='</div>';
        kumar+=	'<div id="bla">';

        kumar+='</div>';
        kumar+='</div>';
        $("#reportsViewBox").after(kumar);
        $( "#kumar" ).draggable({ handle: ".popUpBar" , containment: "window", scroll: false});
        $( "#kumar" ).resizable();

        $("#resetbutton").click(function() {reset();});
        $("#repeatbutton").click(function() {repeat();});

        $("#MGpen").change(function(){defenderTSF();});
        $("#Ascore").change(function(){defenderTSF();});
        $("#Wall").change(function(){defenderTSF();});
        $("#NightP").change(function(){defenderTSF();});
        $("#atackType").change(function(){defenderTSF();});

        $("#rangt").change(function(){defenderTSF();});
        $("#trit").change(function(){defenderTSF();});
        $("#prit").change(function(){defenderTSF();});
        $("#balit").change(function(){defenderTSF();});

        $("#cgD").change(function(){defenderTSF();});
        $("#vanqD").change(function(){defenderTSF();});
        $("#sorcD").change(function(){defenderTSF();});
        $("#rangD").change(function(){defenderTSF();});
        $("#triD").change(function(){defenderTSF();});
        $("#priD").change(function(){defenderTSF();});
        $("#scoutD").change(function(){defenderTSF();});
        $("#horseD").change(function(){defenderTSF();});
        $("#druidD").change(function(){defenderTSF();});
        $("#arbD").change(function(){defenderTSF();});
        $("#praD").change(function(){defenderTSF();});
        $("#ramD").change(function(){defenderTSF();});
        $("#scorpD").change(function(){defenderTSF();});
        $("#balisD").change(function(){defenderTSF();});
        $("#galleyD").change(function(){defenderTSF();});
        $("#stingD").change(function(){defenderTSF();});
        $("#wgD").change(function(){defenderTSF();});

        $("#vanqA ").change(function(){defenderTSF();});
        $("#sorcA").change(function(){defenderTSF();});
        $("#rangA").change(function(){defenderTSF();});
        $("#triA").change(function(){defenderTSF();});
        $("#priA").change(function(){defenderTSF();});
        $("#scoutA").change(function(){defenderTSF();});
        $("#horseA").change(function(){defenderTSF();});
        $("#druidA").change(function(){defenderTSF();});
        $("#arbA").change(function(){defenderTSF();});
        $("#praA").change(function(){defenderTSF();});
        $("#ramA").change(function(){defenderTSF();});
        $("#scorpA").change(function(){defenderTSF();});
        $("#balisA").change(function(){defenderTSF();});
        $("#galleyA").change(function(){defenderTSF();});
        $("#stingA").change(function(){defenderTSF();});
        $("#wgA").change(function(){defenderTSF();});

        $("#cgDR").change(function(){defenderTSF();});
        $("#balisDR").change(function(){defenderTSF();});
        $("#rangDR").change(function(){defenderTSF();});
        $("#triDR").change(function(){defenderTSF();});
        $("#priDR").change(function(){defenderTSF();});
        $("#vanqDR").change(function(){defenderTSF();});
        $("#sorcDR").change(function(){defenderTSF();});
        $("#scoutDR").change(function(){defenderTSF();});
        $("#arbDR").change(function(){defenderTSF();});
        $("#praDR").change(function(){defenderTSF();});
        $("#horseDR").change(function(){defenderTSF();});
        $("#druidDR").change(function(){defenderTSF();});
        $("#ramDR").change(function(){defenderTSF();});
        $("#scorpDR").change(function(){defenderTSF();});
        $("#galleyDR").change(function(){defenderTSF();});
        $("#stingDR").change(function(){defenderTSF();});
        $("#wgDR").change(function(){defenderTSF();});

        $("#balisAR").change(function(){defenderTSF();});
        $("#rangAR").change(function(){defenderTSF();});
        $("#triAR").change(function(){defenderTSF();});
        $("#priAR").change(function(){defenderTSF();});
        $("#vanqAR").change(function(){defenderTSF();});
        $("#sorcAR").change(function(){defenderTSF();});
        $("#scoutAR").change(function(){defenderTSF();});
        $("#arbAR").change(function(){defenderTSF();});
        $("#praAR").change(function(){defenderTSF();});
        $("#horseAR").change(function(){defenderTSF();});
        $("#druidAR").change(function(){defenderTSF();});
        $("#ramAR").change(function(){defenderTSF();});
        $("#scorpAR").change(function(){defenderTSF();});
        $("#galleyAR").change(function(){defenderTSF();});
        $("#stingAR").change(function(){defenderTSF();});
        $("#wgAR").change(function(){defenderTSF();});
    }
    function calculationCC(){
        //calculating attack values
        var infattk=0;
        for(var i=0;i<17;i++){
            var tempinfattk=attackerts[i]*aresearch[i]*ttattack[i]*isinf[i];
            infattk=infattk+tempinfattk;
        }
        infatk=infattk*inputnightp*inputmorale*inputportal;

        var cavattk=0;
        for(var j=0;j<17;j++){
            var tempcavattk=attackerts[j]*aresearch[j]*ttattack[j]*iscav[j];
            cavattk=cavattk+tempcavattk;
        }
        cavatk=cavattk*inputnightp*inputmorale*inputportal;

        var mystattk=0;
        for(var z=0;z<17;z++){
            var tempmystattk=attackerts[z]*aresearch[z]*ttattack[z]*ismgc[z];
            mystattk=mystattk+tempmystattk;
        }
        mystatk=mystattk*inputnightp*inputmorale*inputportal;

        var artattk=0;
        for(var x=0;x<17;x++){
            var tempartattk=attackerts[x]*aresearch[x]*ttattack[x]*isart[x];
            artattk=artattk+tempartattk;
        }
        artatk=artattk*inputnightp*inputmorale*inputportal;
        sumofattk=infatk+cavatk+mystatk+artatk;
        percinfattk=infatk*1.0/sumofattk;
        percavattk=cavatk*1.0/sumofattk;
        permystattk=mystatk*1.0/sumofattk;
        perartattk=artatk*1.0/sumofattk;
        if ( isFinite(percinfattk)==false) {
            percinfattk=0;
        }
        if ( isFinite(percavattk)==false) {
            percavattk=0;
        }
        if ( isFinite(permystattk)==false) {
            permystattk=0;
        }
        if ( isFinite(perartattk)==false) {
            perartattk=0;
        }
        //percentage of each attack multiplied by defence values
        for(var i=0;i<17;i++){
            ttinfdefz[i]=ttinfdef[i]*percinfattk;
            ttcavdefz[i]=ttcavdef[i]*percavattk;
            ttmystdefz[i]=ttmystdef[i]*permystattk;
            ttartdefz[i]=ttartdef[i]*perartattk;
        }
        //defence values
        var infdef=0;
        var cavdef=0;
        var mystdef=0;
        var artdef=0;
        for(var i=0;i<17;i++){
            var tempinfdef=defenderts[i]*dresearch[i]*ttinfdefz[i]*tswalld[i];
            infdef=infdef+tempinfdef;
            var tempcavdef=defenderts[i]*dresearch[i]*ttcavdefz[i]*tswalld[i];
            cavdef=cavdef+tempcavdef;
            var tempmystdef=defenderts[i]*dresearch[i]*ttmystdefz[i]*tswalld[i];
            mystdef=mystdef+tempmystdef;
            var tempartdef=defenderts[i]*dresearch[i]*ttartdefz[i]*tswalld[i];
            artdef=artdef+tempartdef;
        }
        if ( isFinite(infdef)==false) {
            infdef=0;
        }
        if ( isFinite(cavdef)==false) {
            cavdef=0;
        }
        if ( isFinite(mystdef)==false) {
            mystdef=0;
        }
        if ( isFinite(artdef)==false) {
            artdef=0;
        }
        sumofdef=infdef+cavdef+mystdef+artdef;
        //calculating ratios of attack to defence
        ratioinf=infatk*1.0/infdef;
        ratiocav=cavatk*1.0/cavdef
        ratiomyst=mystatk*1.0/mystdef
        ratioart=artatk*1.0/artdef
        ratiosum=sumofattk*1.0/sumofdef
        if ( isFinite(ratioinf)==false) {
            ratioinf=0;
        }
        if ( isFinite(ratiocav)==false) {
            ratiocav=0;
        }
        if ( isFinite(ratiomyst)==false) {
            ratiomyst=0;
        }
        if ( isFinite(ratioart)==false) {
            ratioart=0;
        }
        if ( isFinite(ratiosum)==false) {
            ratiosum=0;
        }
        //dividing into sub groups in order of attack magnitude
        if(ratiosum==0){
            attinflosses=0;
            attcavlosses=0;
            attmystlosses=0;
            attartlosses=0;
            definflosses=0;
            defcavlosses=0;
            defmystlosses=0;
            defartlosses=0;
        }
        if(ratiosum>0 && ratiosum<=0.1111111111){
            attinflosses=Number(1.0/Math.sqrt(ratioinf))*0.3;
            attcavlosses=Number(1.0/Math.sqrt(ratiocav))*0.3;
            attmystlosses=Number(1.0/Math.sqrt(ratiomyst))*0.3;
            attartlosses=Number(1.0/Math.sqrt(ratioart))*0.3;
            definflosses=ratioinf*1.0/10;
            defcavlosses=ratiocav*1.0/10;
            defmystlosses=ratiomyst*1.0/10;
            defartlosses=ratioart*1.0/10;
        }
        if(ratiosum>0.1111111111 && ratiosum<=0.25){
            attinflosses=Number(1.0/Math.sqrt(ratioinf))*0.2;
            attcavlosses=Number(1.0/Math.sqrt(ratiocav))*0.2;
            attmystlosses=Number(1.0/Math.sqrt(ratiomyst))*0.2;
            attartlosses=Number(1.0/Math.sqrt(ratioart))*0.2;
            definflosses=ratioinf*1.0/10;
            defcavlosses=ratiocav*1.0/10;
            defmystlosses=ratiomyst*1.0/10;
            defartlosses=ratioart*1.0/10;
        }
        if(ratiosum>0.25 && ratiosum<=1){
            attinflosses=Number(1.0/Math.sqrt(ratioinf))*1.0/10;
            attcavlosses=Number(1.0/Math.sqrt(ratiocav))*1.0/10;
            attmystlosses=Number(1.0/Math.sqrt(ratiomyst))*1.0/10;
            attartlosses=Number(1.0/Math.sqrt(ratioart))*1.0/10;
            definflosses=ratioinf*1.0/10;
            defcavlosses=ratiocav*1.0/10;
            defmystlosses=ratiomyst*1.0/10;
            defartlosses=ratioart*1.0/10;
        }
        if(ratiosum>1 && ratiosum<=4){
            attinflosses=Number(1.0/ratioinf)*1.0/10;
            attcavlosses=Number(1.0/ratiocav)*1.0/10;
            attmystlosses=Number(1.0/ratiomyst)*1.0/10;
            attartlosses=Number(1.0/ratioart)*1.0/10;
            definflosses=Math.sqrt(ratioinf)*1.0/10;
            defcavlosses=Math.sqrt(ratiocav)*1.0/10;
            defmystlosses=Math.sqrt(ratiomyst)*1.0/10;
            defartlosses=Math.sqrt(ratioart)*1.0/10;
        }
        if(ratiosum>4 && ratiosum<=9){
            attinflosses=Number(1.0/ratioinf)*1.0/10;
            attcavlosses=Number(1.0/ratiocav)*1.0/10;
            attmystlosses=Number(1.0/ratiomyst)*1.0/10;
            attartlosses=Number(1.0/ratioart)*1.0/10;
            definflosses=Math.sqrt(ratioinf)*2.0/10;
            defcavlosses=Math.sqrt(ratiocav)*2.0/10;
            defmystlosses=Math.sqrt(ratiomyst)*2.0/10;
            defartlosses=Math.sqrt(ratioart)*2.0/10;
        }
        if(ratiosum>9 && ratiosum<=100000){
            attinflosses=Number(1.0/ratioinf)*1.0/10;
            attcavlosses=Number(1.0/ratiocav)*1.0/10;
            attmystlosses=Number(1.0/ratiomyst)*1.0/10;
            attartlosses=Number(1.0/ratioart)*1.0/10;
            definflosses=Math.sqrt(ratioinf)*3.0/10;
            defcavlosses=Math.sqrt(ratiocav)*3.0/10;
            defmystlosses=Math.sqrt(ratiomyst)*3.0/10;
            defartlosses=Math.sqrt(ratioart)*3.0/10;
        }
        if(isFinite(attinflosses)==false){
            attinflosses=0;
        }
        if(isFinite(attcavlosses)==false){
            attcavlosses=0;
        }
        if(isFinite(attmystlosses)==false){
            attmystlosses=0;
        }
        if(isFinite(attartlosses)==false){
            attartlosses=0;
        }
        if(isFinite(definflosses)==false){
            definflosses=0;
        }
        if(isFinite(defcavlosses)==false){
            defcavlosses=0;
        }
        if(isFinite(defmystlosses)==false){
            defmystlosses=0;
        }
        if(isFinite(defartlosses)==false){
            defartlosses=0;
        }
        //surving defence TS calculation
        defloss=Number(definflosses*percinfattk+defcavlosses*percavattk+defmystlosses*permystattk+defartlosses*perartattk);
        for(var i=0;i<17;i++){
            lossdefTS[i]=zdefenderts[i]*defloss*defintensity;
            if(lossdefTS[i]>zdefenderts[i]){
                lossdefTS[i]=zdefenderts[i]
            }
            survivingdefTS[i]=parseInt(zdefenderts[i]-lossdefTS[i]);
        }
        $("#survcgD").text(survivingdefTS[0]);
        $("#survanqD").text(survivingdefTS[5]);
        $("#sursorcD").text(survivingdefTS[6]);
        $("#surRangD").text(survivingdefTS[2]);
        $("#surtriD").text(survivingdefTS[3]);
        $("#surpriD").text(survivingdefTS[4]);
        $("#surScoutD").text(survivingdefTS[7]);
        $("#surhorseD").text(survivingdefTS[10]);
        $("#surdruidD").text(survivingdefTS[11]);
        $("#surarbD").text(survivingdefTS[8]);
        $("#surpraD").text(survivingdefTS[9]);
        $("#surramD").text(survivingdefTS[12]);
        $("#surscorpD").text(survivingdefTS[13]);
        $("#surbalisD").text(survivingdefTS[1]);
        $("#surgalleyD").text(survivingdefTS[14]);
        $("#surstingD").text(survivingdefTS[15]);
        $("#surWgD").text(survivingdefTS[16]);
        //surving offensive TS calculation
        for(var i=0;i<17;i++){
            survoffTS[i]= attcavlosses*iscav[i]+ attinflosses*isinf[i]+attmystlosses*ismgc[i]+isart[i]*attartlosses;
        }
        for(var i=0;i<17;i++){
            lossattTS[i]=attackerts[i]*survoffTS[i]*atkintenstity;
            if(lossattTS[i]>attackerts[i]){
                lossattTS[i]=attackerts[i]
            }
            survivingattTS[i]=parseInt(attackerts[i]-lossattTS[i]);
        }
        $("#survanqA").text(survivingattTS[5]);
        $("#sursorcA").text(survivingattTS[6]);
        $("#surRangA").text(survivingattTS[2]);
        $("#surtriA").text(survivingattTS[3]);
        $("#surpriA").text(survivingattTS[4]);
        $("#surScoutA").text(survivingattTS[7]);
        $("#surhorseA").text(survivingattTS[10]);
        $("#surdruidA").text(survivingattTS[11]);
        $("#surarbA").text(survivingattTS[8]);
        $("#surpraA").text(survivingattTS[9]);
        $("#surramA").text(survivingattTS[12]);
        $("#surscorpA").text(survivingattTS[13]);
        $("#surbalisA").text(survivingattTS[1]);
        $("#surgalleyA").text(survivingattTS[14]);
        $("#surstingA").text(survivingattTS[15]);
        $("#surWgA").text(survivingattTS[16]);
    }
})();
