namespace Medseek.Util
{
    using Medseek.Util.Ioc;
    using Medseek.Util.MicroServices;

    /// <summary>
    /// Contains constants describing and related to the Utility components.
    /// </summary>
    public sealed class UtilComponents : ComponentsInstallable
    {
        /// <summary>
        /// The component name for the micro-service component selector used 
        /// by factories that obtain instances of micro-service types that are 
        /// marked by <seealso cref="RegisterMicroServiceAttribute" />.
        /// </summary>
        public const string MicroServiceComponentSelector = "Medseek.Util.MicroServices.MicroServiceComponentSelector";

        /// <summary>
        /// The XML namespace used by the Utility components.
        /// </summary>
        public const string Xmlns = "http://schema.medseek.com/util";

        private static readonly UtilComponents ComponentsInfo = new UtilComponents();

        /// <summary>
        /// Prevents initialization of instances of the <see 
        /// cref="UtilComponents" /> class.
        /// </summary>
        private UtilComponents()
        {
        }

        /// <summary>
        /// Gets the base framework components information, which can be used 
        /// to install the utility library to an inversion of control 
        /// container.
        /// </summary>
        public static UtilComponents Framework
        {
            get
            {
                return ComponentsInfo;
            }
        }
    }
}