using System.ComponentModel.DataAnnotations;

namespace Littera.Models {
    public class Book {
        public int Id { get; set; }
        [Required(ErrorMessage = "O título do livro é obrigatório.")]
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Cover { get; set; }
        public string? Summary { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public int PageCount { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
