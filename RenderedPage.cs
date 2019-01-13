//=============================================================================
// System  : ASP.NET Common Web Page Classes
// File    : RenderedPage.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 03/10/2006
// Note    : Copyright 2002-2006, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a base page class used by ASP.NET applications that
// renders the common header and footer HTML.  This is mostly useful for
// .NET 1.1 applications.  For .NET 2.0, master pages are usually a better
// choice.
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
// 2.0.0.0  02/18/2006  EFW  Refactored the base page class to move the
//                           rendering related code to its own class.
//=============================================================================

using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

// All classes go in the EWSoftware.Web namespace
namespace EWSoftware.Web
{
    /// <summary>
    /// This file contains a base page class used by ASP.NET applications that
    /// renders the common header and footer HTML.  This is mostly useful for
    /// .NET 1.1 applications.  For .NET 2.0, master pages are usually a better
    /// choice.
    /// </summary>
    /// <remarks>Various properties are provided for such things as the page
    /// title, cascading style sheet, etc.  It outputs all the common header
    /// and footer HTML tags so they don't have to appear in the body of
    /// pages derived from this class.  By using this class as the base class
    /// for your web pages, you can reduce the amount of code and HTML that
    /// you have to write.</remarks>
    public class RenderedPage : EWSoftware.Web.BasePage
    {
        //=====================================================================
        // Constants

        /// <summary>
        /// This constant refers to the name of the style sheet file used by
        /// the application.  The default value is <b>Styles.css</b>.  If you
        /// use a style sheet file by a different name, be sure to set it using
        /// the <see cref="PageStyleSheet"/> property.
        /// </summary>
        public const string CssFileName = "Styles.css";

        //=====================================================================
        // Properties

        /// <summary>
        /// This property is used to set the cascading style sheet to use
        /// for the page.  This will be rendered to a &lt;link&gt; tag in
        /// the &lt;head&gt; section of the page's HTML.
        /// </summary>
        /// <value>The default value is the <see cref="CssFileName"/>
        /// constant.</value>
        public string PageStyleSheet
        {
            get { return (string)ViewState["PageStyleSheet"]; }
            set { ViewState["PageStyleSheet"] = value; }
        }

        /// <summary>
        /// The CSS class name to use for the &lt;body&gt; tag.  It is
        /// useful for applying a style that modifies the page margins.
        /// </summary>
        /// <value>The class name should appear in a definition in the
        /// style sheet file specified by the <see cref="PageStyleSheet"/>
        /// property.  The <see cref="RenderedPage"/> class does not use
        /// it.</value>
        /// <seealso cref="MenuPage"/>
        public string PageBodyStyle
        {
            get { return (string)ViewState["PageBodyStyle"]; }
            set { ViewState["PageBodyStyle"] = value; }
        }

        //=====================================================================
        // Methods, etc

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>The constructor defaults the <see cref="PageStyleSheet"/>
        /// property to the value of the <see cref="CssFileName"/> constant.
        /// </remarks>
        public RenderedPage()
        {
            this.PageStyleSheet = RenderedPage.CssFileName;
        }

