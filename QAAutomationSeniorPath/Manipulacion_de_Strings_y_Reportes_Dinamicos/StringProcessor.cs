using System;
using System.Collections.Generic;
using System.Linq;

namespace QAAutomationSeniorPath
{
    internal class StringProcessor
    {
        public static void Main()
        {
            List<string> rawFolios = new List<string>() { "  TRX-9982  ", "trx-5521", "ERROR_001", "  Trx-7744", "INV_221" }; //recibimos datos sucios, con espacios, mayúsculas y minúsculas mezcladas, y algunos que no cumplen la regla de negocio (no empiezan con TRX-)

            // Ejecutamos el proceso
            var report = PurgeAndNormalize(rawFolios);

            // Reporte de Válidos (Reparados y Limpios)
            Console.WriteLine("--- VALID & NORMALIZED FOLIOS ---");
            report.ValidOnes.ForEach(f => Console.WriteLine($"[SUCCESS] {f}"));

            // Reporte de Inválidos
            Console.WriteLine("\n--- INVALID DATA (PURGED) ---");
            report.InvalidOnes.ForEach(f => Console.WriteLine($"[REJECTED] {f}"));
        }

        // Usamos un Tuple para devolver las dos listas (lo bueno y lo malo)
        public static (List<string> ValidOnes, List<string> InvalidOnes) PurgeAndNormalize(List<string> rawList)
        {
            List<string> valids = new List<string>();
            List<string> invalids = new List<string>();

            foreach (var item in rawList)
            {
                // PASO 1: LIMPIEZA INICIAL (Normalización)
                // Usamos null-coalescing para evitar errores si el item fuera null
                // ?? ES UN OPERADOR DE COALESCENCIA NULO, QUE DEVUELVE EL OPERANDO DE LA DERECHA SI EL DE LA IZQUIERDA ES NULL. EN ESTE CASO, SI item ES NULL, SE USARÁ UNA CADENA VACÍA EN SU LUGAR PARA EVITAR ERRORES AL LLAMAR A TRIM() O TOUPPER() SOBRE UN NULL.

                string cleaned = (item ?? "").Trim().ToUpper();
                // PASO 2: Validación de Regla de Negocio
                if (cleaned.StartsWith("TRX-"))
                {
                    valids.Add(cleaned);
                }
                else
                {
                    // Si no empieza con TRX, no nos sirve
                    invalids.Add(item);
                }
            }

            return (valids, invalids);
        }
    }
}