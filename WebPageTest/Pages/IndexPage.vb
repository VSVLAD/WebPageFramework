Imports Microsoft.AspNetCore.Http
Imports WebPages
Imports WebPages.Controls

Public Class IndexPage
    Inherits Page

    ' Создаём контролы
    Private WithEvents formBtn1 As New Button(Me, NameOf(formBtn1))
    Private WithEvents formBtn2 As New Button(Me, NameOf(formBtn2))
    Private WithEvents formTxt1 As New TextBox(Me, NameOf(formTxt1))
    Private WithEvents formCmb1 As New ComboBox(Me, NameOf(formCmb1))
    Private WithEvents fragmentAlert As New AlertFragment(Me, NameOf(fragmentAlert))

    Private Sub IndexPage_Load(FirstRun As Boolean) Handles Me.Load
        If FirstRun Then
            formBtn1.CSS = "btn btn-danger"
            formBtn1.Text = "Нажми меня!"

            formBtn2.CSS = "btn btn-success"
            formBtn2.Text = "Счётчик"
            formTxt1.CSS = "form-control"

            formCmb1.CSS = "form-select"
            formCmb1.Items.Clear()

            For Each y In Enumerable.Range(2000, 50)
                formCmb1.Items.Add(New ComboBoxItem($"Год {y}", y))
            Next

            Context.Session.SetInt32("counter", 0)
        End If

        ViewData("Title") = "Заголовок страницы"
        ViewData("H1") = "<p>Это контент страницы, сгенерированный в код-бихайнде.</p>"
    End Sub


    Private Sub btn1_Click(arg1 As Button, arg2 As String) Handles formBtn1.Click
        formTxt1.Text = "Привет мир! " & Now.Ticks()
        formTxt1.CSS = "form-control bg-danger text-white"

        formBtn1.Text = "Меня уже нажимали =)"
    End Sub

    Private Sub btn2_Click(arg1 As Button, arg2 As String) Handles formBtn2.Click
        formTxt1.CSS = "form-control bg-success text-black"

        Dim counter = Context.Session.GetInt32("counter")
        If Not counter.HasValue Then counter = 0

        counter += 1
        formBtn2.Text = $"Проверка: {counter}"

        Context.Session.SetInt32("counter", counter)
    End Sub

    Private Sub txt1_TextChanged(arg1 As TextBox, arg2 As String) Handles formTxt1.TextChanged
        ViewData("Title") = arg1.Text
    End Sub

End Class
