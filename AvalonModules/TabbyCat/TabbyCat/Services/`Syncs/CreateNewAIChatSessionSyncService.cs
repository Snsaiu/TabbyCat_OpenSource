using TabbyCat.IServices;
using TabbyCat.Service.AiServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services;

[Register<ICreateNewAIChatSessionSyncService>]
public sealed class CreateNewAIChatSessionSyncService(
    IAiChatSessionService chatSessionService,
    IChatSessionSyncService chatSessionSyncService,
    IUser user)
    : ICreateNewAIChatSessionSyncService
{
    public async Task UpdateAsync()
    {
        if (!user.LoginSuccess())
        {
            return;
        }
        else
        {
            var sessions = (await chatSessionService.QueryAsync(x => x.Email == user.Email)).ToArray();
            var currentVersion = (sessions.MaxBy(x => x.Version)?.Version ?? 0) + 1;
            foreach (var session in sessions)
            {
                session.Version = currentVersion;
                session.LastUpdateTime = DateTime.Now;
            }

            await chatSessionService.UpdateRangeAsync(sessions);

            await chatSessionSyncService.SyncAsync();
        }
    }
}