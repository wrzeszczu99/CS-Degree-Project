using System.Collections.Generic;

namespace CEOS.MVVM.Models.Game
{
    public interface IBuyer
    {
        Market Market { get; }
        void Buy(ref List<Offer> offers);
        void Turn();
    }
}