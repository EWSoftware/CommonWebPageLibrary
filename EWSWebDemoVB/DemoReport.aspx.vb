'==============================================================================
' System  : ASP.NET Common Web Page Classes Demo
' File    : DemoReport.aspx.vb
' Author  : Eric Woodruff
' Updated : Fri 11/26/2004
' Compiler: Microsoft VB.NET
'
' A simple demo to show how a page might e-mail itself if passed an e-mail
' address.
'
'    Date     Who  Comments
' =============================================================================
' 11/29/2003  EFW  Created the code
'==============================================================================

Imports System.Net.Mail
Imports System.Text.RegularExpressions

Imports EWSoftware.Web

Namespace EWSWebDemoVB


Partial Class DemoReport
    Inherits EWSoftware.Web.BasePage

    Dim strEMailAddress, strComments As String

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim caller As TestEMailPage2

        Me.PageTitle = "Demo Report"

        ' See if an e-mail address was entered as criteria
        caller = TryCast(Context.Handler, TestEMailPage2)

        If caller IsNot Nothing Then
            strEMailAddress = caller.EMailAddress
            strComments = caller.Comments

            If Not (strEMailAddress Is Nothing) AndAlso strEMailAddress.Length > 0 Then
                Me.EMailRenderedPage = True
            End If
        End If

    End Sub

    ' This event fires if there was a problem e-mailing the page
    Private Sub Page_EMailError(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailErrorEventArgs) Handles MyBase.EMailError
        ' Replace the "sent" message with an error message
        args.EMailEventArguments.RenderedContent = Regex.Replace( _
            args.EMailEventArguments.RenderedContent, _
            "\<!-- EMAILERROR --\>.*?\<!-- EMAILERROR --\>", _
            "<span class='Attn'>The was a problem sending the e-mail. " & _
            "Please e-mail the report manually.</span><br><br>", _
            RegexOptions.IgnoreCase Or RegexOptions.Singleline)
    End Sub

    ' This event fires when the page is ready to be e-mailed
    Private Sub Page_EMailThisPage(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailPageEventArgs) Handles MyBase.EMailThisPage
        ' I don't want it!
        If strEMailAddress.ToLower() = "eric@ewoodruff.us"
            args.Cancel = True
            args.RenderedContent = args.RenderedContent.Replace( _
                "<!-- EMAILCOMMENTS -->", "<b>Eric doesn't want to get " & _
                "spammed with test messages.  Please send it to " & _
                "somebody else, yourself for example.  The event " & _
                "was cancelled.</b><br><hr>")

            Exit Sub
        End If

        ' Set sender, recipient, and subject
        args.EMail.From = New MailAddress(strEMailAddress)
        args.EMail.To.Add(strEMailAddress)
        args.EMail.Subject = Me.PageTitle

        ' Insert e-mail comments if necessary
        If strComments.Length > 0 Then
            args.EMail.Body = args.EMail.Body.Replace("<!-- EMAILCOMMENTS -->", _
                "<b>Sender Comments:</b><br>" & strComments & _
                "<br><br><hr>")
        End If

        ' Insert sent notification to the rendered page
        args.RenderedContent = args.RenderedContent.Replace("<!-- EMAILSENT -->", _
            "<b>This report was sent via e-mail to " & _
            args.EMail.To(0).Address & "</b><br><br><hr>")
    End Sub

End Class

End Namespace
