
Public Interface ITemplateProvider

    ''' <summary>
    ''' Метод вызывается фреймворком при каждом запросе формы, чтобы получить текст шаблона с плейсхолдерами для обработки
    ''' </summary>
    Function GetTemplate(Name As String) As Template

End Interface
