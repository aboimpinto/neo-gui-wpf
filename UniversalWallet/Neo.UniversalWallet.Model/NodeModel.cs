using Neo.UniversalWallet.Data;

namespace Neo.UniversalWallet.Model
{
    public class NodeModel : INodeModel
    {
        #region Private Fields 
        private readonly IApplicationContext _applicationContext;
        #endregion

        #region Constructor 
        public NodeModel(IApplicationContext applicationContext)
        {
            this._applicationContext = applicationContext;
        }
        #endregion
    }
}
