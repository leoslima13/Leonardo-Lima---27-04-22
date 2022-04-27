using System;
using SQLite;

namespace TaskList.Models
{
    public class TaskModel
    {
        [PrimaryKey, AutoIncrement]
        public int TaskId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public TaskModelStatusEnum TaskModelStatusEnum { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public enum TaskModelStatusEnum
    {
        Pending,
        Completed,
        Deleted
    }
}