namespace $safeprojectname$
{
    using System.ServiceModel;

    /// <summary>
    /// An example micro-service service contract.
    /// </summary>
    [ServiceContract(Namespace = Components.Xmlns)]
    public interface IHelloWorldService
    {
        /// <summary>
        /// An example of a micro-service operation.
        /// </summary>
        [OperationContract]
        HelloResponse SayHello(HelloRequest request);
    }
}