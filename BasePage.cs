//===============================================================================================================
// System  : ASP.NET Common Web Page Classes
// File    : BasePage.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 09/13/2013
// Note    : Copyright 2002-2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a common base page class used by ASP.NET applications
//
// This code may be used in compiled form in any way you desire.  This file may be redistributed unmodified by
// any means PROVIDING it is not sold for profit without the author's written consent, and providing that this
// notice and the author's name and all copyright notices remain intact.
//
// This code is provided "as is" with no warranty either express or implied.  The author accepts no liability
// for any damage or loss of business that this product may cause.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.0.0.0  07/12/2002  EFW  Created the code
// 2.0.0.0  02/18/2006  EFW  Updated for use with .NET 2.0
// 3.0.0.0  09/30/2010  EFW  Updated for use with .NET 4.0
// 3.0.0.1  09/13/2013  EFW  Removed support for the DisabledCssClass property as it is no longer needed
//===============================================================================================================

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

// All classes go in the EWSoftware.Web namespace
namespace EWSoftware.Web
{
    /// <summary>
    /// This is a common base class for ASP.NET pages.
    /// </summary>
    /// <remarks>By using this class as the base class for your web pages, you can reduce the amount of code and
    /// HTML that you have to write and also take advantage of the features provided by the class to make your
    /// pages more robust with regard to control focus, control enabling and disabling, checking for changed
    /// field data before leaving a page, error handling, etc.</remarks>
    public class BasePage : System.Web.UI.Page
    {
        //=====================================================================
        // Constants

        /// <summary>
        /// This is the project namespace prefix for embedded resources
        /// </summary>
        internal const string ScriptsPath = "EWSoftware.Web.Scripts.";

        /// <summary>
        /// This constant represents the default name for the message link CSS class.  The default value is the
        /// class name <b>ErrorMsgLink</b>.
        /// </summary>
        /// <seealso cref="MsgLinkCssClass"/>
        /// <seealso cref="MakeMsgLink"/>
        public const string MsgLinkCssName = "ErrorMsgLink";

        //=====================================================================
        // Private class members

        // This is a reference to the form that this page contains.  If the
        // page contains no form, the control points to the page itself.
        private Control ctlForm;

        // The control that should have focus when the page finishes loading
        private string focusedControl;
        private bool findControl;

        // The dirty state flag for data change checking
        private bool isDirty;

        // Message link CSS class name.  Default as shown.
        private string msgLinkClass = BasePage.MsgLinkCssName;

        // This controls whether or not the page e-mails itself
        private bool emailRenderedPage, isRenderingForEMail;

        //=====================================================================
        // Properties

        /// <summary>
        /// The message link CSS class name
        /// </summary>
        /// <value>This is used to get/set the message link CSS class applied by default in the
        /// <see cref="MakeMsgLink"/> method.  Set it to String.Empty to turn it off.  The class should appear
        /// in the style sheet file associated with the application.</value>
        public string MsgLinkCssClass
        {
            get { return msgLinkClass; }
            set
            {
                if(value == null)
                    msgLinkClass = BasePage.MsgLinkCssName;
                else
                    msgLinkClass = value;
            }
        }

        /// <summary>
        /// The disabled control CSS class name
        /// </summary>
        /// <value>This is used to get/set the CSS class for disabled controls.  The class should appear in the
        /// style sheet file associated with the application.</value>
        [Obsolete("Define the aspNetDisabled style in your style sheet instead which is used automatically.")]
        public string DisabledCssClass { get; set; }

        /// <summary>
        /// This property is used to set or get a reference to the form
        /// control that appears on the page as indicated by the presence
        /// of a &lt;form&gt; tag.
        /// </summary>
        /// <value>If not explicitly set, it defaults to the form on
        /// the page or the page itself if there isn't one.
        /// <p/>Derived classes can use this property to access the form
        /// in order to insert additional controls into it.  For example,
        /// the derived <see cref="MenuPage"/> classes use it to insert the
        /// supporting table structure and the menu control around the form
        /// that makes up the page's content.</value>
        /// <exception cref="System.ArgumentException">
        /// This property must be set to an
        /// <see cref="System.Web.UI.HtmlControls.HtmlForm"/> or a
        /// <see cref="System.Web.UI.Page"/> object or it will throw an
        /// exception.</exception>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control PageForm
        {
            get { return ctlForm; }
            set
            {
                // Must be derived from one of these types
                if(value is System.Web.UI.HtmlControls.HtmlForm ||
                   value is System.Web.UI.Page)
                    ctlForm = value;
                else
                    throw new ArgumentException(
                        "PageForm must be set to an HtmlForm or Page object");
            }
        }

        /// <summary>
        /// This is used to get the authentication method in effect for
        /// the application.
        /// </summary>
        /// <remarks>This property is quite useful in determining whether
        /// or not things are working as expected with regard to
        /// authentication.  To distinguish between NTLM and Kerberos
        /// authentication, it relies on the length of the
        /// <b>HTTP_AUTHORIZATION</b> server variable.  NTLM headers are
        /// much shorter than Kerberos headers.</remarks>
        /// <value>
        /// <list type="table">
        ///    <listheader>
        ///       <term>Return Value</term>
        ///       <description>Authentication Type</description>
        ///    </listheader>
        ///    <item>
        ///       <term>Anonymous</term>
        ///       <description>Anonymous access</description>
        ///    </item>
        ///    <item>
        ///       <term>Basic</term>
        ///       <description>Basic authentication</description>
        ///    </item>
        ///    <item>
        ///       <term>NTLM</term>
        ///       <description>NTLM authentication</description>
        ///    </item>
        ///    <item>
        ///       <term>Kerberos</term>
        ///       <description>Kerberos authentication</description>
        ///    </item>
        ///    <item>
        ///       <term>Negotiate</term>
        ///       <description>NTLM or Kerberos.  Typically, when using
        /// these two types of authentication, this property only returns
        /// useful information on the first page accessed in the application.
        /// On subsequent requests, the authorization header is blank and it
        /// can only tell that some form of negotiated authentication was used.
        /// In those cases, <b>Negotiate</b> is returned.</description>
        ///    </item>
        /// </list>
        /// </value>
        [Browsable(false)]
        public string AuthType
        {
            get
            {
                // This prevents an exception being reported in design view
                if(this.Context == null)
                    return null;

                // Figure out the authentication type
                string authType = Request.ServerVariables["AUTH_TYPE"];

                if(authType == "Negotiate")
                {
                    // Typically, NTLM will yield a header that is 300 bytes
                    // or less while Kerberos is more like 5000 bytes.
                    // If blank, the best we can do is return "Negotiate".
                    string authorization = Request.ServerVariables["HTTP_AUTHORIZATION"];

                    if(authorization != null)
                        if(authorization.Length > 1000)
                            authType = "Kerberos";
                        else
                            authType = "NTLM";
                }
                else    // If length != 0, it's probably Basic authentication
                    if(authType.Length == 0)
                        authType = "Anonymous";

                return authType;
            }
        }

