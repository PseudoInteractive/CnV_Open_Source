### 2021/2/12
 - **Fix** Ports and Shipyards work with layouts
 - **New** Clicking on the top border of the chat tab allows you to toggle between predefined sizes
 - **Fix** Castles issue warning on upgrade
 - **Fix** fixed some cases in which the refresh button would not refresh everything in the HTML windows.
 	- Tip:  If you find further cases of similar behavior, try refreshing twice. ;)
 - **Change** music does not play when you are subbing another player
 - **Fix** app would accidentally stop COTG's heartbeat function on occasion which would cause bad things to happen.
 - **Fix** friend online and offline notifications work
 - **Fix** if you had less than about 500 cities, in some cases they would load too fast and might not register properly in your citylists.
 - **Fix** chat translation works again
 - **Fix** another crash with build tab treeview
 - **Request** The auto naming algorithm has been improved:  
	- Names must be in the format (prefix)(continent)(middle)(uniqueNumber)(postfix)
	- prefix, middle and postfix cannot have numbers in them
	- continent and unique number must be only numbers
	- The following are valid names, the continent is in bold and the unique number is italic and the suggested name for the next city is shown after the "=>"
		- C**10** *1001* => C**10** *1002*
		- **11** *001* => **11** *002*
		- **11**NE*142* => **11**NE*143*
	- Prefix and or postfix can be empty.
 - **Request** Build tab cities start out collapsed
 - **Request** added upgrade and downgrade to quick build
 - **Fix** new cities did not show as cities on refresh
 - **Optimize** build queues
 - **New** Planner mode is ready to use.
   - You can switch to planner mode via the city build menus
### 2021/2/24
 - **Request** raids will now be selected such that the carry is *at least* the specified value.   
   - Previously raids would be selected such that the carry was as close as possible to the desired carry
 - **Update** City lists are sorted by name and remarks.
   - Previously they were only sorted by name.
### 2021/3/11
 - **update** extended build queues are disabled by default.  To enable them, set "Build Queue" in the settings page to 'Unlimited'
 - **fix** world map and region view in W23
 - **fix** sharing layouts
 - **update** Each chat tab now has a "copy to clipboard" button.  If messages are selected, it will copy then, otherwise it will copy all messages in the tab
   - If you are submitting a bug report, please switch to the "Debug" tab, copy all messages and paste this in with the bug report, thanks!
 - **added** Warning message if coucillors have expired (app will not work proprerly)
### 2021/3/17
 - **new** Added "Smart Rearrange" to planner: 'Moves buildings whose positions don't matter to where existing corresponding buildings are located or to locations without res nodes'
### 2021/3/26
 - **New** Added "Near Res": For sending resources from near cities
### 2021/3/26
 - **Fix** Near Res mis-calculated everything
 - **Fix** Custom colors work in world view.  "Click" on the "World" tab to set them up
   - Known issue:  The color picker controls overlap the native view, so they cannot be readily clicked
   - The workaround is to press on the web button (left of the refresh button up top) which makes web controls clickable
 - **New** the "Share" button in city layout with publish city layouts for everyone to see and use.  (For people not on the client, you can copy the layout to the clipboard)
### 2021/3/26
 - **New** Support for the _delay between raids_ functionality
  - for some reason COTG does not natively support a delay of 10s, if this is desired I could add it, but it may not work with auto raid
 - **New** Accelerator keys F2, F3 and F4 toggle between view layouts
### 2021/3/30
 - **New** Cities in region view are tinted by default based on your wold color settings
  - This can be changed in the settings page via "Tint"
 - **Update** The heatmap allows you to view changes in a more fine grained manner
### 2021/4/1
 - **New** When your mouse is over a players city in region or wold view, all cities that belong to that player will draw a flag so that you can easily tell what belongs to who
 - **Update** Smart rearrange in the planner will flip layouts in an attenmpt to minimize resource overlaps
 - **Fix** Near res would only work if the target city was yours
   - Kown issue:  The window seems to spontaneously refresh sometimes, recalcaulting resource send values
   - Need to fix
 - **Update** When a new city is founded, a Refresh will automatically be triggered
