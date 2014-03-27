namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Windows.Threading;
    using Medseek.Util.Logging;
    using Medseek.Util.Logging.Log4Net;
    using Medseek.Util.Threading;

    /// <summary>
    /// Provides a windows service model for running the micro-service 
    /// application server.
    /// </summary>
    public class Service : ServiceBase
    {
        private static readonly ILog Log;
        private readonly List<IDisposable> disposables = new List<IDisposable>();
        private Thread thread;

        static Service()
        {
            LogManager.Default = new Log4NetLogManager();
            Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            Log.Info("Initialized logging.");

            foreach (var arg in Environment.GetCommandLineArgs())
                Log.Info("Arg: " + arg);

            AppDomain.CurrentDomain.DomainUnload += (sender, e) => Log.InfoFormat("Unloading AppDomain.");
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var message = string.Format("Unhandled exception from AppDomain; IsTerminating = {0}.", e.IsTerminating);
                if (e.IsTerminating)
                    Log.Fatal(message, e.ExceptionObject as Exception);
                else
                    Log.Error(message, e.ExceptionObject as Exception);
            };

            if (!Console.IsInputRedirected)
            {
                var dispatcher = Dispatcher.CurrentDispatcher;
                Console.CancelKeyPress += (sender, e) => dispatcher.InvokeShutdown();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        public Service()
        {
            CanStop = true;
        }

        /// <summary>
        /// Runs the service.
        /// </summary>
        /// <param name="args">
        /// Arguments from the command line.
        /// </param>
        [STAThread]
        public static void Run(string[] args)
        {
            using (var service = new Service())
            {
                if (args.Contains("/service"))
                    Run(service);
                else
                    service.RunApplicationServer(args);
            }
        }

        /// <summary>
        /// Starts the application server.
        /// </summary>
        /// <param name="args">
        /// Arguments passed to the start command.
        /// </param>
        protected override void OnStart(string[] args)
        {
            thread = new Thread(RunApplicationServer) { Name = "ApplicationServer", IsBackground = false };
            thread.Start(args);
        }

        /// <summary>
        /// Stops the application server.
        /// </summary>
        protected override void OnStop()
        {
            foreach (var disposable in disposables)
                disposable.Dispose();
            disposables.Clear();
            thread.Join();
            thread = null;
        }

        [STAThread]
        private void RunApplicationServer(object parameter)
        {
            var args = (string[])parameter ?? Environment.GetCommandLineArgs();
            args = Environment.GetCommandLineArgs();
            foreach (var arg in Environment.GetCommandLineArgs())
                Log.Info("Arg: " + arg);
            Log.Info("HERE-------");
            using (var applicationServer = new MicroServiceApplicationServer(args))
            {
                disposables.Add(applicationServer);
                applicationServer.Run();
            }
        }
    }
}