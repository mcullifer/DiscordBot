using System;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Discord;
using Discord.Net;
using Discord.Commands;
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
                AutoServiceScopes = true,
                UseCompiledLambda = true,
                EnableAutocompleteHandlers = true
            };

            _client = new DiscordSocketClient(new DiscordSocketConfig { AlwaysDownloadUsers = true }); // Define _client
            _interactionService = new InteractionService(_client, interactionConfig); // Define _interactionService
            _services = new ServiceCollection() // Define _services
                .AddSingleton(_client)
                .AddSingleton(_interactionService)
                .AddSingleton(GameSaveController.CreateAsync())
                .BuildServiceProvider();

            _client.Log += Log; // Logging

            // Read config.json into ConfigModel object
            config = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText("config.json"));

            try
            {
                await RegisterCommandsAsync(); // Call registercommands
                await _client.LoginAsync(TokenType.Bot, config.Token); // Log into the bot user
                await _client.StartAsync(); // Start the bot user
                await _client.SetGameAsync(config.Game); // Set the game the bot is playing
                await Task.Delay(-1); // Delay for -1 to keep the console window opened
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }         
        }

        private async Task RegisterCommandsAsync()
        {
            _client.Ready += Client_Ready;
            _client.InteractionCreated += async x =>
            {
                var context = new SocketInteractionContext(_client, x);
                await _interactionService.ExecuteCommandAsync(context, _services);
            };
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services); // Add module to _interactionService
        }

        public async Task Client_Ready()
        {
            try
            {
#if DEBUG
                await _interactionService.RegisterCommandsToGuildAsync(config.GuildID); // Call RegisterInteractionCommands
#else
                var guildCommands = Array.Empty<ApplicationCommandProperties>();
                await _client.Rest.BulkOverwriteGuildCommands(guildCommands, config.guildID);     
#endif
            }
            catch (HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                Console.WriteLine("ERROR REGISTERING COMMAND:");
                Console.WriteLine(json);
            }
        }

        private Task Log(LogMessage message) // Logging
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
            {
                Console.WriteLine($"[General/{message.Severity}] {message}");
            }              
            return Task.CompletedTask;
        }       
    }
}
