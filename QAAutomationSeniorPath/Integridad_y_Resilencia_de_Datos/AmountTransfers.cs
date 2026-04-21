using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAAutomationSeniorPath.Integridad_y_Resilencia_de_Datos
{
    internal class AmountTransfers
    {
        static List<int> validAmounts = new List<int>();
        static List<string> invalidAmounts = new List<string>();

        public static void Main()
        {
            
            List<string> transfers = new List<string>() { "500", "1000", "Trescientos", "2147483648", "50", null };
            ProcessingTransfers(transfers);
            GenerateReport();


        }

        public static void ProcessingTransfers(List<string> transfers)
        {

            foreach (string transfer in transfers)
            {
                try
                {
                    int validTransfer = int.Parse(transfer);
                    Console.WriteLine($"The amount of the Transfer {transfer} is valid");
                    validAmounts.Add(validTransfer);




                }
                catch (FormatException)
                {
                    Console.WriteLine($"INVALID FORMAT for the amount {transfer}");
                    invalidAmounts.Add(transfer);

                }
                catch (OverflowException)
                {
                    Console.WriteLine($" VALUE IS TOO BIG to be int");
                    invalidAmounts.Add(transfer);

                }

                catch (ArgumentNullException)
                {
                    Console.WriteLine($"NULL VALUE is not supported");
                    invalidAmounts.Add(transfer);

                }

                catch (Exception ex)
                {
                    Console.WriteLine($"[Critical Alert] Unexpected error: {ex.Message}");
                    invalidAmounts.Add(transfer);

                }
            }


        }

        public static void GenerateReport()
        {
            Console.WriteLine("\n--- Reporte de Transferencias ---");
            Console.WriteLine($"Total de transferencias válidas: {validAmounts.Count}");
            Console.WriteLine($"Total de transferencias inválidas: {invalidAmounts.Count}");
            Console.WriteLine("Montos válidos:");
            validAmounts.ForEach(amount => Console.WriteLine($"- {amount}"));
            Console.WriteLine("Montos inválidos:");
            invalidAmounts.ForEach(amount => Console.WriteLine($"- {amount}"));
        }
    }
}
    