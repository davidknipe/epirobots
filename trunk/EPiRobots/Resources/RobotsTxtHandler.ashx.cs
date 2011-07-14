using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiRobots.Services;

namespace EPiRobots.Resources
{
    /// <summary>
    /// Simple handler for returning robots.txt content. Used in conjunction to 
    /// hooks into the friendly URL rewriter to return the robots.txt content
    /// </summary>
    public class RobotsTxtHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        { 
            context.Response.ContentType = "text/plain";
            RobotsContentService service = new RobotsContentService();
            context.Response.Write(service.GetRobotsContent());
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}