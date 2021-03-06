using Microsoft.EntityFrameworkCore;
using Socail.BL.Helper;
using Socail.BL.Interface;
using Socail.DAL.Database;
using Socail.DAL.Entity;
using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Repository
{
    public class SocailRep<T> : ISocailRep<T> where T : class
    {
        private readonly AppDbContext context;

        public SocailRep(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<T> Add(T item)
        {
            context.Set<T>().AddAsync(item);
            return item;
        }

        public T Delete(T item)
        {
            context.Set<T>().Remove(item);
            return item;
        }

        public T Edit(T item)
        {
            context.Set<T>().Update(item);
            return item;
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> match, string[] includes = null)
        {
            IQueryable<T> query = context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.SingleOrDefaultAsync(match);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> match, string[] includes = null)
        {
            IQueryable<T> query = context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(match).ToListAsync();
        }

        

        
        public T GetById(int id)
        {
            var data = context.Set<T>().Find(id);
            return data;
        }

        public async Task<PagedList<T>> GetPagination(UserParams userParams)
        {
            var data =context.Set<T>();
            
            return await PagedList<T>.CreateAsync(data,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<PagedList<ApplicationUser>> GetUsers(UserParams userParams)
        {
            var data = context.Users.AsQueryable();
            data = data.Where(u => u.Id != userParams.UserId);
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId,true);
                data = data.Where(u => userLikers.Contains(u.Id));
            }
            
            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, false);
                data = data.Where(u => userLikees.Contains(u.Id));
            }
            data = data.Where(u => u.Gender == userParams.Gender);


            return await PagedList<ApplicationUser>.CreateAsync(data, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<string>> GetUserLikes(string id,bool likers)
        {
            var user = await context.Users.Include(u=>u.Likers).Include(u=>u.Likees).FirstOrDefaultAsync(u=>u.Id == id);
            if (likers)
            {
                var x = user.Likers.Where(u => u.LikeeId == id).Select(l => l.LikerId);
                return x;
            }
            else
            {
                var x = user.Likees.Where(u => u.LikerId == id).Select(l => l.LikeeId);
                return x;
            }
        }
        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Message>> GetConversation(string userId, string RecsipientId)
        {
            var messages = await context.Messages.Include(m => m.Sender).Include(m => m.Resipient)
                .Where(m=>(m.ResipientId == userId && m.SenderId == RecsipientId) || (m.ResipientId == RecsipientId && m.SenderId == userId))
                .OrderBy(m=>m.messageSent).ToListAsync();

            return messages;


        }
        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = context.Messages.Include(m => m.Sender).Include(m => m.Resipient).AsQueryable();
            switch (messageParams.MessageType)
            {
                case "Inbox":
                    messages = messages.Where(m => m.ResipientId == messageParams.UserId && m.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId && m.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(m => m.ResipientId == messageParams.UserId && m.RecipientDeleted == false && m.IsRead == false);
                    break;
            }
            messages = messages.OrderByDescending(m => m.messageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<ApplicationUser>> GetLikersAndLikees(string userId, string type)
        {
            var users = context.Users.Include(u => u.Photos).OrderBy(u => u.FullName).AsQueryable();
            if (type == "likers")
            {
                var userLikers = await GetUserLikes(userId, true);
                users = users.Where(u => userLikers.Contains(u.Id));
            }
            else if (type == "likees")
            {
                var userLikees = await GetUserLikes(userId, false);
                users = users.Where(u => userLikees.Contains(u.Id));
            }
            else
            {
                throw new Exception("لا توجد بيانات متاحة");
            }

            return users.ToList();
        }
    }
}
