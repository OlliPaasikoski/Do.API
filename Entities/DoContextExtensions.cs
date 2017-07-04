using System;
using System.Collections.Generic;
using System.Linq;

namespace Do.API.Entities
{
    public static class LibraryContextExtensions
    {
        public static void EnsureSeedDataForContext(this DoContext context)
        {
            // first, clear the database.  This ensures we can always start 
            // fresh with each demo.  Not advised for production environments, obviously :-)

            context.TaskCategories.RemoveRange(context.TaskCategories);
            context.SaveChanges();
            context.BlogPosts.RemoveRange(context.BlogPosts);
            context.SaveChanges();

            // init seed data
            var categories = new List<TaskCategory>()
            {
                new TaskCategory()
                {
                     Id = new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                     Title = "Social",
                     Description = "",
                     Tasks = new List<Task>()
                     {
                         new Task()
                         {
                             Id = new Guid("c7ba6add-09c4-45f8-8dd0-eaca221e5d93"),
                             Title = "The Shining",
                             Description = "The Shining is a horror novel by American author Stephen King. Published in 1977, it is King's third published novel and first hardback bestseller: the success of the book firmly established King as a preeminent author in the horror genre. "
                         },
                         new Task()
                         {
                             Id = new Guid("a3749477-f823-4124-aa4a-fc9ad5e79cd6"),
                             Title = "Misery",
                             Description = "Misery is a 1987 psychological horror novel by Stephen King. This novel was nominated for the World Fantasy Award for Best Novel in 1988, and was later made into a Hollywood film and an off-Broadway play of the same name."
                         },
                         new Task()
                         {
                             Id = new Guid("70a1f9b9-0a37-4c1a-99b1-c7709fc64167"),
                             Title = "It",
                             Description = "It is a 1986 horror novel by American author Stephen King. The story follows the exploits of seven children as they are terrorized by the eponymous being, which exploits the fears and phobias of its victims in order to disguise itself while hunting its prey. 'It' primarily appears in the form of a clown in order to attract its preferred prey of young children."
                         },
                         new Task()
                         {
                             Id = new Guid("60188a2b-2784-4fc4-8df8-8919ff838b0b"),
                             Title = "The Stand",
                             Description = "The Stand is a post-apocalyptic horror/fantasy novel by American author Stephen King. It expands upon the scenario of his earlier short story 'Night Surf' and outlines the total breakdown of society after the accidental release of a strain of influenza that had been modified for biological warfare causes an apocalyptic pandemic which kills off the majority of the world's human population."
                         }
                     }
                },
                new TaskCategory()
                {
                     Id = new Guid("76053df4-6687-4353-8937-b45556748abe"),
                     Title = "Programming",
                     Description = "Sample description",
                     Tasks = new List<Task>()
                     {
                         new Task()
                         {
                             Id = new Guid("447eb762-95e9-4c31-95e1-b20053fbe215"),
                             Title = "A Game of Thrones",
                             Description = "A Game of Thrones is the first novel in A Song of Ice and Fire, a series of fantasy novels by American author George R. R. Martin. It was first published on August 1, 1996."
                         },
                         new Task()
                         {
                             Id = new Guid("bc4c35c3-3857-4250-9449-155fcf5109ec"),
                             Title = "The Winds of Winter",
                             Description = "Forthcoming 6th novel in A Song of Ice and Fire."
                         },
                         new Task()
                         {
                             Id = new Guid("09af5a52-9421-44e8-a2bb-a6b9ccbc8239"),
                             Title = "A Dance with Dragons",
                             Description = "A Dance with Dragons is the fifth of seven planned novels in the epic fantasy series A Song of Ice and Fire by American author George R. R. Martin.",
                             Date = new DateTimeOffset(new DateTime(1958, 8, 27))
                         }
                     }
                },
                new TaskCategory()
                {
                    Id = new Guid("412c3012-d891-4f5e-9613-ff7aa63e6bb3"),
                    Title = "Physical Exercise", 
                    Description = "Sample description",
                    Tasks = new List<Task>()
                     {
                         new Task()
                         {
                             Id = new Guid("9edf91ee-ab77-4521-a402-5f188bc0c577"),
                             Title = "Daily ab training",
                             Description = "American Gods is a Hugo and Nebula Award-winning novel by English author Neil Gaiman. The novel is a blend of Americana, fantasy, and various strands of ancient and modern mythology, all centering on the mysterious and taciturn Shadow.",
                             Date = new DateTimeOffset(new DateTime(2017, 1, 6))
                         }
                     }
                },
                new TaskCategory()
                {
                     Id = new Guid("578359b7-1967-41d6-8b87-64ab7605587e"),
                     Title = "Miscellaneous",
                     Tasks = new List<Task>()
                     {
                         new Task()
                         {
                             Id = new Guid("01457142-358f-495f-aafa-fb23de3d67e9"),
                             Title = "Speechless",
                             Description = "Good-natured and often humorous, Speechless is at times a 'song of curses', as Lanoye describes the conflicts with his beloved diva of a mother and her brave struggle with decline and death.",
                             Date = new DateTimeOffset(new DateTime(1958, 8, 27))
                         }
                     }             
                }
            };

            var blogs = new List<BlogPost>()
            {
                new BlogPost()
                {
                    Id = new Guid("688db62d-f019-41c6-a592-2701587815f9"),
                    Title = "First two weeks of daily ab training completed",
                    Content = "Lorem ipsum dolor sit amet, vel errem perfecto dignissim no. Vel id vero tollit detracto. Ut quas sonet posidonium pri. His ea aperiri reformidans voluptatibus, ne lobortis adolescens duo, has an modo similique.",
                    Date = new DateTimeOffset(new DateTime(2017,6,5)),
                    ImageUrl = "https://dummyimage.com/600x400",
                    RelatedTasks = new List<BlogPostTask>()
                    {
                        new BlogPostTask()
                        {
                            BlogPostId = new Guid("688db62d-f019-41c6-a592-2701587815f9"),
                            TaskId = categories.Where(c => c.Title == "Physical Exercise").FirstOrDefault().Tasks.FirstOrDefault().Id
                        }
                     }
                }
            };

            context.TaskCategories.AddRange(categories);
            context.BlogPosts.AddRange(blogs);
            context.SaveChanges();       

        }
    }
}

