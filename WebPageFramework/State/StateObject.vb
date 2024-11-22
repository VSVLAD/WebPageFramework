Public Class StateObject
    Private ReadOnly innerState As Dictionary(Of String, Object)

    ' Конструктор для инициализации пустого состояния
    Public Sub New()
        innerState = New Dictionary(Of String, Object)()
    End Sub

    ' Конструктор для инициализации с существующим словарем
    Public Sub New(state As Dictionary(Of String, Object))
        innerState = state
    End Sub

    ' Индексатор для удобного доступа к элементам
    Default Public Property Item(key As String) As Object
        Get
            Return If(innerState.ContainsKey(key), innerState(key), Nothing)
        End Get
        Set(value As Object)
            innerState(key) = value
        End Set
    End Property

    ' Метод для добавления элемента
    Public Sub Add(key As String, value As Object)
        innerState.Add(key, value)
    End Sub

    ' Метод для проверки наличия ключа
    Public Function ContainsKey(key As String) As Boolean
        Return innerState.ContainsKey(key)
    End Function

    ' Метод для получения всех ключей
    Public Function Keys() As IEnumerable(Of String)
        Return innerState.Keys
    End Function

    ' Метод для получения всех значений
    Public Function Values() As IEnumerable(Of Object)
        Return innerState.Values
    End Function

    ' Метод для получения исходного Dictionary(Of String, Object)
    Public Function ToDictionary() As Dictionary(Of String, Object)
        Return innerState
    End Function

End Class