        /// <summary>
        /// This property can be used to get the current user ID without the
        /// domain qualifier if one is present.
        /// </summary>
        /// <remarks>This property is only useful when using basic or
        /// integrated security with your web application.  It is useful
        /// for auditing purposes or looking up security related information
        /// and saves you from having to manually remove the domain name from
        /// the user ID.</remarks>
        /// <value>Returns the value of the <b>User.Identity.Name</b>
        /// property without the domain qualifier.  For example if it
        /// is <b>MYDOMAIN\EWOODRUFF</b>, this property returns
        /// <b>EWOODRUFF</b>.</value>
        [Browsable(false)]
        public string CurrentUser
        {
            get
            {
                // This prevents an exception being reported in design view
                if(this.Context == null)
                    return null;

                string strUser = User.Identity.Name;
                int nPos = strUser.IndexOf('\\');

                if(nPos != -1)
                    strUser = strUser.Substring(nPos + 1);

                return strUser;
            }
        }

        /// <summary>
        /// Indicate whether or not to check for changed data entry controls.
        /// </summary>
        /// <remarks>This property is used to indicate whether or not the page
        /// should check for changes in data entry controls and track the dirty
        /// state before leaving the page in some manner (i.e. by clicking
        /// an exit button, clicking the browser's back or forward buttons,
        /// navigating to a different URL, or closing the browser).  If set
        /// to true, the page will render additional JavaScript to help ensure
        /// that the user is prompted that they will lose their changes to
        /// the controls on the page and it gives them an option to cancel
        /// leaving and stay on the current page or continuing and lose the
        /// changes <b>(Internet Explorer only)</b>.  For non-IE browsers,
        /// only dirty state tracking is available.</remarks>
        /// <seealso cref="Dirty"/>
        /// <seealso cref="ConfirmLeaveMessage"/>
        /// <seealso cref="BypassPromptIds"/>
        /// <seealso cref="SkipDataCheckIds"/>
        public bool CheckForDataChanges
        {
            get
            {
                Object oCheck = ViewState["CheckForDataChanges"];
                return (oCheck == null) ? false : (bool)oCheck;
            }
            set { ViewState["CheckForDataChanges"] = value; }
        }

        /// <summary>
        /// This property is used to track the dirty state of the data on
        /// a form if change checking is enabled.
        /// </summary>
        /// <remarks>This property is used to track the dirty state of a
        /// data entry web form.  It can also be used to force the page to
        /// a dirty state so that the <see cref="ConfirmLeaveMessage"/>
        /// is always shown <b>(Internet Explorer only)</b>. This is useful
        /// if the form contains auto-postback controls such as a checkbox
        /// or a button that causes other controls to get enabled or disabled,
        /// get set to specific values, etc.  In such cases, it is impossible
        /// to detect changes, as we no longer have the original values from
        /// before the postback.  Setting this property to true in the event
        /// handler of postback controls will ensure that the user is prompted
        /// to save their changes.</remarks>
        /// <seealso cref="CheckForDataChanges"/>
        /// <seealso cref="ConfirmLeaveMessage"/>
        /// <seealso cref="BypassPromptIds"/>
        /// <seealso cref="SkipDataCheckIds"/>
        [Browsable(false)]
        public bool Dirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        /// <summary>
        /// This property is used to set or get the message displayed if
        /// change checking is enabled and the user attempts to leave without
        /// saving the changes <b>(Internet Explorer only)</b>.  A default
        /// message is used if not set.
        /// </summary>
        /// <seealso cref="CheckForDataChanges"/>
        /// <seealso cref="Dirty"/>
        /// <seealso cref="BypassPromptIds"/>
        /// <seealso cref="SkipDataCheckIds"/>
        public string ConfirmLeaveMessage
        {
            get
            {
                Object oLeave = ViewState["ConfirmLeaveMsg"];
                return (oLeave != null) ? (string)oLeave :
                    "You haven't saved your changes.  Leaving now " +
                    "will lose all changes made.";
            }
            set { ViewState["ConfirmLeaveMsg"] = value; }
        }

        /// <summary>
        /// This is used to set a list of control IDs that should not
        /// trigger the data change check (i.e. a save button).
        /// </summary>
        /// <value>Set it to a string array of control ID values.  Any
        /// controls with IDs specified in this list will allow postback or
        /// leaving of the page without prompting to save <b>(Internet
        /// Explorer only)</b>.</value>
        /// <seealso cref="CheckForDataChanges"/>
        /// <seealso cref="Dirty"/>
        /// <seealso cref="ConfirmLeaveMessage"/>
        /// <seealso cref="SkipDataCheckIds"/>
        /// <example>
        /// C#:
        /// <code>
        /// this.BypassPromptIds = new string[] { "cboApplication",
        ///     "chkLimitToTeam", "cboGroupKey", "btnSave",
        ///     "btnDelete", "btnCancel" };
        /// </code>
        /// VB.NET:
        /// <code>
        /// Me.BypassPromptIds = New String() { "cboApplication", _
        ///     "chkLimitToTeam", "cboGroupKey", "btnSave", _
        ///     "btnDelete", "btnCancel" }
        /// </code>
        /// </example>
        public string[] BypassPromptIds
        {
            get
            {
                Object oBypass = ViewState["BypassList"];
                return (oBypass != null) ? (string[])oBypass : null;
            }
            set { ViewState["BypassList"] = value; }
        }

