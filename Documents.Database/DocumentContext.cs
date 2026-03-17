using Documents.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Documents.Database;

public class DocumentContext : DbContext
{
    public DocumentContext(DbContextOptions<DocumentContext> optionsBuilder)
        : base(optionsBuilder)
    { }

    public DbSet<Document> Documents { get; set; }

    public DbSet<DocumentPrediction> DocumentPredictions { get; set; }

}
