namespace Medseek.Util
{
    /// <summary>
    /// Interface for types that provide access to information about the 
    /// runtime environment for the process.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Gets the command line for the process.
        /// </summary>
        string CommandLine
        {
            get;
        }

        /// <summary>
        /// Gets the fully qualified path of the current working directory.
        /// </summary>
        string CurrentDirectory
        {
            get;
        }

        /// <summary>
        /// Gets the command line arguments for the process.
        /// </summary>
        string[] GetCommandLineArgs();
    }
}