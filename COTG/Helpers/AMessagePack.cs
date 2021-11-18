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
	using System.IO;

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
		public static void Serialize<T>(Stream fs, T data)
		{
			MessagePackSerializer.Serialize(fs,data,defaultOptions);
			
		}

		public static T Deserialize<T>(ReadOnlyMemory<byte> data, Func<T> _default)
		{
			try
			{
				if(data.Length>0)
					return MessagePackSerializer.Deserialize<T>(data, defaultOptions);
				else
					Log("Empty MessagePack " + typeof(T) + " " + _default.ToString() );

			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			return _default is not null ? _default() : default;
		}
		public static T Deserialize<T>(Stream data, Func<T> _default)
		{
			try
			{
				if(data.Length>0)
					return MessagePackSerializer.Deserialize<T>(data, defaultOptions);
				else
					Log("Empty MessagePack " + typeof(T) + " " + _default.ToString() );

			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			return _default is not null ? _default() : default;
		}

		internal static T Deserialize<T>(object p) => throw new NotImplementedException();
	}
}
