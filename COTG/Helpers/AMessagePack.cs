using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using MessagePack.Formatters;
namespace COTG
{
	public static class AMessagePack
	{
		public static IFormatterResolver defaultResolver = CompositeResolver.Create(
					StandardResolver.Instance,
					PrimitiveArrayResolver.Instance,
					ImmutableCollectionResolver.Instance
				
		);
		public static MessagePackSerializerOptions defaultOptions =
			MessagePackSerializerOptions.Standard
			.WithCompression(MessagePackCompression.Lz4BlockArray)
			.WithResolver(defaultResolver);
		public static byte[] Serialize<T>(T data)
		{
				return MessagePackSerializer.Serialize(data,defaultOptions);
			
		}


		public static T Deserialize<T>(ReadOnlyMemory<byte> data, Func<T> _default)
		{
			try
			{
				return MessagePackSerializer.Deserialize<T>(data, defaultOptions);
			}
			catch(Exception ex)
			{
				LogEx(ex);
				return _default != null ? _default() : default;
			}
		} 
	}
}
