using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using CEOS.MVVM.Models.Game;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace CEOS.MVVM.ViewModels.Game
{
    public class HomeViewModel : Screen, IHandle<GameInitializedMessage>
    {
        readonly IEventAggregator eventAggregator;
        //new KeyValuePair<string, float>("Max worker amount", PlayerFactory.MaxAdminWorkerAmountActual + PlayerFactory.MaxFactoryWorkerAmountActual - PlayerFactory.FactoryWorkerActual - PlayerFactory.AdminWorkerActual),
        // KeyValuePair<string, float>("Administration Workers", PlayerFactory.AdminWorkerActual),
        //new KeyValuePair<string, float>("Production Workers", PlayerFactory.FactoryWorkerActual)

        Factory playerFactory;
        SeriesCollection workerSeries;
        SeriesCollection storageSeries;
        SeriesCollection marketSeries;

        public Factory PlayerFactory { get => playerFactory; set => playerFactory = value; }

        public SeriesCollection WorkerPieChart
        {
            get
            {
                workerSeries = new SeriesCollection();
                PieSeries maxFactory = new PieSeries();
                PieSeries maxAdmin = new PieSeries();
                PieSeries factory = new PieSeries();
                PieSeries admin = new PieSeries();

                maxFactory.Title = "Możliwość zatrudnienia pracowników fabrycznych";
                maxFactory.Values = new ChartValues<int> { PlayerFactory.MaxFactoryWorkerAmountActual - PlayerFactory.FactoryWorkerActual};
                maxFactory.DataLabels = true;

                maxAdmin.Title = "Możliwość zatrudnienia pracowników administracyjnych";
                maxAdmin.Values = new ChartValues<int> { PlayerFactory.MaxAdminWorkerAmountActual  - PlayerFactory.AdminWorkerActual };
                maxAdmin.DataLabels = true;

                factory.Title = "Pracownicy fabryczni";
                factory.Values = new ChartValues<int> { PlayerFactory.FactoryWorkerActual };
                factory.DataLabels = true;
                
                admin.Title = "Pracownicy administracyjni";
                admin.Values = new ChartValues<int> { PlayerFactory.AdminWorkerActual };
                admin.DataLabels = true;

                workerSeries.Add(maxFactory);
                workerSeries.Add(maxAdmin);
                workerSeries.Add(factory);
                workerSeries.Add(admin);

                return workerSeries;
            }
        } 
        public SeriesCollection StoragePieChart
        {
            get
            {
                storageSeries = new SeriesCollection();
                PieSeries max = new PieSeries();
                PieSeries storage = new PieSeries();

                max.Title = "Pozostała powierzchnia magazynowa";
                max.Values = new ChartValues<int> { PlayerFactory.StorageCap - PlayerFactory.AllStorage };
                max.DataLabels = true;

                storage.Title = "Zmagazynowane";
                storage.Values = new ChartValues<int> {PlayerFactory.AllStorage };
                storage.DataLabels = true;
                

                storageSeries.Add(max);
                storageSeries.Add(storage);

                return storageSeries;
            }
        }
        public SeriesCollection MarketChart
        {
            get
            {
                marketSeries = new SeriesCollection();
                RowSeries sold = new RowSeries();
                RowSeries unsold = new RowSeries();

                sold.Title = "Zarobki w ostatniej turze";
                sold.Values = new ChartValues<float> { PlayerFactory.LastTurnProfits};
                sold.DataLabels = true;

                unsold.Title = "Wartość niesprzedanych produktów";
                unsold.Values = new ChartValues<float> { PlayerFactory.ExpectedRevenue - PlayerFactory.LastTurnProfits };
                unsold.DataLabels = true;
                

                marketSeries.Add(sold);
                marketSeries.Add(unsold);

                return marketSeries;
            }
        }
        public HomeViewModel(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            eventAggregator.SubscribeOnBackgroundThread(this);
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {            
            NotifyOfPropertyChange(() => WorkerPieChart);
            NotifyOfPropertyChange(() => StoragePieChart);
            NotifyOfPropertyChange(() => MarketChart);
            NotifyOfPropertyChange(() => PlayerFactory);

            //eventAggregator.SubscribeOnBackgroundThread(this);
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
    }
}
