using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace CEOS.MVVM.Models.Game
{
    class GameManager
    {
        private static GameManager instance;

        public static GameManager GetInstance()
        {
            if (instance == null)
                instance = new GameManager();

            return instance;
        }

        string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CEOSSaveFile.txt");

        float[,] marketTransportCost;

        List<Product> allProducts = new List<Product>();

        List<Market> allMarkets = new List<Market>();

        List<ProductPriceHistory> allProductPriceHistory = new List<ProductPriceHistory>();

        List<Offer> tradeSpace = new List<Offer>();
        
        public List<IBuyer> buyers = new List<IBuyer>();

        #region Properties
        public float[,] MarketTransportCost { get => marketTransportCost; set => marketTransportCost = value; }
        public List<Product> AllProducts { get => allProducts; set => allProducts = value; }
        internal List<Market> AllMarkets { get => allMarkets; set => allMarkets = value; }
        public List<ProductPriceHistory> AllProductPriceHistory { get => allProductPriceHistory; set => allProductPriceHistory = value; }
        public List<Offer> TradeSpace { get => tradeSpace; set => tradeSpace = value; }


        #endregion
        GameManager()
        {
       
        }

        public GameManager(List<Setup.Product> products, List<Setup.Market> markets)
        {
            for (int i = 0; i < products.Count; i++)
            { 
                AllProducts.Add(new Product(products[i].Name, products[i].WorkNeeded, products[i].StorageCost));
                AllProductPriceHistory.Add(new ProductPriceHistory(new ProductPrice(products[i].Name, 0, 0)));
            }

            foreach (var item in markets)
            {
                AllMarkets.Add(new Market(item));
            }
        }

        public void SetUpTranspotationMatrix(int dimention)
        {
            if (marketTransportCost == null)
            {
                MarketTransportCost = new float[dimention,dimention];
            }
            
        }
        public void AddProductList(List<Product> products)
        {
            foreach (var item in products)
            {
                AllProducts.Add(item);
                AllProductPriceHistory.Add(new ProductPriceHistory(new ProductPrice(item.Name, 0, 0)));
            }
        }

        public void AddMarketList(List<Market> markets)
        {

            foreach (var item in markets)
            {
                AllMarkets.Add(item);
                buyers.AddRange(item.FactoriesOnMarket);
                buyers.AddRange(item.ConsumentsOnMarket);
            }
        }
        public void Bought(Product product, float price)
        {
            foreach (var item in AllProductPriceHistory)
            {
                if(product.Equals(item.ProductPrices[0]))
                {
                    item.ProductPrices.Last().AddPrice(price);
                }
            }
        }

        public void Turn()
        {
            //Prepare the markets
            foreach (var item in allMarkets)
            {
                foreach (var factory in item.FactoriesOnMarket)
                {
                    TradeSpace.AddRange(factory.Produce());
                }
            }

            //Add new price history slot
            foreach (var item in AllProductPriceHistory)
            {
                    item.ProductPrices.Add(new ProductPrice(item.ProductPrices[0].Name, 0, 0));
            }

            //Randomize Buyers
            Shuffle(buyers);

            //TRADE
            foreach (var item in buyers)
            {
                AddTransportPrices(item);
                item.Buy( ref tradeSpace);
                SubstractTransportPrices(item);
            }

            //Discard old offers
            foreach (var item in TradeSpace)
            {
                item.Age++;
            }
            TradeSpace.RemoveAll(offer => offer.Age > 5 || offer.Quantity < 1);

            foreach (var item in buyers)
            {
                item.Turn();
            }
        }

        private void AddTransportPrices(IBuyer buyer)
        {
            foreach (var offer in TradeSpace)
            {
                offer.UnitPrice += TransportCost(offer.SourceMarketID, buyer.Market.ID);
            }
        }

        private void SubstractTransportPrices(IBuyer buyer)
        {
            foreach (var offer in TradeSpace)
            {
                offer.UnitPrice -= TransportCost(offer.SourceMarketID, buyer.Market.ID);
            }
        }

        float TransportCost(int offerMarketID, int buyerMarketID)
        {
            return MarketTransportCost[offerMarketID, buyerMarketID];
        }

        public void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


        public void Save()
        {
            Console.WriteLine("Save");
            var saveObj = JsonConvert.SerializeObject(AllMarkets);

            using (StreamWriter sw = new StreamWriter(filepath))
            {
                sw.Write(saveObj);
            }
        }

        public void Load()
        {
            Console.WriteLine("Load");
            string content = null;
            using (StreamReader sr = new StreamReader(filepath))
            {
                content = sr.ReadToEnd();
            }

            instance = Newtonsoft.Json.JsonConvert.DeserializeObject<GameManager>(content);
            foreach (var market in allMarkets)
            {
                market.InjectMarketInfo();
            }
        }
    }
}
