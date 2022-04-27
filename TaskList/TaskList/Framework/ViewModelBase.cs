using System.Reactive.Disposables;
using Prism.Navigation;

namespace TaskList.Framework
{
    public class ViewModelBase : IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; }
        protected CompositeDisposable Disposables { get; }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
            Disposables = new CompositeDisposable();
        }
        
        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public void Destroy()
        {
            Disposables.Dispose();
        }
    }
}