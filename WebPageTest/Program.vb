Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports WebPages
Imports System.Text

Public Module Program

    Public Sub Main(args() As String)
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)

        Dim builder = WebApplication.CreateBuilder(args)
        builder.Services.AddDistributedMemoryCache()
        builder.Services.AddSession()
        builder.Services.AddResponseCompression(Sub(comperessOptions)
                                                    comperessOptions.EnableForHttps = True
                                                End Sub)
        Dim app = builder.Build()
        app.UseSession()
        app.UseResponseCompression()

        ' ������� ������ ������
        app.MapWebPage(Of IndexPage)("/")
        app.MapWebPage(Of IndexPage)("/IndexPage")
        app.MapWebPage(Of IndexPage)("/IndexPage.htm")

        ' ���������� ���-��������
        app.UseWebPages()
        app.UseStaticFiles()

        app.Run()
    End Sub

End Module