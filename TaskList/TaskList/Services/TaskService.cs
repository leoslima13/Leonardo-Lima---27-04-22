using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using TaskList.Bootstraping;
using TaskList.Extensions;
using TaskList.Models;
using TaskList.Repository;

namespace TaskList.Services
{
    public interface ITaskService
    {
        IObservable<bool> IsBusy { get; }
        IObservable<IList<TaskItem>> ObserveTasks { get; }
        IObservable<int> AddTask(TaskItem taskItem);
        IObservable<int> UpdateTask(TaskItem taskItem);
        IObservable<int> DeleteTask(int taskId);
    }
    
    [Singleton]
    public class TaskService : ITaskService, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ITaskRepository _taskRepository;
        private readonly BusyNotifier _busyNotifier;
        private readonly BehaviorSubject<IList<TaskItem>> _tasks;
        
        private bool _isInitialized;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
            _busyNotifier = new BusyNotifier();
            _tasks = new BehaviorSubject<IList<TaskItem>>(new List<TaskItem>()).AddTo(_disposables);
        }

        public IObservable<bool> IsBusy => _busyNotifier;
        public IObservable<IList<TaskItem>> ObserveTasks => EnsureInitialized(_tasks);
        
        public IObservable<int> AddTask(TaskItem taskItem)
        {
            var taskModel = taskItem.ToTaskModel();
            taskModel.CreatedOn = DateTime.Now;
            return _taskRepository.Insert(taskModel)
                .Do(x =>
                {
                    if (x < 0) return;

                    taskItem.TaskId = x;
                    _tasks.Value.Add(taskItem);
                    _tasks.OnNext(_tasks.Value);
                });
        }
        
        public IObservable<int> UpdateTask(TaskItem taskItem)
        {
            var taskModel = taskItem.ToTaskModel();
            taskModel.UpdatedOn = DateTime.Now;
            return _taskRepository.Update(taskModel)
                .Do(x =>
                {
                    if (x <= 0) return;

                    var item = _tasks.Value.FirstOrDefault(t => t.TaskId == taskItem.TaskId);
                    var index = _tasks.Value.IndexOf(item);
                    _tasks.Value[index] = taskItem;
                    _tasks.OnNext(_tasks.Value);
                });
        }

        public IObservable<int> DeleteTask(int taskId)
        {
            return _taskRepository.Delete(taskId)
                .Do(x =>
                {
                    if (x <= 0) return;

                    var item = _tasks.Value.FirstOrDefault(t => t.TaskId == taskId);
                    _tasks.Value.Remove(item);
                    _tasks.OnNext(_tasks.Value);
                });
        }

        private IObservable<T> EnsureInitialized<T>(IObservable<T> observable)
        {
            if (_isInitialized)
                return observable;

            _isInitialized = true;

            var busy = _busyNotifier.ProcessStart();

            _taskRepository.GetTasks()
                .Finally(busy.Dispose)
                .Subscribe(x =>
                {
                    _tasks.OnNext(x?.Select(y => y.ToTaskItem()).ToList());
                }).AddTo(_disposables);

            return observable;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
    
    public class TaskItem
    {
        public TaskItem()
        {
        }

        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskModelStatusEnum TaskStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}