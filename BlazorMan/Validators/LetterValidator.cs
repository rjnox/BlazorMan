using BlazorMan.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorMan.Validators
{
    public class LetterValidator : ComponentBase
    {
        [Inject] protected GameSessionService _gameSessionService { get; set; }
        [Inject] protected LetterGuessService _letterGuessService { get; set; }
        [Inject] protected IHttpContextAccessor _httpContextAccessor { get; set; }

        private ValidationMessageStore _messageStore;

        [CascadingParameter] EditContext CurrentEditContext { get; set; }

        /// <inheritdoc />  

        protected override void OnInitialized()
        {
            _messageStore = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnFieldChanged += EditContext_OnFieldChanged;
        }

        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            _messageStore.Clear(e.FieldIdentifier);
            
            var letter = e.FieldIdentifier.Model as LetterGuess;
            var model = CurrentEditContext.Model as GameModel;

            letter.Value = model.Letters[letter.LetterIndex].Value; // Override cos letter.Value sometimes from old word for some reason

            if (letter == null || string.IsNullOrEmpty(letter.Value))
                return;

            //var correctLetter = model.Word.Value.ToCharArray()[letter.LetterIndex].ToString();
            var session = _gameSessionService.Get(_httpContextAccessor.HttpContext.User);

            if (_letterGuessService.IsCorrectLetter(session, letter)) //(string.Equals(letter.Value, correctLetter, StringComparison.InvariantCultureIgnoreCase))
            {
                _gameSessionService.RecordCorrectGuess(session);

                // Show this letter if it appears multiple times
                var letters = model.Word.Value.ToCharArray();

                for (int i = 0; i < letters.Length; i++)
                {
                    if (i != letter.LetterIndex && string.Equals(letters[i].ToString(), letter.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        model.Letters[i].Value = letter.Value;
                    }
                }

                // Check if word matches
                if (!model.Letters.Any(l => string.IsNullOrEmpty(l.Value)))
                {
                    var guessedWord = string.Join(null, model.Letters.Select(l => l.Value));

                    if (string.Equals(guessedWord, model.Word.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _gameSessionService.WordFound(session);
                        model.GameState = GameState.WordFound;
                    }
                }
            }
            else
            {
                _gameSessionService.RecordWrongGuess(session);
                model.WrongGuesses++;
                model.Letters[letter.LetterIndex].Value = string.Empty;

                if (session.CurrentWordGuesses >= model.GuessesAllowedPerWord)
                {
                    _gameSessionService.GameOver(session, _httpContextAccessor.HttpContext.User);
                    model.GameState = GameState.GameOver;
                }
            }
        }

        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            foreach (var err in errors)
            {
                _messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
