﻿namespace Neo.Gui.Base.Dialogs.Results
{
    public class CreateWalletDialogResult
    {
        public string WalletPath { get; }

        public string Password { get; }
        
        public CreateWalletDialogResult(string walletPath, string password)
        {
            this.WalletPath = walletPath;
            this.Password = password;
        }
    }
}
