using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEOS.MVVM.Models;
using Caliburn.Micro;
using CEOS.MVVM.Models.Game;
using System.Threading;

namespace CEOS.MVVM.ViewModels.Game
{
    public class StorageViewModel: Screen, IHandle<GameInitializedMessage>
    {
        readonly IEventAggregator eventAggregator;

        Factory playerFactory;

        BindableCollection<Storage> warehouse = new BindableCollection<Storage>();
        BindableCollection<Material> materials = new BindableCollection<Material>();
        BindableCollection<Material> produced = new BindableCollection<Material>();

        #region Properties
        public Factory PlayerFactory { get => playerFactory; set => playerFactory = value; }
        public BindableCollection<Storage> Warehouse
        {
            get
            {
                warehouse.Clear();
                foreach (var item in PlayerFactory.Storage)
                {
                    warehouse.Add(item);
                }
                return warehouse;
            }
        }

        public BindableCollection<Material> Materials
        {
            get
            {
                materials.Clear();
                foreach (var item in PlayerFactory.RawMaterials)
                {
                    materials.Add(item);
                }
                return materials;
            }
        }

        public BindableCollection<Material> Produced
        {
            get
            {
                produced.Clear();
                foreach (var item in PlayerFactory.ProducedMaterials)
                {
                    produced.Add(item);
                }
                return produced;
            }
        }


        #endregion
        #region Buttons methods
        public void ChangeStorageCapTarget(string newTarget)
        {
            if(!String.IsNullOrEmpty(newTarget))
            { 
                int temp = Convert.ToInt32(newTarget);
                PlayerFactory.StorageTarget += temp;
            }
        }

        #endregion



        #region Constructor, Events, Activators
        public StorageViewModel(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            eventAggregator.SubscribeOnBackgroundThread(this);
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            NotifyOfPropertyChange(() => PlayerFactory);
            NotifyOfPropertyChange(() => Warehouse);
            NotifyOfPropertyChange(() => Materials);
            NotifyOfPropertyChange(() => Produced);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public Task HandleAsync(GameInitializedMessage message, CancellationToken cancellationToken)
        {
            PlayerFactory = GameManager.GetInstance().AllMarkets[0].FactoriesOnMarket[0];
            return Task.CompletedTask;
        }

        #endregion
    }
}