        /// <summary>
        /// This is used to set a list of control IDs that should not
        /// be included when checking for data changes (i.e. changeable
        /// message text boxes, read-only or criteria fields that get
        /// modified but do not affect the state of the data to save, etc).
        /// </summary>
        /// <value>Set it to a string array of control ID values.  Any
        /// controls with IDs specified in this list will not affect the
        /// dirty state of the page <b>(any browser)</b> and will not
        /// cause prompting when leaving the page <b>(Internet Explorer
        /// only)</b>.</value>
        /// <seealso cref="CheckForDataChanges"/>
        /// <seealso cref="Dirty"/>
        /// <seealso cref="ConfirmLeaveMessage"/>
        /// <seealso cref="BypassPromptIds"/>
        /// <example>
        /// C#:
        /// <code>
        /// this.SkipDataCheckIds = new string[] { "cboDept", "cboEmployee",
        ///     "txtEntryDate" };
        /// </code>
        /// VB.NET:
        /// <code>
        /// Me.SkipDataCheckIds = New String() { "cboDept", "cboEmployee", _
        ///     "txtEntryDate"  }
        /// </code>
        /// </example>
        public string[] SkipDataCheckIds
        {
            get
            {
                Object oSkipList = ViewState["SkipList"];
                return (oSkipList != null) ? (string[])oSkipList : null;
            }
            set { ViewState["SkipList"] = value; }
        }

        /// <summary>
        /// This property is used to indicate whether or not the page
        /// should e-mail itself to someone when rendered.
        /// </summary>
        /// <value>Set it to true to have the page e-mail itself.  It is
        /// set to false by default.  In order to send the page, you must
        /// handle the <see cref="EMailThisPage"/> event in your derived
        /// class.</value>
        /// <remarks>If this property is set to true at some point before the
        /// page renders itself, it will build an e-mail message containing
        /// the contents of the rendered page.  It will raise a custom event
        /// (<see cref="EMailThisPage"/>) so that the derived page can set the
        /// sender, recipient, subject, and other items as well as modify the
        /// e-mail's content and the information rendered to the client.  It
        /// also raises a custom error event (<see cref="EMailError"/>) if the
        /// e-mail cannot be sent thus giving the page a chance to take
        /// alternate actions.</remarks>
        public bool EMailRenderedPage
        {
            get { return emailRenderedPage; }
            set { emailRenderedPage = value; }
        }

        /// <summary>
        /// This read-only property can be used to determine if the page is
        /// currently in the process of rendering itself for e-mailing.
        /// </summary>
        /// <remarks>This is used by the <b>Render</b> method to determine
        /// whether or not it needs to check to see if it needs to render
        /// for e-mailing or if it is already doing so.</remarks>
        [Browsable(false)]
        public bool IsRenderingForEMail
        {
            get { return isRenderingForEMail; }
        }

        /// <summary>
        /// The page title property.  This will be rendered in a &lt;title&gt;
        /// tag in the &lt;head&gt; section of the page's HTML.
        /// </summary>
        /// <value>Unlike <b>Page.Title</b>, this value is stored in view state
        /// so that it can be modified by events or other methods in derived
        /// classes.  If not specified, the page will not have a title unless
        /// you add it to the master page's HTML.</value>
        public string PageTitle
        {
            get { return (string)ViewState["PageTitle"]; }
            set { ViewState["PageTitle"] = value; }
        }

        /// <summary>
        /// This property is used to set or get the information for the page
        /// description meta tag that is rendered in the <b>&lt;head&gt;</b>
        /// section.
        /// </summary>
        /// <value>The value is stored in view state so that it can be
        /// modified by events or other methods in derived classes.</value>
        public string PageDescription
        {
            get
            {
                Object oDesc = ViewState["PageDesc"];
                return (string)oDesc;
            }
            set { ViewState["PageDesc"] = value; }
        }

        /// <summary>
        /// This property is used to set or get the information for the page
        /// keywords meta tag that is rendered in the <b>&lt;head&gt;</b>
        /// section.
        /// </summary>
        /// <value>The value is stored in view state so that it can be
        /// modified by events or other methods in derived classes.</value>
        public string PageKeywords
        {
            get
            {
                Object oKeywords = ViewState["PageKeywords"];
                return (string)oKeywords;
            }
            set { ViewState["PageKeywords"] = value; }
        }

        /// <summary>
        /// This property is used to set or get the value for the robot
        /// instructions meta tag that is rendered in the <b>&lt;head&gt;</b>
        /// section.
        /// </summary>
        /// <value>The default is <see cref="RobotOptions.NotSet"/> which
        /// will skip rendering of the tag.  The value is stored in view state
        /// so that it can be modified by events or other methods in derived
        /// classes.</value>
        public RobotOptions Robots
        {
            get
            {
                Object oRobots = ViewState["Robots"];
                return (oRobots == null) ? RobotOptions.NotSet :
                    (RobotOptions)oRobots;
            }
            set { ViewState["Robots"] = value; }
        }

        //=====================================================================
        // Events

        /// <summary>
        /// This event is raised when the page wants to e-mail itself.  An
        /// event handler in the derived class should fill in the sender,
        /// recipient, subject, and any other items on the message.
        /// </summary>
        public event EventHandler<EMailPageEventArgs> EMailThisPage;

        /// <summary>
        /// This raises the <see cref="EMailThisPage"/> event
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnEMailThisPage(EMailPageEventArgs e)
        {
            if(EMailThisPage != null)
                EMailThisPage(this, e);
        }

        /// <summary>
        /// This event is raised when there is an error trying to send the
        /// e-mail.  It gives the derived page a chance to take alternate
        /// action.
        /// </summary>
        public event EventHandler<EMailErrorEventArgs> EMailError;

