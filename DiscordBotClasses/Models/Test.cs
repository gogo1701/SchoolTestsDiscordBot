using DiscordBotClasses.Enums;

namespace DiscordBotClasses.Models
{
    public class Test
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Subjects Subject { get; set; }

        public string Description { get; set; }

        public DaysOfWeek DayOfWeek { get; set; }

        public TypesOfTest TypeOfTest { get; set; }

        public ulong Author { get; set; } 

        // nav properties
        public IList<Document> Documents { get; set; } 
        public IList<ExampleTest> ExampleTests { get; set; }
    }
}
