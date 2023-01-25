using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEOS.MVVM.Models.Game
{
    public class Market
    {
        static int IDCount = 0;
        readonly public int ID;




        List<Factory> factoriesOnMarket;
        List<Consument> consumentsOnMarket;
        int fixedFactoryOperatingCost;


        float[] socClassesDistribution = new float[3];
        int[] socClassesSalary = new int[3];
        Dictionary<Product, int> lowerClassNeeds;
        Dictionary<Product, int> middleClassNeeds;
        Dictionary<Product, int> upperClassNeeds;



        #region Properties
        public List<Factory> FactoriesOnMarket { get => factoriesOnMarket; set => factoriesOnMarket = value; }
        public int FixedFactoryOperatingCost { get => fixedFactoryOperatingCost; set => fixedFactoryOperatingCost = (value > 0) ? value : 0; }
        public List<Consument> ConsumentsOnMarket { get => consumentsOnMarket; set => consumentsOnMarket = value; }
        public float[] SocClassesDistribution { get => socClassesDistribution; set => socClassesDistribution = value; }
        public Dictionary<Product, int> LowerClassNeeds { get => lowerClassNeeds; set => lowerClassNeeds = value; }
        public Dictionary<Product, int> MiddleClassNeeds { get => middleClassNeeds; set => middleClassNeeds = value; }
        public Dictionary<Product, int> UpperClassNeeds { get => upperClassNeeds; set => upperClassNeeds = value; }
        public int[] SocClassesSalary { get => socClassesSalary; set => socClassesSalary = value; }

        #endregion
        public Market(Setup.Market market)
        {
            ID = IDCount;
            IDCount++;
            ConsumentsOnMarket = new List<Consument>();
            FixedFactoryOperatingCost = market.FixedFactoryOperatingCost;


            FactoriesOnMarket = new List<Factory>();
            foreach (var item in market.FactoriesOnMarket)
            {
                FactoriesOnMarket.Add(new Factory(item, this));
            }

            for (int i = 0; i < socClassesDistribution.Length; i++)
            {
                socClassesDistribution[i] = market.SocClassesDistribution[i];
                socClassesSalary[i] = market.SocClassesSalary[i];
            }

            // SocClassesNeeds copies here
            lowerClassNeeds = new Dictionary<Product, int>();
            middleClassNeeds = new Dictionary<Product, int>();
            upperClassNeeds = new Dictionary<Product, int>();
            foreach (var item in GameManager.GetInstance().AllProducts)
            {
                foreach (var prod in market.LowerClassNeeds)
                {
                    if (prod.Name == item.Name) lowerClassNeeds.Add(item, (int)prod.WorkNeeded);
                }
                foreach (var prod in market.MiddleClassNeeds)
                {
                    if (prod.Name == item.Name) middleClassNeeds.Add(item, (int)prod.WorkNeeded);
                }
                foreach (var prod in market.UpperClassNeeds)
                {
                    if (prod.Name == item.Name) upperClassNeeds.Add(item, (int)prod.WorkNeeded);
                }
            }
            

            int lowerClass = (int) (market.PopulationSize * SocClassesDistribution[0]);
            int middleClass = (int)(market.PopulationSize * SocClassesDistribution[1]);
            int upperClass = (int) (market.PopulationSize * SocClassesDistribution[2]);

            ConsumentsOnMarket.Add(new Consument(SocClassesSalary[0], LowerClassNeeds, "Lower Class", lowerClass, this));
            ConsumentsOnMarket.Add(new Consument(SocClassesSalary[1], MiddleClassNeeds, "Middle Class", middleClass, this));
            ConsumentsOnMarket.Add(new Consument(SocClassesSalary[2], UpperClassNeeds, "Upper Class", upperClass, this));


            for (int i = 0; i < market.TransportCost.Count; i++)
            {
                GameManager.GetInstance().MarketTransportCost[ID, market.TransportCost[i].ID] = market.TransportCost[i].Cost;
            }
            
        }

        public void InjectMarketInfo()
        {
            foreach (var consumer in consumentsOnMarket)
            {
                consumer.Market = this;
            }
            foreach (var factory in FactoriesOnMarket)
            {
                factory.Market = this;
            }
        }

    }
}
