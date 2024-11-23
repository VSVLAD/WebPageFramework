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

            ' Значения по-умолчанию
            Me.HRef = String.Empty
            Me.Text = String.Empty
        End Sub

        Public Event Click As Action(Of LinkButton, String)

        Public Property HRef As String

        ''' <summary>
        ''' Свойство поддерживает HTML-разметку
        ''' </summary>
        Public Property Text As String

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
                    strBuffer.Append($" onclick=""wpPostBack('{Id}','Click', '')""")
                End If
            End If

            strBuffer.Append($">{Text}</a>")

            Return strBuffer.ToString()
        End Function

        Public Overrides Function RenderScript() As String
            Return String.Empty
        End Function

        Public Overrides Sub ProcessFormData(Value As String)
        End Sub

        Public Overrides Sub ProcessEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "Click" Then
                RaiseEvent Click(Me, EventArgument)
            End If
        End Sub

        Public Overrides Sub FromState(State As StateObject)
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(HRef)) Then HRef = State(NameOf(HRef))
                If State.ContainsKey(NameOf(Text)) Then Text = State(NameOf(Text))
            End If
        End Sub

        Public Overrides Function ToState() As StateObject
            Dim state = MyBase.ToState()

            If EnableState Then
                If Not String.IsNullOrEmpty(HRef) Then state(NameOf(HRef)) = HRef
                If Not String.IsNullOrEmpty(Text) Then state(NameOf(Text)) = Text
            End If

            Return state
        End Function

    End Class

End Namespace