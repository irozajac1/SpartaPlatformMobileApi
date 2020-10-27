using AutoMapper;
using SpartaMobileWebApi.Data.Entities;
using SpartaMobileWebApi.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpartaPlatformMobileApi.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegistrationRequest, SpartaUser>();
        }
    }
}
