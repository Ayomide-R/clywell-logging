using Serilog.Events;
using Serilog.Core;
using Clywell.Core.Logging.Enrichers;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Enrichers;

public sealed class RequestIdEnricherTests
{
    [Fact]
    public void Enrich_WithoutRequestId_DoesNotAddProperty()
    {
        // Arrange
        var enricher = new RequestIdEnricher();
        var logEvent = CreateLogEvent();
        var propertyFactory = new LogEventPropertyFactory();

        // Act
        enricher.Enrich(logEvent, propertyFactory);

        // Assert
        Assert.DoesNotContain("RequestId", logEvent.Properties);
    }

    [Fact]
    public void Enrich_WithExistingRequestId_AddsProperty()
    {
        // Arrange
        var expectedRequestId = "request-123";
        RequestIdEnricher.CurrentRequestId = expectedRequestId;

        var enricher = new RequestIdEnricher();
        var logEvent = CreateLogEvent();
        var propertyFactory = new LogEventPropertyFactory();

        // Act
        enricher.Enrich(logEvent, propertyFactory);

        // Assert
        Assert.Contains("RequestId", logEvent.Properties);
        var requestId = logEvent.Properties["RequestId"].ToString().Trim('"');
        Assert.Equal(expectedRequestId, requestId);

        // Cleanup
        RequestIdEnricher.CurrentRequestId = null;
    }

    [Fact]
    public void Enrich_WithNullLogEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var enricher = new RequestIdEnricher();
        var propertyFactory = new LogEventPropertyFactory();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => enricher.Enrich(null!, propertyFactory));
        Assert.Equal("logEvent", ex.ParamName);
    }

    [Fact]
    public void Enrich_WithNullPropertyFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var enricher = new RequestIdEnricher();
        var logEvent = CreateLogEvent();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => enricher.Enrich(logEvent, null!));
        Assert.Equal("propertyFactory", ex.ParamName);
    }

    [Fact]
    public void CurrentRequestId_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var expectedValue = "request-789";

        // Act
        RequestIdEnricher.CurrentRequestId = expectedValue;
        var actualValue = RequestIdEnricher.CurrentRequestId;

        // Assert
        Assert.Equal(expectedValue, actualValue);

        // Cleanup
        RequestIdEnricher.CurrentRequestId = null;
    }

    [Fact]
    public void Enrich_WithEmptyRequestId_DoesNotAddProperty()
    {
        // Arrange
        RequestIdEnricher.CurrentRequestId = string.Empty;

        var enricher = new RequestIdEnricher();
        var logEvent = CreateLogEvent();
        var propertyFactory = new LogEventPropertyFactory();

        // Act
        enricher.Enrich(logEvent, propertyFactory);

        // Assert
        Assert.DoesNotContain("RequestId", logEvent.Properties);

        // Cleanup
        RequestIdEnricher.CurrentRequestId = null;
    }

    private static LogEvent CreateLogEvent()
    {
        return new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Information,
            null,
            MessageTemplate.Empty,
            Array.Empty<LogEventProperty>());
    }

    private sealed class LogEventPropertyFactory : ILogEventPropertyFactory
    {
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false)
        {
            return new LogEventProperty(name, new ScalarValue(value));
        }
    }
}
