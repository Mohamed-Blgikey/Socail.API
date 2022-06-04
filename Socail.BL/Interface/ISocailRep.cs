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
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> match, string[] includes = null);
        Task<T> GetByIdAsync(Expression<Func<T, bool>> match, string[] includes = null);

        T GetById(int id);
        Task<T> Add(T item);
        T Edit(T item);
        T Delete(T item);

        Task<bool> SaveAll();
    }
}
