Imports System.IO
Imports System.Text
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.AspNetCore.Http

Public Class WebPagesMiddleware

    Private ReadOnly nextDelegate As RequestDelegate
    Private ReadOnly env As IWebHostEnvironment
    Private ReadOnly options As WebPagesOptions

    Public Sub New(nextDelegate As RequestDelegate, env As IWebHostEnvironment)
        Me.New(nextDelegate, env, Nothing)
    End Sub

    Public Sub New(nextDelegate As RequestDelegate, env As IWebHostEnvironment, options As WebPagesOptions)
        Me.nextDelegate = nextDelegate
        Me.env = env
        Me.options = If(options, New WebPagesOptions() With {
                                        .StateProvider = New DefaultStateProvider("Default", "Default"),
                                        .TemplateProvider = New DefaultTemplateProvider(env.WebRootPath)
                                    })
    End Sub

    Public Async Function InvokeAsync(context As HttpContext) As Task

        ' Выполняется работа с формой, если в роутинге адрес помечен для работы как с веб-формой
        Dim pageType As Type = Nothing

        If context.Items.TryGetValue("WebPageType", pageType) Then

            ' Создаем экземпляр формы
            Dim pageInstance = WebPageFactory.Create(pageType, context, env, options)

            ' Выполняем обработку формы и отрисовываем форму, заменяем плейсхолдеры {{ item }} в шаблоне
            Dim content = Await pageInstance.ProcessAsync()

            ' Возвращаем контент
            Await context.Response.WriteAsync(content, Encoding.UTF8)
        Else
            Await nextDelegate(context)
        End If
    End Function

End Class
