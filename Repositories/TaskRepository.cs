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

        public Task DeleteTask(int id)
        {
            string query = "delete from task_app.tasks where id = @Id returning *;";
            using(var connection = new NpgsqlConnection(connectionString.Value))
            {
                var task = connection.QueryFirst<Task>(query, new {Id = id});
                return task;
            }
        }

        public Task EditDone(int id, Task task)
        {
            string query = "update task_app.tasks set done = @Done where id = @Id returning *;";
            using(var connection = new NpgsqlConnection(connectionString.Value))
            {
                var taskResult = connection.QueryFirst(query, new {Id = id, Done = task.Done});
                return taskResult;
            }
        }

        public Task GetTask(int id)
        {
            if(!IsPresent(id))
            {
                return null;
            }

            string query = "select * from task_app.tasks where id = @Id;";
            using(var connection = new NpgsqlConnection(connectionString.Value))
            {   
                var task = connection.QueryFirstOrDefault<Task>(query, new {Id = id}); 
                return task;
            }
 
        }

        public List<Task> GetTasks()
        {
            string query = "select * from task_app.tasks;";
            List<Task> tasks = new List<Task>();
            using(var connection = new NpgsqlConnection(connectionString.Value))
            {
                tasks.AddRange(connection.Query<Task>(query));
                return tasks;
            }
        }

        //TODO: correct relationship objects
        // public Task SaveTask(Task task)
        // {
        //     string query = "insert into task_app.tasks (name, done, todolist_id) values (@Name, @Done, @todolist_id) returning id;";
        //     using(var connection = new NpgsqlConnection(connectionString.Value))
        //     {
                
        //         var id = connection.QueryFirst<int>(query, new {Name = task.Name, Done = task.Done, todolist_id = 3});
        //         task.Id = id;
        //         return task;
        //     }
        // }

        public bool IsPresent(int id)
        {
            bool result = true;
            string query = "select exists(select 1 from fef.tasks where id = @Id);";
            using(var connection = new NpgsqlConnection(connectionString.Value))
            {
                result = connection.QueryFirst<bool>(query, new {Id = id});
            }
            return result;
        }
    }
}