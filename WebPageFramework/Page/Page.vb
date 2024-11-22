Option Strict On

Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports WebPages.Controls

Public MustInherit Class Page
    Implements IPage, IState, IContainerEvents

    Public Property Context As HttpContext Implements IPage.Context
    Public Property Environment As IWebHostEnvironment Implements IPage.Environment
    Public Property Options As WebPagesOptions Implements IPage.Options
    Public Property Form As IFormCollection Implements IPage.Form
    Public Property Id As String Implements IControl.Id
    Public Property Controls As Dictionary(Of String, IHtmlControl) Implements IContainer.Controls
    Public Property ViewState As Dictionary(Of String, Object) Implements IContainer.ViewState
    Public Property ViewData As Dictionary(Of String, Object) Implements IViewData.ViewData
    Public Property EnableState As Boolean Implements IState.EnableState

    Public Event Init() Implements IContainerEvents.Init
    Public Event Load(FirstRun As Boolean) Implements IContainerEvents.Load
    Public Event Render() Implements IContainerEvents.Render

    Protected Friend Sub New()

        ' Значения по-умолчанию
        Me.Controls = New Dictionary(Of String, IHtmlControl)
        Me.ViewData = New Dictionary(Of String, Object)
        Me.ViewState = New Dictionary(Of String, Object)
        Me.EnableState = True
    End Sub

    ''' <summary>
    ''' Обработка формы
    ''' </summary>
    Public Overridable Async Function ProcessAsync() As Task Implements IPage.ProcessAsync

        ' Храним данные в кеше, чтобы можно было возвращаться кнопкой назад в браузере
        Me.Context.Response.Headers.CacheControl = "private"
        Me.Context.Response.Headers.Date = Date.Now.ToUniversalTime().ToString("R")

        ' Тип по-умолчанию
        Me.Context.Response.ContentType = "text/html"

        ' Выбираем все фрагменты на странице
        Dim innerFragments = Me.Controls.Where(Function(ctlKv) TypeOf ctlKv.Value Is Fragment).Select(Function(ctlKv) ctlKv.Value)

        ' Иинициализация формы
        Me.OnInit()

        ' Инициализация фрагментов
        For Each frag In innerFragments.Cast(Of IContainerEvents)
            frag.OnInit()
        Next

        ' Если был PostBack
        If Context.Request.Method.ToUpper() = "POST" Then

            ' Читаем форму
            Me.Form = Await Context.Request.ReadFormAsync()

            ' Применяем состояние формы и контролов
            WebPagesHelper.ApplyState(Me, Options.StateProvider)

            ' Применяем текущее значение полученное из формы
            WebPagesHelper.ApplyControlFormValue(Me)

            ' Сначала событие загрузки формы
            Me.OnLoad(False)

            ' Затем событие загрузки фрагментов
            For Each fragm In innerFragments.Cast(Of IContainerEvents)
                fragm.OnLoad(False)
            Next

            ' После создаём пользовательские события
            WebPagesHelper.GenerateControlEvents(Me)

        Else
            ' Первичная загрузка формы
            Me.OnLoad(True)

            ' После первичная загрузка фрагментов
            For Each frag In innerFragments.Cast(Of IContainerEvents)
                frag.OnLoad(True)
            Next

        End If

    End Function

    Public Async Function RenderAsync() As Task(Of String) Implements IPage.RenderAsync

        ' Вызываем событие перед генерацией контента
        Me.OnRender()

        ' Читаем шаблон
        Dim tplContent = Options.TemplateProvider.GetTemplate(Me.Id)

        ' Генерируем заполнитель для формы
        WebPagesHelper.GenerateBeginEndViewForm(Me)

        ' Генерируем заполнитель для состояния
        WebPagesHelper.GenerateStateViewForm(Me, WebPagesHelper.GenerateState(Me, Options.StateProvider))

        ' Отрисовываем системные заполнители
        Dim lastTemplateLength = tplContent.Length
        tplContent.Replace("<body>", $"<body>{ViewData("__formBegin")}{ViewData("__formViewState")}")

        If lastTemplateLength = tplContent.Length Then
            Throw New Exception($"Шаблон страницы ""{Id}"" должен содержать тег <body>")
        End If

        lastTemplateLength = tplContent.Length
        tplContent.Replace("</body>", $"{ViewData("__formEnd")}</body>")

        If lastTemplateLength = tplContent.Length Then
            Throw New Exception($"Шаблон страницы ""{Id}"" должен содержать тег </body>")
        End If

        ' Отрисовываем пользовательские заполнители
        For Each item In ViewData
            tplContent.Replace($"{{{{ {item.Key} }}}}", item.Value.ToString())
        Next

        ' Отрисовываем контролы
        For Each ctl In Controls
            tplContent.Replace($"{{{{ {ctl.Key} }}}}", ctl.Value.RenderHtml())
        Next

        ' Возвращаем контент
        Return Await Task.FromResult(tplContent.Render())
    End Function

    Public Function ToState() As Dictionary(Of String, Object) Implements IState.ToState
        Return ViewState
    End Function

    Public Sub FromState(State As Dictionary(Of String, Object)) Implements IState.FromState
        ViewState = State
    End Sub

    Public Overridable Sub OnInit() Implements IContainerEvents.OnInit
        RaiseEvent Init()
    End Sub

    Public Overridable Sub OnLoad(FirstRun As Boolean) Implements IContainerEvents.OnLoad
        RaiseEvent Load(FirstRun)
    End Sub

    Public Overridable Sub OnRender() Implements IContainerEvents.OnRender
        RaiseEvent Render()
    End Sub

End Class