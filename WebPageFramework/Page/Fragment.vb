Option Strict On

Imports WebPages.Controls

''' <summary>
''' Фрагмент позволяет подгружать пользовательские шаблоны и выполнять пользовательские классы
''' </summary>
Public MustInherit Class Fragment
    Implements IFragment

    ' Список ключей, которые не могут использоваться для хранения внутри ViewState. Это свойства фрагмента как HTML контрола и они должны отдельно обрабатываться
    Private ReadOnly IgnoredUserViewStateKeys As String() = {NameOf(EnableState), NameOf(EnableEvents), NameOf(Visible), NameOf(Enabled), NameOf(CSS), NameOf(Attributes)}

    Public Property CSS As String Implements IHtmlControl.CSS
    Public Property Enabled As Boolean Implements IHtmlControl.Enabled
    Public Property Visible As Boolean Implements IHtmlControl.Visible
    Public Property Attributes As Dictionary(Of String, String) Implements IHtmlControl.Attributes
    Public Property EnableEvents As Boolean Implements IHtmlControl.EnableEvents

    Public Property Parent As IContainer Implements IHtmlControl.Parent
    Public Property Id As String Implements IControl.Id

    Public Property EnableState As Boolean Implements IState.EnableState

    Public Property Controls As Dictionary(Of String, IHtmlControl) Implements IContainer.Controls
    Public Property ViewData As StateObject Implements IFragment.ViewData
    Public Property ViewState As StateObject Implements IContainer.ViewState

    Public Event Init() Implements IContainerEvents.Init
    Public Event Load(FirstRun As Boolean) Implements IContainerEvents.Load
    Public Event Render() Implements IContainerEvents.Render

    ''' <summary>
    ''' Ссылка на страницу, где содержится фрагмент
    ''' </summary>
    Public ReadOnly Property Page As IPage
        Get
            Return DirectCast(Me.Parent, IPage)
        End Get
    End Property

    Protected Sub New(Parent As IContainer, Id As String)
        ArgumentNullException.ThrowIfNull(Parent, NameOf(Parent))
        ArgumentNullException.ThrowIfNull(Id, NameOf(Id))

        ' Основные идентификаторы
        Me.Id = Id
        Me.Parent = Parent

        ' Добавим элемент управления в форму
        If TypeOf Parent Is IPage Then
            If Not Me.Page.Controls.ContainsKey(Id) Then
                Me.Page.Controls.Add(Id, Me)
            End If
        Else
            Throw New NotImplementedException("Фрагмент в качестве родителя для другого фрагмента пока не поддерживается")
        End If

        ' Значения по-умолчанию
        Me.Controls = New Dictionary(Of String, IHtmlControl)
        Me.ViewData = New StateObject()
        Me.ViewState = New StateObject()

        ' Значения по-умолчанию как для контрола
        Me.Attributes = New Dictionary(Of String, String)
        Me.Enabled = True
        Me.Visible = True
        Me.EnableEvents = True
        Me.EnableState = True
    End Sub

    Public Function RenderHtml() As String Implements IHtmlControl.RenderHtml

        ' Если скрыт, то не выполняем шаг отрисовки
        If Not Visible Then Return String.Empty

        ' Читаем шаблон фрагмента
        Dim tplContent = Page.Options.TemplateProvider.GetTemplate(Me.GetType().Name)

        ' Отрисовываем системные и пользовательские заменители
        For Each item In ViewData
            tplContent.Replace($"{{{{ {item.Key} }}}}", item.Value.ToString())
        Next

        ' Отрисовываем контролы
        For Each ctl In Controls
            tplContent.Replace($"{{{{ {ctl.Key} }}}}", ctl.Value.RenderHtml())
        Next

        Return tplContent.Render()
    End Function

    Public Function RenderScript() As String Implements IHtmlControl.RenderScript
        Return String.Empty
    End Function

    Public Sub ProcessControlEvent(EventName As String, EventArgument As String) Implements IHtmlControl.ProcessControlEvent
    End Sub

    Public Sub ProcessFormData(Value As String) Implements IHtmlControl.ProcessFormData
    End Sub

    Public Sub FromState(State As StateObject) Implements IState.FromState
        If State.ContainsKey(NameOf(EnableState)) Then EnableState = CBool(State(NameOf(EnableState)))

        If EnableState Then
            If State.ContainsKey(NameOf(EnableEvents)) Then EnableEvents = CBool(State(NameOf(EnableEvents)))
            If State.ContainsKey(NameOf(Visible)) Then Visible = CBool(State(NameOf(Visible)))
            If State.ContainsKey(NameOf(Enabled)) Then Enabled = CBool(State(NameOf(Enabled)))
            If State.ContainsKey(NameOf(CSS)) Then CSS = CStr(State(NameOf(CSS)))
            If State.ContainsKey(NameOf(Attributes)) Then Attributes = DirectCast(State(NameOf(Attributes)), Dictionary(Of String, String))

            ' Восстанавливаем пользовательские элементы состояния, кроме свойств фрагмента
            For Each item In State.Where(Function(kv) Not IgnoredUserViewStateKeys.Contains(kv.Key))
                Me.ViewState(item.Key) = item.Value
            Next
        End If
    End Sub

    ' Добавляем в состояние объекты фрагмента и всех внутренних контролов
    Public Function ToState() As StateObject Implements IState.ToState
        Dim state As New StateObject()

        ' Принудительно добавляем свойство в состояние
        state(NameOf(EnableState)) = CStr(EnableState)

        If EnableState Then

            ' Только те свойства, значения которых изменились от умолчания
            If Not Visible Then state(NameOf(Visible)) = CStr(Visible)
            If Not Enabled Then state(NameOf(Enabled)) = CStr(Enabled)
            If Not String.IsNullOrEmpty(CSS) Then state(NameOf(CSS)) = CSS
            If Attributes.Count > 0 Then state(NameOf(Attributes)) = Attributes

            ' Добавляем пользовательские элементы состояния, кроме свойств фрагмента
            For Each item In Me.ViewState.Where(Function(kv) Not IgnoredUserViewStateKeys.Contains(kv.Key))
                state(item.Key) = item.Value
            Next
        End If

        Return state
    End Function

    Public Sub OnInit() Implements IContainerEvents.OnInit
        RaiseEvent Init()
    End Sub

    Public Sub OnLoad(FirstRun As Boolean) Implements IContainerEvents.OnLoad
        RaiseEvent Load(FirstRun)
    End Sub

    Public Sub OnRender() Implements IContainerEvents.OnRender
        RaiseEvent Render()
    End Sub


End Class
