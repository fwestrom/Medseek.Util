namespace Medseek.MicroServices.Run
{
    using System;
    using System.Reflection;
    using System.Windows.Threading;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides the entry point for the micro-service runtime hosting 
    /// application.
    /// </summary>
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">
        /// Arguments from the command line.
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

            if (!Console.IsInputRedirected)
            {
                var dispatcher = Dispatcher.CurrentDispatcher;
                Console.CancelKeyPress += (sender, e) => dispatcher.InvokeShutdown();
            }

            using (var app = new MicroServiceRun(args))
                app.Run();
        }
    }
}