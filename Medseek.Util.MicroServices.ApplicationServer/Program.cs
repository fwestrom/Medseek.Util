namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;

    /// <summary>
    /// Provides the entry point for the micro-service runtime hosting 
    /// application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">
        /// Arguments from the command line.
        /// </param>
        [STAThread]
        public static void Main(string[] args)
        {
            Service.Run(args);
        }
    }
}