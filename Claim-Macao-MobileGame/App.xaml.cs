namespace Claim_Macao_MobileGame
{
    public partial class App : Application
    {
        public static Model.GameManager GameManager { get; private set; }

        static App()
        {
            GameManager = new Model.GameManager();
        }

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
