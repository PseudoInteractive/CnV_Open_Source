// ==UserScript==
// @name         Clickable coords in mail.
// @namespace    https://cotg.app
// @version      0.1
// @description  Clickable coords in mail supports ###:### and ### : ###.
// @author       Jackson
// @match        https://*.crownofthegods.com/
// @grant        none
// @updateURL https://raw.githubusercontent.com/jacksoncds/cotg-scripts/main/mail-city-coords.user.js
// @downloadURL https://raw.githubusercontent.com/jacksoncds/cotg-scripts/main/mail-city-coords.user.js
// ==/UserScript==

function mailcitycoordsInit() {

    $(document).on("click","#mailTable tbody td", function(e){
        setTimeout(function(){
            var mailElement = $("#mailrMess");
            var mailData = mailElement.html();

            var withCoords = mailData.replace(/\d{1,3}\s?:\s?\d{1,3}/gm, '<span class="cityblink shcitt" style="color:#ee0645ad;"><coords>$&</coords></span><sup>s</sup>');

            setTimeout(function(){
                document.getElementById("mailrMess").innerHTML = withCoords;
            }, 100);
        }, 100);
    });
}