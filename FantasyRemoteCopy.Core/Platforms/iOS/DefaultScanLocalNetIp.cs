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
        public Task<ResultBase<List<string>>> ScanLocalNetIpAsync()
        {
            throw new NotImplementedException();
        }
    }
}
