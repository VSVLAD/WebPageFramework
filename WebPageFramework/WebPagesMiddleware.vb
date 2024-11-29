Option Strict On

Imports System.IO
Imports System.Text
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http

Public Class WebPagesMiddleware

    Private ReadOnly nextDelegate As RequestDelegate
    Private ReadOnly env As IWebHostEnvironment
    Private ReadOnly options As WebPagesOptions

    Public Sub New(nextDelegate As RequestDelegate, env As IWebHostEnvironment, options As WebPagesOptions)
        Me.nextDelegate = nextDelegate
        Me.env = env
        Me.options = If(options, New WebPagesOptions())
        Me.options.StateFormatter = If(Me.options.StateFormatter, New DefaultStateFormatter("Default", "Default", True, True))
        Me.options.StateProvider = If(Me.options.StateProvider, New PageStateProvider())
        Me.options.TemplateProvider = If(Me.options.TemplateProvider, New DefaultTemplateProvider(env.WebRootPath))
        Me.options.MappedPages = If(Me.options.MappedPages, New Dictionary(Of String, Type))
    End Sub

    Public Async Function InvokeAsync(context As HttpContext) As Task
        Try
            Dim pageType As Type = Nothing

            ' Выполняется запрашиваемый адрес ассоциирован с формой, то выполняем обработку
            If options.MappedPages.TryGetValue(context.Request.Path, pageType) Then

                ' Создаем экземпляр формы
                Dim pageInstance = WebPageFactory.Create(pageType, context, env, options)

                ' Выполняем обработку формы 
                Await pageInstance.ProcessAsync(context.RequestAborted)

                ' Выполняем отрисовку формы, заменяем плейсхолдеры {{ item }} в шаблоне
                Dim content = Await pageInstance.RenderAsync(context.RequestAborted)

                ' Возвращаем контент
                Await context.Response.WriteAsync(content, Encoding.UTF8)
            Else
                Await nextDelegate(context)
            End If

        Catch ex As OperationCanceledException
            Console.WriteLine($"{context.Request.Path} is canceled by user")

        End Try
    End Function

End Class
