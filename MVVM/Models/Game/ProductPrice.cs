namespace CEOS.MVVM.Models.Game
{
    public class ProductPrice : Product
    {
        float lowestPrice =0;
        float highestPrice =0;

        public float LowestPrice { get => lowestPrice; set => lowestPrice = value; }
        public float HighestPrice { get => highestPrice; set => highestPrice = value; }

        public ProductPrice(string productName, float lowest, float highest) : base(productName)
        {
            LowestPrice = lowest;
            HighestPrice = highest;
        }

        public void AddPrice(float price)
        {
            if (price > HighestPrice) HighestPrice = price;
            if (LowestPrice > 0 && LowestPrice>price) LowestPrice = price;
            if (lowestPrice == 0) LowestPrice = price;
        }
    }
}
