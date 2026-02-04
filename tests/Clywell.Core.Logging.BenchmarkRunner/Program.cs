using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Clywell.Core.Logging.Tests.Benchmarks;

Console.WriteLine("===========================================");
Console.WriteLine("Clywell.Core.Logging Performance Benchmarks");
Console.WriteLine("===========================================");
Console.WriteLine();

if (args.Length > 0)
{
    // Run with command-line arguments (for advanced usage)
    var config = DefaultConfig.Instance
        .WithOptions(ConfigOptions.DisableOptimizationsValidator);

    BenchmarkSwitcher.FromAssembly(typeof(LoggerScopeExtensionsBenchmarks).Assembly).Run(args, config);
}
else
{
    // Interactive menu
    Console.WriteLine("Select which benchmarks to run:");
    Console.WriteLine("  1. LoggerScopes (Scope creation performance)");
    Console.WriteLine("  2. SensitiveDataRedaction (Redaction pattern performance)");
    Console.WriteLine("  3. All Benchmarks");
    Console.WriteLine("  4. Exit");
    Console.WriteLine();
    Console.Write("Enter choice (1-4): ");

    var choice = Console.ReadLine();

    var config = DefaultConfig.Instance
        .WithOptions(ConfigOptions.DisableOptimizationsValidator);

    switch (choice)
    {
        case "1":
            Console.WriteLine("\nRunning LoggerScopes benchmarks...\n");
            BenchmarkRunner.Run<LoggerScopeExtensionsBenchmarks>(config);
            break;

        case "2":
            Console.WriteLine("\nRunning SensitiveDataRedaction benchmarks...\n");
            BenchmarkRunner.Run<SensitiveDataRedactionBenchmarks>(config);
            break;

        case "3":
            Console.WriteLine("\nRunning ALL benchmarks...\n");
            BenchmarkRunner.Run<LoggerScopeExtensionsBenchmarks>(config);
            Console.WriteLine("\n--- Press any key to continue to next benchmark suite ---");
            Console.ReadKey();

            BenchmarkRunner.Run<SensitiveDataRedactionBenchmarks>(config);
            break;

        case "4":
            Console.WriteLine("Exiting...");
            return 0;

        default:
            Console.WriteLine("Invalid choice. Exiting...");
            return 1;
    }
}

Console.WriteLine();
Console.WriteLine("===========================================");
Console.WriteLine("Benchmarks Complete!");
Console.WriteLine("===========================================");
Console.WriteLine();
Console.WriteLine("Results are saved in: BenchmarkDotNet.Artifacts/results/");
Console.WriteLine();

return 0;
