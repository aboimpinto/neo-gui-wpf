﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Neo.Core;
using Neo.IO.Json;
using Neo.SmartContract;

using Neo.UI.Core.Globalization.Resources;
using Neo.Gui.Dialogs.Interfaces;
using Neo.Gui.Dialogs.LoadParameters.Wallets;
using Neo.Gui.Dialogs.Results.Wallets;
using Neo.UI.Core.Data;
using Neo.UI.Core.Wallet;

namespace Neo.Gui.ViewModels.Wallets
{
    public class TradeViewModel : ViewModelBase, IDialogViewModel<TradeLoadParameters>
    {
        private readonly IDialogManager dialogManager;
        private readonly IWalletController walletController;

        private string payToAddress;
        private string myRequest;
        private string counterPartyRequest;
        
        private bool mergeEnabled;

        private UInt160 scriptHash;

        private int selectedTabIndex;

        public TradeViewModel(
            IDialogManager dialogManager,
            IWalletController walletController)
        {
            this.dialogManager = dialogManager;
            this.walletController = walletController;

            this.Items = new ObservableCollection<TransactionOutputItem>();
        }

        public ObservableCollection<TransactionOutputItem> Items { get; }

        public string PayToAddress
        {
            get => this.payToAddress;
            set
            {
                if (this.payToAddress == value) return;

                this.payToAddress = value;

                RaisePropertyChanged();

                // Update dependent properties
                RaisePropertyChanged(nameof(this.ScriptHash));
                
                try
                {
                    this.ScriptHash = this.walletController.AddressToScriptHash(this.PayToAddress);
                }
                catch (FormatException)
                {
                    this.ScriptHash = null;
                }

                this.Items.Clear();
            }
        }

        public UInt160 ScriptHash
        {
            get => this.scriptHash;
            set
            {
                if (this.scriptHash == value) return;

                this.scriptHash = value;

                RaisePropertyChanged();

                // Update dependent property
                RaisePropertyChanged(nameof(this.ItemsListEnabled));
            }
        }

        public bool ItemsListEnabled => this.ScriptHash != null;

        public string MyRequest
        {
            get => this.myRequest;
            set
            {
                if (this.myRequest == value) return;

                this.myRequest = value;

                RaisePropertyChanged();
            }
        }

        public string CounterPartyRequest
        {
            get => this.counterPartyRequest;
            set
            {
                if (this.counterPartyRequest == value) return;

                this.counterPartyRequest = value;

                RaisePropertyChanged();

                // Update dependent property
                RaisePropertyChanged(nameof(this.ValidateEnabled));
            }
        }

        public bool InitiateEnabled => this.Items.Count > 0;

        public bool ValidateEnabled => !string.IsNullOrEmpty(this.CounterPartyRequest) && !string.IsNullOrEmpty(this.MyRequest);

        public bool MergeEnabled
        {
            get => this.mergeEnabled;
            set
            {
                if (this.mergeEnabled == value) return;

                this.mergeEnabled = value;

                RaisePropertyChanged();
            }
        }

        public int SelectedTabIndex
        {
            get => this.selectedTabIndex;
            set
            {
                if (this.selectedTabIndex == value) return;

                this.selectedTabIndex = value;

                RaisePropertyChanged();
            }
        }

        public ICommand InitiateCommand => new RelayCommand(this.Initiate);

        public ICommand ValidateCommand => new RelayCommand(this.Validate);

        public RelayCommand MergeCommand => new RelayCommand(this.Merge);

        #region IDialogViewModel implementation 
        public event EventHandler Close;

        public void OnDialogLoad(TradeLoadParameters parameters)
        {
        }
        #endregion

        public void UpdateInitiateButtonEnabled()
        {
            RaisePropertyChanged(nameof(this.InitiateEnabled));
        }

        private void Initiate()
        {
            var txOutputs = this.Items.Select(p => p.ToTxOutput());

            var tx = this.walletController.MakeTransaction(new ContractTransaction
            {
                Outputs = txOutputs.ToArray()
            }, fee: Fixed8.Zero);

            this.MyRequest = RequestToJson(tx).ToString();

            this.dialogManager.ShowInformationDialog(Strings.TradeRequestCreatedCaption, Strings.TradeRequestCreatedMessage, this.MyRequest);
            
            this.SelectedTabIndex = 1;
        }

