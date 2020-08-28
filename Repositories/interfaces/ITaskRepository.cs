using System.Collections.Generic;
using rest_dapper_task.Models;

namespace rest_dapper_task.Repositories.interfaces
{
    public interface ITaskRepository
    {
        List<Task> GetTasks();
        Task GetTask(int id);
        // Task SaveTask(Task task);
        Task EditDone(int id, Task task);
        Task DeleteTask(int id);
        bool IsPresent(int id);
    }
}