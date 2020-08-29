using System.Collections.Generic;
using rest_dapper_task.Models;

namespace rest_dapper_task.Repositories.interfaces
{
    public interface IToDoListRepository
    {
        ToDoList GetList(int id);
        List<ToDoList> GetLists();
        ToDoList SaveList(ToDoList list);
        ToDoList UpdateList(int id, ToDoList list);
        ToDoList DeleteList(int id);
        ToDoList SaveTaskToList(int id, Task task);
        ToDoList UpdateTaskInList(int listId, int taskId, Task task);
        ToDoList DeleteTaskFromList(int listId, int taskId);
    }
}