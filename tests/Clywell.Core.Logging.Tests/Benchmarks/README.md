# Performance Benchmarks

This directory contains comprehensive performance benchmarks for the Clywell.Core.Logging package using [BenchmarkDotNet](https://benchmarkdotnet.org/).

## Overview

The benchmarks measure the performance characteristics of key logging features:

1. **LoggerScopeExtensionsBenchmarks** - Performance overhead of logging scopes
2. **SensitiveDataRedactionBenchmarks** - Performance impact of sensitive data redaction

## Running Benchmarks

### Prerequisites

- .NET 9.0 or later SDK installed
- Build the project in **Release** mode (benchmarks must run in Release configuration)

### Using the Benchmark Runner (Recommended)

The easiest way to run benchmarks is using the included benchmark runner console application:

```bash
cd tests/Clywell.Core.Logging.BenchmarkRunner
dotnet run -c Release
```

This will show an interactive menu where you can select which benchmarks to run:
```
Select which benchmarks to run:
  1. LoggerScopes (Scope creation performance)
  2. SensitiveDataRedaction (Redaction pattern performance)
  3. All Benchmarks
  4. Exit
```

**Advanced Usage:**

```bash
# Run specific benchmark suite
dotnet run -c Release -- --filter "*LoggerScopeExtensionsBenchmarks*"

# Export results
dotnet run -c Release -- --exporters json html csv
```

See [BenchmarkRunner README](../../Clywell.Core.Logging.BenchmarkRunner/README.md) for more options.

## Understanding the Results

### Key Metrics

- **Mean**: Average execution time across all iterations
- **Error**: Half of 99.9% confidence interval
- **StdDev**: Standard deviation of all measurements
- **Median**: Middle value of all measurements
- **Allocated**: Memory allocated per operation

### Speed Comparison

BenchmarkDotNet will show speed ratios comparing each method to the baseline:

```
| Method                    |     Mean |   Ratio | Allocated |
|-------------------------- |---------:|--------:|----------:|
| Baseline                  | 1.234 μs |    1.00 |     320 B |
| SourceGenerated           | 0.987 μs |    0.80 |     256 B |  <-- 20% faster, 20% less memory
```

### Interpreting Ratios

- **Ratio < 1.00**: Faster than baseline
- **Ratio = 1.00**: Same speed as baseline
- **Ratio > 1.00**: Slower than baseline

## Benchmark Descriptions

### LoggerScopeExtensionsBenchmarks

Measures the performance overhead of different scope creation methods.

**Key Scenarios Tested:**
- Single property scopes
- Multiple property scopes (2-5 properties)
- Specialized scope methods (tenant, user, operation)
- Nested scopes
- High-frequency scope creation

**Expected Results:**
- Specialized methods (`BeginTenantScope`) should be **5-15% faster** than generic dictionary creation
- Tuple-based overloads should have minimal overhead
- Nested scopes have cumulative overhead

### SensitiveDataRedactionBenchmarks

Measures the performance impact of sensitive data redaction patterns.

**Key Scenarios Tested:**
- Messages with no sensitive data (fast path)
- Messages with credit cards, emails, SSNs
- Multiple pattern matches
- Custom vs default policies
- Minimal policies (fewer patterns = better performance)

**Expected Results:**
- Messages without sensitive data: **~10-50 μs** per redaction
- Messages with matches: **~50-200 μs** per redaction
- Minimal policies (fewer patterns) are **20-40% faster**
- Multiple pattern matches have cumulative cost

## Performance Targets

Based on typical production workloads:

### Scope Creation
- **Target**: < 500 nanoseconds overhead per scope
- **Nested scopes**: Linear overhead increase
- **Multiple logs per scope**: Amortized cost

### Sensitive Data Redaction
- **No matches**: < 50 microseconds per message
- **With matches**: < 200 microseconds per message
- **Multiple patterns**: < 500 microseconds per message

## Optimization Tips

### For Application Developers

1. **Reuse scopes** for multiple log statements:
   ```csharp
   // ✅ Preferred - scope overhead amortized
   using (logger.BeginTenantScope(tenantId))
   {
       logger.LogInformation("Operation 1 completed");
       logger.LogInformation("Operation 2 completed");
       logger.LogInformation("Operation 3 completed");
   }
   
   // ❌ Slower - creates 3 scopes
   logger.BeginTenantScope(tenantId).Dispose();
   logger.LogInformation("Operation 1 completed");
   ```

3. **Use minimal redaction policies** if you know what patterns you need:
   ```csharp
   // ✅ Faster - only checks credit cards
   var policy = new SensitiveDataRedactionPolicyConfiguration()
       .DisableDefaultPatterns()
       .AddPattern(creditCardRegex, "[REDACTED-CC]")
       .Build();
   
   // ❌ Slower - checks all default patterns
   var policy = SensitiveDataRedactionPolicy.Default;
   ```

4. **Always check IsEnabled** for verbose logging:
   ```csharp
   // ✅ Avoids string formatting when disabled
   if (logger.IsEnabled(LogLevel.Debug))
   {
       logger.LogDebug("Expensive operation: {Data}", ExpensiveToString());
   }
   ```

### For Package Maintainers

1. Consider adding more source-generated methods for common patterns
2. Cache compiled regex patterns in redaction policies
3. Use `[LoggerMessage]` for all high-frequency logging operations
4. Profile real-world workloads to identify bottlenecks

## Continuous Performance Monitoring

### Running in CI/CD

Add benchmark execution to your CI pipeline:

```yaml
# Example GitHub Actions
- name: Run Benchmarks
  run: |
    cd tests/Clywell.Core.Logging.Tests
    dotnet run -c Release --exporters json
    
- name: Upload Results
  uses: actions/upload-artifact@v3
  with:
    name: benchmark-results
    path: BenchmarkDotNet.Artifacts/results/*.json
```

### Detecting Performance Regressions

Compare benchmark results between commits:

```bash
# Run baseline benchmarks
git checkout main
dotnet run -c Release --exporters json
mv BenchmarkDotNet.Artifacts/results results-baseline

# Run current benchmarks
git checkout feature-branch
dotnet run -c Release --exporters json
mv BenchmarkDotNet.Artifacts/results results-current

# Compare results manually or with tools
```

## Troubleshooting

### Benchmark Results Are Inconsistent

- Ensure you're running in **Release** mode
- Close other applications to reduce CPU/memory noise
- Run benchmarks multiple times and look at median values
- Use `[SimpleJob(warmupCount: 5, iterationCount: 10)]` for longer runs

### Out of Memory Errors

- Reduce iteration counts with `[SimpleJob(iterationCount: 3)]`
- Run benchmarks individually rather than all at once
- Increase available memory for the process

### Benchmarks Take Too Long

- Run specific benchmark classes: `--filter "*LogMessages*"`
- Reduce warmup and iteration counts (less accurate but faster)
- Comment out expensive benchmarks temporarily

## Resources

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/articles/overview.html)
- [.NET Performance Best Practices](https://learn.microsoft.com/en-us/dotnet/framework/performance/performance-tips)
- [LoggerMessage Source Generators](https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator)

## Contributing

When adding new features to the logging package:

1. Add corresponding benchmarks to measure performance impact
2. Document expected performance characteristics
3. Run benchmarks before and after changes
4. Include benchmark results in pull requests for significant changes

---

**Questions?** Open an issue on the project repository with the `performance` label.
