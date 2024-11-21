
Вы можете добавлять пользовательские классы для CodeBehind таким образом:

```
Public Class IndexPage
    Inherits Page

End Class
```

Каждое имя класса связывается с файлом шаблона с расширением htm. Файл шаблона может содержать плейсхолдеры {{ }} внутри которых содержатся уникальные имена. Эти имена могут быть связаны с серверными элементами управления, пользовательскими и системными данными.
Например, шаблон может выглядеть следующим образом:

```
<!DOCTYPE html>
<html lang="ru-RU">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link type="text/css" rel="stylesheet" href="/lib/bootstrap/bootstrap.css" />
</head>
<body>
    {{ formBegin }}
    {{ formViewState }}
    <section class="container">
        <div class="row">
            <div class="col-12 bg-dark text-white">
                <h1 class="h4 py-3 px-5">{{ H1 }}</h1>
            </div>
        </div>
    </section>
    <section class="container">
        <div class="row mt-3">
            <div class="col-md-6">
                {{ formTxt1 }}
            </div>
        </div>
    </section>
    {{ formEnd }}
</body>
</html>
```

Заменитель {{ formBegin }} и {{ formEnd }} раскрывается в теги <form> и </form>. А заменитель {{ formViewState }} раскрывается в <input type=hidden /> в которое будет записан объект состояния. Объект состояния сериализуется в JSON, который упаковывается и шифруется.
По-умолчанию в него записываются все свойства элементов управления, но это поведение можно переопределить.

Что касается заменителя {{ formTxt1 }}, то мы хотели бы, чтобы это было текстовое поле. Для этого в классе для CodeBehind требуется зарегистрировать элемент управления. Перепишем класс таким образом:

```
Imports WebPages
Imports WebPages.Controls

Public Class IndexPage
    Inherits Page

    Private WithEvents txt1 As New TextBox(Me, "formTxt1")

    Private Sub IndexPage_Load(FirstRun As Boolean) Handles Me.Load
        If FirstRun Then
            txt1.CSS = "form-control"
            txt1.Text = "Ваш текст здесь ..."
        End If
    End Sub

    Private Sub txt1_TextChanged(arg1 As TextBox, arg2 As String) Handles txt1.TextChanged
        ViewData("H1") = txt1.Text & " " & Now.Date().ToString()
    End Sub

End Class
```

Сначала мы создаём экземпляр класса для текстового поля и передаём ссылку на объект контейнера (текущая страница) и идентификатор элемента управления. Может обрабатывать его события, например, при изменении текстового поля создадим строку с пользовательским текстом и текущей датой.
Далее записываем это значение в словарь ViewData с ключом "H1". Вы уже наверное поняли, что заменитель в странице {{ H1 }} будет заменён на это значение.

Можете запустить проект и поменять текст в поле. Каждое изменение будет создавать PostBack события. Эти события преобразуются в события элемента управления, которое мы можем перехватить и обработать.
Пользователю вернётся страница и отрисованный элемент управления, в котором будет пользовательский текст. Если будут несколько элементов управления на форме и они будут перехватывать события, то текст в нашем поле не будет изменён, т.к. его значение сохранится в объекте состояния.
Объект состояния передаётся между событиями PostBack практически также, как в классическом WebForms в ASP.NET.
