using ExanimaTools.Controls;
using ExanimaTools.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExanimaTools.Models;

namespace ETModels.Tests
{
    [TestClass]
    public class PipDisplayViewModelTests
    {
        [TestMethod]
        public void ClickingPip_UpdatesValue_And_Logs()
        {
            // Arrange
            bool logCalled = false;
            var logger = new TestLogger(() => logCalled = true);
            var statVm = new StatPipViewModel(StatType.Coverage, 0.5f, _ => { }, logger);

            // Act
            statVm.PipDisplayViewModel.SetValueFromPip(2, false); // Simulate clicking the 3rd full pip

            // Assert
            Assert.AreEqual(3.0f, statVm.Value);
            Assert.IsTrue(logCalled);
        }

        [TestMethod]
        public void PipDisplayViewModel_SetValueFromPip_UpdatesValueAndFiresCallback()
        {
            // Arrange
            float callbackValue = -1;
            var pipVm = new PipDisplayViewModel(v => callbackValue = v);
            pipVm.Value = 0.5f;

            // Act: simulate clicking the 4th full pip (index 3)
            pipVm.SetValueFromPip(3, false);

            // Assert
            Assert.AreEqual(4.0f, pipVm.Value);
            Assert.AreEqual(4.0f, callbackValue);
        }

        [TestMethod]
        public void StatPipViewModel_PipClick_UpdatesStatValueAndLogs()
        {
            // Arrange
            float statValue = -1;
            bool logCalled = false;
            var logger = new TestLogger(() => logCalled = true);
            var statVm = new StatPipViewModel(StatType.Coverage, 0.5f, v => statValue = v, logger);

            // Act: simulate clicking the 2nd half pip (index 1, isHalf=true)
            statVm.PipDisplayViewModel.SetValueFromPip(1, true);

            // Assert
            Assert.AreEqual(1.5f, statVm.Value);
            Assert.AreEqual(1.5f, statValue);
            Assert.IsTrue(logCalled);
        }

        private class TestLogger : ILoggingService
        {
            private readonly System.Action _onLog;
            public TestLogger(System.Action onLog) => _onLog = onLog;
            public void Log(string message) => _onLog();
            public void LogOperation(string operation, string? details = null) => _onLog();
        }
    }
}
