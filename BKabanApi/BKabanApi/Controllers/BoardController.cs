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
        private readonly IBoardRepository _boardRepository;

        public BoardController(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        [HttpPost]
        public ActionResult CreateBoard(BoardModel board)
        {
            int? userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
            {
                return Unauthorized();
            }

            int? insertedId = _boardRepository.CreateBoard((int) userId, board);

            if (insertedId == null)
            {
                return StatusCode(403);
            }

            return Ok(new {id = insertedId});

        }

        [HttpGet("{id}")]
        public ActionResult GetBoard(int id)
        {
            int? userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
            {
                return Unauthorized();
            }

            var board = _boardRepository.GetFullBoard((int) userId, id);

            if (board == null)
            {
                return StatusCode(403);
            }

            return Ok(board);
        }

        [HttpPut]
        public ActionResult RenameBoard(BoardModel board)
        {
            int? userId = AuthHelper.GetUserId(HttpContext);

            if (board.Id == null)
            {
                return BadRequest();
            }

            if (userId == null)
            {
                return Unauthorized();
            }

            bool result = _boardRepository.UpdateBoardName((int) userId, board);

            if (result)
            {
                return Ok();
            }

            return StatusCode(403);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteBoard(int id)
        {
            int? userId = AuthHelper.GetUserId(HttpContext);

            if (userId == null)
            {
                return Unauthorized();
            }

            bool result = _boardRepository.DeleteBoard((int)userId, id);

            if (result)
            {
                return Ok();
            }

            return StatusCode(403);
        }
    }
}