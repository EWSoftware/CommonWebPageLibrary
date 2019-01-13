<%@ Page MasterPageFile="~/MenuLayout.master" Language="c#" Inherits="EWSWebDemoCS.Utilities" CodeFile="Utilities.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

<h1>The PageUtils Class</h1>
The <b>PageUtils</b> class contains some static utility methods that are
used by the page classes and can be used by you as well.  The following
sections describe the available methods.

<h2>HTML Encode an Object For Output to the Page</h2>
The <b>HtmlEncode</b> method can be called to encode an object for output to
an HTML page so that it renders any HTML special characters as literals instead
of letting the browser interpret them.  It encodes the text and replaces
multiple spaces, tabs, and line breaks with their HTML equivalents.  If the
object is null/Nothing, a non-breaking space is returned.  This is useful for
displaying database fields that contain HTML special characters or nulls.  If
the <b>encodeLinks</b> parameters is true, URLs, UNCs, and e-mail addresses
are converted to hyperlinks whenever possible. If false, they are not
converted.

<p>Example with hyperlinks converted:
<blockquote><asp:Label id="lblWithHyperlinks" runat="server" /></blockquote>

<p>Example without hyperlinks converted:
<blockquote><asp:Label id="lblWithoutHyperlinks" runat="server" /></blockquote>

<h2>Convert URLs, UNCs, and E-mail Addresses to HyperLinks</h2>
The <b>EncodeLinks</b> method is used to convert any URLs, UNCs, and e-mail
addresses found in the specified text to hyperlinks.  This is called by the
<b>HtmlEncode</b> method to handle the conversion during its encoding operation
if needed.  For UNC paths, it will include any text up to the first whitespace
character.  If the path contains spaces, you can enclose the entire path in
angle brackets (&lt;\\Server\Folder\Name With Spaces&gt;) and the encoder will
include all text between the brackets in the hyperlink.  The angle brackets
will not appear in the encoded hyperlink.  See the examples above.

<p>The methods can also be used within the HTML using the data-binding syntax.
For example:<br><br>

&lt;asp:Label id="lblTestHTMLEncode" runat="server"&gt;<br>
<b>&lt;%# EWSoftware.Web.PageUtils.HtmlEncode(GetStringToEncode(), true) %&gt;</b><br>
&lt;/asp:Label&gt;

<br><br>Results in:<br><br>

<asp:Label id="lblTestHTMLEncode" runat="server">
<%# EWSoftware.Web.PageUtils.HtmlEncode(GetStringToEncode(), true) %>
</asp:Label>

<br><br>

&lt;asp:Label id="lblTestEncodeLinks" runat="server"&gt;<br>
<b>&lt;%# EWSoftware.Web.PageUtils.EncodeLinks("E-Mail: Eric@EWoodruff.us") %&gt;</b><br>
&lt;/asp:Label&gt;

<br><br>Results in:<br><br>

<asp:Label id="lblTestEncodeLinks" runat="server">
<%# EWSoftware.Web.PageUtils.EncodeLinks("E-Mail: Eric@EWoodruff.us") %>
</asp:Label>
</asp:Content>
