Option Strict On

Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports WebPages.Controls

Public MustInherit Class Page
    Implements IPage, IState, IContainerEvents

    ' Название полей для внутренней инфраструктуры состояния и событий
    Public Const FieldNameEventControl = "wpEventControl"
    Public Const FieldNameEventName = "wpEventName"
    Public Const FieldNameEventArgument = "wpEventArgument"
    Public Const FieldNameViewState = "wpViewState"
    Public Const FunctionNamePostBack = "wpPostBack"

    Public Property Context As HttpContext Implements IPage.Context
    Public Property Environment As IWebHostEnvironment Implements IPage.Environment
    Public Property Options As WebPagesOptions Implements IPage.Options
    Public Property Form As IFormCollection Implements IPage.Form
    Public Property Id As String Implements IControl.Id
    Public Property Controls As Dictionary(Of String, IHtmlControl) Implements IContainer.Controls
    Public Property ViewState As StateObject Implements IContainer.ViewState
    Public Property ViewData As StateObject Implements IViewData.ViewData
    Public Property EnableState As Boolean Implements IState.EnableState

    Public Event Init() Implements IContainerEvents.Init
    Public Event Load(FirstRun As Boolean) Implements IContainerEvents.Load
    Public Event Render() Implements IContainerEvents.Render

    Protected Friend Sub New()

        ' Значения по-умолчанию
        Me.Controls = New Dictionary(Of String, IHtmlControl)
        Me.ViewData = New StateObject()
        Me.ViewState = New StateObject()
        Me.EnableState = True
    End Sub

    ''' <summary>
    ''' Обработка формы
    ''' </summary>
    Public Overridable Sub ProcessPage() Implements IPage.Process

        ' Храним данные в кеше, чтобы можно было возвращаться кнопкой назад в браузере
        ' Если используется SSL - соединение должно быть доверенное, иначе в кеш не попадёт!
        Me.Context.Response.Headers.CacheControl = "private"
        Me.Context.Response.Headers.Date = Date.Now.ToUniversalTime().ToString("R")
        Me.Context.Response.ContentType = "text/html; charset=utf8"
        Me.Context.Response.StatusCode = 200

        ' Для удобства чтения формы
        If Context.Request.HasFormContentType AndAlso Context.Request.Form.Keys.Any() Then
            Me.Form = Context.Request.Form
        End If

        ' Иинициализация формы и фрагментов
        Me.OnInit()

        ' Если был PostBack
        If Me.Form IsNot Nothing Then

            ' Применяем состояние формы и контролов
            Me.ApplyState(Options.StateProvider)

            ' Применяем текущее значение полученное из формы
            Me.ApplyControlFormValue()

            ' Сначала событие загрузки формы и фрагментов
            Me.OnLoad(False)

            ' После создаём пользовательские события
            Me.GenerateControlEvents()

        Else
            ' Первичная загрузка формы и фрагментов
            Me.OnLoad(True)

        End If

    End Sub

    Public Overridable Function RenderPage() As String Implements IPage.Render

        ' Вызываем событие перед генерацией контента формы и фрагментов
        Me.OnRender()

        ' Читаем шаблон
        Dim tplContent = Options.TemplateProvider.GetTemplate(Me.Id)

        ' Генерируем заполнитель для формы
        Me.GenerateBeginEndViewData()

        ' Генерируем заполнитель для состояния
        Me.GenerateStateViewData(Me.GenerateState(Options.StateProvider))

        ' Отрисовываем системные заполнители
        Dim lastTemplateLength = tplContent.Length
        tplContent.Replace("<form>", $"{ViewData("__formBegin")}{ViewData("__formViewState")}")

        If lastTemplateLength = tplContent.Length Then
            Throw New Exception($"Шаблон страницы ""{Id}"" должен содержать начальный тег <form> без атрибутов")
        End If

        lastTemplateLength = tplContent.Length
        tplContent.Replace("</form>", $"{ViewData("__formEnd")} ")

        If lastTemplateLength = tplContent.Length Then
            Throw New Exception($"Шаблон страницы ""{Id}"" должен содержать закрывающий тег </form>")
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
        Return tplContent.Render()
    End Function

    Public Function ToState() As StateObject Implements IState.ToState
        Return ViewState
    End Function

    Public Sub FromState(State As StateObject) Implements IState.FromState
        ViewState = State
    End Sub

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

End Class