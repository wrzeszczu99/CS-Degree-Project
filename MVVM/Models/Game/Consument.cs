using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEOS.MVVM.Models.Game
{
    public class Consument : IBuyer
    {
        //Each field is representet for one person, population is modifier
        readonly string socClassID;
        float turnAccount; 
        float actualAccount;
        float singleSalary;
        int population;
        Dictionary<Product, int> needs;
        List<int> fulfiledNeeds;
        float needsBeingMet;
        [JsonIgnore]
        Market marketConsumentIsOn;

        public float TurnAccount { get => turnAccount; set => turnAccount = (value > 0) ? value : 0;}
        public float ActualAccount { get => actualAccount; set => actualAccount = (value > 0) ? value : 0; }
        public int Population { get => population; set => population = (value > 0) ? value : 0;}
        public Dictionary<Product, int> Needs { get => needs; set => needs = value; }
        public List<int> FulfiledNeeds { get => fulfiledNeeds; set => fulfiledNeeds = value; }
        [JsonIgnore]
        public Market Market { get => marketConsumentIsOn; set => marketConsumentIsOn = value; }
        public string SocClassID => socClassID;
        public float NeedsBeingMet { get => needsBeingMet; set => needsBeingMet = value; }

        public Consument(int salary, Dictionary<Product,int> ClassNeeds, string classID, int popQuan, Market market)
        {
            Population = popQuan;
            socClassID = classID;
            singleSalary = salary;
            TurnAccount = singleSalary*population;
            ActualAccount = TurnAccount;
            marketConsumentIsOn = market;


            Needs = new Dictionary<Product, int>();
            foreach (var item in ClassNeeds)
            {
                if (item.Value > 0) Needs.Add(item.Key, item.Value * Population);
            }

            FulfiledNeeds = new List<int>();


            for (int i = 0; i < Needs.Count; i++)
            {
                FulfiledNeeds.Add(0);
            }


        }

        public void Buy(ref List<Offer> offers)
        {
            //Sort by price
            offers.OrderBy(offer => offer.UnitPrice);

            for (int i = 0; i < Needs.Count; i++)
            {
                int needed = Needs.ElementAt(i).Value;
                foreach (var offer in offers)
                {
                    if (offer.Product.Equals( Needs.ElementAt(i).Key) && offer.Quantity > 0)
                    {
                        FulfiledNeeds[i] += offer.TryGetProduct(ref needed, out float cost);
                        ActualAccount -= cost;
                    }
                    if (FulfiledNeeds[i] == Needs.ElementAt(i).Value) break;
                }
            }
        }
        public void Turn()
        {
            //Needs fulfiment computation
            int needsScore = 0;
            int fulfiledNeedsScore = 0;
            for (int i = 0; i < Needs.Count; i++)
            {
                needsScore += Needs.ElementAt(i).Value;
                fulfiledNeedsScore += fulfiledNeeds[i];
            }
            NeedsBeingMet = needsScore > 0 ? (fulfiledNeedsScore / needsScore) : 0 ;

            //Population growth (2%)
            if (NeedsBeingMet >= .75f) population += (int)Math.Max(population * 0.02f, 1);

            //Prepare for the next turn
            TurnAccount = singleSalary * population;
            ActualAccount = turnAccount;
            for (int i = 0; i < Needs.Count; i++)
            {
                FulfiledNeeds.Add(0);
            }
        }


    }
}
