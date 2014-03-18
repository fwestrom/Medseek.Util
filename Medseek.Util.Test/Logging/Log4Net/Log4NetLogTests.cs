namespace Medseek.Util.Logging.Log4Net
{
    using System;
    using System.Linq.Expressions;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="Log4NetLog"/> class.
    /// </summary>
    [TestFixture]
    public class Log4NetLogTests
    {
        private Mock<log4net.ILog> log;
        private Mock<ILogManager> logManager;
        private ILog obj;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            log = new Mock<log4net.ILog>();
            logManager = new Mock<ILogManager>();
            obj = new Log4NetLog(log.Object, logManager.Object);
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void DebugFormatInvokesLog4Net()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Debug(It.Is<object>(a => a.ToString() == message), null))
                .Verifiable();

            obj.DebugFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void DebugInvokesLog4Net()
        {
            var message = new object();
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.Debug(message, ex))
                .Verifiable();

            obj.Debug(message, ex);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void ErrorFormatInvokesLog4Net()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Error(It.Is<object>(a => a.ToString() == message), null))
                .Verifiable();

            obj.ErrorFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void ErrorInvokesLog4Net()
        {
            var message = new object();
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.Error(message, ex))
                .Verifiable();

            obj.Error(message, ex);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void FatalFormatInvokesLog4Net()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Fatal(It.Is<object>(a => a.ToString() == message), null))
                .Verifiable();

            obj.FatalFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void FatalInvokesLog4Net()
        {
            var message = new object();
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.Fatal(message, ex))
                .Verifiable();

            obj.Fatal(message, ex);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void InfoFormatInvokesLog4Net()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Info(It.Is<object>(a => a.ToString() == message), null))
                .Verifiable();

            obj.InfoFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void InfoInvokesLog4Net()
        {
            var message = new object();
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.Info(message, ex))
                .Verifiable();

            obj.Info(message, ex);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void WarnFormatInvokesLog4Net()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Warn(It.Is<object>(a => a.ToString() == message), null))
                .Verifiable();

            obj.WarnFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the log4net logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void WarnInvokesLog4Net()
        {
            var message = new object();
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.Warn(message, ex))
                .Verifiable();

            obj.Warn(message, ex);

            log.Verify();
        }
        
        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the log4net logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsDebugEnabledReturnsValueFromLog4Net(bool expectedValue)
        {
            Verify(x => x.IsDebugEnabled, x => x.IsDebugEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the log4net logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsErrorEnabledReturnsValueFromLog4Net(bool expectedValue)
        {
            Verify(x => x.IsErrorEnabled, x => x.IsErrorEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the log4net logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsFatalEnabledReturnsValueFromLog4Net(bool expectedValue)
        {
            Verify(x => x.IsFatalEnabled, x => x.IsFatalEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the log4net logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsInfoEnabledReturnsValueFromLog4Net(bool expectedValue)
        {
            Verify(x => x.IsInfoEnabled, x => x.IsInfoEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the log4net logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsWarnEnabledReturnsValueFromLog4Net(bool expectedValue)
        {
            Verify(x => x.IsWarnEnabled, x => x.IsWarnEnabled, expectedValue);
        }

        private void Verify<T>(Func<ILog, T> getResult, Expression<Func<log4net.ILog, T>> propertyExpression, T expectedValue)
        {
            log.Setup(propertyExpression)
                .Returns(expectedValue);

            var result = getResult(obj);

            Assert.That(result, Is.EqualTo(expectedValue));
        }
    }
}
