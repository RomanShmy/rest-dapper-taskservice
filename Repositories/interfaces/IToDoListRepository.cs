using System.Collections.Generic;
using rest_dapper_task.Models;

namespace rest_dapper_task.Repositories.interfaces
{
    public interface IToDoListRepository
    {
       ToDoList GetList(int id);
       List<ToDoList> GetLists();
       ToDoList SaveList(ToDoList list);
       
       
    }
}