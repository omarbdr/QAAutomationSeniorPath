using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PaymentAutomation
{
   // Módulo /1: Tipado Fuerte (Best Practice)
    public enum PaymentStatus { Approved, Pending, Declined, Unknown } //an enum is a value type that defines a set of named constants, making the code more readable and maintainable. It is like a dropdown list of predefined values that a variable can take, improving code clarity and reducing errors from using magic strings or numbers.

    public class PaymentRecord
    {
        //Definimos cómo se verá un "Pago" una vez que lo hayamos limpiado.
        //This class will be instantiated for each line of input, and will hold the cleaned and validated data, along with any errors encountered during processing.
        //It serves as a structured way to represent the payment data throughout the reconciliation process.

        public string TransactionId { get; set; } 
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } //here we are saying there is a field named Status that can only take one of the values defined in the PaymentStatus enum (Approved, Pending, Declined, Unknown).
                                                  //This makes our code more robust and easier to understand, as we are using meaningful names instead of arbitrary strings or numbers to represent the payment status.
        public string Source { get; set; }
        public bool IsValid { get; set; }
        public string ValidationError { get; set; }

        //properties vs variables: properties provide a way to control access to the data, allowing for validation, encapsulation, and the ability to change the internal implementation without affecting external code.
        //Variables are just storage locations without any logic or control over how they are accessed or modified.
    }

    public class ReconciliationPro
    {
        public static void Main()
        {
            // Lista dinámica (List<T>) en lugar de array estático
            List<string> rawData = new List<string>
            {
                "ch_001 | 1250.50 | Approved",
                "PAYID-99 | 45.00 | Pending",
                "ch_001 | 1250.50 | Approved", // Duplicado detectado por lógica
                "TRANSFER_01 | 3000 | ",       // Corrupto (Sin estado)
                "invalid_id | -50.00 | Success", // Monto negativo
                "  ch_005 | 200.00 | Approved  " // Requiere limpieza de espacios
            };

            ExecuteReconciliation(rawData); //calling the execute reconciliation method with our list of raw data,
                                            //which will process the data, perform validations,
                                            //and generate a report based on the cleaned and validated payment records.
        }

        public static void ExecuteReconciliation(List<string> inputs) //checar si lo que vendimos, pagamos coincide con los datos del banco, o con los datos que nos dio el cliente, etc.
                                                                      //Es un proceso de validación y limpieza de datos para asegurarnos de que todo esté correcto antes de generar un reporte final.
        {
            List<PaymentRecord> results = new List<PaymentRecord>(); //I am creating a collection (a List) that is only allowed to hold PaymentRecord objects."
                                                                     //This list will be used to store the results of processing each line of input,
                                                                     //including both valid and invalid payment records, along with any validation errors encountered
                                                                     //during the reconciliation process.

            HashSet<string> uniqueIds = new HashSet<string>(); //the hashset is used to store unique transaction IDs, allowing us to efficiently check for duplicates during the reconciliation process.
                                                               //It provides O(1) average time complexity for lookups, making it ideal for this purpose.
            
            int cleanUpInterventions = 0;// this is a counter to track how many times we had to intervene to clean up the data (e.g., removing extra spaces).
                                         // It helps us understand the quality of the input data and the amount of manual effort required to process it.

            foreach (var line in inputs)
            {
                var record = ParseAndValidate(line, ref cleanUpInterventions); //this method parses and validates each line of input, returning a PaymentRecord object with the cleaned and validated data.
                                                                               //ref represents that the cleanUpInterventions variable is passed by reference, allowing the method to modify its value and keep track of how many times data cleanup was necessary during the parsing process.
                                                                               //¿Qué es ref? Normalmente, cuando pasas una variable a un método, el método recibe una copia. Si el método cambia el valor, tu variable original no se entera.
                                                                               //Al usar ref, le estás dando la llave original de la variable. Si el método ParseAndValidate suma 1 al contador, se suma en tu variable principal.
                                                                               //why do we use var instead of class? Using var allows the compiler to infer the type of the variable based on the right-hand side of the assignment.
                                                                               //In this case, since ParseAndValidate returns a PaymentRecord, the compiler knows that record is of type PaymentRecord without us having to explicitly declare it.
                                                                               //This can make the code cleaner and reduce redundancy, especially when the type is obvious from the context.

                // Módulo 2: Resiliencia e Integridad (Deduplicación)
                if (record.IsValid)
                {
                    if (uniqueIds.Contains(record.TransactionId))//what about this line? This line checks if the transaction ID of the current record already exists in the uniqueIds HashSet. If it does, it means we have a duplicate transaction, and we mark the record as invalid and set the validation error to "Duplicado". If it doesn't exist, we add the transaction ID to the uniqueIds HashSet to keep track of it for future checks.
                    {
                        record.IsValid = false;
                        record.ValidationError = "Duplicado";
                    }
                    else
                    {
                        uniqueIds.Add(record.TransactionId);
                    }
                }
                results.Add(record);
            }

            PrintExecutiveReport(results, cleanUpInterventions);
        }

        private static PaymentRecord ParseAndValidate(string line, ref int cleanUpCount)  //Este método se encarga de impiar el archivo, limpiar los datos, validar cada campo y mapear el estado a un enum. 
                                                                                          //Si encuentra algún error durante el proceso, marca el registro como inválido y almacena el mensaje de error correspondiente.
        {
            var record = new PaymentRecord { IsValid = true, Status = PaymentStatus.Unknown }; 

            try
            {
                // Módulo 3: Manipulación de Strings (Estandarización)
                if (line.Contains("  ")) cleanUpCount++; // Detectamos si hubo que limpiar espacios
                var parts = line.Split('|').Select(p => p.Trim()).ToArray();

                if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[2])) // the index 2 corresponds to the Status field, if it's missing or empty, we consider the record invalid due to incomplete data.
                {
                    record.IsValid = false;
                    record.ValidationError = "Datos Incompletos";
                    return record;
                }

                // Asignación y limpieza de ID
                record.TransactionId = parts[0];

                // Determinar Origen (Stripe vs PayPal)
                record.Source = record.TransactionId.StartsWith("ch_") ? "Stripe" : //the ? and : in human words is like saying "if the transaction ID starts with 'ch_', then the source is 'Stripe',
                                                                                    //otherwise, if it starts with 'PAYID-', it's 'PayPal', and if it doesn't match either pattern, we can classify it as 'Otros' (Others).
                                record.TransactionId.StartsWith("PAYID-") ? "PayPal" : "Otros";

                // Módulo 1: Procesamiento de datos y validaciones de campo
                if (!decimal.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
                {
                    record.IsValid = false;
                    record.ValidationError = "Monto Inválido";
                }
                record.Amount = price;

                // Mapeo de Enum
                if (Enum.TryParse(parts[2], true, out PaymentStatus statusResult))
                    record.Status = statusResult;
                else
                {
                    record.IsValid = false;
                    record.ValidationError = "Estado Desconocido";
                }
            }
            catch
            {
                record.IsValid = false;
                record.ValidationError = "Error Crítico de Formato";
            }

            return record;
        }

        private static void PrintExecutiveReport(List<PaymentRecord> data, int cleanUps)
        {
            // Módulo 3: Reporte Dinámico con LINQ (Lo que vería un Manager)
            var validOnes = data.Where(d => d.IsValid).ToList();

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("     FINANCIAL RECONCILIATION REPORT (SDET)");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Transacciones Totales : {data.Count}");
            Console.WriteLine($"Exitosas              : {validOnes.Count}");
            Console.WriteLine($"Rechazadas/Errores    : {data.Count(d => !d.IsValid)}");
            Console.WriteLine($"Limpiezas de Datos    : {cleanUps}");

            // Agrupación por Pasarela usando LINQ
            var stripeTotal = validOnes.Where(v => v.Source == "Stripe").Sum(v => v.Amount);
            var paypalTotal = validOnes.Where(v => v.Source == "PayPal").Sum(v => v.Amount);

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"TOTAL STRIPE  : {stripeTotal.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            Console.WriteLine($"TOTAL PAYPAL  : {paypalTotal.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"TOTAL CONCILIADO : {(stripeTotal + paypalTotal).ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
            Console.WriteLine("-------------------------------------------------");
        }
    }
}