        /// <summary>
        /// This raises the <see cref="EMailError"/> event
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnEMailError(EMailErrorEventArgs e)
        {
            if(EMailError != null)
                EMailError(this, e);
        }

        //=====================================================================
        // Private class methods

        /// <summary>
        /// This is used to find the form on the page
        /// </summary>
        private static Control FindPageForm(Page page)
        {
            MasterPage master;
            Control form = null;

            // Find the form on this page if there is one
            foreach(Control ctlItem in page.Controls)
                if(ctlItem is System.Web.UI.HtmlControls.HtmlForm)
                {
                    form = ctlItem;
                    break;
                }

            // If not there and it has a master page, search all
            // master pages.
            if(form == null)
            {
                master = page.Master;

                while(master != null && form == null)
                {
                    foreach(Control ctlItem in master.Controls)
                        if(ctlItem is System.Web.UI.HtmlControls.HtmlForm)
                        {
                            form = ctlItem;
                            break;
                        }

                    master = master.Master;
                }
            }

            return form;
        }

         //=====================================================================
        // Methods, etc

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>The constructor defaults the <see cref="PageForm"/>
        /// property to the page itself.  The <see cref="OnInit"/> method will
        /// attempt to locate the form control and set <see cref="PageForm"/>
        /// to it when it is called.</remarks>
        public BasePage()
        {
            ctlForm = this;
        }

        /// <summary>
        /// This sets the control that should have the focus when the page
        /// has finished loading by control reference.
        /// </summary>
        /// <param name="ctl">The control to give focus</param>
        /// <remarks>Use this for controls that are children of the form
        /// control and are not embedded within other controls such as data
        /// grids.
        /// <p/>To clear the focus, pass null (Nothing in VB.NET) to this
        /// method.  When doing so, you will need to use a cast to indicate
        /// that it is of the <b>WebControl</b> type due to the overload.</remarks>
        /// <overloads>This method has two overloads.</overloads>
        /// <example>
        /// C#:
        /// <code>
        /// // txtName is a control on the form
        /// this.SetFocusExtended(txtName);
        ///
        /// // Clear the focus
        /// this.SetFocusExtended((WebControl)null);
        /// </code>
        /// VB.NET:
        /// <code>
        /// ' txtName is a control on the form
        /// Me.SetFocusExtended(txtName)
        ///
        /// ' Clear the focus
        /// Me.SetFocusExtended(CType(Nothing, WebControl));
        /// </code>
        /// </example>
        public void SetFocusExtended(WebControl ctl)
        {
            if(ctl != null)
            {
                focusedControl = ctl.ClientID;
                findControl = false;
            }
            else
                focusedControl = null;
        }

        /// <summary>
        /// This sets the control that should have the focus when the page
        /// has finished loading by control ID.
        /// </summary>
        /// <param name="clientId">The ID of the control to give focus</param>
        /// <remarks>This version is useful for setting the focus to a control
        /// in a data grid's edit item template.  The control doesn't always
        /// exist when you want to set focus so this allows it to be set by
        /// control ID.  The grid mangles the name based on the row so it
        /// will be located by searching for the ID ending in the specified
        /// value.
        /// <p/>To clear the focus, pass null (Nothing in VB.NET) to this
        /// method.  When doing so, you will need to use a cast to indicate
        /// that it is of the <b>String</b> type due to the overload.</remarks>
        /// <example>
        /// C#:
        /// <code>
        /// // If embedded in a data grid, the control ID when rendered will
        /// // be something like dgGrid:_ctl5:txtName.
        /// this.SetFocusExtended("txtName");
        ///
        /// // Clear the focus
        /// this.SetFocusExtended((string)null);
        /// </code>
        /// VB.NET:
        /// <code>
        /// ' If embedded in a data grid, the control ID when rendered will
        /// ' be something like dgGrid:_ctl5:txtName.
        /// Me.SetFocusExtended("txtName")
        ///
        /// ' Clear the focus
        /// Me.SetFocusExtended(CType(Nothing, String));
        /// </code>
        /// </example>
        public void SetFocusExtended(string clientId)
        {
            focusedControl = clientId;
            findControl = true;
        }

        /// <summary>
        /// This method can be used to enable or disable a control and change the CSS style for the disabled
        /// state.  No class is used for the enabled state.
        /// </summary>
        /// <param name="ctl">The control to enable/disable</param>
        /// <param name="enabled">Pass true to enable, false to disable</param>
        /// <remarks>This version uses the current value of the <see cref="DisabledCssClass"/> property when
        /// disabling a control.  If being enabled, the style is simply removed.</remarks>
        /// <exception cref="ArgumentNullException">This is thrown if the specified control is null</exception>
        /// <overloads>This method has four overloads.</overloads>
        [Obsolete("Just set the control's Enabled property directly")]
        public void SetEnabledState(WebControl ctl, bool enabled)
        {
            this.SetEnabledState(ctl, enabled, null);
        }

        /// <summary>
        /// This method can be used to enable or disable a control and change the CSS style for the enabled and
        /// disabled states.
        /// </summary>
        /// <param name="ctl">The control to enable/disable</param>
        /// <param name="enabled">Pass true to enabled, false to disable</param>
        /// <param name="normalClass">The CSS class to apply to enabled controls</param>
        /// <remarks>This version uses the current value of the <see cref="DisabledCssClass"/> property when
        /// disabling a control.  If being enabled, is uses the specified class name.</remarks>
        /// <exception cref="ArgumentNullException">This is thrown if the specified control is null</exception>
        [Obsolete("Just set the control's Enabled property directly")]
        public void SetEnabledState(WebControl ctl, bool enabled, string normalClass)
        {
            if(ctl == null)
                throw new ArgumentNullException("ctl", "The control cannot be null");

            ctl.Enabled = enabled;
        }

