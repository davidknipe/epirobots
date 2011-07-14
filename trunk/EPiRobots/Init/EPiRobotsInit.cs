namespace EPiRobots.Init
{
    using EPiServer.Framework;
    using EPiServer.Web;

    [InitializableModule]
    [ModuleDependency((typeof(EPiServer.Web.InitializationModule)))]
    public class EPiRobotsInit : IInitializableModule
    {
        #region IInitializableModule Members

        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
            UrlRewriteModule.HttpRewriteInit += this.HttpRewriteInit;
        }

        public void Preload(string[] parameters) 
        { 
        }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context) 
        {
            UrlRewriteModule.HttpRewriteInit -= this.HttpRewriteInit;
        }

        #endregion

        #region URL rewrite event handlers

        private void HttpRewriteInit(object sender, UrlRewriteEventArgs e)
        {
            UrlRewriteModule urm = (UrlRewriteModule)sender;
            urm.HttpRewritingToInternal += this.Urm_HttpRewritingToInternal;
        }

        private void Urm_HttpRewritingToInternal(object sender, UrlRewriteEventArgs e)
        {
            // If the request is for robots.txt then map to the handler. This  
            // approach is used to avoid web.config modification
            if (e.Url.Path.ToLower().StartsWith("/robots.txt"))
            {
                e.UrlContext.InternalUrl.Path = "/RobotsTxtHandler.ashx";
                e.IsModified = true;
            }
        }

        #endregion
    }
}
