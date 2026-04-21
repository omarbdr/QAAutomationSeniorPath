using System;
using System.Collections.Generic;
using System.Linq;

public class EmailFormatValidator
{
    public static void Main()
    {
        // BEST PRACTICE 1: Naming Conventions
        // Usar nombres descriptivos. 'rawEmails' indica que son datos sin procesar.
        List<string> rawEmails = new List<string> { "test@test.com", "invalid-email", "qa@company.org", "user@" };

        // BEST PRACTICE 2: Separation of Concerns (Separación de responsabilidades)
        // La lógica de validación se ejecuta y los resultados se analizan después.
        var validationResults = ValidateEmails(rawEmails);

        ReportResults(validationResults);
    }

    // BEST PRACTICE 3: Single Responsibility Principle (SRP)
    // Este método solo se encarga de filtrar, no de imprimir.
    public static List<string> ValidateEmails(List<string> emails)
    {
        List<string> failures = new List<string>();
        foreach (var email in emails)
        {
            // Lógica simple de validación (debería contener '@' y '.')
            if (!email.Contains("@") || !email.Contains("."))
            {
                failures.Add(email);
            }
        }
        return failures;
    }

    public static void ReportResults(List<string> failures)
    {
        if (failures.Any()) // BEST PRACTICE 4: Usar LINQ para legibilidad
        {
            Console.WriteLine($"[FAILED] Se encontraron {failures.Count} errores de formato:");
            failures.ForEach(f => Console.WriteLine($"- Email inválido: {f}")); //what is 'f' here? // 'f' es una variable de iteración que representa cada email inválido en la lista 'failures'.
        }
        else
        {
            Console.WriteLine("[PASSED] Todos los formatos de email son válidos.");
        }
    }
}