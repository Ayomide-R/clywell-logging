using Clywell.Core.Logging.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Extensions;

public sealed class LoggerScopeExtensionsTests
{
    [Fact]
    public void BeginPropertyScope_WithSingleProperty_CreatesScope()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginPropertyScope("TestKey", "TestValue");

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.ContainsKey("TestKey") && d["TestKey"]!.Equals("TestValue"))), Times.Once);
    }

    [Fact]
    public void BeginPropertyScope_WithTwoProperties_CreatesScope()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginPropertyScope(
            ("Key1", "Value1"),
            ("Key2", "Value2"));

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.Count == 2 &&
                 d.ContainsKey("Key1") && d["Key1"]!.Equals("Value1") &&
                 d.ContainsKey("Key2") && d["Key2"]!.Equals("Value2"))), Times.Once);
    }

    [Fact]
    public void BeginPropertyScope_WithThreeProperties_CreatesScope()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginPropertyScope(
            ("Key1", "Value1"),
            ("Key2", "Value2"),
            ("Key3", "Value3"));

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.Count == 3)), Times.Once);
    }

    [Fact]
    public void BeginPropertyScope_WithDictionary_CreatesScope()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        var properties = new Dictionary<string, object?>
        {
            ["Key1"] = "Value1",
            ["Key2"] = 42
        };
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginPropertyScope(properties);

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(properties), Times.Once);
    }

    [Fact]
    public void BeginPropertyScope_WithParamsArray_CreatesScope()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginPropertyScope(
            ("Key1", "Value1"),
            ("Key2", 42),
            ("Key3", true));

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.Count == 3)), Times.Once);
    }

    [Fact]
    public void BeginTenantScope_CreatesScopeWithTenantId()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginTenantScope("tenant-123");

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.ContainsKey("TenantId") && d["TenantId"]!.Equals("tenant-123"))), Times.Once);
    }

    [Fact]
    public void BeginUserScope_CreatesScopeWithUserId()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginUserScope("user-456");

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.ContainsKey("UserId") && d["UserId"]!.Equals("user-456"))), Times.Once);
    }

    [Fact]
    public void BeginTenantUserScope_CreatesScopeWithBothIds()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginTenantUserScope("tenant-123", "user-456");

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.ContainsKey("TenantId") && d["TenantId"]!.Equals("tenant-123") &&
                 d.ContainsKey("UserId") && d["UserId"]!.Equals("user-456"))), Times.Once);
    }

    [Fact]
    public void BeginOperationScope_WithNameOnly_CreatesScopeWithOperationName()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginOperationScope("ProcessOrder");

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.ContainsKey("OperationName") && d["OperationName"]!.Equals("ProcessOrder") &&
                 d.Count == 1)), Times.Once);
    }

    [Fact]
    public void BeginOperationScope_WithNameAndId_CreatesScopeWithBoth()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var scopeMock = new Mock<IDisposable>();
        mockLogger.Setup(l => l.BeginScope(It.IsAny<Dictionary<string, object?>>()))
            .Returns(scopeMock.Object);

        // Act
        var scope = mockLogger.Object.BeginOperationScope("ProcessOrder", "op-789");

        // Assert
        Assert.NotNull(scope);
        mockLogger.Verify(l => l.BeginScope(It.Is<Dictionary<string, object?>>(
            d => d.ContainsKey("OperationName") && d["OperationName"]!.Equals("ProcessOrder") &&
                 d.ContainsKey("OperationId") && d["OperationId"]!.Equals("op-789"))), Times.Once);
    }

    [Fact]
    public void BeginPropertyScope_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger logger = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => logger.BeginPropertyScope("key", "value"));
    }

    [Fact]
    public void BeginTenantScope_WithNullTenantId_ThrowsArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mockLogger.Object.BeginTenantScope(null!));
    }

    [Fact]
    public void BeginUserScope_WithNullUserId_ThrowsArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => mockLogger.Object.BeginUserScope(null!));
    }
}
