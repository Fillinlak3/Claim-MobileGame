namespace Claim_Macao_MobileGame
{
    public partial class App : Application
    {
        public static Model.GameManager GameManager { get; private set; }

        public App()
        {
            InitializeComponent();

            GameManager = new Model.GameManager();
            MainPage = new AppShell();
        }
    }
}
