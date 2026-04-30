using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Library.Domain
{
    public class BookValidator
    {
        // Esta es la función que probará tu Pipeline de CI/CD
        public BookRecord ValidateLine(string item, HashSet<string> mappedRecords)
        {
            BookRecord book = new BookRecord();
            book.IsValid = true;
            book.Error = "";

            try
            {
                var parts = item.Split('|').Select(p => p.Trim()).ToArray();

                // Validación de estructura
                if (parts.Length < 5)
                {
                    book.IsValid = false;
                    book.Error = "Linea Incompleta o mal formada.";
                }
                else
                {
                    book.ISBN = parts[0];

                    // Validación de formato ISBN
                    if (string.IsNullOrEmpty(book.ISBN) || !book.ISBN.StartsWith("ISBN"))
                    {
                        book.IsValid = false;
                        book.Error += "El ISBN no es válido. Debe comenzar con 'ISBN' y no puede estar vacío. ";
                    }

                    // Validación de Duplicados
                    if (mappedRecords.Contains(book.ISBN))
                    {
                        book.IsValid = false;
                        book.Error += $"EL {book.ISBN} IS DUPLICATED. ";
                    }
                    else if (book.IsValid) // Solo agregamos al set si el ISBN al menos es válido
                    {
                        mappedRecords.Add(book.ISBN);
                    }

                    // Validación de Título
                    if (string.IsNullOrEmpty(parts[1]))
                    {
                        book.IsValid = false;
                        book.Error += "El Título no puede estar vacío. ";
                    }

                    // Validación de Fecha
                    if (!int.TryParse(parts[2], out int releasingDate) || releasingDate < 1450 || releasingDate > DateTime.Now.Year)
                    {
                        book.IsValid = false;
                        book.Error += "La fecha es invalida. ";
                    }

                    // Validación de Rating
                    if (!decimal.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rating) || rating < 0 || rating > 5)
                    {
                        book.IsValid = false;
                        book.Error += "La calificación debe ser un número decimal entre 0.0 y 5.0. ";
                    }
                }
            }
            catch (Exception ex)
            {
                book.IsValid = false;
                book.Error = $"Error crítico al procesar el registro: {ex.Message}";
            }

            return book;
        }
    }
}