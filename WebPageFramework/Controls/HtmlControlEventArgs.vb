Option Strict On

Namespace Controls

    Public Class HtmlControlEventArgs
        Inherits EventArgs

        Public Property EventArgument As String

        Public Sub New(EventArgument As String)
            Me.EventArgument = EventArgument
        End Sub

    End Class

End Namespace