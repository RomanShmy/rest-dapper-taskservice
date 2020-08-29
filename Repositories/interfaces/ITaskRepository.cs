using System.Collections.Generic;
using rest_dapper_task.Models;

namespace rest_dapper_task.Repositories.interfaces
{
    public interface ITaskRepository
    {
        List<Task> GetTasks();
        Task GetTask(int id);
        Task SaveTask(int todolist_id, Task task);
        Task EditDone(int listId, int taskId, Task task);
        Task DeleteTask(int taskId, int listId);
        bool IsPresentInList(int taskId, int listId);
    }
}