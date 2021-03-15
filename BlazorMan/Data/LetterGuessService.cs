using BlazorMan.Shared.Web.MvcExtensions;
using System;
using System.Linq;
using System.Security.Claims;

namespace BlazorMan.Data
{
    public class LetterGuessService
    {
        private readonly BlazorManContext _dbContext;

        public LetterGuessService (BlazorManContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsCorrectLetter(GameSession session, LetterGuess letterGuess) // TODO: This could go GET from an API instead
        {
            var word = _dbContext.Words.FirstOrDefault(w => w.Id == session.WordId).Value;
            var correctLetter = word.ToCharArray()[letterGuess.LetterIndex].ToString();

            if (string.Equals(letterGuess.Value, correctLetter, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }
    }
}
