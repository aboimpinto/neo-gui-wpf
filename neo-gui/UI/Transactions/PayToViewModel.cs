﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Neo.Extensions;
using Neo.UI.Base.MVVM;
using Neo.Wallets;

namespace Neo.UI.Transactions
{
    public class PayToViewModel : ViewModelBase
    {
        private bool assetSelectionEnabled;
        private AssetDescriptor selectedAsset;

        private bool payToAddressReadOnly;
        private string payToAddress;

        private string amount;

        private TxOutListBoxItem output;

        public PayToViewModel()
        {
            this.Assets = new ObservableCollection<AssetDescriptor>();
        }

        internal ObservableCollection<AssetDescriptor> Assets { get; }

        public bool AssetSelectionEnabled
        {
            get => this.assetSelectionEnabled;
            set
            {
                if (this.assetSelectionEnabled == value) return;

                this.assetSelectionEnabled = value;

                NotifyPropertyChanged();
            }
        }

        internal AssetDescriptor SelectedAsset
        {
            get => this.selectedAsset;
            set
            {
                if (this.selectedAsset == value) return;

                this.selectedAsset = value;

                NotifyPropertyChanged();

                // Update dependent properties
                NotifyPropertyChanged(nameof(this.AssetBalance));
                NotifyPropertyChanged(nameof(this.OkEnabled));
            }
        }

        public string AssetBalance => this.SelectedAsset?.GetAvailable().ToString();

        public bool PayToAddressReadOnly
        {
            get => this.payToAddressReadOnly;
            set
            {
                if (this.payToAddressReadOnly == value) return;

                this.payToAddressReadOnly = value;

                NotifyPropertyChanged();
            }
        }

        public string PayToAddress
        {
            get => this.payToAddress;
            set
            {
                if (this.payToAddress == value) return;

                this.payToAddress = value;
                
                NotifyPropertyChanged();

                // Update dependent property
                NotifyPropertyChanged(nameof(this.OkEnabled));
            }
        }

        public string Amount
        {
            get => this.amount;
            set
            {
                if (this.amount == value) return;

                this.amount = value;

                NotifyPropertyChanged();

                // Update dependent property
                NotifyPropertyChanged(nameof(this.OkEnabled));
            }
        }

        public bool OkEnabled
        {
            get
            {
                if (this.SelectedAsset == null ||
                    string.IsNullOrEmpty(this.PayToAddress) ||
                    string.IsNullOrEmpty(this.Amount)) return false;
                
                try
                {
                    Wallet.ToScriptHash(this.PayToAddress);
                }
                catch (FormatException)
                {
                    return false;
                }

                if (!Fixed8.TryParse(this.Amount, out var parsedAmount)) return false;

                var asset = this.SelectedAsset;

                if (asset == null) return false;

                if (parsedAmount.GetData() % (long) Math.Pow(10, 8 - asset.Precision) != 0) return false;
                
                return true;
            }
        }

        public ICommand OkCommand => new RelayCommand(this.Ok);


        internal void Load(AssetDescriptor asset = null, UInt160 scriptHash = null)
        {
            this.Assets.Clear();

            if (asset != null)
            {
                this.Assets.Add(asset);
                this.SelectedAsset = asset;
                this.AssetSelectionEnabled = false;
            }
            else
            {
                this.Assets.AddRange(AssetDescriptor.GetAssets());

                this.AssetSelectionEnabled = this.Assets.Any();
            }

            if (scriptHash != null)
            {
                this.PayToAddress = Wallet.ToAddress(scriptHash);
                this.PayToAddressReadOnly = true;
            }
        }

        private void Ok()
        {
            this.output = this.GenerateOutput();

            this.TryClose();
        }

        internal TxOutListBoxItem GetOutput()
        {
            return this.output;
        }

        private TxOutListBoxItem GenerateOutput()
        {
            var asset = this.SelectedAsset;

            if (asset == null) return null;

            return new TxOutListBoxItem
            {
                AssetName = asset.AssetName,
                AssetId = asset.AssetId,
                Value = new BigDecimal(Fixed8.Parse(this.Amount).GetData(), 8),
                ScriptHash = Wallet.ToScriptHash(this.PayToAddress)
            };
        }
    }
}