Option Strict On

Imports System.Text
Imports System.Web
Imports WebPages.Controls

Public Class ButtonBS
    Inherits Button

    Public Property ButtonColor As Colors

    Public Sub New(Parent As IContainer, Id As String)
        MyBase.New(Parent, Id)

        ' По-умолчанию
        Me.ButtonColor = Colors.Primary
    End Sub

    Public Overrides Function RenderHtml() As String
        If Not Visible Then Return String.Empty

        Dim strBuffer As New StringBuilder()
        strBuffer.Append($"<input type=""button"" id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
        strBuffer.Append($" name=""{HttpUtility.HtmlAttributeEncode(Id)}""")
        strBuffer.Append($" value=""{HttpUtility.HtmlAttributeEncode(Text)}""")

        If Not Enabled Then strBuffer.Append($" disabled=""disabled""")

        ' Добавляем классы
        strBuffer.Append($" class=""btn btn-{ButtonColor.Value()}")

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

        If EnableEvents Then strBuffer.Append($" onclick=""wpPostBack('{Id}','Click', '')""")

        strBuffer.Append(" />")

        Return strBuffer.ToString()
    End Function

    Public Overrides Sub FromState(State As StateObject)
        If State.ContainsKey(NameOf(EnableState)) Then EnableState = CBool(State(NameOf(EnableState)))

        If EnableState Then
            If State.ContainsKey(NameOf(Text)) Then Text = CStr(State(NameOf(Text)))
            If State.ContainsKey(NameOf(ButtonColor)) Then ButtonColor = DirectCast(State(NameOf(ButtonColor)), Colors)
            If State.ContainsKey(NameOf(Visible)) Then Visible = CBool(State(NameOf(Visible)))
            If State.ContainsKey(NameOf(Enabled)) Then Enabled = CBool(State(NameOf(Enabled)))
            If State.ContainsKey(NameOf(EnableEvents)) Then EnableEvents = CBool(State(NameOf(EnableEvents)))
            If State.ContainsKey(NameOf(CSS)) Then CSS = CStr(State(NameOf(CSS)))
            If State.ContainsKey(NameOf(Attributes)) Then Attributes = DirectCast(State(NameOf(Attributes)), Dictionary(Of String, String))
        End If
    End Sub

    Public Overrides Function ToState() As StateObject
        Dim state As New StateObject()

        ' Принудительно добавляем свойство в состояние
        state(NameOf(EnableState)) = CStr(EnableState)

        If EnableState Then

            ' Только те свойства, значения которых изменились от умолчания
            If Not String.IsNullOrEmpty(Text) Then state(NameOf(Text)) = CStr(Text)
            If ButtonColor <> Colors.Primary Then state(NameOf(ButtonColor)) = ButtonColor
            If Not Visible Then state(NameOf(Visible)) = CBool(Visible)
            If Not Enabled Then state(NameOf(Enabled)) = CBool(Enabled)
            If Not EnableEvents Then state(NameOf(EnableEvents)) = CBool(EnableEvents)
            If Not String.IsNullOrEmpty(CSS) Then state(NameOf(CSS)) = CSS
            If Attributes.Count > 0 Then state(NameOf(Attributes)) = Attributes
        End If

        Return state
    End Function

End Class
