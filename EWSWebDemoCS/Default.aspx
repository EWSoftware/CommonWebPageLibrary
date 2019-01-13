<%@ Page MasterPageFile="~/MenuLayout.master" Language="c#" Inherits="EWSWebDemoCS.DefaultPage" CodeFile="Default.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

<table width="100%">
  <tr>
	<td class="AboutHeader">
		<h1><asp:Label id="lblAppName" runat="server" /></h1>
	</td>
  </tr>
  <tr>
	<td>
	<div align="center">
	<b>Version <asp:label id="lblVersion" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
Release Date <asp:label id="lblReleaseDate" runat="server" /></b>
	</div>
	</td>
  </tr>
</table>
<br>
This application is used to demonstrate the various classes in the
<b>EWSoftware.Web</b> namespace:<br><br>

<div align="center">
<table cellspacing="0" cellpadding="5" border="1" width="60%">
  <tr>
    <td width="30%" style="background: #BFBFBF;"><b>Class</b></td>
    <td style="background: #BFBFBF;"><b>Description</b></td>
  </tr>
  <tr>
    <td valign="top">BasePage</td>
    <td>A base class for ASP.NET pages that has various features such as
data change checking, the ability to e-mail its rendered content, etc.</td>
  </tr>
  <tr>
    <td valign="top">PageUtils</td>
    <td>A class containing a few useful utility methods.</td>
  </tr>
</table>
</div>

<p>For assistance, e-mail
<a href="mailto:Eric@EWoodruff.us?Subject=EWSoftware.Web Classes">Eric@EWoodruff.us</a>.
</asp:Content>

