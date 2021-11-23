// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1649 // File name should match first type name

namespace Resolve2
{

	namespace MessagePack.Resolvers
	{
		using System;

		public class GeneratedResolver : global::MessagePack.IFormatterResolver
		{
			public static readonly global::MessagePack.IFormatterResolver Instance = new GeneratedResolver();

			private GeneratedResolver()
			{
			}

			public global::MessagePack.Formatters.IMessagePackFormatter<T> GetFormatter<T>()
			{
				return FormatterCache<T>.Formatter;
			}

			private static class FormatterCache<T>
			{
				internal static readonly global::MessagePack.Formatters.IMessagePackFormatter<T> Formatter;

				static FormatterCache()
				{
					var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
					if (f != null)
					{
						Formatter = (global::MessagePack.Formatters.IMessagePackFormatter<T>) f;
					}
				}
			}
		}

		internal static class GeneratedResolverGetFormatterHelper
		{
			private static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

			static GeneratedResolverGetFormatterHelper()
			{
				lookup = new global::System.Collections.Generic.Dictionary<Type, int>(2)
				{
					{ typeof(global::CnVShared.ICnVChatClientConnection.JoinAsyncArgs), 0 },
					{ typeof(global::COTG.CityCustom), 1 },
				};
			}

			internal static object GetFormatter(Type t)
			{
				int key;
				if (!lookup.TryGetValue(t, out key))
				{
					return null;
				}

				switch (key)
				{
					case 0:
						return new MessagePack.Formatters.CnVShared.ICnVChatClientConnection_JoinAsyncArgsFormatter();
					case 1: return new MessagePack.Formatters.COTG.CityCustomFormatter();
					default: return null;
				}
			}
		}
	}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1649 // File name should match first type name




// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

	namespace MessagePack.Formatters.CnVShared
	{
		using global::System.Buffers;
		using global::MessagePack;

		public sealed class CnVJsonMessagePackFormatter<T> : global::MessagePack.Formatters.IMessagePackFormatter<
			global::CnVShared.CnVJsonMessagePack<T>>
		{

			public void Serialize(ref global::MessagePack.MessagePackWriter writer,
				global::CnVShared.CnVJsonMessagePack<T> value, global::MessagePack.MessagePackSerializerOptions options)
			{
				writer.WriteArrayHeader(1);
				writer.Write(value.json);
			}

			public global::CnVShared.CnVJsonMessagePack<T> Deserialize(ref global::MessagePack.MessagePackReader reader,
				global::MessagePack.MessagePackSerializerOptions options)
			{
				if (reader.TryReadNil())
				{
					throw new global::System.InvalidOperationException("typecode is null, struct not supported");
				}

				options.Security.DepthStep(ref reader);
				var length = reader.ReadArrayHeader();
				var ____result = new global::CnVShared.CnVJsonMessagePack<T>();

				for (int i = 0; i < length; i++)
				{
					switch (i)
					{
						case 0:
							____result.json = reader.ReadBytes()?.ToArray();
							break;
						default:
							reader.Skip();
							break;
					}
				}

				reader.Depth--;
				return ____result;
			}
		}

		public sealed class ICnVChatClientConnection_JoinAsyncArgsFormatter : global::MessagePack.Formatters.
			IMessagePackFormatter<global::CnVShared.ICnVChatClientConnection.JoinAsyncArgs>
		{

			public void Serialize(ref global::MessagePack.MessagePackWriter writer,
				global::CnVShared.ICnVChatClientConnection.JoinAsyncArgs value,
				global::MessagePack.MessagePackSerializerOptions options)
			{
				if (value == null)
				{
					writer.WriteNil();
					return;
				}

				global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
				writer.WriteArrayHeader(4);
				formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.playerName, options);
				writer.Write(value.world);
				formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.alliance, options);
				formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.allianceRole, options);
			}

			public global::CnVShared.ICnVChatClientConnection.JoinAsyncArgs Deserialize(
				ref global::MessagePack.MessagePackReader reader,
				global::MessagePack.MessagePackSerializerOptions options)
			{
				if (reader.TryReadNil())
				{
					return null;
				}

				options.Security.DepthStep(ref reader);
				global::MessagePack.IFormatterResolver formatterResolver = options.Resolver;
				var length = reader.ReadArrayHeader();
				var ____result = new global::CnVShared.ICnVChatClientConnection.JoinAsyncArgs();

				for (int i = 0; i < length; i++)
				{
					switch (i)
					{
						case 0:
							____result.playerName = formatterResolver.GetFormatterWithVerify<string>()
								.Deserialize(ref reader, options);
							break;
						case 1:
							____result.world = reader.ReadInt32();
							break;
						case 2:
							____result.alliance = formatterResolver.GetFormatterWithVerify<string>()
								.Deserialize(ref reader, options);
							break;
						case 3:
							____result.allianceRole = formatterResolver.GetFormatterWithVerify<string>()
								.Deserialize(ref reader, options);
							break;
						default:
							reader.Skip();
							break;
					}
				}

				reader.Depth--;
				return ____result;
			}
		}
	}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name

// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

	namespace MessagePack.Formatters.COTG
	{
		using global::System.Buffers;
		using global::MessagePack;

		public sealed class
			CityCustomFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::COTG.CityCustom>
		{

			public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::COTG.CityCustom value,
				global::MessagePack.MessagePackSerializerOptions options)
			{
				writer.WriteArrayHeader(2);
				writer.Write(value.cid);
				writer.Write(value.pinned);
			}

			public global::COTG.CityCustom Deserialize(ref global::MessagePack.MessagePackReader reader,
				global::MessagePack.MessagePackSerializerOptions options)
			{
				if (reader.TryReadNil())
				{
					throw new global::System.InvalidOperationException("typecode is null, struct not supported");
				}

				options.Security.DepthStep(ref reader);
				var length = reader.ReadArrayHeader();
				var ____result = new global::COTG.CityCustom();

				for (int i = 0; i < length; i++)
				{
					switch (i)
					{
						case 0:
							____result.cid = reader.ReadInt32();
							break;
						case 1:
							____result.pinned = reader.ReadBoolean();
							break;
						default:
							reader.Skip();
							break;
					}
				}

				reader.Depth--;
				return ____result;
			}
		}
	}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name

}