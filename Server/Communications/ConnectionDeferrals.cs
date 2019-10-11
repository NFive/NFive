using JetBrains.Annotations;
using NFive.SDK.Server.Communications;

namespace NFive.Server.Communications
{
	[PublicAPI]
	public class ConnectionDeferrals : IConnectionDeferrals
	{
		private readonly dynamic deferrals;

		public string Message
		{
			set => this.deferrals.update(value);
		}

		public ConnectionDeferrals(dynamic deferrals)
		{
			this.deferrals = deferrals;

			//this.Defer(); // TODO: Should we always defer?
		}

		public void Defer()
		{
			this.deferrals.defer();
		}

		public void ShowCard(string json)
		{
			this.deferrals.presentCard(json);
		}

		public void Allow()
		{
			this.deferrals.done();
		}

		public void Reject(string message)
		{
			this.deferrals.done(message);

			// TODO: End session?
		}
	}
}
