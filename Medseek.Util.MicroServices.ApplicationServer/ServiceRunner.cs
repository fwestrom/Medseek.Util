namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Medseek.Util.Logging;
    using Medseek.Util.Objects;

    /// <summary>
    /// Keeps a service running.
    /// </summary>
    public class ServiceRunner : Disposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly TimeSpan StartWaitPeriod = TimeSpan.FromSeconds(10);
        private readonly Process process = new Process();
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        private readonly ServiceDescriptor descriptor;
        private DateTime startTime = DateTime.MinValue;
        private string processId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRunner"/> class.
        /// </summary>
        public ServiceRunner(ServiceDescriptor descriptor)
            : base(true)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            this.descriptor = descriptor;
            process.Exited += OnProcessExited;
            process.ErrorDataReceived += OnProcessErrorDataReceived;
            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.StartInfo = new ProcessStartInfo
            {
                Arguments = descriptor.Args,
                FileName = Path.Combine(Path.GetDirectoryName(descriptor.ManifestPath), descriptor.Run),
                WorkingDirectory = Path.GetDirectoryName(descriptor.ManifestPath),
                UseShellExecute = false,
            };
        }

        /// <summary>
        /// Gets the service descriptor associated with the service runner.
        /// </summary>
        public ServiceDescriptor Descriptor
        {
            get
            {
                return descriptor;
            }
        }

        /// <summary>
        /// Invoked by the application server periodically to allow the service
        /// runner to perform maintenance operations.
        /// </summary>
        public void Poll()
        {
            if (processId == null && DateTime.UtcNow - startTime > StartWaitPeriod)
                Start();
        }

        /// <summary>
        /// Starts the service instance.
        /// </summary>
        public void Start()
        {
            startTime = DateTime.UtcNow;
            process.Start();
            processId = process.Id.ToString("D");
            Log.InfoFormat("Started service process; Service = {0}, Pid = {1}.", descriptor, processId);
            process.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Disposes the service runner.
        /// </summary>
        protected override void OnDisposing()
        {
            if (!process.HasExited)
            {
                Log.WarnFormat("Killing service process; Service = {0}, Pid = {1}.", descriptor, processId);
                process.Kill();
                process.WaitForExit();
            }

            process.Dispose();
        }

        private static void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.Error.WriteLine(e.Data);
        }

        private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.DebugFormat("Read STDOUT from process; Pid = {0}.", processId);
            Console.Out.WriteLine(e.Data);
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            var message = string.Format("Service process exited; Service = {0}, Pid = {1}.", descriptor, processId);
            if (IsDisposed)
                Log.Info(message);
            else
                Log.Warn(message);

            synchronizationContext.Post(ignored =>
            {
                if (!IsDisposed)
                {
                    process.EnableRaisingEvents = false;
                    process.Refresh();
                }
                processId = null;
            }, null);
        }
    }
}