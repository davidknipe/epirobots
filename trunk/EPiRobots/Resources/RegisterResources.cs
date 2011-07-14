namespace EPiRobots
{
    using System.Web;
    using System.Web.Hosting;
    using EPiServer;
    using EPiServer.PlugIn;

    /// <summary>
    /// Register resource class
    /// </summary>
    internal class RegisterResources : PlugInAttribute
    {

        #region Static methods

        /// <summary>
        /// Initialises VPP mappings
        /// </summary>
        public static void Start()
        {
            RegisterVppResources();
        }

        /// <summary>
        /// Registers Vpp resources
        /// </summary>
        private static void RegisterVppResources()
        {
            if (HttpContext.Current == null)
                return;

            HostingEnvironment.RegisterVirtualPathProvider(new ResourcePathProvider("/RobotsTxtHandler.ashx", "EPiRobots.Resources.RobotsTxtHandler.ashx", false));
            HostingEnvironment.RegisterVirtualPathProvider(new ResourcePathProvider("/AdminManageRobotsTxt.aspx", "EPiRobots.Resources.Admin.AdminManageRobotsTxt.aspx", false));
        }

        #endregion Static methods

    }
}
