using System;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Models;

namespace DiscordBot.Controllers
{
    public class ClientController
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        private readonly ConfigModel _config;

        public ClientController(DiscordSocketClient client, InteractionService interactionService, ConfigModel config)
        {
            _client = client;
            _interactionService = interactionService;
            _config = config;
        }

        public async Task InitializeAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _config.Token); // Log into the bot user
            await _client.StartAsync(); // Start the bot user
            await _client.SetGameAsync(_config.Game); // Set the game the bot is playing
            _client.Ready += Client_Ready;
        }

        public async Task Client_Ready()
        {
            try
            {
#if DEBUG
                //await _interactionService.RegisterCommandsToGuildAsync(config.GuildID); // Call RegisterInteractionCommands
                await _interactionService.RegisterCommandsGloballyAsync(true);
#else
                var guildCommands = Array.Empty<ApplicationCommandProperties>();
                await _client.Rest.BulkOverwriteGuildCommands(guildCommands, config.guildID);     
#endif
            }
            catch (HttpException exception)
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                var json = JsonSerializer.Serialize(exception.Errors, jsonOptions);
                Console.WriteLine("ERROR REGISTERING COMMAND:");
                Console.WriteLine(json);
            }
        }
    }
}
