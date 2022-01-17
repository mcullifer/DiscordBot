using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DiscordBot.Models;

namespace DiscordBot.Controllers
{
    public class GameSaveController
    {
        private GameSaveRootObject _gameSaveListModel;

        private readonly string fileName = Directory.GetCurrentDirectory() + "\\GameSaves.json";

        public GameSaveController()
        {
            if (!File.Exists(fileName))
            {
                _gameSaveListModel = new GameSaveRootObject();
                return;
            }
            using FileStream fileStream = File.OpenRead(fileName);
            _gameSaveListModel = JsonSerializer.Deserialize<GameSaveRootObject>(fileStream);
        }

        public static SaveModel BuildSave(Dictionary<string, string> playerdict, string savename, DateTimeOffset savedate)
        {
            var players = new List<PlayerModel>();
            foreach(var key in playerdict.Keys)
            {
                players.Add(new PlayerModel()
                {
                    Name = key.Trim(),
                    Country = playerdict[key].Trim()
                });
            }

            var finalSave = new SaveModel()
            {
                SaveName = savename,
                SaveDate = savedate.ToLocalTime().ToString("MM/dd/yyyy hh:mm tt"),
                Players = players
            };

            return finalSave;
        }
        public async Task AddGameSave(SaveModel newSave)
        {
            _gameSaveListModel.Saves.Add(newSave);
            using FileStream fileStream = File.OpenWrite(Directory.GetCurrentDirectory() + "\\GameSaves.json");
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            await JsonSerializer.SerializeAsync(fileStream, _gameSaveListModel, jsonOptions);
            await fileStream.DisposeAsync();            
        }

        public async Task<GameSaveRootObject> GetGameSaves()
        {
            if (!(_gameSaveListModel.Saves.Count == 0))
            {
                return _gameSaveListModel;
            }
            using FileStream fileStream = File.OpenRead(fileName);
            _gameSaveListModel = await JsonSerializer.DeserializeAsync<GameSaveRootObject>(fileStream);
            return _gameSaveListModel;
        }
    }
}
