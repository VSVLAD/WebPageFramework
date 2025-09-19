Option Strict On

Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Web
Imports WebPages.Controls

Public Module WebPagesHelper

    ' Создать объект состояния
    <Extension>
    Public Sub PageSaveState(ThisPage As Page, StateProvider As IStateProvider, StateFormatter As IStateFormatter)

        ' Готовим дерево состояния
        Dim treeState As New ViewObject()

        ' Добавляем собственное состояние формы в ключ $base
        treeState("$base") = ThisPage.ToState()

        ' Выбираем все контролы формы
        For Each parentCtl In ThisPage.Controls.Values

            ' Если контрол является фрагментом, то перебираем всего дочерние контролы
            If TypeOf parentCtl Is Fragment Then
                Dim thisFragment = DirectCast(parentCtl, Fragment)

                ' Добавляем собственное состояние фрагмента в ключ $base и под уровень фрагмента
                treeState(parentCtl.Id) = New ViewObject()

                Dim treeStateThisFragment = DirectCast(treeState(parentCtl.Id), ViewObject)
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

        ' Сериализуем в дерево и сохраняемв хранилище
        StateProvider.ToStorage(StateFormatter.SerializeState(treeState), ThisPage)
    End Sub

    ' Загружаем объект состояния и применяем к элементам управления
    <Extension>
    Public Function PageLoadState(ThisPage As Page, StateProvider As IStateProvider, StateFormatter As IStateFormatter) As Boolean

        ' Читаем запакованное состояние из хранилища
        Dim packedTreeState = StateProvider.FromStorage(ThisPage)

        If Not String.IsNullOrEmpty(packedTreeState) Then

            ' Десериализуем в дерево
            Dim treeState = StateFormatter.DeserializeState(packedTreeState)

            ' Если есть состояние формы
            If treeState.ContainsKey("$base") Then

                ' Применяем для формы
                Dim treeStateBase = DirectCast(treeState("$base"), ViewObject)
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
                        Dim treeStateThisFragment = DirectCast(treeState(parentCtl.Id), ViewObject)

                        ' Применяем собственное состояние фрагмента
                        If treeStateThisFragment.ContainsKey("$base") Then
                            Dim treeStateThisFragmentBase = DirectCast(treeStateThisFragment("$base"), ViewObject)
                            thisFragment.FromState(treeStateThisFragmentBase)
                        End If

                        ' Выбираем все контролы у фрагмента
                        For Each childCtl In thisFragment.Controls.Values

                            ' Применяем состояние контрола у фрагмента
                            Dim ctlState = DirectCast(treeStateThisFragment(childCtl.Id), ViewObject)
                            childCtl.FromState(ctlState)
                        Next

                    Else
                        ' Применяем состояние контрола у формы
                        Dim ctlState = DirectCast(treeState(parentCtl.Id), ViewObject)
                        parentCtl.FromState(ctlState)
                    End If
                End If
            Next

            Return True
        End If

        Return False
    End Function

    ' Применить значение из формы к элементу управления
    <Extension>
    Public Sub ApplyControlFormValue(ThisPage As Page)
        For Each item In ThisPage.Form
            Dim ctlForm = FindControl(ThisPage, item.Key)
            ctlForm?.ProcessFormData(item.Value)
        Next
    End Sub

    ' Создать пользовательские событие
    <Extension>
    Public Sub GenerateControlEvent(ThisPage As Page)
        Dim eventControl = ThisPage.Form(HtmlControl.FieldNameEventControl)
        Dim eventName = ThisPage.Form(HtmlControl.FieldNameEventName)
        Dim eventArgument = ThisPage.Form(HtmlControl.FieldNameEventArgument)

        ' Ищем элемент управления. Если ЭУ не задан, значит это пустой Postback
        If Not String.IsNullOrEmpty(eventControl) Then
            Dim ctlEvent = FindControl(ThisPage, eventControl)

            ' Если было событие от элемента управления и такой элемент управления существует
            If ctlEvent IsNot Nothing Then
                ctlEvent.ProcessControlEvent(eventName, eventArgument)
            Else
                Throw New Exception($"Элемент управления ""{eventControl}"" создал событие, но он не зарегистрирован в веб-форме")
            End If
        End If
    End Sub


    ' Создать событие загрузки файлов, если форма пришла с файлами
    <Extension>
    Public Sub GenerateFormFilesEvent(ThisPage As Page, tokenCancel As CancellationToken)
        If ThisPage.Form.Files?.Count > 0 Then

            ' Группируем файлы по имени (name атрибут input)
            Dim filesByName = ThisPage.Form.Files.GroupBy(Function(f) f.Name, StringComparer.OrdinalIgnoreCase)

            For Each grp In filesByName
                Dim eventControl = grp.Key
                Dim ctlFile = FindControl(ThisPage, eventControl)

                If ctlFile IsNot Nothing Then
                    ctlFile.ProcessFile(grp, tokenCancel)
                Else
                    Throw New Exception($"Элемент управления ""{eventControl}"" создал событие загрузки файла, но он не зарегистрирован в веб-форме")
                End If
            Next
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
        ThisPage.ViewData("__formBegin") = $"<form name=""{HttpUtility.HtmlAttributeEncode(ThisPage.Id)}"" enctype=""{HtmlControl.FormEncType}"" action=""{ThisPage.Context.Request.Path}"" method=""post"">
    <input type=""hidden"" name=""{HtmlControl.FieldNameEventControl}"" value="""" />
    <input type=""hidden"" name=""{HtmlControl.FieldNameEventName}"" value="""" />
    <input type=""hidden"" name=""{HtmlControl.FieldNameEventArgument}"" value="""" />

    <script>
        function {HtmlControl.FunctionNamePostBack}({HtmlControl.FieldNameEventControl}, {HtmlControl.FieldNameEventName}, {HtmlControl.FieldNameEventArgument}) {{
            let form = document.forms[""{HttpUtility.HtmlAttributeEncode(ThisPage.Id)}""];
            if (!form.onsubmit || form.onsubmit()) {{
                form.{HtmlControl.FieldNameEventControl}.value = {HtmlControl.FieldNameEventControl};
                form.{HtmlControl.FieldNameEventName}.value = {HtmlControl.FieldNameEventName};
                form.{HtmlControl.FieldNameEventArgument}.value = {HtmlControl.FieldNameEventArgument};
                form.submit();
            }}
        }}
    </script>
"
        ThisPage.ViewData("__formEnd") = "</form>"
    End Sub

    <Extension>
    Public Sub RegisterClientJavascript(ThisPage As Page, ScriptBody As String, Defer As Boolean, Async As Boolean)

    End Sub

End Module
