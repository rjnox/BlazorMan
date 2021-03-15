using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMan.Data
{
    public class ScoreBoardService
    {
        private readonly BlazorManContext _dbContext;

        public ScoreBoardService(BlazorManContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static readonly string[] Names = new[]
        {
            "Jeff", "John", "Jack", "Jill", "Jake", "Jude", "Jim", "Jenny"
        };

        public async Task<Score[]> GetScoresAsync()
        {
            if ((await _dbContext.Scores.AnyAsync()))
            {
                return await _dbContext.Scores.OrderByDescending(s => s.WordCount)
                    .ThenBy(s => s.Guesses).ThenBy(s => s.TimeElapsed).Take(50).ToArrayAsync();
            }
            else
                return await GetMockScoresAsync();
        }

        public Task<Score[]> GetMockScoresAsync()
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(0, Names.Length-1).Select(index => new Score
            {
                Date = DateTime.Now.AddDays(index),
                UserId = index.ToString(),
                UserName = Names[index],
                TimeElapsed = new TimeSpan(0, 0, rng.Next(0, 60)),
                Guesses = rng.Next(0, 512),
                WordCount = rng.Next(0, 8)
            }).OrderByDescending(s => s.WordCount).ThenBy(s => s.Guesses).ThenBy(s => s.TimeElapsed).ToArray());
        }
    }
}
