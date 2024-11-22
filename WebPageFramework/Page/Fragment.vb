Imports WebPages.Controls

''' <summary>
''' Фрагмент позволяет подгружать пользовательские шаблоны и выполнять пользовательские классы
''' </summary>
Public MustInherit Class Fragment
    Implements IFragment

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

    Public ReadOnly Property Page As IPage

    Protected Sub New(Parent As IContainer, Id As String)
        ArgumentNullException.ThrowIfNull(NameOf(Parent))
        ArgumentNullException.ThrowIfNull(NameOf(Id))

        ' Основные идентификаторы
        Me.Id = Id
        Me.Parent = Parent

        ' Добавим элемент управления в форму
        If TypeOf Parent Is IPage Then
            Me.Page = Parent
            Me.Page.Controls.Add(Id, Me)
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

        '' Применяем состояние формы и контролов
        'WebPagesHelper.ApplyState(FragmentPage, FragmentPage.Options.StateProvider)

        '' Применяем текущее значение полученное из формы
        'WebPagesHelper.ApplyControlFormValue(FragmentPage)

        '' Создаём пользовательские события
        'WebPagesHelper.GenerateControlEvents(FragmentPage)

        ' Если скрыт, то не выполняем шаг отрисовки
        If Not Visible Then Return String.Empty

        ' Инициализации фрагмента и пользовательских контролов в текущем фрагменте
        RaiseEvent Init()

        ' Если существуют фрагменты в списке контролов, вызываем рекурсивно их инициализацию
        Dim innerFragments = Me.Controls.Where(Function(ctlKv) TypeOf ctlKv.Value Is Fragment).Select(Function(ctlKv) ctlKv.Value)

        ' Инициализации внутреннего фрагмента
        For Each fragm In innerFragments.Cast(Of IContainerEvents)
            fragm.OnInit()
        Next

        ' Если был PostBack
        If Page.Context.Request.Method.ToUpper() = "POST" Then

            '' Применяем состояние формы и контролов
            'WebPagesHelper.ApplyState(Me, Options.StateProvider)

            '' Применяем текущее значение полученное из формы
            'WebPagesHelper.ApplyControlFormValue(Me)

            ' Сначала событие загрузки фрагмента
            RaiseEvent Load(False)

            ' Создаём пользовательские события
            'WebPagesHelper.GenerateControlEvents(Me)

        Else
            ' Первичная загрузка формы
            RaiseEvent Load(True)
        End If

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

    Public Sub ProcessEvent(EventName As String, EventArgument As String) Implements IHtmlControl.ProcessEvent
    End Sub

    Public Sub ProcessFormData(Value As String) Implements IHtmlControl.ProcessFormData
    End Sub

    Public Sub FromState(State As StateObject) Implements IState.FromState
    End Sub

    Public Function ToState() As StateObject Implements IState.ToState
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
