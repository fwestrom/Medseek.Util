namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;
    using System.Text;

    /// <summary>
    /// Describes a service.
    /// </summary>
    public class ServiceDescriptor
    {
        private readonly string id;
        private readonly string run;
        private readonly string args;
        private readonly string manifestPath;
        private readonly string dir;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
        /// </summary>
        public ServiceDescriptor(string id, string run, string args, string manifestPath, string dir)
        {
            if (run == null)
                throw new ArgumentNullException("run");
            if (args == null)
                throw new ArgumentNullException("args");
            if (manifestPath == null)
                throw new ArgumentNullException("manifestPath");

            this.id = id;
            this.run = run;
            this.args = args;
            this.manifestPath = manifestPath;
            this.dir = dir;
        }

        /// <summary>
        /// Gets the service identifier string.
        /// </summary>
        public string Id
        {
            get
            {
                return id;
            }
        }

        public string Dir
        {
            get
            {
                return dir;
            }
        }

        public string ManifestPath
        {
            get
            {
                return manifestPath;
            }
        }

        public string Run
        {
            get
            {
                return run;
            }
        }

        public string Args
        {
            get
            {
                return args;
            }
        }

        /// <summary>
        /// Determines whether the specified is equal to the service desctiptor.
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as ServiceDescriptor;
            return other != null && ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// Gets a hash code value for the service descriptor.
        /// </summary>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(manifestPath);
            if (id != null)
                sb.AppendFormat("[id={0}]", id);
            else
                sb.AppendFormat(@"\{0}", run);
            return sb.ToString();
        }
    }
}