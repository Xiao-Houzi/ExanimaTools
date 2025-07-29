// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using Microsoft.Extensions.DependencyInjection;
using System;

namespace ExanimaTools.Services
{
    /// <summary>
    /// Service locator interface for resolving dependencies in Avalonia controls where constructor injection is not available.
    /// This should be used sparingly and only where constructor injection is not possible.
    /// </summary>
    public interface IServiceLocator
    {
        T GetRequiredService<T>() where T : notnull;
        T? GetService<T>();
    }

    /// <summary>
    /// Implementation of service locator using Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public T GetRequiredService<T>() where T : notnull
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public T? GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
