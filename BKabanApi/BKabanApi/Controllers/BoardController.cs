using BKabanApi.Models;
using Microsoft.AspNetCore.Mvc;
using BKabanApi.Models.DB;
using BKabanApi.Utils;

namespace BKabanApi.Controllers
{
    [Route("api/board")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly IFullBoardRepository _fullBoardRepository;

        public BoardController(IFullBoardRepository fullBoardRepository)
        {
            _fullBoardRepository = fullBoardRepository;
        }

        [HttpGet]
        public ActionResult GetBoard()
        {
            int? userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(_fullBoardRepository.GetUserBoard((int)userId));

        }

        [HttpPut("rename")]
        public ActionResult RenameBoard(BoardModel board)
        {
            int? userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
            {
                return Unauthorized();
            }

            int? result = _fullBoardRepository.UpdateBoardName((int)userId, board);

            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok();
        }

        
    }
}