namespace Medseek.Util.Messaging
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MqAddress"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MqAddressTests
    {
        /// <summary>
        /// Verifies that the address equality members work correctly.
        /// </summary>
        [Test]
        public void EqualityMembersWithMqAdressOfSameValue()
        {
            var address1 = New();
            var address2 = New(address1.Value);
            Assume.That(address1.Value, Is.EqualTo(address2.Value));

            var equalsResult = address1.Equals(address2);
            Assert.That(equalsResult, Is.True);

            var getHashCodeResult1 = address1.GetHashCode();
            var getHashCodeResult2 = address2.GetHashCode();
            Assert.That(getHashCodeResult1, Is.EqualTo(getHashCodeResult2));
        }

        /// <summary>
        /// Verifies that the address equality members work correctly.
        /// </summary>
        [Test]
        public void EqualityMembersWithMqAdressOfDifferentValue()
        {
            var address1 = New();
            var address2 = New();
            Assume.That(address1.Value, Is.Not.EqualTo(address2.Value));

            var equalsResult = address1.Equals(address2);
            Assert.That(equalsResult, Is.False);

            var getHashCodeResult1 = address1.GetHashCode();
            var getHashCodeResult2 = address2.GetHashCode();
            Assert.That(getHashCodeResult1, Is.Not.EqualTo(getHashCodeResult2));
        }

        private static MqAddress New(string value = null)
        {
            var result = new MqAddress(value ?? Guid.NewGuid().ToString());
            return result;
        }
    }
}