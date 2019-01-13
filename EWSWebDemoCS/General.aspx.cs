//=============================================================================
// System  : ASP.NET Common Web Page Classes Demo
// File    : General.aspx.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : Fri 11/26/2004
// Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This demonstrates the features of the BasePage class
//
// Version     Date     Who  Comments
// ============================================================================
// 1.0.0    11/28/2003  EFW  Created the code
//=============================================================================

using System;
using System.Web.UI.WebControls;

using EWSoftware.Web;

namespace EWSWebDemoCS
{
	/// <summary>
	/// The general features page
	/// </summary>
	public partial class General : EWSoftware.Web.BasePage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if(!Page.IsPostBack)
            {
                // Set up form for data change checking.  Don't prompt if
                // the Save button or Enable/Disable buttons are clicked.
                // Prompt to save on all others.
                this.CheckForDataChanges = true;
                this.BypassPromptIds = new String[] { "btnSave", "btnEnDis1",
                    "btnEnDisAll" };

                // Only set default focus on initial load
                this.SetFocusExtended(txtField1);

                this.PageTitle = "General BasePage Features";
            }
		}

        protected void btnEnDis1_Click(Object sender, System.EventArgs e)
        {
            txtField1.Enabled = !txtField1.Enabled;
            rqfField1.Enabled = txtField1.Enabled;

            if(txtField1.Enabled == true)
                this.SetFocusExtended(txtField1);
            else
                this.SetFocusExtended(txtField2);
        }

        protected void btnEnDisAll_Click(Object sender, System.EventArgs e)
        {
            // Re-enable field one to make it look consistent with the others
            if(!txtField1.Enabled)
                txtField1.Enabled = true;

            // Disable or enable all controls
            this.SetEnabledAll(!txtField3.Enabled, this.PageForm);
            rqfField1.Enabled = txtField1.Enabled;
            rqfField2.Enabled = txtField2.Enabled;
            rqfField3.Enabled = txtField3.Enabled;

            if(txtField1.Enabled == true)
                this.SetFocusExtended(txtField1);
            else
                this.SetFocusExtended(btnEnDisAll);
        }

        protected void btnDefault_Click(object sender, System.EventArgs e)
        {
            // Set default values
            txtField1.Text = "Field 1";
            txtField2.Text = "Field 2";
            txtField3.Text = "Field 3";

            // Set the dirty flag to indicate that data has changed
            this.Dirty = true;
        }

        protected void btnSave_Click(Object sender, System.EventArgs e)
        {
            // Turn off the dirty flag to indicate that the data was saved
            if(Page.IsValid)
                this.Dirty = false;
        }

        protected void btnExit_Click(Object sender, System.EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
	}
}