        /// <summary>
        /// This method can be used to enable or disable multiple controls
        /// </summary>
        /// <param name="enabled">Pass true to enabled, false to disable</param>
        /// <param name="ctlList">A set of the controls to enable/disable</param>
        /// <exception cref="ArgumentNullException">This is thrown if the specified control array is null</exception>
        /// <example>
        /// C#:
        /// <code>
        /// this.SetEnabledState(false, this.ctl1, this.ctl2, this.ctl3);
        /// </code>
        /// VB.NET:
        /// <code>
        /// Me.SetEnabledState(False, Me.ctl1, Me.ctl2, Me.ctl3);
        /// </code>
        /// </example>
        [Obsolete("Just set each control's Enabled property directly")]
        public void SetEnabledState(bool enabled, params WebControl[] ctlList)
        {
            if(ctlList == null)
                throw new ArgumentNullException("ctlList", "Control array cannot be null");

            foreach(WebControl ctl in ctlList)
                ctl.Enabled = enabled;
        }

        /// <summary>
        /// This method can be used to enable or disable multiple controls and change the CSS style for the
        /// enabled or disabled state.
        /// </summary>
        /// <param name="normalClass">The CSS class to apply to enabled controls</param>
        /// <param name="enabled">Pass true to enabled, false to disable</param>
        /// <param name="ctlList">A set of the controls to enable/disable</param>
        /// <exception cref="ArgumentNullException">This is thrown if the
        /// specified control array is null</exception>
        [Obsolete("Just set each control's Enabled property directly")]
        public void SetEnabledState(string normalClass, bool enabled, params WebControl[] ctlList)
        {
            if(ctlList == null)
                throw new ArgumentNullException("ctlList",
                    "Control array cannot be null");

            foreach(WebControl ctl in ctlList)
                ctl.Enabled = enabled;
        }

        /// <summary>
        /// This can be used to disable or enable all edit controls on a web page, form, panel, or tab control.
        /// </summary>
        /// <param name="enabled">Set to true to enable, false to disable</param>
        /// <param name="ctlPageForm">The form, page, or other control containing the controls to disable or
        /// enable.  Normally, you will pass the value of the <see cref="PageForm"/> property.</param>
        /// <remarks>This method is aware of the Microsoft Internet Explorer Web Controls <b>MultiPage</b> and
        /// <b>PageView</b> and will also enable or disable controls contained within them.  Note that there is
        /// no dependency on that assembly due to the way the support for it has been implemented.</remarks>
        public void SetEnabledAll(bool enabled, Control ctlPageForm)
        {
            Control form = null;
            string controlType;

            // If null, default to the current page
            if(ctlPageForm == null)
                ctlPageForm = this.PageForm;

            // Yes, I could add a reference to the MS IE Web Controls, but I don't want this library to have a
            // dependency on it so we'll just check for IE Web Controls by type name string instead.
            controlType = ctlPageForm.ToString();

            // If passed a form, panel, multi-page, or page view, use it directly.  If passed a page, see if it
            // contains a form.  If so, use that form.  If not, use the page.
            if(ctlPageForm is System.Web.UI.HtmlControls.HtmlForm ||
               ctlPageForm is System.Web.UI.WebControls.ContentPlaceHolder ||
               ctlPageForm is System.Web.UI.WebControls.Panel ||
               ctlPageForm is System.Web.UI.WebControls.MultiView ||
               ctlPageForm is System.Web.UI.WebControls.View ||
               controlType.IndexOf("MultiPage", StringComparison.Ordinal) != -1 ||
               controlType.IndexOf("PageView", StringComparison.Ordinal) != -1)
            {
                form = ctlPageForm;
            }
            else
                if(ctlPageForm is System.Web.UI.Page && ctlPageForm != this.PageForm)
                    form = BasePage.FindPageForm((Page)ctlPageForm);

            // Ignore anything unexpected
            if(form == null)
                return;

            // Disable each edit control on the page
            foreach(Control ctl in form.Controls)
                if(ctl is System.Web.UI.WebControls.TextBox ||
                   ctl is System.Web.UI.WebControls.DropDownList ||
                   ctl is System.Web.UI.WebControls.ListBox ||
                   ctl is System.Web.UI.WebControls.CheckBox ||
                   ctl is System.Web.UI.WebControls.CheckBoxList ||
                   ctl is System.Web.UI.WebControls.RadioButton ||
                   ctl is System.Web.UI.WebControls.RadioButtonList)
                    ((WebControl)ctl).Enabled = enabled;
                else
                {
                    // As above, done this way to avoid a dependency
                    controlType = ctl.ToString();

                    if(ctl is System.Web.UI.WebControls.ContentPlaceHolder ||
                      ctl is System.Web.UI.WebControls.Panel ||
                      ctl is System.Web.UI.WebControls.MultiView ||
                      ctl is System.Web.UI.WebControls.View ||
                      controlType.IndexOf("MultiPage", StringComparison.Ordinal) != -1 ||
                      controlType.IndexOf("PageView", StringComparison.Ordinal) != -1)
                        this.SetEnabledAll(enabled, ctl);   // Recursive for these
                }
        }

        /// <summary>
        /// This is used to turn a message into a hyperlink so that clicking
        /// on the message will set the focus to the specified control.
        /// </summary>
        /// <param name="id">The ID of the control to focus when the message
        /// is clicked</param>
        /// <param name="msg">The message to turn into a link</param>
        /// <param name="cssClass">The CSS class name to use on the link,
        /// if any.  If passed a null, it uses the value of the
        /// <see cref="MsgLinkCssClass"/> property.</param>
        /// <returns>The specified message as a hyperlink tag</returns>
        /// <remarks>This is useful for validator messages displayed in a
        /// <see cref="System.Web.UI.WebControls.ValidationSummary"/> control.
        /// Clicking the error message takes you straight to the offending
        /// control in the form.</remarks>
        /// <example>
        /// C#:
        /// <code>
        /// // Convert a validator error message to a hyperlink
        /// val.ErrorMessage = this.MakeMsgLink(val.ControlToValidate,
        ///     val.ErrorMessage, this.MsgLinkCssClass);
        /// </code>
        /// VB.NET:
        /// <code>
        /// ' Convert a validator error message to a hyperlink
        /// val.ErrorMessage = Me.MakeMsgLink(val.ControlToValidate,
        ///     val.ErrorMessage, Me.MsgLinkCssClass);
        /// </code>
        /// </example>
        public string MakeMsgLink(string id, string msg, string cssClass)
        {
            string newClass;

            // Don't bother if it's null, empty, or already in the form of
            // a link.
            if(msg == null || msg.Length == 0 ||
              msg.StartsWith("<a ", StringComparison.OrdinalIgnoreCase))
                return msg;

            StringBuilder sb = new StringBuilder(512);

            // Add the anchor tag and the optional CSS class
            sb.Append("<a ");

            newClass = (cssClass == null) ? this.MsgLinkCssClass : cssClass;

            if(newClass != null && newClass.Length > 0)
            {
                sb.Append("class='");
                sb.Append(newClass);
                sb.Append("' ");
            }

            // An HREF is included that does nothing so that we can use the
            // hover style to do stuff like underline the link when the mouse
            // is over it.  OnClick performs the action and returns false so
            // that we don't trigger IE's OnBeforeUnload event which may be
            // tied to data change checking code.

            // NOTE: OnPreRender registers the script containing the function.
            // Tell the function to use the "Find Control" method to locate
            // the ID.  That way, it works for controls embedded in data grids.
            sb.Append("href='javascript:return false;' " +
                "onclick='javascript: return BP_funSetFocus(\"");
            sb.Append(id);
            sb.Append("\", true);'>");
            sb.Append(msg);
            sb.Append("</a>");

            return sb.ToString();
        }

