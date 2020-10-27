using SpartaMobileWebApi.Data;
using SpartaMobileWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpartaMobileWebApi.Repositories
{
    public class UserManagementUnitOfWork : IUserManagementUnitOfWork
    {
        readonly AuthenticationContext _context;
        public UserManagementUnitOfWork(AuthenticationContext context)
        {
            _context = context;
        }
        public IUserManagementRepository<TEntity> GetREpository<TEntity>() where TEntity : class
        {
            return new UserManagementRepository<TEntity>(_context);
        }

        public async Task<int>SaveChanges()
        {
           return await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