        /// <summary>
        /// Render common header tags.
        /// </summary>
        /// <param name="writer">The HTML writer to which the output is written</param>
        /// <remarks>Derived classes can override this method to add other
        /// common tags at the start of the page after the opening &lt;body&gt;
        /// tag.  If overridden, call this base method <b>before</b> outputting
        /// the additional tags unless you are completely replacing the
        /// rendering process for all of the header tags normally generated by
        /// this method.  If you just want to render additional tags within
        /// the &lt;head&gt; section of the page, override the
        /// <see cref="RenderAdditionalHeaderTags"/> method instead.  The
        /// <b>RenderHeader</b> method generates the following tags for you.
        /// <list type="table">
        ///    <listheader>
        ///       <term>Tag</term>
        ///       <description>Description</description>
        ///    </listheader>
        ///    <item>
        ///       <term>&lt;!DOCTYPE&gt;</term>
        ///       <description>A standard DOCTYPE tag is rendered:
        /// <code>
        /// &lt;!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"&gt;
        /// </code></description>
        ///    </item>
        ///    <item>
        ///       <term>&lt;html&gt;</term>
        ///       <description>The opening &lt;html&gt; tag is
        /// rendered.</description>
        ///    </item>
        ///    <item>
        ///       <term>&lt;head&gt; section</term>
        ///       <description>The &lt;head&gt; section will include
        /// &lt;meta&gt;, &lt;link&gt;, &lt;title&gt;, and other additional
        /// tags (if any) and will be closed with the &lt;head&gt;
        /// tag.</description>
        ///    </item>
        ///    <item>
        ///       <term>&lt;meta&gt; tags</term>
        ///       <description>The following &lt;meta&gt; tags are generated
        /// within the &lt;head&gt; section:
        /// <code>
        /// &lt;meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"&gt;
        /// &lt;meta http-equiv="Content-Language" content="en-us"&gt;
        /// &lt;meta http-equiv="Content-Script-Type" content="text/javascript"&gt;
        /// &lt;meta name="GENERATOR" content="APPNAME.CLASSNAME Class"&gt;
        /// &lt;meta name="Title" content="BasePage.PageTitle"&gt;
        /// &lt;meta name="Description" content="BasePage.PageDescription"&gt;
        /// &lt;meta name="Keywords" content="BasePage.PageKeywords"&gt;
        /// &lt;meta name="Robots" content="BasePage.Robots"&gt;
        /// </code>
        /// <p/>The <b>APPNAME.CLASSNAME</b> section of the &lt;meta name&gt;
        /// tag will contain the class name of the object that generated the
        /// HTML page.  The <b>RenderedPage.XXX</b> values are replace with the
        /// named property from the <b>RenderedPage</b> class.</description>
        ///    </item>
        ///    <item>
        ///       <term>&lt;link&gt; for style sheet</term>
        ///       <description>A &lt;link&gt; tag is inserted in the
        /// &lt;head&gt; section to reference the application style sheet
        /// if the <see cref="PageStyleSheet"/> property is not
        /// null.</description>
        ///    </item>
        ///    <item>
        ///       <term>&lt;title&gt;</term>
        ///       <description>A &lt;title&gt; tag is inserted in the
        /// &lt;head&gt; section to display the specified text in the
        /// browser window's title bar if the <see cref="BasePage.PageTitle"/>
        /// property is not null.</description>
        ///    </item>
        ///    <item>
        ///       <term>&lt;body&gt;</term>
        ///       <description>The opening &lt;body&gt; tag is inserted.
        /// If the <see cref="PageBodyStyle"/> property is not null, a
        /// <b>class</b> attribute is added to set the style for the
        /// &lt;body&gt; tag.</description>
        ///    </item>
        /// </list>
        /// <p/>Additional tags generated by the
        /// <see cref="RenderAdditionalHeaderTags"/> virtual method are
        /// inserted after the &lt;title&gt; tag and just before the closing
        /// &lt;/head&gt; tag.  The actual content of the page as defined in
        /// the ASPX file will be rendered immediately after the opening
        /// &lt;body&gt; tag.
        /// </remarks>
        /// <seealso cref="Render"/>
        protected virtual void RenderHeader(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder(1024);

            // Write out the stock header
            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD " +
                "HTML 4.01 Transitional//EN\">\n" +
                "<html>\n\n<head>\n" +
                "<meta http-equiv=\"Content-Type\" content=\"text/html; " +
                    "charset=iso-8859-1\">\n" +
                "<meta http-equiv=\"Content-Language\" content=\"en-us\">\n" +
                "<meta http-equiv=\"Content-Script-Type\" " +
                    "content=\"text/javascript\">\n");

            // Show the class that generated this output.  Note that it uses
            // BaseType so that it shows the actual class name rather than
            // the ASP.NET generated derived page class name.
            sb.AppendFormat("<meta name=\"GENERATOR\" content=\"{0}" +
                " Class\">\n", this.GetType().BaseType);

            // Output description and keywords
            if(this.PageDescription != null)
                sb.AppendFormat("<meta name=\"description\" " +
                    "content=\"{0}\">\n", this.PageDescription);

            if(this.PageKeywords != null)
                sb.AppendFormat("<meta name=\"keywords\" " +
                    "content=\"{0}\">\n", this.PageKeywords);

