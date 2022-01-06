using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MessagePack;
namespace CnV
{
	using System.IO;

	using Windows.Storage;
	using Game;
	using Views;
	using CityCustoms = System.Collections.Concurrent.ConcurrentDictionary<int, CityCustom>;

	[MessagePackObject]
	public struct CityCustom
	{
		[Key(0)]
		public int cid; // this is redundant
		[Key(1)]
		public bool pinned;

		public static SettingsFile<CityCustoms, CityCustom[]> all = new();
		
		public override int GetHashCode() => cid;
		public override string ToString() => $"{City.Get(cid)}: pinned={pinned}";
		
		public static void Save()
		{
			if (all.isLoaded)
			{
				// Clear pinned values
				foreach (var i in all.V)
				{
					var prior = i.Value;
					prior.pinned = (false);
					all.V[i.Key] = prior; // not atomic
				}
				// Set new ones
				foreach (var i in Settings.pinned)
				{
					all.V.AddOrUpdate(i,
						(i)=>new CityCustom() {cid=i,pinned=true},
						(i,prior)=>
						{
							prior.pinned = true;
							return prior;
						} );
				}
				all.Save( (data) => data.Values.ToArray() );
			}
		}

		public static void Load()
		{
			all.Load( (tArray) => new CityCustoms(tArray.ToDictionary( a=>a.cid) ),
				(v, loaded) => Settings.pinned = v.Where(a=>a.Value.pinned).Select(a=>a.Value.cid).ToArray(), // this might wind up on a different thread
				()=>new(), // empty dictionary
				true);  // Async please
		}
	}
	

}
