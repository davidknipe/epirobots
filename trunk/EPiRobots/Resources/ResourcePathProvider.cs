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

        private readonly string _virtualPath;
        private readonly string _resourceName;
        private readonly bool _physicalResource;

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
            _virtualPath = virtualPath;
            _resourceName = resourceName;
            _physicalResource = physicalResource;
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
             return String.Compare(virtualPath, _virtualPath, true) == 0 || Previous.FileExists(virtualPath);
        }

        /// <summary>
        /// Gets the virtual file
        /// </summary>
        /// <param name="virtualPath">Virtual file path</param>
        /// <returns>Virtual file</returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            return String.Compare(virtualPath, _virtualPath, true) == 0 ? new ResourceVirtualFile(_virtualPath, _resourceName, _physicalResource) : Previous.GetFile(virtualPath);
        }

        #endregion Methods

    }
}
