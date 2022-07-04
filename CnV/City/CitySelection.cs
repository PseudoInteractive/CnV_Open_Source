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
				var sel = selected;
				var present = sel.Contains(cid);
				var wantUISync = false;
				if(mod.IsShift() || mod.IsControl() )
				{
					if(present)
					{
						if(!mod.IsShift())
						{
							selected = new HashSet<int>(sel.Where(a => a != cid));
							//		sel0.Remove(this);
							//		sel1.Remove(this);

						}
						else
						{
							//	wantUISync = true;
						}
					}
					else
					{
						var newSel = new HashSet<int>(sel);
						newSel.Add(cid);
						selected = newSel;

						//sel0.Add(this);
						//	sel1.Add(this);
						wantUISync = true;

					}
					//                 SpotTab.SelectedToGrid();
				}

				else
				{
					wantUISync = true;
					// clear selection and select this
					if(present && selected.Count == 1)
					{
						/// nothing
					}
					else
					{
						selected = new HashSet<int>(new[] { cid });

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

