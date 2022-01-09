using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Interactions;
using DiscordBot.Models;
using DiscordBot.Controllers;

namespace DiscordBot
{
    class Program
    {
        static Task Main(string[] args) => new Program().RunBot();
        
        // Creating the necessary variables
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private InteractionService _interactionService;
        private LoggingService _loggingService;
        private ConfigModel config;

        // Runbot task
        public async Task RunBot()
        {
            var socketConfig = new DiscordSocketConfig() // Set up socket config
            {
                GatewayIntents = GatewayIntents.AllUnprivileged,
                AlwaysDownloadUsers = true             
            };

            var interactionConfig = new InteractionServiceConfig() // Set up interaction service config
            {
                DefaultRunMode = Discord.Interactions.RunMode.Async,
                UseCompiledLambda = true
            };

            _client = new DiscordSocketClient(socketConfig); // Define _client
            _interactionService = new InteractionService(_client, interactionConfig); // Define _interactionService
            _loggingService = new LoggingService(_client, _interactionService); // Define _loggingService
            _services = new ServiceCollection() // Define _services
                .AddSingleton(_client)
                .AddSingleton(_interactionService)
                .AddSingleton(_loggingService)
                .AddSingleton<InteractionHandler>()
                .AddTransient<GameSaveController>()
                .BuildServiceProvider();

            // Read config.json into ConfigModel object
            config = JsonSerializer.Deserialize<ConfigModel>(File.ReadAllText("config.json"));
            
            try
            {
                await _services.GetService<InteractionHandler>().InitializeAsync();              
                await _client.LoginAsync(TokenType.Bot, config.Token); // Log into the bot user
                await _client.StartAsync(); // Start the bot user
                await _client.SetGameAsync(config.Game); // Set the game the bot is playing
                _client.Ready += Client_Ready;
                await Task.Delay(-1); // Delay for -1 to keep the console window opened
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }         
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
