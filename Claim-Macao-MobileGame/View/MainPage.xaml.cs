using Claim_Macao_MobileGame.Model;
using Claim_Macao_MobileGame.ViewModel;
using System.Diagnostics;

namespace Claim_Macao_MobileGame.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            SettingsPanel.TranslationY = 250;
        }

        private void PlayersSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = Math.Round(e.NewValue);
            SliderValueLabel.Text = newStep.ToString();
            ((Slider)sender).Value = newStep;

            AddPlayersOnScreen((int)newStep);
        }

        private void AddPlayersOnScreen(int numberOfPlayers)
        {
            // Reversing the iterations will resolve glitches.
            for (int i = 18 - 1; i >= numberOfPlayers; i--)
            {
                (PlayersGrid.Children[i] as Microsoft.Maui.Controls.View)!.IsVisible = false;
                (PlayersGrid.Children[i] as Entry)!.Text = string.Empty;
            }

            for (int i = 0; i < numberOfPlayers - 5; i++)
                (PlayersGrid.Children[5 + i] as Microsoft.Maui.Controls.View)!.IsVisible = true;
        }

        private void HideKeyboard()
        {
            if (PlayersGrid.Children.Count == 0 || PlayersGrid is null)
                throw new ArgumentNullException("All entries are null or empty.");

            foreach (var child in SettingsPanel.Children)
            {
                if (child is Entry _entry && _entry is not null)
                {
                    _entry.IsEnabled = false;
                    _entry.IsEnabled = true;
                    break;
                }
            }

            if (PlayersGrid.Children.All(x => x.IsEnabled == false))
                return;

            // Unfocus & hide keyboard.
            foreach (var child in PlayersGrid.Children)
            {
                if (child is Entry _entry && _entry is not null)
                {
                    _entry.IsEnabled = false;
                    _entry.IsEnabled = true;
                }
            }
        }

        private void EnablePlayersEntries(bool enable)
        {
            foreach (var entry in PlayersGrid.Children)
            {
                if (entry is Entry _entry && _entry is not null)
                    _entry.IsEnabled = enable;
            }
        }

        private void TapToHideKeyboard(object sender, TappedEventArgs e)
        {
            HideKeyboard();
        }

        private async void SwipeToHideSettingsPanel(object sender, SwipedEventArgs e)
        {
            HideKeyboard();
            if (SettingsPanel.IsVisible)
            {
                // Enable all entries while settings panel is visible.
                EnablePlayersEntries(true);

                // Animate the settings panel to slide out of view
                await SettingsPanel.TranslateTo(0, SettingsPanel.Height, 250, Easing.SinInOut);

                // Optionally, toggle visibility of the settings panel
                (BindingContext as MainPageViewModel)!.isVisibleSettingsPanel = false;
            }
        }

        private async void SettingsButton(object sender, EventArgs e)
        {
            if (SettingsPanel.IsVisible == false)
            {
                // Disable all entries while settings panel is visible.
                EnablePlayersEntries(false);

                // Make the settings panel visible
                (BindingContext as MainPageViewModel)!.isVisibleSettingsPanel = true;

                // Animate the settings panel to slide up from the bottom to its original position
                await SettingsPanel.TranslateTo(0, 0, 250, Easing.SinInOut);
            }
        }

        private async void StartGameButton(object sender, EventArgs e)
        {
            /*
                Check if there's a game already started and to be continued. (BACKUP)
            */
            if (App.GameManager.Players.Count > 0)
            {
                /*
                    If a game is already started but user went back to the main screen, there's no
                    need to add players and so on. Just resume to the game-page, content is already set.
                
                    UPDATE: Removed because it was blocking new players to start a game when already started.
                 */
                //if (App.GameManager.Players.All(p => p.Points == 0) && )
                //{
                //    await Shell.Current.GoToAsync("///GamePage");
                //    return;
                //}

                // If players have a score, then ask the user to resume the game.

                var result = await DisplayAlert("Resume game?",
                    "There is a game currently running in background. Do you want to resume?", "Resume", "New game");

                // Check if the user want to resume the game. (RESTORE BACKUP)
                if(result)
                {
                    await Shell.Current.GoToAsync("///GamePage");
                    return;
                }

                result = await DisplayAlert("New game?", "Do you want to start a new game instead?", "Yes", "Cancel");
                // Check if the user didn't resume the game and doesn't want to start a new game.
                if (result == false) return;

                // User didn't want to resume but wanted to start a new game. (RESET BACKUP)
                App.GameManager.Players.Clear();
            }
                
            try
            {
                List<string> PlayerNames = new List<string>();
                for (int i = 0; i < 18; i++)
                {
                    // When a hidden entry is hit stop the loop.
                    if ((PlayersGrid.Children[i] as Entry)!.IsVisible == false)
                        break;
                    
                    // Check for blank inputs in entries.
                    if(string.IsNullOrWhiteSpace((PlayersGrid.Children[i] as Entry)!.Text))
                    {
                        // Check if the first player is missing.
                        if (i == 0)
                            throw new Exception("First player cannot be empty.");
                        // Check for minimum players count.
                        else if (i == 1)
                            throw new Exception("Cannot play solo, 2 players minimum.");
                        /*
                            If all entries are valid but number of players is less than the actual
                            visible entries, stop the loop. => Everything is OK.
                         */
                        else if (RemainingsAreEmptyToo(i))
                            break;
                        // Otherwise, it means that a blank entry is found, so cannot continue.
                        else throw new Exception($"Cannot have blank entries between players {i} and {i + 1}.");
                    }

                    string playername = (PlayersGrid.Children[i] as Entry)!.Text.Trim();
                    // Check for players with the same name.
                    if (PlayerNames.Contains(playername))
                        throw new Exception("Players must have unique names.");

                    // If after all checks, everything is fine.
                    PlayerNames.Add(playername);
                }

                /*
                    Each playername will be assigned to a Player instance creating the Players list
                    and the game can begin.
                */
                App.GameManager.Players.AddRange(PlayerNames.Select(name => new Player(name, 0, false)));
                PlayerNames.Clear();
                await Shell.Current.GoToAsync("///GamePage");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Couldn't start", ex.Message, "OK");
            }
        }

        private bool RemainingsAreEmptyToo(int index)
        {
            for (int i = index; i < 18; i++)
            {
                // When a hidden entry is hit stop the loop.
                if ((PlayersGrid.Children[i] as Entry)!.IsVisible == false)
                    return true;

                if (string.IsNullOrWhiteSpace((PlayersGrid.Children[i] as Entry)!.Text) == false)
                    return false;
            }
            return true;
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            int index = -1;
            if (sender is Entry _entry && _entry is not null)
            {
                _entry.Unfocus();
                index = PlayersGrid.IndexOf(_entry);
            }

            // Something went wrong.
            if (index == -1) return;
            // Last entry.
            int maximumVisible = PlayersGrid.IndexOf(PlayersGrid.FirstOrDefault(x => x.Visibility == Visibility.Collapsed));
            int maximumEntries = PlayersGrid.Count - 1;
            //                 Last visible entry                     Last of entries
            if ((maximumVisible != -1 && index >= maximumVisible) || index >= maximumEntries)
                HideKeyboard();
            else PlayersGrid[index + 1].Focus();
        }
    }
}
