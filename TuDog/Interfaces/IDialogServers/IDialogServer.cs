using TuDog.Bootstrap;
using TuDog.Enums;
using TuDog.Models;

namespace TuDog.Interfaces.IDialogServers;

public interface IDialogServer
{
    public Task ShowMessageDialogAsync(string message, string title = "提示",
        string buttonText = "确定");

    public Task<bool> ShowConfirmDialogAsync(string message, string title = "提示", string confirmButtonText = "确定",
        string cancelButtonText = "取消");

    public Task<DialogResultData<string>> ShowInputDialogAsync(string message, string title = "提示",
        string placeHolder = "请输入...", string confirmButtonText = "确定",
        string cancelButtonText = "取消", string? defaultValue = null);

    public Task<DialogResultData<TResult>?> ShowDialogAsync<TViewModel,TParameter, TResult>(string title,
        string confirmButtonText = "确定",
        string cancelButtonText = "取消", TParameter? parameter=default )
        where TViewModel : DialogViewModelBaseAsync<TParameter,TResult>;

    public Task<DialogResultData<object>?> ShowDialogAsync<TViewModel, TParameter>(string title,
        string confirmButtonText = "确定",
        string cancelButtonText = "取消", TParameter? parameter = default) where TViewModel : DialogViewModelBaseAsync;
}