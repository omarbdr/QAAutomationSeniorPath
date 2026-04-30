using Library.Domain;
using Moq;

[TestFixture]
public class LibraryServiceTests
{
    private Mock<ILibraryDbContext> _mockDb;
    private LibraryService _service;

    [SetUp]
    public void Setup()
    {
        _mockDb = new Mock<ILibraryDbContext>();
        _service = new LibraryService(_mockDb.Object);
    }

    // 1. EL QUE YA TIENES (Validación de multas)
    [Test]
    public void PrestarLibro_UsuarioConMultas_DebeLanzarExcepcion()
    {
        var usuario = new UserRecord { Id = 1, HasPendingFines = true };
        _mockDb.Setup(db => db.Users.Find(1)).Returns(usuario);
        _mockDb.Setup(db => db.Books.Find(It.IsAny<int>())).Returns(new BookRecord());

        var ex = Assert.Throws<Exception>(() => _service.LendBook(1, 1));
        Assert.That(ex.Message, Does.Contain("tiene multas"));
    }

    // 2. EL HAPPY PATH (Crucial para asegurar que el sistema sirve)
    [Test]
    public void PrestarLibro_DatosCorrectos_DebeMarcarComoPrestado()
    {
        // Arrange
        var usuario = new UserRecord { Id = 1, HasPendingFines = false };
        var libro = new BookRecord { Id = 100, OnLoan = false };

        _mockDb.Setup(db => db.Users.Find(1)).Returns(usuario);
        _mockDb.Setup(db => db.Books.Find(100)).Returns(libro);
        // Act
        _service.LendBook(100, 1);

        // Assert
        Assert.That(libro.OnLoan, Is.True); // Verificamos que el cambio ocurrió
        _mockDb.Verify(m => m.SaveChanges(), Times.Once); // Verificamos que se intentó guardar
    }
}