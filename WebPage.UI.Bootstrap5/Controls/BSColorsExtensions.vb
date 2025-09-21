Option Strict On

Imports System.Runtime.CompilerServices

Public Module BSColorsExtensions

    ''' <summary>
    ''' Преобразует перечисление цвета в текстовое представление
    ''' </summary>
    <Extension>
    Public Function Value(ThisValue As BSColors) As String
        Select Case ThisValue
            Case BSColors.Primary
                Return "primary"
            Case BSColors.Secondary
                Return "secondary"
            Case BSColors.Success
                Return "success"
            Case BSColors.Warning
                Return "warning"
            Case BSColors.Danger
                Return "danger"
            Case BSColors.Info
                Return "info"
            Case Else
                Return String.Empty
        End Select
    End Function


End Module
