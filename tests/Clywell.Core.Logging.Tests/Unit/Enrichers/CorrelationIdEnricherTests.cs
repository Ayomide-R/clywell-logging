using Serilog.Events;
using Serilog.Core;
using Clywell.Core.Logging.Enrichers;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Enrichers;

public sealed class CorrelationIdEnricherTests
{
    [Fact]
    public void Enrich_WithoutCorrelationId_GeneratesNewGuid()
    {
        // Arrange
        var enricher = new CorrelationIdEnricher();
        var logEvent = CreateLogEvent();
        var propertyFactory = new LogEventPropertyFactory();

        // Act
        enricher.Enrich(logEvent, propertyFactory);

        // Assert
        Assert.Contains("CorrelationId", logEvent.Properties);
        var correlationId = logEvent.Properties["CorrelationId"].ToString();
        Assert.NotNull(correlationId);
        Assert.NotEmpty(correlationId);
    }

    [Fact]
    public void Enrich_WithExistingCorrelationId_UsesProvidedValue()
    {
        // Arrange
        var expectedCorrelationId = "test-correlation-123";
        CorrelationIdEnricher.CurrentCorrelationId = expectedCorrelationId;

        var enricher = new CorrelationIdEnricher();
        var logEvent = CreateLogEvent();
        var propertyFactory = new LogEventPropertyFactory();

        // Act
        enricher.Enrich(logEvent, propertyFactory);

        // Assert
        Assert.Contains("CorrelationId", logEvent.Properties);
        var correlationId = logEvent.Properties["CorrelationId"].ToString().Trim('"');
        Assert.Equal(expectedCorrelationId, correlationId);

        // Cleanup
        CorrelationIdEnricher.CurrentCorrelationId = null;
    }

    [Fact]
    public void Enrich_WithNullLogEvent_ThrowsArgumentNullException()
    {
        // Arrange
        var enricher = new CorrelationIdEnricher();
        var propertyFactory = new LogEventPropertyFactory();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => enricher.Enrich(null!, propertyFactory));
        Assert.Equal("logEvent", ex.ParamName);
    }

    [Fact]
    public void Enrich_WithNullPropertyFactory_ThrowsArgumentNullException()
    {
        // Arrange
        var enricher = new CorrelationIdEnricher();
        var logEvent = CreateLogEvent();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => enricher.Enrich(logEvent, null!));
        Assert.Equal("propertyFactory", ex.ParamName);
    }

    [Fact]
    public void CurrentCorrelationId_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var expectedValue = "correlation-456";

        // Act
        CorrelationIdEnricher.CurrentCorrelationId = expectedValue;
        var actualValue = CorrelationIdEnricher.CurrentCorrelationId;

        // Assert
        Assert.Equal(expectedValue, actualValue);

        // Cleanup
        CorrelationIdEnricher.CurrentCorrelationId = null;
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
