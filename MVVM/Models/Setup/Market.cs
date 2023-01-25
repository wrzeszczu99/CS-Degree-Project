using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEOS.MVVM.Models.Setup
{
    public class Market
    {
        static int IDCounter = 0;
        int id;
        List<Factory> factoriesOnMarket;
        int fixedFactoryOperatingCost;
        float[] socClassesDistribution = new float[3];
        int[] socClassesSalary = new int[3];
        int populationSize;
        List<Product> lowerClassNeeds;
        List<Product> middleClassNeeds;
        List<Product> upperClassNeeds;
        List<TransportInfo> transportCost;

        #region Properties
        public List<Factory> FactoriesOnMarket { get => factoriesOnMarket; set => factoriesOnMarket = value; }
        public int FixedFactoryOperatingCost { get => fixedFactoryOperatingCost; set => fixedFactoryOperatingCost = (value > 0) ? value : 0; }
        public float[] SocClassesDistribution { get => socClassesDistribution; set => socClassesDistribution = value; }
        public int PopulationSize { get => populationSize; set => populationSize = value; }
        public List<Product> LowerClassNeeds { get => lowerClassNeeds; set => lowerClassNeeds = value; }
        public List<Product> MiddleClassNeeds { get => middleClassNeeds; set => middleClassNeeds = value; }
        public List<Product> UpperClassNeeds { get => upperClassNeeds; set => upperClassNeeds = value; }
        public int[] SocClassesSalary { get => socClassesSalary; set => socClassesSalary = value; }
        public List<TransportInfo> TransportCost
        {
            get => transportCost;
            set
            {
                transportCost = value;
            }
        }
        public int ID { get => id; set => id = value; }

        #endregion

        public Market()
        {
            ID = IDCounter;
            IDCounter++;

            LowerClassNeeds = new List<Product>();
            MiddleClassNeeds = new List<Product>();
            UpperClassNeeds = new List<Product>();

            SocClassesDistribution[0] = 10;
            SocClassesDistribution[1] = 70;
            SocClassesDistribution[2] = 20;
            
            SocClassesSalary[0] = 1000;
            SocClassesSalary[1] = 5000;
            SocClassesSalary[2] = 10000;

            PopulationSize = 10000;

            FixedFactoryOperatingCost = 10000;

            factoriesOnMarket = new List<Factory>();
            transportCost = new List<TransportInfo>();
        }

        public void Copy(Market market)
        {
            LowerClassNeeds = market.LowerClassNeeds.ConvertAll(product => new Product(product.Name, product.WorkNeeded));
            MiddleClassNeeds = market.MiddleClassNeeds.ConvertAll(product => new Product(product.Name, product.WorkNeeded));
            UpperClassNeeds = market.UpperClassNeeds.ConvertAll(product => new Product(product.Name, product.WorkNeeded));

            SocClassesDistribution[0] = market.SocClassesDistribution[0];
            SocClassesDistribution[1] = market.SocClassesDistribution[1];
            SocClassesDistribution[2] = market.SocClassesDistribution[2];

            SocClassesSalary[0] = market.SocClassesSalary[0];
            SocClassesSalary[1] = market.SocClassesSalary[1];
            SocClassesSalary[2] = market.SocClassesSalary[2];
            PopulationSize = market.PopulationSize;
            FixedFactoryOperatingCost = market.FixedFactoryOperatingCost;

            TransportCost = market.transportCost.ConvertAll(cost => new TransportInfo(cost.ID, cost.Cost));
        }


        public static void MarketDeleted()
        {
            --IDCounter;
        }

    }

    public class TransportInfo : PropertyChangedBase
    {
        int id;
        float cost;

        public int ID
        {
            get => id;
            set
            {
                id = value;
                NotifyOfPropertyChange(() => ID);
            }
        }
        public float Cost
        {
            get => cost;
            set
            {
                cost = value;
                NotifyOfPropertyChange(() => Cost);
            }
        }
        public TransportInfo(int id, float cost)
        {
            this.id = id;
            this.cost = cost;
        }
    }
}
