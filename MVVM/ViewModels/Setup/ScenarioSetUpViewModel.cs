using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using Caliburn.Micro;
using CEOS.MVVM.Models.Setup;

namespace CEOS.MVVM.ViewModels.Setup
{
    public class ScenarioSetUpViewModel : Screen
    {
        readonly IEventAggregator eventAggregator;

        BindableCollection<Product> productList = new BindableCollection<Product>();
        BindableCollection<Market> marketList = new BindableCollection<Market>();

        Market selectedMarket;


        public ScenarioSetUpViewModel(IEventAggregator aggregator) 
        {
            eventAggregator = aggregator;
            MarketList.Add(new Market());
            SelectedMarket = MarketList[0];
            SelectedMarket.FactoriesOnMarket.Add(new Factory(productList.ToList()));

        }
        #region Properties
        public BindableCollection<Product> ProductList 
        { 
            get => productList; 
            set 
            {
                productList = value;
                NotifyOfPropertyChange(() => ProductList);
            } 
        }

        public BindableCollection<Market> MarketList 
        { 
            get => marketList;
            set
            {
                marketList = value;
                NotifyOfPropertyChange(() => MarketList);
            }
        }

        public BindableCollection<Product> LowerSelectedNeeds
        {
            get
            {
                BindableCollection<Product> temp = new BindableCollection<Product>();
                foreach (Product product in SelectedMarket.LowerClassNeeds)
                {
                    temp.Add(product);
                }
                return temp;
            }
        }
        public BindableCollection<Product> MiddleSelectedNeeds
        {
            get
            {
                BindableCollection<Product> temp = new BindableCollection<Product>();
                foreach (Product product in SelectedMarket.MiddleClassNeeds)
                {
                    temp.Add(product);
                }
                return temp;
            }
        }
        public BindableCollection<Product> UpperSelectedNeeds
        {
            get
            {
                BindableCollection<Product> temp = new BindableCollection<Product>();
                foreach (Product product in SelectedMarket.UpperClassNeeds)
                {
                    temp.Add(product);
                }
                return temp;
            }
        }
        public BindableCollection<Factory> FactoryList
        {
            get
            {
                BindableCollection<Factory> temp = new BindableCollection<Factory>();
                foreach (Factory factory in SelectedMarket.FactoriesOnMarket)
                {
                    temp.Add(factory);
                }
                return temp;
            }
            set
            {
                selectedMarket.FactoriesOnMarket = value.ToList();
                NotifyOfPropertyChange(() => FactoryList);
            }
        }
        public BindableCollection<TransportInfo> SelectedTransportCost
        {
            get
            {
                BindableCollection<TransportInfo> temp = new BindableCollection<TransportInfo>();
                foreach(TransportInfo info in SelectedMarket.TransportCost)
                {
                    temp.Add(info);
                }
                return temp;
            }
            set
            {
                selectedMarket.TransportCost = value.ToList();
                NotifyOfPropertyChange(() => SelectedTransportCost);
            }
        }

        public Market SelectedMarket
        {
            get => selectedMarket;
            set
            {
                selectedMarket = value;
                NotifyOfPropertyChange(() => SelectedMarket);
                NotifyOfPropertyChange(() => LowerSelectedNeeds);
                NotifyOfPropertyChange(() => MiddleSelectedNeeds);
                NotifyOfPropertyChange(() => UpperSelectedNeeds);
                NotifyOfPropertyChange(() => FactoryList);
                NotifyOfPropertyChange(() => SelectedTransportCost);
            }
        }

        private Product selectedRaw;

        public Product SelectedRaw
        {
            get { return selectedRaw; }
            set 
            { 
                selectedRaw = value;
                NotifyOfPropertyChange(() => SelectedRaw);
            }
        }

        private Product selectedProduced;

        public Product SelectedProduced
        {
            get { return selectedProduced; }
            set 
            { 
                selectedProduced = value;
                NotifyOfPropertyChange(() => SelectedProduced);
            }
        }


        #endregion

