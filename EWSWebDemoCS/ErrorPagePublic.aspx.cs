//=============================================================================
// File    : ErrorPagePublic.aspx.cs
// Author  : Eric Woodruff
// Updated : Fri 11/26/2004
// Compiler: Microsoft Visual C#
//
// This implements the error page for displaying unexpected application errors.
// To use it, change the class name in the Inherits option on the @Page tag in
// the ErrorPagePublic.aspx file and modify your Web.Config file to include a
// customErrors entry like the following:
//
//   <customErrors mode="RemoteOnly" defaultRedirect="ErrorPagePublic.aspx" />
//
// This version automatically e-mails the details to the support e-mail address
// and displays a generic message to the user as defined in the file
// ErrorPagePublic.htm.  This is useful for public internet applications in
// which you should not show full error details.
//
//    Date     Who  Comments
//=============================================================================
// 10/15/2002  EFW  Created the code
//=============================================================================

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using EWSoftware.Web;

namespace EWSWebDemoCS
{
	/// <summary>
	/// The public application error page
	/// </summary>
	public partial class ErrorPagePublic : EWSoftware.Web.BasePage
	{

        private string strErrorRptEMail;

        // Convert the name/value collections to standard sorted lists for use
        // with the repeaters.
        private SortedList ConvertNVCollection(NameValueCollection nvcColl)
        {
            string[] strArray1, strArray2;
            SortedList slColl = new SortedList();

            // Get the names of all keys into a string array
            strArray1 = nvcColl.AllKeys;

            foreach(string strEntry1 in strArray1)
            {
                // Get all the values under this key.  Empty collections and
                // the view state variable are not added.
                strArray2 = nvcColl.GetValues(strEntry1);

                if(strArray2 != null && strEntry1 != "__VIEWSTATE")
                    slColl.Add(strEntry1, String.Join("<br>", strArray2));
            }

            return slColl;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            Hashtable htErrorContext;
            SortedList slTemp;
            StringBuilder strLastError;
            string strRemoteAddr;

            this.EMailThisPage += new EventHandler<EMailPageEventArgs>(
                this.Page_EMailThisPage);
            this.EMailError += new EventHandler<EMailErrorEventArgs>(
                this.Page_EMailError);

            if(Page.IsPostBack)
                return;

            this.PageTitle = "Unexpected Application Error";

            // Because this is a public application, we don't want to give
            // the user the error details.  We'll automatically e-mail the
            // error report and give the user a short note telling them that
            // there was a problem.
            this.EMailRenderedPage = true;

            // Set the application name
            lblAppName.Text = ConfigurationManager.AppSettings["AppName"];

            // Get the help e-mail address
            strErrorRptEMail = ConfigurationManager.AppSettings["ErrorRptEMail"];

            // Retrieve the context information.  It should be there.
            strRemoteAddr = Request.ServerVariables["REMOTE_ADDR"];
            htErrorContext = (Hashtable)Cache[strRemoteAddr];

            if(htErrorContext != null)
            {
                // Do a little formatting on the error
                strLastError = new StringBuilder(htErrorContext["LastError"].ToString());
                strLastError.Replace("  ", "&nbsp;&nbsp;");
                strLastError.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                strLastError.Replace("\r", "");
                strLastError.Replace("\n", "<br>");

                lblLastError.Text = strLastError.ToString();
                lblPageName.Text = htErrorContext["Page"].ToString();

                rptServerVars.DataSource = (SortedList)htErrorContext["ServerVars"];

                slTemp = ConvertNVCollection(
                    (NameValueCollection)htErrorContext["QueryString"]);

                // Don't show query string or form repeater if they are empty
                if(slTemp.Count > 0)
                    rptQueryString.DataSource = slTemp;
                else
                    rptQueryString.Visible = false;

                slTemp = ConvertNVCollection(
                    (NameValueCollection)htErrorContext["Form"]);

                if(slTemp.Count > 0)
                    rptForm.DataSource = slTemp;
                else
                    rptForm.Visible = false;

                Page.DataBind();

                // Clear the error information from the cache
                Cache.Remove(strRemoteAddr);
            }
            else
            {
                rptServerVars.Visible = false;
                rptQueryString.Visible = false;
                rptForm.Visible = false;
                lblPageName.Text = Request.QueryString.ToString();
                lblLastError.Text = "No context information available";
            }
        }

        // This event fires if there was a problem e-mailing the page.
        private void Page_EMailError(Object sender,
          EWSoftware.Web.EMailErrorEventArgs args)
        {
            // Replace the "sent" message with an error message.  It's already
            // been rendered, so replace the text between the comment tags.
            args.EMailEventArguments.RenderedContent =
                Regex.Replace(args.EMailEventArguments.RenderedContent,
                @"\<!-- EMAILERROR --\>.*?\<!-- EMAILERROR --\>",
                "<span class='Attn'><br><br>There was a problem sending the " +
                "error report e-mail.  If the problem persists, you can report " +
                "the problem yourself by sending e-mail to " +
                strErrorRptEMail + ".</span><br><br>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        // This event fires when the page is ready to be e-mailed.
        private void Page_EMailThisPage(Object sender,
          EWSoftware.Web.EMailPageEventArgs args)
        {
            DateTime dtErrorDate;
            int nErrorCount, nMaxErrRpts;
            StreamReader sr = null;

            // See if we have exceeded the maximum number of error reports today.
            // We don't want to overload the recipient of the reports.
            nMaxErrRpts = Convert.ToInt32(ConfigurationManager.AppSettings["MaxErrorReports"]);

            if(nMaxErrRpts > 0)
            {
                if(Application["ErrorReportDate"] != null)
                {
                    dtErrorDate = (DateTime)Application["ErrorReportDate"];
                    nErrorCount = (int)Application["ErrorReportCount"];

                    if(dtErrorDate == DateTime.Today && nErrorCount >= nMaxErrRpts)
                    {
                        args.Cancel = true;
                        return;
                    }

                    if(dtErrorDate != DateTime.Today)
                    {
                        dtErrorDate = DateTime.Today;   // Date rolled over
                        nErrorCount = 1;
                    }
                    else
                        nErrorCount++;      // Another on the same day
                }
                else
                {
                    dtErrorDate = DateTime.Today;   // First one
                    nErrorCount = 1;
                }

                // Store the error report date and count to the application state
                Application.Lock();
                Application["ErrorReportDate"] = dtErrorDate;
                Application["ErrorReportCount"] = nErrorCount;
                Application.UnLock();
            }

            // Set the from address
            args.EMail.From = new MailAddress(ConfigurationManager.AppSettings["ErrorRptFrom"]);

            // Set recipient and subject
            args.EMail.To.Add(strErrorRptEMail);
            args.EMail.Subject = "Error in " + lblAppName.Text;

            // Give the user a less detail report of the error
            try
            {
                sr = new StreamReader(Server.MapPath("ErrorPagePublic.htm"));
                args.RenderedContent = sr.ReadToEnd();
            }
            catch
            {
                args.RenderedContent = "Unexpected application error";
            }
            finally
            {
                if(sr != null)
                    sr.Close();
            }
        }
    }
}
