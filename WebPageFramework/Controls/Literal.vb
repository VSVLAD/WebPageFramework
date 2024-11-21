
Namespace Controls

    ''' <summary>
    ''' Контрол для произвольной вёрстки
    ''' </summary>
    Public Class Literal
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)
        End Sub

        Public Property Text As String = ""

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty
            Return Text
        End Function

        Public Overrides Sub FromState(State As Dictionary(Of String, Object))
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Text)) Then Text = State(NameOf(Text))
            End If
        End Sub

        Public Overrides Function ToState() As Dictionary(Of String, Object)
            Dim state = MyBase.ToState()
            state(NameOf(Text)) = Text
            Return state
        End Function

    End Class

End Namespace