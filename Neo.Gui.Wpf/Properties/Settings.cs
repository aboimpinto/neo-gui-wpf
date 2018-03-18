﻿using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Neo.Gui.Wpf.Properties
{
    internal sealed partial class Settings
    {
        public LightWalletSettings LightWallet { get; }

        public PathsSettings Paths { get; }

        public P2PSettings P2P { get; }
        
        public BrowserSettings Urls { get; }

        public ContractSettings Contracts { get; }

        public Settings()
        {
            if (NeedUpgrade)
            {
                Upgrade();
                NeedUpgrade = false;
                Save();
            }
            var section = new ConfigurationBuilder().AddJsonFile("config.json").Build().GetSection("ApplicationConfiguration");

            this.LightWallet = new LightWalletSettings(section.GetSection("LightWallet"));

            this.Paths = new PathsSettings(section.GetSection("Paths"));

            this.P2P = new P2PSettings(section.GetSection("P2P"));
            
            this.Urls = new BrowserSettings(section.GetSection("Urls"));

            this.Contracts = new ContractSettings(section.GetSection("Contracts"));
        }
    }

    internal class PathsSettings
    {
        public string Chain { get; }
        public string CertCache { get; }

        public PathsSettings(IConfigurationSection section)
        {
            this.Chain = section.GetSection("Chain").Value;
            this.CertCache = section.GetSection("CertCache").Value;
        }
    }

    internal class P2PSettings
    {
        public ushort Port { get; }
        public ushort WsPort { get; }

        public P2PSettings(IConfigurationSection section)
        {
            this.Port = ushort.Parse(section.GetSection("Port").Value);
            this.WsPort = ushort.Parse(section.GetSection("WsPort").Value);
        }
    }

    internal class BrowserSettings
    {
        public string AddressUrl { get; }
        public string AssetUrl { get; }
        public string TransactionUrl { get; }

        public BrowserSettings(IConfigurationSection section)
        {
            this.AddressUrl = section.GetSection("AddressUrl").Value;
            this.AssetUrl = section.GetSection("AssetUrl").Value;
            this.TransactionUrl = section.GetSection("TransactionUrl").Value;
        }
    }

    internal class ContractSettings
    {
        public UInt160[] NEP5 { get; }

        public ContractSettings(IConfigurationSection section)
        {
            this.NEP5 = section.GetSection("NEP5").GetChildren().Select(p => UInt160.Parse(p.Value)).ToArray();
        }
    }

    internal class LightWalletSettings
    {
        public string[] RpcSeedList { get; }

        public LightWalletSettings(IConfigurationSection section)
        {
            this.RpcSeedList = section.GetSection("RpcSeedList").GetChildren().Select(p => p.Value).ToArray();
        }
    }
}
