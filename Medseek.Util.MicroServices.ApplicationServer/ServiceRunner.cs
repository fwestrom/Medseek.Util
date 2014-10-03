namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
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
        private readonly SynchronizationContext synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        private readonly ServiceDescriptor descriptor;
        private DateTime startTime = DateTime.MinValue;
        private Process process;
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
            if (process != null)
                process.Dispose();

            process = new Process();
            process.Exited += OnProcessExited;
            process.StartInfo = new ProcessStartInfo
            {
                Arguments = descriptor.Args,
                WorkingDirectory = Path.GetDirectoryName(descriptor.ManifestPath),
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            var exts = new List<string> { string.Empty };
            exts.AddRange((Environment.GetEnvironmentVariable("PATHEXT") ?? ".exe")
                .Split(Path.PathSeparator)
                .Distinct());

            var paths = new List<string> { process.StartInfo.WorkingDirectory };
            paths.AddRange((Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
                .Split(Path.PathSeparator)
                .Select(x => Path.Combine(x, descriptor.Run))
                .Distinct());

            process.StartInfo.FileName = paths
                .SelectMany(x => exts.Select(ext => Path.ChangeExtension(x, ext)))
                .FirstOrDefault(File.Exists)
                ?? descriptor.Run;

            try
            {
                process.Start();
                processId = process.Id.ToString("D");
                Log.InfoFormat("Started service process; Service = {0}, Pid = {1}, Exe = {2}, Args = {3}.", descriptor, processId, process.StartInfo.FileName, process.StartInfo.Arguments);

                var logger = string.Format("Service.{0}.id-{1}.pid-{2}", Path.GetFileName(Path.GetDirectoryName(descriptor.ManifestPath)), descriptor.Id ?? "none", processId);
                var log = LogManager.GetLogger(logger);
                process.ErrorDataReceived += (sender, e) => log.Warn(e.Data);
                process.OutputDataReceived += (sender, e) => log.Info(e.Data);
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed to start service process; Service = {0}, Exe = {1}, Args = {2}, Cause = {3}: {4}.", descriptor, process.StartInfo.FileName, process.StartInfo.Arguments, ex.GetType().Name, ex.Message.TrimEnd('.'));
                Log.WarnFormat(message, ex);
                process = null;
            }
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