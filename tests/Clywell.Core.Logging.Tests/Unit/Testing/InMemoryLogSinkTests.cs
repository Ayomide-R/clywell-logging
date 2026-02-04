using Clywell.Core.Logging.Testing;
using Serilog.Events;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Testing;

public sealed class InMemoryLogSinkTests
{
    [Fact]
    public void Emit_AddsEventToCollection()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        var logEvent = CreateLogEvent(LogEventLevel.Information, "Test message");

        // Act
        sink.Emit(logEvent);

        // Assert
        Assert.Equal(1, sink.Count);
        Assert.Contains(logEvent, sink.LoggedEvents);
    }

    [Fact]
    public void Clear_RemovesAllEvents()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Message 1"));
        sink.Emit(CreateLogEvent(LogEventLevel.Warning, "Message 2"));

        // Act
        sink.Clear();

        // Assert
        Assert.Equal(0, sink.Count);
        Assert.Empty(sink.LoggedEvents);
    }

    [Fact]
    public void GetEvents_FiltersByMinimumLevel()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Debug, "Debug"));
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Info"));
        sink.Emit(CreateLogEvent(LogEventLevel.Warning, "Warning"));
        sink.Emit(CreateLogEvent(LogEventLevel.Error, "Error"));

        // Act
        var events = sink.GetEvents(LogEventLevel.Warning);

        // Assert
        Assert.Equal(2, events.Count);
        Assert.All(events, e => Assert.True(e.Level >= LogEventLevel.Warning));
    }

    [Fact]
    public void GetEventsForLevel_ReturnsOnlySpecifiedLevel()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Info 1"));
        sink.Emit(CreateLogEvent(LogEventLevel.Warning, "Warning"));
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Info 2"));

        // Act
        var events = sink.GetEventsForLevel(LogEventLevel.Information);

        // Assert
        Assert.Equal(2, events.Count);
        Assert.All(events, e => Assert.Equal(LogEventLevel.Information, e.Level));
    }

    [Fact]
    public void GetEventsContaining_ReturnsMatchingEvents()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "User login successful"));
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "User logout"));
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Data processed"));

        // Act
        var events = sink.GetEventsContaining("User");

        // Assert
        Assert.Equal(2, events.Count);
    }

    [Fact]
    public void GetEventsWithProperty_ReturnsEventsWithProperty()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Message", ("UserId", "123")));
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Message", ("TenantId", "abc")));
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Message"));

        // Act
        var events = sink.GetEventsWithProperty("UserId");

        // Assert
        Assert.Single(events);
    }

    [Fact]
    public void HasEventsAt_ReturnsTrueWhenEventsExist()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Error, "Error occurred"));

        // Act & Assert
        Assert.True(sink.HasEventsAt(LogEventLevel.Error));
        Assert.False(sink.HasEventsAt(LogEventLevel.Warning));
    }

    [Fact]
    public void HasException_DetectsExceptionType()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        var exception = new InvalidOperationException("Test");
        sink.Emit(CreateLogEvent(LogEventLevel.Error, "Error", exception: exception));

        // Act & Assert
        Assert.True(sink.HasException<InvalidOperationException>());
        Assert.False(sink.HasException<ArgumentNullException>());
    }

    [Fact]
    public void GetEventsWithExceptions_ReturnsOnlyEventsWithExceptions()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Information, "Normal message"));
        sink.Emit(CreateLogEvent(LogEventLevel.Error, "Error", exception: new Exception("Test")));
        sink.Emit(CreateLogEvent(LogEventLevel.Warning, "Warning"));

        // Act
        var events = sink.GetEventsWithExceptions();

        // Assert
        Assert.Single(events);
        Assert.NotNull(events[0].Exception);
    }

    [Fact]
    public void GetEventsWithException_FiltersByExceptionType()
    {
        // Arrange
        var sink = new InMemoryLogSink();
        sink.Emit(CreateLogEvent(LogEventLevel.Error, "Error 1", exception: new InvalidOperationException()));
        sink.Emit(CreateLogEvent(LogEventLevel.Error, "Error 2", exception: new ArgumentNullException()));
        sink.Emit(CreateLogEvent(LogEventLevel.Error, "Error 3", exception: new InvalidOperationException()));

        // Act
        var events = sink.GetEventsWithException<InvalidOperationException>();

        // Assert
        Assert.Equal(2, events.Count);
    }

    [Fact]
    public void Emit_WithNullEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var sink = new InMemoryLogSink();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sink.Emit(null!));
    }

    private static LogEvent CreateLogEvent(
        LogEventLevel level,
        string message,
        (string Name, object Value)? property = null,
        Exception? exception = null)
    {
        var properties = new Dictionary<string, LogEventPropertyValue>();
        if (property.HasValue)
        {
            properties[property.Value.Name] = new ScalarValue(property.Value.Value);
        }

        return new LogEvent(
            DateTimeOffset.UtcNow,
            level,
            exception,
            new Serilog.Parsing.MessageTemplateParser().Parse(message),
            properties.Select(kvp => new LogEventProperty(kvp.Key, kvp.Value)));
    }
}
