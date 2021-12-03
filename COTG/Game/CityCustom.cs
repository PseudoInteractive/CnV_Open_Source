using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MessagePack;
namespace CnV
{
	using CnV;

	using System.IO;

	using Windows.Storage;
	using Game;
	using Views;

	public static class SettingsFile<TData,TSerialize> where TSerialize: new() where TData: new()
	{
		public static byte[] lastLoaded;
		public static TData V;
		public static string fileName => $"{ApplicationData.Current.LocalFolder.Path}/Settings{nameof(TData)}.mpk";
		public static async Task<bool> Load( Func<TSerialize,TData> transform, Action<TData,bool> onComplete=null, Func<TData> _default =null )
		{
			if (lastLoaded is not null)
				return false;
			try
			{
				byte[] buffer = null;
				using (FileStream fs = new FileStream(fileName, new FileStreamOptions()
				       {
					       Mode = FileMode.OpenOrCreate,
					       Access = FileAccess.ReadWrite,
					       Share = FileShare.ReadWrite,
					       Options = FileOptions.Asynchronous | FileOptions.SequentialScan
				       }))
				{
					var lg = fs.Length.AsInt();
					buffer = new byte[lg];
					var readSoFar = 0;
					for (;;)
					{
						if (readSoFar >= lg)
							break;
						var count = await fs.ReadAsync( new Memory<byte>(buffer, readSoFar, lg - readSoFar) );
						readSoFar += count;
						if (count == 0 )
							break;
					}
				}

				var loaded = AMessagePack.TryDeserialize<TSerialize>(buffer, out var serialized );
				
				
					V = loaded ? transform(serialized) :
						_default is not null ? _default() :
						default;
				
				onComplete?.Invoke(V, loaded);
				lastLoaded = buffer;

				return true;
				

			}
			catch (Exception ex)
			{
				LogEx(ex);
				return false;
			}

		}



	}

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
					using (FileStream fs = new FileStream(pinsFileName, FileMode.Truncate,FileAccess.Write,FileShare.ReadWrite, 64*1024,false))
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
		public static void Load()
		{
			if(!CityCustom.loaded)
			{
				Assert(JSClient.world != 0);
				try
				{
					using (FileStream fs = new FileStream(pinsFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,FileShare.ReadWrite,64*1024,false))
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


				
				
			}
		}
	}
	

}
