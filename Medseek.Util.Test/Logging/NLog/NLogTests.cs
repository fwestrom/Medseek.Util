namespace Medseek.Util.Logging.NLog
{
    using System;
    using System.Linq.Expressions;
    using Moq;
    using NUnit.Framework;
    using ILogger = global::NLog.Interface.ILogger;
    using NLog = global::NLog;
    
    /// <summary>
    /// Test fixture for the <see cref="NLogLog"/> class.
    /// </summary>
    [TestFixture]
    public class NLogTests
    {
        private Mock<ILogManager> logManager;
        private ILog obj;

        private Mock<ILogger> log;
        
        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            logManager = new Mock<ILogManager>();
            log = new Mock<ILogger>();
            obj = new NLogLog(log.Object, logManager.Object);
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void DebugFormatInvokesNLog()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Log(It.Is<Type>(t => true), It.Is<NLog.LogEventInfo>(y => y.Level == NLog.LogLevel.Debug && y.Message == message)))
                .Verifiable();

            obj.DebugFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void DebugInvokesNLog()
        {
            var message = new object();
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.DebugException(message.ToString(), ex))
                .Verifiable();

            obj.Debug(message, ex);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void ErrorInvokesNLog()
        {
            var message = new object();
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.ErrorException(message.ToString(), ex))
                .Verifiable();

            obj.Error(message, ex);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void FatalFormatInvokesNLog()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Log(It.Is<Type>(t => true), It.Is<NLog.LogEventInfo>(y => y.Level == NLog.LogLevel.Fatal && y.Message == message)))
                .Verifiable();

            obj.FatalFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void FatalInvokesNLog()
        {
            var message = string.Empty;
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.FatalException(message, ex))
                .Verifiable();

            obj.Fatal(message, ex);

            log.Verify();
        }


        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void InfoFormatInvokesNLog()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Log(It.Is<Type>(t => true), It.Is<NLog.LogEventInfo>(y => y.Level == NLog.LogLevel.Info && y.Message == message)))
                .Verifiable();

            obj.InfoFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void InfoInvokesNLog()
        {
            var message = string.Empty;
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.InfoException(message, ex))
                .Verifiable();

            obj.Info(message, ex);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void WarnFormatInvokesNLog()
        {
            const string format = "{0}{1}{2}";
            var args = new object[] { 0, 1, 2 };
            var message = string.Format(format, args);
            log.Setup(x =>
                x.Log(It.Is<Type>(t => true), It.Is<NLog.LogEventInfo>(y => y.Level == NLog.LogLevel.Warn && y.Message == message)))
                .Verifiable();

            obj.WarnFormat(format, args);

            log.Verify();
        }

        /// <summary>
        /// Verifies that the logger invokes the NLog logger to write the 
        /// message to the log.
        /// </summary>
        [Test]
        public void WarnInvokesNLog()
        {
            var message = string.Empty;
            var ex = new Exception("Test assertion exception");
            log.Setup(x =>
                x.WarnException(message, ex))
                .Verifiable();

            obj.Warn(message, ex);

            log.Verify();
        }
        
        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the NLog logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsDebugEnabledReturnsValueFromNLog(bool expectedValue)
        {
            Verify(x => x.IsDebugEnabled, x => x.IsDebugEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the NLog logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsErrorEnabledReturnsValueFromNLog(bool expectedValue)
        {
            Verify(x => x.IsErrorEnabled, x => x.IsErrorEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the NLog logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsFatalEnabledReturnsValueFromNLog(bool expectedValue)
        {
            Verify(x => x.IsFatalEnabled, x => x.IsFatalEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the NLog logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsInfoEnabledReturnsValueFromNLog(bool expectedValue)
        {
            Verify(x => x.IsInfoEnabled, x => x.IsInfoEnabled, expectedValue);
        }

        /// <summary>
        /// Verifies that the logger identifies itself as enabled using the 
        /// value from the NLog logger.
        /// </summary>
        /// <param name="expectedValue">
        /// The value to return from the inner logger mock.
        /// </param>
        [Theory]
        public void IsWarnEnabledReturnsValueFromNLog(bool expectedValue)
        {
            Verify(x => x.IsWarnEnabled, x => x.IsWarnEnabled, expectedValue);
        }
        
        private void Verify<T>(Func<ILog, T> getResult, Expression<Func<ILogger, T>> propertyExpression, T expectedValue)
        {
            log.Setup(propertyExpression)
                .Returns(expectedValue);

            var result = getResult(obj);

            Assert.That(result, Is.EqualTo(expectedValue));
        }
    }
}
