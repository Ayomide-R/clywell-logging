using Serilog.Events;
using Clywell.Core.Logging.Configuration;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Configuration;

public sealed class ClywellLoggerConfigurationTests
{
    [Fact]
    public void Create_WithoutAppConfig_ReturnsConfigurationBuilder()
    {
        // Act
        var config = ClywellLoggerConfiguration.Create();

        // Assert
        Assert.NotNull(config);
    }

    [Fact]
    public void WithMinimumLevel_SetsLogLevel()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithMinimumLevel(LogEventLevel.Debug);

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithConsoleSink_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithConsoleSink();

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithConsoleSink_WithJsonFormatting_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithConsoleSink(useJson: true);

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithFileSink_WithDefaults_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithFileSink();

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithCorrelationId_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithCorrelationId();

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithRequestId_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithRequestId();

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithEnvironmentEnrichers_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithEnvironmentEnrichers();

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithSensitiveDataRedaction_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithSensitiveDataRedaction();

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void WithClywellDefaults_ConfiguresAllDefaultEnrichers()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.WithClywellDefaults();

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }

    [Fact]
    public void Build_CreatesLogger()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create()
            .WithMinimumLevel(LogEventLevel.Information)
            .WithConsoleSink();

        // Act
        var logger = config.Build();

        // Assert
        Assert.NotNull(logger);
    }

    [Fact]
    public void FluentApi_SupportsMethodChaining()
    {
        // Arrange & Act
        var logger = ClywellLoggerConfiguration.Create()
            .WithMinimumLevel(LogEventLevel.Debug)
            .WithConsoleSink()
            .WithCorrelationId()
            .WithRequestId()
            .WithEnvironmentEnrichers()
            .WithSensitiveDataRedaction()
            .Build();

        // Assert
        Assert.NotNull(logger);
    }

    [Fact]
    public void OverrideMinimumLevel_ReturnsConfigurationBuilder()
    {
        // Arrange
        var config = ClywellLoggerConfiguration.Create();

        // Act
        var result = config.OverrideMinimumLevel("Microsoft", LogEventLevel.Warning);

        // Assert
        Assert.NotNull(result);
        Assert.Same(config, result);
    }
}
