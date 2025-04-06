using DSharpPlus;
using DSharpPlus.Entities;

namespace DiscordBusinessLayer
{
    class EmbedManager
    {
        DiscordEmbed generateErrorEmbed(string title, string description)
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

            return embed;
        }
    }
}
