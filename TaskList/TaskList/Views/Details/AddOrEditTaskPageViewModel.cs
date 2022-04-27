using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Prism.Common;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TaskList.Extensions;
using TaskList.Framework;
using TaskList.Models;
using TaskList.Services;
using TaskList.Views.Home;

namespace TaskList.Views.Details
{
    public class AddOrEditTaskPageViewModel : ViewModelBase
    {
        private readonly ITaskService _taskService;
        private readonly IPageDialogService _pageDialogService;

        public AddOrEditTaskPageViewModel(INavigationService navigationService, 
                                          ITaskService taskService,
                                          IPageDialogService pageDialogService) : base(navigationService)
        {
            _taskService = taskService;
            _pageDialogService = pageDialogService;

            TaskItem = new ReactiveProperty<TaskItem>().AddTo(Disposables);
            IsBusy = new ReactiveProperty<bool>().AddTo(Disposables);
            TaskTitle = new ReactiveProperty<string>().AddTo(Disposables);
            TaskDescription = new ReactiveProperty<string>().AddTo(Disposables);
            IsCompleted = new ReactiveProperty<bool>().AddTo(Disposables);

            Title = TaskItem
                .Select(x => x == null ? "Add Task" : "Edit Task")
                .ToReactiveProperty()
                .AddTo(Disposables);

            SaveCommand = IsBusy.Inverse()
                .ToReactiveCommand()
                .WithSubscribeDisposing(OnSaveCommand, Disposables);

            DeleteCommand = IsBusy.Inverse()
                .ToReactiveCommand()
                .WithSubscribeDisposing(OnDeleteCommand, Disposables);
        }
        
        public ReactiveProperty<TaskItem> TaskItem { get; }
        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<bool> IsBusy { get; }
        
        public ReactiveProperty<string> TaskTitle { get; }
        public ReactiveProperty<string> TaskDescription { get; }
        public ReactiveProperty<bool> IsCompleted { get; }
        
        public ReactiveCommand SaveCommand { get; }
        public ReactiveCommand DeleteCommand { get; }

        public override void Initialize(INavigationParameters parameters)
        {
            parameters.TryGetValue<AddOrEditTaskPageParameter>(nameof(AddOrEditTaskPageParameter), out var param);
            TaskItem.Value = param?.TaskItem;

            if (TaskItem.Value == null) return;

            TaskTitle.Value = TaskItem.Value.Title;
            TaskDescription.Value = TaskItem.Value.Description;
            IsCompleted.Value = TaskItem.Value.TaskStatus == TaskModelStatusEnum.Completed;
        }

        private void OnSaveCommand()
        {
            if (IsBusy.Value) return;
            
            IsBusy.Value = true;

            if (TaskItem.Value == null)
            {
                _taskService.AddTask(MapViewModelToModel())
                    .ObserveOnUIDispatcher()
                    .Finally(() => IsBusy.Value = false)
                    .Subscribe(x =>
                    {
                        _pageDialogService.DisplayAlertAsync("Success", "Task added successfully!", "OK")
                            .ToObservable()
                            .Subscribe()
                            .AddTo(Disposables);
                        
                        NavigationService.GoBackAsync()
                            .ToObservable()
                            .Subscribe()
                            .AddTo(Disposables);
                    }, ex =>
                    {
                        _pageDialogService.DisplayAlertAsync("Error",
                                "Something went wrong trying to add this task, try again later", "OK")
                            .ToObservable()
                            .Subscribe()
                            .AddTo(Disposables);
                    })
                    .AddTo(Disposables);

                return;
            }

            _taskService.UpdateTask(MapViewModelToModel())
                .ObserveOnUIDispatcher()
                .Finally(() => IsBusy.Value = false)
                .Subscribe(x =>
                {
                    _pageDialogService.DisplayAlertAsync("Success", "Task added successfully!", "OK");
                    
                    NavigationService.GoBackAsync()
                        .ToObservable()
                        .Subscribe()
                        .AddTo(Disposables);

                }, ex =>
                { 
                    _pageDialogService.DisplayAlertAsync("Error",
                        "Something went wrong trying to update this task, try again later", "OK")
                    .ToObservable()
                    .Subscribe()
                    .AddTo(Disposables);

                }).AddTo(Disposables);

        }

        private void OnDeleteCommand()
        {
            if (IsBusy.Value) return;
            
            IsBusy.Value = true;

            _taskService.DeleteTask(TaskItem.Value.TaskId)
                .ObserveOnUIDispatcher()
                .Finally(() => IsBusy.Value = false)
                .Subscribe(_ =>
                {
                    _pageDialogService.DisplayAlertAsync("Success", "Task deleted successfully!", "OK");
                    
                    NavigationService.GoBackAsync()
                        .ToObservable()
                        .Subscribe()
                        .AddTo(Disposables);
            
                }, ex =>
                {
                    _pageDialogService.DisplayAlertAsync("Error",
                            "Something went wrong trying to delete this task, try again later", "OK")
                        .ToObservable()
                        .Subscribe()
                        .AddTo(Disposables);
                }).AddTo(Disposables);
        }

        private TaskItem MapViewModelToModel()
        {
            if (TaskItem.Value == null)
            {
                return new TaskItem
                {
                    Description = TaskDescription.Value,
                    Title = TaskTitle.Value,
                    CreatedOn = DateTime.Now,
                    TaskStatus = IsCompleted.Value ? TaskModelStatusEnum.Completed : TaskModelStatusEnum.Pending,
                };
            }
            
            return new TaskItem
            {
                Description = TaskDescription.Value,
                Title = TaskTitle.Value,
                UpdatedOn = DateTime.Now,
                TaskStatus = IsCompleted.Value ? TaskModelStatusEnum.Completed : TaskModelStatusEnum.Pending,
            };
        }
    }

    public class AddOrEditTaskPageParameter
    {
        public AddOrEditTaskPageParameter(TaskItem taskItemViewModel)
        {
            TaskItem = taskItemViewModel;
        }
        
        public TaskItem TaskItem { get; }
    }
}