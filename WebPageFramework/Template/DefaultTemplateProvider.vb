Imports System.IO

Public Class DefaultTemplateProvider
    Implements ITemplateProvider

    Private webRootPath As String

    Public Sub New(WebRootPath As String)
        Me.webRootPath = WebRootPath
    End Sub

    Public Function GetTemplate(Name As String) As Template Implements ITemplateProvider.GetTemplate

        ' Преобразуем URL в путь к полному пути файла
        Dim filePath = $"{Path.Combine(webRootPath, Name)}.htm"

        If File.Exists(filePath) Then
            Return New Template(File.ReadAllText(filePath))
        Else
            Throw New TemplateNotFoundException($"Файл-шаблон ""{filePath}"" не найден")
        End If

    End Function

End Class
