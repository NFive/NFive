using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NFive.SDK.Client.Commands;
using NFive.SDK.Client.Communications;
using NFive.SDK.Core.Events;

namespace NFive.Client.Commands
{
	[PublicAPI]
	public class CommandManager : ICommandManager
	{
		private readonly Dictionary<string, Action<IEnumerable<string>>> subscriptions = new Dictionary<string, Action<IEnumerable<string>>>();

		public CommandManager(ICommunicationManager comms)
		{
			comms.Event(CoreEvents.CommandDispatch).FromServer().On<List<string>>(OnCommandDispatch);
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

		private void OnCommandDispatch(ICommunicationMessage e, List<string> args)
		{
			var command = args.First().ToLowerInvariant();
			if (!this.subscriptions.ContainsKey(command)) return;

			this.subscriptions[command](args.Skip(1));
		}
	}
}
