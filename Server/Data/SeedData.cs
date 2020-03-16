using ForumSPA.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // If true, hubs is not already seeded
                if (!context.Hubs.Any())
                {
                    context.Hubs.AddRange(
                        new Hub()
                        {
                            Name = "General"
                        },
                        new Hub()
                        {
                            Name = "Fight Club",
                            Description = "Fight!",
                            Rules = "Rule 1: You do not talk about Fight Club."
                        });
                    context.SaveChanges();
                }

                if (!context.Threads.Any())
                {
                    context.Threads.AddRange(
                        new Thread()
                        {
                            Name = "Hello World!",
                            HubId = 1
                        },
                        new Thread()
                        {
                            Name = "Goodbye World!",
                            HubId = 1
                        });
                    context.SaveChanges();
                }

                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Post()
                        {
                            Body = "How are you?",
                            ThreadId = 1,
                            IsMainPost = true
                        },
                        new Post()
                        {
                            Body = "I'm fine, thank you!",
                            ThreadId = 1,
                            IsMainPost = false
                        },
                        new Post()
                        {
                            Body = "Have a great day!",
                            ThreadId = 2,
                            IsMainPost = true
                        },
                        new Post()
                        {
                            Body = "See you later!",
                            ThreadId = 2,
                            IsMainPost = false
                        });
                    context.SaveChanges();
                }
            }
        }
    }
}
