Imports System.Runtime.CompilerServices
Imports Microsoft.AspNetCore.Builder

Public Module WebPagesMiddlewareExtensions

    ' Список смапленных адресов на страницы 
    Private ReadOnly mappedPages As New Dictionary(Of String, Type)

    <Extension>
    Public Function UseWebPages(app As IApplicationBuilder) As IApplicationBuilder
        Dim options As New WebPagesOptions() With {.MappedPages = New Dictionary(Of String, Type)(mappedPages)}
        Return app.UseMiddleware(Of WebPagesMiddleware)(options)
    End Function

    <Extension>
    Public Function UseWebPages(app As IApplicationBuilder, options As WebPagesOptions) As IApplicationBuilder
        options.MappedPages = If(options.MappedPages, mappedPages)
        Return app.UseMiddleware(Of WebPagesMiddleware)(options)
    End Function

    <Extension>
    Public Function MapWebPage(Of TPage As {IPage, New})(app As IApplicationBuilder, Url As String) As IApplicationBuilder
        mappedPages(Url) = GetType(TPage)
        Return app
    End Function

End Module