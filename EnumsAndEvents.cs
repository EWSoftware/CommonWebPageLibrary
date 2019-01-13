//=============================================================================
// System  : ASP.NET Common Web Page Classes
// File    : EnumsAndEvents.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 03/10/2006
// Note    : Copyright 2002-2006, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains enumerations and event argument classes.
//
// This code may be used in compiled form in any way you desire.  This
// file may be redistributed unmodified by any means PROVIDING it is not
// sold for profit without the author's written consent, and providing
// that this notice and the author's name and all copyright notices
// remain intact.
//
// This code is provided "as is" with no warranty either express or
// implied.  The author accepts no liability for any damage or loss of
// business that this product may cause.
//
// Version     Date     Who  Comments
// ============================================================================
// 2.0.0.0  03/10/2006  EFW  Created the code
//=============================================================================

using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

// All classes go in the EWSoftware.Web namespace
namespace EWSoftware.Web
{
    /// <summary>
    /// This public enumerated type defines the Robots meta tag options
    /// </summary>
    [Flags,Serializable]
    public enum RobotOptions
    {
        /// <summary>No robot options specified (the default)</summary>
        NotSet =    0x0000,
        /// <summary>Index this page</summary>
        Index =     0x0001,
        /// <summary>Follow links on this page</summary>
        Follow =    0x0002,
        /// <summary>Do not index this page</summary>
        NoIndex =   0x0004,
        /// <summary>Do not follow links on this page</summary>
        NoFollow =  0x0008,
        /// <summary>Do not index this page or follow links on it</summary>
        None =      0x000C,
        /// <summary>Do not archive this page</summary>
        NoArchive = 0x0010
    }

    /// <summary>
    /// This is a custom <see cref="EventArgs"/> class for the
    /// <see cref="BasePage.EMailThisPage"/> event.
    /// </summary>
    /// <remarks>The event arguments allow the handler to modify the e-mailed
    /// content, the rendered content, set the SMTP server, or cancel the
    /// event altogether.</remarks>
    public class EMailPageEventArgs : EventArgs
    {
        //=====================================================================
        // Private class members

        // This can be used to cancel the e-mail
        private bool cancel;

        // The SMTP server to use
        private string smtpServer;

        // The e-mail message
        private MailMessage msg;

        // The page content that will be rendered to the client
        private string html;

        // The retry flag and retry counter
        private bool retry;
        private int  retryCount;

        //=====================================================================
        // Properties

	    /// <summary>
        /// This property can be set to true to cancel the sending of
        /// the e-mail.
        /// </summary>
        /// <remarks>If cancelled, the <see cref="RenderedContent"/> will
        /// still be sent to the client's browser.  You can replace it with
        /// an alternate response if necessary when cancelling the e-mail
        /// (i.e. to redirect the user to another location, etc).</remarks>
    	public bool Cancel
        {
			get { return cancel; }
			set { cancel = value; }
    	}

	    /// <summary>
        /// This property lets you set the SMTP server that should be used
        /// when sending the e-mail.
        /// </summary>
        /// <value>It is usually only necessary to set this property if you
        /// are running the application on <b>localhost</b> or if your IIS
        /// server is not configured with the SMTP service.</value>
        public string SmtpServer
        {
            get { return smtpServer; }
            set { smtpServer = value; }
        }

	    /// <summary>
        /// This property gives you access to the e-mail message object.
	    /// </summary>
        /// <value>Use it to set the sender, recipient, and subject and
        /// also to modify the message body.  The
        /// <see cref="System.Net.Mail.MailMessage.Body"/> property of the
        /// returned <see cref="System.Net.Mail.MailMessage"/> will contain
        /// the HTML that will be sent in the e-mail.  You can modify it as
        /// needed.</value>
        public MailMessage EMail
        {
            get { return msg; }
            set { msg = value; }
        }

	    /// <summary>
        /// This property lets you modify the page content that will be
        /// rendered to the client after the e-mail is sent.
	    /// </summary>
        /// <value>Use it to alter the content rendered to the client's
        /// browser.  For example, you may want to remove sections that
        /// aren't relevant in the displayed content, insert a message
        /// telling the user that the message was sent and to whom, or
        /// replace the content with something entirely new.</value>
        public string RenderedContent
        {
            get { return html; }
            set { html = value; }
        }

        /// <summary>
        /// This property lets you specify whether the page should retry
        /// sending the e-mail if it fails.
        /// </summary>
        /// <value>It is set to false by default.  It is also set to false
        /// before each send attempt in order to help prevent an endless loop
        /// in case you forget to turn it off.  Set this to true in the
        /// error event handler to have the page retry sending the e-mail
        /// based on changes you make to the message properties or the
        /// SMTP server property.</value>
        public bool RetryOnFailure
        {
            get { return retry; }
            set { retry = value; }
        }

        /// <summary>
        /// This property can be used to track how many times an attempt to
        /// send the e-mail has failed.
        /// </summary>
        /// <value>You can increment it in the error event handler and use
        /// its value to determine when to stop trying.</value>
        public int RetryCount
        {
            get { return retryCount; }
            set { retryCount = value; }
        }

