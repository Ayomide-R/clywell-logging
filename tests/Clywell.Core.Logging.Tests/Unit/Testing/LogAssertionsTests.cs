using Clywell.Core.Logging.Testing;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Testing;

public sealed class LogAssertionsTests
{
    [Fact]
    public void ShouldHaveLogged_WithLevel_PassesWhenEventExists()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Test message");

        // Act & Assert - should not throw
        sink.ShouldHaveLogged(LogEventLevel.Information);
    }

    [Fact]
    public void ShouldHaveLogged_WithLevel_ThrowsWhenEventDoesNotExist()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Test message");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sink.ShouldHaveLogged(LogEventLevel.Error));
    }

    [Fact]
    public void ShouldHaveLogged_WithLevelAndMessage_PassesWhenEventMatches()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogWarning("User {UserId} failed login", "user-123");

        // Act & Assert - should not throw
        sink.ShouldHaveLogged(LogEventLevel.Warning, "failed login");
    }

    [Fact]
    public void ShouldHaveLogged_WithLevelAndMessage_ThrowsWhenMessageDoesNotMatch()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogWarning("User logged in");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sink.ShouldHaveLogged(LogEventLevel.Warning, "logged out"));
    }

    [Fact]
    public void ShouldNotHaveLogged_PassesWhenNoEventsAtLevel()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Test message");

        // Act & Assert - should not throw
        sink.ShouldNotHaveLogged(LogEventLevel.Error);
    }

    [Fact]
    public void ShouldNotHaveLogged_ThrowsWhenEventsExistAtLevel()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogError("Error occurred");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sink.ShouldNotHaveLogged(LogEventLevel.Error));
    }

    [Fact]
    public void ShouldHaveProperty_PassesWhenPropertyExists()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        using (logger.BeginScope(new Dictionary<string, object> { ["UserId"] = "123" }))
        {
            logger.LogInformation("User action");
        }

        // Act & Assert - should not throw
        sink.ShouldHaveProperty("UserId");
    }

    [Fact]
    public void ShouldHaveProperty_ThrowsWhenPropertyDoesNotExist()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Message without properties");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sink.ShouldHaveProperty("UserId"));
    }

    [Fact]
    public void ShouldHaveException_PassesWhenExceptionExists()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        var exception = new InvalidOperationException("Test error");
        logger.LogError(exception, "Operation failed");

        // Act & Assert - should not throw
        sink.ShouldHaveException<InvalidOperationException>();
    }

    [Fact]
    public void ShouldHaveException_ThrowsWhenExceptionTypeDoesNotMatch()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        var exception = new InvalidOperationException("Test error");
        logger.LogError(exception, "Operation failed");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sink.ShouldHaveException<ArgumentNullException>());
    }

    [Fact]
    public void ShouldHaveEventCount_PassesWhenCountMatches()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Message 1");
        logger.LogInformation("Message 2");
        logger.LogWarning("Message 3");

        // Act & Assert - should not throw
        sink.ShouldHaveEventCount(3);
    }

    [Fact]
    public void ShouldHaveEventCount_ThrowsWhenCountDoesNotMatch()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Message");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sink.ShouldHaveEventCount(5));
    }

    [Fact]
    public void ShouldHaveEventCount_WithLevel_PassesWhenCountMatches()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Info 1");
        logger.LogInformation("Info 2");
        logger.LogWarning("Warning 1");

        // Act & Assert - should not throw
        sink.ShouldHaveEventCount(LogEventLevel.Information, 2);
    }

    [Fact]
    public void ShouldHaveEventCount_WithLevel_ThrowsWhenCountDoesNotMatch()
    {
        // Arrange
        var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();
        logger.LogInformation("Message");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sink.ShouldHaveEventCount(LogEventLevel.Information, 3));
    }
}
