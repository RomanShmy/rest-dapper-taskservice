using System.Collections.Generic;

namespace rest_dapper_task.Models
{
    public class ToDoList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}