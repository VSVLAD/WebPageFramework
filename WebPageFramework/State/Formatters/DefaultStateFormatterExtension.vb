Option Strict On

Imports System.Runtime.CompilerServices
Imports System.Text.Encodings.Web
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Reflection

Public Module DefaultStateFormatterExtension

    Private ReadOnly serializerOptions As New JsonSerializerOptions() With {.Encoder = JavaScriptEncoder.Create(Unicode.UnicodeRanges.All)}

    Private Class ViewObjectType

        <JsonPropertyName("$t")>
        Public Property Type As String

        <JsonPropertyName("$v")>
        Public Property Value As Object

    End Class

    <Extension>
    Public Function SerializeWithTypeInfo(state As ViewObject) As String

        ' Обходим StateObject и сериализуем каждый объект с учетом вложенных словарей
        Dim typedDictionary = state.ToDictionary().ToDictionary(
                    Function(kvp) kvp.Key,
                    Function(kvp) WrapObjectWithTypeInfo(kvp.Value)
                )

        Return JsonSerializer.Serialize(typedDictionary, serializerOptions)
    End Function

    <Extension>
    Public Function DeserializeWithTypeInfo(json As String) As ViewObject

        ' Десериализуем JSON в словарь объектов TypedObject
        Dim typedDictionary = JsonSerializer.Deserialize(Of Dictionary(Of String, ViewObjectType))(json)

        ' Преобразуем значения обратно в объекты исходных типов
        Dim result As New ViewObject

        For Each kvp In typedDictionary
            result(kvp.Key) = UnwrapObjectWithTypeInfo(kvp.Value)
        Next

        Return result
    End Function

    ' Рекурсивный метод для обертывания объектов в TypedObject с учетом вложенности
    Private Function WrapObjectWithTypeInfo(value As Object) As ViewObjectType
        If TypeOf value Is ViewObject Then

            ' Рекурсивно обрабатываем вложенный словарь
            Dim nestedDict = DirectCast(value, ViewObject).ToDictionary().ToDictionary(
                                                    Function(innerKvp) innerKvp.Key,
                                                    Function(innerKvp) WrapObjectWithTypeInfo(innerKvp.Value)
                                                )
            ' Сериализуем вложенный словарь как JsonElement
            Return New ViewObjectType With {
                            .Type = GetType(ViewObject).FullName,
                            .Value = JsonSerializer.SerializeToElement(nestedDict, serializerOptions)
                        }
        Else
            ' Для обычных объектов возвращаем тип и значение
            Return New ViewObjectType With {
                            .Type = value.GetType().FullName,
                            .Value = JsonSerializer.SerializeToElement(value, serializerOptions)
                        }
        End If
    End Function

    Private Function UnwrapObjectWithTypeInfo(typedObj As ViewObjectType) As Object
        Dim targetType = ResolveType(typedObj.Type)
        If targetType Is Nothing Then Throw New InvalidOperationException($"Не удалось разрешить тип ""{typedObj.Type}"" при десериализации ViewObject")

        Dim targetValue = DirectCast(typedObj.Value, JsonElement)

        If targetType Is GetType(ViewObject) Then

            ' Десериализуем вложенный словарь и рекурсивно обрабатываем каждый элемент
            Dim nestedDict As Dictionary(Of String, ViewObjectType) = JsonSerializer.Deserialize(Of Dictionary(Of String, ViewObjectType))(targetValue)

            Return New ViewObject(nestedDict.ToDictionary(
                                                    Function(innerKvp) innerKvp.Key,
                                                    Function(innerKvp) UnwrapObjectWithTypeInfo(innerKvp.Value)
                                                ))
        Else
            ' Десериализация обычного объекта
            Return JsonSerializer.Deserialize(targetValue, targetType)
        End If
    End Function

    ' Попытаться разрешить тип по имени, проверяя:
    ' 1) Type.GetType (может загрузить сборку по имени, если возможно)
    ' 2) по полному/простому имени в уже загруженных сборках
    ' 3) если в typeName есть имя сборки — попытаться Assembly.Load и извлечь тип
    Public Function ResolveType(typeName As String) As Type
        If String.IsNullOrWhiteSpace(typeName) Then Return Nothing

        ' 1) Type.GetType (попробуем сразу)
        Dim t As Type = Type.GetType(typeName, False, True)
        If t IsNot Nothing Then Return t

        ' Упростим имя типа (без указания сборки)
        Dim simpleName As String = typeName
        Dim assemblyNamePart As String = Nothing
        Dim commaIndex As Integer = typeName.IndexOf(","c)
        If commaIndex >= 0 Then
            simpleName = typeName.Substring(0, commaIndex).Trim()
            assemblyNamePart = typeName.Substring(commaIndex + 1).Trim()
        End If

        ' 2) Поиск в уже загруженных сборках
        For Each asm In AppDomain.CurrentDomain.GetAssemblies().Where(Function(a) a.FullName.Contains("WebPages"))
            Try
                ' Попробуем и полный и простой вариант
                t = asm.GetType(typeName, False, True)
                If t IsNot Nothing Then Return t

                t = asm.GetType(simpleName, False, True)
                If t IsNot Nothing Then Return t
            Catch
                ' игнорируем сборки, к которым нет доступа
            End Try
        Next

        ' 3) Если есть имя сборки в строке, попытаться загрузить её (Assembly.Load)
        If Not String.IsNullOrEmpty(assemblyNamePart) Then
            Try
                Dim asm = Assembly.Load(assemblyNamePart)
                If asm IsNot Nothing Then
                    t = asm.GetType(simpleName, False, True)
                    If t IsNot Nothing Then Return t
                End If
            Catch
                ' Assembly.Load может бросать — игнорируем и возвращаем Nothing ниже
            End Try
        End If

        Return Nothing
    End Function

End Module
