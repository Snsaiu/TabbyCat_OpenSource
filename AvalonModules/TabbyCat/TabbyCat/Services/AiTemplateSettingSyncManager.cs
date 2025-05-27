using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Service.AiServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<IAiTemplateSettingSyncManager>(ServiceLifetime.Singleton)]
public sealed class AiTemplateSettingSyncManager(IUser user,
    IAiTemplateSettingSyncService aiTemplateSettingSyncService,
    IAiTemplateSettingService aiTemplateSettingService):IAiTemplateSettingSyncManager
{
    
    private Thread _thread;


    private void Sync()
    {
        if (!user.LoginSuccess())
        {
          return;
        }
                
        var maxVersion = aiTemplateSettingSyncService.QueryLatestVersionAsync(user.Email).GetAwaiter().GetResult();
        if(!maxVersion.Ok||maxVersion.Data==0)
        {
            return;
        }
                
        var aiTemplateSettings =  aiTemplateSettingService.QueryAsync(x => x.Email == user.Email).GetAwaiter().GetResult();
                
        var currentVersion = 0;
                
        if(aiTemplateSettings.Any())
        {
            currentVersion  = aiTemplateSettings.MaxBy(x => x.Version).Version;
        }
              
        if (maxVersion.Data > currentVersion)
        {
            var lastSettings =  aiTemplateSettingSyncService.SyncRemoteAiTemplateSettingEntitiesAsync(new DownloadSettingDto()
                { Email = user.Email, Version = maxVersion.Data }).GetAwaiter().GetResult();
            if(!lastSettings.Ok||!user.LoginSuccess())
            {
                return;
            }
                  
            foreach (var item in lastSettings.Data)
            {
                item.Email=user.Email;
                item.UpdateTime=DateTime.Now;
            }

            aiTemplateSettingService.DeleteRangeAsync(x=>x.Email == user.Email).GetAwaiter().GetResult();
            aiTemplateSettingService.AddRangeAsync(lastSettings.Data).GetAwaiter().GetResult();
           
        }
    }
    
    public Task StartLoopAsync()
    {
        _thread = new Thread(start: () =>
        {
            while (true)
            {
                Sync();
                Thread.Sleep(1000 * 20);

            }

        })
        {
            IsBackground = true
        };
        _thread.Start();
        
        return Task.CompletedTask;
    }

    public Task SyncSetting()
    {
       return Task.Run(Sync);
    }
}