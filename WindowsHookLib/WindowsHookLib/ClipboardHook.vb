'Author: Arman Ghazanchyan
'Created date: 01/11/2008

''' <summary>
''' The ClipboardHook class provides functionality to hook a window to the clipboard chain.
''' </summary>
<DebuggerNonUserCode()> _
<System.ComponentModel.DefaultEvent("StateChanged"), _
Drawing.ToolboxBitmap(GetType(ClipboardHook), "clipboard.bmp"), _
System.ComponentModel.Description("Component that hooks a window to the clipboard chain and raises some useful events.")> _
Public Class ClipboardHook
    Inherits System.ComponentModel.Component

#Region " Event Handlers and Delegates "

    ''' <summary>
    ''' Occurs when the ClipboardHook state changed.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when the ClipboardHook state changed.")> _
    Public Event StateChanged As EventHandler(Of WindowsHook.StateChangedEventArgs)
    ''' <summary>
    ''' Occurs when the clipboard contents is changed.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when the clipboard contents is changed.")> _
    Public Event ClipboardChanged As EventHandler(Of WindowsHook.ClipboardEventArgs)

#End Region

#Region " Members "

    'Holds the nex window handler in the clipboard chain
    Private _nextWind As IntPtr
    'Clipboard NativeWindow
    Private WithEvents _clipboardProc As New ClipboardPorc(Me._nextWind)
    'Holds the hook state
    Private _state As HookState

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the component's assembly information.
    ''' </summary>
    Public Shared ReadOnly Property Info() As ApplicationServices.AssemblyInfo
        Get
            Return New ApplicationServices.AssemblyInfo(Reflection.Assembly.GetExecutingAssembly)
        End Get
    End Property

    ''' <summary>
    ''' Gets the state of the clipboard hook.
    ''' </summary>
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property State() As HookState
        Get
            Return Me._state
        End Get
    End Property

    ''' <summary>
    ''' Gets the hooked window handle.
    ''' </summary>
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property HookedWindow() As IntPtr
        Get
            Return Me._clipboardProc.Handle
        End Get
    End Property

#End Region

#Region " Methods "

    ''' <summary>
    ''' Installs the clipboard hook for a window.
    ''' </summary>
    ''' <param name="window">
    ''' A valid window (Form) within the solution associated with the ClipboardHook.
    ''' </param>
    Public Sub InstallHook(ByVal window As Windows.Forms.Form)
        If Me._state = HookState.Uninstalled Then
            If window IsNot Nothing Then
                Me._nextWind = UnsafeNativeMethods.SetClipboardViewer(window.Handle)
                Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
                If eCode <> 0 Then
                    'Failed to hook. Throw a HookException
                    Throw New ClipboardHookException(New System.ComponentModel.Win32Exception(eCode).Message)
                Else
                    Me._clipboardProc.AssignHandle(window.Handle)
                End If
            Else
                Throw New ArgumentNullException("window", "The argument cannot be Null (Nothing in VB).")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Removes the clipboard hook for this window.
    ''' </summary>
    Public Sub RemoveHook()
        Me._clipboardProc.ReleaseHandle()
    End Sub

    ''' <summary>
    ''' Removes the clipboard hook for this window.
    ''' </summary>
    Protected Sub RemoveHook(ByVal windowHandle As IntPtr)
        If Me._state = HookState.Installed AndAlso windowHandle <> IntPtr.Zero Then
            If Not UnsafeNativeMethods.ChangeClipboardChain(windowHandle, Me._nextWind) Then
                Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
                If eCode <> 0 Then
                    'Failed to hook. Throw a HookException
                    Throw New ClipboardHookException(New System.ComponentModel.Win32Exception(eCode).Message)
                End If
            Else
                Me._nextWind = IntPtr.Zero
                If Me._state <> HookState.Uninstalled Then
                    Me._state = HookState.Uninstalled
                    Me.OnStateChanged(New WindowsHook.StateChangedEventArgs(Me._state))
                End If
            End If
        End If
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                'Remove the hook when component is disposing
                Me._clipboardProc.ReleaseHandle()
                Me._clipboardProc = Nothing
            End If
        Catch ex As Exception
            ErrorLog.ExceptionToConsole(ex, TraceEventType.Critical)
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

#End Region

#Region " Events "

    Private Sub _clipboardProc_ClipboardChanged(ByVal sender As Object, ByVal e As ClipboardEventArgs) Handles _clipboardProc.ClipboardChanged
        Me.OnClipboardChanged(e)
    End Sub

    Private Sub _clipboardProc_HandleChanged(ByVal sender As Object, ByVal e As HandleChangedEventArgs) Handles _clipboardProc.HandleChanged
        If e.NewHandle = IntPtr.Zero Then
            Me.RemoveHook(e.OldHandle)
        Else
            If Me._state <> HookState.Installed Then
                Me._state = HookState.Installed
                Me.OnStateChanged(New WindowsHook.StateChangedEventArgs(Me._state))
            End If
        End If
    End Sub

#End Region

#Region " On Event "

    ''' <summary>
    ''' Raises the WindowsHookLib.ClipboardHook.StateChanged event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.StateChangedEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnStateChanged(ByVal e As WindowsHook.StateChangedEventArgs)
        RaiseEvent StateChanged(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.ClipboardHook.ClipboardChanged event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.ClipboardEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnClipboardChanged(ByVal e As WindowsHook.ClipboardEventArgs)
        RaiseEvent ClipboardChanged(Me, e)
    End Sub

#End Region

#Region " ClipboardPrc Class "

    <DebuggerNonUserCode()> _
    Private Class ClipboardPorc
        Inherits Windows.Forms.NativeWindow

        Public Event HandleChanged As EventHandler(Of WindowsHook.HandleChangedEventArgs)
        Public Event ClipboardChanged As EventHandler(Of WindowsHook.ClipboardEventArgs)
        Private _nextWind As IntPtr
        Private _handle As IntPtr

        Protected Overrides Sub OnHandleChange()
            MyBase.OnHandleChange()
            RaiseEvent HandleChanged(Me, New HandleChangedEventArgs(Me.Handle, Me._handle))
            Me._handle = Me.Handle
        End Sub

        Sub New(ByRef nextWind As IntPtr)
            Me._nextWind = nextWind
        End Sub

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            If m.Msg = UnsafeNativeMethods.WM_DRAWCLIPBOARD Then
                MyBase.WndProc(m)
                RaiseEvent ClipboardChanged(Me, New WindowsHook.ClipboardEventArgs(m.HWnd, m.WParam))
            ElseIf m.Msg = UnsafeNativeMethods.WM_CHANGECBCHAIN Then
                If m.WParam = Me._nextWind Then
                    'The window is being removed is the next window on the clipboard chain.
                    'Change the ClipboardHook._nextWind handle with LParam.
                    'There is no need to pass this massage any farther.
                    Me._nextWind = m.LParam
                Else
                    MyBase.WndProc(m)
                End If
            Else
                MyBase.WndProc(m)
            End If
        End Sub
    End Class

#End Region

End Class
