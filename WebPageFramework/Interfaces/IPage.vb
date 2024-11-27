Imports System.Threading
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http
Imports WebPages.Controls

Public Interface IPage
    Inherits IControl, IContainer, IViewData, IState

    ''' <summary>
    ''' Доступ к контексту веб-запроса
    ''' </summary>
    Property Context As HttpContext

    ''' <summary>
    ''' Доступ к веб-окружению
    ''' </summary>
    Property Environment As IWebHostEnvironment

    ''' <summary>
    ''' Доступ к опциям фреймворка
    ''' </summary>
    Property Options As WebPagesOptions

    ''' <summary>
    ''' Получить доступ к значениям принятым в форме
    ''' </summary>
    Property Form As IFormCollection

    ''' <summary>
    ''' Фреймворк вызывает, чтобы страница загрузила состояние, обработала данные формы, сохранила состояние
    ''' </summary>
    Function ProcessAsync(tokenCancel As CancellationToken) As Task

    ''' <summary>
    ''' Фреймворк вызывает, чтобы отрисовать контент
    ''' </summary>
    Function RenderAsync(tokenCancel As CancellationToken) As Task(Of String)

End Interface