        //=====================================================================
        // Methods, etc

        /// <summary>
        /// Default constructor.  You must set all the other properties.
        /// </summary>
        /// <overloads>There are two constructors for this class</overloads>
        public EMailPageEventArgs()
        {
        }

        /// <summary>
        /// Construct the event arguments with a message.
        /// </summary>
        /// <param name="mm">The <see cref="System.Net.Mail.MailMessage"/>
        /// object to be sent.</param>
        /// <param name="page">A reference to the <see cref="System.Web.UI.Page"/>
        /// that is sending the e-mail.  This is used to translate relative
        /// URL paths to absolute paths.</param>
        /// <remarks>The <see cref="RenderedContent"/> property is set to
        /// the body of the e-mail message.  The e-mail body is then processed
        /// to remove any HTML between <b>&lt;!-- NOEMAIL --&gt;</b>
        /// comment tag blocks as well as the view state and all script tag
        /// blocks.  It also attempts to translate relative URLs to absolute
        /// URLs on all occurrences of <b>src</b> and <b>href</b> attributes.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">A
        /// <see cref="System.Net.Mail.MailMessage"/> object must be specified
        /// when constructing <b>EMailPageEventArgs</b></exception>
        /// <exception cref="System.ArgumentNullException">A
        /// <see cref="System.Web.UI.Page"/> object must be specified when
        /// constructing <b>EMailPageEventArgs</b>.</exception>
        public EMailPageEventArgs(MailMessage mm, System.Web.UI.Page page)
        {
            if(mm == null)
                throw new ArgumentNullException("mm",
                    "A System.Net.Mail.MailMessage object must be " +
                    "specified when constructing EMailPageEventArgs");

            if(page == null)
                throw new ArgumentNullException("page",
                    "A System.Web.UI.Page must be specified when " +
                    "constructing EMailPageEventArgs");

            html = mm.Body;
            msg = mm;

            // Remove unwanted sections from the e-mail
            msg.Body = Regex.Replace(html,
                @"\<!-- NOEMAIL --\>.*?\<!-- NOEMAIL --\>",
                "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Remove view state
            msg.Body = Regex.Replace(msg.Body,
                @"\<input type=.hidden. name=.__VIEWSTATE.* /\>",
                "", RegexOptions.IgnoreCase);

            // Remove all script blocks
            msg.Body = Regex.Replace(msg.Body,
                @"\<script.*?\>.*?\</script.*?\>",
                "", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Translate root relative URLs to absolute URLs
            msg.Body = Regex.Replace(msg.Body,
                @"((?:href|src)\s*=\s*(?:'|\x22))(/.+?/.+?(?:'|\x22))",
                "$1http://" + page.Request.ServerVariables["HTTP_HOST"].ToString() + "$2",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Translate relative URLs to absolute URLs
            msg.Body = Regex.Replace(msg.Body,
                @"((?:href|src)\s*=\s*(?:'|\x22))(?!(?:/+|.{3,5}://))(.+?(?:'|\x22))",
                "$1http://" + page.Request.ServerVariables["HTTP_HOST"].ToString() +
                page.Request.ApplicationPath + "/$2",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }

    /// <summary>
    /// This is a custom event args class for the
    /// <see cref="BasePage.EMailError"/> event.
    /// </summary>
    /// <remarks>The e-mail event properties can be modified as necessary to
    /// show alternate information to the user or to attempt a retry.</remarks>
    public class EMailErrorEventArgs : EventArgs
    {
        //=====================================================================
        // Private class members

        // This is the exception that occurred
        private Exception excp;

        // The e-mail event arguments
        private EMailPageEventArgs args;

        //=====================================================================
        // Properties

	    /// <summary>
        /// This property can be used to get the exception information.
	    /// </summary>
        /// <remarks>Note that you may have to check
        /// <b>EMailException.InnerException</b> and
        /// <b>EMailException.InnerException.InnerException</b> to get the
        /// true cause of the problem.</remarks>
    	public Exception EMailException
        {
			get { return excp; }
    	}

        /// <summary>
        /// This property can be used to access the e-mail event arguments.
        /// </summary>
        /// <remarks>You can modify the event as necessary to show alternate
        /// information to the user or to attempt a retry using alternate
        /// message parameters or an alternate SMTP server.</remarks>
        public EMailPageEventArgs EMailEventArguments
        {
            get { return args; }
        }

        //=====================================================================
        // Methods, etc

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="e">The <see cref="EMailPageEventArgs"/> that were
        /// used when the exception occurred.  You can modify the properties
        /// of the object via the <see cref="EMailEventArguments"/> property
        /// to further modify the content rendered to the client to inform
        /// them of the error or perhaps retry using alternate parameters.</param>
        /// <param name="ex">The <see cref="System.Exception"/> that occurred
        /// while trying to send the e-mail.</param>
        public EMailErrorEventArgs(EMailPageEventArgs e, Exception ex)
        {
            args = e;
            excp = ex;
        }
    }

}
