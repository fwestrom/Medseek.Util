namespace Medseek.Util.Serialization
{
    using Medseek.Util.Ioc;

    [RegisterFactory]
    public interface ISerializerFactory
    {
        ISerializer[] GetAllSerializers();
    }
}