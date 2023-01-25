using System;
using System.Collections.Generic;
namespace CEOS.MVVM.Models.Setup
{
    public class Factory
    {
        string name;

        int maxWorkerAmount;

        int factorySize;

        List<Product> rawMaterialsNeeds;

        List<Product> products;
        #region Properties 

        public string Name { get => name; set => name = value; }
        public int MaxWorkerAmount { get => maxWorkerAmount; set => maxWorkerAmount = (value > 0) ? value : 0; }
        public List<Product> RawMaterialsNeeds { get => rawMaterialsNeeds; set => rawMaterialsNeeds = value; }
        public List<Product> Products { get => products; set => products = value; }

        private Product selectedRaw;

        public Product SelectedRaw
        {
            get { return selectedRaw; }
            set { selectedRaw = value; }
        }

        private Product selectedProduced;

        public Product SelectedProduced
        {
            get { return selectedProduced; }
            set { selectedProduced = value; }
        }

        public int FactorySize { get => factorySize; set => factorySize = value; }


        #endregion

        public Factory(List<Product> products)
        {
            RawMaterialsNeeds = new List<Product>(products);
            Products = new List<Product>(products);
            FactorySize = 10;
        }

    }
}