### 2021/4/4
 - **Fix** The _Update City Groups_ function was misbehaving
### 2021/4/7
 - **New** Setup City sequence allows you to choose a layout (sharestring)
### 2021/4/12
 - **New** Build menu displays whether or not cities are currently building and their current stage
  - if they are not building, you can right click on the city icon and select "Do the stuff" 
  - Shortcut is to click on their build stage (hyperlink, i.e. 'cabins complete' )
### 2021/4/13
 - **Update** The raid dungeon lists misbehaved when they expanded so I moved them to a popup
### 2021/4/13
 - **Fix** "Do the stuff" did not work properly without extended build queues.  Extended build queues will be on by default
### 2021/4/13
 - **Fix** Building towers or walls in a full city would trigger a cabin demo
### 2021/4/13
 - **Fix** Build queues not saving
### 2021/4/13
 - **Fix** Switching cities right after using 'Do the stuff' would not behave as expected
### 2021/4/14
 - **Request** You can now set your cities initial storehouse count (i.e. while building cabins)
### 2021/4/14
 - **New** Sorc towers will only be built when you ave more than tsForSorcTower (or it is a sorc city) and then it will be immediatey raidsed to level 10.
### 2021/4/16
 - **New** Layouts can now have resource settings.  When you share a layout it will incllude resource settings, which will be applied when selecting a layout
 - To export a layout without resource settings turn off the Trade Settings toggle
### 2021/4/16
 - **Known Issue** Options for tags appear twice during setup, the settings on the first page are only used if you do not set a layout
### 2021/4/16
 - **Known Issue** Options for trade resource settings appear twice during setup, the settings on final page will be copied from your layout if you selected one so you can safely ignore them
### 2021/4/16
 - **Update** Smart Rearrange will flip layouts if there are buildings floating in the moat
### 2021/4/17
 - **Fix** Trade settings not updating all the setup windows
### 2021/4/17
 - **New** You can draw troop overlays for your cities via 'troops visible' in settings
### 2021/4/19
 - **Update** When the Near Res Tab is open, trade routes will be shown
