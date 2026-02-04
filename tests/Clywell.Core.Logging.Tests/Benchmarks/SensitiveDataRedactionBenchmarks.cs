using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Clywell.Core.Logging.Policies;
using System.Text.RegularExpressions;

namespace Clywell.Core.Logging.Tests.Benchmarks;

/// <summary>
/// Benchmarks measuring the performance of sensitive data redaction.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 5, iterationCount: 15, invocationCount: 10000)]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column]
public class SensitiveDataRedactionBenchmarks
{
    private SensitiveDataRedactionPolicy _defaultPolicy = null!;
    private SensitiveDataRedactionPolicy _customPolicy = null!;
    private SensitiveDataRedactionPolicy _minimalPolicy = null!;

    private const string MessageWithCreditCard = "Processing payment with card 4532-1234-5678-9010 for customer";
    private const string MessageWithEmail = "Sending notification to user@example.com about the order";
    private const string MessageWithSsn = "User SSN 123-45-6789 verified successfully";
    private const string MessageWithMultiple = "User john.doe@example.com with card 4532-1234-5678-9010 and SSN 123-45-6789";
    private const string MessageWithNoSensitiveData = "Order processed successfully for tenant-123";
    private const string MessageWithUrl = "API call to https://api.example.com/users?token=abc123&key=secret456";

    [GlobalSetup]
    public void Setup()
    {
        // Default policy with all patterns
        _defaultPolicy = SensitiveDataRedactionPolicy.Default;

        // Custom policy with additional patterns
        _customPolicy = SensitiveDataRedactionPolicyOptions.Create()
            .AddCustomPattern(@"api[_-]?key[:\s=]+[^\s]+", RegexOptions.IgnoreCase)
            .AddCustomPattern(@"bearer\s+[^\s]+", RegexOptions.IgnoreCase)
            .Build();

        // Minimal policy (only credit cards)
        _minimalPolicy = SensitiveDataRedactionPolicyOptions.Create()
            .DisableAllDefaults()
            .AddCustomPattern(@"\b\d{4}[-\s]?\d{4}[-\s]?\d{4}[-\s]?\d{4}\b")
            .Build();
    }

    // ============================================================
    // Default Policy Tests
    // ============================================================

    [Benchmark(Baseline = true)]
    public string DefaultPolicy_NoSensitiveData()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithNoSensitiveData);
    }

    [Benchmark]
    public string DefaultPolicy_CreditCard()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithCreditCard);
    }

    [Benchmark]
    public string DefaultPolicy_Email()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithEmail);
    }

    [Benchmark]
    public string DefaultPolicy_Ssn()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithSsn);
    }

    [Benchmark]
    public string DefaultPolicy_MultiplePatterns()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithMultiple);
    }

    [Benchmark]
    public string DefaultPolicy_Url()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithUrl);
    }

    // ============================================================
    // Custom Policy Tests
    // ============================================================

    [Benchmark]
    public string CustomPolicy_NoSensitiveData()
    {
        return _customPolicy.RedactSensitiveDataInternal(MessageWithNoSensitiveData);
    }

    [Benchmark]
    public string CustomPolicy_CreditCard()
    {
        return _customPolicy.RedactSensitiveDataInternal(MessageWithCreditCard);
    }

    [Benchmark]
    public string CustomPolicy_MultiplePatterns()
    {
        return _customPolicy.RedactSensitiveDataInternal(MessageWithMultiple);
    }

    // ============================================================
    // Minimal Policy Tests (Fewer Patterns = Better Performance)
    // ============================================================

    [Benchmark]
    public string MinimalPolicy_NoSensitiveData()
    {
        return _minimalPolicy.RedactSensitiveDataInternal(MessageWithNoSensitiveData);
    }

    [Benchmark]
    public string MinimalPolicy_CreditCard()
    {
        return _minimalPolicy.RedactSensitiveDataInternal(MessageWithCreditCard);
    }

    [Benchmark]
    public string MinimalPolicy_MultiplePatterns()
    {
        return _minimalPolicy.RedactSensitiveDataInternal(MessageWithMultiple);
    }

    // ============================================================
    // High-Frequency Scenarios
    // ============================================================

    [Benchmark]
    public void DefaultPolicy_HighFrequency_100Calls_NoSensitiveData()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithNoSensitiveData);
        }
    }

    [Benchmark]
    public void DefaultPolicy_HighFrequency_100Calls_WithSensitiveData()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithCreditCard);
        }
    }

    [Benchmark]
    public void MinimalPolicy_HighFrequency_100Calls_NoSensitiveData()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _minimalPolicy.RedactSensitiveDataInternal(MessageWithNoSensitiveData);
        }
    }

    [Benchmark]
    public void MinimalPolicy_HighFrequency_100Calls_WithSensitiveData()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _minimalPolicy.RedactSensitiveDataInternal(MessageWithCreditCard);
        }
    }

    // ============================================================
    // Variable Message Lengths
    // ============================================================

    [Benchmark]
    public string ShortMessage_WithSensitiveData()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData("CC: 4532123456789010");
    }

    [Benchmark]
    public string LongMessage_WithSensitiveData()
    {
        var longMessage = string.Concat(
            "Processing a very long transaction log entry with extensive details about the operation. ",
            "Customer information includes email user@example.com and payment card 4532-1234-5678-9010. ",
            "The transaction was initiated at 2026-02-03T10:30:00Z and completed in 2.5 seconds. ",
            "Additional metadata includes request ID req-abc123, session ID sess-xyz789, and tenant tenant-456. ",
            "This is followed by more descriptive text to simulate a realistic log message length.");

        return SensitiveDataRedactionPolicy.RedactSensitiveData(longMessage);
    }

    // ============================================================
    // Comparing Instance vs Static Methods
    // ============================================================

    [Benchmark]
    public string StaticMethod_DefaultPolicy()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(MessageWithCreditCard);
    }

    [Benchmark]
    public string InstanceMethod_DefaultPolicy()
    {
        return _defaultPolicy.RedactSensitiveDataInternal(MessageWithCreditCard);
    }

    // ============================================================
    // Edge Cases
    // ============================================================

    [Benchmark]
    public string EmptyString()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData(string.Empty);
    }

    [Benchmark]
    public string VeryShortString()
    {
        return SensitiveDataRedactionPolicy.RedactSensitiveData("OK");
    }

    [Benchmark]
    public string NoMatchingPatterns()
    {
        var message = "Order-12345 processed for tenant-abc with status SUCCESS in 150ms";
        return SensitiveDataRedactionPolicy.RedactSensitiveData(message);
    }

    [Benchmark]
    public string MultipleOccurrencesSamePattern()
    {
        var message = "Cards: 4532-1234-5678-9010, 5432-9876-5432-1098, 3782-822463-10005";
        return SensitiveDataRedactionPolicy.RedactSensitiveData(message);
    }
}
