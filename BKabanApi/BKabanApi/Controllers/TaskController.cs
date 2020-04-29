using BKabanApi.Models;
using BKabanApi.Models.DB;
using BKabanApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BKabanApi.Controllers
{
    [Route("api/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpPost]
        public ActionResult CreateTask(TaskModelColumnLink task)
        {
            int? userId;

            if ((userId = AuthHelper.GetUserId(HttpContext)) == null)
            {
                return Unauthorized();
            }

            int? taskId = _taskRepository.CreateTask((int) userId, task);

            if (taskId == null)
            {
                return StatusCode(403);
            }

            return Ok(new { id = taskId });
        }

        [HttpPut]
        public ActionResult UpdateTask(TaskModel task)
        {
            int? userId;

            if ((userId = AuthHelper.GetUserId(HttpContext)) == null)
            {
                return Unauthorized();
            }

            if (task.Id == null || task.Name == null && task.Description == null)
            {
                return BadRequest();
            }

            bool result = _taskRepository.UpdateTask((int) userId, task);

            if (result)
            {
                return Ok();
            }

            return StatusCode(403);
        }

        [HttpPut("reorder")]
        public ActionResult MoveTask(TaskModelWithPositionAndNewColumn task)
        {
            int? userId;

            if ((userId = AuthHelper.GetUserId(HttpContext)) == null)
            {
                return Unauthorized();
            }

            if (task.Id == null)
            {
                return BadRequest();
            }

            bool result = _taskRepository.UpdateTaskPositionAndColumn((int)userId, task);

            if (result)
            {
                return Ok();
            }

            return StatusCode(403);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTask(int id)
        {
            int? userId;

            if ((userId = AuthHelper.GetUserId(HttpContext)) == null)
            {
                return Unauthorized();
            }

            bool result = _taskRepository.DeleteTask((int)userId, id);

            if (result)
            {
                return Ok();
            }

            return StatusCode(403);
        }
    }
}