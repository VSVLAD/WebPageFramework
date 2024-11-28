Option Strict On

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
            Me.ResetCounter = True
        End Sub

        Private propInterval As Integer
        Private propResetCounter As Boolean

        ''' <summary>
        ''' Начальное время установки таймера для сохранения в состоянии
        ''' </summary>
        Private FirstCounterDate As Date

        Public Event Tick As HtmlControlEventHandler

        ''' <summary>
        ''' Интервал срабатывания события таймера
        ''' </summary>
        Public Property Interval As Integer
            Get
                Return propInterval
            End Get
            Set(value As Integer)
                FirstCounterDate = Date.UtcNow
                propInterval = value
            End Set
        End Property

        ''' <summary>
        ''' Флаг заставляет счётчик сбрасывать накопленное время между Postback запросами. Это режим работы по-умолчанию.
        ''' Если выключить флаг, тогда после PostBack запроса счётчик продолжать аккумулировать время и событие выполнится ровно как задано интервалом.
        ''' </summary>
        Public Property ResetCounter As Boolean
            Get
                Return propResetCounter
            End Get
            Set(value As Boolean)
                FirstCounterDate = Date.UtcNow
                propResetCounter = value
            End Set
        End Property

        Public Overrides Function RenderHtml() As String
            If Enabled AndAlso EnableEvents AndAlso Interval > 0 Then
                If Not ResetCounter Then

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
                                            wpPostBack('{HttpUtility.HtmlAttributeEncode(Id)}', 'Tick', '');
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

        Public Overrides Sub ProcessControlEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "Tick" Then
                FirstCounterDate = Date.UtcNow
                RaiseEvent Tick(Me, New HtmlControlEventArgs(EventArgument))
            End If
        End Sub

        Public Overrides Sub FromState(State As ViewObject)
            MyBase.FromState(State)

            If State.ContainsKey(NameOf(EnableEvents)) Then EnableEvents = CBool(State(NameOf(EnableEvents)))
            If State.ContainsKey(NameOf(Interval)) Then Interval = CInt(State(NameOf(Interval)))
            If State.ContainsKey(NameOf(ResetCounter)) Then
                ResetCounter = CBool(State(NameOf(ResetCounter)))
                FirstCounterDate = CDate(State(NameOf(FirstCounterDate)))
            End If
        End Sub

        Public Overrides Function ToState() As ViewObject
            Dim state = MyBase.ToState()

            If Not EnableEvents Then state(NameOf(EnableEvents)) = EnableEvents
            If Interval > 0 Then state(NameOf(Interval)) = Interval

            If Not ResetCounter Then
                state(NameOf(ResetCounter)) = ResetCounter
                state(NameOf(FirstCounterDate)) = FirstCounterDate
            End If

            Return state
        End Function

    End Class

End Namespace