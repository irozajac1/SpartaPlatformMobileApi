using SpartaMobileWebApi.Data.Entities;
using SpartaPlatformMobileApi.Models.Request;
using SpartaPlatformMobileApi.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpartaPlatformMobileApi.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Login(AuthenticateRequest request);
        Task<IEnumerable<SpartaUser>> GetAll();
        Task<SpartaUser> GetById(int id);
        Task<SpartaUser> Create(SpartaUser user, string password);
        Task Update(SpartaUser user, string password = null);
        Task Delete(int id);
    }
}
