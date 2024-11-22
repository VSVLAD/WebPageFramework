Imports System.Web

Public Class WebPagesHelper

    ' Создать объект состояния
    Public Shared Function GenerateState(Page As IPage, StateProvider As IStateSerializer) As String
        Dim treeState As New Dictionary(Of String, Object)

        ' Добавляем состояние формы
        treeState(Page.Id) = Page.ToState()

        ' Добавляем все контролы в состояние
        For Each ctlKey In Page.Controls.Keys
            Dim state = Page.Controls(ctlKey).ToState()

            If state IsNot Nothing Then
                treeState(ctlKey) = Page.Controls(ctlKey).ToState()
            End If
        Next

        Return StateProvider.SaveState(treeState)
    End Function

    ' Загружаем объект состояния и применяем к элементам управления
    Public Shared Sub ApplyState(Page As IPage, StateProvider As IStateSerializer)
        Dim viewState = Page.Form("viewState")

        If Not String.IsNullOrEmpty(viewState) Then
            Dim treeState = StateProvider.LoadState(viewState)

            ' Применяем состояние к форме
            If treeState.ContainsKey(Page.Id) Then
                Page.FromState(treeState(Page.Id))
            End If

            ' Перебираем все контролы и применяем состояние, если оно содержится в объекте
            For Each ctl In Page.Controls
                If treeState.ContainsKey(ctl.Key) Then
                    ctl.Value.FromState(treeState(ctl.Key))
                End If
            Next
        End If
    End Sub

    ' Применить значение из формы к элементу управления
    Public Shared Sub ApplyControlFormValue(Page As IPage)
        For Each ctl In Page.Controls
            If Page.Form.ContainsKey(ctl.Key) Then
                ctl.Value.ProcessFormData(Page.Form(ctl.Key))
            End If
        Next
    End Sub

    ' Создать пользовательские событие
    Public Shared Sub GenerateControlEvents(Page As IPage)
        Dim eventControl = Page.Form("eventControl")
        Dim eventName = Page.Form("eventName")
        Dim eventArgument = Page.Form("eventArgument")

        ' Если было событие от элемента управления и такой элемент управления существует
        If Not String.IsNullOrEmpty(eventControl) AndAlso Page.Controls.ContainsKey(eventControl) Then
            Page.Controls(eventControl).ProcessEvent(eventName, eventArgument)
        Else
            Throw New Exception($"Элемент управления ""{eventControl}"" создал событие, но он не зарегистрирован в веб-форме")
        End If
    End Sub


    ' Для создания заменителей тега формы
    Public Shared Sub GenerateBeginEndViewForm(Page As IPage)
        Page.ViewData("__formBegin") = $"<form name=""{HttpUtility.HtmlAttributeEncode(Page.Id)}"" action=""{Page.Context.Request.Path}"" method=""post"">
<input type=""hidden"" name=""eventControl"" value="""" />
<input type=""hidden"" name=""eventName"" value="""" />
<input type=""hidden"" name=""eventArgument"" value="""" />
<script type=""text/javascript"">
    function doPostBack(eventControl, eventName, eventArgument) {{
        let form = document.forms[""{HttpUtility.HtmlAttributeEncode(Page.Id)}""];
        if (!form.onsubmit || (form.onsubmit() != false)) {{
            form.eventControl.value = eventControl;
            form.eventName.value = eventName;
            form.eventArgument.value = eventArgument;
            form.submit();
        }}
    }}
</script>
"
        Page.ViewData("__formEnd") = "</form>"
    End Sub

    ' Создаёт заполнитель для объекта состояния
    Public Shared Sub GenerateStateViewForm(Page As IPage, PackedState As String)
        Page.ViewData("__formViewState") = $"<input type=""hidden"" name=""viewState"" value=""{PackedState}"" />"
    End Sub

End Class
