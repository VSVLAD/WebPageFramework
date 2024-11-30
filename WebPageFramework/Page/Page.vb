Option Strict On

Imports System.Threading
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports WebPages.Controls

Public MustInherit Class Page
    Implements IPage, IStateObject, IContainerEvents

    ' Название полей для внутренней инфраструктуры состояния и событий
    Public Const FieldNameEventControl = "wpEventControl"
    Public Const FieldNameEventName = "wpEventName"
    Public Const FieldNameEventArgument = "wpEventArgument"
    Public Const FunctionNamePostBack = "wpPostBack"

    Public Property Context As HttpContext Implements IPage.Context
    Public Property Environment As IWebHostEnvironment Implements IPage.Environment
    Public Property Options As WebPagesOptions Implements IPage.Options
    Public Property Form As IFormCollection Implements IPage.Form
    Public Property Id As String Implements IControl.Id
    Public Property Controls As Dictionary(Of String, IHtmlControl) Implements IContainer.Controls
    Public Property ViewState As ViewObject Implements IContainer.ViewState
    Public Property ViewData As ViewObject Implements IViewData.ViewData
    Public Property EnableState As Boolean Implements IStateObject.EnableState

    Public Event Init() Implements IContainerEvents.Init
    Public Event Load(FirstRun As Boolean) Implements IContainerEvents.Load
    Public Event Render() Implements IContainerEvents.Render

    Protected Friend Sub New()

        ' Значения по-умолчанию
        Me.Controls = New Dictionary(Of String, IHtmlControl)
        Me.ViewData = New ViewObject()
        Me.ViewState = New ViewObject()
        Me.EnableState = True
    End Sub

    ''' <summary>
    ''' Обработка формы
    ''' </summary>
    Public Overridable Async Function ProcessAsync(tokenCancel As CancellationToken) As Task Implements IPage.ProcessAsync

        ' Храним данные в кеше, чтобы можно было возвращаться кнопкой назад в браузере
        ' Если используется SSL - соединение должно быть доверенное, иначе в кеш не попадёт!
        Me.Context.Response.Headers.CacheControl = "private"
        Me.Context.Response.Headers.Date = Date.Now.ToUniversalTime().ToString("R")
        Me.Context.Response.ContentType = "text/html; charset=utf8"
        Me.Context.Response.StatusCode = 200

        ' Проверяем, пришла ли форма и запишем ссылку в свойство Form
        If Context.Request.HasFormContentType Then
            Me.Form = Await Context.Request.ReadFormAsync(tokenCancel)
        End If

        ' Проверяем, что не запросили отмену
        tokenCancel.ThrowIfCancellationRequested()

        ' Инициализация формы и фрагментов
        Me.OnInit()

        ' Загружаем состояние
        Dim pageStateAvailable = Me.PageLoadState(Options.StateProvider, Options.StateFormatter)

        ' Если состояние было успешно прочитано и был PostBack
        ' Пояснение: контролы должны как минимум иметь загруженное состояние с момента первичной загрузки
        If pageStateAvailable AndAlso Me.Form IsNot Nothing Then

            ' Применяем текущее значение полученное из формы
            Me.ApplyControlFormValue()

            ' Сначала событие загрузки формы и фрагментов
            Me.OnLoad(False)

            ' После пользовательские события контролов
            Me.GenerateControlEvent()

        Else
            ' Если не было PostBack, но состояние было успешно прочитано
            If pageStateAvailable Then
                Me.OnLoad(False) ' Повторная загрузка
            Else
                Me.OnLoad(True) ' Начальная загрузка
            End If
        End If

        ' Сохраняем состояние
        Me.PageSaveState(Options.StateProvider, Options.StateFormatter)

        Await Task.CompletedTask
    End Function

    Public Overridable Async Function RenderAsync(tokenCancel As CancellationToken) As Task(Of String) Implements IPage.RenderAsync

        ' Сначала проверяем, что не запросили отмену
        tokenCancel.ThrowIfCancellationRequested()

        ' Вызываем событие перед генерацией контента формы и фрагментов
        Me.OnRender()

        ' Читаем шаблон
        Dim tplContent = Options.TemplateProvider.GetTemplate(Me.Id)

        ' Генерируем заполнитель для формы
        Me.GenerateBeginEndViewData()

        ' Отрисовываем заполнители
        For Each item In ViewData
            tplContent.Replace($"{{{{ {item.Key} }}}}", item.Value?.ToString())
        Next

        ' Отрисовываем контролы
        For Each ctl In Controls
            tplContent.Replace($"{{{{ {ctl.Key} }}}}", ctl.Value.RenderHtml())
        Next

        ' Возвращаем контент
        Return Await Task.FromResult(tplContent.Render())
    End Function

    Public Overridable Sub OnInit() Implements IContainerEvents.OnInit
        RaiseEvent Init()

        '  После событие инициалзиации для фрагментов
        For Each frag In Me.Controls.Where(Function(ctlKv) TypeOf ctlKv.Value Is Fragment).
                                     Select(Function(ctlKv) ctlKv.Value).
                                     Cast(Of IContainerEvents)
            frag.OnInit()
        Next
    End Sub

    Public Overridable Sub OnLoad(FirstRun As Boolean) Implements IContainerEvents.OnLoad
        RaiseEvent Load(FirstRun)

        ' После событие загрузки для фрагментов
        For Each frag In Me.Controls.Where(Function(ctlKv) TypeOf ctlKv.Value Is Fragment).
                             Select(Function(ctlKv) ctlKv.Value).
                             Cast(Of IContainerEvents)
            frag.OnLoad(FirstRun)
        Next
    End Sub

    Public Overridable Sub OnRender() Implements IContainerEvents.OnRender
        RaiseEvent Render()

        ' После событие отрисовки для фрагментов
        For Each frag In Me.Controls.Where(Function(ctlKv) TypeOf ctlKv.Value Is Fragment).
                             Select(Function(ctlKv) ctlKv.Value).
                             Cast(Of IContainerEvents)
            frag.OnRender()
        Next
    End Sub

    Public Function ToState() As ViewObject Implements IStateObject.ToState
        Return ViewState
    End Function

    Public Sub FromState(State As ViewObject) Implements IStateObject.FromState
        ViewState = State
    End Sub

    ''' <summary>
    ''' Вызвать метод Redirect, который завершает обработку страницы
    ''' </summary>
    Public Sub Redirect(Location As String, Optional Permanent As Boolean = False)
        Context.Response.Redirect(Location, Permanent)
        Throw New OperationCanceledException($"Redirect to {Location} initiated", Context.RequestAborted)
    End Sub

End Class