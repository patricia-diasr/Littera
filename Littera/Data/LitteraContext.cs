using Microsoft.EntityFrameworkCore;
using Littera.Models;

namespace Littera.Data {
    public class LitteraContext : DbContext {
        public LitteraContext(DbContextOptions<LitteraContext> options)
            : base(options) { }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCollection> BookCollections { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; } 
    }
}
