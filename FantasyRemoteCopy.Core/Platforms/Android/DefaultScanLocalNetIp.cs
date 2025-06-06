using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using FantasyResultModel;

namespace FantasyRemoteCopy.Core.Platforms
{
    public class DefaultScanLocalNetIp : IScanLocalNetIp
    {
        private readonly IGetLocalIp _getLocalIp;

        public DefaultScanLocalNetIp(IGetLocalIp getLocalIp)
        {
            _getLocalIp = getLocalIp;
        }


        public Task<ResultBase<List<string>>> ScanLocalNetIpAsync()
        {
            this._getLocalIp.GetLocalIp();
            return null;
        }
    }
}