        /// <summary>
        /// OnInit is overridden to locate the form control on the page and
        /// set the <see cref="PageForm"/> property to it.  For postbacks, it
        /// will also retrieve the value of the dirty flag and store it in
        /// the <see cref="Dirty"/> property.
        /// </summary>
        /// <param name="e">Event arguments</param>
        /// <remarks>If you override this method in a derived class, you
        /// must call this version too.</remarks>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Retrieve the state of the Dirty flag (if used)
            if(!this.DesignMode)
                this.Dirty = Convert.ToBoolean(Request.Form["BP_bIsDirty"], CultureInfo.InvariantCulture);

            // Find the form on the page if there is one
            if(ctlForm is System.Web.UI.Page)
            {
                Control form = BasePage.FindPageForm((Page)ctlForm);

                if(form != null)
                    ctlForm = form;
            }
        }

        /// <summary>
        /// OnPreRender is overridden to generate the data change checking
        /// variables and script and the focused control script if necessary.
        /// It also adds some additional tags to the page header if needed.
        /// </summary>
        /// <param name="e">Event arguments</param>
        /// <remarks>If you override this method in a derived class, you
        /// must call this version too.
        /// <p/>If a page header control exists, the following additional tags
        /// are inserted into it using the values from the related properties
        /// in this class.
        /// <code>
        /// &lt;meta name="GENERATOR" content="APPNAME.CLASSNAME Class"&gt;
        /// &lt;meta name="Title" content="BasePage.PageTitle"&gt;
        /// &lt;meta name="Description" content="BasePage.PageDescription"&gt;
        /// &lt;meta name="Keywords" content="BasePage.PageKeywords"&gt;
        /// &lt;meta name="Robots" content="BasePage.Robots"&gt;
        /// </code>
        /// <p/>The <b>APPNAME.CLASSNAME</b> section of the &lt;meta name&gt;
        /// tag will contain the class name of the object that generated the
        /// HTML page.  <b>BasePage.PageTitle</b> is also used to set the
        /// <b>Page.Title</b> property.
        /// </remarks>
        protected override void OnPreRender(EventArgs e)
        {
            StringBuilder sb;
            string[] idList;

            base.OnPreRender(e);

            // Add additional tags to the page header if needed
            if(this.Page.Header != null)
            {
                sb = new StringBuilder(1024);

                // Show the class that generated this output.  Note that
                // it uses BaseType so that it shows the actual class name
                // rather than the ASP.NET generated derived page class name.
                sb.AppendFormat("<meta name=\"GENERATOR\" content=\"{0}" +
                    " Class\">\n", this.GetType().BaseType);

                // Output description and keywords
                if(this.PageDescription != null)
                    sb.AppendFormat("<meta name=\"description\" " +
                        "content=\"{0}\">\n", this.PageDescription);

                if(this.PageKeywords != null)
                    sb.AppendFormat("<meta name=\"keywords\" " +
                        "content=\"{0}\">\n", this.PageKeywords);

                // Output page title
                if(this.PageTitle != null)
                {
                    sb.AppendFormat("<meta name=\"Title\" content=\"{0}" +
                        "\">\n", this.PageTitle);
                    this.Page.Title = this.PageTitle;
                }

                // Output robot instructions
                if(this.Robots != RobotOptions.NotSet)
                    sb.AppendFormat("<meta name=\"Robots\" content=\"{0}" +
                        "\">\n", this.Robots.ToString());

                this.Page.Header.Controls.Add(new LiteralControl(sb.ToString()));
            }

            // If data change checking has been requested, output the dirty
            // flag, exclusion arrays, confirm message, and the script.
            if(this.CheckForDataChanges == true)
            {
                // Register a hidden field so that the client can pass
                // back changes to the Dirty flag.
                this.Page.ClientScript.RegisterHiddenField("BP_bIsDirty",
                    this.Dirty.ToString(CultureInfo.InvariantCulture).ToLower(
                    CultureInfo.InvariantCulture));

                // Register an OnSubmit function call so that we can get the
                // state of the Dirty flag and put it in the hidden field.  It
                // can't occur in the OnBeforeUnload event as everything has
                // been packaged up ready for sending to the server and changes
                // made in that event don't get sent to the server.
                this.Page.ClientScript.RegisterOnSubmitStatement(
                    typeof(BasePage), "BP_DirtyCheck",
                    "BP_funCheckForChanges(true);");

                // Create a script block containing the array declarations,
                // the data loss message variable, the dirty flag, and the
                // change checking script.
                sb = new StringBuilder(
                    "<script type='text/javascript'>\n<!--\n" +
                    "var BP_arrBypassList = new Array(", 4096);

                idList = this.BypassPromptIds;
                if(idList != null)
                {
                    sb.Append('\"');
                    sb.Append(String.Join("\",\"", idList));
                    sb.Append('\"');
                }

                sb.Append(");\nvar BP_arrSkipList = new Array(");
                idList = this.SkipDataCheckIds;
                if(idList != null)
                {
                    sb.Append('\"');
                    sb.Append(String.Join("\",\"", idList));
                    sb.Append('\"');
                }

                sb.Append(");\nvar BP_strDataLossMsg = \"");
                sb.Append(this.ConfirmLeaveMessage);
                sb.Append("\";\nvar BP_strFormName = \"");

                // BP_strFormName tells the script what form to use
                // for the change checking.
                sb.Append(this.PageForm.UniqueID);
                sb.Append("\";\n//-->\n</script>\n");

                this.Page.ClientScript.RegisterClientScriptBlock(
                    typeof(BasePage), "BP_DCCJS", sb.ToString());

                this.Page.ClientScript.RegisterClientScriptInclude(
                    typeof(BasePage), "BP_DCCJSFile",
                    this.Page.ClientScript.GetWebResourceUrl(typeof(BasePage),
                    BasePage.ScriptsPath + "DataChange.js"));
            }

            // If a focused control has been specified or there are validators
            // on the page, register the "set focus" script.  The validators
            // may use it if MakeMsgLink converts any error messages to links.
            if(focusedControl != null || this.Validators.Count > 0)
            {
                // Generate script to set the focused control?
                if(focusedControl != null)
                {
                    sb = new StringBuilder();

                    sb.Append("<script type='text/javascript'>\n" +
                        "<!--\nBP_funSetFocus('");

                    sb.Append(focusedControl);

                    if(findControl == false)
                        sb.Append("', false);\n");
                    else
                        sb.Append("', true);\n");

                    sb.Append("//-->\n</script>");

                    this.Page.ClientScript.RegisterStartupScript(
                        typeof(BasePage), "BP_FocusCtl", sb.ToString());
                }

                // Add the reference to retrieve the script resource
                this.Page.ClientScript.RegisterClientScriptInclude(
                    typeof(BasePage), "BP_SFJSFile",
                    this.Page.ClientScript.GetWebResourceUrl(typeof(BasePage),
                    BasePage.ScriptsPath + "SetFocus.js"));
            }
        }

