using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Neo.UniversalWallet.Data
{
    public class WalletDto
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        public ScryptDto scrypt { get; set; }

        [JsonProperty(PropertyName = "accounts")]
        public IEnumerable<Account> Accounts { get; set; }

        [JsonProperty(PropertyName = "extra")]
        public string Extra { get; set; }
    }

    public class Account
    {
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty(PropertyName = "lock")]
        public bool Lock { get; set; }

        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        [JsonProperty(PropertyName = "contract")]
        public AccountContract Contract { get; set; }

        [JsonProperty(PropertyName = "extra")]
        public string Extra { get; set; }
    }

    public class AccountContract
    {
        [JsonProperty(PropertyName = "script")]
        public string Script { get; set; }

        [JsonProperty(PropertyName = "parameters")]
        public IEnumerable<ContractParameter> Paramerters { get; set; }

        [JsonProperty(PropertyName = "deployed")]
        public bool Deployed { get; set; }
    }

    public class ContractParameter
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }

    public class ScryptDto
    {
        public string n { get; set; }

        public string r { get; set; }

        public string p { get; set; }
    }
}
