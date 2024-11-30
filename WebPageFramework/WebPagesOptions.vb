Option Strict On

Public Class WebPagesOptions

    Public Property StateFormatter As IStateFormatter

    Public Property StateProvider As IStateProvider

    Public Property TemplateProvider As ITemplateProvider

    Public Property MappedPages As Dictionary(Of String, Type)

    Public Function Clone() As WebPagesOptions
        Return New WebPagesOptions() With {
                        .StateFormatter = Me.StateFormatter,
                        .StateProvider = Me.StateProvider,
                        .TemplateProvider = Me.TemplateProvider,
                        .MappedPages = New Dictionary(Of String, Type)(Me.MappedPages)
                    }
    End Function

End Class
