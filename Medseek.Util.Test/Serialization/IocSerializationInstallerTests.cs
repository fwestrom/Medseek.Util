namespace Medseek.Util.Serialization
{
    using System;
    using System.Linq;

    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Medseek.Util.Ioc;
    using Medseek.Util.Ioc.Castle;
    using Medseek.Util.Serialization.Newtonsoft.Json;

    using NUnit.Framework;

    public class IocSerializationInstallerTests
    {
        private WindsorContainer container;

        [SetUp]
        public void SetUp()
        {
            var iocPlugin = new CastlePlugin();
            container = new WindsorContainer();
            container.AddFacility<TypedFactoryFacility>()
           .Register(Registrations.FromAssemblyContaining(typeof(UtilComponents))
                .Select(iocPlugin.ToRegistration)
                .Cast<IRegistration>()
                .ToArray())
            .Register(Registrations.FromAssemblyContaining(typeof(NewtonsoftJsonComponents))
                .Select(iocPlugin.ToRegistration)
                .Cast<IRegistration>()
                .ToArray());
        }

        [TestCase(typeof(NewtonsoftJsonSerializer))]
        [TestCase(typeof(SystemRuntimeSerializationDataContractSerializer))]
        public void CanResolveSerializerFromFactory(Type serializerType)
        {
            var factory = container.Resolve<ISerializerFactory>();
            var results = factory.GetAllSerializers();
            Assert.That(results, Has.Some.InstanceOf(serializerType));
        }
    }
}