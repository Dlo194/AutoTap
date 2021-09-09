
#Region " Enumarations "

''' <summary>
''' Specifies constants that define the state of the hook.
''' </summary>
<CLSCompliant(True)> _
Public Enum HookState As Integer
    Uninstalled = 0
    Installed = 1
End Enum

#End Region

''' <summary>
''' Provides data for the WindowsHookLib.MouseHook.StateChanged,
''' WindowsHookLib.KeyboardHook.StateChanged and WindowsHookLib.ClipboardHook.StateChanged events.
''' </summary>
<DebuggerNonUserCode()> _
<DebuggerDisplay("State = {_state} HWnd = {_hwnd}")> _
Public Class StateChangedEventArgs
    Inherits System.EventArgs

#Region " Members "

    Private ReadOnly _state As HookState

#End Region

#Region " Methods "

    ''' <param name="hookState">A WindowsHookLib.HookState enumeration 
    ''' value representing the state of the hook.</param>
    Sub New(ByVal hookState As HookState)
        Me._state = hookState
    End Sub

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets a value indicating whether the hook is installed. 
    ''' </summary>
    Public ReadOnly Property State() As HookState
        Get
            Return Me._state
        End Get
    End Property

#End Region

End Class


