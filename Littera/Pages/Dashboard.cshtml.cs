using Littera.Data;
using Littera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Littera.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly LitteraContext _context;

        public int TotalBooks { get; set; }
        public int BooksRead { get; set; }
        public int BooksReading { get; set; }
        public int BooksToRead { get; set; }
        public int TotalPages { get; set; }
        public double AverageRating { get; set; }
        public string BooksPerMonthJson { get; set; }
        public string BooksByStatusJson { get; set; }
        public string BooksByRatingJson { get; set; }
        public string TopAuthorsJson { get; set; }
        public string ReadingProgressJson { get; set; }
        public string TagDistributionJson { get; set; }

        public DashboardModel(LitteraContext context) {
            _context = context;
        }

        public async Task OnGetAsync() {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(claim.Value);

            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            TotalBooks = books.Count;
            BooksRead = books.Count(b => b.Status == "Lido");
            BooksReading = books.Count(b => b.Status == "Lendo");
            BooksToRead = books.Count(b => b.Status == "Quero Ler");
            TotalPages = books.Sum(b => b.PageCount);
            AverageRating = books.Where(b => b.Rating.HasValue).Average(b => b.Rating.Value);

            var booksPerMonth = books
                .Where(b => b.EndDate.HasValue && b.Status == "Lido")
                .GroupBy(b => new { b.EndDate.Value.Year, b.EndDate.Value.Month })
                .Select(g => new {
                    Month = $"{g.Key.Year}-{g.Key.Month:00}",
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            BooksPerMonthJson = JsonSerializer.Serialize(new {
                labels = booksPerMonth.Select(x => x.Month).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Livros Lidos",
                        data = booksPerMonth.Select(x => x.Count).ToArray(),
                        backgroundColor = "rgba(54, 162, 235, 0.6)",
                        borderColor = "rgba(54, 162, 235, 1)",
                        borderWidth = 1
                    }
                }
            });

            var booksByStatus = new[]
            {
                new { Status = "Lido", Count = BooksRead },
                new { Status = "Lendo", Count = BooksReading },
                new { Status = "Quero Ler", Count = BooksToRead }
            };

            BooksByStatusJson = JsonSerializer.Serialize(new {
                labels = booksByStatus.Select(x => x.Status).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        data = booksByStatus.Select(x => x.Count).ToArray(),
                        backgroundColor = new[] { "#36A2EB", "#FF6384", "#FFCE56" }
                    }
                }
            });

            var ratingDistribution = books
                .Where(b => b.Rating.HasValue)
                .GroupBy(b => b.Rating.Value)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .OrderBy(x => x.Rating)
                .ToList();

            BooksByRatingJson = JsonSerializer.Serialize(new {
                labels = ratingDistribution.Select(x => $"{x.Rating} estrelas").ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Quantidade de Livros",
                        data = ratingDistribution.Select(x => x.Count).ToArray(),
                        backgroundColor = "rgba(255, 99, 132, 0.6)",
                        borderColor = "rgba(255, 99, 132, 1)",
                        borderWidth = 1
                    }
                }
            });

            var topAuthors = books
                .GroupBy(b => b.Author.Name)
                .Select(g => new { Author = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            TopAuthorsJson = JsonSerializer.Serialize(new {
                labels = topAuthors.Select(x => x.Author).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Número de Livros",
                        data = topAuthors.Select(x => x.Count).ToArray(),
                        backgroundColor = "rgba(75, 192, 192, 0.6)",
                        borderColor = "rgba(75, 192, 192, 1)",
                        borderWidth = 1
                    }
                }
            });

            var readingProgress = books
                .Where(b => b.EndDate.HasValue && b.Status == "Lido")
                .GroupBy(b => new { b.EndDate.Value.Year, b.EndDate.Value.Month })
                .Select(g => new {
                    Month = $"{g.Key.Year}-{g.Key.Month:00}",
                    Pages = g.Sum(b => b.PageCount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            ReadingProgressJson = JsonSerializer.Serialize(new {
                labels = readingProgress.Select(x => x.Month).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Páginas Lidas",
                        data = readingProgress.Select(x => x.Pages).ToArray(),
                        backgroundColor = "rgba(153, 102, 255, 0.6)",
                        borderColor = "rgba(153, 102, 255, 1)",
                        borderWidth = 1,
                        fill = false
                    }
                }
            });

            var bookTags = await _context.BookTags
                .Include(bt => bt.Tag)
                .Include(bt => bt.Book)
                .Where(bt => bt.Book.UserId == userId)
                .ToListAsync();

            var tagDistribution = bookTags
                .GroupBy(bt => bt.Tag.Name)
                .Select(g => new { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            TagDistributionJson = JsonSerializer.Serialize(new {
                labels = tagDistribution.Select(x => x.Tag).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        data = tagDistribution.Select(x => x.Count).ToArray(),
                        backgroundColor = new[]
                        {
                            "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0",
                            "#9966FF", "#FF9F40", "#FF6384", "#C9CBCF",
                            "#4BC0C0", "#FF6384"
                        }
                    }
                }
            });
        }
    }
}
