using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEOS.MVVM.Models.Setup
{
    public class Product : PropertyChangedBase
    {
        string name = " ";
        float workNeeded = 0;
        float storageCost = 0;

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
        public float WorkNeeded
        {
            get => workNeeded;
            set
            {
                workNeeded = value;
                NotifyOfPropertyChange(() => WorkNeeded);
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

        public Product()
        {

        }
        public Product(string s, float f)
        {
            Name = s;
            WorkNeeded = f;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
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
}
