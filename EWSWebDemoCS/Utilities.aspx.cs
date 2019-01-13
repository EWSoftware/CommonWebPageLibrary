//=============================================================================
// System  : ASP.NET Common Web Page Classes Demo
// File    : Utilities.aspx.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 09/13/2013
// Note    : Copyright 2002-2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This demonstrates the features of the PageUtils utility class
//
// Version     Date     Who  Comments
// ============================================================================
// 1.0.0    11/29/2003  EFW  Created the code
//=============================================================================

using System;
using System.Text;
using System.Web.UI.WebControls;

using EWSoftware.Web;

namespace EWSWebDemoCS
{
	/// <summary>
	/// The utilities page
	/// </summary>
	public partial class Utilities : EWSoftware.Web.BasePage
	{

        // Return a test string to demonstrate the HTMLEncode() and MakeLinks()
        // methods in the CtrlUtils class.
        protected string GetStringToEncode()
        {
            StringBuilder strTest = new StringBuilder(1024);

            strTest.Append("This contains special characters and tags:   <   &   <b>");
            strTest.Append("\t");
            strTest.Append("Test</b>   >");
            strTest.Append("\r\n");
            strTest.Append("A URL with protocol: http://www.EWoodruff.us!");
            strTest.Append("\r\n");
            strTest.Append("A URL without protocol: www.microsoft.com.");
            strTest.Append("\r\n");
            strTest.Append("A URL followed by non-breaking spaces: http://www.microsoft.com  Test!");
            strTest.Append("\r\n");
            strTest.Append("A URL with trailing special chars: www.microsoft.com<>");
            strTest.Append("\r\n");
            strTest.Append("A URL with parameters: http://localhost/ServiceRequest/Request/AccountRequest.aspx?TicketID=254&mytest=5");
            strTest.Append("\r\n");
            strTest.Append(@"A UNC path: \\Server\Folder\SubFolder1\SubFolder2");
            strTest.Append("\r\n");
            strTest.Append(@"A UNC path with trailing special chars: \\Server\Folder\SubFolder1\SubFolder2<>");
            strTest.Append("\r\n");
            strTest.Append(@"A UNC path followed by spaces: \\Server\Folder\SubFolder1\SubFolder2.  Test!");
            strTest.Append("\r\n");
            strTest.Append(@"A UNC path with spaces: <\\Server\Folder\A Really Long Folder Name With Spaces>");
            strTest.Append("\r\n");
            strTest.Append(@"A test path <\\Test\My Test\File.txt> in a string.");
            strTest.Append("\r\n");
            strTest.Append(@"A test path <\\Test\My Test\File name with spaces.txt> in a string.");
            strTest.Append("\r\n");
            strTest.Append(@"A UNC path with dotted server name: \\Server.org\Folder\SubFolder1\SubFolder2");
            strTest.Append("\r\n");
            strTest.Append(@"A UNC path with a dotted server name and spaces: <\\Server.org\Folder\SubFolder1\SubFolder2\A Test File.txt>");
            strTest.Append("\r\n");
            strTest.Append("An e-mail address: Eric@EWoodruff.us");
            strTest.Append("\r\n");

            return strTest.ToString();
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if(!Page.IsPostBack)
            {
                this.PageTitle = "The PageUtils Class";

                // Demonstrate the HTMLEncode and EncodeLinks methods.  These
                // methods can be called in the ASPX page in the data binding
                // code blocks of a data grid too.  See the HTML for this page
                // for an example.
                lblWithHyperlinks.Text = PageUtils.HtmlEncode(
                    GetStringToEncode(), true);
                lblWithoutHyperlinks.Text = PageUtils.HtmlEncode(
                    GetStringToEncode(), false);

                Page.DataBind();
            }
		}
	}
}
