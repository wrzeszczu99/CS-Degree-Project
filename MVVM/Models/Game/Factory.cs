using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace CEOS.MVVM.Models.Game
{
    public class Factory : IBuyer
    {

        string name;
        bool playerControlled = false;

        int maxFactoryWorkerAmountActual;
        int maxFactoryWorkerAmountTarget;
        int maxAdminWorkerAmountActual;
        int maxAdminWorkerAmountTarget;

        int factoryWorkerActual;
        int factoryWorkerTarget;
        int adminWorkerActual;
        int adminWorkerTarget;

        float workpower;

        float workerEfficiency;
        
        float account;
        float lastTurnAccount;
        float expectedRevenue = 0;

        List<Material> rawMaterials;
        List<Material> producedMaterials;
        List<Storage> storage;

        int storageCap;
        int storageTarget;
        [JsonIgnore]
        Market marketFactoryIsOn;

        #region Properties 

        public string AccountString
        {
            get => "Stan konta: " + Account.ToString();
        }
        public string Name { get => name; set => name = value; }
        public int MaxFactoryWorkerAmountActual { get => maxFactoryWorkerAmountActual; set => maxFactoryWorkerAmountActual = (value > 0) ? value : maxFactoryWorkerAmountActual; }
        public int MaxFactoryWorkerAmountTarget { get => maxFactoryWorkerAmountTarget; set => maxFactoryWorkerAmountTarget = (value > 0) ? value : maxFactoryWorkerAmountTarget; }
        public int MaxAdminWorkerAmountActual { get => maxAdminWorkerAmountActual; set => maxAdminWorkerAmountActual = (value > 0) ? value : maxAdminWorkerAmountActual; }
        public int MaxAdminWorkerAmountTarget { get => maxAdminWorkerAmountTarget; set => maxAdminWorkerAmountTarget = (value > 0) ? value : maxAdminWorkerAmountTarget; }
        public int FactoryWorkerActual { get => factoryWorkerActual; set => factoryWorkerActual = (value > 0) ? value : factoryWorkerActual; }
        public int FactoryWorkerTarget { get => factoryWorkerTarget; set => factoryWorkerTarget = (value > 0 && value <= maxFactoryWorkerAmountActual) ? value : factoryWorkerTarget; }
        public int AdminWorkerActual { get => adminWorkerActual; set => adminWorkerActual = (value > 0) ? value : adminWorkerActual; }
        public int AdminWorkerTarget { get => adminWorkerTarget; set => adminWorkerTarget = (value > 0 && value <= maxAdminWorkerAmountActual) ? value : adminWorkerTarget; }
        public float Workpower { get => workpower; set => workpower = (value > 0) ? value : 0; }
        public float WorkerEfficiency { get => workerEfficiency; set => workerEfficiency = (value > 0) ? value : 0; }
        public float Account { get => account; set => account = (value > 0) ? value : 0; }
        public List<Material> RawMaterials { get => rawMaterials; set => rawMaterials = value; }
        public List<Material> ProducedMaterials { get => producedMaterials; set => producedMaterials = value; }
        public float LastTurnProfits { get => account - lastTurnAccount; }
        public List<Storage> Storage { get => storage; set => storage = value; }
        public int StorageCap { get => storageCap; set => storageCap = value; }
        public int StorageTarget { get => storageTarget; set => storageTarget = value; }
        public int AllStorage
        {
            get
            {
                int temp = 0;
                foreach (var item in Storage)
                {
                    temp += item.Actual;
                }
                return temp;
            }
        }
        [JsonIgnore]
        public Market Market { get => marketFactoryIsOn; set => marketFactoryIsOn = value; }
        public bool PlayerControlled { get => playerControlled; set => playerControlled = value; }
        public float ExpectedRevenue { get => expectedRevenue; set => expectedRevenue = value; }

        public string WorkerAmount { get => (factoryWorkerActual + adminWorkerActual).ToString(); }
        public string MaxWorkerAmount { get => (maxFactoryWorkerAmountActual + maxAdminWorkerAmountActual).ToString(); }



        #endregion


        //From Setup Conversion
        public Factory(Setup.Factory factory, Market market)
        {
            name = factory.Name;
            marketFactoryIsOn = market;
            account = factory.FactorySize * 100;
            MaxFactoryWorkerAmountActual = (int) (factory.FactorySize * 0.8f);
            MaxFactoryWorkerAmountTarget = (int)(factory.FactorySize * 0.8f);
            MaxAdminWorkerAmountActual = (int)(factory.FactorySize * 0.2f);
            MaxAdminWorkerAmountTarget = (int)(factory.FactorySize * 0.2f);
            FactoryWorkerActual = (int)Math.Floor(MaxFactoryWorkerAmountActual * 0.5f);
            FactoryWorkerTarget = FactoryWorkerActual;
            AdminWorkerActual = (int)Math.Floor(MaxAdminWorkerAmountActual * 0.5f);
            AdminWorkerTarget = AdminWorkerActual;
            WorkerEfficiency = 0.5f + Math.Min(Math.Max((AdminWorkerActual/AdminWorkerActual+FactoryWorkerActual) * 2, 0f), 0.4f);
            Workpower = FactoryWorkerActual * WorkerEfficiency;
            StorageCap = factory.FactorySize * 10;
            StorageTarget = factory.FactorySize * 10;

            RawMaterials = new List<Material>();
            ProducedMaterials = new List<Material>();
            Storage = new List<Storage>();

            int count = 0;
            foreach (var item in GameManager.GetInstance().AllProducts)
            {
                foreach (var prod in factory.RawMaterialsNeeds)
                {
                    float temp = prod.WorkNeeded;
                    if (prod.Name == item.Name && !RawMaterials.Contains(item) &&  temp > 0)
                    {
                        RawMaterials.Add(new Material(item.Name, item.WorkHours, item.StorageCost, 1f, prod.WorkNeeded));
                        Storage.Add(new Storage(item, factory.FactorySize * 10 / factory.RawMaterialsNeeds.Count, factory.FactorySize * 10 / factory.RawMaterialsNeeds.Count));
                    }
                }

                foreach (var prod in factory.Products)
                {
                    float temp = prod.WorkNeeded;
                    if (prod.Name == item.Name && !ProducedMaterials.Contains(item) && temp > 0)
                    {
                        ProducedMaterials.Add(new Material(item.Name, item.WorkHours, item.StorageCost, 1, prod.WorkNeeded));
                        count++;
                    }
                }
            }

            for (int i = 0; i < ProducedMaterials.Count; i++)
            {
                ProducedMaterials[i].WorkpowerShareAct = 1 / count;
                ProducedMaterials[i].WorkpowerShareTrg = 1 / count;
            }

        }

        public List<Offer> Produce()
        {
            lastTurnAccount = account;

            //Expand Factory
            Expand(ref storageTarget, ref storageCap, 3);
            Expand(ref maxFactoryWorkerAmountTarget, ref maxFactoryWorkerAmountActual, 3);
            Expand(ref maxAdminWorkerAmountTarget, ref maxAdminWorkerAmountActual, 3);


            //Hire new workers
            Expand(ref factoryWorkerTarget, ref factoryWorkerActual, 5);
            Expand(ref adminWorkerTarget, ref adminWorkerActual, 3);

            //Compute Workpower
            for (int i = 0; i < ProducedMaterials.Count; i++)
            {
                if (ProducedMaterials[i].WorkpowerShareTrg > ProducedMaterials[i].WorkpowerShareAct)
                    ProducedMaterials[i].WorkpowerShareAct += Growth(ProducedMaterials[i].WorkpowerShareTrg, ProducedMaterials[i].WorkpowerShareAct);
            }
            WorkerEfficiency = 0.5f + Math.Min(Math.Max((AdminWorkerActual / AdminWorkerActual + FactoryWorkerActual) * 2, 0f), 0.4f);
            Workpower = FactoryWorkerActual * WorkerEfficiency;


            //Pay workers
            Account -= FactoryWorkerActual * Market.SocClassesSalary[0] + 
                       AdminWorkerActual * Market.SocClassesSalary[1] + Market.FixedFactoryOperatingCost;

            //Produce

            //Check Raw storage and compute total number of conversions
            List<int> numberOfConversions = new List<int>(ProducedMaterials.Count);
            for (int i = 0; i < numberOfConversions.Capacity; i++)
            {
                numberOfConversions.Add(Int32.MaxValue);
            }

            int packNumberInStorage = Int32.MaxValue;
            for (int i = 0; i < Storage.Count; i++)
            {
                for (int j = 0; j < RawMaterials.Count; j++)
                {
                    if (RawMaterials[j].Equals(Storage[i].Product) && Storage[i].Actual>=0)
                    {
                        packNumberInStorage = Math.Min(packNumberInStorage, (int)(Storage[i].Actual / RawMaterials[j].NeededToProduce));
                    }
                }
            }

            for (int i = 0; i < ProducedMaterials.Count; i++)
            {
                numberOfConversions[i] = (int)Math.Min(packNumberInStorage * ProducedMaterials[i].WorkpowerShareAct, Workpower * ProducedMaterials[i].WorkpowerShareAct / ProducedMaterials[i].WorkHours);
            }

            for (int i = 0; i < Storage.Count; i++)
            {
                for (int j = 0; j < RawMaterials.Count; j++)
                {
                    if (RawMaterials[j].Equals(Storage[i].Product))
                    {
                        Storage[i].Actual -= (int)(numberOfConversions.Sum() * RawMaterials[j].NeededToProduce);
                    }
                }
            }
            
            List<Offer> produced = new List<Offer>();
            expectedRevenue = 0;
            for (int i = 0; i < ProducedMaterials.Count; i++)
            {
                produced.Add(new Offer(ProducedMaterials[i], (int)(numberOfConversions[i] * ProducedMaterials[i].NeededToProduce), ProducedMaterials[i].Price, this, Market.ID));
                expectedRevenue += (((int)(numberOfConversions[i] * ProducedMaterials[i].NeededToProduce)) * ProducedMaterials[i].Price);
            }


            return produced;

        }

        private void Expand( ref int target, ref int actual, int modifier)
        {
            if (target > actual && Account > (int)Growth(target, actual) * modifier)
            {
                Account -= (int)Growth(target, actual) * modifier;
                actual += (int)Growth(target, actual);
            }

            if (target < actual)
            {
                Account += (int)Growth(actual, target) * modifier;
                actual -= (int)Growth(actual, target);
            }
        }

        public float Growth(float target, float actual)
        {
            return Math.Max(Math.Max(Math.Min(actual / 20, target - actual), (target - actual) / 2), 1);
        }

        public void Buy(ref List<Offer> offers)
        {
            //Sort by price
            offers.OrderBy(offer => offer.UnitPrice);

            foreach (var item in storage)
            {
                if(item.Actual<item.Target)
                {
                    int needed = Math.Min(item.Target - item.Actual, StorageCap - AllStorage);
                    foreach (var offer in offers)
                    {
                        if (needed <= 0) break;
                        if (offer.Product.Equals(item.Product) && offer.Quantity > 0 && offer.UnitPrice <= RawMaterials.Find(x => x.Equals(item.Product)).Price)
                        {
                            item.Actual += offer.TryGetProduct(ref needed, out float price);
                            Account -= price;
                        }
                        
                    }
                }
            }
        }

        public void Turn()
        {
            if (PlayerControlled) return;

            //AI computations
            float soldPrecentage = LastTurnProfits / expectedRevenue;

            //Prices
            if(soldPrecentage> 0.8f || LastTurnProfits < 0) 
            {
                foreach (var item in producedMaterials)
                {
                    item.Price *= 1.05f;
                }
            }
            if(soldPrecentage < 0.2f && LastTurnProfits>0)
            {
                foreach (var item in producedMaterials)
                {
                    item.Price *= 0.95f;
                }
            }

            //Expand Factory
            if (maxAdminWorkerAmountActual < maxAdminWorkerAmountTarget && LastTurnProfits > 0 && account > 500) maxAdminWorkerAmountTarget ++;
            if (adminWorkerActual < maxAdminWorkerAmountActual) adminWorkerTarget++; 
            if (maxFactoryWorkerAmountActual < maxFactoryWorkerAmountTarget && LastTurnProfits > 0 && account > 500) maxFactoryWorkerAmountTarget ++;
            if (factoryWorkerActual < maxFactoryWorkerAmountActual) factoryWorkerTarget++;
            
            //Sell factory parts
            if (LastTurnProfits < 0 && Account < 100)
            {
                maxAdminWorkerAmountTarget--;
                maxFactoryWorkerAmountTarget--;
                storageTarget--;
                if (maxAdminWorkerAmountTarget < maxAdminWorkerAmountActual) maxAdminWorkerAmountActual--;
                if (maxFactoryWorkerAmountTarget < maxFactoryWorkerAmountActual) maxFactoryWorkerAmountActual--;
                if (maxAdminWorkerAmountActual < AdminWorkerActual) { adminWorkerActual--; adminWorkerTarget--; }
                if (maxFactoryWorkerAmountActual < factoryWorkerActual) { factoryWorkerActual--; factoryWorkerTarget--; }
            }
        }
    }
}