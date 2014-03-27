namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    /// <summary>
    /// Installer for the micro-service application server windows service.
    /// </summary>
    [RunInstaller(true)]
    public class WindowsServiceInstaller : Installer
    {
        private readonly ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem, Username = null, Password = null };
        private readonly ServiceInstaller serviceInstaller = new ServiceInstaller
        {
            DisplayName = "Medseek Micro-Service Application Server",
            ServiceName = "MedseekMSAS",
            StartType = ServiceStartMode.Automatic,
            DelayedAutoStart = true,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceInstaller"/> class.
        /// </summary>
        public WindowsServiceInstaller()
        {
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);

            BeforeInstall += OnBeforeInstall;
            BeforeUninstall += OnBeforeInstall;
        }

        private void OnBeforeInstall(object sender, InstallEventArgs e)
        {
            foreach (string key in Context.Parameters.Keys)
                Console.WriteLine("Context.Parameters[{0}] = {1}", key, Context.Parameters[key]);
            
            if (Context.Parameters.ContainsKey("id"))
            {
                var id = Context.Parameters["id"];
                Console.WriteLine("[id] = " + id);

                serviceInstaller.DisplayName += " (" + id + ")";
                serviceInstaller.ServiceName += "-" + id;
            }

            var commandLine = '"' + Context.Parameters["assemblypath"].Trim('"') + "\" \"/service";
            if (Context.Parameters.ContainsKey("dir"))
            {
                var dir = Context.Parameters["dir"];
                Console.WriteLine("[dir] = " + dir);
                commandLine += "\" \"/dir=" + dir;
            }
            if (Context.Parameters.ContainsKey("broker"))
            {
                var broker = Context.Parameters["broker"];
                Console.WriteLine("[broker] = " + broker);
                commandLine += "\" \"/broker=" + broker;
            }

            commandLine += '"';
            Console.WriteLine("[assemblypath] = " + commandLine);
            Context.Parameters["assemblypath"] = commandLine;
        }
    }
}