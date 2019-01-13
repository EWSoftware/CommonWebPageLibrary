'==============================================================================
' System  : ASP.NET Common Web Page Classes Demo
' File    : Utilities.aspx.vb
' Author  : Eric Woodruff  (Eric@EWoodruff.us)
' Updated : 09/13/2013
' Note    : Copyright 2002-2013, Eric Woodruff, All rights reserved
' Compiler: Microsoft VB.NET
'
' This demonstrates the features of the PageUtils utility class
'
' Version     Date     Who  Comments
' =============================================================================
' 1.0.0    11/28/2003  EFW  Created the code
'==============================================================================

Option Strict On

Imports System.Text

Imports EWSoftware.Web


Namespace EWSWebDemoVB


Partial Class Utilities
    Inherits EWSoftware.Web.BasePage

    ' Return a test string to demonstrate the HTMLEncode() and MakeLinks()
    ' methods in the CtrlUtils class.
    Protected Function GetStringToEncode() As String
        Dim strTest As New StringBuilder(1024)

        strTest.Append("This contains special characters and tags:   <   &   <b>")
        strTest.Append(vbTab)
        strTest.Append("Test</b>   >")
        strTest.Append(vbCrLf)
        strTest.Append("A URL with protocol: http://www.EWoodruff.us!")
        strTest.Append(vbCrLf)
        strTest.Append("A URL without protocol: www.microsoft.com.")
        strTest.Append(vbCrLf)
        strTest.Append("A URL followed by non-breaking spaces: http://www.microsoft.com  Test!")
        strTest.Append(vbCrLf)
        strTest.Append("A URL with trailing special chars: www.microsoft.com<>")
        strTest.Append(vbCrLf)
        strTest.Append("A URL with parameters: http://localhost/ServiceRequest/Request/AccountRequest.aspx?TicketID=254&mytest=5")
        strTest.Append(vbCrLf)
        strTest.Append("A UNC path: \\Server\Folder\SubFolder1\SubFolder2")
        strTest.Append(vbCrLf)
        strTest.Append("A UNC path with trailing special chars: \\Server\Folder\SubFolder1\SubFolder2<>")
        strTest.Append(vbCrLf)
        strTest.Append("A UNC path followed by spaces: \\Server\Folder\SubFolder1\SubFolder2.  Test!")
        strTest.Append(vbCrLf)
        strTest.Append("A UNC path with spaces: <\\Server\Folder\A Really Long Folder Name With Spaces>")
        strTest.Append(vbCrLf)
        strTest.Append("A UNC path with dotted server name: \\Server.org\Folder\SubFolder1\SubFolder2")
        strTest.Append(vbCrLf)
        strTest.Append("A UNC path with a dotted server name and spaces: <\\Server.org\Folder\SubFolder1\SubFolder2\A Test File.txt>")
        strTest.Append(vbCrLf)
        strTest.Append("An e-mail address: Eric@EWoodruff.us")
        strTest.Append(vbCrLf)

        Return strTest.ToString
    End Function

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            Me.PageTitle = "The PageUtils Class"

            ' Demonstrate the HTMLEncode and EncodeLinks methods.  These
            ' methods can be called in the ASPX page in the data binding
            ' code blocks of a data grid too.  See the HTML for this page
            ' for an example.
            lblWithHyperlinks.Text = PageUtils.HTMLEncode(GetStringToEncode(), True)
            lblWithoutHyperlinks.Text = PageUtils.HTMLEncode(GetStringToEncode(), False)

            Page.DataBind()
        End If
    End Sub

End Class

End Namespace
