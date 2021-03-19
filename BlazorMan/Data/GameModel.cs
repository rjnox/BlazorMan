using System;
using System.Collections.Generic;

namespace BlazorMan.Data
{
    public class GameModel
    {
        private Word _word;

        public GameState GameState { get; set; }
        public GameSession GameSession { get; set; }
        public Word Word
        {
            get => _word;
            set
            {
                _word = _word ?? new Word();

                Letters = new List<LetterGuess>();

                for (int l = 0; l < value.Value.Length; l++)
                {
                    Letters.Add(new LetterGuess { LetterIndex = l, Value = string.Empty });
                }

                _word = value;
            }
        }
        public List<LetterGuess> Letters { get; set; }
        public long TimeElapsed { get; set; }
        public long TimeAllowed => TimeSpan.FromMinutes(3).Ticks;
        public int GuessesAllowedPerWord => 11;
        public int WrongGuesses { get; set; }
    }

    public enum GameState 
    {
        Idle,
        Playing,
        WordFound,
        GameOver
    }
}
