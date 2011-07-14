namespace EPiRobots
{
    using System.IO;
    using System.Reflection;
    using System.Web.Hosting;

    /// <summary>
    /// Resource virtual file class
    /// </summary>
    internal class ResourceVirtualFile : VirtualFile
    {
        #region Members

        private readonly string resourceName;
        private readonly bool physicalResource;

        #endregion Members

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVirtualFile"/> class.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="physicalResource">if set to <c>true</c> [physical resource].</param>
        public ResourceVirtualFile(string virtualPath, string resourceName, bool physicalResource) : base(virtualPath)
        {
            this.resourceName = resourceName;
            this.physicalResource = physicalResource;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Opens the resource stream
        /// </summary>
        /// <returns>The resource stream</returns>
        public override Stream Open()
        {
            return !this.physicalResource
                       ? Assembly.GetExecutingAssembly().GetManifestResourceStream(this.resourceName)
                       : File.OpenRead(this.resourceName);
        }

        #endregion Methods
    }
}
