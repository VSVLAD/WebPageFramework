Public Interface IStateProvider

    Sub FromStorage(Page As IPage, StateSerializer As IStateSerializer)

    Sub ToStorage(Page As IPage, StateSerializer As IStateSerializer)

End Interface
