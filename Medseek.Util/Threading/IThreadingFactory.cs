namespace Medseek.Util.Threading
{
    using System.Threading;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Factory interface for types that can provide instances of the threading
    /// components.
    /// </summary>
    [RegisterFactory(Lifestyle = Lifestyle.Transient)]
    public interface IThreadingFactory
    {
        /// <summary>
        /// Returns an instance of a threading component that can be used to 
        /// control and interact with a new thread.
        /// </summary>
        /// <param name="start">
        /// The delegate to invoke as the entry point for the thread.
        /// </param>
        /// <param name="maxStackSize">
        /// The maximum stack size, in bytes, to be used by the thread, or 0 to
        /// use the default maximum stack size.
        /// </param>
        /// <returns>
        /// A component instance that can be used to interact with the new thread.
        /// </returns>
        /// <seealso cref="System.Threading.Thread(ThreadStart,int)" />
        IThread CreateThread(ThreadStart start, int maxStackSize = 0);

        /// <summary>
        /// Returns an instance of a threading component that can be used to 
        /// control and interact with a new thread.
        /// </summary>
        /// <param name="start">
        /// The delegate to invoke as the entry point for the thread.
        /// </param>
        /// <param name="maxStackSize">
        /// The maximum stack size, in bytes, to be used by the thread, or 0 to
        /// use the default maximum stack size.
        /// </param>
        /// <returns>
        /// A component instance that can be used to interact with the new thread.
        /// </returns>
        /// <seealso cref="System.Threading.Thread(ParameterizedThreadStart,int)" />
        IThread CreateThread(ParameterizedThreadStart start, int maxStackSize = 0);
    }
}