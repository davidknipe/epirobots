namespace EPiRobots.Resources.Admin
{
    using System;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using EPiRobots.Services;
    using EPiServer.Core;
    using EPiServer.Framework.Configuration;
    using EPiServer.PlugIn;
    using EPiServer.UI;

    [GuiPlugIn(
        Area = PlugInArea.AdminMenu,
        DisplayName = "Manage robots.txt content",
        Description = "Tool to manage the robots.txt",
        Url = "~/AdminManageRobotsTxt.aspx"
    )]
    public class AdminManageRobotsTxt : SystemPageBase
    {
        protected global::System.Web.UI.WebControls.DropDownList ddlSite;
        protected global::System.Web.UI.WebControls.TextBox txtRobots;
        protected global::System.Web.UI.WebControls.Button btnSave;

        protected override void OnPreInit(EventArgs e)
        {
            this.MasterPageFile = ResolveUrlFromUI("MasterPages/EPiServerUI.master");
            this.SystemMessageContainer.Heading = LanguageManager.Instance.TranslateFallback("#title", "Manage robots.txt");

            base.OnPreInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RobotsContentService srv = new RobotsContentService();

            if (!Page.IsPostBack)
            {
                //Fill the dropdown with all siteIds and associated host(s)
                var allHosts = (from HostNameCollection hosts in EPiServerFrameworkSection.Instance.SiteHostMapping
                                from HostNameElement host in hosts
                                orderby hosts.SiteId, host.Name
                                select new ListItem(srv.GetSiteKey(hosts.SiteId, host.Name))).ToArray();

                this.ddlSite.Items.Clear();
                this.ddlSite.Items.AddRange(allHosts);

                //Populate the text area to prepare for editing
                this.DdlSiteChange(null, null);
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            RobotsContentService srv = new RobotsContentService();
            srv.SaveRobotsContent(this.txtRobots.Text, this.ddlSite.SelectedValue);

            if (ddlSite.SelectedValue.EndsWith("*"))
            {
                string siteId = this.ddlSite.SelectedValue.Split(">".ToCharArray())[0].Trim();
                string confirmationMessage = LanguageManager.Instance.TranslateFallback("#defaultHostMessage", "Sucessfully saved robots.txt content. All requests for Site ID '{0}' that do not have an explicit host name mapping will serve this content");
                base.SystemMessageContainer.Message = string.Format(confirmationMessage, siteId);
            }
            else
            {
                string hostLink = string.Format("<a href=\"http://{0}/robots.txt\" target=\"_blank\">http://{0}/robots.txt</a>", this.ddlSite.SelectedValue.Split(">".ToCharArray())[1].Trim());
                string confirmationMessage = LanguageManager.Instance.TranslateFallback("#specificHostMessage", "Sucessfully saved robots.txt content. All requests to {0} will serve this content");
                base.SystemMessageContainer.Message = string.Format(confirmationMessage, hostLink);
            }
        }

        protected void DdlSiteChange(object source, EventArgs e)
        {
            RobotsContentService srv = new RobotsContentService();
            this.txtRobots.Text = srv.GetRobotsContent(this.ddlSite.SelectedValue);
        }
    }
}