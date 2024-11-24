Option Strict On

Imports System.Runtime.CompilerServices
Imports System.Web
Imports WebPages.Controls

Public Module WebPagesHelper

    ' Создать объект состояния
    <Extension>
    Public Function GenerateState(ThisPage As Page, StateProvider As IStateProvider) As String

        ' Создаём дерево состояния
        Dim treeState As New StateObject()

        ' Добавляем собственное состояние формы в ключ $base
        treeState("$base") = ThisPage.ToState()

        ' Выбираем все контролы формы
        For Each parentCtl In ThisPage.Controls.Values

            ' Если контрол является фрагментом, то перебираем всего дочерние контролы
            If TypeOf parentCtl Is Fragment Then
                Dim thisFragment = DirectCast(parentCtl, Fragment)

                ' Добавляем собственное состояние фрагмента в ключ $base и под уровень фрагмента
                treeState(parentCtl.Id) = New StateObject()

                Dim treeStateThisFragment = DirectCast(treeState(parentCtl.Id), StateObject)
                treeStateThisFragment("$base") = thisFragment.ToState()

                ' Выбираем все контролы у фрагмента
                For Each childCtl In thisFragment.Controls.Values

                    ' Прикрепляем состояние контрола под уровень фрагмента
                    Dim ctlState = childCtl.ToState()
                    treeStateThisFragment(childCtl.Id) = ctlState
                Next

            Else
                ' Прикрепляем состояние контрола у формы
                Dim ctlState = parentCtl.ToState()
                treeState(parentCtl.Id) = ctlState
            End If
        Next

        ' Сериализуем в дерево
        Return StateProvider.SaveState(treeState)
    End Function

    ' Загружаем объект состояния и применяем к элементам управления
    <Extension>
    Public Sub ApplyState(ThisPage As Page, StateProvider As IStateProvider)
        Dim viewState = ThisPage.Form(Page.FieldNameViewState)

        ' Если присутствует объект состояния
        If Not String.IsNullOrEmpty(viewState) Then

            ' Десериализуем в дерево
            Dim treeState = StateProvider.LoadState(viewState)

            ' Если есть состояние формы
            If treeState.ContainsKey("$base") Then

                ' Применяем для формы
                Dim treeStateBase = DirectCast(treeState("$base"), StateObject)
                ThisPage.FromState(treeStateBase)
            End If

            ' Перебираем все контролы формы и если они есть в состоянии, то применяем
            For Each parentCtl In ThisPage.Controls.Values

                ' Если в состояние есть ID контрола
                If treeState.ContainsKey(parentCtl.Id) Then

                    ' Если контрол является фрагментом, то перебираем также его дочерние контролы
                    If TypeOf parentCtl Is Fragment Then
                        Dim thisFragment = DirectCast(parentCtl, Fragment)

                        ' Ссылка для работы с элементами состояния фрагмента
                        Dim treeStateThisFragment = DirectCast(treeState(parentCtl.Id), StateObject)

                        ' Применяем собственное состояние фрагмента
                        If treeStateThisFragment.ContainsKey("$base") Then
                            Dim treeStateThisFragmentBase = DirectCast(treeStateThisFragment("$base"), StateObject)
                            thisFragment.FromState(treeStateThisFragmentBase)
                        End If

                        ' Выбираем все контролы у фрагмента
                        For Each childCtl In thisFragment.Controls.Values

                            ' Применяем состояние контрола у фрагмента
                            Dim ctlState = DirectCast(treeStateThisFragment(childCtl.Id), StateObject)
                            childCtl.FromState(ctlState)
                        Next

                    Else
                        ' Применяем состояние контрола у формы
                        Dim ctlState = DirectCast(treeState(parentCtl.Id), StateObject)
                        parentCtl.FromState(ctlState)
                    End If
                End If
            Next

        End If
    End Sub

    ' Применить значение из формы к элементу управления
    <Extension>
    Public Sub ApplyControlFormValue(ThisPage As Page)
        For Each item In ThisPage.Form
            Dim ctlForm = FindControl(ThisPage, item.Key)

            If ctlForm IsNot Nothing Then
                ctlForm.ProcessFormData(ThisPage.Form(item.Value))
            End If
        Next
    End Sub

    ' Создать пользовательские событие
    <Extension>
    Public Sub GenerateControlEvents(ThisPage As Page)
        Dim ctlEvent As IHtmlControl = Nothing

        Dim eventControl = ThisPage.Form(Page.FieldNameEventControl)
        Dim eventName = ThisPage.Form(Page.FieldNameEventName)
        Dim eventArgument = ThisPage.Form(Page.FieldNameEventArgument)

        ' Ищем элемент управления
        If Not String.IsNullOrEmpty(eventControl) Then
            ctlEvent = FindControl(ThisPage, eventControl)
        End If

        ' Если было событие от элемента управления и такой элемент управления существует
        If ctlEvent IsNot Nothing Then
            ctlEvent.ProcessEvent(eventName, eventArgument)
        Else
            Throw New Exception($"Элемент управления ""{eventControl}"" создал событие, но он не зарегистрирован в веб-форме")
        End If
    End Sub

    ''' <summary>
    ''' Выполняет поиск элемента управления на странице, а также во всех фрагментах
    ''' </summary>
    <Extension>
    Public Function FindControl(ThisPage As Page, ControlId As String) As IHtmlControl
        Dim returnControl As IHtmlControl = Nothing

        If ThisPage.Controls.TryGetValue(ControlId, returnControl) Then
            Return returnControl
        Else
            For Each frm In ThisPage.Controls.Values.Where(Function(ctl) TypeOf ctl Is Fragment).Cast(Of Fragment)
                If frm.Controls.TryGetValue(ControlId, returnControl) Then
                    Return returnControl
                End If
            Next
        End If

        Return Nothing
    End Function

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
