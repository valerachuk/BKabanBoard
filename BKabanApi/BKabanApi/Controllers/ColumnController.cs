﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BKabanApi.Models;
using BKabanApi.Models.DB;
using BKabanApi.Utils;
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
        public ActionResult CreateColumn(ColumnModel column)
        {
            int? userId;

            if ((userId = AuthHelper.GetUserId(HttpContext)) == null)
            {
                return Unauthorized();
            }

            int? columnId = _columnRepository.CreateColumn((int) userId, column);

            if (columnId == null)
            {
                return BadRequest();
            }

            return Ok(new {id = columnId});
        }

        [HttpPut]
        public ActionResult UpdateColumn(ColumnModel column)
        {
            int? userId;

            if ((userId = AuthHelper.GetUserId(HttpContext)) == null)
            {
                return Unauthorized();
            }

            if (column.Id == null)
            {
                return BadRequest();
            }

            bool result = _columnRepository.UpdateColumn((int)userId, column);

            if (result)
            {
                return Ok();
            }

            return StatusCode(403);
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteColumn(int id)
        {
            int? userId;

            if ((userId = AuthHelper.GetUserId(HttpContext)) == null)
            {
                return Unauthorized();
            }

            bool result = _columnRepository.DeleteColumn((int)userId, id);

            if (result)
            {
                return Ok();
            }

            return StatusCode(403);
        }
    }
}