        private async void Validate()
        {
            IEnumerable<CoinReference> inputs;
            IEnumerable<TransactionOutput> outputs;
            var json = JObject.Parse(this.CounterPartyRequest);
            if (json.ContainsProperty("hex"))
            {
                var txMine = this.JsonToRequest(JObject.Parse(this.MyRequest));
                var txOthers = (ContractTransaction)ContractParametersContext.FromJson(json).Verifiable;
                inputs = txOthers.Inputs.Except(txMine.Inputs);
                var outputsOthers = new List<TransactionOutput>(txOthers.Outputs);
                foreach (var outputMine in txMine.Outputs)
                {
                    var outputOthers = outputsOthers.FirstOrDefault(p => p.AssetId == outputMine.AssetId && p.Value == outputMine.Value && p.ScriptHash == outputMine.ScriptHash);
                    if (outputOthers == null)
                    {
                        this.dialogManager.ShowMessageDialog(Strings.Failed, Strings.TradeFailedFakeDataMessage);
                        return;
                    }
                    outputsOthers.Remove(outputOthers);
                }
                outputs = outputsOthers;
            }
            else
            {
                var txOthers = this.JsonToRequest(json);
                inputs = txOthers.Inputs;
                outputs = txOthers.Outputs;
            }

            try
            {
                var tradeIsValid = false;
                foreach (var input in inputs)
                {
                    var transaction = await this.walletController.GetTransaction(input.PrevHash);

                    if (!this.walletController.WalletContainsAccount(transaction.Outputs[input.PrevIndex].ScriptHash .ToString())) continue;
                    tradeIsValid = true;
                    break;
                }

                if (tradeIsValid)
                {
                    this.dialogManager.ShowMessageDialog(Strings.Failed, Strings.TradeFailedInvalidDataMessage);
                    return;
                }
            }
            catch
            {
                this.dialogManager.ShowMessageDialog(Strings.Failed, Strings.TradeFailedNoSyncMessage);
                return;
            }

            outputs = outputs.Where(p => this.walletController.WalletContainsAccount(p.ScriptHash.ToString()));

            var dialogResult = this.dialogManager.ShowDialog<TradeVerificationLoadParameters, TradeVerificationDialogResult>(
                new TradeVerificationLoadParameters(outputs));

            this.MergeEnabled = dialogResult.TradeAccepted;
        }

        private async void Merge()
        {
            ContractParametersContext context;
            var json1 = JObject.Parse(this.CounterPartyRequest);
            if (json1.ContainsProperty("hex"))
            {
                context = ContractParametersContext.FromJson(json1);
            }
            else
            {
                var tx1 = this.JsonToRequest(json1);
                var tx2 = this.JsonToRequest(JObject.Parse(this.MyRequest));
                context = new ContractParametersContext(new ContractTransaction
                {
                    Attributes = new TransactionAttribute[0],
                    Inputs = tx1.Inputs.Concat(tx2.Inputs).ToArray(),
                    Outputs = tx1.Outputs.Concat(tx2.Outputs).ToArray()
                });
            }

            this.walletController.Sign(context);

            if (context.Completed)
            {
                context.Verifiable.Scripts = context.GetScripts();

                var transaction = (ContractTransaction)context.Verifiable;

                var success = await this.walletController.Relay(transaction);

                if (success)
                {
                    this.dialogManager.ShowInformationDialog(Strings.TradeSuccessCaption, Strings.TradeSuccessMessage,
                        transaction.Hash.ToString());
                }
                else
                {
                    
                }
            }
            else
            {
                this.dialogManager.ShowInformationDialog(Strings.TradeNeedSignatureCaption, Strings.TradeNeedSignatureMessage, context.ToString());
            }
        }

        private ContractTransaction JsonToRequest(JObject json)
        {
            return new ContractTransaction
            {
                Inputs = ((JArray)json["vin"]).Select(p => new CoinReference
                {
                    PrevHash = UInt256.Parse(p["txid"].AsString()),
                    PrevIndex = (ushort)p["vout"].AsNumber()
                }).ToArray(),
                Outputs = ((JArray)json["vout"]).Select(p => new TransactionOutput
                {
                    AssetId = UInt256.Parse(p["asset"].AsString()),
                    Value = Fixed8.Parse(p["value"].AsString()),
                    ScriptHash = this.walletController.AddressToScriptHash(p["address"].AsString())
                }).ToArray()
            };
        }

        private JObject RequestToJson(Transaction tx)
        {
            var json = new JObject
            {
                ["vin"] = tx.Inputs.Select(p => p.ToJson()).ToArray(),
                ["vout"] = tx.Outputs.Select((p, i) => p.ToJson((ushort)i)).ToArray(),
                ["change_address"] = this.walletController.ScriptHashToAddress(this.walletController.GetChangeAddress().ToString())
            };
            return json;
        }
    }
}