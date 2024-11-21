Imports System.Runtime.CompilerServices
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.Json.Serialization

Public Module SerializerExtension

    Private serializerOptions As New JsonSerializerOptions() With {.Encoder = JavaScriptEncoder.Default}

    Private Class TypedObject

        <JsonPropertyName("$type")>
        Public Property Type As String

        <JsonPropertyName("$value")>
        Public Property Value As Object

    End Class

    <Extension>
    Public Function SerializeWithTypeInfo(dict As Dictionary(Of String, Object)) As String
        Dim typedDictionary = dict.ToDictionary(
                                            Function(kvp) kvp.Key,
                                            Function(kvp) WrapObjectWithTypeInfo(kvp.Value)
                                        )
        Return JsonSerializer.Serialize(typedDictionary, serializerOptions)
    End Function

    ' Рекурсивный метод для обертывания объектов в TypedObject с учетом вложенности
    Private Function WrapObjectWithTypeInfo(value As Object) As TypedObject
        If TypeOf value Is Dictionary(Of String, Object) Then

            ' Рекурсивно обрабатываем вложенный словарь
            Dim nestedDict = DirectCast(value, Dictionary(Of String, Object)).ToDictionary(
                                                    Function(innerKvp) innerKvp.Key,
                                                    Function(innerKvp) WrapObjectWithTypeInfo(innerKvp.Value)
                                                )
            ' Сериализуем вложенный словарь как JsonElement
            Return New TypedObject With {
                            .Type = GetType(Dictionary(Of String, Object)).AssemblyQualifiedName,
                            .Value = JsonSerializer.SerializeToElement(nestedDict, serializerOptions)
                        }
        Else
            ' Для обычных объектов возвращаем тип и значение
            Return New TypedObject With {
                            .Type = value.GetType().FullName,
                            .Value = JsonSerializer.SerializeToElement(value, serializerOptions)
                        }
        End If
    End Function

    <Extension>
    Public Function DeserializeWithTypeInfo(json As String) As Dictionary(Of String, Object)

        ' Десериализуем JSON в словарь объектов TypedObject
        Dim typedDictionary = JsonSerializer.Deserialize(Of Dictionary(Of String, TypedObject))(json)

        Dim result As New Dictionary(Of String, Object)

        ' Преобразуем значения обратно в исходные типы
        For Each kvp In typedDictionary
            result(kvp.Key) = UnwrapObjectWithTypeInfo(kvp.Value)
        Next

        Return result
    End Function

    Private Function UnwrapObjectWithTypeInfo(typedObj As TypedObject) As Object
        Dim targetType = Type.GetType(typedObj.Type)

        If targetType Is GetType(Dictionary(Of String, Object)) Then

            ' Десериализуем вложенный словарь и рекурсивно обрабатываем каждый элемент
            Dim nestedDict As Dictionary(Of String, TypedObject) = JsonSerializer.Deserialize(Of Dictionary(Of String, TypedObject))(typedObj.Value)

            Return nestedDict.ToDictionary(
                                    Function(innerKvp) innerKvp.Key,
                                    Function(innerKvp) UnwrapObjectWithTypeInfo(innerKvp.Value)
                                )
        Else
            ' Десериализация обычного объекта
            Return JsonSerializer.Deserialize(typedObj.Value, targetType)
        End If
    End Function

End Module
