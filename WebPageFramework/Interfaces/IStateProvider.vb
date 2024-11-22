Public Interface IStateProvider

    Function LoadState(State As String) As StateObject

    Function SaveState(State As StateObject) As String

End Interface
