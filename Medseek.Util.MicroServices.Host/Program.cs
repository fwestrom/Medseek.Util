namespace Medseek.Util.MicroServices.Host
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Threading;
    using Medseek.Util.Interactive;
    using Medseek.Util.Ioc;
    using Medseek.Util.Ioc.Castle;
    using Medseek.Util.Logging;
    using Medseek.Util.Logging.NLog;
    using Medseek.Util.Messaging;
    using Medseek.Util.Messaging.RabbitMq;

    /// <summary>
    /// Provides the entry point for the micro-service host application, which 
    /// is used as a hosting process for executing micro-services defined in an
    /// external .NET assembly (which does not need to be executable, as is the
    /// case with a class library project compiled to a DLL).
    /// </summary>
    public class Program
    {
        private static readonly ILoggingPlugin Logging = NLogComponents.Plugin;
        private static readonly ILog Log = Logging.GetLogManager().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IIocPlugin Ioc = new CastlePlugin();
        private static readonly IMqPlugin Messaging = new RabbitMqPlugin();

        /// <summary>
        /// The entry point for the micro-service host application.
        /// </summary>
        /// <param name="args">
        /// The arguments passed to the process from the command line.
        /// </param>
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.DomainUnload += (sender, e) => Log.InfoFormat("Unloading AppDomain.");

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var message = string.Format("Unhandled exception from AppDomain; Type = {0}, IsTerminating = {1}.", e.ExceptionObject.GetType().FullName, e.IsTerminating);
                if (e.IsTerminating)
                {
                    Log.Fatal(message, e.ExceptionObject as Exception);
                    Environment.Exit(1);
                }
                else
                {
                    Log.Error(message, e.ExceptionObject as Exception);
                }
            };

            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                if (e.Exception.GetType() == typeof(ConnectionForcedClosedException))
                {
                    Log.Fatal("Connection to MQ closed.", e.Exception);
                    Environment.Exit(1);
                }
            };

            Dispatcher.CurrentDispatcher.UnhandledException += (sender, e) =>
            {
                var message = string.Format("Unhandled exception from Dispatcher; Type = {0}, Handled = {1}.", e.Exception.GetType().FullName, e.Handled);
                Log.Error(message, e.Exception);
            };

            var assembliesToInstall = args
                .Do(x => Console.WriteLine("Args = " + x))
                .Where(x => !x.StartsWith("/") && !x.StartsWith("-") && File.Exists(x))
                .Select(Assembly.LoadFrom)
                .Do(x => Console.WriteLine("Assembly = " + x.GetName().Name))
                .ToArray();

            using (var microServiceHost = new MicroServiceHost(Ioc, Logging, Messaging, assembliesToInstall))
            {
                microServiceHost.Start();
                Console.WriteLine("Started micro-service host.");
                try
                {
                    var thread = Thread.CurrentThread;
                    Console.CancelKeyPress += (sender, e) => thread.Interrupt();
                    if (Console.IsInputRedirected)
                    {
                        Thread.Sleep(Timeout.InfiniteTimeSpan);
                    }
                    else
                    {
                        do Console.WriteLine("Press escape to exit.");
                        while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                    }
                }
                catch (ThreadInterruptedException)
                {
                    Console.WriteLine("Interrupted.");
                }

                Console.WriteLine("Exiting.");
            }
        }
    }
}