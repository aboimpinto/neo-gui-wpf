﻿using Neo.Gui.Wpf.Extensions;
using Neo.Gui.Wpf.Properties;
using Neo.UI.Core.Services.Interfaces;

namespace Neo.Gui.Wpf.Native.Services
{
    public class SettingsManager : ISettingsManager
    {
        public string LastWalletPath
        {
            get => Settings.Default.LastWalletPath;
            set => Settings.Default.LastWalletPath = value;
        }

        public string AppThemeJson
        {
            get => Settings.Default.AppTheme;
            set => Settings.Default.AppTheme = value;
        }

        public bool LightWalletMode
        {
            get => Settings.Default.LightWalletMode;
            set => Settings.Default.LightWalletMode = value;
        }

        public string[] LightWalletRpcSeedList => Settings.Default.LightWallet.RpcSeedList;

        public string CertificateCachePath => Settings.Default.Paths.CertCache;

        public string[] NEP5WatchScriptHashes
        {
            get => Settings.Default.NEP5Watched.ToArray();
            set
            {
                Settings.Default.NEP5Watched.Clear();
                Settings.Default.NEP5Watched.AddRange(value);
            }
        }

        #region Local Node Settings

        public string BlockchainDataDirectoryPath => Settings.Default.Paths.Chain;

        public int LocalNodePort => Settings.Default.P2P.Port;

        public int LocalWSPort => Settings.Default.P2P.WsPort;

        #endregion

        #region URL Format Settings

        public string AddressURLFormat => Settings.Default.Urls.AddressUrl;

        public string AssetURLFormat => Settings.Default.Urls.AssetUrl;

        public string TransactionURLFormat => Settings.Default.Urls.TransactionUrl;

        #endregion

        public void Save()
        {
            Settings.Default.Save();
        }
    }
}
