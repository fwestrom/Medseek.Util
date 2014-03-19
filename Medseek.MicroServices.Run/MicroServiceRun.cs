﻿namespace Medseek.MicroServices.Run
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Threading;
    using System.Xml.Linq;
    using Medseek.Util.Logging;
    using Medseek.Util.Objects;

    /// <summary>
    /// A micro-service runtime hosting application.
    /// </summary>
    public class MicroServiceRun : Disposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private readonly List<ServiceRunner> runners = new List<ServiceRunner>();
        private readonly string broker;
        private readonly DirectoryInfo directory;
        private readonly DispatcherTimer refreshTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceRun" /> 
        /// class.
        /// </summary>
        public MicroServiceRun(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            broker = args.ArgValue("broker");
            directory = new DirectoryInfo(args.ArgValue("dir"));
            refreshTimer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher);
            refreshTimer.Tick += OnTick;
        }

        /// <summary>
        /// Invoked to enter the main application loop.
        /// </summary>
        public void Run()
        {
            dispatcher.VerifyAccess();

            Log.InfoFormat("Broker = {0}", broker);
            Log.InfoFormat("Directory = {0}", directory);

            refreshTimer.Start();
            try
            {
                Dispatcher.Run();
            }
            finally
            {
                Log.Info("Dispatcher exited.");
                refreshTimer.Stop();

                foreach (var runner in runners)
                    runner.Dispose();
                runners.Clear();
            }
        }

        /// <summary>
        /// Disposes the application.
        /// </summary>
        protected override void OnDisposing()
        {
            refreshTimer.Stop();
            dispatcher.InvokeShutdown();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                directory.Refresh();
                if (directory.Exists)
                {
                    var descriptors = directory.GetFiles("services.xml", SearchOption.AllDirectories)
                        .SelectMany(file => XDocument.Load((string)file.FullName)
                            .Descendants(XName.Get("service", string.Empty))
                            .Select(xe => new ServiceDescriptor(XmlExtensions.Attrib(xe, "id"), XmlExtensions.Attrib(xe, "run"), XmlExtensions.Attrib(xe, "args"), file.FullName)))
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
    }
}