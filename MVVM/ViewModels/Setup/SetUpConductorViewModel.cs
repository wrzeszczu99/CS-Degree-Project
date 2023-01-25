using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;

namespace CEOS.MVVM.ViewModels.Setup
{
    public class SetUpConductorViewModel : Conductor<object>.Collection.OneActive
    {
        //readonly IEventAggregator eventAggregator;
        readonly ScenarioSetUpViewModel scenarioSetUpViewModel;
        public SetUpConductorViewModel(/*IEventAggregator aggregator,*/ ScenarioSetUpViewModel scenarioSetUp)
        {
            //eventAggregator = aggregator;
            scenarioSetUpViewModel = scenarioSetUp;


            Items.Add(scenarioSetUp);
        }


        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //eventAggregator.SubscribeOnPublishedThread(this);
            ActivateItemAsync(scenarioSetUpViewModel);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            //eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
    }
}