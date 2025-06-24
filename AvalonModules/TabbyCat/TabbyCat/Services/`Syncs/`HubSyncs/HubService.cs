using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TabbyCat.IServices;
using TabbyCat.Models.HubModels;
using TabbyCat.Repository.Entities.AiEntities;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IHubService>(ServiceLifetime.Singleton)]
public sealed class HubService : IHubService
{
    private readonly IUser _user;
    private readonly ILogger<HubService> _logger;
    private HubConnection? _connection;

    private const string Url = "https://api.yyan.cc/yyan-hubs/tabbycat-hub/route";

    public HubService(IUser user, ILogger<HubService> logger)
    {
        _user = user;
        _logger = logger;
    }

    public async Task StartAsync()
    {
        try
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(Url, option => { option.AccessTokenProvider = () => Task.FromResult(_user.AccessToken); })
                .WithAutomaticReconnect()
                .Build();
            await _connection.StartAsync();
            if (_connection.State == HubConnectionState.Connected)
            {
                _logger.LogInformation("Hub 连接成功!");

                Register<HubLoginModel>("NewDeviceLogin", (device) =>
                {
                    _logger.LogInformation("新客户端登录:{0}", device.DeviceType);
                    return Task.CompletedTask;
                });
                //发送登录
                await SendMessageAsync("NewDeviceLoginAsync", new HubLoginModel());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Hub 连接失败：" + ex.Message);
        }
    }


    public void Register<T>(string methodName, Func<T, Task>? callback)
    {
        if (_connection is null)
        {
            _logger.LogError("hub没有初始化！");
            return;
        }

        _connection.On<T>(methodName, message =>
        {
            _logger.LogInformation("接收消息{0}", JsonConvert.SerializeObject(message));
            callback?.Invoke(message);
        });
    }

    public Task StopAsync()
    {
        return _connection is null ? Task.CompletedTask : _connection.StopAsync();
    }

    public Task SendMessageAsync<T>(string methodName, T? data)
    {
        if (_connection is null)
        {
            return Task.CompletedTask;
        }
        else
        {
            _logger.LogInformation("发送到Hub同步，调用Hub的方法名称为:{0},数据为:{1}", methodName, JsonConvert.SerializeObject(data));
            return _connection.InvokeAsync(methodName, data);
        }
    }
}