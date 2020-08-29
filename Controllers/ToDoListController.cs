using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using rest_dapper_task.Models;
using rest_dapper_task.Repositories.interfaces;

namespace rest_dapper_task.Controllers
{
    [ApiController]
    [Route("api/list")]
    public class ToDoListController : ControllerBase
    {
        private IToDoListRepository listRepository;
        private ITaskRepository taskRepository;

        public ToDoListController(IToDoListRepository listRepository, ITaskRepository taskRepository)
        {
            this.listRepository = listRepository;
            this.taskRepository = taskRepository;
        }

        [HttpGet]
        public ActionResult<List<ToDoList>> GetLists()
        {
            return Ok(listRepository.GetLists());
        }

        [HttpGet("{id}")]
        public ActionResult<ToDoList> GetList(int id)
        {
            var list = listRepository.GetList(id);
            if (list == null)
            {
                return NotFound(new {Message = $"Don't have list with id = {id}"});
            }

            return Ok(list);
        }

        [HttpPost]
        public ActionResult<ToDoList> SaveList([FromBody] ToDoList list)
        {
            var request = HttpContext.Request;
            return new CreatedResult(request.Host.Value + request.Path.Value, listRepository.SaveList(list));
        }

        [HttpPatch("{id}")]
        public ActionResult<ToDoList> UpdateList(int id, [FromBody] ToDoList list)
        {
            ToDoList toDoList = listRepository.UpdateList(id, list);
            if (list == null)
            {
                return NotFound(new {Message = $"Don't have list with id = {id}"});
            }

            return toDoList;
        }

        [HttpDelete("{id}")]
        public ActionResult<ToDoList> DeleteList(int id)
        {
            ToDoList list = listRepository.DeleteList(id);
            if (list == null)
            {
                return NotFound(new {Message = $"Don't have list with id = {id}"});
            }

            return list;
        }

        [HttpPost("{id}/task")]
        public ActionResult<ToDoList> SaveTaskToList(int id, [FromBody] Task task)
        {
            var list = listRepository.SaveTaskToList(id, task);
            if (list == null)
            {
                return NotFound(new {Message = $"Don't have list with id = {id}"});
            }

            return Ok(list);
        }

        [HttpPatch("{listId}/task/{taskId}")]
        public ActionResult<ToDoList> UpdateTaskInList(int listId, int taskId, [FromBody] Task task)
        {
            if (!taskRepository.IsPresentInList(taskId, listId))
            {
                return NotFound(new {Message = $"Don't have task with id = {taskId} or list don't have it"});
            }

            var list = listRepository.UpdateTaskInList(listId, taskId, task);
            if (list == null)
            {
                return NotFound(new {Message = $"Don't have list with id = {listId}"});
            }


            return Ok(list);
        }

        [HttpDelete("{listId}/task/{taskId}")]
        public ActionResult<ToDoList> DeleteTaskFromList(int listId, int taskId)
        {
            if (!taskRepository.IsPresentInList(taskId, listId))
            {
                return NotFound(new {Message = $"Don't have task with id = {taskId} or list don't have it"});
            }

            var list = listRepository.DeleteTaskFromList(listId, taskId);
            if (list == null)
            {
                return NotFound(new {Message = $"Don't have list with id = {listId}"});
            }

            return Ok(list);
        }
    }
}