using System;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using SQLite;
using TaskList.Bootstraping;
using TaskList.Models;

namespace TaskList.Repository
{
    public interface ITaskRepository
    {
        IObservable<IEnumerable<TaskModel>> GetTasks();
        IObservable<TaskModel> GetTask(int taskId);
        IObservable<int> Insert(TaskModel taskModel);
        IObservable<int> Update(TaskModel taskModel);
        IObservable<int> Delete(int taskId);
    }
    
    [Singleton]
    public class TaskRepository : ITaskRepository
    {
        private readonly SQLiteAsyncConnection _connection;

        public TaskRepository(ISqliteDb sqliteDb)
        {
            _connection = sqliteDb.GetDb();
            _connection.CreateTableAsync<TaskModel>().Wait();
        }
        
        public IObservable<IEnumerable<TaskModel>> GetTasks()
        {
            return _connection.Table<TaskModel>().ToListAsync().ToObservable();
        }

        public IObservable<TaskModel> GetTask(int taskId)
        {
            return _connection.FindAsync<TaskModel>(taskId).ToObservable();
        }

        public IObservable<int> Insert(TaskModel taskModel)
        {
            return _connection.InsertAsync(taskModel).ToObservable();
        }
        
        public IObservable<int> Update(TaskModel taskModel)
        {
            return _connection.UpdateAsync(taskModel).ToObservable();
        }

        public IObservable<int> Delete(int taskId)
        {
            return _connection.DeleteAsync(taskId, new TableMapping(typeof(TaskModel))).ToObservable();
        }
    }
}