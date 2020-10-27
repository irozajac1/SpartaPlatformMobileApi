using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpartaMobileWebApi.Interfaces
{
    public interface IUserManagementRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> FistOrDefault(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate);
        Task<T> GetById(int id);
        bool Any(Expression<Func<T, bool>> predicate);
        void Insert(T entity);
        void Update(T entity);
        void Detele(T entity);
        IQueryable<T> IncludeAll();
    }
}
