using Clywell.Core.Logging.Testing;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Testing;

public sealed class TestLoggerFactoryTests
{
    [Fact]
    public void CreateTestLogger_ReturnsWorkingLogger()
    {
        // Arrange
        var sink = new InMemoryLogSink();

        // Act
        var logger = TestLoggerFactory.CreateTestLogger(sink);
        logger.LogInformation("Test message");

        // Assert
        Assert.Equal(1, sink.Count);
        sink.ShouldHaveLogged(LogEventLevel.Information, "Test message");
    }

    [Fact]
    public void CreateTestLoggerWithSink_ReturnsBothLoggerAndSink()
    {
        // Act
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogWarning("Warning message");

        // Assert
        Assert.NotNull(logger);
        Assert.NotNull(sink);
        Assert.Equal(1, sink.Count);
        sink.ShouldHaveLogged(LogEventLevel.Warning);
    }

    [Fact]
    public void CreateTestLogger_Generic_ReturnsTypedLogger()
    {
        // Arrange
        var sink = new InMemoryLogSink();

        // Act
        var logger = TestLoggerFactory.CreateTestLogger<TestLoggerFactoryTests>(sink);
        logger.LogError("Error message");

        // Assert
        Assert.Equal(1, sink.Count);
        sink.ShouldHaveLogged(LogEventLevel.Error);
    }

    [Fact]
    public void CreateTestLoggerWithSink_Generic_ReturnsBothTypedLoggerAndSink()
    {
        // Act
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink<TestLoggerFactoryTests>();
        logger.LogDebug("Debug message");

        // Assert
        Assert.NotNull(logger);
        Assert.NotNull(sink);
        Assert.Equal(1, sink.Count);
    }

    [Fact]
    public void CreateSerilogTestLogger_ReturnsWorkingSerilogLogger()
    {
        // Arrange
        var sink = new InMemoryLogSink();

        // Act
        var logger = TestLoggerFactory.CreateSerilogTestLogger(sink);
        logger.Information("Test message");

        // Assert
        Assert.Equal(1, sink.Count);
    }

    [Fact]
    public void CreateSerilogTestLoggerWithSink_ReturnsBothLoggerAndSink()
    {
        // Act
        var (logger, sink) = TestLoggerFactory.CreateSerilogTestLoggerWithSink();
        logger.Warning("Warning message");

        // Assert
        Assert.NotNull(logger);
        Assert.NotNull(sink);
        Assert.Equal(1, sink.Count);
    }

    [Fact]
    public void CreateTestLogger_WithMinimumLevel_FiltersLogs()
    {
        // Arrange
        var sink = new InMemoryLogSink();

        // Act
        var logger = TestLoggerFactory.CreateTestLogger(sink, LogEventLevel.Warning);
        logger.LogDebug("Debug message");
        logger.LogInformation("Info message");
        logger.LogWarning("Warning message");
        logger.LogError("Error message");

        // Assert
        Assert.Equal(2, sink.Count); // Only Warning and Error
        Assert.False(sink.HasEventsAt(LogEventLevel.Debug));
        Assert.False(sink.HasEventsAt(LogEventLevel.Information));
        Assert.True(sink.HasEventsAt(LogEventLevel.Warning));
        Assert.True(sink.HasEventsAt(LogEventLevel.Error));
    }

    [Fact]
    public void CreateTestLogger_WithNullSink_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            TestLoggerFactory.CreateTestLogger(null!));
    }
}
