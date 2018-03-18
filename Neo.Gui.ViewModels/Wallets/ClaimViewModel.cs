﻿using System;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Neo.Gui.Dialogs.Interfaces;
using Neo.Gui.Dialogs.LoadParameters.Wallets;
using Neo.UI.Core.Messaging.Interfaces;
using Neo.UI.Core.Wallet;
using Neo.UI.Core.Wallet.Messages;

namespace Neo.Gui.ViewModels.Wallets
{
    public class ClaimViewModel :
        ViewModelBase,
        ILoadable,
        IUnloadable,
        IMessageHandler<WalletStatusMessage>,
        IDialogViewModel<ClaimLoadParameters>
    {
        #region Private Fields 
        private readonly IMessageSubscriber messageSubscriber;
        private readonly IWalletController walletController;

        private decimal availableGas = decimal.Zero;
        private decimal unavailableGas = decimal.Zero;

        private bool claimEnabled;
        #endregion

        #region Public Properties
        public decimal AvailableGas
        {
            get => this.availableGas;
            set
            {
                if (this.availableGas == value) return;

                this.availableGas = value;

                RaisePropertyChanged();
            }
        }

        public decimal UnavailableGas
        {
            get => this.unavailableGas;
            set
            {
                if (this.unavailableGas == value) return;

                this.unavailableGas = value;

                RaisePropertyChanged();
            }
        }

        public bool ClaimEnabled
        {
            get => this.claimEnabled;
            set
            {
                if (this.claimEnabled == value) return;

                this.claimEnabled = value;

                RaisePropertyChanged();
            }
        }

        public RelayCommand ClaimCommand => new RelayCommand(this.Claim);
        #endregion Public Properties

        #region Constructor 
        public ClaimViewModel(
            IMessageSubscriber messageSubscriber,
            IWalletController walletController)
        {
            this.messageSubscriber = messageSubscriber;
            this.walletController = walletController;
        }
        #endregion

        #region IDialogViewModel Implementation 
        public event EventHandler Close;

        public void OnDialogLoad(ClaimLoadParameters parameters)
        {
        }
        #endregion

        #region ILoadable implementation
        public void OnLoad()
        {
            this.messageSubscriber.Subscribe(this);

            this.CalculateBonusAvailable();
        }
        #endregion

        #region IUnloadable implementation
        public void OnUnload()
        {
            this.messageSubscriber.Unsubscribe(this);
        }
        #endregion

        #region IMessageHandler implementation
        public void HandleMessage(WalletStatusMessage message)
        {
            this.CalculateBonusUnavailable(message.BlockchainStatus.Height + 1);
        }

        #endregion

        #region Private Methods 
        private void CalculateBonusAvailable()
        {
            var bonusAvailable = this.walletController.CalculateBonus();
            this.AvailableGas = bonusAvailable;

            if (bonusAvailable == decimal.Zero)
            {
                this.ClaimEnabled = false;
            }
            else
            {
                this.ClaimEnabled = true;
            }
        }

        private void CalculateBonusUnavailable(uint height)
        {
           this.UnavailableGas = this.walletController.CalculateUnavailableBonusGas(height);
        }        

        private async void Claim()
        {
            await this.walletController.ClaimUtilityTokenAsset();

            this.Close(this, EventArgs.Empty);
        }
        #endregion
    }
}