            // Output the application-specific style sheet if specified
            if(this.PageStyleSheet != null)
                sb.AppendFormat("<link rel=\"stylesheet\" " +
                    "type=\"text/css\" href=\"{0}\">\n", this.PageStyleSheet);

            // Output page title
            if(this.PageTitle != null)
                sb.AppendFormat("<meta name=\"Title\" content=\"{0}" +
                    "\">\n<title>{0}</title>\n", this.PageTitle);

            // Output robot instructions
            if(this.Robots != RobotOptions.NotSet)
                sb.AppendFormat("<meta name=\"Robots\" content=\"{0}" +
                    "\">\n", this.Robots.ToString());

            // Insert other header tags from derived classes (if any)
            this.RenderAdditionalHeaderTags(sb);

            sb.Append("</head>\n<body");

            // Output body style if specified
            if(this.PageBodyStyle != null)
            {
                sb.Append(" class='");
                sb.Append(this.PageBodyStyle);
                sb.Append('\'');
            }

            sb.Append(">\n\n");
            writer.Write(sb.ToString());
        }

        /// <summary>
        /// Render additional header tags other than those output by
        /// <see cref="RenderHeader"/>.
        /// </summary>
        /// <remarks>This method can be overridden by derived classes to add
        /// other tags inside the &lt;head&gt; section (i.e. other
        /// &lt;meta name&gt; tags for title, keywords, etc).  The base class
        /// version does nothing.</remarks>
        /// <param name="header">The string builder to which the tags are
        /// appended.  Append any additional tags to the string builder.  You
        /// are also free to modify the existing content of the header in the
        /// passed string builder object.</param>
        /// <seealso cref="Render"/>
        protected virtual void RenderAdditionalHeaderTags(StringBuilder header)
        {
        }

        /// <summary>Render common footer tags.</summary>
        /// <remarks>This method can be overridden by derived classes to add
        /// other common tags at the end of the body before the closing
        /// &lt;/body&gt; tag.  If overridden, call this base method
        /// <b>after</b> outputting the additional tags unless you are
        /// completely replacing the rendering process for all of the closing
        /// tags normally generated by this method.
        /// <p/>The <b>RenderFooter</b> method generates the following tags
        /// for you.
        /// <list type="table">
        ///    <listheader>
        ///       <term>Tag</term>
        ///       <description>Description</description>
        ///    </listheader>
        ///    <item>
        ///       <term>&lt;/body&gt;</term>
        ///       <description>The closing &lt;/body&gt; tag is generated.</description>
        ///    </item>
        ///    <item>
        ///       <term>&lt;/html&gt;</term>
        ///       <description>The closing &lt;/html&gt; tag is generated.</description>
        ///    </item>
        /// </list>
        /// </remarks>
        /// <param name="writer">The HTML writer to which the output is written</param>
        /// <seealso cref="Render"/>
        protected virtual void RenderFooter(HtmlTextWriter writer)
        {
            writer.Write("\n</body>\n</html>\n");
        }

        /// <summary>
        /// This is overridden to render the page with the common elements.
        /// </summary>
        /// <remarks>Unless it is necessary, you should not override this
        /// method to alter rendering of the page content.  Instead, override
        /// the methods <see cref="RenderHeader"/>,
        /// <see cref="RenderAdditionalHeaderTags"/>, and
        /// <see cref="RenderFooter"/> as needed.  <see cref="BasePage.OnInit"/>
        /// can also be overridden to insert controls via the
        /// <see cref="BasePage.PageForm"/> property.</remarks>
        /// <param name="writer">The HTML writer to which the output is written</param>
        /// <seealso cref="RenderHeader"/>
        /// <seealso cref="RenderAdditionalHeaderTags"/>
        /// <seealso cref="RenderFooter"/>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            // Render the page and e-mail it if necessary
            if(!this.IsRenderingForEMail && this.EMailRenderedPage)
            {
                this.RenderForEMail(writer);
                return;
            }

            this.RenderHeader(writer);

            // Render the content of the base and derived classes
            base.Render(writer);

            this.RenderFooter(writer);
        }
    }
}
