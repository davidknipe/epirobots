using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Framework;
using System.Web;
using Microsoft.Web.Administration;
using EPiServer.Web;

namespace EPiRobots.Init
{
    [InitializableModule]
    [ModuleDependency((typeof(EPiServer.Web.InitializationModule)))]
    public class EPiRobotsInit : IInitializableModule
    {
        #region IInitializableModule Members

        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
            UrlRewriteModule.HttpRewriteInit += HttpRewriteInit;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context) 
        {
            UrlRewriteModule.HttpRewriteInit -= HttpRewriteInit;
        }

        #endregion

        #region URL rewrite event handlers

        void HttpRewriteInit(object sender, UrlRewriteEventArgs e)
        {
            UrlRewriteModule urm = (UrlRewriteModule)sender;
            urm.HttpRewritingToInternal += urm_HttpRewritingToInternal;
        }

        void urm_HttpRewritingToInternal(object sender, UrlRewriteEventArgs e)
        {
            //If the request is for robots.txt then map to the handler. This  
            //approach is used to avoid web.config modification
            if ((e.Url.Path.ToLower().StartsWith("/robots.txt")))
            {
                e.UrlContext.InternalUrl.Path = "/RobotsTxtHandler.ashx";
                e.IsModified = true;
            }
        }

        #endregion

    }
}
