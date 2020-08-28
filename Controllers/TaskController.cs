using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using rest_dapper_task.Models;
using rest_dapper_task.Repositories.interfaces;

namespace rest_dapper_task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private ITaskRepository repository;

        public TaskController(ITaskRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult<List<Task>> GetTasks()
        {
            return Ok(repository.GetTasks());
        }

        [HttpGet("{id}")]
        public ActionResult<Task> GetTaskByIdAsync(int id)
        {
            Task task = repository.GetTask(id);
            if (task == null)
            {
                return NotFound(new {Messege = $"Don't have data with id = {id}"});
            }

            return Ok(task);
        }

        

    }
}