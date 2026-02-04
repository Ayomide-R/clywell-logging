using Microsoft.Extensions.Logging;
using Moq;
using Clywell.Core.Logging.Extensions;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Extensions;

public sealed class LoggerExtensionsTests
{
    [Fact]
    public void Debug_WhenDebugEnabled_CallsUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Debug)).Returns(true);

        // Act
        mockLogger.Object.Debug("Debug message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Debug_WhenDebugDisabled_DoesNotCallUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Debug)).Returns(false);

        // Act
        mockLogger.Object.Debug("Debug message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public void Debug_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger logger = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => logger.Debug("message"));
        Assert.Equal("logger", ex.ParamName);
    }

    [Fact]
    public void Trace_WhenTraceEnabled_CallsUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Trace)).Returns(true);

        // Act
        mockLogger.Object.Trace("Trace message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Trace_WhenTraceDisabled_DoesNotCallUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Trace)).Returns(false);

        // Act
        mockLogger.Object.Trace("Trace message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public void Info_WhenInformationEnabled_CallsUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Information)).Returns(true);

        // Act
        mockLogger.Object.Info("Info message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Info_WhenInformationDisabled_DoesNotCallUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Information)).Returns(false);

        // Act
        mockLogger.Object.Info("Info message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public void Warning_WhenWarningEnabled_CallsUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Warning)).Returns(true);

        // Act
        mockLogger.Object.Warning("Warning message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Error_WhenErrorEnabled_CallsUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Error)).Returns(true);

        // Act
        mockLogger.Object.Error("Error message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Error_WithException_WhenErrorEnabled_CallsUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Error)).Returns(true);
        var exception = new InvalidOperationException("Test exception");

        // Act
        mockLogger.Object.Error(exception, "Error message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Critical_WhenCriticalEnabled_CallsUnderlyingLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(l => l.IsEnabled(LogLevel.Critical)).Returns(true);

        // Act
        mockLogger.Object.Critical("Critical message {Param}", "value");

        // Assert
        mockLogger.Verify(
            l => l.Log(
                LogLevel.Critical,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void LogExecutionTime_ExecutesOperation_ReturnsResult()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        const int expectedResult = 42;

        // Act
        var result = mockLogger.Object.LogExecutionTime("TestOperation", () => expectedResult);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void LogExecutionTime_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger logger = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => logger.LogExecutionTime("test", () => 42));
        Assert.Equal("logger", ex.ParamName);
    }

    [Fact]
    public void LogExecutionTime_WithNullOperation_ThrowsArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => mockLogger.Object.LogExecutionTime<int>("test", null!));
        Assert.Equal("operation", ex.ParamName);
    }

    [Fact]
    public async Task LogExecutionTimeAsync_ExecutesOperation_ReturnsResult()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        const int expectedResult = 42;

        // Act
        var result = await mockLogger.Object.LogExecutionTimeAsync(
            "TestOperation",
            async () =>
            {
                await Task.Delay(10);
                return expectedResult;
            });

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task LogExecutionTimeAsync_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger logger = null!;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await logger.LogExecutionTimeAsync("test", () => Task.FromResult(42)));
        Assert.Equal("logger", ex.ParamName);
    }

    [Fact]
    public async Task LogExecutionTimeAsync_WithNullOperation_ThrowsArgumentNullException()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () => await mockLogger.Object.LogExecutionTimeAsync<int>("test", null!));
        Assert.Equal("operation", ex.ParamName);
    }

    [Fact]
    public void BeginTimedScope_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        ILogger logger = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => logger.BeginTimedScope("test"));
        Assert.Equal("logger", ex.ParamName);
    }

    [Fact]
    public void BeginTimedScope_ReturnsDisposableScope()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();

        // Act
        var scope = mockLogger.Object.BeginTimedScope("TestScope");

        // Assert
        Assert.NotNull(scope);
        Assert.IsAssignableFrom<IDisposable>(scope);

        // Cleanup
        scope.Dispose();
    }

    [Fact]
    public void BeginTimedScope_WithProperties_ReturnsDisposableScope()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var properties = new Dictionary<string, object>
        {
            ["Property1"] = "Value1",
            ["Property2"] = 42
        };

        // Act
        var scope = mockLogger.Object.BeginTimedScope("TestScope", properties);

        // Assert
        Assert.NotNull(scope);

        // Cleanup
        scope.Dispose();
    }
}
