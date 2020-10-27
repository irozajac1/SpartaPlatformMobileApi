using Microsoft.EntityFrameworkCore;
using SpartaMobileWebApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpartaMobileWebApi.Data
{
    public class AuthenticationContext : DbContext
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {    
        }

        public DbSet<SpartaUser> Users { get; set; }
    }
}
