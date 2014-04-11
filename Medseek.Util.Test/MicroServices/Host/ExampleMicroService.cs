namespace Medseek.Util.MicroServices.Host
{
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using Medseek.Util.Logging;

    /// <summary>
    /// An example micro-service for testing related functionality.
    /// </summary>
    [ReferenceMicroServiceHost]
    [RegisterMicroService(typeof(IExampleMicroService))]
    public class ExampleMicroService : IExampleMicroService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ExampleResponse DoSomething(ExampleRequest request)
        {
            Log.InfoFormat("DoSomething({0})", request);
            return new ExampleResponse();
        }
    }

    [ServiceContract(Namespace = "")]
    public interface IExampleMicroService
    {
        [OperationContract]
        ExampleResponse DoSomething(ExampleRequest request);
    }

    [DataContract(Namespace = "")]
    public class ExampleRequest
    {
    }

    [DataContract(Namespace = "")]
    public class ExampleResponse
    {
    }
}