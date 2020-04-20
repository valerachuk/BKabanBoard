using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BKabanApi.Models.DB;

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
        public ActionResult getBoard()
        {
            return Ok(_fullBoardRepository.getUserBoard(1));
        }
    }
}