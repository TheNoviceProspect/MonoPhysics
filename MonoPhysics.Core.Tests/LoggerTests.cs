using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonoPhysics.Core;
using System.IO;

[TestClass]
public class LoggerTests
{
    #region Fields

    private Logger? _logger;
    private string _testLogFile = "test.log";

    #endregion Fields

    #region Public Methods

    [TestCleanup]
    public void Cleanup()
    {
        if (_logger != null)
        {
            _logger.Dispose();
            _logger = null;
        }

        // Wait briefly to ensure file handles are released
        Thread.Sleep(100);

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var files = Directory.GetFiles(baseDir, "test*.log");
        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }

    [TestMethod]
    public void Logger_FormatsMessage_WithCorrectLogLevel()
    {
        // Arrange
        _logger = new Logger(_testLogFile, false);

        // Act
        _logger.Error("Test error");

        // Assert
        string content = File.ReadAllText(_testLogFile);
        StringAssert.Contains(content, "[ERROR]");
    }

    [TestMethod]
    public void Logger_WritesToFile_WhenMessageLogged()
    {
        // Arrange
        string uniqueLogFile = $"test_{Guid.NewGuid()}.log";

        // Act
        using (var logger = new Logger(uniqueLogFile, false))
        {
            logger.Info("Test message");
        }

        // Assert
        Assert.IsTrue(File.Exists(uniqueLogFile));
        string content = File.ReadAllText(uniqueLogFile);
        StringAssert.Contains(content, "Test message");

        // Cleanup
        if (File.Exists(uniqueLogFile))
        {
            File.Delete(uniqueLogFile);
        }
    }

    #endregion Public Methods
}