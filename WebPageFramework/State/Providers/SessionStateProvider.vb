Option Strict On

Imports Microsoft.AspNetCore.Http.SessionExtensions

Public Class SessionStateProvider
    Implements IStateProvider

    Private Const FieldNameSessionState = "wpSession"

    ''' <summary>
    ''' Записать состояние в хранилище
    ''' </summary>
    Public Sub ToStorage(TreeState As String, Page As IPage) Implements IStateProvider.ToStorage
        If Page.Context.Session.IsAvailable Then

            ' Получаем из формы уникальный ID страницы или генерируем новый
            Dim pageUniqId = If(Page.Form IsNot Nothing AndAlso Page.Form.ContainsKey(FieldNameSessionState), Page.Form(FieldNameSessionState).ToString(), Guid.NewGuid().ToString())

            ' Сохраняем в сессии упакованный объект состояния с учётом уникального ID
            Dim sessionKey = $"{FieldNameSessionState}:{Page.Context.Session.Id}:{Page.Id}:{pageUniqId}"
            Page.Context.Session.SetString(sessionKey, TreeState)

            ' Сохраняем в поле страницы уникальный ID страницы
            Page.ViewData("__formState") = $"<input type=""hidden"" name=""{FieldNameSessionState}"" value=""{pageUniqId}"" />"
        End If
    End Sub

    ''' <summary>
    ''' Прочитать состояние из хранилища
    ''' </summary>
    Public Function FromStorage(Page As IPage) As String Implements IStateProvider.FromStorage
        If Page.Context.Session.IsAvailable Then

            ' Получаем из формы уникальный ID страницы
            If Page.Form IsNot Nothing AndAlso Page.Form.ContainsKey(FieldNameSessionState) Then

                Dim pageUniqId = Page.Form(FieldNameSessionState).ToString()
                Dim sessionKey = $"{FieldNameSessionState}:{Page.Context.Session.Id}:{Page.Id}:{pageUniqId}"

                ' Если такая сессия была сохранена ранее
                If Page.Context.Session.Keys.Contains(sessionKey) Then
                    Return Page.Context.Session.GetString(sessionKey)
                End If
            End If
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Для принудительной очистки состояния из сессии
    ''' </summary>
    ''' <param name="Page"></param>
    Public Shared Sub ClearStorage(Page As IPage)
        If Page.Context.Session.IsAvailable Then
            Dim sessionKey = $"{FieldNameSessionState}-{Page.Context.Session.Id}-{Page.Id}"
            Page.Context.Session.Remove(sessionKey)
        End If
    End Sub

End Class
