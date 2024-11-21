Public Interface IStateSerializer

    Function LoadState(PackedState As String) As Dictionary(Of String, Object)

    Function SaveState(State As Dictionary(Of String, Object)) As String

End Interface
