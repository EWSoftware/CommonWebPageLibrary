'==============================================================================
' File    : ErrorPageInternal.aspx.vb
' Author  : Eric Woodruff  (Eric@EWoodruff.us)
' Updated : Fri 11/26/2004
' Compiler: Microsoft VB.NET
'
' This implements the error page for displaying unexpected application errors.
' To use it, change the class name in the Inherits option on the @Page tag in
' the ErrorPageInternal.aspx file and modify your Web.Config file to include a
' customErrors entry like the following:
'
'   <customErrors mode="RemoteOnly" defaultRedirect="ErrorPageInternal.aspx" />
'
'    Date     Who  Comments
'==============================================================================
' 10/15/2002  EFW  Created the code
'==============================================================================

Option Strict

Imports System.Collections.Specialized
Imports System.Net.Mail
Imports System.Text
Imports System.Text.RegularExpressions

Imports EWSoftware.Web


Namespace EWSWebDemoVB


Partial Class ErrorPageInternal
    Inherits EWSoftware.Web.BasePage

    ' Convert the name/value collections to standard sorted lists for use
    ' with the repeaters.
    Private Function ConvertNVCollection(nvcColl As NameValueCollection) As SortedList
        Dim strArray1(), strArray2(), strEntry1 As String
        Dim slColl As New SortedList

        ' Get the names of all keys into a string array
        strArray1 = nvcColl.AllKeys
        For Each strEntry1 In strArray1
            ' Get all the values under this key.  Empty collections and
            ' the view state variable are not added.
            strArray2 = nvcColl.GetValues(strEntry1)
            If Not (strArray2 Is Nothing) And strEntry1 <> "__VIEWSTATE" Then
                slColl.Add(strEntry1, String.Join("<br>", strArray2))
            End If
        Next

        Return slColl
    End Function

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim htErrorContext As Hashtable, slTemp As SortedList
        Dim strLastError As StringBuilder, strRemoteAddr As String

        If Page.IsPostBack Then
            Exit Sub
        End If

        Me.PageTitle = "Unexpected Application Error"
        txtReplyEMail.Focus()

        ' Set the application name
        lblAppName.Text = ConfigurationManager.AppSettings("AppName")

        ' Format the help hyperlink with the necessary information
        hlHelpLink.Text = ConfigurationManager.AppSettings("ErrorRptEMail")
        hlHelpLink.NavigateUrl = "mailto:" & hlHelpLink.Text & _
            "?Subject=Error in " & lblAppName.Text & _
            "&Body=Copy and paste error information here along with " & _
            "any other relevant information."

        ' Retrieve the context information.  It should be there.
        strRemoteAddr = Request.ServerVariables("REMOTE_ADDR")
        htErrorContext = DirectCast(Cache(strRemoteAddr), Hashtable)

        If Not (htErrorContext Is Nothing) Then
            ' Do a little formatting on the error
            strLastError = New StringBuilder(htErrorContext("LastError").ToString)
            strLastError.Replace("  ", "&nbsp;&nbsp;")
            strLastError.Replace(vbTab, "&nbsp;&nbsp;&nbsp;&nbsp;")
            strLastError.Replace(vbCr, "")
            strLastError.Replace(vbLf, "<br>")

            lblLastError.Text = strLastError.ToString
            lblPageName.Text = htErrorContext("Page").ToString

            rptServerVars.DataSource = CType(htErrorContext("ServerVars"), SortedList)

            slTemp = ConvertNVCollection( _
                CType(htErrorContext("QueryString"), NameValueCollection))

            ' Don't show query string or form repeater if they are empty
            If slTemp.Count > 0 Then
                rptQueryString.DataSource = slTemp
            Else
                rptQueryString.Visible = False
            End If

            slTemp = ConvertNVCollection( _
                CType(htErrorContext("Form"), NameValueCollection))

            If slTemp.Count > 0 Then
                rptForm.DataSource = slTemp
            Else
                rptForm.Visible = False
            End If

            Page.DataBind()

            ' Clear the error information from the cache
            Cache.Remove(strRemoteAddr)
        Else
            rptServerVars.Visible = False
            rptQueryString.Visible = False
            rptForm.Visible = False
            lblPageName.Text = Request.QueryString.ToString()
            lblLastError.Text = "No context information available"
        End If
    End Sub

    Private Sub btnEMail_Click(ByVal sender As Object, _
      ByVal e As System.EventArgs) Handles btnEMail.Click
        Me.EMailRenderedPage = True
        lblMsg.Text = "The e-mail has been sent.<br><br>"
    End Sub

    ' This event fires if there was a problem e-mailing the page.
    Private Sub Page_EMailError(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailErrorEventArgs) Handles MyBase.EMailError
        ' Replace the "sent" message with an error message.  It's already been
        ' rendered, so replace the text between the comment tags.
        args.EMailEventArguments.RenderedContent = Regex.Replace( _
            args.EMailEventArguments.RenderedContent, _
            "\<!-- EMAILERROR --\>.*?\<!-- EMAILERROR --\>", _
            "<span class='Attn'>The was a problem sending the e-mail. " & _
            "Please submit the information manually.</span><br><br>", _
            RegexOptions.IgnoreCase Or RegexOptions.Singleline)
    End Sub

    ' This event fires when the page is ready to be e-mailed.
    Private Sub Page_EMailThisPage(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailPageEventArgs) Handles MyBase.EMailThisPage
        Dim dtErrorDate As Date, nErrorCount, nMaxErrRpts As Integer

        ' See if we have exceeded the maximum number of error reports today.
        ' We don't want to overload the recipient of the reports.
        nMaxErrRpts = Convert.ToInt32(ConfigurationManager.AppSettings("MaxErrorReports"))
        If nMaxErrRpts > 0 Then
            If Not Application("ErrorReportDate") Is Nothing Then
                dtErrorDate = DirectCast(Application("ErrorReportDate"), Date)
                nErrorCount = DirectCast(Application("ErrorReportCount"), Integer)

                If dtErrorDate = DateTime.Today And nErrorCount >= nMaxErrRpts Then
                    args.Cancel = True
                    Exit Sub
                End If

                If dtErrorDate <> DateTime.Today Then
                    dtErrorDate = DateTime.Today    ' Date rolled over
                    nErrorCount = 1
                Else
                    nErrorCount += 1        ' Another on the same day
                End If
            Else
                dtErrorDate = DateTime.Today    ' First one
                nErrorCount = 1
            End If

            ' Store the error report date and count to the application state
            Application.Lock()
            Application("ErrorReportDate") = dtErrorDate
            Application("ErrorReportCount") = nErrorCount
            Application.UnLock()
        End If

        ' If no e-mail address was specified, make one up.  If not, it won't
        ' send the message.
        If txtReplyEMail.Text.Length = 0 Then
            args.EMail.From = New MailAddress("Unknown@Unknown.com")
        Else
            args.EMail.From = New MailAddress(txtReplyEMail.Text)
        End If

        ' Set recipient and subject
        args.EMail.To.Add(hlHelpLink.Text)
        args.EMail.Subject = "Error in " & lblAppName.Text

        ' Insert user comments into the e-mail if specified
        If txtComments.Text.Length > 0 Then
            args.EMail.Body = args.EMail.Body.Replace("<!-- EMAILCOMMENTS -->", _
                "<b>User Comments:</b><br>" & txtComments.Text & "<br><br><hr>")
        End If

    End Sub
End Class

End Namespace
