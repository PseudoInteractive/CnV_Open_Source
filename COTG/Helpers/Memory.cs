global using ByteOwner = Microsoft.Toolkit.HighPerformance.Buffers.MemoryOwner<byte>;

using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.HighPerformance.Buffers;

namespace COTG
{
	public static class AMem
	{
		public static void SafeDispose<T>( ref T me ) where T: class,IDisposable
		{
			if(me is null )
				return;
			var _me = me;
			me = null;
			_me.Dispose();
		}

		public static int compressionBufferSize= 81920;
		// closes stream when done
		public static ByteOwner ReadCompressed(System.IO.Stream source)
		{
			try
			{
				using var deflate = new GZipStream(source,CompressionMode.Decompress);
				var readBuffer = ByteOwner.Allocate(compressionBufferSize);
				var readOffset = 0;

				for(;;)
				{
					var spaceLeft = readBuffer.Length - readOffset;

					// need to reallocate?
					if(spaceLeft <= 8*1024)
					{
						var _readBuffer = readBuffer;
						compressionBufferSize = compressionBufferSize*3/2;
						readBuffer = ByteOwner.Allocate(compressionBufferSize);
						_readBuffer.Span.CopyTo(readBuffer.Span);
						_readBuffer.Dispose();
					}

					var readSize = deflate.Read(readBuffer.Span.Slice(readOffset) );
					if(readSize == 0)
						break;
					readOffset += readSize;
				}

				return readBuffer.Slice(0,readOffset);
			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}
			finally
			{
				source.Close();
			}
			return ByteOwner.Empty;
		}
		//public static async Task<MemoryOwner> ReadCompressedAsync(System.IO.Stream source)
		//{
			
			
		//	return BinaryData.FromStreamAsync(deflate);

		//	//var readBuffer = ByteOwner.Allocate(compressionBufferSize);
		//	//var readOffset = 0;

		//	//for(;;)
		//	//{
		//	//	var spaceLeft = readBuffer.Length - readOffset;

		//	//	// need to reallocate?
		//	//	if(spaceLeft <= 8*1024)
		//	//	{
		//	//		var _readBuffer = readBuffer;
		//	//		compressionBufferSize = compressionBufferSize*3/2;
		//	//		readBuffer = ByteOwner.Allocate(compressionBufferSize);
		//	//		_readBuffer.Span.CopyTo(readBuffer.Span);
		//	//		_readBuffer.Dispose();
		//	//	}

		//	//	var readSize = deflate.Read(readBuffer.Slice(readOffset,readBuffer.Length-readOffset).Span);
		//	//	if(readSize == 0)
		//	//		break;
		//	//	readOffset += readSize;
		//	//}
		//	//return readBuffer.Slice(0,readOffset);

		//}

	}
}
