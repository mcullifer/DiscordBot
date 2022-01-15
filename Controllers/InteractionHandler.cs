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
            _client.InteractionCreated += HandleInteraction;

            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services); // Add module to _interactionService
			
		}

        public async Task HandleInteraction(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, _services);
        }
    }
}
