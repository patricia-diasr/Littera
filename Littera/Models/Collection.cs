using System.ComponentModel.DataAnnotations;

namespace Littera.Models {
    public class Collection {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome da coleção é obrigatório.")]
        public string Name { get; set; }
        public string Color { get; set; }
        public int Priority { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
