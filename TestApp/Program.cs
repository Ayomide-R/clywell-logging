using Clywell.Core.Logging.Extensions;
using TestApp.Models;
using TestApp.Services;
using Serilog.Events;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<StudentService>();

// Clywell Logging Configuration
builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Debug)
        .WithConsoleSink()
        .WithClywellDefaults();
});

var app = builder.Build();

// Clywell Middleware
app.UseRequestTracking();
app.UseRequestLogging();

// ─── Minimal API Endpoints ───────────────────────────────────────────────────

app.MapGet("/api/students", (StudentService studentService, ILogger<Program> logger) =>
{
    using (logger.BeginTimedScope("GetAllStudents", new Dictionary<string, object>()))
    {
        logger.Info("Retrieving all students");

        var students = logger.LogExecutionTime("FetchStudentsFromService", () =>
            studentService.GetAll());

        logger.Info("Retrieved {StudentCount} students", students.Count);

        return Results.Ok(students);
    }
});

app.MapGet("/api/students/{id:guid}", (Guid id, StudentService studentService, ILogger<Program> logger) =>
{
    using (logger.BeginTimedScope("GetStudentById", new Dictionary<string, object> { ["StudentId"] = id }))
    {
        var student = studentService.GetById(id);

        if (student is null)
        {
            logger.Warning("Student with ID {StudentId} not found", id);
            return Results.NotFound();
        }

        logger.Info("Found student {StudentName}", student.Name);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.Debug("Full student data: {Json}", JsonSerializer.Serialize(student));
        }

        return Results.Ok(student);
    }
});

// ─────────────────────────────────────────────────────────────────────────────

app.Run();
