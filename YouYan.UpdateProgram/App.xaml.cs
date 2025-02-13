namespace YouYan.UpdateProgram
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "更新程序",
                Width = 600, Height = 400,
                MaximumHeight = 400,MaximumWidth = 600,
                MinimumHeight = 400,MinimumWidth = 600
            };
        }
    }
}
