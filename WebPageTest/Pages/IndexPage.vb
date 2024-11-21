Imports Microsoft.AspNetCore.Http
Imports WebPages
Imports WebPages.Controls

Public Class IndexPage
    Inherits Page

    ' Создаём контролы
    Private WithEvents btn1 As New Button(Me, "formBtn1")
    Private WithEvents btn2 As New Button(Me, "formBtn2")
    Private WithEvents txt1 As New TextBox(Me, "formTxt1")
    Private WithEvents cmb1 As New ComboBox(Me, "formCmb1")
    Private WithEvents fragmentAlert As New AlertFragment(Me, "fragmentAlert")


    Private Sub IndexPage_Load(First As Boolean) Handles Me.Load
        If First Then
            btn1.CSS = "btn btn-danger"
            btn1.Text = "Нажми меня!"

            btn2.CSS = "btn btn-success"
            btn2.Text = "Счётчик"
            txt1.CSS = "form-control"

            cmb1.CSS = "form-select"
            cmb1.Items.Clear()

            For Each y In Enumerable.Range(2000, 50)
                cmb1.Items.Add(New ComboBoxItem($"Год {y}", y))
            Next

            Context.Session.SetInt32("counter", 0)
        End If

        ViewData("Title") = "Заголовок страницы"
        ViewData("H1") = "<p>Это контент страницы, сгенерированный в код-бихайнде.</p>"
    End Sub


    Private Sub btn1_Click(arg1 As Button, arg2 As String) Handles btn1.Click
        txt1.Text = "Привет мир! " & Now.Ticks()
        txt1.CSS = "form-control bg-danger text-white"

        btn1.Text = "Меня уже нажимали =)"
    End Sub

    Private Sub btn2_Click(arg1 As Button, arg2 As String) Handles btn2.Click
        txt1.CSS = "form-control bg-success text-black"

        Dim counter = Context.Session.GetInt32("counter")
        If Not counter.HasValue Then counter = 0

        counter += 1
        btn2.Text = $"Проверка: {counter}"

        Context.Session.SetInt32("counter", counter)
    End Sub

    Private Sub txt1_TextChanged(arg1 As TextBox, arg2 As String) Handles txt1.TextChanged
        ViewData("Title") = arg1.Text
    End Sub

End Class
