using LMS.Domain.Enums;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.DAL.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            
            await context.Database.EnsureCreatedAsync();

            
            string[] roles = ["Instructor", "Student"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            
            var standardCourseTitle = "Mastering ASP.NET Core & Clean Architecture";
            if (await context.Courses.AnyAsync(c => c.Title == standardCourseTitle))
            {
                
                var student2 = new ApplicationUser
                {
                    Id = "student-2",
                    UserName = "student2@test.com",
                    Email = "student2@test.com",
                    FirstName = "Alex",
                    LastName = "Carter",
                    EmailConfirmed = true
                };

                var existingStudent2 = await userManager.FindByIdAsync(student2.Id)
                                      ?? await userManager.FindByEmailAsync(student2.Email);
                if (existingStudent2 == null)
                {
                    var result = await userManager.CreateAsync(student2, "Password123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(student2, "Student");
                    }
                }

                
                var emptyCourseTitle = "Introduction to Artificial Intelligence";
                var hasEmptyCourse = await context.Courses.AnyAsync(c => c.Title == emptyCourseTitle);
                if (!hasEmptyCourse)
                {
                    var instructorUser = await userManager.FindByIdAsync("instructor-1");
                    var emptyCourse = new Course
                    {
                        Title = emptyCourseTitle,
                        ThumbnailUrl = "https://images.unsplash.com/photo-1677442136019-21780efad99a?auto=format&fit=crop&w=800&q=80",
                        Price = 19.99m,
                        Description = "An introductory course to AI, Machine Learning, and Deep Learning without any modules or content yet.",
                        InstructorId = "instructor-1",
                        Instructor = instructorUser!,
                        TotalLessonCount = 0
                    };
                    await context.Courses.AddAsync(emptyCourse);
                    await context.SaveChangesAsync();
                }
                
                if (!await context.Assignments.AnyAsync())
                {
                    var existingModules = await context.Modules.ToListAsync();
                    var assignmentsToSeed = new List<Assignment>();

                    void AddAssignmentIfModuleExists(string moduleTitle, string assignmentTitle, string fileUrl, int daysFromNow, int maxScore)
                    {
                        var targetModule = existingModules.FirstOrDefault(m => m.Title == moduleTitle);
                        if (targetModule != null)
                        {
                            assignmentsToSeed.Add(new Assignment
                            {
                                Title = assignmentTitle,
                                FileUrl = fileUrl,
                                DueDate = DateTime.UtcNow.AddDays(daysFromNow),
                                MaxScore = maxScore,
                                ModuleId = targetModule.Id
                            });
                        }
                    }

                    AddAssignmentIfModuleExists("Application Layer", "Build a Clean Architecture Solution", "https://example.com/assignments/clean-arch.pdf", 14, 100);
                    AddAssignmentIfModuleExists("Flexbox & Grid Mastery", "Design a Responsive Landing Page", "https://example.com/assignments/landing-page.pdf", 10, 80);
                    AddAssignmentIfModuleExists("Delegates and Events", "Implement a Custom Event System", "https://example.com/assignments/event-system.pdf", 7, 100);
                    AddAssignmentIfModuleExists("State Management", "Build a Todo App with Redux", "https://example.com/assignments/redux-todo.pdf", 12, 90);
                    AddAssignmentIfModuleExists("Normalization", "Database Schema Design Project", "https://example.com/assignments/db-schema.pdf", 21, 100);
                    AddAssignmentIfModuleExists("App Service & Containers", "Deploy an App to Azure", "https://example.com/assignments/azure-deploy.pdf", 18, 80);

                    if (assignmentsToSeed.Any())
                    {
                        await context.Assignments.AddRangeAsync(assignmentsToSeed);
                        await context.SaveChangesAsync();
                    }
                }

                await SeedAdditionalUsersAndEnrollmentsAsync(context, userManager);
                return;
            }

            
            var instructor = new ApplicationUser
            {
                Id = "instructor-1",
                UserName = "instructor@test.com",
                Email = "instructor@test.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true
            };

            var existingInstructor = await userManager.FindByIdAsync(instructor.Id)
                                    ?? await userManager.FindByEmailAsync(instructor.Email);
            if (existingInstructor == null)
            {
                var result = await userManager.CreateAsync(instructor, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(instructor, "Instructor");
                }
            }
            else
            {
                instructor = existingInstructor;
            }

            
            var student = new ApplicationUser
            {
                Id = "student-1",
                UserName = "student@test.com",
                Email = "student@test.com",
                FirstName = "Jane",
                LastName = "Smith",
                EmailConfirmed = true
            };

            var existingStudent = await userManager.FindByIdAsync(student.Id)
                                 ?? await userManager.FindByEmailAsync(student.Email);
            if (existingStudent == null)
            {
                var result = await userManager.CreateAsync(student, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, "Student");
                }
            }
            else
            {
                student = existingStudent;
            }

            
            var courses = new List<Course>
            {
                new Course
                {
                    Title = "Mastering ASP.NET Core & Clean Architecture",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?auto=format&fit=crop&w=800&q=80",
                    Price = 49.99m,
                    Description = "Learn how to build scalable and maintainable web applications using ASP.NET Core and Clean Architecture principles.",
                    InstructorId = instructor.Id,
                    Instructor = instructor,
                    TotalLessonCount = 0
                },
                new Course
                {
                    Title = "Modern UI Design with Vanilla CSS",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1550751827-4bd374c3f58b?auto=format&fit=crop&w=800&q=80",
                    Price = 29.99m,
                    Description = "Master the art of creating beautiful, responsive user interfaces using only modern CSS features.",
                    InstructorId = instructor.Id,
                    Instructor = instructor,
                    TotalLessonCount = 0
                },
                new Course
                {
                    Title = "Advanced C# Development",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1516116216624-53e697fedbea?auto=format&fit=crop&w=800&q=80",
                    Price = 59.99m,
                    Description = "Deep dive into C# features, performance optimization, and multithreading.",
                    InstructorId = instructor.Id,
                    Instructor = instructor,
                    TotalLessonCount = 0
                },
                new Course
                {
                    Title = "React & Redux Masterclass",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1633356122544-f134324a6cee?auto=format&fit=crop&w=800&q=80",
                    Price = 39.99m,
                    Description = "Build complex frontend applications with React and manage state with Redux Toolkit.",
                    InstructorId = instructor.Id,
                    Instructor = instructor,
                    TotalLessonCount = 0
                },
                new Course
                {
                    Title = "Database Design & SQL Mastery",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1544383835-bda2bc66a55d?auto=format&fit=crop&w=800&q=80",
                    Price = 34.99m,
                    Description = "Learn to design efficient relational databases and write complex SQL queries.",
                    InstructorId = instructor.Id,
                    Instructor = instructor,
                    TotalLessonCount = 0
                },
                new Course
                {
                    Title = "Cloud Native Apps with Azure",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1451187580459-43490279c0fa?auto=format&fit=crop&w=800&q=80",
                    Price = 69.99m,
                    Description = "Deploy and scale your applications in the cloud using Microsoft Azure services.",
                    InstructorId = instructor.Id,
                    Instructor = instructor,
                    TotalLessonCount = 0
                },
                new Course
                {
                    Title = "Introduction to Artificial Intelligence",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1677442136019-21780efad99a?auto=format&fit=crop&w=800&q=80",
                    Price = 19.99m,
                    Description = "An introductory course to AI, Machine Learning, and Deep Learning without any modules or content yet.",
                    InstructorId = instructor.Id,
                    Instructor = instructor,
                    TotalLessonCount = 0
                }
            };

            await context.Courses.AddRangeAsync(courses);

            
            
            
            var modules = new List<Module>
            {
                
                new Module { Title = "Project Setup", OrderIndex = 1, Course = courses[0] },           
                new Module { Title = "Domain Layer", OrderIndex = 2, Course = courses[0] },            
                new Module { Title = "Application Layer", OrderIndex = 3, Course = courses[0] },       
                new Module { Title = "Infrastructure & Persistence", OrderIndex = 4, Course = courses[0] }, 

                
                new Module { Title = "CSS Variables & Themes", OrderIndex = 1, Course = courses[1] },  
                new Module { Title = "Flexbox & Grid Mastery", OrderIndex = 2, Course = courses[1] },  
                new Module { Title = "Animations & Transitions", OrderIndex = 3, Course = courses[1] },

                
                new Module { Title = "Delegates and Events", OrderIndex = 1, Course = courses[2] },    
                new Module { Title = "LINQ Deep Dive", OrderIndex = 2, Course = courses[2] },          
                new Module { Title = "Async & Parallel Programming", OrderIndex = 3, Course = courses[2] }, 

                
                new Module { Title = "React Fundamentals", OrderIndex = 1, Course = courses[3] },      
                new Module { Title = "State Management", OrderIndex = 2, Course = courses[3] },        
                new Module { Title = "Advanced Patterns & Hooks", OrderIndex = 3, Course = courses[3] }, 

                
                new Module { Title = "SQL Basics", OrderIndex = 1, Course = courses[4] },              
                new Module { Title = "Normalization", OrderIndex = 2, Course = courses[4] },           
                new Module { Title = "Advanced Queries & Performance", OrderIndex = 3, Course = courses[4] }, 

                
                new Module { Title = "Azure Fundamentals", OrderIndex = 1, Course = courses[5] },      
                new Module { Title = "App Service & Containers", OrderIndex = 2, Course = courses[5] },
                new Module { Title = "CI/CD & DevOps", OrderIndex = 3, Course = courses[5] }           
            };

            await context.Modules.AddRangeAsync(modules);

            
            
            
            var contents = new List<Content>
            {
                
                new Content { Title = "Welcome to the Course", OrderIndex = 1, Module = modules[0],
                    VideoUrl = "https://example.com/v1",
                    Text = "<p>Welcome to <strong>Mastering ASP.NET Core & Clean Architecture</strong>. In this course you will learn how to structure enterprise-grade applications using proven architectural patterns.</p><p>We will cover the Domain, Application, Infrastructure, and Presentation layers step by step.</p>" },
                new Content { Title = "Setting up Solution Folders", OrderIndex = 2, Module = modules[0],
                    VideoUrl = "https://example.com/v2",
                    Text = "<p>Learn how to create a multi-project solution in Visual Studio with the correct folder structure for Clean Architecture.</p><ul><li>Domain project</li><li>Application project</li><li>Infrastructure project</li><li>Presentation (Web) project</li></ul>",
                    ArticleUrl = "https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures" },
                new Content { Title = "Installing Required NuGet Packages", OrderIndex = 3, Module = modules[0],
                    VideoUrl = "https://example.com/v3",
                    Text = "<p>We install MediatR, FluentValidation, Entity Framework Core, and AutoMapper to wire up the application layer properly.</p>" },

                
                new Content { Title = "Defining Domain Entities", OrderIndex = 1, Module = modules[1],
                    VideoUrl = "https://example.com/v4",
                    Text = "<p>Domain entities represent the core business objects. In this lesson we create <code>Course</code>, <code>Module</code>, and <code>Content</code> entities with proper relationships.</p>" },
                new Content { Title = "Value Objects & Enums", OrderIndex = 2, Module = modules[1],
                    VideoUrl = "https://example.com/v5",
                    Text = "<p>Value objects encapsulate small pieces of domain logic. We also define enums like <code>EnrollmentStatus</code> and <code>SubmissionStatus</code>.</p>",
                    ArticleUrl = "https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects" },

                
                new Content { Title = "CQRS with MediatR", OrderIndex = 1, Module = modules[2],
                    VideoUrl = "https://example.com/v6",
                    Text = "<p>CQRS separates read and write operations. We use MediatR to implement commands and queries as request/handler pairs.</p>" },
                new Content { Title = "Validation with FluentValidation", OrderIndex = 2, Module = modules[2],
                    VideoUrl = "https://example.com/v7",
                    Text = "<p>FluentValidation provides a clean way to define validation rules for your commands and queries using a fluent API.</p>" },
                new Content { Title = "Mapping with AutoMapper", OrderIndex = 3, Module = modules[2],
                    VideoUrl = "https://example.com/v8",
                    Text = "<p>AutoMapper helps us map between domain entities and DTOs / view models, keeping our layers decoupled.</p>" },

                
                new Content { Title = "EF Core DbContext Setup", OrderIndex = 1, Module = modules[3],
                    VideoUrl = "https://example.com/v9",
                    Text = "<p>We configure Entity Framework Core with SQL Server, set up the <code>ApplicationDbContext</code>, and apply entity configurations.</p>",
                    ArticleUrl = "https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/" },
                new Content { Title = "Repository Pattern", OrderIndex = 2, Module = modules[3],
                    VideoUrl = "https://example.com/v10",
                    Text = "<p>The repository pattern abstracts data access behind interfaces, making our application testable and maintainable.</p>" },

                
                new Content { Title = "Introduction to CSS Custom Properties", OrderIndex = 1, Module = modules[4],
                    VideoUrl = "https://example.com/css1",
                    Text = "<p>CSS custom properties (variables) allow you to define reusable values throughout your stylesheets. Learn the syntax <code>--my-color: #3b82f6;</code> and how to use <code>var()</code>.</p>" },
                new Content { Title = "Building a Theme Switcher", OrderIndex = 2, Module = modules[4],
                    VideoUrl = "https://example.com/css2",
                    Text = "<p>Create a dark/light mode toggle using CSS variables and a small amount of JavaScript.</p>" },

                
                new Content { Title = "Flexbox Layouts Deep Dive", OrderIndex = 1, Module = modules[5],
                    VideoUrl = "https://example.com/css3",
                    Text = "<p>Master <code>display: flex</code>, alignment, wrapping, and ordering to build responsive one-dimensional layouts.</p>",
                    ArticleUrl = "https://css-tricks.com/snippets/css/a-guide-to-flexbox/" },
                new Content { Title = "CSS Grid for Complex Layouts", OrderIndex = 2, Module = modules[5],
                    VideoUrl = "https://example.com/css4",
                    Text = "<p>CSS Grid is perfect for two-dimensional layouts. Learn <code>grid-template-columns</code>, <code>grid-area</code>, and responsive patterns.</p>" },

                
                new Content { Title = "CSS Transitions Fundamentals", OrderIndex = 1, Module = modules[6],
                    VideoUrl = "https://example.com/css5",
                    Text = "<p>Transitions animate property changes smoothly. Learn <code>transition-property</code>, <code>transition-duration</code>, and easing functions.</p>" },
                new Content { Title = "Keyframe Animations", OrderIndex = 2, Module = modules[6],
                    VideoUrl = "https://example.com/css6",
                    Text = "<p>Use <code>@keyframes</code> to create complex multi-step animations for loading spinners, hover effects, and page transitions.</p>" },

                
                new Content { Title = "Understanding Delegates", OrderIndex = 1, Module = modules[7],
                    VideoUrl = "https://example.com/cs1",
                    Text = "<p>Delegates are type-safe function pointers in C#. Learn how to declare, instantiate, and invoke delegates.</p>" },
                new Content { Title = "Events & Event Handlers", OrderIndex = 2, Module = modules[7],
                    VideoUrl = "https://example.com/cs2",
                    Text = "<p>Events provide a publish-subscribe mechanism. We explore the <code>event</code> keyword and the <code>EventHandler</code> pattern.</p>" },
                new Content { Title = "Func, Action & Predicate", OrderIndex = 3, Module = modules[7],
                    VideoUrl = "https://example.com/cs3",
                    Text = "<p>Built-in generic delegates simplify your code: <code>Func&lt;T,TResult&gt;</code>, <code>Action&lt;T&gt;</code>, and <code>Predicate&lt;T&gt;</code>.</p>" },

                
                new Content { Title = "LINQ Query Syntax vs Method Syntax", OrderIndex = 1, Module = modules[8],
                    VideoUrl = "https://example.com/cs4",
                    Text = "<p>Compare the two ways to write LINQ queries and learn when to use each style.</p>" },
                new Content { Title = "Advanced LINQ Operations", OrderIndex = 2, Module = modules[8],
                    VideoUrl = "https://example.com/cs5",
                    Text = "<p>GroupBy, Join, SelectMany, Aggregate — master the advanced LINQ operators for complex data transformations.</p>",
                    ArticleUrl = "https://learn.microsoft.com/en-us/dotnet/csharp/linq/" },

                
                new Content { Title = "Async/Await Fundamentals", OrderIndex = 1, Module = modules[9],
                    VideoUrl = "https://example.com/cs6",
                    Text = "<p>Learn the <code>async</code> and <code>await</code> keywords, Task-based asynchronous pattern, and how to avoid deadlocks.</p>" },
                new Content { Title = "Parallel.ForEach & PLINQ", OrderIndex = 2, Module = modules[9],
                    VideoUrl = "https://example.com/cs7",
                    Text = "<p>Use <code>Parallel.ForEach</code> and PLINQ to leverage multi-core CPUs for CPU-bound operations.</p>" },

                
                new Content { Title = "JSX Basics", OrderIndex = 1, Module = modules[10],
                    VideoUrl = "https://example.com/react1",
                    Text = "<p>JSX is a syntax extension that lets you write HTML-like code inside JavaScript. Learn the rules and best practices.</p>" },
                new Content { Title = "Components & Props", OrderIndex = 2, Module = modules[10],
                    VideoUrl = "https://example.com/react2",
                    Text = "<p>Components are the building blocks of React apps. Learn functional components, prop drilling, and default props.</p>" },
                new Content { Title = "Handling Events in React", OrderIndex = 3, Module = modules[10],
                    VideoUrl = "https://example.com/react3",
                    Text = "<p>React uses synthetic events. Learn how to handle clicks, form submissions, and keyboard events.</p>" },

                
                new Content { Title = "useState & useReducer", OrderIndex = 1, Module = modules[11],
                    VideoUrl = "https://example.com/react4",
                    Text = "<p>Manage component-level state with <code>useState</code> and complex state logic with <code>useReducer</code>.</p>" },
                new Content { Title = "Redux Toolkit Setup", OrderIndex = 2, Module = modules[11],
                    VideoUrl = "https://example.com/react5",
                    Text = "<p>Redux Toolkit simplifies Redux with <code>createSlice</code>, <code>configureStore</code>, and built-in Immer support.</p>",
                    ArticleUrl = "https://redux-toolkit.js.org/introduction/getting-started" },

                
                new Content { Title = "Custom Hooks", OrderIndex = 1, Module = modules[12],
                    VideoUrl = "https://example.com/react6",
                    Text = "<p>Extract reusable logic into custom hooks like <code>useFetch</code>, <code>useLocalStorage</code>, and <code>useDebounce</code>.</p>" },
                new Content { Title = "React Performance Optimization", OrderIndex = 2, Module = modules[12],
                    VideoUrl = "https://example.com/react7",
                    Text = "<p>Use <code>React.memo</code>, <code>useMemo</code>, and <code>useCallback</code> to prevent unnecessary re-renders.</p>" },

                
                new Content { Title = "SELECT Statements", OrderIndex = 1, Module = modules[13],
                    VideoUrl = "https://example.com/sql1",
                    Text = "<p>The foundation of SQL — learn <code>SELECT</code>, <code>WHERE</code>, <code>ORDER BY</code>, and <code>LIMIT</code> clauses.</p>" },
                new Content { Title = "INSERT, UPDATE & DELETE", OrderIndex = 2, Module = modules[13],
                    VideoUrl = "https://example.com/sql2",
                    Text = "<p>Manipulate data with DML statements. Learn safe update practices and transaction basics.</p>" },

                
                new Content { Title = "1NF, 2NF, 3NF Explained", OrderIndex = 1, Module = modules[14],
                    VideoUrl = "https://example.com/sql3",
                    Text = "<p>Database normalization reduces redundancy. Walk through First, Second, and Third Normal Forms with examples.</p>" },
                new Content { Title = "When to Denormalize", OrderIndex = 2, Module = modules[14],
                    VideoUrl = "https://example.com/sql4",
                    Text = "<p>Sometimes performance trumps normalization. Learn when denormalization is acceptable and how to do it safely.</p>" },

                
                new Content { Title = "JOINs Masterclass", OrderIndex = 1, Module = modules[15],
                    VideoUrl = "https://example.com/sql5",
                    Text = "<p>INNER, LEFT, RIGHT, FULL OUTER, and CROSS joins explained with visual diagrams and practical examples.</p>",
                    ArticleUrl = "https://www.sqlshack.com/sql-join-overview-and-tutorial/" },
                new Content { Title = "Window Functions & CTEs", OrderIndex = 2, Module = modules[15],
                    VideoUrl = "https://example.com/sql6",
                    Text = "<p>Advanced SQL: <code>ROW_NUMBER()</code>, <code>RANK()</code>, <code>LAG/LEAD</code>, and Common Table Expressions for complex reporting.</p>" },

                
                new Content { Title = "Azure Portal & CLI Overview", OrderIndex = 1, Module = modules[16],
                    VideoUrl = "https://example.com/az1",
                    Text = "<p>Navigate the Azure portal and learn the Azure CLI basics for managing cloud resources.</p>" },
                new Content { Title = "Resource Groups & Subscriptions", OrderIndex = 2, Module = modules[16],
                    VideoUrl = "https://example.com/az2",
                    Text = "<p>Organize your Azure resources with resource groups, tags, and subscription management best practices.</p>" },

                
                new Content { Title = "Deploying to Azure App Service", OrderIndex = 1, Module = modules[17],
                    VideoUrl = "https://example.com/az3",
                    Text = "<p>Deploy your ASP.NET Core app to Azure App Service using Visual Studio, CLI, or GitHub Actions.</p>",
                    ArticleUrl = "https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore" },
                new Content { Title = "Docker & Azure Container Instances", OrderIndex = 2, Module = modules[17],
                    VideoUrl = "https://example.com/az4",
                    Text = "<p>Containerize your application with Docker and deploy it to Azure Container Instances for quick, serverless hosting.</p>" },

                
                new Content { Title = "GitHub Actions for .NET", OrderIndex = 1, Module = modules[18],
                    VideoUrl = "https://example.com/az5",
                    Text = "<p>Automate build, test, and deployment pipelines with GitHub Actions workflows for your .NET projects.</p>" },
                new Content { Title = "Azure DevOps Pipelines", OrderIndex = 2, Module = modules[18],
                    VideoUrl = "https://example.com/az6",
                    Text = "<p>Set up CI/CD pipelines with Azure DevOps for enterprise-grade deployment automation.</p>" }
            };

            await context.Contents.AddRangeAsync(contents);

            foreach (var course in courses)
            {
                course.TotalLessonCount = contents.Count(c => c.Module.Course == course);
            }

            
            
            
            var assignments = new List<Assignment>
            {
                new Assignment
                {
                    Title = "Build a Clean Architecture Solution",
                    FileUrl = "https://example.com/assignments/clean-arch.pdf",
                    DueDate = DateTime.UtcNow.AddDays(14),
                    MaxScore = 100,
                    Module = modules[2]
                },
                new Assignment
                {
                    Title = "Design a Responsive Landing Page",
                    FileUrl = "https://example.com/assignments/landing-page.pdf",
                    DueDate = DateTime.UtcNow.AddDays(10),
                    MaxScore = 80,
                    Module = modules[5]
                },
                new Assignment
                {
                    Title = "Implement a Custom Event System",
                    FileUrl = "https://example.com/assignments/event-system.pdf",
                    DueDate = DateTime.UtcNow.AddDays(7),
                    MaxScore = 100,
                    Module = modules[7]
                },
                new Assignment
                {
                    Title = "Build a Todo App with Redux",
                    FileUrl = "https://example.com/assignments/redux-todo.pdf",
                    DueDate = DateTime.UtcNow.AddDays(12),
                    MaxScore = 90,
                    Module = modules[11]
                },
                new Assignment
                {
                    Title = "Database Schema Design Project",
                    FileUrl = "https://example.com/assignments/db-schema.pdf",
                    DueDate = DateTime.UtcNow.AddDays(21),
                    MaxScore = 100,
                    Module = modules[14]
                },
                new Assignment
                {
                    Title = "Deploy an App to Azure",
                    FileUrl = "https://example.com/assignments/azure-deploy.pdf",
                    DueDate = DateTime.UtcNow.AddDays(18),
                    MaxScore = 80,
                    Module = modules[17]
                }
            };

            await context.Assignments.AddRangeAsync(assignments);

            
            
            
            var enrollments = new List<Enrollment>
            {
                new Enrollment
                {
                    StudentId = student.Id,
                    Course = courses[0],
                    EnrolledAt = DateTime.UtcNow.AddDays(-10),
                    Status = EnrollmentStatus.Active,
                    CompletedLessonsCount = 5
                },
                new Enrollment
                {
                    StudentId = student.Id,
                    Course = courses[1],
                    EnrolledAt = DateTime.UtcNow.AddDays(-2),
                    Status = EnrollmentStatus.Completed,
                    CompletedLessonsCount = 6
                },
                new Enrollment
                {
                    StudentId = student.Id,
                    Course = courses[2],
                    EnrolledAt = DateTime.UtcNow.AddDays(-5),
                    Status = EnrollmentStatus.Active,
                    CompletedLessonsCount = 2
                },
                new Enrollment
                {
                    StudentId = student.Id,
                    Course = courses[3],
                    EnrolledAt = DateTime.UtcNow.AddDays(-1),
                    Status = EnrollmentStatus.Active,
                    CompletedLessonsCount = 0
                }
            };

            await context.Enrollments.AddRangeAsync(enrollments);

            
            
            
            var progresses = new List<Progress>
            {
                
                new Progress { StudentId = student.Id, Content = contents[0], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-9) },
                new Progress { StudentId = student.Id, Content = contents[1], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-8) },
                new Progress { StudentId = student.Id, Content = contents[2], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-7) },
                new Progress { StudentId = student.Id, Content = contents[3], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-6) },
                new Progress { StudentId = student.Id, Content = contents[4], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-5) },

                
                new Progress { StudentId = student.Id, Content = contents[10], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-4) },
                new Progress { StudentId = student.Id, Content = contents[11], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-4) },
                new Progress { StudentId = student.Id, Content = contents[12], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-3) },
                new Progress { StudentId = student.Id, Content = contents[13], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-3) },
                new Progress { StudentId = student.Id, Content = contents[14], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-2) },
                new Progress { StudentId = student.Id, Content = contents[15], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-2) },

                
                new Progress { StudentId = student.Id, Content = contents[16], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-4) },
                new Progress { StudentId = student.Id, Content = contents[17], IsCompleted = true, CompletedAt = DateTime.UtcNow.AddDays(-3) },
            };

            await context.Progresses.AddRangeAsync(progresses);

            await context.SaveChangesAsync();

            await SeedAdditionalUsersAndEnrollmentsAsync(context, userManager);
        }

        private static async Task SeedAdditionalUsersAndEnrollmentsAsync(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            
            var student3 = new ApplicationUser
            {
                Id = "student-3",
                UserName = "student3@test.com",
                Email = "student3@test.com",
                FirstName = "Robert",
                LastName = "Johnson",
                EmailConfirmed = true
            };
            var existingStudent3 = await userManager.FindByIdAsync(student3.Id)
                                  ?? await userManager.FindByEmailAsync(student3.Email);
            if (existingStudent3 == null)
            {
                var result = await userManager.CreateAsync(student3, "Password123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(student3, "Student");
            }

            
            var student4 = new ApplicationUser
            {
                Id = "student-4",
                UserName = "student4@test.com",
                Email = "student4@test.com",
                FirstName = "Emily",
                LastName = "Davis",
                EmailConfirmed = true
            };
            var existingStudent4 = await userManager.FindByIdAsync(student4.Id)
                                  ?? await userManager.FindByEmailAsync(student4.Email);
            if (existingStudent4 == null)
            {
                var result = await userManager.CreateAsync(student4, "Password123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(student4, "Student");
            }

            
            var student5 = new ApplicationUser
            {
                Id = "student-5",
                UserName = "student5@test.com",
                Email = "student5@test.com",
                FirstName = "Michael",
                LastName = "Brown",
                EmailConfirmed = true
            };
            var existingStudent5 = await userManager.FindByIdAsync(student5.Id)
                                  ?? await userManager.FindByEmailAsync(student5.Email);
            if (existingStudent5 == null)
            {
                var result = await userManager.CreateAsync(student5, "Password123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(student5, "Student");
            }

            
            var courses = await context.Courses.ToListAsync();
            var course1 = courses.FirstOrDefault(c => c.Title == "Mastering ASP.NET Core & Clean Architecture");
            var course2 = courses.FirstOrDefault(c => c.Title == "Modern UI Design with Vanilla CSS");
            var course3 = courses.FirstOrDefault(c => c.Title == "Advanced C# Development");
            var course4 = courses.FirstOrDefault(c => c.Title == "React & Redux Masterclass");

            async Task EnrollIfMissing(string studentId, Course? course, int completedCount, EnrollmentStatus status)
            {
                if (course == null) return;
                var exists = await context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == course.Id);
                if (!exists)
                {
                    await context.Enrollments.AddAsync(new Enrollment
                    {
                        StudentId = studentId,
                        CourseId = course.Id,
                        EnrolledAt = DateTime.UtcNow.AddDays(-new Random().Next(1, 15)),
                        Status = status,
                        CompletedLessonsCount = completedCount
                    });
                }
            }

            await EnrollIfMissing("student-3", course1, 3, EnrollmentStatus.Active);
            await EnrollIfMissing("student-3", course2, 6, EnrollmentStatus.Completed);
            
            await EnrollIfMissing("student-4", course2, 2, EnrollmentStatus.Active);
            await EnrollIfMissing("student-4", course3, 4, EnrollmentStatus.Active);

            await EnrollIfMissing("student-5", course3, 7, EnrollmentStatus.Completed);
            await EnrollIfMissing("student-5", course4, 1, EnrollmentStatus.Active);

            await context.SaveChangesAsync();

            await SeedSubmissionsAsync(context);
        }

        private static async Task SeedSubmissionsAsync(ApplicationDbContext context)
        {
            if (await context.Submissions.AnyAsync())
            {
                return;
            }

            var assignments = await context.Assignments.ToListAsync();
            var cleanArchAssignment = assignments.FirstOrDefault(a => a.Title == "Build a Clean Architecture Solution");
            var responsiveLandingAssignment = assignments.FirstOrDefault(a => a.Title == "Design a Responsive Landing Page");
            var customEventAssignment = assignments.FirstOrDefault(a => a.Title == "Implement a Custom Event System");
            var todoReduxAssignment = assignments.FirstOrDefault(a => a.Title == "Build a Todo App with Redux");

            var submissions = new List<Submission>();

            // Student 1 (student-1)
            if (cleanArchAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-1",
                    AssignmentId = cleanArchAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-5),
                    Status = SubmissionStatus.Graded,
                    Grade = 95,
                    Comment = "Excellent work on separating the layers. Clean architecture is implemented beautifully!",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/clean-arch-janesmith.zip",
                            FileName = "clean-arch-janesmith.zip",
                            FileType = "application/zip",
                            FileSize = 1024 * 15.5
                        }
                    }
                });
            }

            if (responsiveLandingAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-1",
                    AssignmentId = responsiveLandingAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-2),
                    Status = SubmissionStatus.Pending,
                    Link = "https://github.com/janesmith/responsive-landing",
                    Comment = "I used CSS Grid and Flexbox for the layout. Let me know if the animations are okay.",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/responsive-landing-janesmith.pdf",
                            FileName = "responsive-landing-janesmith.pdf",
                            FileType = "application/pdf",
                            FileSize = 1024 * 3.2
                        }
                    }
                });
            }

            if (customEventAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-1",
                    AssignmentId = customEventAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-1),
                    Status = SubmissionStatus.Pending,
                    Comment = "Implemented the custom delegate and event handlers as requested.",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/event-system-janesmith.zip",
                            FileName = "event-system-janesmith.zip",
                            FileType = "application/zip",
                            FileSize = 1024 * 8.4
                        }
                    }
                });
            }

            // Student 3 (student-3)
            if (cleanArchAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-3",
                    AssignmentId = cleanArchAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-4),
                    Status = SubmissionStatus.Graded,
                    Grade = 88,
                    Comment = "Good project structure. Watch out for dependencies leaking from Application to Infrastructure layer.",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/clean-arch-robertj.zip",
                            FileName = "clean-arch-robertj.zip",
                            FileType = "application/zip",
                            FileSize = 1024 * 12.1
                        }
                    }
                });
            }

            if (responsiveLandingAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-3",
                    AssignmentId = responsiveLandingAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-3),
                    Status = SubmissionStatus.Graded,
                    Grade = 75,
                    Comment = "Looks good, but the design is not fully responsive on mobile devices (width < 375px).",
                    Link = "https://github.com/robertj/responsive-landing"
                });
            }

            // Student 4 (student-4)
            if (responsiveLandingAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-4",
                    AssignmentId = responsiveLandingAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-2),
                    Status = SubmissionStatus.Pending,
                    Link = "https://github.com/emilyd/responsive-landing",
                    Comment = "Here is my landing page. Added smooth keyframe animations!",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/responsive-landing-emilyd.zip",
                            FileName = "responsive-landing-emilyd.zip",
                            FileType = "application/zip",
                            FileSize = 1024 * 18.2
                        }
                    }
                });
            }

            if (customEventAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-4",
                    AssignmentId = customEventAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-3),
                    Status = SubmissionStatus.Graded,
                    Grade = 92,
                    Comment = "Well done! The decoupling of publishers and subscribers is clean.",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/event-system-emilyd.zip",
                            FileName = "event-system-emilyd.zip",
                            FileType = "application/zip",
                            FileSize = 1024 * 9.1
                        }
                    }
                });
            }

            // Student 5 (student-5)
            if (customEventAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-5",
                    AssignmentId = customEventAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-6),
                    Status = SubmissionStatus.Graded,
                    Grade = 85,
                    Comment = "Correct implementation, but consider thread safety when raising events in multi-threaded contexts.",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/event-system-michaelb.zip",
                            FileName = "event-system-michaelb.zip",
                            FileType = "application/zip",
                            FileSize = 1024 * 7.8
                        }
                    }
                });
            }

            if (todoReduxAssignment != null)
            {
                submissions.Add(new Submission
                {
                    StudentId = "student-5",
                    AssignmentId = todoReduxAssignment.Id,
                    SubmittedAt = DateTime.UtcNow.AddDays(-1),
                    Status = SubmissionStatus.Pending,
                    Link = "https://github.com/michaelb/redux-todo",
                    Comment = "Used Redux Toolkit with slice architecture.",
                    SubmissionFiles = new List<SubmissionFile>
                    {
                        new SubmissionFile
                        {
                            FileUrl = "https://example.com/submissions/redux-todo-michaelb.zip",
                            FileName = "redux-todo-michaelb.zip",
                            FileType = "application/zip",
                            FileSize = 1024 * 24.0
                        }
                    }
                });
            }

            if (submissions.Any())
            {
                await context.Submissions.AddRangeAsync(submissions);
                await context.SaveChangesAsync();
            }
        }
    }
}
