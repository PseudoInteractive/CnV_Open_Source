using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
// seen tips

using TTipsPersist = System.Collections.Immutable.ImmutableDictionary<double,CnV.TTipPersist>;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	[MessagePackObject]
	public class TTipPersist
	{
		[Key(0)]
		public double id;
		[Key(1)]
		public ServerTime nextReady;
	}


	internal class TTip
	{
	
	//	internal static ImmutableSortedSet<double> seen = ImmutableSortedSet<double>.Empty;
		internal double id; // sequence id.  Tips are shown in ascending order when their predicates are valid
		internal FrameworkElement target;
		internal TeachingTipPlacementMode placement;
		internal bool lightDismiss = true;
		internal Func<bool> predicate;
		internal string title;
		internal string subTitle;
		internal TeachingTip tip;

		internal TTip(double id,string title,string subTitle=null,FrameworkElement target=null,TeachingTipPlacementMode placement=TeachingTipPlacementMode.Auto,Func<bool> predicate = null,bool lightDismiss=true)
		{
			this.id=id;
			this.target=target;
			this.placement=placement;
			this.lightDismiss=lightDismiss;
			this.predicate=predicate;
			this.title=title;
			this.subTitle=subTitle;
		
		}
		

		internal static SettingsFile<TTipsPersist,TTipsPersist,PersistDataTransformNone<TTipsPersist>> all = new("ttips");
	}


}
