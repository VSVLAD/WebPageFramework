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

            ' Значения по-умолчанию
            Me.Text = String.Empty
            Me.MultiLine = False
            Me.Rows = 0
        End Sub

        Public Event TextChanged As HtmlControlEventHandler

        Public Property Text As String
        Public Property MultiLine As Boolean
        Public Property Rows As Integer

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
                    strBuffer.Append($" onchange=""javascript:setTimeout(wpPostBack('{HttpUtility.HtmlAttributeEncode(Id)}','TextChanged', ''), 0)""")
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
                    strBuffer.Append($" onchange=""javascript:setTimeout(wpPostBack('{HttpUtility.HtmlAttributeEncode(Id)}','TextChanged', ''), 0)""")
                End If

                strBuffer.Append(" />")
            End If

            Return strBuffer.ToString()
        End Function

        Public Overrides Function RenderScript() As String
            Return String.Empty
        End Function

        Public Overrides Sub ProcessEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "TextChanged" Then
                RaiseEvent TextChanged(Me, New HtmlControlEventArgs(EventArgument))
            End If
        End Sub

        Public Overrides Sub ProcessFormData(Value As String)
            Me.Text = Value
        End Sub

        Public Overrides Sub FromState(State As StateObject)
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Text)) Then Text = State(NameOf(Text))
                If State.ContainsKey(NameOf(MultiLine)) Then MultiLine = State(NameOf(MultiLine))
                If State.ContainsKey(NameOf(Rows)) Then Rows = State(NameOf(Rows))
            End If
        End Sub

        Public Overrides Function ToState() As StateObject
            Dim state = MyBase.ToState()

            If EnableState Then
                If Not String.IsNullOrEmpty(Text) Then state(NameOf(Text)) = Text
                If MultiLine Then state(NameOf(MultiLine)) = MultiLine
                If Rows > 0 Then state(NameOf(Rows)) = Rows
            End If

            Return state
        End Function

    End Class

End Namespace