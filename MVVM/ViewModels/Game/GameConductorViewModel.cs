using System;
using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;
using CEOS.MVVM.Models.Game;

namespace CEOS.MVVM.ViewModels.Game
{
    public class GameConductorViewModel : Conductor<object>.Collection.OneActive
    {
        readonly IEventAggregator eventAggregator;

        readonly HomeViewModel HomeViewModel;
        readonly WorkersViewModel WorkersViewModel;
        readonly StorageViewModel StorageViewModel;
        readonly MarketsViewModel MarketsViewModel;

        public string CompanyName
        {
            get => GameManager.GetInstance().AllMarkets[0].FactoriesOnMarket[0].Name;
        }

        public GameConductorViewModel(IEventAggregator aggregator, HomeViewModel home, WorkersViewModel workers, StorageViewModel storage, MarketsViewModel markets)
        {
            HomeViewModel = home;
            WorkersViewModel = workers;
            StorageViewModel = storage;
            MarketsViewModel = markets;
            eventAggregator = aggregator;

            Items.Add(home);
            Items.Add(workers);
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            eventAggregator.SubscribeOnPublishedThread(this);
            ActivateItemAsync(HomeViewModel);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }


        public void LoadHomeView()
        {
            ActivateItemAsync(HomeViewModel);
        }

        public void LoadWorkersView()
        {
            ActivateItemAsync(WorkersViewModel);
        }
        
        public void LoadStorageView()
        {
            ActivateItemAsync(StorageViewModel);
        }
        public void LoadMarketsView()
        {
            ActivateItemAsync(MarketsViewModel);
        }
        public void Turn()
        {
            GameManager.GetInstance().Turn();
            //ActivateItemAsync(ActiveItem);
            NotifyOfPropertyChange(() => ActiveItem);
        }

        public void Save()
        {
            GameManager.GetInstance().Save();
        }
    }
}