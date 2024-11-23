Imports System.Runtime.CompilerServices
Imports System.Web

Public Module WebPagesHelper

    ' Создать объект состояния
    <Extension>
    Public Function GenerateState(ThisPage As Page, StateProvider As IStateProvider) As String
        Dim treeState As New StateObject

        ' Добавляем состояние формы
        treeState(ThisPage.Id) = ThisPage.ToState()

        ' Добавляем все контролы в состояние
        For Each ctlKey In ThisPage.Controls.Keys
            Dim state = ThisPage.Controls(ctlKey).ToState()

            If state IsNot Nothing Then
                treeState(ctlKey) = ThisPage.Controls(ctlKey).ToState()
            End If
        Next

        Return StateProvider.SaveState(treeState)
    End Function

    ' Загружаем объект состояния и применяем к элементам управления
    <Extension>
    Public Sub ApplyState(ThisPage As Page, StateProvider As IStateProvider)
        Dim viewState = ThisPage.Form(Page.FieldNameViewState)

        If Not String.IsNullOrEmpty(viewState) Then
            Dim treeState = StateProvider.LoadState(viewState)

            ' Применяем состояние к форме
            If treeState.ContainsKey(ThisPage.Id) Then
                ThisPage.FromState(treeState(ThisPage.Id))
            End If

            ' Перебираем все контролы и применяем состояние, если оно содержится в объекте
            For Each ctl In ThisPage.Controls
                If treeState.ContainsKey(ctl.Key) Then
                    ctl.Value.FromState(treeState(ctl.Key))
                End If
            Next
        End If
    End Sub

    ' Применить значение из формы к элементу управления
    <Extension>
    Public Sub ApplyControlFormValue(ThisPage As Page)
        For Each ctl In ThisPage.Controls
            If ThisPage.Form.ContainsKey(ctl.Key) Then
                ctl.Value.ProcessFormData(ThisPage.Form(ctl.Key))
            End If
        Next
    End Sub

    ' Создать пользовательские событие
    <Extension>
    Public Sub GenerateControlEvents(ThisPage As Page)
        Dim eventControl = ThisPage.Form(Page.FieldNameEventControl)
        Dim eventName = ThisPage.Form(Page.FieldNameEventName)
        Dim eventArgument = ThisPage.Form(Page.FieldNameEventArgument)

        ' Если было событие от элемента управления и такой элемент управления существует
        If Not String.IsNullOrEmpty(eventControl) AndAlso ThisPage.Controls.ContainsKey(eventControl) Then
            ThisPage.Controls(eventControl).ProcessEvent(eventName, eventArgument)
        Else
            Throw New Exception($"Элемент управления ""{eventControl}"" создал событие, но он не зарегистрирован в веб-форме")
        End If
    End Sub

    ' Для создания заменителей тега формы
    <Extension>
    Public Sub GenerateBeginEndViewData(ThisPage As Page)
        ThisPage.ViewData("__formBegin") = $"<form name=""{HttpUtility.HtmlAttributeEncode(ThisPage.Id)}"" action=""{ThisPage.Context.Request.Path}"" method=""post"">
<input type=""hidden"" name=""{Page.FieldNameEventControl}"" value="""" />
<input type=""hidden"" name=""{Page.FieldNameEventName}"" value="""" />
<input type=""hidden"" name=""{Page.FieldNameEventArgument}"" value="""" />
<script type=""text/javascript"">
    function {Page.FunctionNamePostBack}({Page.FieldNameEventControl}, {Page.FieldNameEventName}, {Page.FieldNameEventArgument}) {{
        let form = document.forms[""{HttpUtility.HtmlAttributeEncode(ThisPage.Id)}""];
        if (!form.onsubmit || form.onsubmit()) {{
            form.{Page.FieldNameEventControl}.value = {Page.FieldNameEventControl};
            form.{Page.FieldNameEventName}.value = {Page.FieldNameEventName};
            form.{Page.FieldNameEventArgument}.value = {Page.FieldNameEventArgument};
            form.submit();
        }}
    }}
</script>
"
        ThisPage.ViewData("__formEnd") = "</form>"
    End Sub

    ' Создаёт заполнитель для объекта состояния
    <Extension>
    Public Sub GenerateStateViewData(ThisPage As Page, State As String)
        ThisPage.ViewData("__formViewState") = $"<input type=""hidden"" name=""{Page.FieldNameViewState}"" value=""{State}"" />"
    End Sub

End Module
