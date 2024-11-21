Public Class StateWrongDataException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(Message As String)
        MyBase.New(Message)
    End Sub

    Public Sub New(Message As String, InnerException As Exception)
        MyBase.New(Message, InnerException)
    End Sub

End Class
