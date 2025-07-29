// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using Microsoft.Extensions.DependencyInjection;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using ExanimaTools.ViewModels;
using System;
using System.IO;

namespace ExanimaTools.Services
{
    /// <summary>
    /// Configures dependency injection services for the ExanimaTools application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all ExanimaTools services with the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddExanimaToolsServices(this IServiceCollection services)
        {
            // Logging Service - Singleton to maintain log file handles
            services.AddSingleton<ILoggingService>(provider =>
            {
                var logDir = Path.Combine(Environment.CurrentDirectory, "logs");
                return new FileLoggingService(logDir);
            });

            // Repository Services - Scoped for proper resource management
            services.AddScoped<EquipmentRepository>(provider =>
            {
                var logger = provider.GetRequiredService<ILoggingService>();
                var dbPath = DbManager.GetDbPath();
                return new EquipmentRepository($"Data Source={dbPath}", logger);
            });

            services.AddScoped<ArsenalRepository>(provider =>
            {
                var dbPath = DbManager.GetDbPath();
                return new ArsenalRepository($"Data Source={dbPath}");
            });

            services.AddScoped<CompanyMemberRepository>(provider =>
            {
                var dbPath = DbManager.GetDbPath();
                return new CompanyMemberRepository($"Data Source={dbPath}");
            });

            // ViewModels - Transient for new instances per request
            services.AddTransient<CompanyViewModel>();
            services.AddTransient<ArsenalManagerViewModel>();
            services.AddTransient<EquipmentManagerViewModel>();

            // Main Window - Transient for DI support
            services.AddTransient<MainWindow>();

            return services;
        }
    }
}
