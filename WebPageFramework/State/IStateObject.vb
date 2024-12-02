''' <summary>
''' Интерфейс предоставляет методы с помощью которых, объект может сохранить/восстановить своё состояние используя объект состояния
''' </summary>
Public Interface IStateObject

    ''' <summary>
    ''' Включено ли сохранение состояния
    ''' </summary>
    Property EnableState As Boolean

    ''' <summary>
    ''' Метод вызывается фреймворком и передаёт объект состояния, чтобы формы, фрагменты, контролы могли инициализироваться
    ''' </summary>
    Sub FromState(State As ViewObject)

    ''' <summary>
    ''' Метод вызывается фреймворком, чтобы получить текущее состояние для сериализации объекта: формы, фрагменты, контролы
    ''' </summary>
    Function ToState() As ViewObject

End Interface
