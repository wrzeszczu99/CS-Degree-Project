using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CEOS.MVVM.Models;
using Caliburn.Micro;
using CEOS.MVVM.Models.Game;
using System.Threading;

namespace CEOS.MVVM.ViewModels.Game
{
    public class WorkersViewModel : Screen, IHandle<GameInitializedMessage>
    {
        readonly IEventAggregator eventAggregator;

        Factory playerFactory;

        #region Properties
        public Factory PlayerFactory { get => playerFactory; set => playerFactory = value; }

        public string FactoryWorkers { get => "Ilość pracowników wytwórczych: " + PlayerFactory.FactoryWorkerActual.ToString() + " (Cel: " + PlayerFactory.FactoryWorkerTarget.ToString() + ")"; }
        public string AdminWorkers { get => "Ilość pracowników administracyjnych: " + PlayerFactory.AdminWorkerActual.ToString() + " (Cel: " + PlayerFactory.AdminWorkerTarget.ToString() + ")"; }

        public string MaxFactoryWorkers { get => "Aktualna maksymalna liczba pracowników wytwórczych:" + PlayerFactory.MaxFactoryWorkerAmountActual.ToString() + " (Cel: " + PlayerFactory.MaxFactoryWorkerAmountTarget.ToString() + ")"; }
        public string MaxAdminWorkers { get => "Aktualna maksymalna liczba pracowników wytwórczych:" + PlayerFactory.MaxAdminWorkerAmountActual.ToString() + " (Cel: " + PlayerFactory.MaxAdminWorkerAmountTarget.ToString() + ")"; }

        #endregion
        #region Buttons methods
        public void ChangeWorkersTarget(string factoryWorkersTarget, string adminWorkersTarget)
        {
            int worker = factoryWorkersTarget !=  "" ? Convert.ToInt32(factoryWorkersTarget) : 0;
            int admin = adminWorkersTarget != "" ? Convert.ToInt32(adminWorkersTarget) : 0;
            PlayerFactory.FactoryWorkerTarget =  worker;
            PlayerFactory.AdminWorkerTarget = admin;
            NotifyOfPropertyChange(() => FactoryWorkers);
            NotifyOfPropertyChange(() => AdminWorkers);
        }    
        public void ChangeMaxWorkersTarget(string maxFactoryWorkersTarget, string maxAdminWorkersTarget)
        {
            int worker = maxFactoryWorkersTarget != "" ? Convert.ToInt32(maxFactoryWorkersTarget) : 0;
            int admin = maxAdminWorkersTarget != "" ? Convert.ToInt32(maxAdminWorkersTarget) : 0;
            PlayerFactory.MaxFactoryWorkerAmountTarget = worker;
            PlayerFactory.MaxAdminWorkerAmountTarget = admin;
            NotifyOfPropertyChange(() => MaxFactoryWorkers);
            NotifyOfPropertyChange(() => MaxAdminWorkers);
        }

        #endregion



        #region Constructor, Events, Activators
        public WorkersViewModel(IEventAggregator aggregator)
        {
            eventAggregator = aggregator;
            eventAggregator.SubscribeOnBackgroundThread(this);
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            NotifyOfPropertyChange(() => PlayerFactory);
            NotifyOfPropertyChange(() => FactoryWorkers);
            NotifyOfPropertyChange(() => AdminWorkers);
            NotifyOfPropertyChange(() => MaxFactoryWorkers);
            NotifyOfPropertyChange(() => MaxAdminWorkers);

            //eventAggregator.SubscribeOnBackgroundThread(this);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public Task HandleAsync(GameInitializedMessage message, CancellationToken cancellationToken)
        {
            PlayerFactory = GameManager.GetInstance().AllMarkets[0].FactoriesOnMarket[0];
            return Task.CompletedTask;
        }

        #endregion
    }
}
