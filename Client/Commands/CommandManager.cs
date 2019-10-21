using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NFive.Client.Rpc;
using NFive.SDK.Client.Commands;
using NFive.SDK.Client.Communications;
using NFive.SDK.Core.Chat;
using NFive.SDK.Core.Events;

namespace NFive.Client.Commands
{
	[PublicAPI]
	public class CommandManager : ICommandManager
	{
		private readonly Dictionary<string, Action<IEnumerable<string>>> subscriptions = new Dictionary<string, Action<IEnumerable<string>>>();

		public CommandManager()
		{
			RpcManager.On<ChatMessage>(CoreEvents.ChatSendMessage, OnChatSendMessage);
		}

		public void On(string command, Action action)
		{
			this.subscriptions.Add(command.ToLowerInvariant(), a => action());
		}

		public void On(string command, Action<string> action)
		{
			this.subscriptions.Add(command.ToLowerInvariant(), a => action(string.Join(" ", a)));
		}

		public void On(string command, Action<IEnumerable<string>> action)
		{
			this.subscriptions.Add(command.ToLowerInvariant(), action);
		}

		// TODO: Off()s

		private void OnChatSendMessage(ICommunicationMessage e, ChatMessage message)
		{
			var content = message.Content.Trim();
			if (!content.StartsWith("/")) return;

			var commandArgs = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			var command = commandArgs.First().Substring(1).ToLowerInvariant();
			if (!this.subscriptions.ContainsKey(command)) return;

			this.subscriptions[command](commandArgs.Skip(1));
		}
	}
}
