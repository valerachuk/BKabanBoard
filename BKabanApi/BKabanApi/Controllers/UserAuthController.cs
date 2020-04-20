using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BKabanApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKabanApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserAuthController : ControllerBase //authController 
    {
        private readonly IUserRepository _userRepository;

        public UserAuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("id")]
        public IActionResult getId()
        {
            int? userId;
            if ((userId = HttpContext.Session.GetInt32("userId")) != null)
            {
                return Ok(userId);
            }

            return BadRequest();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(UserCredentials user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            int? userId;
            if ((userId = _userRepository.GetUserIdFullMatch(user.Email, user.Password)) != null)
            {
                HttpContext.Session.SetInt32("userId", (int)userId);
                return Ok();
            }

            return BadRequest("Invalid username or password");
        }

        [HttpPost("register")]
        public IActionResult Register(UserCredentials user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (_userRepository.GetIdByEmail(user.Email) != null)
            {
                return BadRequest("This email is already used!");
            }

            _userRepository.Create(user);
            return Login(user);
        }
    }
}