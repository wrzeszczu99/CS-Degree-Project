using Caliburn.Micro;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CEOS.MVVM.ViewModels;
using CEOS.MVVM.ViewModels.Game;
using CEOS.MVVM.ViewModels.Setup;

namespace CEOS
{
    public class Bootstraper : BootstrapperBase
    {
        private readonly SimpleContainer container = new SimpleContainer();

        public Bootstraper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            base.Configure();
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();

            //Conductors
            container.Singleton<ShellViewModel>();
            container.Singleton<SetUpConductorViewModel>();
            container.Singleton<GameConductorViewModel>();


            //Screens 
            container.Singleton<HomeViewModel>();
            container.Singleton<WorkersViewModel>();
            container.Singleton<StorageViewModel>();
            container.Singleton<MarketsViewModel>();
            container.Singleton<ScenarioSetUpViewModel>();
        
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetExecutingAssembly() };
        }
    }
}
