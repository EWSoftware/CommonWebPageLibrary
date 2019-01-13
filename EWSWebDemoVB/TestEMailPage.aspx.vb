'==============================================================================
' System  : ASP.NET Common Web Page Classes Demo
' File    : TestEMailPage.aspx.vb
' Author  : Eric Woodruff  (Eric@EWoodruff.us)
' Updated : Fri 11/26/2004
' Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
' Compiler: Microsoft VB.NET
'
' This demonstrates the e-mail page content features of the Base class
'
' Version     Date     Who  Comments
' =============================================================================
' 1.0.0    11/28/2003  EFW  Created the code
'==============================================================================

Option Strict On

Imports System.Net.Mail

Imports EWSoftware.Web

Namespace EWSWebDemoVB

Partial Class TestEMailPage
    Inherits EWSoftware.Web.BasePage

    Private Sub Page_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack
            Me.PageTitle = "E-Mail Rendered Content Test"
            txtFrom.Focus()
        End If
    End Sub

    Private Sub btnEMail_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs) Handles btnEMail.Click
        If Page.IsValid = True Then
            ' Set this to true to have the page render itself and send
            ' a copy via e-mail.
            Me.EMailRenderedPage = True
        End If
    End Sub

    ' This handles the EMailThisPage event to set the e-mail info and
    ' make some modifications to the e-mail and the page rendered to the
    ' browser.
    Private Sub Page_EMailThisPage(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailPageEventArgs) Handles MyBase.EMailThisPage

        ' Please don't send it to me!
        If txtTo.Text.ToLower() = "eric@ewoodruff.us" Then
            args.Cancel = True

            args.RenderedContent = args.RenderedContent.Replace( _
                "<!-- SENTNOTES -->", "<b>Eric doesn't want to get " & _
                "spammed with test messages.  Please send it to " & _
                "somebody else, yourself for example.  The event " & _
                "was cancelled.</b><br><hr>")

            Exit Sub
        End If

        If txtSMTPServer.Text.Length > 0 Then
            args.SMTPServer = txtSMTPServer.Text
        End If

        args.EMail.From = New MailAddress(txtFrom.Text)
        args.EMail.To.Add(txtTo.Text)
        args.EMail.Subject = txtSubject.Text

        ' Insert user comments into the e-mail
        args.EMail.Body = args.EMail.Body.Replace("<!-- EMAILCOMMENTS -->", _
            "User Comments:<br>" & txtComments.Text & "<br><hr>")

        args.RenderedContent = args.RenderedContent.Replace("<!-- SENTNOTES -->", _
            "The following was sent via e-mail to " & _
            args.EMail.To(0).Address & ":<br><br>")
    End Sub

    ' This handles the EMailError event to add some text to the page to tell
    ' the user that something went wrong.
    Private Sub Page_EMailError(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailErrorEventArgs) Handles MyBase.EMailError
        Dim strMsg As String, strAltServer As String = Nothing

        ' Here's an example of retrying the failed send with an
        ' alternate SMTP server.  In a real app, you probably want to
        ' check the exception to see what caused it before retrying.
        If args.EMailEventArguments.RetryCount = 0 Then
            strAltServer = _
                ConfigurationManager.AppSettings("AlternateSMTPServer")
        End If

        If Not (strAltServer Is Nothing) Then
            ' Set the alternate server
            args.EMailEventArguments.SMTPServer = strAltServer

            ' Increment the counter and tell the page to retry
            args.EMailEventArguments.RetryCount += 1
            args.EMailEventArguments.RetryOnFailure = True
        Else
            strMsg = String.Format("<hr><b>Ok, I lied, there was a problem and " & _
                "the e-mail couldn't be sent</b><br>Details:<br>{0}<br><hr>", _
                PageUtils.HTMLEncode(args.EMailException.ToString, False))

            args.EMailEventArguments.RenderedContent = _
                args.EMailEventArguments.RenderedContent.Replace( _
                    "<!-- EMAILERROR -->", strMsg)
        End If
    End Sub
End Class

End Namespace
