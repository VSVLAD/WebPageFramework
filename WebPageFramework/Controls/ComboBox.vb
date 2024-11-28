Imports System.Text
Imports System.Web

Namespace Controls

    ''' <summary>
    ''' Элемент раскрывающийся список
    ''' </summary>
    Public Class ComboBox
        Inherits HtmlControl

        Public Sub New(Parent As IContainer, Id As String)
            MyBase.New(Parent, Id)

            ' Значения по-умолчанию
            Me.SelectedItem = Nothing
            Me.SelectedText = String.Empty
            Me.SelectedValue = String.Empty
            Me.Multiple = False
            Me.Items = New List(Of ComboBoxItem)
        End Sub

        Public Event SelectedItemChanged As HtmlControlEventHandler

        Public Property SelectedText As String
        Public Property SelectedValue As String
        Public Property SelectedItem As ComboBoxItem
        Public Property Items As List(Of ComboBoxItem)
        Public Property Multiple As Boolean

        Public Overrides Function RenderHtml() As String
            If Not Visible Then Return String.Empty

            Dim strBuffer As New StringBuilder()

            strBuffer.Append($"<select id=""{HttpUtility.HtmlAttributeEncode(Id)}""")
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
                strBuffer.Append($" onchange=""javascript:setTimeout(wpPostBack('{HttpUtility.HtmlAttributeEncode(Id)}','SelectedItemChanged', ''), 0)""")
            End If

            strBuffer.Append($">{vbCrLf}")

            ' Наполняем элементами
            For Each item In Items
                strBuffer.Append($"<option value=""{HttpUtility.HtmlAttributeEncode(item.Value)}""")

                If item.Value = SelectedValue Or item.Selected Then
                    strBuffer.Append($" selected")
                End If

                If Not item.Enabled Then strBuffer.Append($" disabled")

                strBuffer.Append($">{HttpUtility.HtmlAttributeEncode(item.Text)}</option>{vbCrLf}")
            Next

            strBuffer.Append($"</select>{vbCrLf}")

            Return strBuffer.ToString()
        End Function

        Public Overrides Function RenderScript() As String
            Return String.Empty
        End Function

        Public Overrides Sub ProcessControlEvent(EventName As String, EventArgument As String)
            If EnableEvents AndAlso EventName = "SelectedItemChanged" Then
                RaiseEvent SelectedItemChanged(Me, New HtmlControlEventArgs(EventArgument))
            End If
        End Sub

        Public Overrides Sub ProcessFormData(Value As String)
            Dim selItem = Items.FirstOrDefault(Function(item) item.Value = Value)

            If selItem IsNot Nothing Then
                Me.SelectedItem = selItem
                Me.SelectedValue = selItem.Value
                Me.SelectedText = selItem.Text
            Else
                Me.SelectedValue = Value
                Me.SelectedItem = Nothing
                Me.SelectedText = String.Empty
            End If
        End Sub

        Public Overrides Sub FromState(State As ViewObject)
            MyBase.FromState(State)

            If EnableState Then
                If State.ContainsKey(NameOf(SelectedValue)) Then SelectedValue = CStr(State(NameOf(SelectedValue)))
                If State.ContainsKey(NameOf(Multiple)) Then Multiple = CBool(State(NameOf(Multiple)))
                If State.ContainsKey(NameOf(Items)) Then Items = State(NameOf(Items))
            End If
        End Sub

        Public Overrides Function ToState() As ViewObject
            Dim state = MyBase.ToState()

            If EnableState Then
                If Not String.IsNullOrEmpty(SelectedValue) Then state(NameOf(SelectedValue)) = SelectedValue
                If Multiple Then state(NameOf(Multiple)) = Multiple
                If Items.Count > 0 Then state(NameOf(Items)) = Items
            End If

            Return state
        End Function


    End Class

    ''' <summary>
    ''' Представляет один элемент option для элемента select
    ''' </summary>
    Public Class ComboBoxItem
        Public Property Text As String
        Public Property Value As String
        Public Property Enabled As Boolean
        Public Property Selected As Boolean

        Public Sub New()
            Me.New(String.Empty, String.Empty, True, False)
        End Sub

        Public Sub New(Text As String, Value As String)
            Me.New(Text, Value, True, False)
        End Sub

        Public Sub New(Text As String, Value As String, Enabled As Boolean)
            Me.New(Text, Value, Enabled, False)
        End Sub

        Public Sub New(Text As String, Value As String, Enabled As Boolean, Selected As Boolean)
            Me.Text = Text
            Me.Value = Value
            Me.Enabled = Enabled
            Me.Selected = Selected
        End Sub
    End Class

End Namespace