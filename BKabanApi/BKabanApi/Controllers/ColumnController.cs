using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BKabanApi.Models;
using BKabanApi.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BKabanApi.Controllers
{
    [Route("api/column")]
    [ApiController]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnRepository _columnRepository;

        public ColumnController(IColumnRepository columnRepository)
        {
            _columnRepository = columnRepository;
        }

        [HttpPost]
        public ActionResult createColumn(ColumnModel column)
        {
            int? userId;

            if ((userId = HttpContext.Session.GetInt32("userId")) == null)
            {
                return Unauthorized();
            }

            int? columnId = _columnRepository.createColumn((int) userId, column);

            if (columnId == null)
            {
                return Forbid();
            }

            return Ok(new {id = columnId});
        }

        [HttpPut]
        public ActionResult updateColumn(ColumnModel column)
        {
            int? userId;

            if ((userId = HttpContext.Session.GetInt32("userId")) == null)
            {
                return Unauthorized();
            }

            if (column.Id == null)
            {
                return BadRequest();
            }

            bool result = _columnRepository.updateColumn((int)userId, column);

            if (result)
            {
                return Ok();
            }

            return Forbid();
        }


        [HttpDelete]
        public ActionResult deleteColumn(ColumnModel column)
        {
            int? userId;

            if ((userId = HttpContext.Session.GetInt32("userId")) == null)
            {
                return Unauthorized();
            }

            if (column.Id == null)
            {
                return BadRequest();
            }

            bool result = _columnRepository.deleteColumn((int)userId, column);

            if (result)
            {
                return Ok();
            }

            return Forbid();
        }
    }
}