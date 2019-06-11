using Ionic.Zlib;
using Newtonsoft.Json;
using NFive.SDK.Core.Diagnostics;
using NFive.Server.Diagnostics;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NFive.Server.Rpc
{
	public class RpcPacker
	{
		private static readonly Logger Logger = new Logger(LogLevel.Trace, "Pack");

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

			using (var inputStream = new MemoryStream(json, false))
			using (var compressedStream = new ZlibStream(inputStream, CompressionMode.Compress, (CompressionLevel)Level, true))
			using (var outputStream = new MemoryStream())
			{
				compressedStream.CopyTo(outputStream);

				var r = outputStream.ToArray();
				Logger.Trace($"Packed {new UnicodeEncoding(false, false).GetString(json).Length} chars, {inputStream.Length} bytes to {r.Length} bytes: {message.Event}");

				return r;
			}
		}

		public static InboundMessage Unpack(byte[] data)
		{
			using (var compressionStream = new ZlibStream(new MemoryStream(data, false), CompressionMode.Decompress, false))
			using (var streamReader = new StreamReader(compressionStream, new UnicodeEncoding(false, false)))
			{
				var json = streamReader.ReadToEnd();

				var message = Deserialize<InboundMessage>(json);
				message.Received = DateTime.UtcNow;

				Logger.Trace($"Unpacked {data.Length} bytes to {json.Length} chars, {new UnicodeEncoding(false, false).GetBytes(json).Length} bytes: {message.Event}");

				return message;
			}
		}

		//private const int CharSize = sizeof(char);

		//public byte[] Pack(string data) => Compress(StringToBytes(data));

		//public string Unpack(byte[] data) => BytesToString(Decompress(data));

		//public static byte[] Compress(byte[] data)
		//{
		//	using (var outputStream = new MemoryStream())
		//	{
		//		using (var compressedStream = new ZlibStream(new MemoryStream(data), CompressionMode.Compress, (CompressionLevel)Level, false))
		//		{
		//			compressedStream.CopyTo(outputStream);
		//		}

		//		return outputStream.ToArray();
		//	}
		//}

		//public byte[] Decompress(byte[] data)
		//{
		//	using (var outputStream = new MemoryStream())
		//	{
		//		using (var compressedStream = new ZlibStream(new MemoryStream(data), CompressionMode.Decompress, false))
		//		{
		//			compressedStream.CopyTo(outputStream);
		//		}

		//		return outputStream.ToArray();
		//	}
		//}

		//public static byte[] StringToBytes(string str)
		//{
		//	if (str == null) throw new ArgumentNullException(nameof(str));
		//	if (str.Length == 0) return new byte[0];

		//	var data = new byte[str.Length * CharSize];

		//	for (var i = 0; i < str.Length; i++)
		//	{
		//		Array.Copy(BitConverter.GetBytes(str[i]), 0, data, i * CharSize, CharSize);
		//	}

		//	return data;
		//}

		//public static string BytesToString(byte[] data)
		//{
		//	if (data == null) throw new ArgumentNullException(nameof(data));
		//	if (data.Length % CharSize != 0) throw new ArgumentException($"Invalid {nameof(data)} length");
		//	if (data.Length == 0) return string.Empty;

		//	unsafe
		//	{
		//		fixed (byte* ptr = data)
		//		{
		//			return new string((char*)ptr, 0, data.Length / CharSize);
		//		}
		//	}
		//}

		//public static byte[] Pack(OutboundMessage message)
		//{
		//	message.Sent = DateTime.UtcNow;

		//	var json = JsonConvert.SerializeObject(message);

		//	var data = StringToBytesFast(json);
		//	var compressed = Compress(data);

		//	return compressed;
		//}

		//public static InboundMessage Unpack(byte[] data)
		//{
		//	var decompressed = Decompress(data);
		//	var json = BytesToStringFast(decompressed);

		//	var message = JsonConvert.DeserializeObject<InboundMessage>(json);
		//	message.Received = DateTime.UtcNow;

		//	return message;
		//}


		//public static byte[] Compress(byte[] data)
		//{
		//	using (var outputStream = new MemoryStream())
		//	{
		//		using (var compressedStream = new ZlibStream(new MemoryStream(data), CompressionMode.Compress, (CompressionLevel)Level, false))
		//		{
		//			compressedStream.CopyTo(outputStream);
		//		}

		//		return outputStream.ToArray();
		//	}
		//}

		//public static byte[] Decompress(byte[] data)
		//{
		//	using (var outputStream = new MemoryStream())
		//	{
		//		using (var compressedStream = new ZlibStream(new MemoryStream(data), CompressionMode.Decompress, false))
		//		{
		//			compressedStream.CopyTo(outputStream);
		//		}

		//		return outputStream.ToArray();
		//	}
		//}

		//public static byte[] StringToBytes(string str)
		//{
		//	if (str == null) throw new ArgumentNullException(nameof(str));
		//	if (str.Length == 0) return new byte[0];

		//	var data = new byte[str.Length * CharSize];

		//	for (var i = 0; i < str.Length; i++)
		//	{
		//		Array.Copy(BitConverter.GetBytes(str[i]), 0, data, i * CharSize, CharSize);
		//	}

		//	return data;
		//}

		//public static byte[] StringToBytesFast(string str)
		//{
		//	if (str == null) throw new ArgumentNullException(nameof(str));
		//	if (str.Length == 0) return new byte[0];

		//	var data = new byte[str.Length * 2];

		//	unsafe
		//	{
		//		fixed (void* ptr = str)
		//		{
		//			Marshal.Copy((IntPtr)ptr, data, 0, data.Length);
		//		}
		//	}

		//	return data;
		//}

		//public static string BytesToString(byte[] data)
		//{
		//	if (data == null) throw new ArgumentNullException(nameof(data));
		//	if (data.Length == 0) return string.Empty;

		//	var chars = new char[data.Length / 2];

		//	for (var i = 0; i < chars.Length; i++)
		//	{
		//		chars[i] = BitConverter.ToChar(data, i * 2);
		//	}

		//	return new string(chars);
		//}

		//public static string BytesToStringFast(byte[] data)
		//{
		//	if (data == null) throw new ArgumentNullException(nameof(data));
		//	if (data.Length % CharSize != 0) throw new ArgumentException($"Invalid {nameof(data)} length");
		//	if (data.Length == 0) return string.Empty;

		//	unsafe
		//	{
		//		fixed (void* ptr = data)
		//		{
		//			return new string((char*)ptr, 0, data.Length / CharSize);
		//		}
		//	}
		//}
	}
}
