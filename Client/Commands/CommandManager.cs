using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core.Native;
using NFive.Client.Events;
using NFive.SDK.Client.Chat;
using NFive.SDK.Client.Rpc;
using NFive.SDK.Core.Arguments;
using NFive.SDK.Core.Chat;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Core.Rpc;

namespace NFive.Client.Commands
{
	public class CommandManager
	{
		public class Subscription
		{
			private readonly Delegate handler;

			public Subscription(Delegate handler)
			{
				this.handler = handler;
			}

			public void Handle(IEnumerable<string> args) => this.handler.DynamicInvoke(args);
		}

		private Dictionary<string, Subscription> Callbacks { get; set; } = new Dictionary<string, Subscription>();

		public CommandManager(IRpcHandler rpc)
		{
			rpc.Event(RpcEvents.ChatSendMessage).On<ChatMessage>(Handle);
		}

		public void Register<T>(string command, Action<T> callback)
		{
			this.Callbacks.Add(command.ToLowerInvariant(), new Subscription(new Action<IEnumerable<string>>(args =>
			{
				callback(Argument.Parse<T>(args));
			})));
		}

		public void Register(string command, Action callback)
		{
			this.Callbacks.Add(command.ToLowerInvariant(), new Subscription(callback));
		}

		private void Handle(IRpcEvent e, ChatMessage message)
		{
			var content = message.Content.Trim();
			if (!content.StartsWith("/")) return;

			var commandArgs = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var command = commandArgs.First().Substring(1);
			if (!this.Callbacks.ContainsKey(command.ToLowerInvariant())) return;

			this.Callbacks[command].Handle(commandArgs.Skip(1).ToList());
		}
	}
}
