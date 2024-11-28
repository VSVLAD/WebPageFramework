Imports WebPages.Controls

Public Interface IContainer

    ''' <summary>
    ''' Список элементов управления содержащихся в контейнере
    ''' </summary>
    Property Controls As Dictionary(Of String, IHtmlControl)

    ''' <summary>
    ''' Объект состояния прикрепленный к контейнеру
    ''' </summary>
    Property ViewState As ViewObject

End Interface
