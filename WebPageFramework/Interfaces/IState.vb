Public Interface IState

    ''' <summary>
    ''' Включено ли сохранение состояния
    ''' </summary>
    Property EnableState As Boolean

    ''' <summary>
    ''' Метод вызывается фреймворком и передаёт объект состояние для инициализации
    ''' </summary>
    Sub FromState(State As Dictionary(Of String, Object))

    ''' <summary>
    ''' Метод вызывается фреймворком, чтобы получить текущее состояние для сериализации
    ''' </summary>
    Function ToState() As Dictionary(Of String, Object)

End Interface
