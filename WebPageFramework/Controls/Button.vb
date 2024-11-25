Option Strict On

Imports System.Text
Imports System.Web

Namespace Controls

    ''' <summary>
    ''' Обычная кнопка
    ''' </summary>
    Public Class Button
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)

            ' Значения по-умолчанию
            Me.Text = String.Empty
        End Sub

        Public Event Click As HtmlControlEventHandler

        Public Property Text As String

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty

            Dim strBuffer As New StringBuilder()
            strBuffer.Append($"<input type=""button"" id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
            strBuffer.Append($" name=""{HttpUtility.HtmlAttributeEncode(Id)}""")
            strBuffer.Append($" value=""{HttpUtility.HtmlAttributeEncode(Text)}""")

            If Not Enabled Then strBuffer.Append($" disabled=""disabled""")

            If Not String.IsNullOrEmpty(CSS) Then
                strBuffer.Append($" class=""{If(Not Enabled, "disabled ", "")}{HttpUtility.HtmlAttributeEncode(CSS)}""")
            Else
                If Not Enabled Then strBuffer.Append($" class=""disabled""")
            End If

            For Each attr In Attributes
                strBuffer.Append($" {HttpUtility.HtmlAttributeEncode(attr.Key)}=""{HttpUtility.HtmlAttributeEncode(attr.Value)}""")
            Next

            If EnableEvents Then strBuffer.Append($" onclick=""wpPostBack('{Id}','Click', '')""")

            strBuffer.Append(" />")

            Return strBuffer.ToString()
        End Function

        Public Overrides Function RenderScript() As String
            Return String.Empty
        End Function

        Public Overrides Sub ProcessControlEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "Click" Then
                RaiseEvent Click(Me, New HtmlControlEventArgs(EventArgument))
            End If
        End Sub

        Public Overrides Sub ProcessFormData(Value As String)
        End Sub

        Public Overrides Sub FromState(State As StateObject)
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Text)) Then Text = CStr(State(NameOf(Text)))
            End If
        End Sub

        Public Overrides Function ToState() As StateObject
            Dim state = MyBase.ToState()

            If EnableEvents Then
                If Not String.IsNullOrEmpty(Text) Then state(NameOf(Text)) = Text
            End If

            Return state
        End Function

    End Class

End Namespace