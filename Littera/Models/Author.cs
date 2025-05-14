namespace Littera.Models {

    public class Author {
        public int Id { get; set; }
        public string Name { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
