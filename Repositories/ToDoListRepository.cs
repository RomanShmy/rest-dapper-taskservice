using System.Collections.Generic;
using System.Linq;
using Dapper;
using Npgsql;
using rest_dapper_task.Models;
using rest_dapper_task.Repositories.interfaces;

namespace rest_dapper_task.Repositories
{
    public class ToDoListRepository : IToDoListRepository
    {
        private readonly ConnectionString connectionString;
        private ITaskRepository taskRepository;

        public ToDoListRepository(ConnectionString connectionString, ITaskRepository taskRepository)
        {
            this.connectionString = connectionString;
            this.taskRepository = taskRepository;
        }

        public ToDoList DeleteList(int id)
        {
            string query = "delete from task_app.to_do_list l where l.id = @Id returning *";
            if (!IsPresent(id))
            {
                return null;
            }

            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var list = connection.QueryFirst<ToDoList>(query, new {Id = id});
                return list;
            }
        }

        public ToDoList DeleteTaskFromList(int listId, int taskId)
        {
            if (!IsPresent(listId))
            {
                return null;
            }

            taskRepository.DeleteTask(taskId, listId);

            return GetList(listId);
        }


        public ToDoList GetList(int id)
        {
            if (!IsPresent(id))
            {
                return null;
            }

            string query =
                "select * from task_app.to_do_list l full join task_app.tasks t on l.id = t.todolist_id where l.id = @Id;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var listDictionary = new Dictionary<int, ToDoList>();

                var list = connection.Query<ToDoList, Task, ToDoList>(
                    query,
                    (toDoList, task) =>
                    {
                        ToDoList listEntry;

                        if (!listDictionary.TryGetValue(toDoList.Id, out listEntry))
                        {
                            listEntry = toDoList;
                            listEntry.Tasks = new List<Task>();
                            listDictionary.Add(listEntry.Id, listEntry);
                        }

                        if (task != null)
                        {
                            listEntry.Tasks.Add(task);
                        }

                        return listEntry;
                    },
                    new {Id = id},
                    splitOn: "id");

                return list.First();
            }
        }

        public List<ToDoList> GetLists()
        {
            string query = "select * from task_app.to_do_list l full join task_app.tasks t on l.id = t.todolist_id;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var listDictionary = new Dictionary<int, ToDoList>();


                var list = connection.Query<ToDoList, Task, ToDoList>(
                        query,
                        (toDoList, task) =>
                        {
                            ToDoList listEntry;

                            if (!listDictionary.TryGetValue(toDoList.Id, out listEntry))
                            {
                                listEntry = toDoList;
                                listEntry.Tasks = new List<Task>();
                                listDictionary.TryAdd(listEntry.Id, listEntry);
                            }

                            if (task != null)
                            {
                                listEntry.Tasks.Add(task);
                            }

                            return listEntry;
                        },
                        splitOn: "id")
                    .Distinct()
                    .ToList();
                return list;
            }
        }

        public ToDoList SaveList(ToDoList list)
        {
            string query = "insert into task_app.to_do_list (name) values(@Name) returning id";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var id = connection.QueryFirst<int>(query, new {Name = list.Name});
                list.Id = id;
                return list;
            }
        }

        public ToDoList SaveTaskToList(int id, Task task)
        {
            if (!IsPresent(id))
            {
                return null;
            }

            taskRepository.SaveTask(id, task);

            return GetList(id);
        }

        public ToDoList UpdateList(int id, ToDoList list)
        {
            if (!IsPresent(id))
            {
                return null;
            }

            string query = "update task_app.to_do_list set name = @Name where id = @Id;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                connection.Execute(query, new {Id = id, Name = list.Name});
                return GetList(id);
            }
        }

        public ToDoList UpdateTaskInList(int listId, int taskId, Task task)
        {
            if (!IsPresent(listId))
            {
                return null;
            }

            taskRepository.EditDone(listId, taskId, task);

            return GetList(listId);
        }

        private bool IsPresent(int id)
        {
            bool result = true;
            string query = "select exists(select 1 from task_app.to_do_list where id = @Id);";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                result = connection.QueryFirst<bool>(query, new {Id = id});
            }

            return result;
        }
    }
}