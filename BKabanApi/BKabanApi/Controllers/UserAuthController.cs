using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BKabanApi.Models;
using BKabanApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKabanApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserAuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("isLogged")]
        public IActionResult IsLogged()
        {
            int? userId = AuthHelper.GetUserId(HttpContext);
            return Ok(new {isLogged = userId != null});
        }

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            AuthHelper.LogOut(HttpContext);
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(UserModel user)
        {
            int? userId;
            if ((userId = _userRepository.GetUserIdFullMatch(user)) != null)
            {
                AuthHelper.AddUserToSession(HttpContext, (int) userId);
                return Ok();
            }

            return BadRequest("Invalid username or password");
        }

        [HttpPost("register")]
        public IActionResult Register(UserModel user)
        {
            if (_userRepository.GetUserIdByUsername(user.Username) != null)
            {
                return BadRequest("This username is already used!");
            }

            int? userId = _userRepository.Create(user);

            if (userId == null)
            {
                return StatusCode(500);
            }

            AuthHelper.AddUserToSession(HttpContext, (int) userId);

            return Ok();
        }
    }
}