Public Interface IContainerEvents

    ''' <summary>
    ''' Выполняется перед обработкой формы
    ''' </summary>
    Event Init()

    ''' <summary>
    ''' Выполняется после обработки формы, далее выполняются пользовательские события
    ''' </summary>
    Event Load(FirstRun As Boolean)

    ''' <summary>
    ''' Выполняется после завершения пользовательских событий перед отрисовкой шаблона
    ''' </summary>
    Event Render()

    ''' <summary>
    ''' Выполняется перед обработкой формы. Вызывается фреймворком
    ''' </summary>
    Sub OnInit()

    ''' <summary>
    ''' Выполняется после обработки формы, далее выполняются пользовательские события. Вызывается фреймворком
    ''' </summary>
    Sub OnLoad(FirstRun As Boolean)

    ''' <summary>
    ''' Выполняется после завершения пользовательских событий перед отрисовкой шаблона. Вызывается фреймворком
    ''' </summary>
    Sub OnRender()

End Interface
