using Medseek.Util.Extensions.Strings;
using NUnit.Framework;
using System;


namespace Medseek.Util.Extensions
{
    [TestFixture]
    public class StringsTests
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
        public void TestFormatWithUsingStrings()
        {
            var value = "{0} {1} {2}".FormatWith("zero", "one", "two");
            Assert.AreEqual(value, "zero one two");
        }

        [Test]
        public void TestFormatWithUsingInts()
        {
            var value = "{0} {1} {2}".FormatWith(0, 1, 2);
            Assert.AreEqual(value, "0 1 2");
        }

        [Test]
        public void TestFormatWithUsingObjectsWithNoToString()
        {
            var foo1 = new Foo() {Bar = "1"};
            var value = "{0} {1} {2}".FormatWith(foo1, 1, 2);
            Assert.AreEqual(value, foo1.ToString() + " 1 2");
        }

        [Test]
        public void TestFormatWithUsingObjectsWithToString()
        {
            var foo1 = new Baz() { Bar = "1" };
            var value = "{0} {1} {2}".FormatWith(foo1, 1, 2);
            Assert.AreEqual(value,  "1 1 2");
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ThrowsExceptionWhenWrongParamCount()
        {
            var value = "{0} {1} {2}".FormatWith(1, 2);  
        }

        [Test]
        public void TestIsNullOrEmptyWithValue()
        {
            Assert.False("goo".IsNullOrEmpty());
        }

        [Test]
        public void TestIsNullOrEmptyWithNull()
        {
            string nullString = null;
            Assert.True(nullString.IsNullOrEmpty());
        }

        [Test]
        public void TestIsNullOrEmptyWithEmpty()
        {
            Assert.True(string.Empty.IsNullOrEmpty());
        }

        [Test]
        public void TestIsNullOrWhitespaceWithValue()
        {
            Assert.False("goo".IsNullOrWhitespace());
        }

        [Test]
        public void TestIsNullOrWhitespaceWithNull()
        {
            string nullString = null;
            Assert.True(nullString.IsNullOrWhitespace());
        }

        [Test]
        public void TestIsNullOrWhitespaceWithEmpty()
        {
            Assert.True(string.Empty.IsNullOrWhitespace());
        }

        [Test]
        public void TestIsNullOrWhitespaceWithWhitespace()
        {
            Assert.True("    ".IsNullOrWhitespace());
        }

        [Test]
        public void TestIsNullOrWhitespaceWithTab()
        {
            Assert.True("\t".IsNullOrWhitespace());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGuardThrowsExceptionWithNull()
        {
            string nullstring = null;
            Strings.Strings.Guard(nullstring, "nullstring");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGuardThrowsExceptionWithEmpty()
        {
            Strings.Strings.Guard(string.Empty, "foo");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGuardThrowsExceptionWithWhitespace()
        {
            Strings.Strings.Guard(" ", "foo");
        }

        [Test]
        public void TestExceptionMessageContainsStringName()
        {
            try
            {
                Strings.Strings.Guard(" ", "foo");
            }
            catch (ArgumentException e)
            {
               Assert.True(e.Message.Contains("foo"));
            }
            
        }

        [Test]
        public void TestExceptionMessageContainsProperFormat()
        {
            try
            {
                Strings.Strings.Guard(" ", "foo");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.Message, "foo cannot be null or whitespace.\r\nParameter name: foo");
                Assert.AreEqual(e.ParamName, "foo");
            }
        }

        [Test]
        public void TestExceptionMessageContainsProperFormatWithSuppliedFormat()
        {
            try
            {
                Strings.Strings.Guard(" ", "foo", "{0} bar baz");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual(e.Message, "foo bar baz\r\nParameter name: foo");
                Assert.AreEqual(e.ParamName, "foo");
            }
        }
    }
}
