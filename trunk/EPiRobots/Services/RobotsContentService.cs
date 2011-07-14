using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Data.Dynamic;
using log4net;
using EPiServer.Framework.Configuration;
using System.Web;
using EPiServer;

namespace EPiRobots.Services
{
    public class RobotsContentService
    {
        private static ILog _log = LogManager.GetLogger(typeof(RobotsContentService));

        private string CurrentSiteKey
        {
            get
            {
                //Look for a specific host name mapping for the current host name in the config
                var hostLookup = from HostNameCollection hosts in EPiServerFrameworkSection.Instance.SiteHostMapping
                                 from HostNameElement host in hosts
                                 where host.Name == HttpContext.Current.Request.Url.Host
                                 select host;

                if (hostLookup.Count() >= 1)
                {
                    //If the host is explicitly listed then return the key for the siteId and host
                    return this.GetSiteKey(EPiServer.Configuration.Settings.Instance.Parent.SiteId, hostLookup.FirstOrDefault().Name);
                }
                else
                {
                    //Otherwise this is the default "*" mapping of the site
                    return this.GetSiteKey(EPiServer.Configuration.Settings.Instance.Parent.SiteId, "*");
                }
            }
        }

        public string GetRobotsContent()
        {
            return this.GetRobotsContent(this.CurrentSiteKey);
        }

        public string GetRobotsContent(string RobotsKey)
        {
            try
            {
                //Always try to retrieve from cache first
                if (CacheManager.Get(this.GetCacheKey(RobotsKey)) != null)
                {
                    return (CacheManager.Get(this.GetCacheKey(RobotsKey)) as RobotsTxtData).RobotsTxtContent;
                }
                else
                {
                    using (var robotsDataStore = typeof(RobotsTxtData).GetStore())
                    {
                        //Look up the content in the DDS
                        var result = robotsDataStore.Find<RobotsTxtData>("Key", RobotsKey).FirstOrDefault();
                        if (result == null)
                        {
                            //OK there is nothing found so create some default content
                            result = new RobotsTxtData()
                            {
                                Key = RobotsKey,
                                RobotsTxtContent = this.GetDefaultRobotsContent()
                            };
                            this.SaveRobotsContent(result.RobotsTxtContent, RobotsKey);
                        }

                        //Ensure we cache the result using the EPiServer cache manager to ensure 
                        //everything works in a load balanced/mirrored environement
                        CacheManager.Add(this.GetCacheKey(RobotsKey), result);
                        return result.RobotsTxtContent;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error getting robots.txt content, returning default robots.txt content", ex);
                return this.GetDefaultRobotsContent();
            }
        }

        public void SaveRobotsContent(string RobotsContent)
        {
            this.SaveRobotsContent(RobotsContent, CurrentSiteKey);
        }

        public void SaveRobotsContent(string RobotsContent, string RobotsKey)
        {
            try
            {
                //Save the updated robots content down into the DDS
                using (var robotsDataStore = typeof(RobotsTxtData).GetStore())
                {
                    var result = robotsDataStore.Find<RobotsTxtData>("Key", RobotsKey).FirstOrDefault();
                    if (result == null)
                    {
                        robotsDataStore.Save(new RobotsTxtData() { Key = RobotsKey, RobotsTxtContent = RobotsContent });
                    }
                    else
                    {
                        result.RobotsTxtContent = RobotsContent;
                        robotsDataStore.Save(result);

                        //Ensure that the cache item is removed if it already exists
                        if (CacheManager.Get(this.GetCacheKey(RobotsKey)) != null)
                        {
                            CacheManager.Remove(this.GetCacheKey(RobotsKey));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error saving robots.txt content", ex);
            }
        }

        private string GetCacheKey(string RobotsKey)
        {
            return "EPiRobotsCache_{0}" + RobotsKey;
        }

        private string GetDefaultRobotsContent()
        {
            
            StringBuilder defaultText = new StringBuilder();
            defaultText.Append("User-agent: *" + System.Environment.NewLine);

            try
            {
                //By default we will remove the UI and Util URL paths
                defaultText.Append("Disallow: " + EPiServer.Configuration.Settings.Instance.UIUrl.OriginalString.TrimStart("~".ToCharArray()) + System.Environment.NewLine);
                defaultText.Append("Disallow: " + EPiServer.Configuration.Settings.Instance.UtilUrl.OriginalString.TrimStart("~".ToCharArray()) + System.Environment.NewLine);
            }
            catch (Exception ex)
            {
                _log.Error("Error accessing EPiServer configuration settings", ex);
            }

            return defaultText.ToString();
        }

        public string GetSiteKey(string SiteId, string SiteHost)
        {
            return SiteId + " > " + SiteHost;
        }
    }
}
