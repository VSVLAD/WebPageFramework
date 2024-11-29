Option Strict On

Friend Class PageStateProvider
    Implements IStateProvider

    Private Const FieldNameViewState = "wpViewState"

    ''' <summary>
    ''' Записать состояние в хранилище
    ''' </summary>
    Public Sub ToStorage(TreeState As String, Page As IPage) Implements IStateProvider.ToStorage
        Page.ViewData("__formState") = $"<input type=""hidden"" name=""{FieldNameViewState}"" value=""{TreeState}"" />"
    End Sub

    ''' <summary>
    ''' Прочитать состояние из хранилища
    ''' </summary>
    Public Function FromStorage(Page As IPage) As String Implements IStateProvider.FromStorage
        If Page.Form IsNot Nothing Then
            Return Page.Form(FieldNameViewState)
        Else
            Return Nothing
        End If
    End Function

End Class
