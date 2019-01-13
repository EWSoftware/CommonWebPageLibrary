Imports System.Configuration

Partial Class MenuLayout
    Inherits System.Web.UI.MasterPage

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        ' Get title from Web.Config
        If Not Page.IsPostBack Then
            lblAppName.Text = ConfigurationManager.AppSettings("MenuAppName")
        End If

    End Sub
End Class

