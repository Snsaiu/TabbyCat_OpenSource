using TabbyCat.IServices.LocalConfigs;
using TuDog.Interfaces.PreferenceServices;
using TuDog.Interfaces.PreferenceServices.Impl;
using TuDog.IocAttribute;

namespace TabbyCat.Services.LocalConfigs;

[Register<IStoreChatRecordService>]
public sealed class StoreChatRecordService
    : LocalConfigService<bool>, IStoreChatRecordService
{
    public override string Key { get; } = "keepChatRecord";
    public override bool Default { get; } = true;
}