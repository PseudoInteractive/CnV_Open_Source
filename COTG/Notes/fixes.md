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
### 2021/4/14
 - **Known Issue** The first time you open the raid box, it will be empty, please close and re-open
 