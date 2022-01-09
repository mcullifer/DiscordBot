using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Controllers
{
	public class InteractionHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly InteractionService _interactionService;
		private readonly IServiceProvider _services;

		public InteractionHandler(IServiceProvider services, DiscordSocketClient client, InteractionService interactionService)
		{
			_interactionService = interactionService;
			_services = services;
			_client = client;
		}

		public async Task InitializeAsync()
		{
			await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services); // Add module to _interactionService
			_client.InteractionCreated += async x =>
			{
				var context = new SocketInteractionContext(_client, x);
				await _interactionService.ExecuteCommandAsync(context, _services);
			};
		}
	}
}