### 2021/4/19
 - **Update** When you use "Find Hub" on a city that is a hub, it will clear its target hub (setting the target of a hub is a mistake I've made far too many times)
### 2021/4/19
 - **Tip** If you want to update the hubs of a cluster (say you added another hub)
    - first make sure that city lists are updated
	- Hold shift and click on the citys in the cluster
	- Still holding shift, right click on any city and use "Setup - Find Hub"
	- Alternatively right click on a hub (still holding shift) and select "Setup - Set Target Hub" to send to a specific hub
	 - There will be a new interface for setting shipper targets, but until that is ready this is the easiest way to direct shippers to send somewhere
	 - I.e. filter by City Group "Shippers", select all, shift click on the target and Setup Set Target Hub
	- There is also "Setup Source Hub" to pull from a specific hub
### 2021/4/25
 - **Known Issue**
   - When the heatmaps were combined I did some bad time zone stuff.  
   - Historical snapshops that you and others submitted are off by the number of hours it takes for the world to spin you to England (or wherever, depending on the server).
   - If you walk through a heatmap it may seem like a jumpy trip, but the asymptotic results will be accurate and statistically everything will be mostly fine (which is really not that imporant)
### 2021/4/26
 - **New**
  - Global continent filter lets you filter cities and heat map results by continent
  - _Because we don't have enough ways to filter by continent..._
  - Selecting more than one continent to filter by is supported
  - If no contients are selected the filter is disabled and everything passes
  - Supported for build tab, raid tab, and Heatmap summaries (to see how much activity took place on a continent)
### 2021/4/26
 - **New**
  - Food Warning context item on cities
  - Select multiple cities to apply one setting to all
### 2021/4/28
  - **Update**
   - Streamlined setup, combined setting sharestring with setting hub settings
   - There are three expadning groups:  Layout, trade Settings and Tags
    - Groups that are collapsed will not be be applied to the city
  - **New**
    - You can set Layout/Trade setup/Tags via city context menu selecting 'Setup/Change'
	- You can select multiple cities using shift click, then (still holding shift) right click and select 'Setup/Change' and the settings will be applied to all of them
### 2021/4/29
  - **Update** 
   - AttackPlanner now accepts one max travel time for senators and a separate one for SE
   - AttackPlanner not aims to minimize morale penalty.  If you are with FBK, don't worry, this has nothing to do with W23.
### 2021/4/30
  - **Change** 
   - New city setup is now a notice rather than a popup.
   - You can click on the notice or find the Setup hyperlink in the debug tab to run setup
### 2021/5/3
  - **New** 
   - Q key is not a shortcut for Layout build
  - **New**
   - Added Recruit Senator command to War context menu
  - **Update**
   - Autobuild settings are only set on *Setup*, previously they would get set when setting the Layout
### 2021/5/3
 - **Fix**
   - Temple donation items would go to a city at 0,0 due to COTG not findhing the region view
 - **New**
   - The continent filter has been upgraded to filter by continent and optionally tags.
     - This is similar to city lists without the city lists
	 - For example you might select vanqs sorcs, druids, scorps, horses to see all offense, or add 43 to see all offense on 43
 - **Fix**
   - Raiding would often fail when set to "Enough" rather than "all"
   - Dungeons are selected based on min, max, target and Distance biased but which will produce the best raid revenue
### 2021/6/19
   - **Fix**
    - World map not displaying properly on W24
   - **Tip**
    - To run a second instance of the app (for another world for instance) press [Windows Key]+R, then type: cotg:launch?n=1
	- Not thoroughly tested but it seems to work...
	- https://i.imgur.com/OrxwWHv.png
### 2021/6/21
   - **Workaround**
	 - Some mysterious change has caused the server to not send a cookie when you login
	 - The temporary workaround is to log in with a browser, copy your cookie and set it in the App.
	  - Once set, the cookie will last until you clear the cookie (i.e. switch user or uninstall)
	 - The problematic cookie is called "remember_me"
	 - We will post instruction on how to get the cookie from various browsers in Discord (the cookie is a number)
	 - In app you press the cookie button (top center) and enter the number, this will be remembered, so you should only have to do it once
### 2021/6/22
   - **Workaround 2**
	 - More cookies
	 - Now there are two
	 - Please see https://docs.google.com/document/d/1V8Zd47Sw47zzj0UIVge9VBkg4Y8uTut6OIDLR1WR2s8/edit?usp=sharing
### 2021/6/24
   - **New**
	 - The Heatmap now shows measures of player activity
	 - Select a range of dates, set a continent filter if desired and player activity will be displayed in the bottom tab
	 - **Tip**
	  - To see changes for a day, first tap on the "..click to load" text to load the data
	 - **Tip**
	  - To export to a spread sheel, use control-a and then control-c, then paste somewhere
### 2021/7/14
  - **Update**
    - Res clearing now removes all res nodes when buildings should go, previously it only removed the center
      - This can be disabled in settings
  - **Update**
    - ExportForWar temples 3 state option has been replaced with a onlyTemples checkbox
  - **New**
    - Preliminay Discord bridge for chat
    - Works best if player nicknames match in game names
    - Alliance Discord admins need me to set this up for it to work.

### 2021/7/17
  - **Fix** 
    - Temples do not demo when you don't have building space.
  - **Fix** 
    - Citys called *New City* did not work with auto build

### 2021/7/18
  - **Fix**
    - Smart rearrange did not cooperate with ports and shipyards
  - **Update**
    - When linking discord nicknames to player names, case it not considered.  I.e. "Avatar" now matches "avatar"
  - **New**
    - You can turn off building overlays using BuildingOverlays setting in Settings.
- ### 2021/7/18
  - **Fix**
    - When swapping buildings if one was building the move would fail and leave a building in a random place
  - **Upate**
    - If you have cabins in your layout cabins will be put there during inital cabin layout
  - **Note** subs are finicky
    - Here is how to enter a sub:
		- Click Accept
        - If your requested sub is set, clear it
        - Set it to someone random
        - Clear it
        - Set it back to your desired sub
        - Click Enter Account 
        - Don't ask questions;
  - **Fix**
    - Fixed a bug. 
- ### 2021/7/18
  - **New**
    - You can scale down the font sizes for some things (if the fonts are too large for you)
    - You must restart to see the changes
  - **New**
    - Donation priorization no longer treats Do No Send as the highest setting
  - **Update**
    - When donating, the ratio sent will be proportionate to what is missing
      - You can enable/disable this with the "Wood Vs Stone" slider
- ### 2021/7/25
   - **New**
     - ReturnRaidsForAttacks button in raid tab to return all raids in time for scheduled attacks
       - Experimental, please double check results
- ### 2021/8/01
   - **New**
     - You can now set the max ratio of triari to send out (setting was 1 and missing in prior build)
     - Various other raiding options
 ### 2021/8/01
   - **New**
     - If your build queue might be block in a situation lie:
       - 100 buildings
       - 15 castles queued to be built (in COTG queue)
       - 15 demos queue after (not yet in COTG queue)
      - A coloumn (to the far right) labedl ?? will be checked
      - A checked entry does not guarantee this situation, I dont' know why.
      - WildHeader and Ting wanted it, its not my fault
 ### 2021/8/06
   - **Update**
     - Login process now uses 50% less cookies and you can no longer log into other peoples accounts
     - For login instruction https://docs.google.com/document/d/1V8Zd47Sw47zzj0UIVge9VBkg4Y8uTut6OIDLR1WR2s8/edit?usp=sharing 
 ### 2021/8/08
   - **Fix** 
     - Background build queues overestimated their duration by 1000x.
   - **Update**
     - New algorithm for unblocking build queue
     - Not it handles things like 49 cabins + clear res more gracefully
   - **New**
     - You can scale down the COTG side bar for more screen space
       - click on the side bar, hold down control and scroll mouse wheel down
       - Or on touch screen pinch it
       - Or on track pad either pinch the track pad or do a two finger scroll. I really don't know.
     - When done press F5 or refresh button and things will mostly adjust
### 2021/8/08
     - **Fix**
        - Attack Planner failed to load in release builds
### 2021/8/16
     - **Fix**
        - City stats show in raid tab
     - **Update**
        - Hubs will ask to build sorc tower to 10 when their score >= 2000 (configuarable)
     - **Update**
        - DoTheStuff will warn when a layout is very different from the current building setup and would require many moves to fix
        - You may want to select a new layout or open planner and select "Use Buildings"
     - **Update**
        - For every 8 or so moves performed during a operation, DoTheStuff will warn you, allowing you to stop
     - **Update**
        - Use Buildings is available from the Planner tab
     - **Fix**
        - Attack planner will add newlines to the plans that get emailed out
            - Why are HTML newlines so temperamental?
     - **New**
        - Right click on the continent/tag filter to clear it
            - tlgger request
### 2021/8/21
    -   **Fix**
        - The number of milliseconds since Unix was started is now to big to fit in an integer, this caused ministers to think that they were expired.
            - Changed the date at which Unix was started.
    -   **New**
        -   City Tag: "SevenPoint", for 7 point castles
            -   If you need a good layout, I have shared one: see "7pt"
    -   **Fix**
        - Boss tab has fewer focus issues