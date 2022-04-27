using TaskList.Models;
using TaskList.Services;

namespace TaskList.Extensions
{
    public static class ExtensionsOfTaskModel
    {
        public static TaskItem ToTaskItem(this TaskModel taskModel)
        {
            return new TaskItem
            {
                Description = taskModel.Description,
                Title = taskModel.Title,
                CreatedOn = taskModel.CreatedOn,
                TaskId = taskModel.TaskId,
                TaskStatus = taskModel.TaskModelStatusEnum,
                UpdatedOn = taskModel.UpdatedOn
            };
        }
    }
}