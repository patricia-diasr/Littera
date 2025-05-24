using System.ComponentModel.DataAnnotations;

namespace Littera.Models {

    public class Author {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome do autor é obrigatório.")]
        public string Name { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
