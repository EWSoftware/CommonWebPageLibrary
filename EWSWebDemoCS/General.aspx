<%@ Page MasterPageFile="~/MenuLayout.master" Language="c#" Inherits="EWSWebDemoCS.General" CodeFile="General.aspx.cs" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

<h1>General BasePage Features</h1>
This page demonstrates some of the general features of the
<b>BasePage</b> class.<br><br>

<asp:ValidationSummary id="vsSummary" DisplayMode="BulletList" ForeColor="" CssClass="ErrorMsg"
	HeaderText="Please correct the following problems:" runat="server" />
<table class="Shaded" cellSpacing="0" cellPadding="4" width="100%" border="0">
  <tr>
	<td class="FieldLabel" width="15%">Field 1</td>
	<td><asp:TextBox id="txtField1" runat="server" MaxLength="20" Columns="20" />
        <asp:RequiredFieldValidator id="rqfField1" runat="server" Display="None"
            ControlToValidate="txtField1" ErrorMessage="Field 1 is required" />
    </td>
  </tr>
  <tr>
	<td class="FieldLabel" width="15%">Field 2</td>
	<td><asp:TextBox id="txtField2" runat="server" MaxLength="20" Columns="20" />
        <asp:RequiredFieldValidator id="rqfField2" runat="server" Display="None"
            ControlToValidate="txtField2" ErrorMessage="Field 2 is required" />
    </td>
  </tr>
  <tr>
	<td class="FieldLabel" width="15%">Field 3</td>
	<td><asp:TextBox id="txtField3" runat="server" MaxLength="20" Columns="20" />
        <asp:RequiredFieldValidator id="rqfField3" runat="server" Display="None"
            ControlToValidate="txtField3" ErrorMessage="Field 3 is required" />
    </td>
  </tr>
</table>
<table class="Shaded" cellSpacing="0" cellPadding="4" width="100%" border="0">
  <tr>
    <td>
		<asp:button id="btnSave" runat="server" CssClass="FormBtn"
			ToolTip="Save info" Text="Save" onclick="btnSave_Click" />
		<asp:button id="btnEnDis1" runat="server" CssClass="FormBtn"
			ToolTip="Enable/Disable field 1" Text="Field 1"
            CausesValidation="False" onclick="btnEnDis1_Click" />
		<asp:button id="btnEnDisAll" runat="server" CssClass="FormBtn"
			ToolTip="Enable/Disable all" Text="All"
            CausesValidation="False" onclick="btnEnDisAll_Click" />
		<asp:button id="btnDefault" runat="server" CssClass="FormBtn"
			ToolTip="Set default values" Text="Defaults"
            CausesValidation="False" onclick="btnDefault_Click" />
    </td>
	<td align="right">
		<asp:Button id="btnExit" runat="server" CssClass="FormBtn"
			ToolTip="Exit without saving" Text="Exit" CausesValidation="false" onclick="btnExit_Click" />
	</td>
  </tr>
</table>

<h2>Set Focus</h2>
The <b>BasePage</b> class has the ability to set the focus to controls
on the page.  Although not demonstrated here, it will set focus to
controls nested within other controls such as the <b>DataGrid</b> and
<b>MultiPage</b> IE Web Control.

<h2>Enable/Disable One or More Controls</h2>
The <b>SetEnabled...</b> group of methods can be used to enable and disable
one or more controls or all controls on a page at the same time.  A CSS class
name can be applied to the disabled controls to change their visual style too.

<h2>Make Validation Summary Messages Into Links</h2>
The <b>MakeMsgLinks</b> method is used by the <b>BasePage</b> class to
turn validation error messages into clickable hyperlinks.  Clicking the
error message in the validation summary will set the focus to the
failing control.  If the control is a text box, it will also select the
text in the control.  To see this in action, enter invalid data in the
controls on this page.

<h2>Data Change Checking</h2>
In Internet Explorer, navigating away from the page after making changes
without saving them will cause a prompt to appear asking whether or not
you want to lose your changes.

</asp:Content>
