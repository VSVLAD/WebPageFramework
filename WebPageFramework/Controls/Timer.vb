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
            Me.SaveCounterMode = False
        End Sub

        Public Event Tick As HtmlControlEventHandler

        ''' <summary>
        ''' Интервал срабатывания события таймера
        ''' </summary>
        Public Property Interval As Integer

        ''' <summary>
        ''' Начальное время установки таймера для сохранения в состоянии
        ''' </summary>
        Private FirstCounterDate As Date

        Private propSaveCounterMode As Boolean

        ''' <summary>
        ''' Флаг заставляет счётчик не сбрасывать накопленное время между Postback запросами
        ''' </summary>
        Public Property SaveCounterMode As Boolean
            Get
                Return propSaveCounterMode
            End Get
            Set(value As Boolean)
                FirstCounterDate = Date.UtcNow
                propSaveCounterMode = value
            End Set
        End Property

        Public Overrides Function RenderHtml() As String
            If Enabled AndAlso EnableEvents AndAlso Interval > 0 Then
                If SaveCounterMode Then

                    ' Рассчитать заново дату тика с учетом начальной даты таймера
                    Dim nextExecutionTime = FirstCounterDate.AddMilliseconds(Interval)

                    Return $"<script defer>
                                (function() {{
                                    const nextExecutionTime = new Date('{nextExecutionTime:yyyy-MM-ddTHH:mm:ss.fffZ}').getTime();
                                    let timerId;

                                    const timerCheck = function() {{
                                        const currentTime = new Date().getTime();
                                        if (currentTime >= nextExecutionTime) {{
                                            clearInterval(timerId);
                                            wpPostBack('{Id}', 'Tick', '');
                                        }}
                                    }}

                                    timerId = setInterval(timerCheck, 1000);
                                    timerCheck();
                                }})();
                            </script>"
                Else
                    Return $"<script defer>
                            setTimeout(() => wpPostBack('{HttpUtility.HtmlAttributeEncode(Id)}', 'Tick', ''), {Interval});
                        </script>"
                End If
            Else
                Return String.Empty
            End If
        End Function

        Public Overrides Function RenderScript() As String
            Return String.Empty
        End Function

        Public Overrides Function ProcessEvent(EventName As String, EventArgument As String) As Boolean
            If EnableEvents AndAlso EventName = "Tick" Then
                FirstCounterDate = Date.UtcNow
                RaiseEvent Tick(Me, New HtmlControlEventArgs(EventArgument))

                Return True
            Else
                Return False
            End If
        End Function

        Public Overrides Function ProcessFormData(Value As String) As Boolean
            Return False
        End Function

        Public Overrides Sub FromState(State As StateObject)
            MyBase.FromState(State)

            If State.ContainsKey(NameOf(EnableEvents)) Then EnableEvents = CBool(State(NameOf(EnableEvents)))
            If State.ContainsKey(NameOf(Interval)) Then Interval = CInt(State(NameOf(Interval)))
            If State.ContainsKey(NameOf(SaveCounterMode)) Then
                SaveCounterMode = CBool(State(NameOf(SaveCounterMode)))
                FirstCounterDate = CDate(State(NameOf(FirstCounterDate)))
            End If
        End Sub

        Public Overrides Function ToState() As StateObject
            Dim state = MyBase.ToState()

            If Not EnableEvents Then state(NameOf(EnableEvents)) = EnableEvents
            If Interval > 0 Then state(NameOf(Interval)) = Interval

            If SaveCounterMode Then
                state(NameOf(SaveCounterMode)) = SaveCounterMode
                state(NameOf(FirstCounterDate)) = FirstCounterDate
            End If

            Return state
        End Function

    End Class

End Namespace