namespace Claim_Macao_MobileGame.Model
{
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public int Points { get; set; } = 0;
        public bool GameOver { get; set; } = false;

        public Player(string name, int points, bool gameOver)
        {
            Name = name;
            Points = points;
            GameOver = gameOver;
        }
    }
}
