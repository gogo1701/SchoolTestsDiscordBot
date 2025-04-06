using DiscordBotClasses.Enums;
using DiscordBotClasses.Models;
using DiscordDBConnection;
using Microsoft.EntityFrameworkCore;

namespace DiscordBusinessLayer
{
    public class TestManager
    {
        public IList<Test> GetTests()
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    return context.Tests.Include(t => t.Documents)
                                        .Include(t => t.ExampleTests)
                                        .ToList();
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    return new List<Test>();
                }
            }
        }

        public IList<Test> GetTestsByDay(DaysOfWeek dayOfWeek)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Tests.Where(x => x.DayOfWeek == dayOfWeek)
                                    .Include(t => t.Documents)
                                    .Include(t => t.ExampleTests)
                                    .ToList();

            }
        }

        public IList<Test> GetTestsBySubject(Subjects subject)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Tests.Where(x => x.Subject == subject)
                                    .Include(t => t.Documents)
                                    .Include(t => t.ExampleTests)
                                    .ToList();
            }
        }

        public Test GetTestById(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    return context.Tests.Include(t => t.Documents)
                                        .Include(t => t.ExampleTests)
                                        .FirstOrDefault(x => x.Id == id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public bool DoesTestExistWithId(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    return context.Tests.Any(x => x.Id == id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public void DeleteTest(Test test)
        {
            using (var context = new ApplicationDbContext())
            {
                var existingTest = context.Tests.Find(test.Id);
                if (existingTest != null)
                {
                    context.Tests.Remove(existingTest); 
                    context.SaveChanges();  
                }
            }
        }
        public void DeleteAllTests()
        {
            using (var context = new ApplicationDbContext())
            {
                var tests = context.Tests.ToList();

                context.Tests.RemoveRange(tests);
                context.SaveChanges(); 
            }
        }

        public async Task AddTest(Test test)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    context.Tests.Add(test);
                    await context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task AddDocument(Document document)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    context.Documents.Add(document);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task AddExampleTest(ExampleTest document)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    context.ExampleTests.Add(document);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
