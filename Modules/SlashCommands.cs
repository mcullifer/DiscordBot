using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Attributes;
using DiscordBot.Controllers;

namespace DiscordBot.Modules
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public DiscordSocketClient Client { get; set; }
        public InteractionService InteractionService { get; set; }
        public GameSaveController GameSave { get; set; }
        public SlashCommands(DiscordSocketClient client, InteractionService interactionService, GameSaveController gameSave)
        {
            Client = client;
            InteractionService = interactionService;
            GameSave = gameSave;
        }

        [SlashCommand("create-save", "Create a new CK save file", runMode: Discord.Interactions.RunMode.Async)]        
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
                    await Context.Interaction.RespondAsync("You must have the same number of countries as players");
                    return;
                }

                if (playersList[0].Length > 75 | countriesList[0].Length > 75)
                {
                    await Context.Interaction.RespondAsync("Stop trolling");
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
                
                var embedBuiler = new EmbedBuilder() // Build embed
                    .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                    .WithTitle($"- New CK3 Save -\n{saveName}")
                    .WithDescription(description)
                    .WithColor(Color.Blue)
                    .WithCurrentTimestamp();               
                
                var newSave = GameSaveController.BuildSave(playerdict, saveName, Context.Interaction.CreatedAt); // Create save

                await GameSave.AddGameSave(newSave); // Write save to disk
                // Respond with embed
                await Context.Interaction.ModifyOriginalResponseAsync((message) => message.Embed = embedBuiler.Build());
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
