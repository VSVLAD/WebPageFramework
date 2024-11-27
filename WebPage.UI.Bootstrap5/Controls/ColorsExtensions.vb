Option Strict On

Imports System.Runtime.CompilerServices

Public Module ColorsExtensions

    ''' <summary>
    ''' Преобразует перечисление цвета в текстовое представление
    ''' </summary>
    <Extension>
    Public Function Value(ThisValue As Colors) As String
        Select Case ThisValue
            Case Colors.Primary
                Return "primary"
            Case Colors.Secondary
                Return "secondary"
            Case Colors.Success
                Return "success"
            Case Colors.Warning
                Return "warning"
            Case Colors.Danger
                Return "danger"
            Case Colors.Info
                Return "info"
            Case Else
                Return String.Empty
        End Select
    End Function


End Module
