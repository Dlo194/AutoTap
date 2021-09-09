''' <summary>
''' Provides data for the WindowsHookLib.ClipboardHook.HandleChanged events.
''' </summary>
<DebuggerNonUserCode()> _
<DebuggerDisplay("NewHandle = {_newHandle} OldHandle = {_oldHandle}")> _
Friend Class HandleChangedEventArgs
    Inherits System.EventArgs

#Region " Members "

    Private _oldHandle As IntPtr
    Private _newHandle As IntPtr

#End Region

#Region " Methods "

    ''' <param name="newHandle">The new handle.</param>
    ''' <param name="oldHandle">The old handle.</param>
    Sub New(ByVal newHandle As IntPtr, _
    ByVal oldHandle As IntPtr)
        MyBase.New()
        Me._newHandle = newHandle
        Me._oldHandle = oldHandle
    End Sub

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the old handle  of the object.
    ''' </summary>
    Public ReadOnly Property OldHandle() As IntPtr
        Get
            Return Me._oldHandle
        End Get
    End Property

    ''' <summary>
    ''' Gets the new handle of the object.
    ''' </summary>
    Public ReadOnly Property NewHandle() As IntPtr
        Get
            Return Me._newHandle
        End Get
    End Property

#End Region

End Class
