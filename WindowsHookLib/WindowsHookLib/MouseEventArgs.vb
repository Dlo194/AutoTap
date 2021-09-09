''' <summary>
''' Provides data for the WindowsHookLib.MouseHook.MouseDown,
''' WindowsHookLib.MouseHook.MouseUp, WindowsHookLib.MouseHook.MouseMove 
''' and WindowsHookLib.MouseHook.MouseWheel events.  
''' </summary>
<DebuggerNonUserCode()> _
Public Class MouseEventArgs
    Inherits System.Windows.Forms.MouseEventArgs

#Region " Members "

    Private _mHandled As Boolean

#End Region

#Region " Methods "

    ''' <param name="button">One of the System.Windows.Forms.MouseButtons 
    ''' values indicating which mouse button was pressed.</param>
    ''' <param name="clicks">The number of times a mouse button was pressed.</param>
    ''' <param name="x">The x-coordinate of a mouse click, in pixels.</param>
    ''' <param name="y">The y-coordinate of a mouse click, in pixels.</param>
    ''' <param name="delta">A signed count of the number 
    ''' of detents the wheel has rotated.</param>
    Sub New(ByVal button As Windows.Forms.MouseButtons, _
    ByVal clicks As Integer, _
    ByVal x As Integer, _
    ByVal y As Integer, _
    ByVal delta As Integer)
        MyBase.New(button, clicks, x, y, delta)
    End Sub

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets a value indicating whether the event was handled.
    ''' </summary>
    Public Property Handled() As Boolean
        Get
            Return Me._mHandled
        End Get
        Set(ByVal value As Boolean)
            Me._mHandled = value
        End Set
    End Property

#End Region

End Class

