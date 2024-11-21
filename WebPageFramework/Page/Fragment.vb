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
    Public Property ViewData As Dictionary(Of String, Object) Implements IFragment.ViewData
    Public Property ViewState As Dictionary(Of String, Object) Implements IContainer.ViewState

    Public ReadOnly Property Page As IPage

    Public Sub New(Parent As IContainer, Id As String)
        If Parent Is Nothing Then Throw New ArgumentNullException(NameOf(Parent))
        If Id Is Nothing Then Throw New ArgumentNullException(NameOf(Id))

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

        ' Свойства по-умолчанию
        Me.Controls = New Dictionary(Of String, IHtmlControl)
        Me.ViewData = New Dictionary(Of String, Object)
        Me.ViewState = New Dictionary(Of String, Object)

        ' Свойства по-умолчанию как для контрола
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
        For Each fragm In innerFragments.Cast(Of ILifeCycleEvents)
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

    Public Sub FromState(State As Dictionary(Of String, Object)) Implements IState.FromState

    End Sub

    Public Function ToState() As Dictionary(Of String, Object) Implements IState.ToState

    End Function

    Public Sub OnInit() Implements ILifeCycleEvents.OnInit
        RaiseEvent Init()
    End Sub

    Public Sub OnLoad(FirstRun As Boolean) Implements ILifeCycleEvents.OnLoad
        RaiseEvent Load(FirstRun)
    End Sub

    Public Event Init() Implements ILifeCycleEvents.Init
    Public Event Load(FirstRun As Boolean) Implements ILifeCycleEvents.Load
    Public Event Render() Implements ILifeCycleEvents.Render

    Public Sub OnRender() Implements ILifeCycleEvents.OnRender
        RaiseEvent Render()
    End Sub

End Class
