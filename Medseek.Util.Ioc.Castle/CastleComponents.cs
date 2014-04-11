namespace Medseek.Util.Ioc.Castle
{
    /// <summary>
    /// Contains constants describing and related to the Castle components.
    /// </summary>
    public class CastleComponents
    {
        private static readonly CastlePlugin CastlePlugin = new CastlePlugin();

        /// <summary>
        /// Prevents a default instance of the <see 
        /// cref="CastleComponents" /> class from being created.
        /// </summary>
        private CastleComponents()
        {
        }

        /// <summary>
        /// Gets the plugin that provides pluggable functionality using the 
        /// Castle project integration components.
        /// </summary>
        public static CastlePlugin Plugin
        {
            get
            {
                return CastlePlugin;
            }
        }
    }
}