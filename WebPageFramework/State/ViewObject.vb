Option Strict On

Public Class ViewObject
    Implements IEnumerable(Of KeyValuePair(Of String, Object))

    Private ReadOnly innerDict As Dictionary(Of String, Object)

    ' Конструктор для инициализации пустого состояния
    Public Sub New()
        innerDict = New Dictionary(Of String, Object)()
    End Sub

    ' Конструктор для инициализации с существующим словарем
    Public Sub New(state As Dictionary(Of String, Object))
        innerDict = state
    End Sub

    ' Индексатор для удобного доступа к элементам
    Default Public Property Item(key As String) As Object
        Get
            Dim value As Object = Nothing
            Return If(innerDict.TryGetValue(key, value), value, Nothing)
        End Get
        Set(value As Object)
            innerDict(key) = value
        End Set
    End Property

    ' Метод для добавления элемента
    Public Sub Add(key As String, value As Object)
        innerDict.Add(key, value)
    End Sub

    ' Метод для удаления элемента
    Public Sub Remove(key As String)
        innerDict.Remove(key)
    End Sub

    ' Метод для удаления всех элементов
    Public Sub Clear()
        innerDict.Clear()
    End Sub

    ' Метод для проверки наличия ключа
    Public Function ContainsKey(key As String) As Boolean
        Return innerDict.ContainsKey(key)
    End Function

    ' Метод для получения всех ключей
    Public Function Keys() As IEnumerable(Of String)
        Return innerDict.Keys
    End Function

    ' Метод для получения всех значений
    Public Function Values() As IEnumerable(Of Object)
        Return innerDict.Values
    End Function

    ' Метод для получения исходного Dictionary(Of String, Object)
    Public Function ToDictionary() As Dictionary(Of String, Object)
        Return innerDict
    End Function

    ' Для получения перечислителя, чтобы использовать цикл по коллекции
    Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, Object)) Implements IEnumerable(Of KeyValuePair(Of String, Object)).GetEnumerator
        Return innerDict.GetEnumerator()
    End Function

    ' Для получения перечислителя, чтобы использовать цикл по коллекции
    Private Function GetEnumeratorObj() As IEnumerator Implements IEnumerable.GetEnumerator
        Return innerDict.GetEnumerator()
    End Function

End Class
