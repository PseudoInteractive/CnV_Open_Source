using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace COTG
{
	using System.IO;
	using Windows.Storage;

	[MessagePackObject]
	public struct CityCustom:IEquatable<CityCustom>
	{
		[Key(0)]
		public int cid; // this is redundant
		[Key(1)]
		public bool pinned;

		public static ImmutableHashSet<CityCustom> all = ImmutableHashSet<CityCustom>.Empty;
		public static bool loaded;

		public bool Equals(CityCustom other)
		{
			return cid == other.cid;
		}

		public override int GetHashCode() => cid;
		public override string ToString() => $"{City.Get(cid)}: pinned={pinned}";
		public static string pinsFileName => $"{folder.Path}/CityCustom{JSClient.world}.mpk";
		static StorageFolder folder => ApplicationData.Current.LocalFolder;
		public static void Save()
		{
			if (CityCustom.loaded)
			{
				try
				{
					Assert(JSClient.world != 0);
					all = SettingsPage.pinned.Select( a => new CityCustom() { cid = a, pinned = true} )
						.ToImmutableHashSet();
					using (FileStream fs = new FileStream(pinsFileName, FileMode.Create))
					{
						AMessagePack.Serialize(fs,all);
					}
				}
				catch (Exception e)
				{
					Debug.LogEx(e);
				}
			}

		}
		public async static Task Load()
		{
			if(!CityCustom.loaded)
			{
				Assert(JSClient.world != 0);
				try
				{
					using (FileStream fs = new FileStream(pinsFileName, FileMode.Open))
					{
						CityCustom.all = (AMessagePack.Deserialize(fs,() => Array.Empty<CityCustom>() )).ToImmutableHashSet();
						SettingsPage.pinned = CityCustom.all.Where(a=>a.pinned).Select(a=>a.cid).ToArray();
						CityCustom.loaded = true;
					}
				}
				catch (Exception ex)
				{
					LogEx(ex);
				}


				
				Trace("Pins");
		
			}
		}
	}
	

}
