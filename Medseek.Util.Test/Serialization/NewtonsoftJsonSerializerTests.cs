namespace Medseek.Util.Serialization
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Medseek.Util.Messaging;
    using Medseek.Util.Serialization.Newtonsoft.Json;
    using Medseek.Util.Testing;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="NewtonsoftJsonSerializer"/> class.
    /// </summary>
    [TestFixture]
    public sealed class NewtonsoftJsonSerializerTests : TestFixture<NewtonsoftJsonSerializer>
    {
        /// <summary>
        /// Verifies that message properties can be serialized and then 
        /// deserialized without losing any information.
        /// </summary>
        [Test]
        public void CanSerializeAndDeserializeMessagePropertiesWithNoLoss()
        {
            var original = new MessageProperties
            {
                ContentType = "application/xml",
                CorrelationId = "11",
                ReplyTo = new MqAddress("topic://medseek-api/test.of.json.serializer.with.messageproperties.replyto"),
                AdditionalProperties = new Dictionary<string, object>
                {
                    { "key1", "value1" },
                    { "key2", "value2" },
                    { "key3", 3 },
                }
            };

            MessageProperties result;
            using (var ms = new MemoryStream())
            {
                Obj.Serialize(typeof(MessageProperties), original, ms);
                ms.Position = 0;

                var text = Encoding.Default.GetString(ms.ToArray());
                Debug.WriteLine(text);

                result = (MessageProperties)Obj.Deserialize(typeof(MessageProperties), ms);
            }
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ContentType, Is.EqualTo(original.ContentType));
            Assert.That(result.CorrelationId, Is.EqualTo(original.CorrelationId));
            Assert.That(result.ReplyTo.Value, Is.EqualTo(original.ReplyTo.Value));
            foreach (var item in original.AdditionalProperties)
                Assert.That(result.AdditionalProperties[item.Key], Is.EqualTo(item.Value));
        }
    }
}