using Claim_Macao_MobileGame.ViewModel;
using System.Diagnostics;

namespace Claim_Macao_MobileGame.View
{
    public partial class GamePage : ContentPage
	{
        List<Entry> PlayersEntries;

		public GamePage(GamePageViewModel vm)
		{
			InitializeComponent();
			BindingContext = vm;
            PlayersEntries = new List<Entry>();
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            if (App.GameManager.Players.All(p => p.GameOver == false) == false)
                return;

            // Disable the button.
            UpdateScoreBtn.IsEnabled = false;
            UpdateScoreBtn.Background = new SolidColorBrush(Colors.DarkGray);

            AddPlayersOnScreen();
            UpdateScoreboard();
        }

        private void AddPlayersOnScreen()
        {
            PlayersEntries.Clear();
            PlayersGrid.Children.Clear();

            int rows = PlayersGrid.RowDefinitions.Count;
            int cols = PlayersGrid.ColumnDefinitions.Count;
            int playerIndex = 0;

            for (int j = 0; j < cols && playerIndex < App.GameManager.Players.Count; j++)
            {
                for (int i = 0; i < rows && playerIndex < App.GameManager.Players.Count; i++, playerIndex++)
                {
                    // Create Frame
                    Frame frame = new Frame
                    {
                        BorderColor = Color.FromRgb(34, 176, 255),
                        Background = Color.FromRgba(0, 0, 0, 0.6),
                        CornerRadius = 5,
                        Padding = new Thickness(5)
                    };

                    // Create Vertical StackLayout
                    VerticalStackLayout verticalStackLayout = new VerticalStackLayout
                    {
                        Spacing = -10
                    };

                    // Create Label
                    Label label = new Label
                    {
                        Text = App.GameManager.Players[playerIndex].Name,
                        FontFamily = "OpenSans-Semibold",
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    // Create Entry
                    Entry entry = new Entry
                    {
                        Placeholder = "score",
                        PlaceholderColor = Colors.GhostWhite,
                        FontFamily = "OpenSansRegular",
                        Keyboard = Keyboard.Numeric,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    entry.Completed += (sender, args) =>
                    {
                        int index = -1;
                        if (sender is Entry _entry && _entry is not null)
                        {
                            _entry.Unfocus();
                            index = PlayersEntries.IndexOf(_entry);
                        }

                        // Something went wrong.
                        if (index == -1) return;
                        // Last entry.
                        else if(index >= PlayersEntries.Count - 1)
                        {
                            HideKeyboard();
                        }
                        else PlayersEntries[index + 1].Focus();
                    };
                    entry.TextChanged += (sender, e) =>
                    {
                        string points = new string(entry.Text.Where(c => char.IsDigit(c) || c == '-').ToArray());

                        if (points.Length > 1 && points[0] == '0')
                            points = points.TrimStart('0');

                        entry.Text = points;

                        if (PlayersEntries.Count == 0 || PlayersEntries is null) return;

                        if (PlayersEntries.Any(x => string.IsNullOrWhiteSpace(x.Text) == false))
                        {
                            // Enable the button.
                            UpdateScoreBtn.IsEnabled = true;
                            UpdateScoreBtn.Background = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            // Disable the button.
                            UpdateScoreBtn.IsEnabled = false;
                            UpdateScoreBtn.Background = new SolidColorBrush(Colors.DarkGray);
                        }
                    };
                    PlayersEntries.Add(entry);

                    // Add Label and Entry to VerticalStackLayout
                    verticalStackLayout.Children.Add(label);
                    verticalStackLayout.Children.Add(entry);

                    /* Add new functionions on Player's Frame:
                        - Single tap to edit entry
                        - Double tap to quickly erase the entry points.
                     */
                    var singleTapGestureRecognizer = new TapGestureRecognizer();
                    singleTapGestureRecognizer.NumberOfTapsRequired = 1;
                    singleTapGestureRecognizer.Tapped += (sender, e) =>
                    {
                        entry.Focus();
                    };
                    frame.GestureRecognizers.Add(singleTapGestureRecognizer);
                    var doubleTapGestureRecognizer = new TapGestureRecognizer();
                    doubleTapGestureRecognizer.NumberOfTapsRequired = 2;
                    doubleTapGestureRecognizer.Tapped += (sender, e) =>
                    {
                        entry.Text = string.Empty;
                    };
                    frame.GestureRecognizers.Add(doubleTapGestureRecognizer);

                    // Set Content of Frame to VerticalStackLayout
                    frame.Content = verticalStackLayout;

                    // Add Frame in PlayersGrid.
                    PlayersGrid.SetRow(frame, i);
                    PlayersGrid.SetColumn(frame, j);
                    PlayersGrid.Children.Add(frame);
                }
            }
        }

        private void HideKeyboard()
        {
            if (PlayersEntries.Count == 0 || PlayersEntries is null)
                throw new ArgumentNullException("All entries are null or empty.");

            // Unfocus & hide keyboard.
            foreach (var entry in PlayersEntries)
            {
                entry.IsEnabled = false;
                entry.IsEnabled = true;
            }
        }

        private void UpdateScoreboard()
        {
            HideKeyboard();

            Scoreboard.Text = string.Empty;
            foreach (var player in App.GameManager.Players.OrderByDescending(p => p.Points))
                Scoreboard.Text += $"{player.Name}: {player.Points} -- <State: {(player.GameOver == true ? "eliminated" : "playing")}>\n";
        }

        private void RearrangeGrid()
        {
            int row = 0;
            int col = 0;

            // Iterate through each cell in the grid
            foreach (var child in PlayersGrid.Children)
            {
                Grid.SetRow(child as BindableObject, row);
                Grid.SetColumn(child as BindableObject, col);

                row++;
                if (row >= PlayersGrid.RowDefinitions.Count)
                {
                    row = 0;
                    col++;
                }
            }
        }

        private async void UpdateScore(object sender, EventArgs e)
        {
            // Firstly disable the button.
            UpdateScoreBtn.IsEnabled = false;
            UpdateScoreBtn.Background = new SolidColorBrush(Colors.DarkGray);

            // Then hide the keyboard.
            HideKeyboard();

            /*
                If every input is numeric, then add the corresponding points to each player. 
                If nothing is inputed, then it will automatically add 0.
                Clear the entries.
             */
            List<Model.Player> RemainingPlayers = App.GameManager.Players.Where(p => p.GameOver == false).ToList();
            for (int i = 0; i < RemainingPlayers.Count; i++)
            {
                string pointsText = PlayersEntries[i].Text;
                PlayersEntries[i].Text = string.Empty;
                RemainingPlayers[i].Points += string.IsNullOrWhiteSpace(pointsText) ? 0 : int.Parse(pointsText);
            }

            // Check players in-game state.
            int removedCount = 0;
            foreach (var player in RemainingPlayers)
            {
                // Reset players score if it's same as the maximum score.
                if(player.Points == App.GameManager.MaxPoints)
                {
                    player.Points = 0;
                    player.GameOver = false;
                    await DisplayAlert("So lucky!", $"{player.Name}'s score reset.", "OK");
                }
                // Skip the already removed player.
                else if(player.Points > App.GameManager.MaxPoints)
                {
                    player.GameOver = true;
                    await DisplayAlert("Eliminated", $"{player.Name} was eliminated.", "OK");
                    int indexWhere = RemainingPlayers.IndexOf(player) - removedCount++;
                    Debug.WriteLine(indexWhere);
                    PlayersGrid.Children.RemoveAt(indexWhere);
                    PlayersEntries.RemoveAt(indexWhere);
                    RearrangeGrid();
                }
            }
            // Update the list after checks.
            RemainingPlayers.RemoveAll(p => p.GameOver == true);
                
            UpdateScoreboard();

            // Check for winner.
            if (RemainingPlayers.Count == 1)
            {
                await DisplayAlert("CONGRATULATIONS!",
                    $"The winner is: {RemainingPlayers.First().Name}", "GG");
                App.GameManager.Players.Clear();

                (BindingContext as GamePageViewModel)!.GoBackComand.Execute(null);
            }
        }

        private async void ResetScore(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Reset game?",
                "Do you want to reset the game? Players score will be lost.", "Yes", "Cancel");

            if (result)
            {
                // Reset players score.
                foreach (var player in App.GameManager.Players)
                {
                    player.Points = 0;
                    player.GameOver = false;
                }
                AddPlayersOnScreen();
                UpdateScoreboard();
            }
        }

        private void SwipeToHideKeyboard(object sender, SwipedEventArgs e)
        {
            HideKeyboard();
        }

        private void TapToHideKeyboard(object sender, TappedEventArgs e)
        {
            HideKeyboard();
        }
    }
}