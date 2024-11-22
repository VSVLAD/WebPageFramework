Public Interface IState

    ''' <summary>
    ''' Включено ли сохранение состояния
    ''' </summary>
    Property EnableState As Boolean

    ''' <summary>
    ''' Метод вызывается фреймворком и передаёт объект состояние для инициализации
    ''' </summary>
    Sub FromState(State As StateObject)

    ''' <summary>
    ''' Метод вызывается фреймворком, чтобы получить текущее состояние для сериализации
    ''' </summary>
    Function ToState() As StateObject

End Interface
