Public Interface IStateProvider

    Function LoadState(State As String) As ViewObject

    Function SaveState(State As ViewObject) As String

End Interface
