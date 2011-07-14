namespace EPiRobots
{
    using System;
    using System.Web.Caching;
    using System.Web.Hosting;

    /// <summary>
    /// Resource path provider class
    /// </summary>
    internal class ResourcePathProvider : VirtualPathProvider
    {
        #region Members

        private readonly string virtualPath;
        private readonly string resourceName;
        private readonly bool physicalResource;

        #endregion Members

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePathProvider"/> class.
        /// </summary>
        /// <param name="virtualPath">Virtual path</param>
        /// <param name="resourceName">Resource name</param>
        /// <param name="physicalResource">True if physical resource, otherwise false</param>
        public ResourcePathProvider(string virtualPath, string resourceName, bool physicalResource)
        {
            this.virtualPath = virtualPath;
            this.resourceName = resourceName;
            this.physicalResource = physicalResource;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Determines whether the requested virtual file exists
        /// </summary>
        /// <param name="virtualPath">Virtual file path</param>
        /// <returns>True if the file exists, otherwise false</returns>
        public override bool FileExists(string virtualPath)
        {
             return string.Compare(virtualPath, virtualPath, true) == 0 || Previous.FileExists(virtualPath);
        }

        /// <summary>
        /// Gets the virtual file
        /// </summary>
        /// <param name="virtualPath">Virtual file path</param>
        /// <returns>Virtual file</returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            return string.Compare(this.virtualPath, this.virtualPath, true) == 0 ? new ResourceVirtualFile(this.virtualPath, this.resourceName, this.physicalResource) : Previous.GetFile(this.virtualPath);
        }

        #endregion Methods
    }
}
