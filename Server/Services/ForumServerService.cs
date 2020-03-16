using ForumSPA.Server.Data;
using ForumSPA.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Services
{
    // This is completely different from the client-side
    // Forum Service
    public class ForumServerService
    {
        private readonly ApplicationDbContext _context;

        public ForumServerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hub>> GetAllHubs()
        {
            var hubs = await (from h in _context.Hubs
                              select h).ToListAsync();
            return hubs;
        }

        public async Task<Hub> GetHub(int hubId)
        {
            return await _context.Hubs.FindAsync(hubId);
        }

        public async Task<Hub> CreateHub(Hub hub)
        {
            _context.Hubs.Add(hub);
            await _context.SaveChangesAsync();
            return hub;
        }

        public async Task<Hub> UpdateHub(Hub hub)
        {
            _context.Hubs.Update(hub);
            await _context.SaveChangesAsync();
            return hub;
        }

        public async Task<Hub> DeleteHub(int hubId)
        {
            var hub = await GetHub(hubId);
            if (hub == null)
                return null;

            _context.Hubs.Remove(hub);
            await _context.SaveChangesAsync();
            return hub;
        }

        public async Task<IEnumerable<Thread>> GetAllThreadsByHub(int hubId)
        {
            var threads = await (from t in _context.Threads
                                 where t.HubId == hubId
                                 select t).ToListAsync();
            return threads;
        }

        public async Task<IEnumerable<Thread>> GetAllThreadsByUser(string userId)
        {
            var threads = await (from t in _context.Threads
                                 where t.UserId.Equals(userId)
                                 select t).ToListAsync();
            return threads;
        }

        public async Task<Thread> GetThread(int threadId)
        {
            return await _context.Threads.FindAsync(threadId);
        }

        public async Task<Thread> CreateThread(Thread thread)
        {
            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();
            return thread;
        }

        // Should be used with GetThread
        public async Task<Thread> UpdateThread(Thread thread)
        {
            _context.Threads.Update(thread);
            await _context.SaveChangesAsync();
            return thread;
        }

        public async Task<Thread> DeleteThread(int threadId)
        {
            var thread = await GetThread(threadId);
            if (thread == null)
                return null;

            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();
            return thread;
        }

        public async Task<IEnumerable<Post>> GetAllPostsByThread(int threadId)
        {
            var posts = await (from p in _context.Posts
                               where p.ThreadId == threadId
                               orderby p.DateCreated ascending
                               select p).ToListAsync();
            return posts;
        }

        public async Task<IEnumerable<Post>> GetAllPostsByUser(string userId)
        {
            var posts = await (from p in _context.Posts
                               where p.UserId.Equals(userId)
                               orderby p.DateCreated descending
                               select p).ToListAsync();
            return posts;
        }

        public async Task<Post> GetPost(int postId)
        {
            return await _context.Posts.FindAsync(postId);
        }

        public async Task<Post> CreatePost(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdatePost(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> DeletePost(int postId)
        {
            var post = await GetPost(postId);
            if (post == null)
                return null;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return post;
        }
    }
}
