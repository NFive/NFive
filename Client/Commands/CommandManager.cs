using NFive.SDK.Client.Commands;
using NFive.SDK.Client.Rpc;
using NFive.SDK.Core.Arguments;
using NFive.SDK.Core.Chat;
using NFive.SDK.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NFive.Client.Commands
{
	public class CommandManager : ICommandManager
	{
		private readonly Dictionary<string, Action<IEnumerable<string>>> callbacks = new Dictionary<string, Action<IEnumerable<string>>>();

		public CommandManager(IRpcHandler rpc)
		{
			rpc.Event(RpcEvents.ChatSendMessage).On<ChatMessage>(Handle);
		}

		public void Register(string command, Action callback)
		{
			this.callbacks.Add(command.ToLowerInvariant(), a => callback());
		}

		public void Register(string command, Action<string> callback)
		{
			this.callbacks.Add(command.ToLowerInvariant(), a => callback(string.Join(" ", a)));
		}

		public void Register(string command, Action<IEnumerable<string>> callback)
		{
			this.callbacks.Add(command.ToLowerInvariant(), callback);
		}

		public void Register<T>(string command, Action<T> callback)
		{
			this.callbacks.Add(command.ToLowerInvariant(), a => callback(Argument.Parse<T>(a)));
		}

		private void Handle(IRpcEvent e, ChatMessage message)
		{
			var content = message.Content.Trim();
			if (!content.StartsWith("/")) return;

			var commandArgs = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var command = commandArgs.First().Substring(1).ToLowerInvariant();
			if (!this.callbacks.ContainsKey(command)) return;

			this.callbacks[command](commandArgs.Skip(1));
		}
	}
}
