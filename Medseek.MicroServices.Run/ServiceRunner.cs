namespace Medseek.MicroServices.Run
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
        private readonly Process process = new Process();
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        private readonly ServiceDescriptor descriptor;

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
                //RedirectStandardError = true,
                //RedirectStandardInput = true,
                //RedirectStandardOutput = true,
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
        /// Starts the service instance.
        /// </summary>
        public void Start()
        {
            process.Start();
            Log.InfoFormat("Started service process; Service = {0}, Pid = {1}.", descriptor, process.Id);
            process.EnableRaisingEvents = true;
            //process.BeginErrorReadLine();
            //process.BeginOutputReadLine();
        }

        /// <summary>
        /// Disposes the service runner.
        /// </summary>
        protected override void OnDisposing()
        {
            if (!process.HasExited)
            {
                Log.WarnFormat("Killing service process; Service = {0}, Pid = {1}.", descriptor, process.Id);
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
            Log.DebugFormat("Read STDOUT from process; Pid = {0}.", process.Id);
            Console.Out.WriteLine(e.Data);
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            var senderProcess = (Process)sender;
            var message = string.Format("Service process exited; Service = {0}, Pid = {1}.", descriptor, senderProcess.Id);
            if (IsDisposed)
                Log.Info(message);
            else
                Log.Warn(message);

            synchronizationContext.Post(ignored =>
            {
                process.EnableRaisingEvents = false;
                if (!IsDisposed)
                    Start();
            }, null);
        }
    }
}