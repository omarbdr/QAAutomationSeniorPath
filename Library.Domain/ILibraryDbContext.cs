
using System.Data.Entity; // Necesitarás el paquete NuGet de EF Core

namespace Library.Domain
{
    public interface ILibraryDbContext
    {
        DbSet<BookRecord> Libros { get; set; }
        DbSet<Usuario> Usuarios { get; set; }

        void BeginTransaction();
        void SaveChanges();
        void Commit();
        void Rollback();
    }
}