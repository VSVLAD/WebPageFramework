
Namespace Controls

    ''' <summary>
    ''' Обычная кнопка
    ''' </summary>
    Public MustInherit Class HtmlControl
        Implements IHtmlControl, IState

        Public Sub New(Parent As IContainer, Id As String)
            If Parent Is Nothing Then Throw New ArgumentNullException(NameOf(Parent))
            If Id Is Nothing Then Throw New ArgumentNullException(NameOf(Id))

            Me.Id = Id
            Me.Parent = Parent

            ' Свойства по-умолчанию
            Me.Attributes = New Dictionary(Of String, String)
            Me.Enabled = True
            Me.Visible = True
            Me.EnableEvents = True
            Me.EnableState = True

            ' Добавляем элемент в коллекцию контролов родителя
            Me.Parent.Controls.Add(Id, Me)
        End Sub

        Public Property Id As String Implements IHtmlControl.Id

        Public Property Parent As IContainer Implements IHtmlControl.Parent

        Public Property CSS As String Implements IHtmlControl.CSS

        Public Property Enabled As Boolean Implements IHtmlControl.Enabled

        Public Property Visible As Boolean Implements IHtmlControl.Visible

        Public Property Attributes As Dictionary(Of String, String) Implements IHtmlControl.Attributes

        Public Property EnableEvents As Boolean Implements IHtmlControl.EnableEvents

        Public Property EnableState As Boolean Implements IState.EnableState

        Public Overridable Function RenderHtml() As String Implements IHtmlControl.RenderHtml
            Return String.Empty
        End Function

        Public Overridable Function RenderScript() As String Implements IHtmlControl.RenderScript
            Return String.Empty
        End Function

        Public Overridable Sub ProcessEvent(EventName As String, EventArgument As String) Implements IHtmlControl.ProcessEvent
        End Sub

        Public Overridable Sub ProcessFormData(Value As String) Implements IHtmlControl.ProcessFormData
        End Sub

        ''' <summary>
        ''' Восстанавливаем свойства контрола из объекта состояния
        ''' </summary>
        Public Overridable Sub FromState(State As Dictionary(Of String, Object)) Implements IState.FromState
            If State.ContainsKey(NameOf(EnableState)) Then EnableState = CBool(State(NameOf(EnableState)))

            If EnableState Then
                If State.ContainsKey(NameOf(EnableEvents)) Then EnableEvents = CBool(State(NameOf(EnableEvents)))
                If State.ContainsKey(NameOf(Visible)) Then Visible = CBool(State(NameOf(Visible)))
                If State.ContainsKey(NameOf(Enabled)) Then Enabled = CBool(State(NameOf(Enabled)))
                If State.ContainsKey(NameOf(CSS)) Then CSS = CStr(State(NameOf(CSS)))
                If State.ContainsKey(NameOf(Attributes)) Then Attributes = DirectCast(State(NameOf(Attributes)), Dictionary(Of String, String))
            End If
        End Sub

        ''' <summary>
        ''' Сохраняем свойства контрола в объект состояния
        ''' </summary>
        Public Overridable Function ToState() As Dictionary(Of String, Object) Implements IState.ToState
            Dim state As New Dictionary(Of String, Object)
            state(NameOf(EnableState)) = CStr(EnableState)

            If EnableState Then
                state(NameOf(Visible)) = CStr(Visible)
                state(NameOf(Enabled)) = CStr(Enabled)
                state(NameOf(CSS)) = CSS
                state(NameOf(Attributes)) = Attributes
            End If

            Return state
        End Function

        Public Overrides Function ToString() As String
            Return $"{Id}"
        End Function

    End Class

End Namespace