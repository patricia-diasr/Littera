namespace Littera.Models {
    public class BookCollection {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
