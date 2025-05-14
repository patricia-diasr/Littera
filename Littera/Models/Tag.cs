namespace Littera.Models {
    public class Tag {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
