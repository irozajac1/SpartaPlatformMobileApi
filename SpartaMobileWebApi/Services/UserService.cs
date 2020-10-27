using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpartaMobileWebApi.Data.Entities;
using SpartaMobileWebApi.Interfaces;
using SpartaPlatformMobileApi.Helpers;
using SpartaPlatformMobileApi.Interfaces;
using SpartaPlatformMobileApi.Models.Request;
using SpartaPlatformMobileApi.Models.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SpartaMobileWebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserManagementUnitOfWork _uow;
        readonly AppSettings _appSettings;

        public UserService(IUserManagementUnitOfWork uow, IOptions<AppSettings> appSettings)
        {
            _uow = uow;
            _appSettings = appSettings.Value;
        }

        public async Task<SpartaUser> Create(SpartaUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Lozinka je obavezna.");
            }
            var userExists = _uow.GetREpository<SpartaUser>().Any(x => x.UserName == user.UserName);

            if (userExists)
                throw new ArgumentNullException("Korisničko ime \"" + user.UserName + "\" već postoji.");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            try
            {
                _uow.GetREpository<SpartaUser>().Insert(user);
                await _uow.SaveChangesAsync();

                return user;
            }
            catch
            {
                throw;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("Password ne može biti nula.");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password ne može biti nula.");

            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task Delete(int id)
        {
            var user = await _uow.GetREpository<SpartaUser>().GetById(id);
            if(user != null)
            {
                _uow.GetREpository<SpartaUser>().Detele(user);
                await _uow.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SpartaUser>> GetAll()
        {
            return await _uow.GetREpository<SpartaUser>().GetAll();
        }

        public async Task<SpartaUser> GetById(int id)
        {
            return await _uow.GetREpository<SpartaUser>().GetById(id);
        }

        public async Task<AuthenticateResponse> Login(AuthenticateRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return null;

            var user = await _uow.GetREpository<SpartaUser>().FistOrDefault(x => x.UserName == request.Username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return null;

            var token = GenerateJwtToken(user);

            // authentication successful
            return new AuthenticateResponse(user, token);

        }

        private string GenerateJwtToken(SpartaUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public async Task Update(SpartaUser userRequest, string password = null)
        {
            var user = await _uow.GetREpository<SpartaUser>().GetById(userRequest.Id);

            if (user == null)
                throw new ArgumentNullException("Empty");

            if(!string.IsNullOrWhiteSpace(userRequest.UserName) && userRequest.UserName != user.UserName)
            {
                var userExists = _uow.GetREpository<SpartaUser>().Any(x => x.UserName == userRequest.UserName);
                if(userExists)
                    throw new ArgumentNullException("Username " + userRequest.UserName + " is already taken");

                user.UserName = userRequest.UserName;
            }

            if (!string.IsNullOrWhiteSpace(userRequest.FirstName))
                user.FirstName = userRequest.FirstName;

            if (!string.IsNullOrWhiteSpace(userRequest.LastName))
                user.LastName = userRequest.LastName;

            if (!string.IsNullOrWhiteSpace(userRequest.Unit))
                user.Unit = userRequest.Unit;

            if (!string.IsNullOrWhiteSpace(userRequest.Title))
                user.Title = userRequest.Title;

            if (!string.IsNullOrWhiteSpace(userRequest.Role))
                user.Role = userRequest.Role;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _uow.GetREpository<SpartaUser>().Update(user);
            await _uow.SaveChangesAsync();

        }
    }
}
