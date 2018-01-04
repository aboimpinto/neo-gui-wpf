﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Neo.Cryptography.ECC;

using Neo.Gui.Base.Controllers.Interfaces;
using Neo.Gui.Base.Dialogs.Interfaces;
using Neo.Gui.Base.Dialogs.Results.Wallets;
using Neo.Gui.Base.Managers.Interfaces;

namespace Neo.Gui.ViewModels.Accounts
{
    public class CreateLockAccountViewModel : ViewModelBase, IDialogViewModel<CreateLockAccountDialogResult>
    {
        #region Private Fields 
        private const int HoursInDay = 24;
        private const int MinutesInHour = 60;

        private readonly IDialogManager dialogManager;
        private readonly IWalletController walletController;

        private ECPoint selectedKeyPair;
        private DateTime unlockDate;
        private int unlockHour;
        private int unlockMinute;
        #endregion

        #region Public Properties 
        public ObservableCollection<ECPoint> KeyPairs { get; }      // TODO: this is not KeyPairs anymore but a list of PublicKeys. Need to be checked with jlgaffney

        public ECPoint SelectedKeyPair
        {
            get => this.selectedKeyPair;
            set
            {
                if (Equals(this.selectedKeyPair, value)) return;

                this.selectedKeyPair = value;

                RaisePropertyChanged();

                // Update dependent property
                RaisePropertyChanged(nameof(this.CreateEnabled));
            }
        }

        public List<int> Hours { get; }

        public List<int> Minutes { get; }

        public DateTime MinimumDate { get; }

        public DateTime UnlockDate
        {
            get => this.unlockDate;
            set
            {
                if (this.unlockDate == value) return;

                this.unlockDate = value;

                RaisePropertyChanged();
            }
        }

        public int UnlockHour
        {
            get => this.unlockHour;
            set
            {
                if (this.unlockHour == value) return;

                this.unlockHour = value;

                RaisePropertyChanged();
            }
        }

        public int UnlockMinute
        {
            get => this.unlockMinute;
            set
            {
                if (this.unlockMinute == value) return;

                this.unlockMinute = value;

                RaisePropertyChanged();
            }
        }

        public bool CreateEnabled => this.SelectedKeyPair != null;

        public RelayCommand CreateCommand => new RelayCommand(this.HandleCreateAccount);

        public RelayCommand CancelCommand => new RelayCommand(() => this.Close(this, EventArgs.Empty));
        #endregion

        #region Constructor 
        public CreateLockAccountViewModel(
            IDialogManager dialogManager,
            IWalletController walletController)
        {
            this.dialogManager = dialogManager;
            this.walletController = walletController;

            // TODO: All the calls to objects should be in the Loaded method.
            this.KeyPairs = new ObservableCollection<ECPoint>(
                walletController.GetStandardAccounts()
                    .Select(p => p.GetKey().PublicKey).ToArray());

            this.Hours = new List<int>();

            this.Hours = Enumerable.Range(0, HoursInDay).ToList();

            this.Minutes = Enumerable.Range(0, MinutesInHour).ToList();

            var now = DateTime.UtcNow;

            this.MinimumDate = now.Date;
            this.UnlockDate = now;

            // Set unlock time
            var time = now.TimeOfDay;

            this.UnlockHour = time.Hours;
            this.UnlockMinute = time.Minutes;
        }
        #endregion

        #region IDialogViewModel implementation 
        public event EventHandler Close;

        public event EventHandler<CreateLockAccountDialogResult> SetDialogResultAndClose;

        public CreateLockAccountDialogResult DialogResult { get; private set; }
        #endregion

        #region Private Methods
        private void HandleCreateAccount()
        {
            if (this.SelectedKeyPair == null) return;

            var unlockDateTime = this.UnlockDate.Date
                .AddHours(this.UnlockHour)
                .AddMinutes(this.UnlockMinute)
                .ToTimestamp();

            this.walletController.AddAccountContract(this.SelectedKeyPair, unlockDateTime);

            this.Close(this, EventArgs.Empty);
        }
        #endregion
    }
}