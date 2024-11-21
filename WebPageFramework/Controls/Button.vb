﻿Imports System.Text
Imports System.Web

Namespace Controls

    ''' <summary>
    ''' Обычная кнопка
    ''' </summary>
    Public Class Button
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)
        End Sub

        Public Event Click As Action(Of Button, String)

        Public Property Text As String = ""

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

            If EnableEvents Then strBuffer.Append($" onclick=""doPostBack('{Id}','Click', '')""")

            strBuffer.Append(" />")

            Return strBuffer.ToString()
        End Function

        Public Overrides Sub ProcessEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "Click" Then RaiseEvent Click(Me, EventArgument)
            MyBase.ProcessEvent(EventName, EventArgument)
        End Sub

        Public Overrides Sub FromState(State As Dictionary(Of String, Object))
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(Text)) Then Text = State(NameOf(Text))
            End If
        End Sub

        Public Overrides Function ToState() As Dictionary(Of String, Object)
            Dim state = MyBase.ToState()
            state(NameOf(Text)) = Text
            Return state
        End Function

    End Class

End Namespace