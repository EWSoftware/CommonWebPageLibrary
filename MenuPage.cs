//=============================================================================
// System  : ASP.NET Common Web Page Classes
// File    : MenuPage.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 03/10/2006
// Note    : Copyright 2002-2006, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains common menu page classes used by ASP.NET applications.
// They derive from RenderedPage and include support for a menu control that
// is rendered horizontally across the top or vertically down the left side
// of the page.
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
// 1.0.0.0  07/12/2002  EFW  Created the code
// 2.0.0.0  02/18/2006  EFW  Updated for use with .NET 2.0
//=============================================================================

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

// All classes go in the EWSoftware.Web namespace
namespace EWSoftware.Web
{
    /// <summary>
    /// This class will format a page with a table containing a menu user
    /// control across the top.  The rest of the page contains the derived
    /// class's contents.  To ensure that any controls in the menu can do
    /// postbacks, the table and menu control are inserted as children of the
    /// derived class's form control if one is present.
	/// </summary>
    /// <remarks>As with the <see cref="RenderedPage"/> class, this is really
    /// only useful in .NET 1.1 web applications.  For .NET 2.0, master pages
    /// are a better option.</remarks>
    public class MenuPage : EWSoftware.Web.RenderedPage
    {
        //=====================================================================
        // Constants

        /// <summary>
        /// This constant refers to the name of the menu control that will be
        /// loaded into the page.  The default value is <b>MenuCtrl.ascx</b>.
        /// </summary>
        public const string MenuCtrlFileName = "MenuCtrl.ascx";

        /// <summary>
        /// This constant refers to the CSS class name to use on the
        /// &lt;body&gt; tag for the page.  The default value is the class
        /// name <b>MenuPage</b>.
        /// </summary>
        public const string PageBodyCssClass = "MenuPage";

        //=====================================================================
        // Protected class members

        /// <summary>
        /// This contains the filename of the menu control (the ASCX file).
        /// It is set to the value of the <see cref="MenuCtrlFileName"/>
        /// constant by default.
        /// </summary>
        protected string menuControl;

        /// <summary>
        /// Set this flag to true if the menu will render itself vertically
        /// down the left side of the page rather than horizontally across
        /// the top.
        /// </summary>
        protected bool verticalMenu;

        //=====================================================================
        // Properties

	    /// <summary>
        /// The menu control file property.
        /// <p/><b>NOTE:</b> If you need to change it, do so in the derived
        /// class's constructor or <see cref="OnInit"/> event or it will not
        /// get loaded.
	    /// </summary>
    	public string MenuControlFile
        {
            get { return menuControl; }
            set { menuControl = value; }
    	}

        //=====================================================================
        // Methods, etc

        /// <summary>
        /// Default constructor.  Defaults: Horizontal menu, body style is
        /// set to the value of the <see cref="PageBodyCssClass"/> constant,
        /// and the <see cref="MenuControlFile"/> property is set to the value
        /// of the <see cref="MenuCtrlFileName"/> property.
        /// </summary>
        public MenuPage()
        {
            this.PageBodyStyle = MenuPage.PageBodyCssClass;
            menuControl = MenuPage.MenuCtrlFileName;
        }

        /// <summary>
        /// OnInit is overridden to add the table HTML and menu control
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Insert the table and menu control at the start of the page.
            // The layout depends on whether or not the menu is going to
            // be rendered horizontally at the top or vertically down the
            // left side of the page.
            if(verticalMenu)
            {
                this.PageForm.Controls.AddAt(0, new LiteralControl(
                    "<table height='100%' cellpadding='0' width='100%'>\n" +
                    "<tr valign='top'>\n  <td width='15%'>\n"));
            }
            else
            {
                this.PageForm.Controls.AddAt(0, new LiteralControl(
                    "<table cellpadding='0' width='100%'>\n" +
                    "<tr>\n<td>\n"));
            }

            if(menuControl != null)
                this.PageForm.Controls.AddAt(1, LoadControl(menuControl));
            else
                this.PageForm.Controls.AddAt(1, new LiteralControl(
                    "MenuControlFile property not set in derived OnInit!"));

            if(verticalMenu)
            {
                this.PageForm.Controls.AddAt(2,
                    new LiteralControl("</td><td>&nbsp;</td>\n<td>\n"));

                // Page content goes in between and this wraps it up
                this.PageForm.Controls.Add(
                    new LiteralControl("</td>\n</tr>\n</table>\n"));
            }
            else    // For a horizontal menu, the page is rendered below the menu
                this.PageForm.Controls.AddAt(2,
                    new LiteralControl("</td>\n</tr>\n</table>\n"));
        }
    }

    /// <summary>
    /// This class is the same as <see cref="MenuPage"/> but it renders the
    /// menu vertically down the left side of the page.
	/// </summary>
    public class VerticalMenuPage : MenuPage
    {
        /// <summary>
        /// Default constructor.  Defaults: Vertical menu, body style is
        /// set to the value of the
        /// <see cref="EWSoftware.Web.MenuPage.PageBodyCssClass"/> constant.
        /// </summary>
        public VerticalMenuPage()
        {
            verticalMenu = true;
        }
    }
}
