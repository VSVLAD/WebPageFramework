Option Strict On

Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports WebPages.Controls

Public MustInherit Class Page
    Implements IPage, IState, ILifeCycleEvents

    Public Property Context As HttpContext Implements IPage.Context
    Public Property Environment As IWebHostEnvironment Implements IPage.Environment
    Public Property Options As WebPagesOptions Implements IPage.Options
    Public Property Form As IFormCollection Implements IPage.Form
    Public Property Id As String Implements IControl.Id
    Public Property Controls As Dictionary(Of String, IHtmlControl) Implements IContainer.Controls
    Public Property ViewState As Dictionary(Of String, Object) Implements IContainer.ViewState
    Public Property ViewData As Dictionary(Of String, Object) Implements IViewData.ViewData
    Public Property EnableState As Boolean Implements IState.EnableState


    ' Создавать экземпляр сможет только фабрика форм
    Protected Friend Sub New()

        ' Свойства по-умолчанию
        Me.Controls = New Dictionary(Of String, IHtmlControl)
        Me.ViewData = New Dictionary(Of String, Object)
        Me.ViewState = New Dictionary(Of String, Object)
        Me.EnableState = True
    End Sub

    ''' <summary>
    ''' Обработка формы
    ''' </summary>
    Public Overridable Async Function ProcessAsync() As Task(Of String) Implements IPage.ProcessAsync

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
        For Each frag In innerFragments.Cast(Of ILifeCycleEvents)
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
            For Each fragm In innerFragments.Cast(Of ILifeCycleEvents)
                fragm.OnLoad(False)
            Next

            ' После создаём пользовательские события
            WebPagesHelper.GenerateControlEvents(Me)

        Else
            ' Первичная загрузка формы
            Me.OnLoad(True)

            ' После первичная загрузка фрагментов
            For Each frag In innerFragments.Cast(Of ILifeCycleEvents)
                frag.OnLoad(True)
            Next

        End If

        ' Генерируем заменители для формы
        WebPagesHelper.GenerateBeginEndViewForm(Me)

        ' Генерируем заменитель для состояния
        WebPagesHelper.GenerateStateViewForm(Me, WebPagesHelper.GenerateState(Me, Options.StateProvider))

        ' Подсказываем, что начинаем генерировать контент
        Me.OnRender()

        ' Читаем шаблон
        Dim tplContent = Options.TemplateProvider.GetTemplate(Me.Id)

        ' Отрисовываем системные и пользовательские заменители
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

    Public Overridable Sub OnInit() Implements ILifeCycleEvents.OnInit
        RaiseEvent Init()
    End Sub

    Public Overridable Sub OnLoad(FirstRun As Boolean) Implements ILifeCycleEvents.OnLoad
        RaiseEvent Load(FirstRun)
    End Sub

    Public Event Init() Implements ILifeCycleEvents.Init
    Public Event Load(FirstRun As Boolean) Implements ILifeCycleEvents.Load
    Public Event Render() Implements ILifeCycleEvents.Render

    Public Overridable Sub OnRender() Implements ILifeCycleEvents.OnRender
        RaiseEvent Render()
    End Sub

End Class