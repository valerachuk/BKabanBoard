using BKabanApi.Models;
using BKabanApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BKabanApi.Controllers
{
    [Route("api/userData")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataRepository _userDataRepository;

        public UserDataController(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        [HttpGet]
        public ActionResult GetUserData()
        {
            int? userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(_userDataRepository.GetUserData((int) userId));
        }
    }
}