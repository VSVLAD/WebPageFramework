Imports System.Text
Imports System.Web

Namespace Controls

    ''' <summary>
    ''' Таймер для периодических PostBack действий
    ''' </summary>
    Public Class Timer
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)

            ' Значения по-умолчанию
            Me.Interval = 0
        End Sub

        Public Event Tick As Action(Of Timer, String)

        Public Property Interval As Integer

        Public Overrides Function RenderHtml() As String
            If Enabled AndAlso EnableEvents AndAlso Interval > 0 Then
                Return $"<script defer>
                            setInterval(() => wpPostback('{HttpUtility.HtmlAttributeEncode(Id)}', 'Tick', ''), {Interval});
                        </script>"
            Else
                Return String.Empty
            End If
        End Function

        Public Overrides Function RenderScript() As String
            Return String.Empty
        End Function

        Public Overrides Sub ProcessEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "Tick" Then
                RaiseEvent Tick(Me, EventArgument)
            End If
        End Sub

        Public Overrides Sub ProcessFormData(Value As String)
        End Sub

        Public Overrides Sub FromState(State As StateObject)
            MyBase.FromState(State)

            If State.ContainsKey(NameOf(EnableEvents)) Then EnableEvents = CBool(State(NameOf(EnableEvents)))
            If State.ContainsKey(NameOf(Interval)) Then Interval = CInt(State(NameOf(Interval)))
        End Sub

        Public Overrides Function ToState() As StateObject
            Dim state = MyBase.ToState()

            If Not EnableEvents Then state(NameOf(EnableEvents)) = EnableEvents
            If Interval > 0 Then state(NameOf(Interval)) = Interval

            Return state
        End Function

    End Class

End Namespace