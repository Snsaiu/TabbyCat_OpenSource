using FantasyResultModel;
using FantasyResultModel.Impls;

using System.Net.NetworkInformation;

using System.Text;

namespace FantasyRemoteCopy.Core.Platforms;

public class GlobalScanLocalNetIp : IGlobalScanLocalNetIp
{
    private readonly IGetLocalIp _getLocalIp;
    public GlobalScanLocalNetIp(IGetLocalIp getLocalIp)
    {
        _getLocalIp = getLocalIp;
    }

    public async Task<ResultBase<bool>> GlobalSearch()
    {

        ResultBase<List<string>> localIpResult = _getLocalIp.GetLocalIp();
        if (localIpResult.Ok == false)
            return await Task.FromResult(new ErrorResultModel<bool>(localIpResult.ErrorMsg ?? string.Empty));
        List<Task> tasks = [];
        try
        {
            List<string> localIps = localIpResult.Data;

            foreach (string ip in localIps)
            {
                Task t = Task.Run(() =>
                {
                    string ipDuan = ip.Remove(ip.LastIndexOf('.'));
                    //MessageBox.Show(ipDuan);
                    //枚举网段计算机

                    string data = "";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);


                    for (int i = 1; i < 255; i++)
                    {
                        string pingIP = ipDuan + "." + i.ToString();

                        try
                        {
                            Ping myPing = new Ping();

                            PingReply pingReply = myPing.Send(pingIP, 20, buffer);


                        }
                        catch (Exception)
                        {
                        }


                    }


                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());



            return new SuccessResultModel<bool>(true);

        }
        catch (Exception e)
        {
            return new ErrorResultModel<bool>(e.Message);
        }


    }
}