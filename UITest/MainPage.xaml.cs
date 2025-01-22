using System.Windows.Input;

namespace UITest
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();
        }
    }

    public class MainPageViewModel
    {
        public ICommand IconClickCommand { get; set; }

        public MainPageViewModel()
        {
            this.IconClickCommand = new Command( async x =>
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "http://www.baidu.com");
                var content = new StringContent(
                    "",
                    null, "application/json");
               
                var response = await client.SendAsync(request, CancellationToken.None);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
            });
        }
    }
}