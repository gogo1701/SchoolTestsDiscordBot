using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBusinessLayer
{
    public class EmbedManager
    {
        public DiscordEmbed GenerateErrorEmbed(string title, string description)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = DiscordColor.Red,
                Footer = new()
                {
                    Text = "Developed by georgi170109"
                }
            };

            return embed.Build();
        }
    }
}
