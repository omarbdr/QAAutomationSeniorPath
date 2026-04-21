using NUnit.Framework;
using Library.Domain; // Asegúrate que coincida con tu namespace
using System.Collections.Generic;

namespace Library.Tests
{
    [TestFixture]
    public class BookValidatorTests
    {
        private BookValidator _validator;
        private HashSet<string> _mappedRecords;

        [SetUp]
        public void Setup()
        {
            _validator = new BookValidator();
            _mappedRecords = new HashSet<string>();
        }

        [Test]
        public void ValidateLine_ShouldReturnError_WhenLineIsMalformed()
        {
            // Arrange (Preparamos los datos basura)
            string badLine = "ISBN-001 | Solo Titulo | 2024"; // Faltan campos

            // Act (Ejecutamos la lógica)
            var result = _validator.ValidateLine(badLine, _mappedRecords);

            // Assert (Verificamos que el "Quality Gate" lo detenga)
            Assert.That(result.esValido, Is.False);
            Assert.That(result.error, Is.EqualTo("Linea Incompleta o mal formada."));
        }
        [Test]
        public void ValidateLine_ShouldReturnError_WhenISBNIsDuplicated()
        {
            // Arrange
            string isbn = "ISBN-12345";
            string line1 = $"{isbn} | Libro Original | 2020 | 5.0 | Autor A";
            string line2 = $"{isbn} | Libro Duplicado | 2021 | 4.0 | Autor B";

            // Primero procesamos la línea 1 para que el ISBN ya exista en el set
            _validator.ValidateLine(line1, _mappedRecords);

            // Act - Intentamos validar la línea 2 con el mismo ISBN
            var result = _validator.ValidateLine(line2, _mappedRecords);

            // Assert
            Assert.That(result.esValido, Is.False);
            Assert.That(result.error, Does.Contain("IS DUPLICATED"));
        }
    }
}