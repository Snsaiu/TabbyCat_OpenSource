using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IStoreChatRecordService>]
public sealed class StoreChatRecordService(IPreferenceService preferenceService)
    : LocalConfigService<bool>(preferenceService), IStoreChatRecordService
{
    public override string Key { get; } = "keepChatRecord";
    public override bool Default { get; } = true;
}