using TaskList.Models;
using TaskList.Services;

namespace TaskList.Extensions
{
    public static class ExtensionsOfTaskItem
    {
        public static TaskModel ToTaskModel(this TaskItem taskItem)
        {
            return new TaskModel
            {
                TaskId = taskItem.TaskId,
                Title = taskItem.Title,
                Description = taskItem.Description,
                TaskModelStatusEnum = taskItem.TaskStatus
            };
        }
    }
}