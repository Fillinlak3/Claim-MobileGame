using Claim_Macao_MobileGame.Command;
using System.Diagnostics;

namespace Claim_Macao_MobileGame.ViewModel
{
    public class GamePageViewModel : ViewModelBase
    {
        private bool _isVisibleScoreboard;
        public bool isVisibleScoreboard
        {
            get => _isVisibleScoreboard;
            set
            { 
                _isVisibleScoreboard = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand GoBackComand { get; }
        public RelayCommand ShowScoreboardCommand { get; }

        public GamePageViewModel()
        {
            GoBackComand = new RelayCommand(GoBack);
            ShowScoreboardCommand = new RelayCommand(ShowScoreboard);
        }

        private async void GoBack(object? parameter = null)
        {
            await Shell.Current.GoToAsync("///MainPage");
            isVisibleScoreboard = false;
            Debug.WriteLine("Navigated");
        }

        private void ShowScoreboard(object? parameter = null)
        {
            isVisibleScoreboard = !_isVisibleScoreboard;
        }
    }
}
