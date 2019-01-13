<%@ Page MasterPageFile="~/EMailLayout.master" Language="vb" AutoEventWireup="false" Inherits="EWSWebDemoVB.ErrorPageInternal" CodeFile="ErrorPageInternal.aspx.vb" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
<!-- NOEMAIL -->
<!-- EMAILERROR --><asp:Label id="lblMsg" runat="server" class="Attn"/><!-- EMAILERROR -->
Please enter a reply e-mail address and any additional comments and click the
<b>E-Mail</b> button to report this error.  If there is a problem sending
the e-mail, you may send the information manually to
<asp:HyperLink id="hlHelpLink" runat="Server" />.
<br><br>
<table cellspacing="4" cellpadding="0" border="0" width="100%">
  <tr>
    <td align="right"><b>Reply E-Mail Address:</b></td>
    <td><asp:TextBox id="txtReplyEMail" runat="server" Columns="50"/></td>
  </tr>
  <tr>
	<td align="right" valign="top"><b>Comments:</b></td>
	<td><asp:TextBox id="txtComments" runat="server" TextMode="MultiLine" Columns="75" Rows="5" /></td>
  </tr>
</table>
<br>
<asp:Button id="btnEMail" runat="Server" class="FormBtn" Text="E-Mail" Tooltip="E-Mail the info" />
<hr>
<!-- NOEMAIL -->
<!-- EMAILCOMMENTS -->
<h1>Unexpected Application Error</h1>
An unexpected error has occurred in the application
<b><asp:Label id="lblAppName" runat="server" /></b>.<br><br>

<b>Page on which the error occurred:&nbsp;&nbsp;</b><asp:Label id="lblPageName" runat="server" /><br><br>
<b>Error Message</b><br><asp:Label id="lblLastError" runat="server" /><br><br>
<asp:Repeater id="rptServerVars" runat="server">
	<HeaderTemplate>
		<table cellSpacing="0" cellPadding="2" width="100%" border="0">
			<tr>
				<td width="20%" class="ColHeader">Server Variable</td>
                <td class="ColHeader">Value</td>
			</tr>
	</HeaderTemplate>
	<ItemTemplate>
			<tr>
				<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Key") %> </td>
				<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Value") %> </td>
			</tr>
	</ItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>
<br>
<asp:Repeater id="rptQueryString" runat="server">
	<HeaderTemplate>
		<table cellSpacing="0" cellPadding="2" width="100%" border="0">
			<tr>
				<td width="20%" class="ColHeader">QueryString Variable</td>
                <td class="ColHeader">Value</td>
			</tr>
	</HeaderTemplate>
	<ItemTemplate>
			<tr>
				<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Key") %> </td>
				<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Value") %> </td>
			</tr>
	</ItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>
<br>
<asp:Repeater id="rptForm" runat="server">
	<HeaderTemplate>
		<table cellSpacing="0" cellPadding="2" width="100%" border="0">
			<tr>
				<td width="20%" class="ColHeader">Form Variable</td>
                <td class="ColHeader">Value</td>
			</tr>
	</HeaderTemplate>
	<ItemTemplate>
			<tr>
				<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Key") %> </td>
				<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Value") %> </td>
			</tr>
	</ItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>

</asp:Content>
