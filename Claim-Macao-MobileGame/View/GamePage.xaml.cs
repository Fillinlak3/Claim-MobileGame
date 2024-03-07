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
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center
                    };
                    PlayersEntries.Add(entry);

                    // Add Label and Entry to VerticalStackLayout
                    verticalStackLayout.Children.Add(label);
                    verticalStackLayout.Children.Add(entry);

                    // Set Content of Frame to VerticalStackLayout
                    frame.Content = verticalStackLayout;

                    // Add Frame in PlayersGrid.
                    PlayersGrid.SetRow(frame, i);
                    PlayersGrid.SetColumn(frame, j);
                    PlayersGrid.Children.Add(frame);
                }
            }
        }

        //private void AddPlayersOnScreen2()
        //{
        //    // Reversing the iterations will resolve glitches.
        //    for (int i = 18 - 1; i >= 0; i--)
        //    {
        //        (((PlayersGrid.Children[i] as Frame)!.Content 
        //            as VerticalStackLayout)!.Children[0] as Label)!.Text = string.Empty;
        //        (PlayersGrid.Children[i] as Frame)!.IsVisible = false;
        //    }

        //    for (int i = 0; i < App.GameManager.Players.Count; i++)
        //    {
        //        (((PlayersGrid.Children[i] as Frame)!.Content
        //            as VerticalStackLayout)!.Children[0] as Label)!.Text = App.GameManager.Players[i].Name;
        //        (PlayersGrid.Children[i] as Frame)!.IsVisible = true;
        //    }
        //}

        private void UpdateScoreboard()
        {
            Scoreboard.Text = string.Empty;
            foreach (var player in App.GameManager.Players.OrderByDescending(p => p.Points))
                Scoreboard.Text += $"{player.Name}: {player.Points} -- <State: {(player.GameOver == true ? "eliminated" : "playing")}>\n";
        }

        private bool isNumeric(string input)
        {
            // Ignore empty string inputs and consider as 0.
            return string.IsNullOrWhiteSpace(input) || int.TryParse(input, out _);
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
            try
            {
                // Check if all entries are numeric.
                bool noNumericEntries = true;
                foreach(var entry in PlayersEntries)
                {
                    string pointsText = entry.Text;

                    if (isNumeric(pointsText) == false)
                        throw new Exception("Letters shouldn't exist in a number-like score.");

                    if (string.IsNullOrWhiteSpace(pointsText) == false) noNumericEntries = false;
                }

                // Check if all entries are blank.
                if (noNumericEntries) throw new Exception("Empty slots. No points added.");

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
            catch(Exception ex)
            {
                await DisplayAlert("Couldn't update score", ex.Message, "OK");
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
    }
}