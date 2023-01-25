using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEOS.MVVM.Models.Game
{
    public class Offer
    {
        Product product;
        int quantity;
        float unitPrice;
        Factory producer;
        int sourceMarketID;
        int age;

        public Product Product { get => product; set => product = value; }
        public int Quantity { get => quantity; set => quantity = (value > 0) ? value : 0; }
        public float UnitPrice { get => unitPrice; set => unitPrice = (value > 0) ? value : 0; }
        public Factory Producer { get => producer; set => producer = value; }
        public int SourceMarketID { get => sourceMarketID; set => sourceMarketID = value; }
        public int Age { get => age; set => age = value; }

        public Offer(Product prod, int quan, float price, Factory factory, int marketID)
        {
            
            Product = prod;
            Quantity = quan;
            UnitPrice = price;
            Producer = factory;
            SourceMarketID = marketID;
            age = 0;
        }

        public int TryGetProduct(ref int needed, out float price)
        {
            if(needed< quantity)
            {
                int temp = needed;
                Quantity -= needed;
                needed = 0;
                price = needed * UnitPrice;
                GameManager.GetInstance().Bought(Product, UnitPrice);
                Producer.Account += price;
                return temp;
            }
            else
            {
                int temp = Quantity;
                Quantity = 0;
                needed -= temp;
                price = temp * UnitPrice;
                GameManager.GetInstance().Bought(Product, UnitPrice);
                Producer.Account += price;
                return temp;
            }
        }
    }
}
