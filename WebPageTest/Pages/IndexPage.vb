Option Strict On

Imports WebPages
Imports WebPages.Controls
Imports WebPages.Bootstrap5.Controls

Public Class IndexPage
    Inherits Page

    ' Создаём контролы
    Private WithEvents formBtnBs1 As New Button(Me, NameOf(formBtnBs1))
    Private WithEvents formBtn2 As New Button(Me, NameOf(formBtn2))
    Private WithEvents formTxt1 As New TextBox(Me, NameOf(formTxt1))
    Private WithEvents formCmb1 As New ComboBox(Me, NameOf(formCmb1))
    Private WithEvents fragmentAlert As New AlertFragment(Me, NameOf(fragmentAlert))
    Private WithEvents timer1 As New Timer(Me, NameOf(timer1))

    Private Sub IndexPage_Load(FirstRun As Boolean) Handles Me.Load
        If FirstRun Then
            formTxt1.CSS = "form-control"
            formBtnBs1.Text = "Нажми меня!"
            formBtnBs1.CSS = "btn btn-danger"

            formBtn2.CSS = "btn btn-success"
            formBtn2.Text = "Счётчик"

            formCmb1.EnableEvents = False
            formCmb1.CSS = "form-select"
            formCmb1.Items.Clear()

            For Each y In Enumerable.Range(2000, 25)
                formCmb1.Items.Add(New ComboBoxItem($"Год {y}", CStr(y)))
            Next

            ' Каждые 5 секунд будем нажимать сами на зелёную кнопку
            timer1.Enabled = False
            timer1.Interval = 5000
            timer1.ResetCounter = True
        End If

        ViewData("Title") = "Заголовок страницы"
        ViewData("H1") = "<p>Это контент страницы, сгенерированный в код-бихайнде.</p>"
    End Sub


    Private Sub formBtn1_Click(sender As HtmlControl, e As HtmlControlEventArgs) Handles formBtnBs1.Click
        formTxt1.Text = "Привет мир! " & Now.Ticks()
        formTxt1.CSS = "form-control bg-danger text-white"

        formBtnBs1.Text = "Меня уже нажимали =)"
    End Sub

    Private Sub timer1_Tick(sender As HtmlControl, e As HtmlControlEventArgs) Handles timer1.Tick
        formBtn2_Click(formBtn2, Nothing)
    End Sub

    Private Sub formTxt1_TextChanged(Sender As HtmlControl, e As HtmlControlEventArgs) Handles formTxt1.TextChanged
        ViewData("Title") = formTxt1.Text
    End Sub

    Private Sub formBtn2_Click(sender As HtmlControl, e As HtmlControlEventArgs) Handles formBtn2.Click
        formTxt1.CSS = "form-control bg-success text-black"

        ' Читаем
        Dim counter = CInt(If(ViewState("counter"), 0))
        counter += 1

        formBtn2.Text = $"Проверка: {counter}"

        If counter >= 10 Then
            timer1.ResetCounter = False
        End If

        ' на каждое четное нажатие показываем фрагмент
        If fragmentAlert.Visible = False AndAlso counter Mod 2 = 0 Then
            fragmentAlert.Visible = True
        End If

        ' Выводим год в текстовое поле
        formTxt1.Text = $"Выбран год: {formCmb1.SelectedValue}"

        ' Запоминаем
        ViewState("counter") = counter
    End Sub

End Class
