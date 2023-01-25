namespace CEOS.MVVM.Models.Game
{
    public class Storage
    {
        Product product;
        int actual;
        int target;

        public Product Product { get => product; set => product = value; }
        public int Actual { get => actual; set => actual = value; }
        public int Target { get => target; set => target = value; }

        public float StoringCost
        {
            get => actual * product.StorageCost;
        }


        public Storage(Product product, int actual, int target)
        {
            this.product = product;
            this.actual = actual;
            this.target = target;
        }


    }
}