namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Threading;
    using System.Xml.Linq;
    using Medseek.Util.Logging;
    using Medseek.Util.Objects;

    /// <summary>
    /// A micro-service runtime hosting application.
    /// </summary>
    public class MicroServiceApplicationServer : Disposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, string> cmdLineParams = new Dictionary<string, string>();
        private readonly List<ServiceRunner> runners = new List<ServiceRunner>();
        private readonly string broker;
        private readonly DirectoryInfo directory;
        private Dispatcher dispatcher;
        private DispatcherTimer pollTimer;
        private DispatcherTimer refreshTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceApplicationServer" /> 
        /// class.
        /// </summary>
        public MicroServiceApplicationServer(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            cmdLineParams = args.ToArgDictionary();

            broker = cmdLineParams.Get("broker");
            directory = new DirectoryInfo(cmdLineParams.Get("dir"));
        }

        /// <summary>
        /// Invoked to enter the main application loop.
        /// </summary>
        public void Run()
        {
            Log.InfoFormat("Broker = {0}", broker);
            Log.InfoFormat("Directory = {0}", directory);

            dispatcher = Dispatcher.FromThread(Thread.CurrentThread) ?? Dispatcher.CurrentDispatcher;
            dispatcher.UnhandledException += OnDispatcherUnhandledException;
            dispatcher.ShutdownStarted += OnDispatcherShutdownStarted;

            pollTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnPollTimerTick, dispatcher);
            refreshTimer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher);
            refreshTimer.Tick += OnRefreshTimerTick;
            pollTimer.Start();
            refreshTimer.Start();
            try
            {
                Dispatcher.Run();
                Log.Info("Dispatcher exited.");
                if (runners.Count > 0)
                    Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected failure from Dispatcher.", ex);
            }
            finally
            {
                foreach (var runner in runners)
                    runner.Dispose();
                runners.Clear();
            }
        }

        private void OnDispatcherShutdownStarted(object sender, EventArgs e)
        {
            Log.Info("Dispatcher is shutting down.");
            pollTimer.Stop();
            refreshTimer.Stop();
            foreach (var runner in runners)
                runner.Stop();
        }

        /// <summary>
        /// Disposes the application.
        /// </summary>
        protected override void OnDisposing()
        {
            if (dispatcher != null)
            {
                Log.Info("Disposing.");
                dispatcher.InvokeShutdown();
            }
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var message = string.Format("Unhandled exception from Dispatcher; Handled = {0}.", e.Handled);
            Log.Error(message, e.Exception);
            e.Handled = true;
        }

        private void OnPollTimerTick(object sender, EventArgs e)
        {
            foreach (var runner in runners)
                runner.Poll();
        }

        private void OnRefreshTimerTick(object sender, EventArgs e)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                directory.Refresh();
                if (directory.Exists)
                {
                    var descriptors = directory.GetFiles("services.xml", SearchOption.AllDirectories)
                        .SelectMany(file => XDocument.Load(file.FullName)
                            .Descendants(XName.Get("service", string.Empty))
                            .Select(xe => new ServiceDescriptor(xe.Attrib("id"), xe.Attrib("run"), MapArgumentTokens(xe.Attrib("args")), file.FullName)))
                        .ToArray();

                    var removed = runners.Where(r => !descriptors.Contains(r.Descriptor)).ToArray();
                    foreach (var runner in removed)
                    {
                        Log.InfoFormat("Removed service {0}.", runner);
                        runners.Remove(runner);
                        runner.Dispose();
                    }

                    var added = descriptors.Except(runners.Select(x => x.Descriptor)).ToArray();
                    foreach (var descriptor in added)
                    {
                        Log.InfoFormat("Added service {0}.", descriptor);
                        var runner = new ServiceRunner(descriptor);
                        runners.Add(runner);
                        runner.Start();
                    }
                }
            }
            finally
            {
                Log.DebugFormat("{0}: Refresh completed; Elapsed = {1}.", MethodBase.GetCurrentMethod().Name, sw.Elapsed);
                refreshTimer.Interval = TimeSpan.FromSeconds(10);
            }
        }

        private string MapArgumentTokens(string args)
        {
            foreach (var cmdLineParam in cmdLineParams)
            {
                var token = string.Format("$({0})", cmdLineParam.Key);
                args = args.Replace(token, cmdLineParam.Value);
            }

            return args;
        }
    }
}