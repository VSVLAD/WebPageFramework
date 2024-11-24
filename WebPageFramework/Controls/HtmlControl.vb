
Namespace Controls

    ''' <summary>
    ''' Обычная кнопка
    ''' </summary>
    Public MustInherit Class HtmlControl
        Implements IHtmlControl, IState

        Public Sub New(Parent As IContainer, Id As String)
            If Parent Is Nothing Then Throw New ArgumentNullException(NameOf(Parent))
            If Id Is Nothing Then Throw New ArgumentNullException(NameOf(Id))

            ' Основные свйоства
            Me.Id = Id
            Me.Parent = Parent

            ' Значение свойств по-умолчанию, далее их могут перебивать значения из состояния
            Me.Attributes = New Dictionary(Of String, String)
            Me.EnableState = True
            Me.Enabled = True
            Me.Visible = True
            Me.EnableEvents = True

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


        Public MustOverride Function RenderHtml() As String Implements IHtmlControl.RenderHtml

        Public MustOverride Function RenderScript() As String Implements IHtmlControl.RenderScript

        Public MustOverride Sub ProcessEvent(EventName As String, EventArgument As String) Implements IHtmlControl.ProcessEvent

        Public MustOverride Sub ProcessFormData(Value As String) Implements IHtmlControl.ProcessFormData

        ''' <summary>
        ''' Восстанавливаем свойства контрола из объекта состояния
        ''' </summary>
        Public Overridable Sub FromState(State As StateObject) Implements IState.FromState
            If State.ContainsKey(NameOf(EnableState)) Then EnableState = CBool(State(NameOf(EnableState)))

            If EnableState Then
                If State.ContainsKey(NameOf(EnableEvents)) Then EnableEvents = CBool(State(NameOf(EnableEvents)))
                If State.ContainsKey(NameOf(Visible)) Then Visible = CBool(State(NameOf(Visible)))
                If State.ContainsKey(NameOf(Enabled)) Then Enabled = CBool(State(NameOf(Enabled)))
                If State.ContainsKey(NameOf(CSS)) Then CSS = CStr(State(NameOf(CSS)))
                If State.ContainsKey(NameOf(Attributes)) Then Attributes = State(NameOf(Attributes))
            End If
        End Sub

        ''' <summary>
        ''' Сохраняем свойства контрола в объект состояния
        ''' </summary>
        Public Overridable Function ToState() As StateObject Implements IState.ToState
            Dim state As New StateObject()

            ' Принудительно добавляем свойство в состояние
            state(NameOf(EnableState)) = CStr(EnableState)

            If EnableState Then
                ' Только те свойства, значения которых изменились от умолчания
                If Not Visible Then state(NameOf(Visible)) = CStr(Visible)
                If Not Enabled Then state(NameOf(Enabled)) = CStr(Enabled)
                If Not String.IsNullOrEmpty(CSS) Then state(NameOf(CSS)) = CSS
                If Attributes.Count > 0 Then state(NameOf(Attributes)) = Attributes
            End If

            Return state
        End Function

        Public Overrides Function ToString() As String
            Return $"{Id}"
        End Function

    End Class

End Namespace