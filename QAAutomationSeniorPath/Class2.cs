using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QAAutomationSeniorPath
{
    internal class Class2
    {


        public class bookRecord
        {

            public string ISBN { get; set; }
            public string title { get; set; }
            public int year { get; set; }
            public decimal rating { get; set; }
            public string Autor { get; set; }
            public bool esValido { get; set; }
            public string error { get; set; }
        }



         static void Main(string[] args)
        {
            List<string> rawData = new List<string> {
                "ISBN-0-0-0-1 | 100 Años de Soledad | 1967 | 4.8 | Gabriel García Márquez",
                "ISBN-0-0-0-2 | El Señor de los Anillos: La Comunidad del Anillo | 1954 | 4.9 | J.R.R. Tolkien",
                "ISBN-0-0-0-3 | El Retrato de Dorian Gray | 1890 | 4.5 | Oscar Wilde",
                "ISBN-0-0-0-4 | Harry Potter y el Prisionero de Azkaban | 1999 | 4.7 | J.K. Rowling",
                "ISBN-0-0-0-5 | El Señor de los Anillos: Las 2 Torres | 1954 | 4.8 | J.R.R. Tolkien",
                "ISBN-0-0-0-5 | El Señor de los Anillos: El Retorno del Rey | 1955 | 4.8 | J.R.R. Tolkien",
                "ISBN-0-0-0-7 | El Señor de los Anillos: El Retorno del Rey | 1955 | 4.8"};

            var results = ValidationRecords.CleanParsingRecords(rawData);
            Console.WriteLine("************REPORT*************");
            
            foreach (var result in results)
            {
                if (result.esValido)
                {
                    Console.WriteLine($"[SUCCESS]: {result.ISBN}");
                }
                else
                {
                    Console.WriteLine($"[ERROR]: The line with the  {result.ISBN} has the following error: {result.error}");
                }
            }
        }



        public class ValidationRecords
        {
            
            public static List<bookRecord> CleanParsingRecords(List<string> rawData)
            {

                var CleanRecords= new List <bookRecord>();
               
                
               var mappedRecords = new HashSet<string>();
 
                    foreach (var item in rawData)
                {
                    bookRecord book = new bookRecord();
                    book.esValido = true;
                    book.error = " ";
                    try
                    {
                        var parts = (item.Split('|').Select(p => p.Trim()).ToArray());

                        //validamos que la linea sea valida, es decir que tenga los 5 campos requeridos, si no es asi se marca como error critico y se asigna el mensaje de error correspondiente
                        if (parts.Length < 5)
                        {
                            book.esValido = false;
                            book.error = "Linea Incompleta o mal formada.";
                        }
                        //una vez que confirmamos que la linea tiene los 5 campos requeridos, procedemos a validar cada campo de acuerdo a las reglas de negocio.
                        else
                        {
                            book.ISBN = parts[0];

                                                    
                            
                            //validamos que el ISBN no este vacio y que comience con "ISBN", si no cumple con esta regla se marca como no valido y se asigna el mensaje de error correspondiente
                            if (!book.ISBN.StartsWith("ISBN") || string.IsNullOrEmpty(book.ISBN))
                            {
                                book.esValido = false;
                                book.error += "El ISBN no es válido. Debe comenzar con 'ISBN' y no puede estar vacío.";

                            }

                            //validamos que el ISBN no se haya repetido en el archivo, si se encuentra un ISBN duplicado se marca como no valido y se asigna el mensaje de error correspondiente
                            if (mappedRecords.Contains(book.ISBN))
                            {
                                book.esValido = false;
                                book.error += $"EL {book.ISBN} IS DUPLICATED";
                            }
                            else
                            {
                                mappedRecords.Add(book.ISBN);
                            }

                            if (string.IsNullOrEmpty(parts[1]))
                            {
                                book.esValido = false;
                                book.error += "El Título no puede estar vacío.";
                            }

                            if (!int.TryParse(parts[2], out int releasingDate) || releasingDate < 1450 || releasingDate > DateTime.Now.Year)

                            {
                                book.esValido = false;
                                book.error = "La fecha es invalida";

                            }

                            if (!decimal.TryParse(parts[3], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rating) || rating < 0 || rating > 5)
                            {
                                book.esValido = false;
                                book.error += "La calificación debe ser un número decimal entre 0.0 y 5.0.";
                            }
                                                      
                        }
                    }

                    catch (Exception ex)
                    {
                        book.esValido = false;
                        book.error = $"Error crítico al procesar el registro: {ex.Message}";
                    }

                    CleanRecords.Add(book); // Agregamos el libro a la lista de registros limpios, independientemente de si es válido o no, para poder reportar todos los errores al final del proceso.
                }
                return CleanRecords;
            }
          
        }
    }
}


/*
 Tus Requerimientos (Business Rules):
ISBN (Identificador): Es el primer campo. No puede estar vacío y debe ser único en todo el archivo.

Título: Segundo campo. Debe tener contenido.

Año de Publicación: Tercer campo. Debe ser un número entero (int). Regla: No aceptamos libros publicados antes del año 1450 (antes de la imprenta de Gutenberg) ni años en el futuro.

Calificación (Rating): Cuarto campo. Debe ser un número decimal (decimal). Regla: Debe estar en el rango de 0.0 a 5.0.

Robustez: Si la línea no tiene los 4 campos o algo falla al procesarla, debe marcarse como error crítico sin detener el programa.
 */