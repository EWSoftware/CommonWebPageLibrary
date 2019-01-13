//=============================================================================
// System  : ASP.NET Common Web Page Classes Demo
// File    : TestEMailPage.aspx.vb
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : Fri 11/26/2004
// Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
// Compiler: Microsoft VB.NET
//
// This demonstrates the e-mailing rendered content features of the BasePage
// class.
//
// Version     Date     Who  Comments
// ============================================================================
// 1.0.0.0  11/28/2003  EFW  Created the code
//=============================================================================

using System;
using System.Configuration;
using System.Net.Mail;
using System.Web.UI.WebControls;

using EWSoftware.Web;

namespace EWSWebDemoCS
{
	/// <summary>
	/// The e-mail page
	/// </summary>
	public partial class TestEMailPage : EWSoftware.Web.BasePage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.EMailThisPage += new EventHandler<EMailPageEventArgs>(
                this.Page_EMailThisPage);
            this.EMailError += new EventHandler<EMailErrorEventArgs>(
                this.Page_EMailError);

            if(!Page.IsPostBack)
            {
                this.PageTitle = "E-Mail Rendered Content Test";
                txtFrom.Focus();
            }
		}

        protected void btnEMail_Click(Object sender, System.EventArgs e)
        {
            if(Page.IsValid == true)
            {
                // Set this to true to have the page render itself and send
                // a copy via e-mail.
                this.EMailRenderedPage = true;
            }
        }

        // This handles the EMailThisPage event to set the e-mail info and
        // make some modifications to the e-mail and the page rendered to the
        // browser.
        private void Page_EMailThisPage(Object sender,
          EWSoftware.Web.EMailPageEventArgs args)
        {
            // Please don't send it to me!
            if(txtTo.Text.ToLower() == "eric@ewoodruff.us")
            {
                args.Cancel = true;

                args.RenderedContent = args.RenderedContent.Replace(
                    "<!-- SENTNOTES -->", "<b>Eric doesn't want to get " +
                    "spammed with test messages.  Please send it to " +
                    "somebody else, yourself for example.  The event " +
                    "was cancelled.</b><br><hr>");

                return;
            }

            if(txtSMTPServer.Text.Length > 0)
                args.SmtpServer = txtSMTPServer.Text;

            args.EMail.From = new MailAddress(txtFrom.Text);
            args.EMail.To.Add(txtTo.Text);
            args.EMail.Subject = txtSubject.Text;

            // Insert user comments into the e-mail
            args.EMail.Body = args.EMail.Body.Replace("<!-- EMAILCOMMENTS -->",
                "User Comments:<br>" + txtComments.Text + "<br><hr>");

            args.RenderedContent = args.RenderedContent.Replace(
                "<!-- SENTNOTES -->",
                "The following was sent via e-mail to " +
                args.EMail.To + ":<br><br>");
        }

        // This handles the EMailError event to add some text to the page to
        // tell the user that something went wrong.
        private void Page_EMailError(Object sender,
          EWSoftware.Web.EMailErrorEventArgs args)
        {
            string strMsg, strAltServer = null;

            // Here's an example of retrying the failed send with an
            // alternate SMTP server.  In a real app, you probably want to
            // check the exception to see what caused it before retrying.
            if(args.EMailEventArguments.RetryCount == 0)
                strAltServer =
                    ConfigurationManager.AppSettings["AlternateSMTPServer"];

            if(strAltServer != null)
            {
                // Set the alternate server
                args.EMailEventArguments.SmtpServer = strAltServer;

                // Increment the counter and tell the page to retry
                args.EMailEventArguments.RetryCount++;
                args.EMailEventArguments.RetryOnFailure = true;
            }
            else
            {
                strMsg = String.Format("<hr><b>Ok, I lied, there was a problem and " +
                    "the e-mail couldn't be sent</b><br>Details:<br>{0}<br><hr>",
                    PageUtils.HtmlEncode(args.EMailException.ToString(), false));

                args.EMailEventArguments.RenderedContent =
                    args.EMailEventArguments.RenderedContent.Replace(
                        "<!-- EMAILERROR -->", strMsg);
            }
        }
	}
}
