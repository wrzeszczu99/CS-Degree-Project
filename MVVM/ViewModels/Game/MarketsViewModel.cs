using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEOS.MVVM.Models;
using Caliburn.Micro;
using CEOS.MVVM.Models.Game;
using System.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;

namespace CEOS.MVVM.ViewModels.Game
{
    public class MarketsViewModel: Screen, IHandle<GameInitializedMessage>
    {
        readonly IEventAggregator eventAggregator;

        ProductPrice selectedProduct;

        SeriesCollection priceSeries;

        #region Properties
        public ProductPrice SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                NotifyOfPropertyChange(() => PriceSeries);
            }
        }

        public BindableCollection<ProductPrice> SoldProducts
        {
            get
            {
                BindableCollection<ProductPrice> temp = new BindableCollection<ProductPrice>();
                foreach (var item in GameManager.GetInstance().AllProductPriceHistory)
                {
                    temp.Add(new ProductPrice(item.ProductPrices.Last().Name, item.ProductPrices.Last().LowestPrice, item.ProductPrices.Last().HighestPrice));
                }
                return temp;
            }
        }

        // Graph property to do
        public SeriesCollection PriceSeries 
        {
            get
            {
                priceSeries = new SeriesCollection();
                LineSeries low = new LineSeries();
                LineSeries high = new LineSeries();


                low.Title = "Najniższa cena";
                low.Values = new ChartValues<float>();
                low.Stroke = Brushes.Green;

                high.Title = "Najwyższa cena";
                high.Values = new ChartValues<float>();
                high.Stroke = Brushes.Red;

                foreach (var item in GameManager.GetInstance().AllProductPriceHistory)
                {
                    if (item.ProductPrices[0].Equals(selectedProduct))
                    {
                        for (int i = 0; i < item.ProductPrices.Count; i++)
                        {
                            low.Values.Add(item.ProductPrices[i].LowestPrice);
                            high.Values.Add(item.ProductPrices[i].HighestPrice);
                        }
                    }
                }
                priceSeries.Add(low);
                priceSeries.Add(high);

                return priceSeries;
            }
        }

        


        #endregion




        #region Constructor, Events, Activators
        public MarketsViewModel(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            eventAggregator.SubscribeOnBackgroundThread(this);
            
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            NotifyOfPropertyChange(() => SelectedProduct);
            NotifyOfPropertyChange(() => SoldProducts);
            NotifyOfPropertyChange(() => PriceSeries);

            eventAggregator.SubscribeOnBackgroundThread(this);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public Task HandleAsync(GameInitializedMessage message, CancellationToken cancellationToken)
        {
            SelectedProduct = SoldProducts[0];
            return Task.CompletedTask;
        }

        #endregion
    }
}
