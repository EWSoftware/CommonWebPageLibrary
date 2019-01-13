//=============================================================================
// System  : ASP.NET Common Web Page Classes Demo
// File    : TestEMailPage2.aspx.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : Fri 11/26/2004
// Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This demonstrates the e-mail page content features of the BasePage class
// and the error information in the BasePage class.
//
// Version     Date     Who  Comments
// ============================================================================
// 1.0.0    11/29/2003  EFW  Created the code
//=============================================================================

using System;
using System.Web.UI.WebControls;

using EWSoftware.Web;

namespace EWSWebDemoCS
{
	/// <summary>
	/// The test e-mail page #2
	/// </summary>
	public partial class TestEMailPage2 : EWSoftware.Web.BasePage
	{

        // This property returns the e-mail address for the report page
        public string EMailAddress
        {
            get { return txtTo.Text; }
        }

        // This property returns the comments for the report page
        public string Comments
        {
            get { return txtComments.Text; }
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if(!Page.IsPostBack)
            {
                this.PageTitle = "E-Mail Page Content/BasePage Error Info";
                txtTo.Focus();
            }
		}

        protected void btnError_Click(Object sender, System.EventArgs e)
        {
            // Throw an exception to see the e-mail page content option in
            // use on a custom error page.  Also shows the BasePage class's
            // extended error stuff.
            throw new Exception("Test Exception");
        }

        protected void btnReport_Click(Object sender, System.EventArgs e)
        {
            // Demonstrate a page that e-mails itself if the calling
            // page (this one) passes an e-mail address to it.
            Server.Transfer("DemoReport.aspx");
        }
	}
}
