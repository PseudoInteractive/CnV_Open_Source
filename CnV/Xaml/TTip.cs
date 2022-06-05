using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
// seen tips

using TTipsPersist = System.Collections.Immutable.ImmutableDictionary<string,CnV.TTipPersist>;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace CnV
{
	[MessagePackObject]
	public record class TTipPersist
	{
		[Key(0)]
		public string id;
		[Key(1)]
		public ServerTime nextReady;

		public TTipPersist(string id,ServerTime nextReady)
		{
			this.id=id;
			this.nextReady=nextReady;
		}
	}
	/*
	  Name format:
	 */
	internal record struct TTipId
	{
		internal string name;
		internal int group;
		internal int sequence;
	//	internal int sequence;
		internal TTipId(string s)
		{

			var ss = new StringTokenizer(s,new[] { '_' }).ToArray();
			Assert(ss.Length >= 2);
			Assert(ss[0].Value == "TT");
			name= ss[1].Value;
			group = ss.Length > 2 ? ss[2].Value.ParseInt().GetValueOrDefault() : 0;
			sequence = ss.Length > 3 ? ss[3].Value.ParseInt().GetValueOrDefault() : 0;

		}
	}

	internal class TTip
	{
		internal static ConcurrentDictionary<string,ImmutableQueue<TTip>> pending = new();

		//	internal static ImmutableSortedSet<double> seen = ImmutableSortedSet<double>.Empty;
		internal string id; // sequence id.  Tips are shown in ascending order when their predicates are valid
		internal FrameworkElement target;
		internal TeachingTipPlacementMode placement;
		internal bool lightDismiss = true;
		internal Func<bool> predicate;
		internal string title;
		internal string subTitle;
		internal TTipX tip;
		internal TTipId tid => new TTipId(id);
		static bool PredicateTrue() => true;
		internal TTip(string id,Func<bool> predicate =null,string title = null,string subTitle = null,FrameworkElement target = null,TeachingTipPlacementMode placement = TeachingTipPlacementMode.Auto,bool lightDismiss = true)
		{
			this.id=id;
			this.target=target;
			this.placement=placement;
			this.lightDismiss=lightDismiss;
			this.predicate=predicate ?? PredicateTrue;
			this.title=title;
			this.subTitle=subTitle;
			var tid = this.tid;

			// has it been seen?

			var p = pending.AddOrUpdate(tid.name,ImmutableQueue.Create(this), (a,prior)=> prior.Enqueue(this) );
			//p = p.Enqueue(this);

		}
		internal Task<bool>  Show()
		{
			return AppS.DispatchOnUIThreadAsync((tcs) =>
			{

			if(tip == null)
			{
				if(!TTipX.all.TryGetValue(id,out tip))
				{
					Assert(false);
					return;
				}
				tip.tip = this;
				tip.Closed +=(TeachingTip sender,TeachingTipClosedEventArgs args) =>
					{
					 ImmutableInterlocked.AddOrUpdate(ref persist.V,id,new TTipPersist(id,ServerTime.infinity),(k,prior) => prior with { nextReady = ServerTime.infinity } );
					tcs.SetResult(true);
					};
				}
				tip.IsOpen = true;
			});

		}

		

		internal static SettingsFile<TTipsPersist,TTipsPersist,PersistDataTransformNone<TTipsPersist>> persist = new("ttips",TTipsPersist.Empty);
	}


}
