using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SpartaMobileWebApi.Data.Entities;
using SpartaMobileWebApi.Models.Request;
using SpartaPlatformMobileApi.Interfaces;
using SpartaPlatformMobileApi.Models.Request;

namespace SpartaMobileWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUserService userService;
        readonly IMapper mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            this.mapper = mapper;
            this.userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticateRequest request)
        {
            var response = await userService.Login(request);
            if (response == null)
                return BadRequest("Molim Vas unesite Vaše podatke.");

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult>Registration (RegistrationRequest request)
        {
            var user = mapper.Map<SpartaUser>(request);
            try
            {
                await userService.Create(user, request.Password);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await userService.GetAll();
            var result = mapper.Map<IList<SpartaUser>>(users);
            return Ok(result);
        }

    }
}
