Option Strict On

Imports System.Text
Imports System.Web
Imports WebPages.Controls

Public Class BSButton
    Inherits Button

    Public Property Color As BSColors = BSColors.Primary

    Public Sub New(Parent As IContainer, Id As String)
        MyBase.New(Parent, Id)
    End Sub

    Public Overrides Function RenderHtml() As String
        If Not Visible Then Return String.Empty

        Dim strBuffer As New StringBuilder()
        strBuffer.Append($"<input type=""button"" id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
        strBuffer.Append($" name=""{HttpUtility.HtmlAttributeEncode(Id)}""")
        strBuffer.Append($" value=""{HttpUtility.HtmlAttributeEncode(Text)}""")

        If Not Enabled Then strBuffer.Append($" disabled=""disabled""")

        ' Добавляем классы
        strBuffer.Append($" class=""btn btn-{Color.Value()}")

        If Not String.IsNullOrEmpty(CSS) Then
            strBuffer.Append($" {HttpUtility.HtmlAttributeEncode(CSS)}""")
        Else
            strBuffer.Append($"""")
        End If

        If Not Enabled Then strBuffer.Append($" ""disabled""")

        ' Добавляем атрибуты
        For Each attr In Attributes
            strBuffer.Append($" {HttpUtility.HtmlAttributeEncode(attr.Key)}=""{HttpUtility.HtmlAttributeEncode(attr.Value)}""")
        Next

        If EnableEvents Then strBuffer.Append($" onclick=""{FunctionNamePostBack}('{HttpUtility.HtmlAttributeEncode(Id)}','Click', '')""")

        strBuffer.Append(" />")

        Return strBuffer.ToString()
    End Function

    Public Overrides Sub FromState(State As ViewObject)
        If EnableState Then
            If State.ContainsKey(NameOf(Color)) Then Color = DirectCast(State(NameOf(Color)), BSColors)
        End If

        MyBase.FromState(State)
    End Sub

    Public Overrides Function ToState() As ViewObject
        Dim state = MyBase.ToState()

        If EnableState Then

            ' Только те свойства, значения которых изменились от умолчания
            If Color <> BSColors.Primary Then state(NameOf(Color)) = Color
        End If

        Return state
    End Function

End Class