        public void NewProduct()
        {
            productList.Add(new Product());
            foreach (Market market in MarketList)
            {
                market.LowerClassNeeds.Add(new Product());
                market.MiddleClassNeeds.Add(new Product());
                market.UpperClassNeeds.Add(new Product());
                foreach (Factory factory in market.FactoriesOnMarket)
                {
                    factory.RawMaterialsNeeds.Add(new Product());
                    factory.Products.Add(new Product());
                }
            }
            NotifyOfPropertyChange(() => LowerSelectedNeeds);
            NotifyOfPropertyChange(() => MiddleSelectedNeeds);
            NotifyOfPropertyChange(() => UpperSelectedNeeds);
            NotifyOfPropertyChange(() => FactoryList);
        }

        public void DeleteProduct()
        {
            foreach (Market market in MarketList)
            {
                market.LowerClassNeeds.RemoveAt(productList.Count - 1);
                market.MiddleClassNeeds.RemoveAt(productList.Count - 1);
                market.UpperClassNeeds.RemoveAt(productList.Count - 1);
                foreach (Factory factory in market.FactoriesOnMarket)
                {
                    factory.RawMaterialsNeeds.RemoveAt(productList.Count - 1);
                    factory.Products.RemoveAt(productList.Count - 1);
                }
            }
            NotifyOfPropertyChange(() => LowerSelectedNeeds);
            NotifyOfPropertyChange(() => MiddleSelectedNeeds);
            NotifyOfPropertyChange(() => UpperSelectedNeeds);
            NotifyOfPropertyChange(() => FactoryList);
            productList.RemoveAt(productList.Count-1);

        }

        public void NewMarket()
        {
            Market temp = new Market();
            if(MarketList.Count>0) temp.Copy(MarketList.First());
            temp.TransportCost.Add(new TransportInfo(MarketList.First().ID, 0));
            foreach (Market market in MarketList)
            {
                market.TransportCost.Add(new TransportInfo(temp.ID, 0f));
            }
            MarketList.Add(temp);
            NotifyOfPropertyChange(() => SelectedMarket);
            NotifyOfPropertyChange(() => SelectedTransportCost);
        }

        public void DeleteMarket()
        {
            if (MarketList.Count > 1)
            {
                if (SelectedMarket.ID == MarketList.Last().ID) SelectedMarket = MarketList[MarketList.Count - 2];
                MarketList.RemoveAt(MarketList.Count - 1);
                Market.MarketDeleted();
                foreach (Market market in MarketList)
                {
                    market.TransportCost.RemoveAt(market.TransportCost.Count - 1);                    
                }
            }
            NotifyOfPropertyChange(() => SelectedMarket);
            NotifyOfPropertyChange(() => SelectedTransportCost);
        }

        public void NewFactory()
        {
            SelectedMarket.FactoriesOnMarket.Add(new Factory(productList.ToList()));

            NotifyOfPropertyChange(() => FactoryList);
        }

        public void DeleteFactory()
        {
            
            if (SelectedMarket.FactoriesOnMarket.Count > 1)
                SelectedMarket.FactoriesOnMarket.RemoveAt(SelectedMarket.FactoriesOnMarket.Count-1);

            NotifyOfPropertyChange(() => FactoryList);
        }



        public void ProductEdited(object sender, DataGridCellEditEndingEventArgs args)
        {
            Product temp = (Product)args.Row.Item;
            foreach (Market market in MarketList)
            {
                market.LowerClassNeeds.ElementAt(args.Row.GetIndex()).Name = temp.Name;
                market.MiddleClassNeeds.ElementAt(args.Row.GetIndex()).Name = temp.Name;
                market.UpperClassNeeds.ElementAt(args.Row.GetIndex()).Name = temp.Name;
                foreach (Factory factory in market.FactoriesOnMarket)
                {
                    factory.RawMaterialsNeeds.ElementAt(args.Row.GetIndex()).Name = temp.Name;
                    factory.Products.ElementAt(args.Row.GetIndex()).Name = temp.Name;
                }
            }
            NotifyOfPropertyChange(() => LowerSelectedNeeds);
            NotifyOfPropertyChange(() => MiddleSelectedNeeds);
            NotifyOfPropertyChange(() => UpperSelectedNeeds);
        }



        public void StartGame()
        {
            eventAggregator.PublishOnBackgroundThreadAsync(new GameInitializedMessage(ProductList.ToList(), MarketList.ToList()));
        }

        public void LoadGame()
        {
            Models.Game.GameManager.GetInstance().Load();
        }

    }
}
