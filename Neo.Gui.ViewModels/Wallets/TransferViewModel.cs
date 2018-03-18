﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.Gui.Dialogs.Interfaces;
using Neo.Gui.Dialogs.LoadParameters.Wallets;
using Neo.UI.Core.Globalization.Resources;
using Neo.UI.Core.Data;
using Neo.UI.Core.Helpers.Extensions;
using Neo.UI.Core.Transactions.Parameters;
using Neo.UI.Core.Wallet;

namespace Neo.Gui.ViewModels.Wallets
{
    public class TransferViewModel : ViewModelBase, IDialogViewModel<TransferLoadParameters>
    {
        #region Private Fields 
        private readonly IDialogManager dialogManager;
        private readonly IWalletController walletController;

        private bool showAdvancedSection;

        private string fee = "0";

        private string selectedChangeAddress;

        private string remark = string.Empty;
        #endregion

        #region Public Properties 
        public ObservableCollection<TransactionOutputItem> Items { get; }

        public ObservableCollection<string> Addresses { get; private set; }

        public bool ShowAdvancedSection
        {
            get => this.showAdvancedSection;
            set
            {
                if (this.showAdvancedSection == value) return;

                this.showAdvancedSection = value;

                RaisePropertyChanged();
            }
        }

        public string Fee
        {
            get => this.fee;
            set
            {
                if (this.fee == value) return;

                this.fee = value;

                RaisePropertyChanged();
            }
        }

        public string SelectedChangeAddress
        {
            get => this.selectedChangeAddress;
            set
            {
                if (this.selectedChangeAddress == value) return;

                this.selectedChangeAddress = value;

                RaisePropertyChanged();
            }
        }

        public bool OkEnabled => this.Items.Count > 0;

        public RelayCommand RemarkCommand => new RelayCommand(this.Remark);

        public RelayCommand AdvancedCommand => new RelayCommand(this.ToggleAdvancedSection);

        public RelayCommand OkCommand => new RelayCommand(this.Ok);

        public RelayCommand CancelCommand => new RelayCommand(() => this.Close(this, EventArgs.Empty));
        #endregion

        #region Constructor 
        public TransferViewModel(
            IDialogManager dialogManager,
            IWalletController walletController)
        {
            this.dialogManager = dialogManager;
            this.walletController = walletController;

            this.Items = new ObservableCollection<TransactionOutputItem>();
            this.Addresses = new ObservableCollection<string>();
        }
        #endregion

        #region IDialogViewModel implementation 
        public event EventHandler Close;

        public void OnDialogLoad(TransferLoadParameters parameters)
        {
            var addresses = this.walletController.GetAccountsAddresses();
            this.Addresses.AddRange(addresses);

            this.Items.CollectionChanged += (sender, e) =>
            {
                this.RaisePropertyChanged(nameof(this.OkEnabled));
            };
        }
        #endregion

        #region Private Methods 
        private void Remark()
        {
            var result = this.dialogManager.ShowInputDialog(Strings.EnterRemarkTitle, Strings.EnterRemarkMessage, remark);

            if (string.IsNullOrEmpty(result)) return;

            this.remark = result;
        }

        private void ToggleAdvancedSection()
        {
            this.ShowAdvancedSection = !this.ShowAdvancedSection;
        }

        private async void Ok()
        {
            if (!this.OkEnabled) return;

            var accountScriptHashes = this.walletController.GetAccounts().Select(p => p.ScriptHash.ToString()).ToArray();

            var assetTransferParameters = new AssetTransferTransactionParameters(accountScriptHashes, this.Items, this.SelectedChangeAddress, this.remark, this.Fee);

            await this.walletController.BuildSignAndRelayTransaction(assetTransferParameters);
            
            this.Close(this, EventArgs.Empty);
        }
        #endregion
    }
}