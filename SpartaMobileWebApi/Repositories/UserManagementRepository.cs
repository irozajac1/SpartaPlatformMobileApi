using Microsoft.EntityFrameworkCore;
using SpartaMobileWebApi.Data;
using SpartaMobileWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpartaMobileWebApi.Repositories
{
    public class UserManagementRepository<T> : IUserManagementRepository<T> where T : class
    {
        private readonly AuthenticationContext _context;
        private readonly DbSet<T> _dbSet;

        public UserManagementRepository(AuthenticationContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll() => await _dbSet.ToListAsync();

        public async Task<T> FistOrDefault(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

        public async Task<T> GetById(int id) => await _dbSet.FindAsync(id);
        public bool Any(Expression<Func<T, bool>> predicate) => _dbSet.Any(predicate);

        public void Insert(T entity) => _dbSet.Add(entity);

        public void Detele(T entity) => _dbSet.Remove(entity);

        public IQueryable<T> IncludeAll()
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var property in _context.Model.FindEntityType(typeof(T)).GetNavigations())
                query = query.Include(property.Name);
            return query;
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
