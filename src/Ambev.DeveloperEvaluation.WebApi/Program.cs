using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen(options =>
            {
                // Include XML comments
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var app = builder.Build();
            
            // Only run migrations in non-Test environments
            if (!app.Environment.IsEnvironment("Test"))
            {
                using var scope = app.Services.CreateScope();
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            
                    // Log para você saber o que está rolando
                    Log.Information("🚀 Starting Database Migration...");

                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        await context.Database.MigrateAsync();
                        Log.Information("✅ Database Migration Completed Successfully!");
                    }
                    else
                    {
                        Log.Information("✅ Database is already up to date.");
                    }

                    // Run Seeding
                    Log.Information("🌱 Starting Database Seeding...");
                    await Common.DbInitializer.InitializeAsync(app.Services);
                    Log.Information("✅ Database Seeding Completed Successfully!");
                }
                catch (Exception ex)
                {
                    // Log Fatal para destacar no console do Docker
                    Log.Fatal(ex, "❌ CRITICAL ERROR: Database migration failed.");
            
                    // IMPORTANTE: Derruba a aplicação para não subir quebrada.
                    // O Docker vai tentar reiniciar automaticamente.
                    throw; 
                }
            }
            
            app.UseMiddleware<ValidationExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
public partial class Program;
