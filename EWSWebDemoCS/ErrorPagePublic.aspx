<%@ Page MasterPageFile="~/EMailLayout.master" Language="c#" Inherits="EWSWebDemoCS.ErrorPagePublic" CodeFile="ErrorPagePublic.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

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
