namespace Medseek.Util.MicroServices
{
    using System;

    public class UnhandledExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="UnhandledExceptionEventArgs" /> class.
        /// </summary>
        public UnhandledExceptionEventArgs(Exception ex)
        {
            Ex = ex;
        }

        public Exception Ex
        {
            get;
            private set;
        }

        public bool IsHandled
        {
            get;
            private set;
        }

        public void SetHandled()
        {
            IsHandled = true;
        }
    }
}