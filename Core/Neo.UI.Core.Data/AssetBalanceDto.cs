using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.UI.Core.Data
{
    public class AssetBalanceDto
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public double Balance { get; private set; }

        public AssetTypeDto AssetType { get; private set; }

        public AssetBalanceDto(string id, string name, double balance, AssetTypeDto assetType)
        {
            this.Id = id;
            this.Name = name;
            this.Balance = balance;
            this.AssetType = assetType;
        }
    }
}
