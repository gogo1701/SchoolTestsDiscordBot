using DiscordBotClasses.Enums;
using DiscordBotClasses.Models;
using DiscordBusinessLayer;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBotSlashCommands
{
    public class TestCommands : ApplicationCommandModule
    {
        private readonly ulong _allowedEditorId = 1358351354044878969;

        private readonly TestManager _testManager = new();
        private readonly EmbedManager _embedManager = new();
        private readonly RoleManager _roleManager;

        public TestCommands()
        {
            _roleManager = new RoleManager(_allowedEditorId);
        }

        [SlashCommand("viewtests", "View all tests for week.")]
        public async Task ViewAllTests(InteractionContext ctx)
        {
            IList<Test> tests = _testManager.GetTests();

            var embed = new DiscordEmbedBuilder
            {
                Title = "Tests for this Week",
                Description = "Here are the tests for this week:",
                Color = DiscordColor.Blurple,
                Footer = new()
            };

            embed.Footer.Text = "Developed by georgi170109";

            var groupedTests = tests
               .GroupBy(t => t.DayOfWeek)
               .OrderBy(g => g.Key)
               .ToList();

            foreach (var group in groupedTests)
            {
                string dayText = "";

                foreach (var test in group)
                {
                    dayText += $"**TestId**: {test.Id}\n**Test Name**: {test.Name}\n**Subject**: {test.Subject}\n**Type of Test**: {test.TypeOfTest}\n\n";
                }

                embed.AddField(group.Key.ToString(), dayText, false);
            }

            await ctx.CreateResponseAsync(embed: embed.Build());
        }


        [SlashCommand("addtest", "Add a new test to the database")]
        public async Task AddTestCommand(InteractionContext ctx,
    [Option("name", "The name of the test")] string name,
    [Option("subject", "The subject of the test (Programming, Robotics, etc.)")] Subjects subject,
    [Option("type", "The type of the test (ControlTest, ClassTest, etc.)")] TypesOfTest type,
    [Option("day", "The day of the week for the test (Monday, Tuesday, etc.)")] DaysOfWeek day,
    [Option("description", "A short description of the test")] string description = "")
        {

            if (_roleManager.DoesUserHaveEditorRole(ctx.Member))
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("No permissions", "User does not have role that is assigned to editors."));
                return;
            }

            var newTest = new Test
            {
                Name = name,
                Subject = subject,
                TypeOfTest = type,
                DayOfWeek = day,
                Author = ctx.User.Id,
                Description = description
            };

            try
            {
                await _testManager.AddTest(newTest);

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Test Added Successfully",
                    Description = $"Test **{name}** has been added for **{subject}** on **{day}**!",
                    Color = DiscordColor.Green,
                    Footer = new()
                };

                embed.Footer.Text = "Developed by georgi170109";
                embed.AddField("Type of Test", type.ToString(), true);
                embed.AddField("Day of the Week", day.ToString(), true);

                if (!string.IsNullOrEmpty(description))
                {
                    embed.AddField("Description", description, false);
                }

                await ctx.CreateResponseAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Failed to add test.", $"Failed to add the test. Error: {ex.Message}"));
            }
        }


        [SlashCommand("viewtestsbyday", "View tests for a specific day of the week")]
        public async Task ViewTestsForDayCommand(InteractionContext ctx, [Option("day", "Choose a day of the week")] DaysOfWeek dayOfWeek)
        {
            IList<Test> tests = _testManager.GetTestsByDay(dayOfWeek);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Tests for {dayOfWeek}",
                Description = $"Here are the tests for {dayOfWeek}:",
                Color = DiscordColor.Blurple,
                Footer = new()
            };

            embed.Footer.Text = "Developed by georgi170109";

            if (tests.Any())
            {
                foreach (var test in tests)
                {
                    var testText = $"**TestId**: {test.Id}\n**Subject**: {test.Subject}\n**Type of Test**: {test.TypeOfTest}";
                    embed.AddField(test.Name, testText, false);
                }
            }
            else
            {
                embed.Description = $"No tests found for {dayOfWeek}.";
            }

            await ctx.CreateResponseAsync(embed: embed.Build());
        }


        [SlashCommand("viewtestsbysubject", "View tests for a specific subject")]
        public async Task ViewTestsForSubjectCommand(InteractionContext ctx, [Option("Subject", "Choose a subject")] Subjects subject)
        {
            IList<Test> tests = _testManager.GetTestsBySubject(subject);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Tests for {subject}",
                Description = $"Here are the tests for {subject}:",
                Color = DiscordColor.Blurple,
                Footer = new()
            };

            embed.Footer.Text = "Developed by georgi170109";

            if (tests.Any())
            {
                foreach (var test in tests)
                {
                    var testText = $"**TestId**: {test.Id}\n**Subject**: {test.Subject}\n**Type of Test**: {test.TypeOfTest}";
                    embed.AddField(test.Name, testText, false);
                }
            }
            else
            {
                embed.Description = $"No tests found for {subject}.";
            }

            await ctx.CreateResponseAsync(embed: embed.Build());
        }

        [SlashCommand("viewtestdetails", "View all details of a specific test")]
        public async Task ViewTestDetailsCommand(InteractionContext ctx, [Option("test-id", "The ID of the test to view details for")] long testId)
        {
            Test test = _testManager.GetTestById((int)testId);

            if (test == null)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Failed to view test details.", $"Test was not found for given id."));
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Test Details for {test.Name}",
                Description = $"Here are all the details for the test **{test.Name}**.",
                Color = DiscordColor.Blurple,
                Footer = new()
            };

            embed.Footer.Text = "Developed by georgi170109";

            embed.AddField("Test ID", test.Id.ToString(), true);
            if (test.Description != "")
            {
                embed.AddField("Description", test.Description, false);
            }
            embed.AddField("Subject", test.Subject.ToString(), true);
            embed.AddField("Type of Test", test.TypeOfTest.ToString(), true);
            embed.AddField("Day of the Week", test.DayOfWeek.ToString(), true);

            // Add Example Tests (if any)
            if (test.ExampleTests.Any())
            {
                string exampleTestsText = string.Join("\n", test.ExampleTests.Select(et => $"- {et.Link}"));
                embed.AddField("Example Tests", exampleTestsText, false);
            }
            else
            {
                embed.AddField("Example Tests", "No example tests available.", false);
            }

            if (test.Documents.Any())
            {
                string documentsText = string.Join("\n", test.Documents.Select(d => $"- {d.Link}"));
                embed.AddField("Documents", documentsText, false);
            }
            else
            {
                embed.AddField("Documents", "No documents available.", false);
            }

            await ctx.CreateResponseAsync(embed: embed.Build());
        }

        [SlashCommand("deletetest", "Delete a specific test from the database")]
        public async Task DeleteTestCommand(InteractionContext ctx, [Option("test-id", "The ID of the test to delete")] long testId)
        {

            if (_roleManager.DoesUserHaveEditorRole(ctx.Member))
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("No permissions", "User does not have role that is assigned to editors."));
                return;
            }

            Test test = _testManager.GetTestById((int)testId);

            if (test == null)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Failed to delete test.", $"Test with given id was not found."));
                return;
            }

            try
            {
                _testManager.DeleteTest(test);

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Test Deleted",
                    Description = $"The test **{test.Name}** has been successfully deleted.",
                    Color = DiscordColor.Green,
                    Footer = new()
                    {
                        Text = "Developed by georgi170109"
                    }

                };

                await ctx.CreateResponseAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Error Deleting Test",
                    Description = $"An error occurred while trying to delete the test: {ex.Message}",
                    Color = DiscordColor.Red,
                    Footer = new()
                    {
                        Text = "Developed by georgi170109"
                    }


                };
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Error deleting test.", $"An error occurred while trying to delete the test: {ex.Message}"));

                await ctx.CreateResponseAsync(embed: embed.Build());
            }
        }

        [SlashCommand("deletealltests", "Delete all tests from the database (admin only)")]
        public async Task DeleteAllTestsCommand(InteractionContext ctx)
        {
            var member = await ctx.Guild.GetMemberAsync(ctx.User.Id);

            if (member == null || _roleManager.DoesUserHaveAdminPerms(ctx.Member))
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Failed to delete all tests.", $"User does not have admin permissions."));
            }


            try
            {
                _testManager.DeleteAllTests();

                var embed = new DiscordEmbedBuilder
                {
                    Title = "All Tests Deleted",
                    Description = "All tests have been successfully deleted from the database.",
                    Color = DiscordColor.Green,
                    Footer = new()
                    {
                        Text = "Developed by georgi170109"
                    }
                };

                await ctx.CreateResponseAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Failed to delete tests.", $"An error occurred while trying to delete all tests: {ex.Message}"));
            }
        }

        [SlashCommand("adddocument", "Add a document to a specific test")]
        public async Task AddDocumentCommand(InteractionContext ctx,
                                     [Option("test-id", "The ID of the test to add the document to")] long testId,
                                     [Option("document-link", "The link to the document")] string documentLink)
        {

            if (_roleManager.DoesUserHaveEditorRole(ctx.Member))
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("No permissions", "User does not have role that is assigned to editors."));
                return;
            }

            Test test = _testManager.GetTestById((int)testId);

            if (test == null)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Test was not found.", "Test for given id was not found."));
                return;
            }

            try
            {
                Document newDocument = new Document
                {
                    Link = documentLink,
                    Author = ctx.User.Id,
                    TestId = test.Id
                };

                _testManager.AddDocument(newDocument);

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Document Added",
                    Description = $"The document with link **{documentLink}** has been successfully added to the test **{test.Name}**.",
                    Color = DiscordColor.Green,
                    Footer = new()
                    {
                        Text = "Developed by georgi170109"
                    }
                };

                await ctx.CreateResponseAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Error Adding Document", $"An error occurred while adding the document: {ex.Message}"));
            }
        }

        [SlashCommand("addexampletest", "Add an example test to a specific test")]
        public async Task AddExampleTestCommand(InteractionContext ctx,
                                        [Option("test-id", "The ID of the test to add the example test to")] long testId,
                                        [Option("example-link", "The link to the example test")] string exampleTestLink,
                                        [Option("scheduled-at", "The scheduled date and time for the example test (in Discord timestamp format)")] string scheduledAt)
        {

            if (_roleManager.DoesUserHaveEditorRole(ctx.Member))
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("No permissions", "User does not have role that is assigned to editors."));
                return;
            }

            Test test = _testManager.GetTestById((int)testId);

            if (test == null)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Test was not found.", "Test for given id was not found."));
                return;
            }

            try
            {
                if (long.TryParse(scheduledAt.Trim('<', 't', ':', '>', 'F'), out long unixTimestamp))
                {
                    DateTime scheduledDateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;

                    ExampleTest newExampleTest = new ExampleTest
                    {
                        Link = exampleTestLink,
                        ScheduledAt = scheduledDateTime,
                        Author = ctx.User.Id,
                        TestId = test.Id
                    };

                    _testManager.AddExampleTest(newExampleTest);


                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Example Test Added",
                        Description = $"The example test with link **{exampleTestLink}** has been successfully added to the test **{test.Name}**.",
                        Color = DiscordColor.Green,
                        Footer = new()
                        {
                            Text = "Developed by georgi170109"
                        }
                    };

                    await ctx.CreateResponseAsync(embed: embed.Build());
                }
                else
                {
                    await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Timestamp error.", "Invalid timestamp format. Please provide a valid Discord timestamp. (Make sure you are using the fullest date possible so that the unix stamp ends with :F>)"));
                }
            }
            catch (Exception ex)
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Error Adding Example Test", $"An error occurred while adding example test: {ex.Message}"));
            }
        }

        [SlashCommand("viewdocuments", "View all documents of a specific test")]
        public async Task ViewDocumentsCommand(InteractionContext ctx,
                                       [Option("test-id", "The ID of the test to view the documents of")] long testId)
        {
            Test test = _testManager.GetTestById((int)testId);

            if (test == null)
            {
                var errorembed = new DiscordEmbedBuilder
                {
                    Title = "Test was not found.",
                    Description = $"An error occurred while viewing documents: Test was not found.",
                    Color = DiscordColor.Red,
                    Footer = new()
                    {
                        Text = "Developed by georgi170109"
                    }
                };

                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Test was not found.", $"An error occurred while viewing documents: Test was not found."));


                await ctx.CreateResponseAsync(embed: errorembed.Build()); return;
            }

            if (test.Documents == null || !test.Documents.Any())
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("No documents.", "No documents were found for given test."));
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Documents for {test.Name}",
                Description = $"Here are all the documents for the test **{test.Name}**:",
                Color = DiscordColor.Blurple,
                Footer = new()
                {
                    Text = "Developed by georgi170109"
                }
            };

            foreach (var document in test.Documents)
            {
                string documentInfo = $"**Link**: {document.Link}\n**Author**: <@{document.Author}>";

                embed.AddField($"Document #{document.Id}", documentInfo, false);
            }

            await ctx.CreateResponseAsync(embed: embed.Build());
        }
        [SlashCommand("viewexampletests", "View all example tests of a specific test")]
        public async Task ViewExampleTestsCommand(InteractionContext ctx,
                                      [Option("test-id", "The ID of the test to view the example tests of")] long testId)
        {
            Test test = _testManager.GetTestById((int)testId);

            if (test == null)
            {
                var errorembed = new DiscordEmbedBuilder
                {
                    Title = "Error Viewing Example Test",
                    Description = $"An error occurred while example tests: Test was not found.",
                    Color = DiscordColor.Red,
                    Footer = new()
                    {
                        Text = "Developed by georgi170109"
                    }
                };

                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("Test was not found.", "Test for given testid was not found."));


                await ctx.CreateResponseAsync(embed: errorembed.Build()); return;
            }

            if (test.ExampleTests == null || !test.ExampleTests.Any())
            {
                await ctx.CreateResponseAsync(embed: _embedManager.GenerateErrorEmbed("No example tests.", "No example tests were found for given test."));
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Example Tests for {test.Name}",
                Description = $"Here are all the example tests for the test **{test.Name}**:",
                Color = DiscordColor.Blurple,
                Footer = new()
                {
                    Text = "Developed by georgi170109"
                }
            };

            foreach (var exampleTest in test.ExampleTests)
            {
                string exampleTestInfo = $"**Link**: {exampleTest.Link}\n**Scheduled At**: <t:{((DateTimeOffset)exampleTest.ScheduledAt).ToUnixTimeSeconds()}:f>\n**Author**: <@{exampleTest.Author}>";

                embed.AddField($"Example Test #{exampleTest.Id}", exampleTestInfo, false);
            }

            await ctx.CreateResponseAsync(embed: embed.Build());
        }
    }
}
