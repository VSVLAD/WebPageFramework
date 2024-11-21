Imports System.Reflection
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http

Public Class WebPageFactory

    Public Shared Function Create(Id As String, Context As HttpContext, Environment As IWebHostEnvironment, Options As WebPagesOptions) As IPage

        ' Попробуем найти соответствующий Code-Behind класс
        Dim pageType = Assembly.GetEntryAssembly().GetType(Id)

        If pageType Is Nothing Then
            Throw New WebPageNotFoundException($"Не найден класс формы ""{Id}""")
        End If

        Return Create(pageType, Context, Environment, Options)
    End Function

    Public Shared Function Create(PageType As Type, Context As HttpContext, Environment As IWebHostEnvironment, Options As WebPagesOptions) As IPage
        If Not GetType(IPage).IsAssignableFrom(PageType) Then
            Throw New ArgumentException($"Класс формы ""{PageType.Name}"" должен реализовывать интерфейс IPage")
        End If

        ' Создаем экземпляр класса для Code-Behind
        Dim pageInstance = CType(Activator.CreateInstance(PageType), IPage)

        ' Заполняем основные объекты формы
        pageInstance.Id = pageInstance.GetType().Name
        pageInstance.Context = Context
        pageInstance.Environment = Environment
        pageInstance.Options = Options

        Return pageInstance
    End Function

End Class
