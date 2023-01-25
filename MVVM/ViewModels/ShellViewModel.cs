using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CEOS.MVVM.Models;
using CEOS.MVVM.ViewModels.Game;
using CEOS.MVVM.ViewModels.Setup;

namespace CEOS.MVVM.ViewModels
{
    public class ShellViewModel : Conductor<object>.Collection.OneActive, IHandle<GameInitializedMessage>
    {
        readonly IEventAggregator eventAggregator;

        readonly SetUpConductorViewModel setUpConductorViewModel;
        readonly GameConductorViewModel gameConductorViewModel;



        public ShellViewModel(IEventAggregator aggregator, SetUpConductorViewModel setUpConductor, GameConductorViewModel gameConductor)
        {
            eventAggregator = aggregator;
            eventAggregator.SubscribeOnPublishedThread(this);

            setUpConductorViewModel = setUpConductor;
            gameConductorViewModel = gameConductor;

            Items.AddRange(new Screen[] { setUpConductorViewModel, gameConductorViewModel });

        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            eventAggregator.SubscribeOnPublishedThread(this);
            ActivateItemAsync(setUpConductorViewModel);
            return base.OnActivateAsync(cancellationToken);
            
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public void Exit()
        {
            TryCloseAsync();
        }

        public Task HandleAsync(GameInitializedMessage message, CancellationToken cancellationToken)
        {
            ActivateItemAsync(gameConductorViewModel);
            return Task.CompletedTask;
        }
    }
}
