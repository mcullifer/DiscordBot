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
        private InteractionService _interactionService;
        private LoggingService _loggingService;
        private ConfigModel _config;
        private IServiceProvider _services;

        // Runbot task
        public async Task RunBot()
        {
            // Read config.json into ConfigModel object
            _config = JsonSerializer.Deserialize<ConfigModel>(File.ReadAllText("config.json"));

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
                .AddSingleton(_config)
                .AddSingleton<InteractionHandler>()
                .AddSingleton<ClientController>()
                .AddTransient<GameSaveController>()
                .BuildServiceProvider();
            
            try
            { // Initialize handlers
                await _services.GetService<InteractionHandler>().InitializeAsync();
                await _services.GetService<ClientController>().InitializeAsync();
                await Task.Delay(-1); // Delay for -1 to keep the console window opened
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }         
        }  
    }
}
