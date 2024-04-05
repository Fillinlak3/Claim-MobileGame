using Claim_Macao_MobileGame.Command;
using System.Diagnostics;

namespace Claim_Macao_MobileGame.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        public int MaximumPoints
        {
            get => App.GameManager.MaxPoints;
            set { }
        }

        private bool _isVisibleBottomPanel = true;
        public bool isVisibleBottomPanel
        {
            get => _isVisibleBottomPanel;
            set
            {
                _isVisibleBottomPanel = value;
                OnPropertyChanged();
            }
        }

        private bool _isVisibleSettingsPanel = false;
        public bool isVisibleSettingsPanel
        {
            get => _isVisibleSettingsPanel;
            set
            {
                _isVisibleSettingsPanel = value;
                // Also when changing Settings Panel visibility, update the Bottom's Panel too.
                isVisibleBottomPanel = !_isVisibleSettingsPanel;
                OnPropertyChanged();
            }
        }

        public RelayCommand StartGameCommand { get; }

        public MainPageViewModel()
        {
            StartGameCommand = new RelayCommand(StartGame);
        }

        private async void StartGame(object? parameter = null)
        {
            await Shell.Current.GoToAsync("///GamePage");
        }
    }
}
