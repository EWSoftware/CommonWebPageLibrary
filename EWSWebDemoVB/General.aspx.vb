'==============================================================================
' System  : ASP.NET Common Web Page Classes Demo
' File    : General.aspx.vb
' Author  : Eric Woodruff  (Eric@EWoodruff.us)
' Updated : Fri 11/26/2004
' Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
' Compiler: Microsoft VB.NET
'
' This demonstrates the features of the BasePage class
'
' Version     Date     Who  Comments
' =============================================================================
' 1.0.0    11/28/2003  EFW  Created the code
'==============================================================================

Option Strict On

Imports EWSoftware.Web


Namespace EWSWebDemoVB


Partial Class General
    Inherits EWSoftware.Web.BasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            ' Set up form for data change checking.  Don't prompt if
            ' the Save button or Enable/Disable buttons are clicked.
            ' Prompt to save on all others.
            Me.CheckForDataChanges = True
            Me.BypassPromptIDs = New String() { "btnSave", "btnEnDis1", _
                "btnEnDisAll" }

            ' Only set default focus on initial load
            Me.SetFocusExtended(txtField1)

            Me.PageTitle = "General BasePage Features"
        End If
    End Sub

    Private Sub btnEnDis1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEnDis1.Click
        txtField1.Enabled = Not txtField1.Enabled
        rqfField1.Enabled = txtField1.Enabled

        If txtField1.Enabled = True Then
            Me.SetFocusExtended(txtField1)
        Else
            Me.SetFocusExtended(txtField2)
        End If
    End Sub

    Private Sub btnEnDisAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEnDisAll.Click
        ' Re-enable field one to make it look consistent with the others
        If txtField1.Enabled = False
            txtField1.Enabled = True
        End If

        ' Disable or enable all controls
        Me.SetEnabledAll(Not Me.txtField3.Enabled, Me.PageForm)
        rqfField1.Enabled = txtField1.Enabled
        rqfField2.Enabled = txtField2.Enabled
        rqfField3.Enabled = txtField3.Enabled

        If txtField1.Enabled = True Then
            Me.SetFocusExtended(txtField1)
        Else
            Me.SetFocusExtended(btnEnDisAll)
        End If
    End Sub

    Private Sub btnDefault_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDefault.Click
        ' Set default values
        txtField1.Text = "Field 1"
        txtField2.Text = "Field 2"
        txtField3.Text = "Field 3"

        ' Set the dirty flag to indicate that data has changed
        Me.Dirty = True
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        ' Turn off the dirty flag to indicate that the data was saved
        If Page.IsValid Then
            Me.Dirty = False
        End If
    End Sub

    Private Sub btnExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Response.Redirect("Default.aspx")
    End Sub

End Class

End Namespace
