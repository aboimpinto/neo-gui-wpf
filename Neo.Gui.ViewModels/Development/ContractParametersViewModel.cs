﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Neo.Gui.Dialogs.Interfaces;
using Neo.Network;
using Neo.SmartContract;
using Neo.UI.Core.Globalization.Resources;
using Neo.UI.Core.Wallet;

namespace Neo.Gui.ViewModels.Development
{
    public class ContractParametersViewModel : ViewModelBase
    {
        private readonly IDialogManager dialogManager;
        private readonly IWalletController walletController;

        private ContractParametersContext context;

        private string selectedScriptHashAddress;
        private ContractParameter selectedParameter;

        private string currentValue;
        private string newValue;

        private bool showEnabled;
        private bool broadcastVisible;

        public ContractParametersViewModel(
            IDialogManager dialogManager,
            IWalletController walletController)
        {
            this.dialogManager = dialogManager;
            this.walletController = walletController;

            this.ScriptHashAddresses = new ObservableCollection<string>();
        }
        
        public ObservableCollection<string> ScriptHashAddresses { get; }

        public ObservableCollection<ContractParameter> Parameters
        {
            get
            {
                var emptyCollection = new ObservableCollection<ContractParameter>();

                if (!this.walletController.WalletIsOpen) return emptyCollection;

                if (string.IsNullOrEmpty(this.SelectedScriptHashAddress)) return emptyCollection;

                var scriptHash = this.walletController.AddressToScriptHash(this.SelectedScriptHashAddress);

                if (scriptHash == null) return emptyCollection;

                // Get parameters
                return new ObservableCollection<ContractParameter>(context.GetParameters(scriptHash));
            }
        }

        public string SelectedScriptHashAddress
        {
            get => this.selectedScriptHashAddress;
            set
            {
                if (this.selectedScriptHashAddress == value) return;

                this.selectedScriptHashAddress = value;

                RaisePropertyChanged();

                // Update dependent properties
                RaisePropertyChanged(nameof(this.Parameters));
            }
        }

        public ContractParameter SelectedParameter
        {
            get => this.selectedParameter;
            set
            {
                if (this.selectedParameter == value) return;

                this.selectedParameter = value;

                RaisePropertyChanged();

                // Update dependent properties
                if (this.selectedParameter == null) return;

                this.CurrentValue = this.SelectedParameter.ToString();

                this.NewValue = string.Empty;
            }
        }

        public string CurrentValue
        {
            get => this.currentValue;
            set
            {
                if (this.currentValue == value) return;

                this.currentValue = value;

                RaisePropertyChanged();
            }
        }

        public string NewValue
        {
            get => this.newValue;
            set
            {
                if (this.newValue == value) return;

                this.newValue = value;

                RaisePropertyChanged();
            }
        }

        public bool ShowEnabled
        {
            get => this.showEnabled;
            set
            {
                if (this.showEnabled == value) return;

                this.showEnabled = value;

                RaisePropertyChanged();
            }
        }

        public bool BroadcastVisible
        {
            get => this.broadcastVisible;
            set
            {
                if (this.broadcastVisible == value) return;

                this.broadcastVisible = value;

                RaisePropertyChanged();
            }
        }

        public ICommand LoadCommand => new RelayCommand(this.Load);

        public ICommand ShowCommand => new RelayCommand(this.Show);

        public ICommand BroadcastCommand => new RelayCommand(this.Broadcast);

        public ICommand UpdateCommand => new RelayCommand(this.Update);

        private void Load()
        {
            var input = this.dialogManager.ShowInputDialog("ParametersContext", "ParametersContext");

            if (string.IsNullOrEmpty(input)) return;

            try
            {
                context = ContractParametersContext.Parse(input);
            }
            catch (FormatException ex)
            {
                this.dialogManager.ShowMessageDialog(string.Empty, ex.Message);
                return;
            }

            this.ScriptHashAddresses.Clear();
            this.CurrentValue = string.Empty;
            this.NewValue = string.Empty;

            var contextScriptHashes = context.ScriptHashes.Select(x => x.ToString());
            foreach (var scriptHash in contextScriptHashes)
            {
                this.ScriptHashAddresses.Add(this.walletController.ScriptHashToAddress(scriptHash));
            }

            this.SelectedScriptHashAddress = null;

            this.ShowEnabled = true;

            this.BroadcastVisible = context.Completed;

            RaisePropertyChanged(nameof(this.Parameters));
        }

        private void Show()
        {
            this.dialogManager.ShowInformationDialog("ParametersContext", "ParametersContext", context.ToString());
        }

        private async void Broadcast()
        {
            context.Verifiable.Scripts = context.GetScripts();

            var inventory = (IInventory) context.Verifiable;

            var success = await this.walletController.Relay(inventory);

            if (success)
            {
                this.dialogManager.ShowInformationDialog(Strings.RelaySuccessTitle, Strings.RelaySuccessText, inventory.Hash.ToString());
            }
            else
            {
                this.dialogManager.ShowMessageDialog("Broadcase Unsuccessful", "Data could not be broadcast");//Strings.RelayUnsuccessfulTitle, Strings.RelayUnsuccessfulMessage);
            }
        }

        private void Update()
        {
            if (this.SelectedScriptHashAddress == null || this.SelectedParameter == null) return;

            this.SelectedParameter.SetValue(this.NewValue);

            this.CurrentValue = this.NewValue;

            this.BroadcastVisible = context.Completed;
        }
    }
}