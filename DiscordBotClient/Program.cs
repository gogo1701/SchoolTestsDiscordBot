using DiscordBotSlashCommands;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace DiscordBotClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var botToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = botToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<TestCommands>();
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