        /// <summary>
        /// OnError is overridden so that the page can save all error context
        /// information to the application cache.  A custom error page can
        /// then retrieve it and display the information to the user, write
        /// it to the event log, etc.
        /// </summary>
        /// <param name="e">Event arguments</param>
        /// <remarks>By default, this method will store the following
        /// information in a hash table and place it in the application cache.
        /// <list type="table">
        ///    <listheader>
        ///       <term>Hash Table Key</term>
        ///       <description>Description</description>
        ///    </listheader>
        ///    <item>
        ///       <term>LastError</term>
        ///       <description>This element contains a string that represents
        /// the information returned from
        /// <b>Server.GetLastError().ToString()</b>.</description>
        ///    </item>
        ///    <item>
        ///       <term>ServerVars</term>
        ///       <description>This element contains a <b>SortedList</b> that
        /// contains the following entries from the
        /// <b>Request.ServerVariables</b> collection:
        ///     <list type="bullet">
        ///         <item>SCRIPT_NAME</item>
        ///         <item>HTTP_HOST</item>
        ///         <item>HTTP_USER_AGENT</item>
        ///         <item>AUTH_TYPE</item>
        ///         <item>AUTH_USER</item>
        ///         <item>LOGON_USER</item>
        ///         <item>SERVER_NAME</item>
        ///         <item>LOCAL_ADDR</item>
        ///         <item>REMOTE_ADDR</item>
        ///     </list>
        ///       </description>
        ///    </item>
        ///    <item>
        ///       <term>QueryString</term>
        ///       <description>This element contains a copy of the
        /// <b>Request.QueryString</b> collection.</description>
        ///    </item>
        ///    <item>
        ///       <term>Form</term>
        ///       <description>This element contains a copy of the
        /// <b>Request.Form</b> collection.</description>
        ///    </item>
        ///    <item>
        ///       <term>Page</term>
        ///       <description>This contains a string representing the name
        /// of the page that caused the error as returned by
        /// <b>Request.Path</b>.</description>
        ///    </item>
        /// </list>
        /// <p/>The hash table is stored in the application cache using the
        /// current user's remote address as obtained by the <b>REMOTE_ADDR</b>
        /// server variable.  The entries are stored with a time limit of
        /// five minutes so that it doesn't hold resources for too long.
        /// The custom error page can also delete the object from the cache
        /// once it has retrieved it.  This method should work well for most
        /// applications.  Unless you are expecting an extremely large number
        /// of users and there was an unexpected error that everyone got, it
        /// shouldn't put much of a load on the server.</remarks>
        protected override void OnError(System.EventArgs e)
        {
            string remoteAddr;

            if(!this.DesignMode)
            {
                Hashtable htErrorContext = new Hashtable(5);
                SortedList slServerVars = new SortedList(9);

                // Extract a subset of the server variables
                slServerVars["SCRIPT_NAME"] = Request.ServerVariables["SCRIPT_NAME"];
                slServerVars["HTTP_HOST"] = Request.ServerVariables["HTTP_HOST"];
                slServerVars["HTTP_USER_AGENT"] = Request.ServerVariables["HTTP_USER_AGENT"];
                slServerVars["AUTH_TYPE"] = this.AuthType;
                slServerVars["AUTH_USER"] = Request.ServerVariables["AUTH_USER"];
                slServerVars["LOGON_USER"] = Request.ServerVariables["LOGON_USER"];
                slServerVars["SERVER_NAME"] = Request.ServerVariables["SERVER_NAME"];
                slServerVars["LOCAL_ADDR"] = Request.ServerVariables["LOCAL_ADDR"];
                remoteAddr = Request.ServerVariables["REMOTE_ADDR"];
                slServerVars["REMOTE_ADDR"] = remoteAddr;

                // Save the context information
                htErrorContext["LastError"] = Server.GetLastError().ToString();
                htErrorContext["ServerVars"] = slServerVars;
                htErrorContext["QueryString"] = Request.QueryString;
                htErrorContext["Form"] = Request.Form;
                htErrorContext["Page"] = Request.Path;

                // Store it in the cache with a short time limit.  The remote
                // address is used as a key.  We can't use the session ID or store
                // the info in the session as it's not always the same session on
                // the error page.
                Cache.Insert(remoteAddr, htErrorContext,
                    null, DateTime.MaxValue, TimeSpan.FromMinutes(5));
            }

            base.OnError(e);
        }

