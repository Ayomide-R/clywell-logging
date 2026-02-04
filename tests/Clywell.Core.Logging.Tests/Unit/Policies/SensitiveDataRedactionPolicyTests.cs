using Clywell.Core.Logging.Policies;
using Xunit;

namespace Clywell.Core.Logging.Tests.Unit.Policies;

public sealed class SensitiveDataRedactionPolicyTests
{
    [Theory]
    [InlineData("4532-1234-5678-9010", "***REDACTED***")]
    [InlineData("4532 1234 5678 9010", "***REDACTED***")]
    [InlineData("4532123456789010", "***REDACTED***")]
    public void RedactSensitiveData_WithCreditCard_RedactsValue(string input, string expected)
    {
        // Act
        var result = SensitiveDataRedactionPolicy.RedactSensitiveData(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123-45-6789", "***REDACTED***")]
    [InlineData("My SSN is 123-45-6789", "My SSN is ***REDACTED***")]
    public void RedactSensitiveData_WithSocialSecurity_RedactsValue(string input, string expected)
    {
        // Act
        var result = SensitiveDataRedactionPolicy.RedactSensitiveData(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("password: mySecretPassword123", "***REDACTED***")]
    [InlineData("pwd=test123", "***REDACTED***")]
    [InlineData("passwd: admin", "***REDACTED***")]
    public void RedactSensitiveData_WithPassword_RedactsValue(string input, string expected)
    {
        // Act
        var result = SensitiveDataRedactionPolicy.RedactSensitiveData(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("api_key: abc123xyz", "***REDACTED***")]
    [InlineData("apikey=secret", "***REDACTED***")]
    [InlineData("access_token: bearer_token_here", "***REDACTED***")]
    public void RedactSensitiveData_WithApiKey_RedactsValue(string input, string expected)
    {
        // Act
        var result = SensitiveDataRedactionPolicy.RedactSensitiveData(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    [InlineData("Hello World", "Hello World")]
    [InlineData("No sensitive data here", "No sensitive data here")]
    public void RedactSensitiveData_WithNonSensitiveData_ReturnsOriginal(string input, string expected)
    {
        // Act
        var result = SensitiveDataRedactionPolicy.RedactSensitiveData(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TryDestructure_WithStringValue_ReturnsTrue()
    {
        // Arrange
        var policy = new SensitiveDataRedactionPolicy();
        var value = "password: secret";

        // Act
        var result = policy.TryDestructure(
            value,
            new TestPropertyValueFactory(),
            out var propertyValue);

        // Assert
        Assert.True(result);
        Assert.NotNull(propertyValue);
    }

    [Fact]
    public void TryDestructure_WithNonStringValue_ReturnsFalse()
    {
        // Arrange
        var policy = new SensitiveDataRedactionPolicy();
        var value = 123;

        // Act
        var result = policy.TryDestructure(
            value,
            new TestPropertyValueFactory(),
            out var propertyValue);

        // Assert
        Assert.False(result);
        Assert.Null(propertyValue);
    }

    [Fact]
    public void TryDestructure_WithNonSensitiveString_ReturnsFalse()
    {
        // Arrange
        var policy = new SensitiveDataRedactionPolicy();
        var value = "Hello World";

        // Act
        var result = policy.TryDestructure(
            value,
            new TestPropertyValueFactory(),
            out var propertyValue);

        // Assert
        Assert.False(result);
        Assert.Null(propertyValue);
    }

    [Fact]
    public void Create_WithDefaultConfig_HasAllPatterns()
    {
        // Arrange & Act
        var policy = SensitiveDataRedactionPolicyOptions.Create().Build();

        // Assert - test that all default patterns work
        Assert.Equal("***REDACTED***", policy.RedactSensitiveDataInternal("4532-1234-5678-9010"));
        Assert.Equal("***REDACTED***", policy.RedactSensitiveDataInternal("123-45-6789"));
        Assert.Equal("***REDACTED***", policy.RedactSensitiveDataInternal("password: secret"));
    }

    [Fact]
    public void Create_WithDisabledCreditCard_DoesNotRedactCreditCard()
    {
        // Arrange
        var policy = SensitiveDataRedactionPolicyOptions.Create()
            .DisableCreditCardRedaction()
            .Build();

        // Act
        var result = policy.RedactSensitiveDataInternal("4532-1234-5678-9010");

        // Assert
        Assert.Equal("4532-1234-5678-9010", result);
    }

    [Fact]
    public void Create_WithDisabledPassword_DoesNotRedactPassword()
    {
        // Arrange
        var policy = SensitiveDataRedactionPolicyOptions.Create()
            .DisablePasswordRedaction()
            .Build();

        // Act
        var result = policy.RedactSensitiveDataInternal("password: secret");

        // Assert
        Assert.Equal("password: secret", result);
    }

    [Fact]
    public void Create_WithCustomPattern_RedactsCustomData()
    {
        // Arrange
        var policy = SensitiveDataRedactionPolicyOptions.Create()
            .DisableAllDefaults()
            .AddCustomPattern(@"\bDATABASE_URL\b")
            .Build();

        // Act
        var result = policy.RedactSensitiveDataInternal("DATABASE_URL=server:1234");

        // Assert
        Assert.Equal("***REDACTED***=server:1234", result);
    }

    [Fact]
    public void Create_WithMultipleCustomPatterns_RedactsAll()
    {
        // Arrange
        var policy = SensitiveDataRedactionPolicyOptions.Create()
            .DisableAllDefaults()
            .AddCustomPattern(@"\btoken\b")
            .AddCustomPattern(@"\bsecret\b")
            .Build();

        // Act
        var result = policy.RedactSensitiveDataInternal("token and secret values");

        // Assert
        Assert.Equal("***REDACTED*** and ***REDACTED*** values", result);
    }

    [Fact]
    public void Create_WithCustomAndDefaultPatterns_RedactsBoth()
    {
        // Arrange
        var policy = SensitiveDataRedactionPolicyOptions.Create()
            .AddCustomPattern(@"\bCLIENT_SECRET\b")
            .Build();

        // Act
        var result = policy.RedactSensitiveDataInternal("password: admin and CLIENT_SECRET");

        // Assert
        Assert.Equal("***REDACTED*** and ***REDACTED***", result);
    }

    [Fact]
    public void Default_Property_ReturnsDefaultInstance()
    {
        // Act
        var defaultPolicy = SensitiveDataRedactionPolicy.Default;

        // Assert
        Assert.NotNull(defaultPolicy);
        Assert.Equal("***REDACTED***", defaultPolicy.RedactSensitiveDataInternal("4532-1234-5678-9010"));
    }

    private sealed class TestPropertyValueFactory : Serilog.Core.ILogEventPropertyValueFactory
    {
        public Serilog.Events.LogEventPropertyValue CreatePropertyValue(object? value, bool destructureObjects = false)
        {
            return new Serilog.Events.ScalarValue(value);
        }
    }
}
