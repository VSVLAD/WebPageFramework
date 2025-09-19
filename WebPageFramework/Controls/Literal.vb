Option Strict On

Namespace Controls

    ''' <summary>
    ''' Контрол для произвольной вёрстки
    ''' </summary>
    Public Class Literal
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)
        End Sub

        ''' <summary>
        ''' Свойство поддерживает HTML-разметку
        ''' </summary>
        Public Property Text As String = String.Empty

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty
            Return Text
        End Function

        Public Overrides Sub FromState(State As ViewObject)
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Text)) Then Text = CStr(State(NameOf(Text)))
            End If
        End Sub

        Public Overrides Function ToState() As ViewObject
            Dim state = MyBase.ToState()

            If EnableState Then
                If Not String.IsNullOrEmpty(Text) Then state(NameOf(Text)) = Text
            End If

            Return state
        End Function

    End Class

End Namespace