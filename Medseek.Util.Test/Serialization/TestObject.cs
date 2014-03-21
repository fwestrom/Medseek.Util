namespace Medseek.Util.Serialization
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    internal class TestObject
    {
        [DataMember]
        public bool Bool { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public string String { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }
   }
}
