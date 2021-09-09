''' <summary>
''' Provides data for the WindowsHookLib.KeyboardHook.KeyDown and 
''' WindowsHookLib.KeyboardHook.KeyUp events. 
''' </summary>
<DebuggerNonUserCode()> _
Public Class KeyboardEventArgs
    Inherits System.Windows.Forms.KeyEventArgs

#Region " Members "

    Private _vkCode As Windows.Forms.Keys

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets a value indicating whether the event was handled.
    ''' </summary>
    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), _
    DebuggerBrowsable(DebuggerBrowsableState.Never)> _
    Public Overloads Property SuppressKeyPress() As Boolean
        Get
            Return MyBase.SuppressKeyPress
        End Get
        Set(ByVal value As Boolean)
            MyBase.SuppressKeyPress = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the virtual key code for a KeyDown or KeyUp event.
    ''' </summary>
    Public ReadOnly Property VirtualKeyCode() As Windows.Forms.Keys
        Get
            Return Me._vkCode
        End Get
    End Property

#End Region

#Region " Methods "

    ''' <param name="keyData">A System.Windows.Forms.Keys representing 
    ''' the key that was pressed, combined with any modifier flags that 
    ''' indicate which CTRL, SHIFT, and ALT keys were pressed at the same time. 
    ''' Possible values are obtained by applying bitwise OR (|) operator 
    ''' to constants from the System.Windows.Forms.Keys enumeration.</param>
    Sub New(ByVal keyData As Windows.Forms.Keys, ByVal virtualKeyCode As Windows.Forms.Keys)
        MyBase.New(keyData)
        Me._vkCode = virtualKeyCode
    End Sub

#End Region

End Class
