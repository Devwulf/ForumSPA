using ForumSPA.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Utils
{
    internal static class ApplicationDbContextHelper
    {
        internal static void SyncEntitiesStatePreCommit(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                var dateTrackable = entry.Entity as IDateTrackable;
                if (dateTrackable != null)
                {
                    var now = DateTime.Now;

                    if (entry.State == EntityState.Added)
                    {
                        dateTrackable.DateCreated = now;
                        dateTrackable.DateModified = now;
                    }
                    else
                    {
                        // We don't want DateCreated to change when modified
                        entry.Property("DateCreated").IsModified = false;
                    }

                    if (entry.State == EntityState.Modified)
                        dateTrackable.DateModified = now;
                }

                var userInfo = entry.Entity as IUserInfo;
                if (userInfo != null)
                {
                    // It doesn't make sense for threads and posts
                    // to be given to other users, so this will always
                    // guarantee the original user's ownership.
                    if (entry.State != EntityState.Added)
                        entry.Property("UserId").IsModified = false;
                }

                var threadInfo = entry.Entity as IThreadInfo;
                if (threadInfo != null)
                {
                    if (entry.State != EntityState.Added)
                        entry.Property("ThreadId").IsModified = false;
                }
            }
        }
    }
}
