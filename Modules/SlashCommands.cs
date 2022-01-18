using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Attributes;
using DiscordBot.Controllers;
using DiscordBot.Models;

namespace DiscordBot.Modules
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly GameSaveController _gameSaveController;
        public SlashCommands(GameSaveController gameSaveController)
        {
            _gameSaveController = gameSaveController;
        }

        [SlashCommand("create-save", "Create a new CK save file")]        
        public async Task CreateSave(
            [Summary(description: "Name of the save")] string saveName, 
            [Summary(description: "Player names comma separated")] string players, 
            [Summary(description: "Countries in the same order as players comma separated")] string countries)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                // Initialize variables               
                var guildUser = Context.User; //command.User;
                List<string> playersList = players.Split(',').ToList();
                List<string> countriesList = countries.Split(',').ToList();
                Dictionary<string, string> playerdict = new();

                if (playersList.Count != countriesList.Count)
                {
                    await Context.Interaction.ModifyOriginalResponseAsync((message) => message.Content = "You must have the same number of countries as players");
                    return;
                }

                if (playersList[0].Length > 75 | countriesList[0].Length > 75)
                {
                    await Context.Interaction.ModifyOriginalResponseAsync((message) => message.Content = "Stop trolling");
                    return;
                }
                
                for (int i = 0; i < playersList.Count; i++) // Fill dictionary of player:country pairs
                {
                    playerdict.Add(playersList[i], countriesList[i]);
                }

                string description = $"Host: {playersList[0]}\n\nPlayers:\n";
                
                foreach (string key in playerdict.Keys) // Build description string of Player As Country pairs
                {
                    description += $"- {key} As {playerdict[key]}\n";
                }
                
                var embedBuilder = new EmbedBuilder() // Build embed
                    .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                    .WithTitle($"- New CK3 Save -\n{saveName}")
                    .WithDescription(description)
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp();               
                
                var newSave = GameSaveController.BuildSave(playerdict, saveName, Context.Interaction.CreatedAt); // Create save

                await _gameSaveController.AddGameSave(newSave); // Write save to disk
                // Respond with embed
                await Context.Interaction.ModifyOriginalResponseAsync((message) => message.Embed = embedBuilder.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [SlashCommand("find-save", "Find a CK save file")]
        public async Task FindSave([Summary(description: "Name of the save host")] string hostName)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var guildUser = Context.User;
                var gameSaves = await _gameSaveController.GetGameSaves(); // Get list of saves
                IEnumerable<SaveModel> foundSaves = gameSaves.Saves.Where((save) => save.Players[0].Name.ToLower() == hostName.ToLower()); // Select saves that match hostName

                if (!foundSaves.Any()) // Exit if no saves exist
                {
                    await Context.Interaction.ModifyOriginalResponseAsync((message) => message.Content = $"No saves found with host: {hostName}");
                    return;
                }
                
                var title = foundSaves.Count() == 1 ? $"{foundSaves.Count()} Save Found With Host: {hostName}" : $"{foundSaves.Count()} Saves Found With Host: {hostName}"; // Select title
                
                var description = string.Empty;
                foreach (var save in foundSaves) // Build save dictionary
                {
                    description += $"{save.SaveName}:\n";
                    foreach (var player in save.Players)
                    {
                        description += $"- {player.Name} As {player.Country}\n";
                    }
                    description += "\n";
                }
                
                var embedBuilder = new EmbedBuilder() // Build embed
                    .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                    .WithTitle(title)
                    .WithDescription(description)
                    .WithColor(Color.Green)
                    .WithCurrentTimestamp();

                await Context.Interaction.ModifyOriginalResponseAsync((message) => message.Embed = embedBuilder.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [SlashCommand("ping", "pong"), Cooldown(1f)]
        public async Task Ping()
        {
            await Context.Interaction.RespondAsync("Pong! 🏓 **" + Context.Client.Latency + "ms**");
        }
    }
}
