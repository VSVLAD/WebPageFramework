Imports System.Text
Imports System.Web

Namespace Controls

    ''' <summary>
    ''' Текстовое поле
    ''' </summary>
    Public Class TextBox
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)
        End Sub

        Public Event TextChanged As Action(Of TextBox, String)

        Public Property Text As String = ""

        Public Property MultiLine As Boolean = False

        Public Property Rows As Integer = 0

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty

            Dim strBuffer As New StringBuilder()

            If MultiLine Then
                strBuffer.Append($"<textarea id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
                strBuffer.Append($" name=""{HttpUtility.HtmlAttributeEncode(Id)}""")

                If Not Enabled Then strBuffer.Append($" disabled=""disabled""")

                If Not String.IsNullOrEmpty(CSS) Then
                    strBuffer.Append($" class=""{If(Not Enabled, "disabled ", "")}{HttpUtility.HtmlAttributeEncode(CSS)}""")
                Else
                    If Not Enabled Then strBuffer.Append($" class=""disabled""")
                End If

                For Each attr In Attributes
                    strBuffer.Append($" {HttpUtility.HtmlAttributeEncode(attr.Key)}=""{HttpUtility.HtmlAttributeEncode(attr.Value)}""")
                Next

                If Rows > 0 Then
                    strBuffer.Append($" rows=""{HttpUtility.HtmlAttributeEncode(Rows)}""")
                End If

                If EnableEvents Then
                    strBuffer.Append($" onchange=""javascript:setTimeout(doPostBack('{HttpUtility.HtmlAttributeEncode(Id)}','TextChanged', ''), 0)""")
                End If

                strBuffer.Append($">{HttpUtility.HtmlEncode(Text)}")
                strBuffer.Append("</textarea>")
            Else
                strBuffer.Append($"<input type=""text"" id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
                strBuffer.Append($" name=""{HttpUtility.HtmlAttributeEncode(Id)}""")

                If Not Enabled Then strBuffer.Append($" disabled=""disabled""")

                If Not String.IsNullOrEmpty(CSS) Then
                    strBuffer.Append($" class=""{If(Not Enabled, "disabled ", "")}{HttpUtility.HtmlAttributeEncode(CSS)}""")
                Else
                    If Not Enabled Then strBuffer.Append($" class=""disabled""")
                End If

                For Each attr In Attributes
                    strBuffer.Append($" {HttpUtility.HtmlAttributeEncode(attr.Key)}=""{HttpUtility.HtmlAttributeEncode(attr.Value)}""")
                Next

                strBuffer.Append($" value=""{HttpUtility.HtmlAttributeEncode(Text)}""")

                If EnableEvents Then
                    strBuffer.Append($" onchange=""javascript:setTimeout(doPostBack('{HttpUtility.HtmlAttributeEncode(Id)}','TextChanged', ''), 0)""")
                End If

                strBuffer.Append(" />")
            End If

            Return strBuffer.ToString()
        End Function

        Public Overrides Sub ProcessEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "TextChanged" Then RaiseEvent TextChanged(Me, EventArgument)
            MyBase.ProcessEvent(EventName, EventArgument)
        End Sub

        Public Overrides Sub ProcessFormData(Value As String)
            Text = Value
            MyBase.ProcessFormData(Value)
        End Sub

        Public Overrides Sub FromState(State As Dictionary(Of String, Object))
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Text)) Then Text = State(NameOf(Text))
                If State.ContainsKey(NameOf(MultiLine)) Then MultiLine = State(NameOf(MultiLine))
                If State.ContainsKey(NameOf(Rows)) Then Rows = State(NameOf(Rows))
            End If
        End Sub

        Public Overrides Function ToState() As Dictionary(Of String, Object)
            Dim state = MyBase.ToState()

            If EnableState Then
                state(NameOf(Text)) = Text
                state(NameOf(MultiLine)) = MultiLine
                state(NameOf(Rows)) = Rows
            End If

            Return state
        End Function

    End Class

End Namespace