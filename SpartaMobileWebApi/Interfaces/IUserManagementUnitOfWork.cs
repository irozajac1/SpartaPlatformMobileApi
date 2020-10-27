using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpartaMobileWebApi.Interfaces
{
    public interface IUserManagementUnitOfWork
    {
        IUserManagementRepository<TEntity> GetREpository<TEntity>() where TEntity : class;

        Task SaveChangesAsync();
        Task<int> SaveChanges();
    }
}
