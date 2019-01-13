<%@ Page MasterPageFile="~/EMailLayout.master" Language="c#" Inherits="EWSWebDemoCS.TestEMailPage" CodeFile="TestEMailPage.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

<!-- NOEMAIL -->
<!-- This section will not appear in the e-mail because it appears
     between NOEMAIL comment tags. -->

<h1>E-Mailing Rendered Content Demo</h1>
This page demonstrates the e-mailing of rendered content.  It also demonstrates
the clickable links in the validation summary control if you do not enter a
From and/or To e-mail address.<br><br>

<asp:ValidationSummary id="vsSummary" DisplayMode="BulletList" ForeColor="" CssClass="ErrorMsg"
	HeaderText="Please correct the following problems:" runat="server" />
Send this page via e-mail:
<table cellspacing="0" cellpadding="2" border="0" width="100%">
  <tr>
    <td width="12%">SMTP Server:</td>
    <td><asp:TextBox id="txtSMTPServer" runat="Server" Columns="20" />
    Leave blank to use IIS server or the EMailPage_SmtpServer setting from
    Web.config if defined.  May fail if SMTP is not installed or not
    configured for relaying.
    </td>
  </tr>
  <tr>
    <td>From:</td>
    <td><asp:TextBox id="txtFrom" Columns="50" runat="Server" />
    <asp:RequiredFieldValidator id="rqfFrom" runat="server" ControlToValidate="txtFrom"
    Display="None" ErrorMessage="A From e-mail address is required" />
    </td>
  </tr>
  <tr>
    <td>To:</td>
    <td><asp:TextBox id="txtTo" Columns="50" runat="Server" />
    <asp:RequiredFieldValidator id="rqfTo" runat="server" ControlToValidate="txtTo"
        Display="None" ErrorMessage="A To e-mail address is required" />
    </td>
  </tr>
  <tr>
    <td>Subject:</td>
    <td><asp:TextBox id="txtSubject" runat="Server" Columns="50" /></td>
  </tr>
  <tr>
    <td valign="top">Comments:</td>
    <td><asp:TextBox id="txtComments" runat="server" TextMode="MultiLine"
	    Columns="75" Rows="5" />
    </td>
  </tr>
</table>
<br>
<asp:Button id="btnEMail" runat="Server" class="FormBtn" Text="E-Mail"
    Tooltip="E-Mail the info" onclick="btnEMail_Click" />
<input type="button" class="FormBtn" value="Exit" Title="Return to About page"
    onclick="javascript: window.location='Default.aspx'; return false;" />
<hr>
<br>
<!-- NOEMAIL -->

<!-- Comments entered above will be displayed here. -->
<!-- EMAILCOMMENTS -->

<!-- Notes about a successful send will appear here in the rendered page
     but not in the e-mail. -->
<!-- SENTNOTES -->

<!-- Notes about an error sending the e-mail will appear here in the
     rendered page. -->
<!-- EMAILERROR -->

This is just a test of the <b>EWSoftware.Web.BasePage</b> class and it's
ability to e-mail the rendered content of the page.  All of this text below
the horizontal rule will be sent in the body of the e-mail message prefixed
with any comments entered in the field above.  The form controls above the
rule will not appear in the e-mail.

</asp:Content>
