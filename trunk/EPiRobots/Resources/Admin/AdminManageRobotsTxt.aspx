<%@ Page Language="C#" AutoEventWireup="true" Inherits="EPiRobots.Resources.Admin.AdminManageRobotsTxt" %>

<%--<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentRegion" runat="server">
</asp:Content>--%>
<asp:Content ID="Content3" ContentPlaceHolderID="MainRegion" runat="server">

<div>
<EPiServer:Translate runat="server" Text="#intro" LocalizedText="Select your site and host to edit the robots.txt file:" />
<asp:DropDownList ID="ddlSite" runat="server" OnSelectedIndexChanged="ddlSiteChange" AutoPostBack="true" />
</div>
<br />
<div style="min-width: 300px; max-width: 770px;">
<asp:TextBox id="txtRobots" runat="server" Width="100%" Height="400px" TextMode="MultiLine" />
</div>
<br />
<div class="epi-buttonContainer">
    <span class="epi-cmsButton">
        <asp:Button CssClass="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Save" ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" onmouseover="EPi.ToolButton.MouseDownHandler(this)" onmouseout="EPi.ToolButton.ResetMouseDownHandler(this)" />    
    </span>
</div>

</asp:Content>

