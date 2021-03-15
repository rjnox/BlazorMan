using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorMan.Data
{
    public class GameSession
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; } // Tried long but EF didnt like it
        public string UserId { get; set; }
        public int WordId { get; set; }
        public int CurrentWordGuesses { get; set; }
        public int TotalGuesses { get; set; }
        public int WordCount { get; set; }
        public bool Active { get; set; }
        public DateTime StartTime { get; set; }
    }
}
