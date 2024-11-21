Public Class WebPagesOptions

    Public Property StateProvider As IStateSerializer

    Public Property TemplateProvider As ITemplateProvider

    Public Property MappedPages As Dictionary(Of String, Type)

End Class
