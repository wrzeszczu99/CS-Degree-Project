using System.Collections.Generic;

namespace CEOS.MVVM.Models.Game
{
    public class ProductPriceHistory
    {
        List<ProductPrice> productPrices;

        public List<ProductPrice> ProductPrices { get => productPrices; }


        public ProductPriceHistory(ProductPrice firstRecord)
        {
            productPrices = new List<ProductPrice>();
            AddRecord(firstRecord);
        }

        public void AddRecord(ProductPrice productPrice)
        {
            productPrices.Add(productPrice);
        }
    }
}
