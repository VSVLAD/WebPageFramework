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
    Private WithEvents timer1 As New Timer(Me, NameOf(timer1))

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

            ' Каждые 5 секунд будем нажимать сами на зелёную кнопку
            timer1.Interval = 5000
            timer1.SaveCounterMode = True
        End If

        ViewData("Title") = "Заголовок страницы"
        ViewData("H1") = "<p>Это контент страницы, сгенерированный в код-бихайнде.</p>"
    End Sub


    Private Sub formBtn1_Click(sender As HtmlControl, e As HtmlControlEventArgs) Handles formBtn1.Click
        formTxt1.Text = "Привет мир! " & Now.Ticks()
        formTxt1.CSS = "form-control bg-danger text-white"

        formBtn1.Text = "Меня уже нажимали =)"
    End Sub

    Private Sub timer1_Tick(sender As HtmlControl, e As HtmlControlEventArgs) Handles timer1.Tick
        formBtn2_Click(formBtn2, Nothing)
    End Sub

    Private Sub formTxt1_TextChanged(Sender As HtmlControl, e As HtmlControlEventArgs) Handles formTxt1.TextChanged
        ViewData("Title") = formTxt1.Text
    End Sub

    Private Sub formBtn2_Click(sender As HtmlControl, e As HtmlControlEventArgs) Handles formBtn2.Click
        formTxt1.CSS = "form-control bg-success text-black"

        Dim counter = Context.Session.GetInt32("counter")
        If Not counter.HasValue Then counter = 0

        counter += 1
        formBtn2.Text = $"Проверка: {counter}"

        If counter >= 10 Then
            timer1.SaveCounterMode = False
        End If

        Context.Session.SetInt32("counter", counter)
    End Sub

End Class