        /// <summary>
        /// Convert validation summary error messages to message links.
        /// </summary>
        /// <remarks>This method is called by <see cref="Render"/> to convert
        /// all message strings in validation controls that will appear in a
        /// <see cref="System.Web.UI.WebControls.ValidationSummary"/> control
        /// into message links.  It uses the <see cref="MakeMsgLink"/> method
        /// to do this.  To be converted, the validator control must be
        /// visible, must have its
        /// <see cref="System.Web.UI.WebControls.BaseValidator.ControlToValidate"/>
        /// property set to a control ID, and must have its
        /// <see cref="System.Web.UI.WebControls.BaseValidator.Display"/>
        /// property set to the value
        /// <see cref="System.Web.UI.WebControls.ValidatorDisplay">None</see>.
        /// <p/>This method can be overridden by derived classes to extend
        /// or suppress this behavior.
        /// </remarks>
        /// <seealso cref="Render"/>
        protected virtual void ConvertValMsgsToLinks()
        {
            BaseValidator bv;

            foreach(IValidator val in this.Validators)
            {
                bv = val as BaseValidator;

                if(bv != null && bv.Visible == true &&
                  bv.ControlToValidate.Length > 0 &&
                  bv.Display == ValidatorDisplay.None)
                    bv.ErrorMessage = MakeMsgLink(bv.ControlToValidate,
                        bv.ErrorMessage, this.MsgLinkCssClass);
            }
        }

        /// <summary>
        /// This is used to e-mail the page when necessary.
        /// </summary>
        /// <param name="writer">The HTML writer to which the output is written</param>
        /// <remarks>This renders the page and then raises the
        /// <see cref="EMailThisPage"/> event to give the derived class a
        /// chance to fill in the necessary sender and recipient information
        /// and also a chance to cancel sending the e-mail.  If not cancelled,
        /// the e-mail will be sent.  If there is an error while trying to send
        /// it, the <see cref="EMailError"/> event is raised to give the
        /// derived class a chance to take alternate action.</remarks>
        /// <event cref="EMailThisPage">This event is raised to let the derived
        /// page class fill in the sender, recipient, subject, etc.</event>
        /// <event cref="EMailError">This event is raised if an error occurs
        /// while trying to send the e-mail and gives the derived page a chance
        /// to take action.</event>
        /// <exception cref="System.InvalidOperationException">The e-mail recipient
        /// must be specified via the <b>To</b> property of the
        /// <see cref="EMailPageEventArgs.EMail"/> message object.</exception>
        protected void RenderForEMail(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();
            HtmlTextWriter hw = new HtmlTextWriter(new StringWriter(sb,
                CultureInfo.InvariantCulture));

            try
            {
                isRenderingForEMail = true;
                this.Render(hw);
            }
            finally
            {
                isRenderingForEMail = false;
            }

            MailMessage msg = new MailMessage();

            // The string builder contains all of the HTML
            msg.IsBodyHtml = true;
            msg.Body = sb.ToString();

            EMailPageEventArgs args = new EMailPageEventArgs(msg, this);

            // Get the SMTP server from the Web.config file if specified
            args.SmtpServer = ConfigurationManager.AppSettings["EMailPage_SmtpServer"];

            // Give the derived page a chance to modify or cancel the e-mail
            this.OnEMailThisPage(args);

            if(args.Cancel == false)
            {
                SmtpClient smtpClient = new SmtpClient();

                // Try sending until it succeeds or told to stop
                do
                {
                    try
                    {
                        // Turn off retry so we don't get stuck here.
                        // The error event handler must turn it on if
                        // a retry is wanted.
                        args.RetryOnFailure = false;

                        // It has to come from somebody
                        if(msg.From == null)
                            throw new InvalidOperationException(
                                "A sender must be specified for the e-mail message");

                        // It has to go to somebody
                        if(msg.To.Count == 0)
                            throw new InvalidOperationException(
                                "A recipient must be specified for the e-mail message");

                        // Set the server?
                        if(args.SmtpServer != null &&
                          args.SmtpServer.Length != 0)
                            smtpClient.Host = args.SmtpServer;

                        smtpClient.Send(msg);
                    }
                    catch(Exception excp)
                    {
                        // Raise an EMailError event if it fails so that
                        // the derived page can take any action it wants
                        // including a retry.
                        EMailErrorEventArgs err = new EMailErrorEventArgs(
                            args, excp);
                        this.OnEMailError(err);
                    }

                } while(args.RetryOnFailure == true);
            }

            msg.Dispose();

            // And finally, write the HTML to the original writer.  It's
            // rendered from the event args in case the handler modified it.
            writer.Write(args.RenderedContent);
        }

        /// <summary>
        /// This is overridden to convert validation messages to hyperlinks and
        /// handle e-mailing the rendered page content if necessary.
        /// </summary>
        /// <param name="writer">The HTML writer to which the output is written</param>
        /// <remarks>If the <see cref="EMailRenderedPage"/> property has been
        /// set to true, the page will attempt to e-mail itself by calling the
        /// <see cref="RenderForEMail"/> method.  If set to false, the page is
        /// rendered normally.</remarks>
        /// <seealso cref="ConvertValMsgsToLinks"/>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            // If already rendering for e-mail, just render the page
            if(!this.IsRenderingForEMail)
            {
                this.ConvertValMsgsToLinks();

                // Rendering normally or going to e-mail?
                if(this.EMailRenderedPage)
                {
                    this.RenderForEMail(writer);
                    return;
                }
            }

            base.Render(writer);
        }
    }
}
