Imports System.Runtime.CompilerServices
Imports Microsoft.AspNetCore.Builder

Public Module WebPagesMiddlewareExtensions

    <Extension>
    Public Function UseWebPages(app As IApplicationBuilder) As IApplicationBuilder
        Return app.UseMiddleware(Of WebPagesMiddleware)()
    End Function

    <Extension>
    Public Function UseWebPages(app As IApplicationBuilder, options As WebPagesOptions) As IApplicationBuilder
        Return app.UseMiddleware(Of WebPagesMiddleware)(options)
    End Function

    <Extension>
    Public Sub MapWebPage(Of TPage As IPage)(app As IApplicationBuilder, Url As String)
        app.Use(Function(context, nextMiddleware)

                    ' Проверяем, что маршрут ещё не был обработан
                    If Not context.Items.ContainsKey("WebPageMatched") AndAlso context.Request.Path.Equals(Url, StringComparison.OrdinalIgnoreCase) Then

                        ' Устанавливаем имя страницы и флаг, что маршрут проверен и идём дальше, пока не достигнем WebPagesMiddleware
                        context.Items("WebPageType") = GetType(TPage)
                        context.Items("WebPageMatched") = True
                        Return nextMiddleware()
                    End If

                    Return nextMiddleware()
                End Function)
    End Sub

End Module