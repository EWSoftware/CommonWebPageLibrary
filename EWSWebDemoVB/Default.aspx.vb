'==============================================================================
' System  : ASP.NET Common Web Page Classes Demo
' File    : Default.aspx.vb
' Author  : Eric Woodruff  (Eric@EWoodruff.us)
' Updated : Fri 11/26/2004
' Note    : Copyright 2002-2003, Eric Woodruff, All rights reserved
' Compiler: Microsoft VB.NET
'
' This is the About/start-up page for the application
'
' Version     Date     Who  Comments
' =============================================================================
' 1.0.0    11/28/2003  EFW  Created the code
'==============================================================================

Option Strict

Imports EWSoftware.Web


Namespace EWSWebDemoVB


Partial Class DefaultPage
    Inherits EWSoftware.Web.BasePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            Me.PageTitle = "ASP.NET Common Web Page Classes Demo"
            Me.PageDescription = "The About page for the EWSoftware.Web demo"
            Me.PageKeywords = "BasePage, PageUtils"
            Me.Robots = RobotOptions.Index Or RobotOptions.Follow

            ' Retrieve name and version from application settings in Web.Config
            lblAppName.Text = ConfigurationManager.AppSettings("AppName")
            lblVersion.Text = ConfigurationManager.AppSettings("Version")
            lblReleaseDate.Text = ConfigurationManager.AppSettings("ReleaseDate")
        End If
    End Sub

End Class

End Namespace
