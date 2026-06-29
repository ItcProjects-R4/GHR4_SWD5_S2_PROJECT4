using LMS.Domain.Models;
using LMS.Domain.Models.LMS.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace LMS.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Module> Modules { get; set; }

        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<SubmissionFile> SubmissionFiles { get; set; }

        public DbSet<NewsletterSubscriber> NewsletterSubscribers { get; set; }
    }
}
