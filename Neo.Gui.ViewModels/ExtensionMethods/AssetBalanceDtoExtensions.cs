using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Neo.UI.Core.Data;

namespace Neo.Gui.ViewModels.ExtensionMethods
{
    public static class AssetBalanceDtoExtensions
    {
        public static AssetBalanceDto RetrieveNeoToken(this IEnumerable<AssetBalanceDto> assets)
        {
            return assets.Single(x => x.AssetType == AssetTypeDto.GoverningToken);
        }

        public static AssetBalanceDto RetrieveGasToken(this IEnumerable<AssetBalanceDto> assets)
        {
            return assets.Single(x => x.AssetType == AssetTypeDto.UtilityToken);
        }

        public static IReadOnlyCollection<AssetBalanceDto> RetrieveNep5Tokens(this IEnumerable<AssetBalanceDto> assets)
        {
            return new ReadOnlyCollection<AssetBalanceDto>(assets.Where(x => x.AssetType == AssetTypeDto.Token).ToList());
        }
    }
}
