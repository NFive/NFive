using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using JetBrains.Annotations;
using NFive.SDK.Client.Interface;
using NFive.SDK.Core.Utilities;

namespace NFive.Client.Interface
{
	[PublicAPI]
	public class NuiManager : INuiManager
	{
		private readonly EventHandlerDictionary events;

		public NuiManager(EventHandlerDictionary events)
		{
			this.events = events;
		}

		public void Focus(bool hasFocus, bool showCursor)
		{
			API.SetNuiFocus(hasFocus, showCursor);
		}

		public void Emit(object data)
		{
			API.SendNuiMessage(new Serializer().Serialize(data));
		}

		public void On(string @event, Action action)
		{
			API.RegisterNuiCallbackType(@event);

			this.events[$"__cfx_nui:{@event}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
			{
				action();

				callback("{}");
			});
		}

		public void On<T>(string @event, Action<T> action)
		{
			API.RegisterNuiCallbackType(@event);

			this.events[$"__cfx_nui:{@event}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
			{
				var serializer = new Serializer();
				var typedData = serializer.Deserialize<T>(serializer.Serialize(data));

				action(typedData);

				callback("{}");
			});
		}

		public void On<TReturn>(string @event, Func<TReturn> action)
		{
			API.RegisterNuiCallbackType(@event);

			this.events[$"__cfx_nui:{@event}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
			{
				var result = action();

				callback(new Serializer().Serialize(result));
			});
		}

		public void On<T, TReturn>(string @event, Func<T, TReturn> action)
		{
			API.RegisterNuiCallbackType(@event);

			this.events[$"__cfx_nui:{@event}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
			{
				var typedData = new Serializer().Deserialize<T>(new Serializer().Serialize(data));

				var result = action(typedData);

				callback(new Serializer().Serialize(result));
			});
		}
	}
}
