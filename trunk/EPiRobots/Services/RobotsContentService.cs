namespace EPiRobots.Services
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web;
    using EPiServer;
    using EPiServer.Data.Dynamic;
    using EPiServer.Framework.Configuration;
    using log4net;

    public class RobotsContentService
    {
        #region Private Properties

        private static ILog logger = LogManager.GetLogger(typeof(RobotsContentService));

        private string CurrentSiteKey
        {
            get
            {
                // Look for a specific host name mapping for the current host name in the config
                var hostLookup = from HostNameCollection hosts in EPiServerFrameworkSection.Instance.SiteHostMapping
                                 from HostNameElement host in hosts
                                 where host.Name == HttpContext.Current.Request.Url.Host
                                 select host;

                if (hostLookup.Count() >= 1)
                {
                    // If the host is explicitly listed then return the key for the siteId and host
                    return this.GetSiteKey(EPiServer.Configuration.Settings.Instance.Parent.SiteId, hostLookup.FirstOrDefault().Name);
                }
                else
                {
                    // Otherwise this is the default "*" mapping of the site
                    return this.GetSiteKey(EPiServer.Configuration.Settings.Instance.Parent.SiteId, "*");
                }
            }
        }

        #endregion

        #region Public methods

        public string GetRobotsContent()
        {
            return this.GetRobotsContent(this.CurrentSiteKey);
        }

        public string GetRobotsContent(string robotsKey)
        {
            try
            {
                // Always try to retrieve from cache first
                if (CacheManager.Get(this.GetCacheKey(robotsKey)) != null)
                {
                    return (CacheManager.Get(this.GetCacheKey(robotsKey)) as RobotsTxtData).RobotsTxtContent;
                }
                else
                {
                    using (var robotsDataStore = typeof(RobotsTxtData).GetStore())
                    {
                        // Look up the content in the DDS
                        var result = robotsDataStore.Find<RobotsTxtData>("Key", robotsKey).FirstOrDefault();
                        if (result == null)
                        {
                            // OK there is nothing found so create some default content
                            result = new RobotsTxtData()
                            {
                                Key = robotsKey,
                                RobotsTxtContent = this.GetDefaultRobotsContent()
                            };
                            this.SaveRobotsContent(result.RobotsTxtContent, robotsKey);
                        }

                        // Ensure we cache the result using the EPiServer cache manager to ensure 
                        // everything works in a load balanced/mirrored environement
                        CacheManager.Add(this.GetCacheKey(robotsKey), result);
                        return result.RobotsTxtContent;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error getting robots.txt content, returning default robots.txt content", ex);
                return this.GetDefaultRobotsContent();
            }
        }

        public void SaveRobotsContent(string robotsContent)
        {
            this.SaveRobotsContent(robotsContent, this.CurrentSiteKey);
        }

        public void SaveRobotsContent(string robotsContent, string robotsKey)
        {
            try
            {
                // Save the updated robots content down into the DDS
                using (var robotsDataStore = typeof(RobotsTxtData).GetStore())
                {
                    var result = robotsDataStore.Find<RobotsTxtData>("Key", robotsKey).FirstOrDefault();
                    if (result == null)
                    {
                        robotsDataStore.Save(new RobotsTxtData() { Key = robotsKey, RobotsTxtContent = robotsContent });
                    }
                    else
                    {
                        result.RobotsTxtContent = robotsContent;
                        robotsDataStore.Save(result);

                        // Ensure that the cache item is removed if it already exists
                        if (CacheManager.Get(this.GetCacheKey(robotsKey)) != null)
                        {
                            CacheManager.Remove(this.GetCacheKey(robotsKey));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error saving robots.txt content", ex);
            }
        }

        public string GetSiteKey(string siteId, string siteHost)
        {
            return siteId + " > " + siteHost;
        }

        #endregion

        #region Private methods

        private string GetCacheKey(string robotsKey)
        {
            return "EPiRobotsCache_{0}" + robotsKey;
        }

        private string GetDefaultRobotsContent()
        {
            StringBuilder defaultText = new StringBuilder();
            defaultText.Append("User-agent: *" + System.Environment.NewLine);

            try
            {
                // By default we will remove the UI and Util URL paths
                defaultText.Append("Disallow: " + EPiServer.Configuration.Settings.Instance.UIUrl.OriginalString.TrimStart("~".ToCharArray()) + System.Environment.NewLine);
                defaultText.Append("Disallow: " + EPiServer.Configuration.Settings.Instance.UtilUrl.OriginalString.TrimStart("~".ToCharArray()) + System.Environment.NewLine);
            }
            catch (Exception ex)
            {
                logger.Error("Error accessing EPiServer configuration settings", ex);
            }

            return defaultText.ToString();
        }

        #endregion
    }
}
