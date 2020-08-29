using System.Collections.Generic;
using System.Linq;
using Dapper;
using Npgsql;
using rest_dapper_task.Models;
using rest_dapper_task.Repositories.interfaces;

namespace rest_dapper_task.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ConnectionString connectionString;

        public TaskRepository(ConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public Task DeleteTask(int taksId, int listId)
        {
            if (!IsPresent(taksId))
            {
                return null;
            }

            string query =
                "delete from task_app.tasks where id = @TaskId and todolist_id = @ListId returning id, name, done;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var task = connection.QueryFirst<Task>(query, new {TaskId = taksId, ListId = listId});
                return task;
            }
        }

        public Task EditDone(int listId, int taskId, Task task)
        {
            if (!IsPresent(taskId))
            {
                return null;
            }

            string query =
                "update task_app.tasks set done = @Done where id = @TaskId and todolist_id = @ListId returning id, name, done;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var taskResult =
                    connection.QueryFirst<Task>(query, new {TaskId = taskId, ListId = listId, Done = task.Done});
                return taskResult;
            }
        }

        public Task GetTask(int id)
        {
            if (!IsPresent(id))
            {
                return null;
            }

            string query = "select * from task_app.tasks where id = @Id;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var task = connection.QueryFirstOrDefault<Task>(query, new {Id = id});
                return task;
            }
        }

        public List<Task> GetTasks()
        {
            string query = "select * from task_app.tasks;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var tasks = connection.Query<Task>(query);
                return tasks.AsList();
            }
        }


        public bool IsPresentInList(int taskId, int listId)
        {
            string query = "select todolist_id from task_app.tasks where id = @Id;";
            if (!IsPresent(taskId))
            {
                return false;
            }

            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var id = connection.QueryFirst<int>(query, new {Id = taskId});
                if (id != listId)
                {
                    return false;
                }
            }

            return true;
        }

        public Task SaveTask(int todolist_id, Task task)
        {
            string query =
                "insert into task_app.tasks (name, done, todolist_id) values (@Name, @Done, @ToDoId) returning id;";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                var id = connection.QueryFirst<int>(query,
                    new {Name = task.Name, Done = task.Done, ToDoId = todolist_id});
                task.Id = id;
                return task;
            }
        }

        private bool IsPresent(int id)
        {
            bool result = true;
            string query = "select exists(select 1 from task_app.tasks where id = @Id);";
            using (var connection = new NpgsqlConnection(connectionString.Value))
            {
                result = connection.QueryFirst<bool>(query, new {Id = id});
            }

            return result;
        }
    }
}