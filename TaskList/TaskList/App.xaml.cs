using System;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using TaskList.Bootstraping;
using TaskList.Extensions;
using TaskList.Views.Details;
using TaskList.Views.Home;
using Xamarin.Forms;

namespace TaskList
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }
        
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.GetContainer().BootstrapTypes(new[] { typeof(App).Assembly });
            
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<AddOrEditTaskPage, AddOrEditTaskPageViewModel>();
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

           var result = await NavigationService.NavigateAsync(nameof(HomePage).AsNavigation());
           if (result.Success)
           {
               
           }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}