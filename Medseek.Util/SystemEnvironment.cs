namespace Medseek.Util
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides access to information about the runtime environment for the 
    /// current process by wrapping calls to <see cref="Environment" />.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Register(typeof(IEnvironment))]
    public class SystemEnvironment : IEnvironment
    {
        /// <summary>
        /// Gets the command line for the process.
        /// </summary>
        public string CommandLine
        {
            get
            {
                return Environment.CommandLine;
            }
        }

        /// <summary>
        /// Gets the fully qualified path of the current working directory.
        /// </summary>
        public string CurrentDirectory
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

        /// <summary>
        /// Gets the command line arguments for the process.
        /// </summary>
        public string[] GetCommandLineArgs()
        {
            var result = Environment.GetCommandLineArgs();
            return result;
        }
    }
}