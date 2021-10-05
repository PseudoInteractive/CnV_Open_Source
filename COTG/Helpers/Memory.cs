using System;

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
		

		public static int compressionBufferSize= 256*1024;
		//public static MemoryOwner<byte> ReadCompressed(System.IO.Stream source)
		//{
		//	using var deflate = new GZipStream(source,CompressionMode.Decompress);
		//	var readBuffer = MemoryOwner<byte>.Allocate(compressionBufferSize);
		//	var readOffset = 0;

		//	for(;;)
		//	{
		//		var spaceLeft = readBuffer.Length - readOffset;

		//		// need to reallocate?
		//		if(spaceLeft <= 8*1024)
		//		{
		//			var _readBuffer = readBuffer;
		//			compressionBufferSize = compressionBufferSize*3/2;
		//			readBuffer = MemoryOwner<byte>.Allocate(compressionBufferSize);
		//			_readBuffer.Span.CopyTo(readBuffer.Span);
		//			_readBuffer.Dispose();
		//		}

		//		var readSize = deflate.Read(readBuffer.Slice(readOffset,readBuffer.Length-readOffset).Span );
		//		if(readSize == 0)
		//			break;
		//		readOffset += readSize;
		//	}
		//	return readBuffer.Slice(0,readOffset);

		//}
		public static Task<BinaryData> ReadCompressedAsync(System.IO.Stream source)
		{
			
			using var deflate = new GZipStream(source,CompressionMode.Decompress);
			return BinaryData.FromStreamAsync(deflate);

			//var readBuffer = MemoryOwner<byte>.Allocate(compressionBufferSize);
			//var readOffset = 0;

			//for(;;)
			//{
			//	var spaceLeft = readBuffer.Length - readOffset;

			//	// need to reallocate?
			//	if(spaceLeft <= 8*1024)
			//	{
			//		var _readBuffer = readBuffer;
			//		compressionBufferSize = compressionBufferSize*3/2;
			//		readBuffer = MemoryOwner<byte>.Allocate(compressionBufferSize);
			//		_readBuffer.Span.CopyTo(readBuffer.Span);
			//		_readBuffer.Dispose();
			//	}

			//	var readSize = deflate.Read(readBuffer.Slice(readOffset,readBuffer.Length-readOffset).Span);
			//	if(readSize == 0)
			//		break;
			//	readOffset += readSize;
			//}
			//return readBuffer.Slice(0,readOffset);

		}

	}
}
