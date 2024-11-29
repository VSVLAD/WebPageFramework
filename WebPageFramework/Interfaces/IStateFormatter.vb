''' <summary>
''' Интерфейс для сериализации объекта состояния в строковое представление, а также обратной десериализации из строки в объект
''' </summary>
Public Interface IStateFormatter

    ''' <summary>
    ''' Десериализация состояния из строки в объект
    ''' </summary>
    Function DeserializeState(TreeState As String) As ViewObject

    ''' <summary>
    ''' Сериализация состояния в строку по данным объекта
    ''' </summary>
    Function SerializeState(TreeState As ViewObject) As String

End Interface
