using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit.Utils;
using Microsoft.Extensions.DependencyInjection;
using TabbyCat.Enums;
using TabbyCat.Factories;
using TabbyCat.IServices;
using TabbyCat.Models;
using TabbyCat.Models.AiReqRes.AiChatRequests;
using TabbyCat.Models.AiReqRes.AiChatRequests.TabbyCatAi;
using TabbyCat.Models.AiReqRes.AiChatResponses;
using TabbyCat.Shared;
using TabbyCat.Shared.Enums;
using TuDog.Bootstrap;

namespace TabbyCat.Views;

public partial class QuickMenuItemWindow : Window
{
    private string _text = string.Empty;

    private INavigationMenuItemService _navigationMenuItemService =
        TuDogApplication.ServiceProvider.GetRequiredService<INavigationMenuItemService>();

    public QuickMenuItemWindow()
    {
        var localization = LocalizationResourceManager.Instance;
        
       _languages = new Dictionary<string, string>()
        {
            {localization["English"],"English"},
            {localization["Chinese"],"Chinese"},
            {localization["Japanese"],"Japanese"},
            {localization["Korean"],"Korean"},
            {localization["Thai"],"Thai"},
            {localization["French"],"French"},
            {localization["German"],"German"},
            {localization["Spanish"],"Spanish"},
            {localization["Arabic"],"Arabic"},
            {localization["Indonesian"],"Indonesian"},
            {localization["Vietnamese"],"Vietnamese"},
            {localization["Portuguese"],"Portuguese"},
            {localization["Italian"],"Italian"},
            {localization["Dutch"],"Dutch"},
            {localization["Russian"],"Russian"},
            {localization["Khmer"],"Khmer"},
            {localization["Cebuano"],"Cebuano"},
            {localization["Filipino"],"Filipino"},
            {localization["Czech"],"Czech"},
            {localization["Polish"],"Polish"},
            {localization["Persian"],"Persian"},
            {localization["Hebrew"],"Hebrew"},
            {localization["Turkish"],"Turkish"},
            {localization["Hindi"],"Hindi"},
            {localization["Bengali"],"Bengali"},
            {localization["Urdu"],"Urdu"},
        };
        Dictionary<string, string> lansourceDic = new Dictionary<string, string>();
        lansourceDic.Add("Auto","auto");
        lansourceDic.AddRange(_languages);
        
        InitializeComponent();
        lanSource.ItemsSource= lansourceDic.Select(x=>x.Key);
        lanOutput.ItemsSource = _languages.Select(x=>x.Key);

        lanSource.SelectedIndex = 0;
        var culture = localization.GetCulture();

        if (culture.Name == "en-US")
        {
            lanOutput.SelectedIndex = 1;
        }
        else
        {
            lanOutput.SelectedIndex = 0;
        }
    }

    public void Init(string text)
    {
        _text = text;
        this.translateGrid.IsVisible = false;
        this.inputSource.Text = _text;
        this.outputSource.Text = string.Empty;
    }

    private Dictionary<string, string> _languages;
    private void AskClick(object? sender, RoutedEventArgs e)
    {
        Send(new QuickMenuItemModel() { Content = _text, Type = QuickMenuItem.Ask });
    }

    private void Send(QuickMenuItemModel model)
    {
        TuDogApplication.MainWindow.WindowState = WindowState.Normal;
        TuDogApplication.MainWindow?.Show();
        TuDogApplication.MainWindow?.Activate();

        _navigationMenuItemService.NavigationAsync(AiMediaWorkType.AiChat, model).ContinueWith(x =>
        {
            Dispatcher.UIThread.Invoke(() => { this.Hide(); });
        });
    }

    private void CopyClick(object? sender, RoutedEventArgs e)
    {
        TuDogApplication.TopLevel.Clipboard.SetTextAsync(_text).ContinueWith(x =>
        {
            Dispatcher.UIThread.Invoke(() => { this.Hide(); });
        });
    }

    private void SummarizeClick(object? sender, RoutedEventArgs e)
    {
        Send(new QuickMenuItemModel() { Content = _text, Type = QuickMenuItem.Summarize });
    }

    private void TranslateClick(object? sender, RoutedEventArgs e)
    {
        this.translateGrid.IsVisible = !this.translateGrid.IsVisible;
    }

    private async void ConfirmTraslateClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(inputSource.Text))
            return;

        var aimodel = await AiTemplateFactory.GetTemplateAsync(AiModelType.DeepSeek);

        outputSource.Text = string.Empty;

        var requestMessage = new TabbyCatAiRequestModel();
        requestMessage.IsTranslate = true;
        requestMessage.Messages.Add(MessagesItem.Create(inputSource.Text, Role.User, Guid.CreateVersion7(), false, true,
            null));
        requestMessage.TranslationParameter.SourceLang = lanSource.SelectedItem=="Auto" ? "auto" : _languages.First(x=>x.Key==lanSource.SelectedValue.ToString()).Value;
        requestMessage.TranslationParameter.TargetLang = _languages.First(x=>x.Key==lanOutput.SelectedValue.ToString()).Value;
        requestMessage.Stream = true;

        var server = AiRequestFactory.CreateService(requestMessage, aimodel);
        await server.StreamProcessResponseAsync((text) =>
        {
            if (text is UnityResponseModel model)
            {
                if (!model.Ok)
                    return Task.FromResult(true);

                if (model.StreamFinished)
                    return Task.FromResult(true);
                
                Dispatcher.UIThread.Invoke(() => { outputSource.Text = model.Content; });
            }


            return Task.FromResult(false);
        }, default);
    }
}