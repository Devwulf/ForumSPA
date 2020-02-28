using ForumSPA.Server.Data.Models;
using ForumSPA.Server.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Hub> Hubs { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }

        public Task<int> SaveChangesAsync()
        {
            this.SyncEntitiesStatePreCommit();
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed the roles into the database
            builder.Entity<IdentityRole>()
                   .HasData(new IdentityRole()
                   {
                       Name = "User",
                       NormalizedName = "USER",
                       Id = Guid.NewGuid().ToString(),
                       ConcurrencyStamp = Guid.NewGuid().ToString()
                   });
            builder.Entity<IdentityRole>()
                   .HasData(new IdentityRole()
                   {
                       Name = "Moderator",
                       NormalizedName = "MODERATOR",
                       Id = Guid.NewGuid().ToString(),
                       ConcurrencyStamp = Guid.NewGuid().ToString()
                   });
            builder.Entity<IdentityRole>()
                   .HasData(new IdentityRole()
                   {
                       Name = "Administrator",
                       NormalizedName = "ADMINISTRATOR",
                       Id = Guid.NewGuid().ToString(),
                       ConcurrencyStamp = Guid.NewGuid().ToString()
                   });

            builder.Entity<Hub>()
                   .HasMany(h => h.Threads)
                   .WithOne(t => t.Hub)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Thread>()
                   .HasMany(t => t.Posts)
                   .WithOne(p => p.Thread)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Thread>()
                   .HasOne(t => t.User)
                   .WithMany()
                   .HasForeignKey("UserId")
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Post>()
                   .HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey("UserId")
                   .OnDelete(DeleteBehavior.SetNull);

            /*
            builder.Entity<Thread>(b =>
            {
                b.HasOne(t => t.User)
                 .WithMany()
                 .HasForeignKey("UserId")
                 .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Post>(b =>
            {
                b.HasOne(t => t.User)
                 .WithMany()
                 .HasForeignKey("UserId")
                 .OnDelete(DeleteBehavior.SetNull);
            });
            /**/

            // Indexed to DateModified because we always want the recently
            // modified threads to be on top
            builder.Entity<Thread>()
                   .HasIndex(t => new { t.HubId, t.DateModified });

            builder.Entity<Post>()
                   .HasIndex(p => new { p.ThreadId, p.DateCreated });
        }
    }
}
