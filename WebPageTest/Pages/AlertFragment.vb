Imports WebPages
Imports WebPages.Controls

Public Class AlertFragment
    Inherits Fragment

    Private WithEvents litTime As New Literal(Me, "litTime")
    Private WithEvents btnHideMe As New Button(Me, "btnHideMe")

    Public Sub New(Parent As IContainer, Id As String)
        MyBase.New(Parent, Id)
    End Sub

    Public Sub AlertFragment_Load(FirstRun As Boolean) Handles Me.Load
        If FirstRun Then
            ViewState("alertColor") = "success"

            btnHideMe.CSS = "btn btn-success"
            btnHideMe.Text = "[ X ]"
        End If

        litTime.Text = Date.Now.ToLongTimeString()

        ' Цвет возьмём, который ранее был сохранен в состоянии фрагмента
        ViewData("alertColor") = ViewState("alertColor")
    End Sub

    Private Sub btnHideMe_Click(sender As HtmlControl, e As HtmlControlEventArgs) Handles btnHideMe.Click
        Me.Visible = False
    End Sub

End Class
