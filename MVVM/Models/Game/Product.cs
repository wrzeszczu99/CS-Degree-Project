using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEOS.MVVM.Models.Game
{
    public class Product : PropertyChangedBase
    {
        string name;
        float workhours;
        float storageCost;

        #region Properties
        public string Name 
        { 
            get => name;
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
        public float WorkHours
        {
            get => workhours;
            set
            {
                workhours = value;
                NotifyOfPropertyChange(() => WorkHours);
            }
        }

        public float StorageCost
        {
            get => storageCost;
            set
            {
                storageCost = value;
                NotifyOfPropertyChange(() => StorageCost);
            }
        }
        #endregion

        public Product(string productName, float workhours, float storageCost)
        {
            Name = productName;
            WorkHours = workhours;
            StorageCost = storageCost;
        }

        public Product(string productName)
        {
            Name = productName;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || (!obj.GetType().Equals(typeof(Product)) && !obj.GetType().Equals(typeof(Material)) && !obj.GetType().Equals(typeof(ProductPrice))))
            {
                return false;
            }
            if (Name.Equals((obj as Product).Name)) return true;
            return base.Equals(obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class Material : Product
    {
        float price;
        float neededToProduce;
        float workpowerShareAct;
        float workpowerShareTrg;
        public Material(string productName, float price, float workShrActual = 0, float workShrTarget = 0) : base(productName)
        {
            Name = productName;
            Price = price;
            WorkpowerShareAct = workShrActual;
            WorkpowerShareTrg = workShrTarget;
        }
        public Material(string productName,  float workhours, float storageCost,float price, float neededToProduce, float workShrActual = 0, float workShrTarget = 0)
            : base(productName, workhours, storageCost)
        {
            Name = productName;
            WorkHours = workhours;
            StorageCost = storageCost;
            Price = price;
            NeededToProduce = neededToProduce;
            WorkpowerShareAct = workShrActual;
            WorkpowerShareTrg = workShrTarget;
        }

        public float Price
        {
            get => price;
            set
            {
                price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }
        public float WorkpowerShareAct { get => workpowerShareAct; set => workpowerShareAct = value; }
        public float WorkpowerShareTrg
        {
            get => workpowerShareTrg;
            set
            {
                workpowerShareTrg = value;
                NotifyOfPropertyChange(() => WorkpowerShareTrg);
            }
        }

        public float NeededToProduce { get => neededToProduce; set => neededToProduce = value; }
    }
}
