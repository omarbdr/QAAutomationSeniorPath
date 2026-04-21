using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SistemaVentas
{
    // 1. Las opciones de categorías permitidas
    public enum Categoria { Electronica, Ropa, Alimentos }

    // 2. El modelo de datos (lo que representa una venta)
    public class Venta
    {
        public string ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public Categoria Tipo { get; set; }
        public bool EsValido { get; set; }
        public string Error { get; set; }
    }

    class Program
    {
        // EL MAIN: El punto de partida real en el mundo real
        static void Main(string[] args)
        {
            // Simulamos datos que vienen de un archivo .txt
            string[] archivoFalso = {
                "P001 | 2 | 1500.50 | Electronica",
                "P002 | -1 | 20.00 | Ropa",        // Error: Cantidad negativa
                "P003 | 5 | ABC | Alimentos",      // Error: Precio no es número
                "P001 | 1 | 500.00 | Electronica", // Error: Duplicado (P001 ya existe)
                "P004 | 3 | 15.00 | Dulces"        // Error: Categoría no existe
            };

            var procesador = new ProcesadorVentas();//what is this line: Se crea una instancia de la clase ProcesadorVentas, lo que permite acceder a sus métodos y funcionalidades para procesar los datos de ventas.
            var resultados = procesador.ProcesarArchivo(archivoFalso);//Explica esta linea: Se crea una instancia de la clase ProcesadorVentas y se llama al método ProcesarArchivo, pasando el arreglo de líneas simulado como argumento.
                                                                      //El resultado es una lista de objetos Venta que contienen información sobre cada línea procesada, incluyendo si fue válida o si hubo errores.

            Console.WriteLine("--- REPORTE DE PROCESAMIENTO ---");
            foreach (var v in resultados)
            {
                if (v.EsValido) //DOES THE SYSTEM ASSUME THAT IF EsValido IS TRUE? WHY? Yes, the system assumes that if EsValido is true, then the data for that sale is correct and can be processed without issues.
                                //This is because EsValido is set to true by default when a Venta object is created, and it only changes to false if any validation checks fail during the processing of the line.
                                //Therefore, if EsValido remains true after all validations, it indicates that the sale data is valid and can be used to calculate the total price and display a success message.
                    Console.WriteLine($"[EXITO] ID: {v.ProductoId}, Total: ${v.Precio * v.Cantidad}");
                else
                    Console.WriteLine($"[ERROR] {v.Error}");
            }
        }
    }

    public class ProcesadorVentas
    {
        public List<Venta> ProcesarArchivo(string[] lineas) //Se utiliza el tipo de dato Venta para la lista de resultados porque cada línea del archivo
                                                            //representa una venta con múltiples atributos (ProductoId, Cantidad, Precio, Tipo, EsValido, Error).
                                                            //Al usar una lista de objetos Venta, se puede almacenar toda la información relevante de cada línea procesada en un solo objeto, lo que facilita el manejo y la presentación de los resultados
                                                            //Cada objeto Venta puede contener tanto los datos válidos como los errores encontrados durante el procesamiento, lo que permite un reporte detallado al final.
        {
            var resultados = new List<Venta>(); //Esta variable es independiente de la variable resultado en el método Main porque se encuentra dentro del ámbito de la clase ProcesadorVentas, mientras que la variable resultado en el método Main es local a ese método. 
                                                //Esto significa que cada método tiene su propia instancia de la variable resultados, y no hay conflicto entre ellas. 
                                                //El método ProcesarArchivo devuelve una lista de objetos Venta, que luego es utilizada por el método Main para mostrar los resultados al usuario.
            var productosLeidos = new HashSet<string>();

            foreach (var linea in lineas)
            {
                var venta = new Venta { EsValido = true };//Es buena práctica asumir que la venta es válida al crear el objeto porque permite un enfoque optimista en el procesamiento de datos.
                                                          //Al establecer EsValido en true inicialmente, se puede proceder con las validaciones y solo cambiarlo a false si se encuentra un error específico.
                                                          //Esto simplifica la lógica de validación, ya que no es necesario manejar un estado "indefinido" o "no procesado". Además, mejora la legibilidad del código, ya que se puede ver claramente que el estado predeterminado es válido y cualquier error se marca explícitamente durante el proceso de validación.

                try
                {
                    // PASO 1: Separar y limpiar
                    var parts = linea.Split('|').Select(p => p.Trim()).ToArray();
                    venta.ProductoId = parts[0];  //Es buena práctica definir el valor de las propiedades del objeto venta dentro del bloque try porque permite manejar cualquier excepción que pueda ocurrir durante el proceso de asignación de valores.

                    // PASO 2: Validar Cantidad (Entero y > 0)
                    if (!int.TryParse(parts[1], out int cant) || cant<=0)
                        {
                        venta.EsValido = false;
                        venta.Error = "Cantidad debe ser un entero positivo";
                        }
                    else
                        {
                        venta.Cantidad = cant;
                        }

                    // PASO 3: Validar Precio (Decimal y > 0)
                    if (!decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
                    
                        {
                        venta.EsValido = false;
                        venta.Error = "Precio debe ser un número decimal positivo";
                        }

                    else
                        {
                        venta.Precio = price;
                        }




                    // PASO 4: Validar Categoría (Enum)
                    if (!Enum.TryParse(parts[3], out Categoria Category))
                    {   
                        venta.EsValido = false;
                        venta.Error = "Categoría inválida";
                    }
                    else
                    {
                        venta.Tipo = Category;
                    }

                        // PASO 5: Validar Duplicados
                        if (productosLeidos.Contains(venta.ProductoId))
                    {
                        venta.EsValido = false;
                        venta.Error = "Es un duplicado";
                        /* TU TURNO DE ESCRIBIR EL PASO 5 */
                    }
                    else
                    {
                        productosLeidos.Add(venta.ProductoId);
                    }
                }
                catch
                {
                    venta.EsValido = false;
                    venta.Error = "Línea con formato roto o incompleta";
                }

                resultados.Add(venta);
            }
            return resultados;
        }
    }
}