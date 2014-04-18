namespace Medseek.Util.MicroServices.Host
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Threading;
    using Medseek.Util.Interactive;
    using Medseek.Util.Ioc.Castle;
    using Medseek.Util.Logging;
    using Medseek.Util.Logging.NLog;
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
                var message = string.Format("Unhandled exception from AppDomain; IsTerminating = {0}.", e.IsTerminating);
                if (e.IsTerminating)
                    Log.Fatal(message, e.ExceptionObject as Exception);
                else
                    Log.Error(message, e.ExceptionObject as Exception);
            };

            Dispatcher.CurrentDispatcher.UnhandledException += (sender, e) =>
            {
                var message = string.Format("Unhandled exception from Dispatcher; Handled = {0}.", e.Handled);
                Log.Error(message, e.Exception);
            };

            var assembliesToInstall = args
                .Do(x => Console.WriteLine("Args = " + x))
                .Where(x => !x.StartsWith("/") && !x.StartsWith("-") && File.Exists(x))
                .Select(Assembly.LoadFrom)
                .Do(x => Console.WriteLine("Assembly = " + x.GetName().Name))
                .ToArray();

            using (var microServiceHost = new MicroServiceHost(CastleComponents.Plugin, Logging, RabbitMqComponents.Plugin, assembliesToInstall))
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