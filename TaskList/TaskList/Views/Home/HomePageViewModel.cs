using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using TaskList.Extensions;
using TaskList.Framework;
using TaskList.Helpers;
using TaskList.Models;
using TaskList.Services;
using TaskList.Views.Details;

namespace TaskList.Views.Home
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly ITaskService _taskService;

        public HomePageViewModel(INavigationService navigationService, ITaskService taskService) : base(navigationService)
        {
            _taskService = taskService;
            
            IsBusy = taskService.IsBusy.ToReactiveProperty().AddTo(Disposables);
            
            Tasks = taskService.ObserveTasks
                .Select(CreateReactiveCollection)
                .ToReactiveProperty()
                .AddTo(Disposables);
            
            IsBusy = new ReactiveProperty<bool>().AddTo(Disposables);
            AddTaskCommand = IsBusy.Inverse().ToReactiveCommand().WithSubscribeDisposing(OnAddTaskCommand, Disposables);
        }

        public ReactiveProperty<bool> IsBusy { get; }
        public ReactiveProperty<EnhancedReactiveCollection<TaskItemViewModel>> Tasks { get; }
        public ReactiveCommand AddTaskCommand { get; }
        
        private EnhancedReactiveCollection<TaskItemViewModel> CreateReactiveCollection(IList<TaskItem> items)
        {
            var viewModels = items.Select(x => new TaskItemViewModel(x, NavigationService).AddTo(Disposables)).ToList();
            var reactiveCollection = new EnhancedReactiveCollection<TaskItemViewModel>();
            reactiveCollection.AddRange(viewModels);
            return reactiveCollection;
        }

        private void OnAddTaskCommand()
        {
            var param = new NavigationParameters();
            param.Add(nameof(AddOrEditTaskPageParameter), new AddOrEditTaskPageParameter(null));
            NavigationService.NavigateAsync(nameof(AddOrEditTaskPage), param)
                .ToObservable()
                .Subscribe(x =>
                {
                    
                })
                .AddTo(Disposables);
        }
    }

    public class TaskItemViewModel : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly BusyNotifier _busyNotifier = new BusyNotifier();
        private readonly TaskItem _taskItem;
        private readonly INavigationService _navigationService;

        public TaskItemViewModel(TaskItem taskItem, INavigationService navigationService)
        {
            _taskItem = taskItem;
            _navigationService = navigationService;

            Title = new ReactiveProperty<string>(taskItem.Title).AddTo(_disposables);
            Description = new ReactiveProperty<string>(taskItem.Description).AddTo(_disposables);
            Status = new ReactiveProperty<TaskModelStatusEnum>(taskItem.TaskStatus).AddTo(_disposables);

            ViewDetailsCommand = _busyNotifier.Inverse()
                .ToReactiveCommand()
                .WithSubscribeDisposing(OnViewDetailsCommand, _disposables);
        }

        public int TaskId => _taskItem.TaskId;
        public DateTime CreatedOn => _taskItem.CreatedOn;
        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<string> Description { get; }
        public ReactiveProperty<TaskModelStatusEnum> Status { get; }
        
        public ReactiveCommand ViewDetailsCommand { get; }

        private void OnViewDetailsCommand()
        {
            var busy = _busyNotifier.ProcessStart();
            var param = new NavigationParameters();
            
            param.Add(nameof(AddOrEditTaskPageParameter), new AddOrEditTaskPageParameter(_taskItem));
            
            _navigationService.NavigateAsync(nameof(AddOrEditTaskPage), param)
                .ToObservable()
                .Finally(busy.Dispose)
                .Subscribe()
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}