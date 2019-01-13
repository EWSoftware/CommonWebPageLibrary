//=============================================================================
// System  : ASP.NET Common Web Page Classes Demo
// File    : Default.aspx.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : Fri 11/26/2004
// Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This is the About/start-up page for the application
//
// Version     Date     Who  Comments
// ============================================================================
// 1.0.0    11/29/2003  EFW  Created the code
//=============================================================================

using System;
using System.Configuration;
using System.Web.UI.WebControls;

using EWSoftware.Web;

namespace EWSWebDemoCS
{
	/// <summary>
	/// The default page
	/// </summary>
	public partial class DefaultPage : EWSoftware.Web.BasePage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if(!Page.IsPostBack)
            {
                this.PageTitle = "ASP.NET Common Web Page Classes Demo";
                this.PageDescription = "The About page for the EWSoftware.Web demo";
                this.PageKeywords = "BasePage, PageUtils";
                this.Robots = RobotOptions.Index | RobotOptions.Follow;

                // Retrieve name and version from application settings in Web.Config
                lblAppName.Text = ConfigurationManager.AppSettings["AppName"];
                lblVersion.Text = ConfigurationManager.AppSettings["Version"];
                lblReleaseDate.Text = ConfigurationManager.AppSettings["ReleaseDate"];
            }
		}
	}
}
