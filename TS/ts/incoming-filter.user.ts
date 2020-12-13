// ==UserScript==
// @name         Incoming attack filter
// @namespace    https://cotg.app
// @version      1.0
// @description  Lets you watch for attacks on other members of your alliance.
// @author       Jackson
// @match        https://*.crownofthegods.com/
// @grant        none
// @updateURL https://raw.githubusercontent.com/jacksoncds/cotg-scripts/main/incoming-filter.user.js
// @downloadURL https://raw.githubusercontent.com/jacksoncds/cotg-scripts/main/incoming-filter.user.js
// ==/UserScript==


function incommingfilterInit() {

let cachedIncoming = -1;
    $("#contselectorAIPic").after("<button id='jscript_incattackfilter_choose' class='greensel aipcbsel' style='width:150px'>Incoming Filter</button>");
    $("#incomingsSpan").append("<span id='jscript_incattackfilter'>(0)</span>");


    let content = `<div id="jscript_incattackfilter_players" style="display:none; z-index: 4003;width: 300px;height: 500px;position: absolute;top: 98px;left: 425px;background-color: rgba(0, 0, 0, 0.8);color: white;font-size: 21px;box-shadow: 2px 2px 4px #ee000000;/* overflow-y: auto; */">
        <div style="width:100%;height: 40px;background-color: #ff000080;/* padding: 20px 10px; */"><span style="
        padding: 0px 10px;
        cursor: grab;
        display: flex;
        line-height: 2;
        justify-content: space-between;
        ">Incoming Filter - Players <div id="centxbuttondiv" class="jscript_incattackfilter_close" style="
        cursor: pointer;
        "></div></span></div><input type="text" id="jscript_incattackfilter_search" label="searchPlayer" style="width:100%;padding:10px;margin-top: 10px;" placeholder="Search Player">
        <ul id="jscript_incattackfilter_listplayers" style="
        list-style-type: none;
        width: 100%;
        height: calc(100% - 100px);
        overflow-y: auto;
        ">
        </ul>
        </div>`

    $("body").append(content);
    $("#jscript_incattackfilter_players").draggable();

    let button = $("#jscript_incattackfilter_choose");

    button.on('click', () => {
        $("#jscript_incattackfilter_players").show();
    });


    function addPlayer(name) {
        $("#jscript_incattackfilter_listplayers").prepend(`<li style="
            padding: 5px;
            "><div style="
            display: flex;
            justify-content: space-between;
            "><span>${name}</span><span class="jscript_incattackfilter_remove" data-player=${name} style="cursor: pointer;">X</span></div></li>`);

        let current = JSON.parse(localStorage.getItem('jscript_incattackfilter'));

        if (!current) {
            current = [];
        }

        if (current.includes(name) == false) {
            current.push(name);
        }

        localStorage.setItem('jscript_incattackfilter', JSON.stringify(current));
        notify();
    }

    function getAllianceMembers() {
        
        let allianceName = aldt["n"];
        if (allianceName) {
            $.post('includes/gAd.php', { a: allianceName }, function (res, error) {
                let data = JSON.parse(res);

                $("#jscript_incattackfilter_search").autocomplete({
                    source: data.me.map(r => r.n),
                    select: function (ev, ui) {
                        ev.preventDefault();
                        addPlayer(ui.item.value);
                    }
                })
            });
        }
    }

    getAllianceMembers();

    $(".jscript_incattackfilter_close").click(function () {
        $("#jscript_incattackfilter_players").hide();
    });

    $('body').on('click', '.jscript_incattackfilter_remove', function () {
        let player = $(this).data('player');
        let current = JSON.parse(localStorage.getItem('jscript_incattackfilter'));

        current = current.filter(value => value !== player);
        localStorage.setItem('jscript_incattackfilter', JSON.stringify(current));
        $(this).closest("li").remove();
        notify();
    });

    // load
    let currentPlayers = JSON.parse(localStorage.getItem('jscript_incattackfilter'));
    if (currentPlayers) {
        currentPlayers.forEach(p => {
            addPlayer(p);
        });
    }

    function notify() {
        let players = JSON.parse(localStorage.getItem('jscript_incattackfilter'));
        if (players) {
            $.post('includes/getIO.php', {}, data => {
                let attacks = [];

                let parsedData = JSON.parse(data);

                parsedData.inc.forEach(atk => {
                    if (players.includes(atk.tpn)) {
                        attacks.push(atk);
                    }
                });

                $("#jscript_incattackfilter").text("(" + attacks.length + ")");
            });
        }
    }
    

    setInterval(() => {


        let incoming = Number($("#allianceIncomings").text().replace('(', '').replace(')', ''));

        if (cachedIncoming !== incoming) {
            cachedIncoming = incoming;

            notify();
        }

    }, 30000);

    notify();
}