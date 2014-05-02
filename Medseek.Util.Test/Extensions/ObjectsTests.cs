using Medseek.Util.Extensions.Objects;
using NUnit.Framework;
using System;


namespace Medseek.Util.Extensions
{
    [TestFixture]
    public class ObjectsTests
    {
        internal class Foo
        {
            internal string Bar;
        }

        internal class Baz
        {
            internal string Bar;

            public override string ToString()
            {
                return Bar;
            }
        }
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGuardThrowsExceptionWithNull()
        {
            Foo nullstring = null;
            nullstring.Guard("nullstring");
        }

        [Test]
        public void TestExceptionMessageContainsStringName()
        {
            Foo nullfoo = null;
            try
            {
                nullfoo.Guard("foo");
            }
            catch (ArgumentNullException e)
            {
                Assert.True(e.Message.Contains("foo"));
            }

        }

        [Test]
        public void TestExceptionMessageContainsProperFormat()
        {
            Foo nullfoo = null;
            try
            {
                nullfoo.Guard("foo");
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual(e.Message, "Value cannot be null.\r\nParameter name: foo");
                Assert.AreEqual(e.ParamName, "foo");
            }
        }

        [Test]
        public void TestExceptionMessageContainsProperFormatWithSuppliedFormat()
        {
            Foo nullfoo = null;
            try
            {
                nullfoo.Guard("foo", "{0} bar baz");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.Message, "foo bar baz\r\nParameter name: foo");
                Assert.AreEqual(e.ParamName, "foo");
            }
        }
    }
}
