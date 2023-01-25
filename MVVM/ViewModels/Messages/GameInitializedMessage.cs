using System.Collections.Generic;
using CEOS.MVVM;

namespace CEOS.MVVM.ViewModels
{
    public class GameInitializedMessage
    {
        List<Models.Game.Product> Products;
        List<Models.Game.Market> Markets;
        public GameInitializedMessage(List<Models.Setup.Product> products, List<Models.Setup.Market> markets)
        {
            Products = new List<Models.Game.Product>();
            Markets = new List<Models.Game.Market>();

            Models.Game.GameManager.GetInstance().SetUpTranspotationMatrix(markets[0].TransportCost.Count + 1);

            foreach (var item in products)
            {
                Products.Add(new Models.Game.Product(item.Name, item.WorkNeeded, item.StorageCost));
            }

            Models.Game.GameManager.GetInstance().AddProductList(Products);

            foreach (var item in markets)
            {
                Markets.Add(new Models.Game.Market(item));
            }

            Models.Game.GameManager.GetInstance().AddMarketList(Markets);

            Models.Game.GameManager.GetInstance().AllMarkets[0].FactoriesOnMarket[0].PlayerControlled = true;

        }
    }
}