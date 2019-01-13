<%@ Page MasterPageFile="~/EMailLayout.master" Language="c#" Inherits="EWSWebDemoCS.TestEMailPage2" CodeFile="TestEMailPage2.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

<h1>E-Mail Page Content and Error Information Demo</h1>
For an example of using the e-mailing of page content for a custom error page
handler and the <b>BasePage</b> class's extended error information, click this
button:<br>
<asp:Button id="btnError" runat="Server" class="FormBtn" Text="Throw"
    Tooltip="Throw an exception" CausesValidation="False" onclick="btnError_Click" />
<br><br>
Note that the <b>customErrors</b> tag in <b>Web.config</b> should be set
to one of the following:<br><br>

<b>&lt;customErrors mode="On" defaultRedirect="ErrorPageInternal.aspx" /&gt;</b><br>
A page that shows full details.  Useful for intranet or internal
applications where you don't mind if the user sees the error details.<br><br>

<b>&lt;customErrors mode="On" defaultRedirect="ErrorPagePublic.aspx" /&gt;</b><br>
A page that shows no details.  Useful for public internet applications where
you want to send the details but not show them to the user.
<br><br>
There are also some <b>appSettings</b> that should be modified if you copy
the page to your own applications (i.e. <b>AppName</b> and <b>ErrorRptEMail</b>).
<hr>
<br><br>
Send a report page via e-mail.  This calls a demo report page that
sends its content via e-mail if the e-mail address is filled in:<br><br>
<table cellspacing="0" cellpadding="2" border="0" width="100%">
  <tr>
    <td>To:</td>
    <td><asp:TextBox id="txtTo" Columns="50" runat="Server" /></td>
  </tr>
  <tr>
    <td valign="top">Comments:</td>
    <td><asp:TextBox id="txtComments" runat="server" TextMode="MultiLine"
	    Columns="75" Rows="5" />
    </td>
  </tr>
</table>
<br>
<asp:Button id="btnReport" runat="Server" class="FormBtn" Text="Report"
    Tooltip="Demo e-mail report" onclick="btnReport_Click" />
<input type="button" class="FormBtn" value="Exit" Title="Return to About page"
    onclick="javascript: window.location='Default.aspx'; return false;" />

</asp:Content>
