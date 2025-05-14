namespace Littera.Models {
    public class User {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        protected string Password { get; set; }
        public string Photo { get; set; }
        public List<Book> Books { get; set; }
        public List<Collection> Collections { get; set; }
        public List<Tag> Tags { get; set; }
    }

}
