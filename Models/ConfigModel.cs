namespace DiscordBot.Models
{
    public class ConfigModel
    {
        public string Prefix { get; set; }
        public string Token { get; set; }
        public string Game { get; set; }
        public ulong GuildID { get; set; }
    }
}
