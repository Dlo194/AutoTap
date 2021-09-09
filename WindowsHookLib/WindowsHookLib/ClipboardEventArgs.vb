''' <summary>
''' Provides data for the WindowsHookLib.ClipboardHook.ClipboardChanged events.
''' </summary>
<DebuggerNonUserCode()> _
<DebuggerDisplay("HWnd = {_hwnd} DHWnd = {_dhwnd}")> _
Public Class ClipboardEventArgs
    Inherits System.EventArgs

#Region " Members "

    Private ReadOnly _hWnd As IntPtr
    Private ReadOnly _sWnd As IntPtr

#End Region

#Region " Methods "

    ''' <param name="hookedWindow">A window handle associated with this hook.</param>
    ''' <param name="sourceWindow">The window handle that has set the last clipboard data.</param>
    Sub New(ByVal hookedWindow As IntPtr, ByVal sourceWindow As IntPtr)
        Me._hWnd = hookedWindow
        Me._sWnd = sourceWindow
    End Sub

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets a window handle associated with this hook.
    ''' </summary>
    Public ReadOnly Property HookedWindow() As IntPtr
        Get
            Return Me._hWnd
        End Get
    End Property

    ''' <summary>
    ''' Gets the window handle that has set the last clipboard data.
    ''' </summary>
    Public ReadOnly Property SourceWindow() As IntPtr
        Get
            Return Me._sWnd
        End Get
    End Property

#End Region

End Class
