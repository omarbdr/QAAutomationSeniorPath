using System;
using System.Collections.Generic;

namespace MyAutomationProject.Core.Resilience
{
    public class SafeParsing
    {
        public static void Main()
        {
            // Datos simulados: Algunos son números, otros son texto basura
            List<string> rawData = new List<string> { "100", "250", "DATO_CORRUPTO", "500", null, "2, 147, 483, 648" };
            ProcessData(rawData);
        }

        public static void ProcessData(List<string> dataList)
        {
            foreach (var item in dataList)
            {
                try
                {
                    // Intentamos convertir el string a entero
                    int value = int.Parse(item);
                    Console.WriteLine($"[LOG] Procesado con éxito: {value}");
                }
                catch (FormatException) //FormatException comes from System namespace, it is thrown when the format of an argument is invalid.
                {
                    // Este bloque atrapa específicamente cuando el texto no es un número (ej. "DATO_CORRUPTO")
                    Console.WriteLine($"[ERROR TÉCNICO] '{item}' no tiene un formato numérico válido. Saltando...");
                }
                catch (ArgumentNullException)
                {
                    // Este bloque atrapa si el dato es nulo (null)
                    Console.WriteLine("[ERROR TÉCNICO] Se detectó un valor nulo. No se puede procesar.");
                }
                catch (Exception ex) 
                {
                    // Este es el "paracaídas" final para cualquier otro error no previsto
                    Console.WriteLine($"[ALERTA CRÍTICA] Error inesperado: {ex.Message}");
                }
            }
        }
    }
}