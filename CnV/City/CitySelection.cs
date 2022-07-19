using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CnV.City;
namespace CnV;

using Windows.System;

public static partial class CityUI
	{
	public static void ProcessSelection(this City me, ClickModifiers mod)
	{
		++SpotTab.silenceSelectionChanges;

		AppS.DispatchOnUIThread(() =>
		{
			try
			{
				//	var sel0 = SpotTab.instance.selectedGrid.SelectedItems;
				//var grid = GetGrid();
				//var sel1 = grid.SelectedItems;
				var cid = me.cid;
				
				var present = selected.Contains(cid);
				if(mod.IsShift()) {
					if(!present)
						selected = selected.Add(cid);
				}

				else if(mod.IsControl()) {
					
						selected = ImmutableHashSet<int>.Empty.Add(cid);
					}

					else {
					// toggle
						// clear selection and select this
						if(present) {
							/// nothing
							selected = selected.Remove(cid);
						}
						else {
							selected = selected.Add(cid);

							///sel0.Clear();
							//sel0.Add(this);

							//sel1.Clear();
							//sel1.Add(this);
						}
						//                   SpotTab.SelectOne(this);
					}
					SpotTab.SyncSelectionToUI();
				}
			catch(Exception e)
			{
				LogEx(e);
			}
			finally
			{
				--SpotTab.silenceSelectionChanges;
			}
		});
		//    SpotTab.SelectedToGrid();
	}

}

