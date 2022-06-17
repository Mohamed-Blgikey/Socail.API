using Socail.BL.Helper;
using Socail.DAL.Entity;
using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Interface
{
    public interface ISocailRep<T> where T : class
    {
        Task<PagedList<T>> GetPagination(UserParams userParams);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetConversation(string userId,string RecsipientId);
        Task<PagedList<ApplicationUser>> GetUsers(UserParams userParams);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> match, string[] includes = null);
        Task<T> GetByIdAsync(Expression<Func<T, bool>> match, string[] includes = null);

        Task<IEnumerable<ApplicationUser>> GetLikersAndLikees(string userId, string type);
        T GetById(int id);
        Task<T> Add(T item);
        T Edit(T item);
        T Delete(T item);

        Task<bool> SaveAll();
    }
}
