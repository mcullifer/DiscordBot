using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DiscordBot.Models
{
    public class GameSaveRootObject
    {
        [JsonPropertyName("Saves")]
        public List<SaveModel> Saves { get; set; } = new();
    }

    public class SaveModel
    {
        [JsonPropertyName("SaveName")]
        public string SaveName { get; set; }

        [JsonPropertyName("SaveDate")]
        public string SaveDate { get; set; }

        [JsonPropertyName("Players")]
        public List<PlayerModel> Players { get; set; } = new();
    }

    public class PlayerModel
    {
        [JsonPropertyName("Player")]
        public string Name { get; set; }

        [JsonPropertyName("Country")]
        public string Country { get; set; }
    }
}
