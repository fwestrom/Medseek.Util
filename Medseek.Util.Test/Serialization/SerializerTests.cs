﻿namespace Medseek.Util.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Medseek.Util.Serialization.Newtonsoft.Json;

    using NUnit.Framework;

    public class SerializerTests
    {
        private List<TestObject> items;

        [SetUp]
        public void SetUp()
        {
            items = new List<TestObject>
                {
                    new TestObject
                        {
                            Bool = true,
                            DateTime = DateTime.Now,
                            Number = 1,
                            String = string.Empty
                        },
                         new TestObject
                        {
                            Bool = false,
                            DateTime = DateTime.Now.AddDays(3),
                            Number = 12,
                            String = "new value"
                        }
                };
        }

        /// <summary>
        /// Gets the serializers.
        /// </summary>
        /// <value>
        /// The ISerializer list.
        /// </value>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public List<ISerializer> Serializers 
        {
            get
            {
                return new List<ISerializer>
                    {
                        new SystemRuntimeSerializationDataContractSerializer(),
                        new NewtonsoftJsonSerializer()
                    };
            }
        }

        /// <summary>
        /// Is the serializers should serialize data.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        [TestCaseSource("Serializers")]
        public void SerializersShouldSerializeData(ISerializer serializer)
        {
            var ms = new MemoryStream();
            serializer.Serialize(typeof(Exception), new Exception("blow"), ms);
            var data = ms.ToArray();
            Assert.That(data.GetType(), Is.EqualTo(typeof(byte[])));
        }

        [TestCaseSource("Serializers")]
        public void SerializerShouldDeserializeData(ISerializer serializer)
        {
            var ms = new MemoryStream();
            serializer.Serialize(typeof(List<TestObject>), items, ms);
            ms.Position = 0;
            var deserialize = (List<TestObject>)serializer.Deserialize(typeof(List<TestObject>), ms);
            Assert.That(deserialize.Count, Is.EqualTo(items.Count));
        }

        [TestCaseSource("Serializers")]
        public void SerializerShouldBeAbleToDeserializeTypesItSerialized(ISerializer serializer)
        {
            var ms = new MemoryStream();
            serializer.Serialize(typeof(List<TestObject>), items, ms);
            ms.Position = 0;
            var canDeserialize = serializer.CanDeserialize(typeof(List<TestObject>), ms, "application/json")
                                 || serializer.CanDeserialize(typeof(List<TestObject>), ms, "application/xml");
            Assert.That(canDeserialize);
        }

        [Test]
        public void JsonSerializerShouldSerializeEnumAsString()
        {
            var ms = new MemoryStream();
            var testObj = new TestObject();
            var serializer = new NewtonsoftJsonSerializer();
            serializer.Serialize(typeof(TestObject), testObj, ms);
            ms.Position = 0;
            var value = new StreamReader(ms).ReadToEnd();
            Assert.That(value, Contains.Substring("One"));
        }
    }
}
