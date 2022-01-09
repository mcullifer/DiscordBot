using System.Text.Json.Serialization;

namespace DiscordBot.Models
{
    public class ConfigModel
    {
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("game")]
        public string Game { get; set; }

        [JsonPropertyName("guildID"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public ulong GuildID { get; set; }
    }
}
