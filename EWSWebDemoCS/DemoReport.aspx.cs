//=============================================================================
// System  : ASP.NET Common Web Page Classes Demo
// File    : DemoReport.aspx.cs
// Author  : Eric Woodruff
// Updated : Fri 11/26/2004
// Compiler: Microsoft Visual C#
//
// A simple demo to show how a page might e-mail itself if passed an e-mail
// address.
//
//    Date     Who  Comments
// ============================================================================
// 11/29/2003  EFW  Created the code
//=============================================================================

using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

using EWSoftware.Web;

namespace EWSWebDemoCS
{
	/// <summary>
	/// The demo report page
	/// </summary>
	public partial class DemoReport : EWSoftware.Web.BasePage
	{
        private string strEMailAddress, strComments;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            TestEMailPage2 caller;

            this.EMailThisPage += new EventHandler<EMailPageEventArgs>(
                this.Page_EMailThisPage);
            this.EMailError += new EventHandler<EMailErrorEventArgs>(
                this.Page_EMailError);

            if(!Page.IsPostBack)
            {
                this.PageTitle = "Demo Report";

                // See if an e-mail address was entered as criteria
                if(Context.Handler.GetType().BaseType == typeof(TestEMailPage2))
                {
                    caller = (TestEMailPage2)Context.Handler;
                    strEMailAddress = caller.EMailAddress;
                    strComments = caller.Comments;

                    if(strEMailAddress != null && strEMailAddress.Length > 0)
                        this.EMailRenderedPage = true;
                }
            }
		}

        // This event fires if there was a problem e-mailing the page
        private void Page_EMailError(Object sender,
          EWSoftware.Web.EMailErrorEventArgs args)
        {
            // Replace the "sent" message with an error message
            args.EMailEventArguments.RenderedContent =
                Regex.Replace(args.EMailEventArguments.RenderedContent,
                @"\<!-- EMAILERROR --\>.*?\<!-- EMAILERROR --\>",
                "<span class='Attn'>The was a problem sending the e-mail. " +
                "Please e-mail the report manually.</span><br><br>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        // This event fires when the page is ready to be e-mailed
        private void Page_EMailThisPage(Object sender,
          EWSoftware.Web.EMailPageEventArgs args)
        {
            // I don't want it!
            if(strEMailAddress.ToLower() == "eric@ewoodruff.us")
            {
                args.Cancel = true;
                args.RenderedContent = args.RenderedContent.Replace(
                    "<!-- EMAILCOMMENTS -->", "<b>Eric doesn't want to get " +
                    "spammed with test messages.  Please send it to " +
                    "somebody else, yourself for example.  The event " +
                    "was cancelled.</b><br><hr>");

                return;
            }

            // Set sender, recipient, and subject
            args.EMail.From = new MailAddress(strEMailAddress);
            args.EMail.To.Add(strEMailAddress);
            args.EMail.Subject = this.PageTitle;

            // Insert e-mail comments if necessary
            if(strComments.Length > 0)
                args.EMail.Body = args.EMail.Body.Replace(
                    "<!-- EMAILCOMMENTS -->",
                    "<b>Sender Comments:</b><br>" + strComments +
                    "<br><br><hr>");

            // Insert sent notification to the rendered page
            args.RenderedContent = args.RenderedContent.Replace(
                "<!-- EMAILSENT -->",
                "<b>This report was sent via e-mail to " +
                args.EMail.To[0].Address + "</b><br><br><hr>");
        }
	}
}
