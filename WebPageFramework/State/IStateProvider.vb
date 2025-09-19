''' <summary>
''' Интерфейс предоставляет методы для сохранения и загрузки объектов состояния из поддерживаемого хранилища
''' </summary>
Public Interface IStateProvider

    ''' <summary>
    ''' Метод должен вернуть потребителю ранее сохраненный объект состояния из хранилища. Если состояния нет, то вернуть Nothing
    ''' </summary>
    Function FromStorage(Page As IPage) As String

    ''' <summary>
    ''' Метод должен сохранить объект состояния в постоянное хранилище. Для идентификации состояния также передаётся уникальный ключ, который генерирует потребитель
    ''' </summary>
    ''' <param name="TreeState">Объект состояния</param>
    ''' <param name="Page">Страница</param>
    Sub ToStorage(TreeState As String, Page As IPage)

End Interface
