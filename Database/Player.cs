using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace database
{
    [Table("players")]
    public class Player
    {
        public int Id { get; set; }
        public string? Nickname { get; set; }
        [Required]
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public string WinRatio { get; set; }

        public Player()
        {
            GamesPlayed = 0;
            GamesWon = 0;
            WinRatio = "0%";
        }
    }
}
