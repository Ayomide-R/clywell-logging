# Benchmark Runner

This is a console application for running performance benchmarks for the Clywell.Core.Logging package.

## Quick Start

### Run Interactively

```bash
cd tests/Clywell.Core.Logging.BenchmarkRunner
dotnet run -c Release
```

This will show an interactive menu:
```
===========================================
Clywell.Core.Logging Performance Benchmarks
===========================================

Select which benchmarks to run:
  1. LoggerScopes (Scope creation performance)
  2. SensitiveDataRedaction (Redaction pattern performance)
  3. All Benchmarks
  4. Exit

Enter choice (1-4):
```

### Run Specific Benchmark

```bash
# Run LoggerScopes benchmarks
dotnet run -c Release -- --filter "*LoggerScopeExtensionsBenchmarks*"

# Run with specific configuration
dotnet run -c Release -- --job short --filter "*LoggerScopeExtensionsBenchmarks*"
```

### Export Results

```bash
# Export to multiple formats
dotnet run -c Release -- --exporters json html csv markdown

# Export with artifacts folder
dotnet run -c Release -- --artifacts ./benchmark-results
```

## Available Benchmark Suites

### 1. LoggerScope Benchmarks
Measures the performance overhead of different scope creation methods.

**Key Measurements:**
- Single, double, and triple property scopes
- Specialized methods (tenant, user, operation)
- Nested scopes
- High-frequency scope creation

**Expected Results:** Specialized methods should be 5-15% faster than generic dictionary creation.

### 2. SensitiveDataRedaction Benchmarks
Measures the performance impact of sensitive data redaction patterns.

**Key Measurements:**
- Messages with/without sensitive data
- Multiple pattern matches
- Custom vs default policies
- Various message lengths

**Expected Results:** Messages without sensitive data ~10-50μs, with matches ~50-200μs.

## Understanding Results

BenchmarkDotNet will display results in a table format:

```
| Method                    |     Mean |   Ratio | Allocated |
|-------------------------- |---------:|--------:|----------:|
| Baseline                  | 1.234 μs |    1.00 |     320 B |
| SourceGenerated           | 0.987 μs |    0.80 |     256 B |
```

- **Mean**: Average execution time
- **Ratio**: Speed compared to baseline (< 1.00 is faster)
- **Allocated**: Memory allocated per operation

## Advanced Usage

### Custom Job Configuration

```bash
# Run with specific runtime
dotnet run -c Release -- --runtimes net8.0 net9.0

# Run with specific iteration counts
dotnet run -c Release -- --warmupCount 5 --iterationCount 10

# Run with memory profiler
dotnet run -c Release -- --memory
```

### Filter Examples

```bash
# Run all benchmarks containing "Tenant"
dotnet run -c Release -- --filter "*Tenant*"

# Run multiple filters
dotnet run -c Release -- --filter "*SourceGenerated* *Tenant*"

# Exclude specific benchmarks
dotnet run -c Release -- --filter "*" --exclude "*HighFrequency*"
```

## Results Location

Benchmark results are saved to:
```
tests/Clywell.Core.Logging.BenchmarkRunner/BenchmarkDotNet.Artifacts/results/
```

Files include:
- `*.html` - HTML report with charts
- `*.csv` - Raw data for analysis
- `*.json` - Structured results
- `*.md` - Markdown summary

## CI/CD Integration

### GitHub Actions Example

```yaml
- name: Run Benchmarks
  run: |
    cd tests/Clywell.Core.Logging.BenchmarkRunner
    dotnet run -c Release -- --exporters json
    
- name: Upload Results
  uses: actions/upload-artifact@v3
  with:
    name: benchmark-results
    path: tests/Clywell.Core.Logging.BenchmarkRunner/BenchmarkDotNet.Artifacts/
```

## Troubleshooting

### Results Are Inconsistent
- Ensure you're running in **Release** mode (`-c Release`)
- Close other applications
- Run benchmarks multiple times

### Out of Memory
- Run benchmark suites individually (options 1-3)
- Reduce iteration counts: `--iterationCount 3`

### Takes Too Long
- Use `--job short` for quick runs (less accurate)
- Run specific benchmarks with `--filter`

## Performance Optimization Tips

Based on benchmark results:

1. **Use specialized scope methods** for tenant/user/operation scopes (5-15% faster)
2. **Reuse scopes** for multiple log statements
3. **Use minimal redaction policies** if you know what patterns you need

## More Information

For detailed benchmark documentation, see:
- [Benchmark Tests README](../Clywell.Core.Logging.Tests/Benchmarks/README.md)
- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
