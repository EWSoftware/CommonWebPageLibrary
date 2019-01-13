'==============================================================================
' System  : ASP.NET Common Web Page Classes Demo
' File    : TestEMailPage2.aspx.vb
' Author  : Eric Woodruff  (Eric@EWoodruff.us)
' Updated : Fri 11/26/2004
' Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
' Compiler: Microsoft VB.NET
'
' This demonstrates the e-mail page content features of the BasePage class
' and the error information in the BasePage class.
'
' Version     Date     Who  Comments
' =============================================================================
' 1.0.0    11/29/2003  EFW  Created the code
'==============================================================================

Option Strict On

Imports EWSoftware.Web


Namespace EWSWebDemoVB


Partial Class TestEMailPage2
    Inherits EWSoftware.Web.BasePage

    ' This property returns the e-mail address for the report page
    Public ReadOnly Property EMailAddress As String
        Get
            Return txtTo.Text
        End Get
    End Property

    ' This property returns the comments for the report page
    Public ReadOnly Property Comments As String
        Get
            Return txtComments.Text
        End Get
    End Property

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack
            Me.PageTitle = "E-Mail Page Content/BasePage Error Info"
            txtTo.Focus()
        End If
    End Sub

    Private Sub btnError_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnError.Click
        ' Throw an exception to see the e-mail page content option in
        ' use on a custom error page.  Also shows the BasePage class's
        ' extended error stuff.
        Throw New Exception("Test Exception")
    End Sub

    Private Sub btnReport_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnReport.Click
        ' Demonstrate a page that e-mails itself if the calling
        ' page (this one) passes an e-mail address to it.
        Server.Transfer("DemoReport.aspx")
    End Sub

End Class

End Namespace
