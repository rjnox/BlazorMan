using BlazorMan.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorMan
{
    public class PopulateDatabaseHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _environment;

        public PopulateDatabaseHostedService(IServiceProvider serviceProvider, IWebHostEnvironment environment)
        {
            _serviceProvider = serviceProvider;
            _environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return PopulateDatabaseAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task PopulateDatabaseAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BlazorManContext>();

                await AddWords(dbContext);
            }
        }

        private async Task AddWords(BlazorManContext dbContext)
        {
            if (!(await dbContext.Words.AnyAsync()))
            {
                Console.WriteLine("Populating words table ...");

                var words = new List<Word>();
                var filePath = Path.Combine(_environment.WebRootPath, "words.txt");

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string line;

                        while ((line = (await sr.ReadLineAsync())) != null)
                        {
                            var trimmed = line.Trim();

                            if (trimmed.Length > 3 && trimmed.Length < 9) // Avoid too small and big words 
                                words.Add(new Word { Value = trimmed });
                        }
                    }
                }

                dbContext.Words.AddRange(words);
                words.Clear();
                dbContext.SaveChanges();
            }
        }
    }
}
