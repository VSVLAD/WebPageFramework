
Namespace Controls

    ''' <summary>
    ''' Контрол для произвольной вёрстки
    ''' </summary>
    Public Class Literal
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)

            ' Значения по-умолчанию
            Me.Text = String.Empty
        End Sub

        ''' <summary>
        ''' Свойство поддерживает HTML-разметку
        ''' </summary>
        Public Property Text As String

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty
            Return Text
        End Function

        Public Overrides Function RenderScript() As String
            Return String.Empty
        End Function

        Public Overrides Sub ProcessEvent(EventName As String, EventArgument As String)
        End Sub

        Public Overrides Sub ProcessFormData(Value As String)
        End Sub

        Public Overrides Sub FromState(State As StateObject)
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Text)) Then Text = State(NameOf(Text))
            End If
        End Sub

        Public Overrides Function ToState() As StateObject
            Dim state = MyBase.ToState()

            If EnableState Then
                If Not String.IsNullOrEmpty(Text) Then state(NameOf(Text)) = Text
            End If

            Return state
        End Function

    End Class

End Namespace