using System.ComponentModel.DataAnnotations.Schema;

namespace APIServer.Models.DAO
{
    [Table("user_game_data")]
    public class UserGameData
    {
        [Column("email")]
        public string Email { get; set; } = null!;
        [Column("level")]
        public int Level { get; set; }
        [Column("exp")]
        public int Exp { get; set; }
        [Column("win_count")]
        public int WinCount { get; set; }
        [Column("lose_count")]
        public int LoseCount { get; set; }
        [Column("money")]
        public int Money { get; set; }
    }
}
