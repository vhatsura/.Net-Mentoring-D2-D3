using System;
using System.Globalization;
using NUnit.Framework;

namespace InteroperatingWithUnmanagedCode.Tests
{
    [TestFixture]
    public class PowerStateManagerTests
    {
        [Test]
        public void GetLastSleepTimeTest()
        {
            // Arrange
            var powerManager = new PowerStateManager();

            // Act
            var date = powerManager.GetLastSleepTime();

            // Assert
            Console.WriteLine($"Last Sleep Time in UTC: {date:u}. In Local Time: {date.ToLocalTime()}");
        }

        [Test]
        public void GetLastWakeTimeTest()
        {
            // Arrange
            var powerManager = new PowerStateManager();

            // Act
            var date = powerManager.GetLastWakeTime();

            // Assert
            Console.WriteLine($"Last Wake Time in UTC: {date:u}. In Local Time: {date.ToLocalTime()}");
        }
    }
}
