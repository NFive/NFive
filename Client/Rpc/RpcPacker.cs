using Ionic.Zlib;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NFive.Client.Rpc
{
	public class RpcPacker
	{
		private static readonly JsonSerializer Serializer = new JsonSerializer
		{
			Culture = CultureInfo.CurrentCulture,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
			NullValueHandling = NullValueHandling.Ignore
		};

		public static int Level { get; set; } = 6;

		public static byte[] Serialize(object obj)
		{
			using (var jsonStream = new MemoryStream())
			using (var streamWriter = new StreamWriter(jsonStream, new UnicodeEncoding(false, false)))
			{
				Serializer.Serialize(streamWriter, obj);
				streamWriter.Flush();

				return jsonStream.ToArray();
			}
		}

		public static T Deserialize<T>(string json)
		{
			using (var jsonReader = new JsonTextReader(new StringReader(json)))
			{
				return Serializer.Deserialize<T>(jsonReader);
			}
		}

		public static byte[] Pack(OutboundMessage message)
		{
			message.Sent = DateTime.UtcNow;

			var json = Serialize(message);

			using (var compressedStream = new ZlibStream(new MemoryStream(json, false), CompressionMode.Compress, (CompressionLevel)Level, false))
			using (var outputStream = new MemoryStream())
			{
				compressedStream.CopyTo(outputStream);

				return outputStream.ToArray();
			}
		}

		public static InboundMessage Unpack(byte[] data)
		{
			using (var compressionStream = new ZlibStream(new MemoryStream(data, false), CompressionMode.Decompress, false))
			using (var streamReader = new StreamReader(compressionStream, new UnicodeEncoding(false, false)))
			{
				var message = Deserialize<InboundMessage>(streamReader.ReadToEnd());
				message.Received = DateTime.UtcNow;

				return message;
			}
		}
	}
}
