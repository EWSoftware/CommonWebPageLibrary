using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MenuLayout : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get title from Web.Config
        if(!Page.IsPostBack)
            lblAppName.Text = ConfigurationManager.AppSettings["MenuAppName"];
    }
}
