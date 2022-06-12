using Microsoft.EntityFrameworkCore;
using Socail.BL.Helper;
using Socail.BL.Interface;
using Socail.DAL.Database;
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

        public async Task<PagedList<T>> GetPagination(UserParams userParams)
        {
            var data =context.Set<T>();
            
            return await PagedList<T>.CreateAsync(data,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<PagedList<ApplicationUser>> GetUsers(UserParams userParams)
        {
            var data = context.Users.AsQueryable();

            data = data.Where(u => u.Id != userParams.UserId);
            data = data.Where(u => u.Gender == userParams.Gender);


            return await PagedList<ApplicationUser>.CreateAsync(data, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
