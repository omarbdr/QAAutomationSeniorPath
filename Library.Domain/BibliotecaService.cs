using System;

namespace Library.Domain
{
    public class BibliotecaService
    {
        private readonly ILibraryDbContext _db;

        // Inyección de dependencias: alimentamos al servicio con la BD
        public BibliotecaService(ILibraryDbContext db)
        {
            _db = db;
        }

        public void PrestarLibro(int libroId, int usuarioId)
        {
            try
            {
                _db.BeginTransaction();

                var libro = _db.Libros.Find(libroId);
                var usuario = _db.Usuarios.Find(usuarioId);

                // GATEKEEPERS (Validaciones rápidas)
                if (libro == null) throw new Exception("El libro no existe.");
                if (usuario == null) throw new Exception("El usuario no existe.");

                // NUESTRA LÓGICA DE MULTAS
                if (usuario.HasPendingFines)
                {
                    throw new Exception($"El usuario {usuario.Name} tiene multas.");
                }

                if (libro.OnLoan) throw new Exception("El libro ya está prestado.");

                // Si pasó las validaciones, actualizamos
                libro.OnLoan = true;

                _db.SaveChanges();
                _db.Commit();
            }
            catch (Exception)
            {
                _db.Rollback();
                throw; // Re-lanzamos para que la ConsoleApp sepa que falló
            }
        }
    }
}