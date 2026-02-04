using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Clywell.Core.Logging.Extensions;
using Clywell.Core.Logging.Testing;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Clywell.Core.Logging.Tests.Benchmarks;

/// <summary>
/// Benchmarks measuring the performance overhead of logging scopes.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 5, iterationCount: 15, invocationCount: 10000)]
[RankColumn, MinColumn, MaxColumn, Q1Column, Q3Column]
public class LoggerScopeExtensionsBenchmarks
{
    private ILogger _logger = null!;
    private InMemoryLogSink _sink = null!;

    [GlobalSetup]
    public void Setup()
    {
        _sink = new InMemoryLogSink();
        _logger = TestLoggerFactory.CreateTestLogger(_sink, LogEventLevel.Information);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _sink.Clear();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _sink.Clear();
    }

    // ============================================================
    // Baseline: Logging without Scopes
    // ============================================================

    [Benchmark(Baseline = true)]
    public void LoggingWithoutScope()
    {
        _logger.LogInformation("Operation completed");
    }

    // ============================================================
    // Single Property Scope
    // ============================================================

    [Benchmark]
    public void SinglePropertyScope_TraditionalBeginScope()
    {
        using (_logger.BeginScope(new Dictionary<string, object?> { ["TenantId"] = "tenant-123" }))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void SinglePropertyScope_ExtensionMethod()
    {
        using (_logger.BeginPropertyScope("TenantId", "tenant-123"))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void TenantScope_SpecializedMethod()
    {
        using (_logger.BeginTenantScope("tenant-123"))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    // ============================================================
    // Multiple Property Scopes
    // ============================================================

    [Benchmark]
    public void TwoPropertyScope_TraditionalBeginScope()
    {
        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["TenantId"] = "tenant-123",
            ["UserId"] = "user-456"
        }))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void TwoPropertyScope_TupleOverload()
    {
        using (_logger.BeginPropertyScope(
            ("TenantId", "tenant-123"),
            ("UserId", "user-456")))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void TenantUserScope_SpecializedMethod()
    {
        using (_logger.BeginTenantUserScope("tenant-123", "user-456"))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    // ============================================================
    // Three Property Scopes
    // ============================================================

    [Benchmark]
    public void ThreePropertyScope_TraditionalBeginScope()
    {
        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["TenantId"] = "tenant-123",
            ["UserId"] = "user-456",
            ["OperationId"] = "op-789"
        }))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void ThreePropertyScope_TupleOverload()
    {
        using (_logger.BeginPropertyScope(
            ("TenantId", "tenant-123"),
            ("UserId", "user-456"),
            ("OperationId", "op-789")))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    // ============================================================
    // Variable Property Scopes (params)
    // ============================================================

    [Benchmark]
    public void VariablePropertyScope_ParamsOverload_TwoProperties()
    {
        using (_logger.BeginPropertyScope(
            ("TenantId", "tenant-123"),
            ("UserId", "user-456")))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void VariablePropertyScope_ParamsOverload_FiveProperties()
    {
        using (_logger.BeginPropertyScope(
            ("TenantId", "tenant-123"),
            ("UserId", "user-456"),
            ("OperationId", "op-789"),
            ("RequestId", "req-abc"),
            ("SessionId", "sess-xyz")))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    // ============================================================
    // Operation Scope
    // ============================================================

    [Benchmark]
    public void OperationScope_WithoutId()
    {
        using (_logger.BeginOperationScope("ProcessPayment"))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void OperationScope_WithId()
    {
        using (_logger.BeginOperationScope("ProcessPayment", "op-12345"))
        {
            _logger.LogInformation("Operation completed");
        }
    }

    // ============================================================
    // Nested Scopes
    // ============================================================

    [Benchmark]
    public void NestedScopes_TwoLevels()
    {
        using (_logger.BeginTenantScope("tenant-123"))
        {
            using (_logger.BeginUserScope("user-456"))
            {
                _logger.LogInformation("Operation completed");
            }
        }
    }

    [Benchmark]
    public void NestedScopes_ThreeLevels()
    {
        using (_logger.BeginTenantScope("tenant-123"))
        {
            using (_logger.BeginUserScope("user-456"))
            {
                using (_logger.BeginOperationScope("ProcessOrder", "op-789"))
                {
                    _logger.LogInformation("Operation completed");
                }
            }
        }
    }

    // ============================================================
    // Multiple Logs within Scope
    // ============================================================

    [Benchmark]
    public void TenantScope_WithMultipleLogs()
    {
        using (_logger.BeginTenantScope("tenant-123"))
        {
            _logger.LogInformation("Starting operation");
            _logger.LogInformation("Processing data");
            _logger.LogInformation("Operation completed");
        }
    }

    [Benchmark]
    public void TenantUserScope_WithMultipleLogs()
    {
        using (_logger.BeginTenantUserScope("tenant-123", "user-456"))
        {
            _logger.LogInformation("Starting operation");
            _logger.LogInformation("Processing data");
            _logger.LogInformation("Operation completed");
        }
    }

    // ============================================================
    // High-Frequency Scope Creation
    // ============================================================

    [Benchmark]
    public void HighFrequency_100ScopeCreations()
    {
        for (int i = 0; i < 100; i++)
        {
            using (_logger.BeginTenantScope($"tenant-{i}"))
            {
                _logger.LogInformation("Processing item {ItemId}", i);
            }
        }
    }
}
