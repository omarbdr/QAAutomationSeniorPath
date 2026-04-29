using System;
using Library.Domain;

namespace Library.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // NOTA: En un proyecto real, aquí instanciarías tu BD real (SQL Server/SQLite)
            // Por ahora, imagina que 'miBaseDeDatos' ya existe.
            ILibraryDbContext miBaseDeDatos = null; // Mock o DB real

            var servicio = new BibliotecaService(miBaseDeDatos);

            Console.WriteLine("--- Sistema de Préstamos ---");

            try
            {
                // Simulamos que el usuario elige el libro 1 y el usuario 5
                servicio.PrestarLibro(1, 5);
                Console.WriteLine("¡Préstamo exitoso!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error en la operación: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}