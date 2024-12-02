Imports System.Text

Public Class Template

    Private sbContent As StringBuilder

    Public Sub New(Content As String)
        sbContent = New StringBuilder(Content)
    End Sub

    Public Property Length As Integer
        Get
            Return sbContent.Length
        End Get
        Set(value As Integer)
            sbContent.Length = value
        End Set
    End Property

    Public Function Render() As String
        Return sbContent.ToString()
    End Function

    Public Sub Replace(Key As String, Value As String)
        sbContent.Replace(Key, Value)
    End Sub

    Public Sub Append(Value As String)
        sbContent.Append(Value)
    End Sub

End Class
