using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExanimaTools.Models
{
    public interface ILoggingService
    {
        void Log(string message);
        void LogOperation(string operation, string? details = null);
        void LogError(string message, Exception? ex = null);
    }

    public class FileLoggingService : ILoggingService, IDisposable
    {
        private readonly string _logDirectory;
        private readonly string _logFilePath;
        private readonly object _lock = new object();
        private readonly ConcurrentQueue<string> _queue = new();
        private readonly Task _flushTask;
        private bool _disposed;

        public FileLoggingService(string? logDirectory = null)
        {
            _logDirectory = logDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(_logDirectory);
            CleanupOldLogs();
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            _logFilePath = Path.Combine(_logDirectory, $"log_{timestamp}.txt");
            _flushTask = Task.Run(FlushLoop);
        }

        public void Log(string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            _queue.Enqueue(line);
        }

        public void LogOperation(string operation, string? details = null)
        {
            Log($"OPERATION: {operation}{(details != null ? $" | {details}" : "")}");
        }

        public void LogError(string message, Exception? ex = null)
        {
            var errorLine = $"[ERROR] [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            if (ex != null)
                errorLine += $"\nException: {ex.Message}\n{ex.StackTrace}";
            _queue.Enqueue(errorLine);
        }

        private async Task FlushLoop()
        {
            while (!_disposed)
            {
                await Task.Delay(500);
                Flush();
            }
        }

        private void Flush()
        {
            if (_queue.IsEmpty) return;
            lock (_lock)
            {
                using var sw = new StreamWriter(_logFilePath, append: true, Encoding.UTF8);
                while (_queue.TryDequeue(out var line))
                {
                    sw.WriteLine(line);
                }
            }
        }

        private void CleanupOldLogs()
        {
            var files = Directory.GetFiles(_logDirectory, "log_*.txt")
                .OrderByDescending(f => f)
                .ToList();
            foreach (var file in files.Skip(5))
            {
                try { File.Delete(file); } catch { }
            }
        }

        public void Dispose()
        {
            _disposed = true;
            Flush();
        }
    }
}
