Imports System.Threading
Imports Microsoft.AspNetCore.Http

Namespace Controls

    Public Interface IHtmlControl
        Inherits IControl, IStateObject

        ''' <summary>
        ''' Класс CSS
        ''' </summary>
        Property CSS As String

        ''' <summary>
        ''' Можно ли использовать элемент управления
        ''' </summary>
        Property Enabled As Boolean

        ''' <summary>
        ''' Видимый ли элемент управления
        ''' </summary>
        Property Visible As Boolean

        ''' <summary>
        ''' Дополнительные атрибуты для элемента управления
        ''' </summary>
        Property Attributes As Dictionary(Of String, String)

        ''' <summary>
        ''' Включена ли генерация событий
        ''' </summary>
        Property EnableEvents As Boolean

        ''' <summary>
        ''' Ссылка на страницу где размещается элемент управления
        ''' </summary>
        Property Parent As IContainer

        ''' <summary>
        ''' Метод должен возвращать тело элемента управления
        ''' </summary>
        Function RenderHtml() As String

        ''' <summary>
        ''' Метод может возвращать дополнительный скрипт для инициализации контрола или обработки элемента управления
        ''' </summary>
        Function RenderScript() As String

        ''' <summary>
        ''' Метод вызывается фреймворком и передаёт имя события и опциональное значение события
        ''' Элемент управления должен проверить и создать пользовательское событие и вернуть успех
        ''' </summary>
        Sub ProcessControlEvent(EventName As String, EventArgument As String)

        ''' <summary>
        ''' Метод вызывается фреймворком и передаётся значение формы, чтобы элемент управления мог себя инициализировать этим значением. Должен вернуть успех, если выполнено
        ''' </summary>
        Sub ProcessFormData(Value As String)

        ''' <summary>
        ''' Метод вызывается фреймворком и передаётся объект файла из формы, если происходит загрузка файла
        ''' </summary>
        Sub ProcessFile(Files As IEnumerable(Of IFormFile), cancellationToken As CancellationToken)

    End Interface

End Namespace