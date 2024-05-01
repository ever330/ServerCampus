﻿using System.ComponentModel.DataAnnotations.Schema;

namespace APIServer.Models.DAO
{
    [Table("user_game_data")]
    public class UserGameData
    {
        [Column("id")]
        public string Id { get; set; } = null!;
        [Column("level")]
        public int Level { get; set; }
        [Column("exp")]
        public int Exp { get; set; }
        [Column("winCount")]
        public int WinCount { get; set; }
        [Column("loseCount")]
        public int LoseCount { get; set; }
        [Column("money")]
        public int Money { get; set; }
    }
}
