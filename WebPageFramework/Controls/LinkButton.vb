Imports System.Text
Imports System.Web

Namespace Controls

    ''' <summary>
    ''' Кнопка в виде ссылки
    ''' </summary>
    Public Class LinkButton
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)
        End Sub

        Public Event Click As Action(Of LinkButton, String)

        Public Property HRef As String = ""

        Public Property Html As String = ""

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty

            Dim strBuffer As New StringBuilder()
            strBuffer.Append($"<a id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
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

            If EnableEvents Then
                If Not String.IsNullOrEmpty(HRef) Then
                    strBuffer.Append($" href=""{HttpUtility.HtmlAttributeEncode(HRef)}""")
                Else
                    strBuffer.Append($" onclick=""doPostBack('{Id}','Click', '')""")
                End If
            End If

            strBuffer.Append($">{Html}</a>")

            Return strBuffer.ToString()
        End Function

        Public Overrides Sub ProcessEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "Click" Then RaiseEvent Click(Me, EventArgument)
            MyBase.ProcessEvent(EventName, EventArgument)
        End Sub

        Public Overrides Sub FromState(State As Dictionary(Of String, Object))
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(HRef)) Then HRef = State(NameOf(HRef))
                If State.ContainsKey(NameOf(Html)) Then Html = State(NameOf(Html))
            End If
        End Sub

        Public Overrides Function ToState() As Dictionary(Of String, Object)
            Dim state = MyBase.ToState()

            If EnableState Then
                state(NameOf(HRef)) = HRef
                state(NameOf(Html)) = Html
            End If

            Return state
        End Function

    End Class

End Namespace