using BlazorMan.Shared.Web.MvcExtensions;
using System;
using System.Linq;
using System.Security.Claims;

namespace BlazorMan.Data
{
    public class GameSessionService
    {
        private readonly BlazorManContext _dbContext;

        public GameSessionService (BlazorManContext dbContext)
        {
            _dbContext = dbContext;
        }

        public GameSession Get(ClaimsPrincipal user)
        {
            var session = _dbContext.GameSessions.FirstOrDefault(x => x.UserId == user.GetUserId());

            if (session != null)
                return session;
            else
                return Create(user);

            throw new Exception("Couldn't get a game session");
        }

        private GameSession Get(int sessionId) => _dbContext.GameSessions.FirstOrDefault(x => x.SessionId == sessionId);

        public void RecordCorrectGuess(GameSession session)
        {
            var dbSession = Get(session.SessionId);
            dbSession.CurrentWordGuesses++;
            dbSession.TotalGuesses++;
            _dbContext.SaveChanges();
            SyncClientSession(session, dbSession);
        }

        public void RecordWrongGuess(GameSession session)
        {
            var dbSession = Get(session.SessionId);
            dbSession.CurrentWordGuesses++;
            dbSession.TotalGuesses++;
            _dbContext.SaveChanges();
            SyncClientSession(session, dbSession);
        }

        public void WordFound(GameSession session)
        {
            session.WordCount++;
            _dbContext.SaveChanges();
        }

        public Word ChangeWord(GameSession session)
        {
            var dbSession = Get(session.SessionId);
            dbSession.WordId = GetRandomWord().Id;
            session.CurrentWordGuesses = 0;
            _dbContext.SaveChanges();
            SyncClientSession(session, dbSession);

            return _dbContext.Words.FirstOrDefault(w => w.Id == dbSession.WordId);
        }

        public void GameOver(GameSession session, ClaimsPrincipal user)
        {
            var dbSession = Get(session.SessionId);
            dbSession.Active = false;

            _dbContext.Scores.Add(new Score
            {
                Date = DateTime.Now,
                Guesses = session.TotalGuesses,
                WordCount = session.WordCount,
                UserId = session.UserId,
                UserName = user.Identity.Name,
                TimeElapsed = DateTime.Now.Subtract(session.StartTime)
            });

            _dbContext.SaveChanges();
            SyncClientSession(session, dbSession);
        }

        public Word NewGame(GameSession session)
        {
            var dbSession = Get(session.SessionId);
            dbSession.Active = true;
            dbSession.StartTime = DateTime.Now;
            dbSession.WordId = GetRandomWord().Id;
            dbSession.CurrentWordGuesses = 0;
            dbSession.TotalGuesses = 0;
            dbSession.WordCount = 0;
            _dbContext.SaveChanges();
            SyncClientSession(session, dbSession);

            return _dbContext.Words.FirstOrDefault(w => w.Id == dbSession.WordId);
        }

        private GameSession Create(ClaimsPrincipal user)
        {
            var freeSession = _dbContext.GameSessions.FirstOrDefault(x => x.Active == false);
            var randomWord = GetRandomWord();

            if (freeSession == null)
            {
                var session = _dbContext.GameSessions.Add(new GameSession
                {
                    Active = true,
                    StartTime = DateTime.Now,
                    UserId = user.GetUserId(),
                    WordId = randomWord.Id
                });

                return session.Entity;
            }
            else
            {
                ReuseSession(freeSession, user.GetUserId(), randomWord.Id);
                return freeSession;
            }

            throw new Exception("Couldn't create a game session");
        }

        private Word GetRandomWord() => _dbContext.Words.OrderBy(x => Guid.NewGuid()).Take(1).FirstOrDefault();

        private void ReuseSession(GameSession session, string userId, int wordId)
        {
            session.UserId = userId;
            session.WordId = wordId;
            session.Active = true;
            session.StartTime = DateTime.Now;
            _dbContext.SaveChanges();
        }

        private void SyncClientSession(GameSession clientSession, GameSession serverSession)
        {
            clientSession = serverSession;
        }
    }
}
