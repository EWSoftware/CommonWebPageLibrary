'==============================================================================
' File    : ErrorPagePublic.aspx.vb
' Author  : Eric Woodruff
' Updated : Fri 11/26/2004
' Compiler: Microsoft VB.NET
'
' This implements the error page for displaying unexpected application errors.
' To use it, change the class name in the Inherits option on the @Page tag in
' the ErrorPagePublic.aspx file and modify your Web.Config file to include a
' customErrors entry like the following:
'
'   <customErrors mode="RemoteOnly" defaultRedirect="ErrorPagePublic.aspx" />
'
' This version automatically e-mails the details to the support e-mail address
' and displays a generic message to the user as defined in the file
' ErrorPagePublic.htm.  This is useful for public internet applications in
' which you should not show full error details.
'
'    Date     Who  Comments
'==============================================================================
' 10/15/2002  EFW  Created the code
'==============================================================================

Option Strict

Imports System.Collections.Specialized
Imports System.IO
Imports System.Net.Mail
Imports System.Text
Imports System.Text.RegularExpressions

Imports EWSoftware.Web


Namespace EWSWebDemoVB


Partial Class ErrorPagePublic
    Inherits EWSoftware.Web.BasePage

    Dim strErrorRptEMail As String

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

        ' Because this is a public application, we don't want to give
        ' the user the error details.  We'll automatically e-mail the
        ' error report and give the user a short note telling them that
        ' there was a problem.
        Me.EMailRenderedPage = True

        ' Set the application name
        lblAppName.Text = ConfigurationManager.AppSettings("AppName")

        ' Get the help e-mail address
        strErrorRptEMail = ConfigurationManager.AppSettings("ErrorRptEMail")

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

    ' This event fires if there was a problem e-mailing the page.
    Private Sub Page_EMailError(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailErrorEventArgs) Handles MyBase.EMailError
        ' Replace the "sent" message with an error message.  It's already been
        ' rendered, so replace the text between the comment tags.
        args.EMailEventArguments.RenderedContent = Regex.Replace( _
            args.EMailEventArguments.RenderedContent, _
            "\<!-- EMAILERROR --\>.*?\<!-- EMAILERROR --\>", _
            "<span class='Attn'><br><br>There was a problem sending the " & _
            "error report e-mail.  If the problem persists, you can report " & _
            "the problem yourself by sending e-mail to " & _
            strErrorRptEMail & ".</span><br><br>", _
            RegexOptions.IgnoreCase Or RegexOptions.Singleline)
    End Sub

    ' This event fires when the page is ready to be e-mailed.
    Private Sub Page_EMailThisPage(ByVal sender As Object, _
      ByVal args As EWSoftware.Web.EMailPageEventArgs) Handles MyBase.EMailThisPage
        Dim dtErrorDate As Date, nErrorCount, nMaxErrRpts As Integer
        Dim sr As StreamReader = Nothing

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

        ' Set the from address
        args.EMail.From = New MailAddress(ConfigurationManager.AppSettings("ErrorRptFrom"))

        ' Set recipient and subject
        args.EMail.To.Add(strErrorRptEMail)
        args.EMail.Subject = "Error in " & lblAppName.Text

        ' Give the user a less detail report of the error
        Try
            sr = New StreamReader(Server.MapPath("ErrorPagePublic.htm"))
            args.RenderedContent = sr.ReadToEnd()
        Catch
            args.RenderedContent = "Unexpected application error"
        Finally
            If Not sr Is Nothing
                sr.Close()
            End If
        End Try
    End Sub
End Class

End Namespace
