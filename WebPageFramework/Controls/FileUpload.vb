Option Strict On

Imports System.Text
Imports System.Threading
Imports System.Web
Imports Microsoft.AspNetCore.Http

Namespace Controls

    ''' <summary>
    ''' Загрузка файлов
    ''' </summary>
    Public Class FileUpload
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)

            ' Переопределим тип формы
            FormEncType = "multipart/form-data"

        End Sub

        Public Event FileReceived As EventHandler(Of FileUploadEventArgs)

        Public Property Accept As String = "*/*"

        Public Property Multiple As Boolean = False

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty

            Dim strBuffer As New StringBuilder()
            strBuffer.Append($"<input type=""file"" id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
            strBuffer.Append($" name=""{HttpUtility.HtmlAttributeEncode(Id)}""")
            strBuffer.Append($" accept=""{HttpUtility.HtmlAttributeEncode(Accept)}""")

            If Multiple Then strBuffer.Append($" multiple")

            If Not String.IsNullOrEmpty(CSS) Then
                strBuffer.Append($" class=""{If(Not Enabled, "disabled ", "")}{HttpUtility.HtmlAttributeEncode(CSS)}""")
            Else
                If Not Enabled Then strBuffer.Append($" class=""disabled""")
            End If

            For Each attr In Attributes
                strBuffer.Append($" {HttpUtility.HtmlAttributeEncode(attr.Key)}=""{HttpUtility.HtmlAttributeEncode(attr.Value)}""")
            Next

            strBuffer.Append(" />")

            Return strBuffer.ToString()
        End Function

        Public Overrides Sub ProcessFile(Files As IEnumerable(Of IFormFile), TokenCancel As CancellationToken)
            For Each file In Files
                RaiseEvent FileReceived(Me, New FileUploadEventArgs(file, TokenCancel))
            Next
        End Sub

        Public Overrides Sub FromState(State As ViewObject)
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Accept)) Then Accept = CStr(State(NameOf(Accept)))
                If State.ContainsKey(NameOf(Multiple)) Then Multiple = CBool(State(NameOf(Multiple)))
            End If
        End Sub

        Public Overrides Function ToState() As ViewObject
            Dim state = MyBase.ToState()

            If EnableEvents Then
                If Not String.IsNullOrEmpty(Accept) Then state(NameOf(Accept)) = Accept
                If Multiple Then state(NameOf(Multiple)) = Multiple
            End If

            Return state
        End Function

    End Class

    ''' <summary>
    ''' Для получения доступа к загружаемому файлу
    ''' </summary>
    Public Class FileUploadEventArgs
        Inherits EventArgs

        Public Property File As IFormFile

        Public Property TokenCancel As CancellationToken

        Public Sub New(File As IFormFile, TokenCancel As CancellationToken)
            Me.File = File
            Me.TokenCancel = TokenCancel
        End Sub

    End Class

End Namespace