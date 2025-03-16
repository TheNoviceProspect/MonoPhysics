using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoPhysics.Core
{
    public class Logger : IDisposable
    {
        private readonly string? _logFile;
        private readonly bool _useConsole;
        private static readonly object _lock = new();
        private readonly int _maxLogFiles;
        private bool _disposed;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        public Logger(string? logFile = null, bool useConsole = true, int maxLogFiles = 5)
        {
            _logFile = logFile;
            _useConsole = useConsole;
            _maxLogFiles = maxLogFiles;
            if (_useConsole && !Console.IsOutputRedirected)
            {
                AllocConsole();
            }
            RollLogs();
        }

        public enum LogLevel
        {
            DEBUG,
            INFO,
            WARNING,
            ERROR,
            CRITICAL
        }

        private void RollLogs()
        {
            if (_logFile == null) return;

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var baseName = Path.GetFileNameWithoutExtension(_logFile);
            var extension = Path.GetExtension(_logFile);

            // Roll existing logs
            for (int i = _maxLogFiles - 1; i >= 1; i--)
            {
                var oldFile = Path.Combine(baseDir, $"{baseName}.{i}{extension}");
                var newFile = Path.Combine(baseDir, $"{baseName}.{i + 1}{extension}");

                if (File.Exists(oldFile))
                {
                    if (File.Exists(newFile))
                    {
                        File.Delete(newFile);
                    }
                    File.Move(oldFile, newFile);
                }
            }

            // Handle the current log file
            var currentLog = Path.Combine(baseDir, _logFile);
            if (File.Exists(currentLog))
            {
                var firstRollover = Path.Combine(baseDir, $"{baseName}.1{extension}");
                if (File.Exists(firstRollover))
                {
                    File.Delete(firstRollover);
                }
                File.Move(currentLog, firstRollover);
            }

            // Clean up any logs beyond the max count
            for (int i = _maxLogFiles + 1; ; i++)
            {
                var oldLog = Path.Combine(baseDir, $"{baseName}.{i}{extension}");
                if (File.Exists(oldLog))
                {
                    File.Delete(oldLog);
                }
                else
                {
                    break;
                }
            }
        }

        private static ConsoleColor GetColorForLevel(LogLevel level) => level switch
        {
            LogLevel.DEBUG => ConsoleColor.Gray,
            LogLevel.INFO => ConsoleColor.Green,
            LogLevel.WARNING => ConsoleColor.Yellow,
            LogLevel.ERROR => ConsoleColor.Red,
            LogLevel.CRITICAL => ConsoleColor.DarkRed,
            _ => ConsoleColor.White
        };

        public void Log(LogLevel level, string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [{level}] [{Path.GetFileName(sourceFilePath)}:{lineNumber}] [{memberName}] {message}";

            lock (_lock)
            {
                if (_useConsole)
                {
                    var originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = GetColorForLevel(level);
                    Console.Write($"[{level}] ");
                    Console.ForegroundColor = originalColor;
                    Console.WriteLine(message);
                }

                if (_logFile != null)
                {
                    var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFile);
                    File.AppendAllText(logPath, logMessage + Environment.NewLine);
                }
            }
        }

        public void ConsoleOnlyLog(string message)
        {
            lock (_lock)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ForegroundColor = originalColor;
            }
        }

        // Convenience methods
        public void Debug(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int lineNumber = 0)
#if DEBUG
            => Log(LogLevel.DEBUG, message, memberName, sourceFilePath, lineNumber);

#else
            => {};
#endif

        public void Info(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int lineNumber = 0)
            => Log(LogLevel.INFO, message, memberName, sourceFilePath, lineNumber);

        public void Warning(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int lineNumber = 0)
            => Log(LogLevel.WARNING, message, memberName, sourceFilePath, lineNumber);

        public void Error(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int lineNumber = 0)
            => Log(LogLevel.ERROR, message, memberName, sourceFilePath, lineNumber);

        public void Critical(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int lineNumber = 0)
            => Log(LogLevel.CRITICAL, message, memberName, sourceFilePath, lineNumber);

        public void Dispose()
        {
            if (_disposed) return;

            if (_useConsole && !Console.IsOutputRedirected)
            {
                FreeConsole();
            }

            _disposed = true;
        }
    }
}