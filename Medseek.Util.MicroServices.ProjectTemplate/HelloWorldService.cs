namespace $safeprojectname$
{
    using System;
    using Medseek.Util.MicroServices;

    /// <summary>
    /// An example micro-service.
    /// </summary>
    [RegisterMicroService(typeof(IHelloWorldService))]
    public class HelloWorldService : IHelloWorldService
    {
        /// <summary>
        /// An example of a micro-service operation.
        /// </summary>
        public HelloResponse SayHello(HelloRequest request)
        {
            Console.WriteLine("Hello, world!  Request = {0}", request);
            return new HelloResponse();
        }
    }
}