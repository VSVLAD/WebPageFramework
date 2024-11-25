Option Strict On

Imports System.Runtime.CompilerServices
Imports System.Text.Encodings.Web
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization

Public Module SerializerExtension

    Private ReadOnly serializerOptions As New JsonSerializerOptions() With {.Encoder = JavaScriptEncoder.Create(Unicode.UnicodeRanges.All)}

    Private Class StateTypeInfo

        <JsonPropertyName("$type")>
        Public Property Type As String

        <JsonPropertyName("$value")>
        Public Property Value As Object

    End Class

    <Extension>
    Public Function SerializeWithTypeInfo(state As StateObject) As String

        ' Обходим StateObject и сериализуем каждый объект с учетом вложенных словарей
        Dim typedDictionary = state.ToDictionary().ToDictionary(
                    Function(kvp) kvp.Key,
                    Function(kvp) WrapObjectWithTypeInfo(kvp.Value)
                )

        Return JsonSerializer.Serialize(typedDictionary, serializerOptions)
    End Function

    <Extension>
    Public Function DeserializeWithTypeInfo(json As String) As StateObject

        ' Десериализуем JSON в словарь объектов TypedObject
        Dim typedDictionary = JsonSerializer.Deserialize(Of Dictionary(Of String, StateTypeInfo))(json)

        ' Преобразуем значения обратно в объекты исходных типов
        Dim result As New StateObject

        For Each kvp In typedDictionary
            result(kvp.Key) = UnwrapObjectWithTypeInfo(kvp.Value)
        Next

        Return result
    End Function

    ' Рекурсивный метод для обертывания объектов в TypedObject с учетом вложенности
    Private Function WrapObjectWithTypeInfo(value As Object) As StateTypeInfo
        If TypeOf value Is StateObject Then

            ' Рекурсивно обрабатываем вложенный словарь
            Dim nestedDict = DirectCast(value, StateObject).ToDictionary().ToDictionary(
                                                    Function(innerKvp) innerKvp.Key,
                                                    Function(innerKvp) WrapObjectWithTypeInfo(innerKvp.Value)
                                                )
            ' Сериализуем вложенный словарь как JsonElement
            Return New StateTypeInfo With {
                            .Type = GetType(StateObject).FullName,
                            .Value = JsonSerializer.SerializeToElement(nestedDict, serializerOptions)
                        }
        Else
            ' Для обычных объектов возвращаем тип и значение
            Return New StateTypeInfo With {
                            .Type = value.GetType().FullName,
                            .Value = JsonSerializer.SerializeToElement(value, serializerOptions)
                        }
        End If
    End Function

    Private Function UnwrapObjectWithTypeInfo(typedObj As StateTypeInfo) As Object
        Dim targetType = Type.GetType(typedObj.Type)
        Dim targetValue = DirectCast(typedObj.Value, JsonElement)

        If targetType Is GetType(StateObject) Then

            ' Десериализуем вложенный словарь и рекурсивно обрабатываем каждый элемент
            Dim nestedDict As Dictionary(Of String, StateTypeInfo) = JsonSerializer.Deserialize(Of Dictionary(Of String, StateTypeInfo))(targetValue)

            Return New StateObject(nestedDict.ToDictionary(
                                                    Function(innerKvp) innerKvp.Key,
                                                    Function(innerKvp) UnwrapObjectWithTypeInfo(innerKvp.Value)
                                                ))
        Else
            ' Десериализация обычного объекта
            Return JsonSerializer.Deserialize(targetValue, targetType)
        End If
    End Function

End Module
