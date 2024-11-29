Option Strict On

Imports System.Runtime.CompilerServices
Imports System.Text.Encodings.Web
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization

Public Module DefaultStateFormatterExtension

    Private ReadOnly serializerOptions As New JsonSerializerOptions() With {.Encoder = JavaScriptEncoder.Create(Unicode.UnicodeRanges.All)}

    Private Class ViewObjectType

        <JsonPropertyName("$type")>
        Public Property Type As String

        <JsonPropertyName("$value")>
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
        Dim targetType = Type.GetType(typedObj.Type)
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

End Module
