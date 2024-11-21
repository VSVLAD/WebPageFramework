Imports WebPages
Imports WebPages.Controls

Public Class AlertFragment
    Inherits Fragment

    Private WithEvents litTime As New Literal(Me, "litTime")

    Public Sub New(Parent As IContainer, Id As String)
        MyBase.New(Parent, Id)
    End Sub

    Public Sub AlertFragment_Load(FirstRun As Boolean) Handles Me.Load
        litTime.Text = Date.Now.ToLongTimeString()
        ViewData("alertColor") = "success"
    End Sub

End Class
