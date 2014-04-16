namespace Medseek.Util.Serialization
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class TestObject
    {
        [DataMember]
        public bool Bool { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public string String { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }

        [DataMember]
        public TestEnum Enum { get; set; }
   }

    [DataContract]
    public enum TestEnum
    {
        /// <summary>
        /// The one member.
        /// </summary>
        [EnumMember]
        One,

        /// <summary>
        /// The two member.
        /// </summary>
        [EnumMember]
        Two,

        /// <summary>
        /// The three member.
        /// </summary>
        [EnumMember]
        Three
    }
}