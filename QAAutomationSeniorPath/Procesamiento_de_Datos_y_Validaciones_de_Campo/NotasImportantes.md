**Estructuras de Datos (Colecciones)**

List vs Array: En Automation usamos List<T> porque los datos de las apps web son dinámicos (no sabemos cuántos elementos habrá en una tabla). El Array es estático y rígido.

LINQ (.Any(), .Count(), .Where()): Es una herramienta poderosa en C# para consultar y filtrar colecciones rápidamente sin escribir bucles foreach complejos.



**Best Practices de Programación (Clean Code)**

Naming Conventions: * PascalCase: Para nombres de Clases y Métodos (ValidateUser).

camelCase: Para variables locales (rawNames).

Descriptivo: No uses x o list1. Usa failingUsers o priceList.

SRP (Single Responsibility Principle): Un método debe hacer una sola cosa. El que valida no debe imprimir; el que imprime no debe calcular. Esto hace que tus tests sean modulares y reutilizables.

Separation of Concerns: Mantén tus datos de prueba (rawNames) separados de tu lógica de validación.



**Conceptos de C# (Arquitectura)**
Static vs Instance:

static: El método pertenece a la Clase. Se puede llamar sin crear un objeto. Ideal para "Utilities" o "Helpers" de testing.

instance (no static): El método pertenece al Objeto. Requiere hacer un new MiClase(). Se usa para Page Objects donde cada página es un objeto distinto.

Main Method: Es el punto de entrada (Entry Point). Siempre debe ser static void Main.


**Mentalidad de QA (Edge Cases)**
Validación de Negocio: Siempre cuestiona los límites. Si la regla dice "mínimo 4", el código debe ser length < 4.

Off-by-one errors: Errores comunes donde el código falla por un solo número (usar < en lugar de <=).

Data Integrity: Validar no solo lo obvio (longitud), sino también caracteres prohibidos (números en nombres) o espacios en blanco accidentales.