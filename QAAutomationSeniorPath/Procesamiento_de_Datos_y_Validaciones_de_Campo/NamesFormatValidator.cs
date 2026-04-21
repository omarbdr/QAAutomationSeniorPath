using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAAutomationSeniorPath.Procesamiento_de_Datos_y_Validaciones_de_Campo
{
    public class NamesFormatValidator
    {
        public static void Main() //static was removed to allow instance methods (non-static) to be called without needing to be static themselves)
        {
            List<string> rawNames = new List<string> { "Luis", "Al", "Maria123", "Jose Perez", "Juan", "P" };

            var validateFailures =  ValidateNames(rawNames);

            ResultsReport(validateFailures);
        }

        public static List<string> ValidateNames(List<string> rawnames)
        {
            List<string> failingNames = new List<string>();

            foreach (var name in rawnames)
            {
                if (name.Length < 4 || name.Contains(" ") || name.Any(char.IsDigit))
                {
                    failingNames.Add(name);
                }
              
            }
         return failingNames;
        }

        
        public static void ResultsReport(List<string> failures)
        {
            if (failures.Any())
            {
                Console.WriteLine($"[FAILED] Se encontraron {failures.Count} errores de formato:");
                failures.ForEach(f => Console.WriteLine($"- Nombre inválido: {f}"));
            }
            else
            {
                Console.WriteLine("[PASSED] Todos los nombres son válidos.");
            }
        }
    }
}
