namespace Medseek.Util.Extensions
{
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="TypeExt" /> class.
    /// </summary>
    [TestFixture]
    public sealed class TypeExtTests
    {
        private Mock<IHelper> helper;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            helper = new Mock<IHelper>();
        }

        /// <summary>
        /// Verifies that the method information is returned.
        /// </summary>
        [Test]
        public void GetMethodByActionOfTReturnsMethodInfo()
        {
            var result = TypeExt.GetMethod<IHelper>(x => x.Invoke1(null));
            Assert.That(result, Is.Not.Null.With.Property("Name").EqualTo("Invoke1"));
        }

        /// <summary>
        /// Verifies that the method information is returned.
        /// </summary>
        [Test]
        public void GetMethodByActionReturnsMethodInfo()
        {
            var result = TypeExt.GetMethod<IHelper>(x => x.Invoke0());
            Assert.That(result, Is.Not.Null.With.Property("Name").EqualTo("Invoke0"));
        }

        /// <summary>
        /// Verifies that the method information is returned.
        /// </summary>
        [Test]
        public void GetMethodByObjActionOfTReturnsMethodInfo()
        {
            var result = helper.Object.GetMethod(x => x.Invoke1(null));
            Assert.That(result, Is.Not.Null.With.Property("Name").EqualTo("Invoke1"));
        }

        /// <summary>
        /// Verifies that the method information is returned.
        /// </summary>
        [Test]
        public void GetMethodByObjActionReturnsMethodInfo()
        {
            var result = helper.Object.GetMethod(x => x.Invoke0());
            Assert.That(result, Is.Not.Null.With.Property("Name").EqualTo("Invoke0"));
        }

        /// <summary>
        /// Verifies that the method information is returned.
        /// </summary>
        [Test]
        public void GetMethodByObjFuncOfTReturnsMethodInfo()
        {
            var result = helper.Object.GetMethod(x => x.Invoke1Return(null));
            Assert.That(result, Is.Not.Null.With.Property("Name").EqualTo("Invoke1Return"));
        }

        /// <summary>
        /// Verifies that the method information is returned.
        /// </summary>
        [Test]
        public void GetMethodByObjFuncReturnsMethodInfo()
        {
            var result = helper.Object.GetMethod(x => x.Invoke0Return());
            Assert.That(result, Is.Not.Null.With.Property("Name").EqualTo("Invoke0Return"));
        }

        /// <summary>
        /// Helper interface used for verifying extension behavior.
        /// </summary>
        public interface IHelper
        {
            void Invoke0();
            object Invoke0Return();
            void Invoke1(object a);
            object Invoke1Return(object a);
        }
    }
}