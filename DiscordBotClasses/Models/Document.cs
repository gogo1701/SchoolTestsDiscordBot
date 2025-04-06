namespace DiscordBotClasses.Models
{
    public class Document
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        public string Link { get; set; }

        public ulong Author { get; set; }

        public Test Test { get; set; }
    }
}
