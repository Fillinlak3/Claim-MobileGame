namespace Claim_Macao_MobileGame.Model
{
    public class GameManager
    {
        public List<Player> Players { get; set; }
        public int MaxPoints { get; set; } = 201;

        public GameManager()
        {
            Players = new List<Player>();
        }
    }
}
