'Author: Arman Ghazanchyan
'Created date: 11/02/2006

''' <summary>
''' The KeyboardHook class provides functionality to hook the keyboard system wide (low level).
''' </summary>
<DebuggerNonUserCode()> _
<System.ComponentModel.DefaultEvent("StateChanged"), _
Drawing.ToolboxBitmap(GetType(KeyboardHook), "keyboard.png"), _
System.ComponentModel.Description("Component that hooks the keyboard system wide and raises some useful events.")> _
Public Class KeyboardHook
    Inherits System.ComponentModel.Component

#Region " Event Handlers "

    ''' <summary>
    ''' Occurs when the KeyboardHook state changed.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when the KeyboardHook state changed.")> _
    Public Event StateChanged As EventHandler(Of WindowsHook.StateChangedEventArgs)
    ''' <summary>
    ''' Occurs when a key is first pressed.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when a key is first pressed.")> _
    Public Event KeyDown As EventHandler(Of WindowsHook.KeyboardEventArgs)
    ''' <summary>
    ''' Occurs when a key is released.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when a key is released.")> _
    Public Event KeyUp As EventHandler(Of WindowsHook.KeyboardEventArgs)

#End Region

#Region " Members "

    'Holds a method pointer to KeyboardProc for callback.
    'Needed for InstallHook method.
    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.FunctionPtr)> _
    Private _keyboardProc As New WindowsHook.UnsafeNativeMethods.WKCallBack(AddressOf KeyboardProc)
    'Holds the keyboard hook handle. Needed 
    'for RemoveHook and KeyboardProc methods.
    Private _hKeyboardHook As IntPtr
    'Holds a bitwise combination of the keys that are up or down.
    'Needed for key down and key up events’ KeyEventArgs object.
    Private _keyData As Windows.Forms.Keys
    'Holds the hook state.
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
    ''' Gets a Boolean value indicating if the ALT key is down.
    ''' </summary>
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property AltKeyDown() As Boolean
        Get
            Return (Me._keyData And Windows.Forms.Keys.Alt) <> 0
        End Get
    End Property

    ''' <summary>
    ''' Gets a Boolean value indicating if the CTRL key is down.
    ''' </summary>
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property CtrlKeyDown() As Boolean
        Get
            Return (Me._keyData And Windows.Forms.Keys.Control) <> 0
        End Get
    End Property

    ''' <summary>
    ''' Gets a Boolean value indicating if the SHIFT key is down.
    ''' </summary>
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property ShiftKeyDown() As Boolean
        Get
            Return (Me._keyData And Windows.Forms.Keys.Shift) <> 0
        End Get
    End Property

    ''' <summary>
    ''' Gets the state of the keyboard hook.
    ''' </summary>
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property State() As HookState
        Get
            Return Me._state
        End Get
    End Property

#End Region

#Region " Methods "

    'This sub processes all the keyboard messages and passes to the other windows
    Private Function KeyboardProc( _
    ByVal nCode As Integer, _
    ByVal wParam As IntPtr, _
    ByRef lParam As WindowsHook.UnsafeNativeMethods.KeyboardData) As IntPtr
        If (nCode >= UnsafeNativeMethods.HC_ACTION) Then
            Dim e As WindowsHook.KeyboardEventArgs
            Dim keyCode As Windows.Forms.Keys = CType(lParam.vkCode, Windows.Forms.Keys)
            Select Case True
                Case CInt(wParam) = UnsafeNativeMethods.WM_KEYDOWN Or CInt(wParam) = UnsafeNativeMethods.WM_SYSKEYDOWN
                    If keyCode = Windows.Forms.Keys.LMenu Or keyCode = Windows.Forms.Keys.RMenu Then
                        Me._keyData = Me._keyData Or Windows.Forms.Keys.Alt
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or Windows.Forms.Keys.Menu, keyCode)
                    ElseIf keyCode = Windows.Forms.Keys.LControlKey Or keyCode = Windows.Forms.Keys.RControlKey Then
                        Me._keyData = Me._keyData Or Windows.Forms.Keys.Control
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or Windows.Forms.Keys.ControlKey, keyCode)
                    ElseIf keyCode = Windows.Forms.Keys.LShiftKey Or keyCode = Windows.Forms.Keys.RShiftKey Then
                        Me._keyData = Me._keyData Or Windows.Forms.Keys.Shift
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or Windows.Forms.Keys.ShiftKey, keyCode)
                    Else
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or keyCode, keyCode)
                    End If
                    Me.OnKeyDown(e)
                    If e.Handled Then
                        Return CType(1, IntPtr)
                    End If
                Case CInt(wParam) = UnsafeNativeMethods.WM_KEYUP Or CInt(wParam) = UnsafeNativeMethods.WM_SYSKEYUP
                    If keyCode = Windows.Forms.Keys.LMenu Or keyCode = Windows.Forms.Keys.RMenu Then
                        Me._keyData = Me._keyData And Not (Windows.Forms.Keys.Alt)
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or Windows.Forms.Keys.Menu, keyCode)
                    ElseIf keyCode = Windows.Forms.Keys.LControlKey Or keyCode = Windows.Forms.Keys.RControlKey Then
                        Me._keyData = Me._keyData And Not (Windows.Forms.Keys.Control)
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or Windows.Forms.Keys.ControlKey, keyCode)
                    ElseIf keyCode = Windows.Forms.Keys.LShiftKey Or keyCode = Windows.Forms.Keys.RShiftKey Then
                        Me._keyData = Me._keyData And Not (Windows.Forms.Keys.Shift)
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or Windows.Forms.Keys.ShiftKey, keyCode)
                    Else
                        e = New WindowsHook.KeyboardEventArgs(Me._keyData Or keyCode, keyCode)
                    End If
                    Me.OnKeyUp(e)
                    If e.Handled Then
                        Return CType(1, IntPtr)
                    End If
            End Select
        End If
        Return UnsafeNativeMethods.CallNextHookEx(Me._hKeyboardHook, nCode, wParam, lParam)
    End Function

    ''' <summary>
    ''' Installs the keyboard hook for this application. 
    ''' </summary>
    Public Sub InstallHook()
        If Me._state = HookState.Uninstalled Then
            Dim hinstDLL As IntPtr = Runtime.InteropServices.Marshal.GetHINSTANCE( _
            Reflection.Assembly.GetExecutingAssembly().GetModules()(0))
            Me._hKeyboardHook = UnsafeNativeMethods.SetWindowsHookEx(
            UnsafeNativeMethods.WH_KEYBOARD_LL, Me._keyboardProc, IntPtr.Zero, 0)
            If Me._hKeyboardHook = IntPtr.Zero Then
                'Failed to hook. Throw a HookException
                Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
                Throw New KeyboardHookException(New System.ComponentModel.Win32Exception(eCode).Message)
            Else
                If Me._state <> HookState.Installed Then
                    Me._state = HookState.Installed
                    Me.OnStateChanged(New WindowsHook.StateChangedEventArgs(Me._state))
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Removes the keyboard hook for this application.
    ''' </summary>
    Public Sub RemoveHook()
        If Me._state = HookState.Installed Then
            If Not UnsafeNativeMethods.UnhookWindowsHookEx(Me._hKeyboardHook) Then
                'Failed to remove the hook. Throw a HookException
                Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
                Me._hKeyboardHook = IntPtr.Zero
                Throw New KeyboardHookException(New System.ComponentModel.Win32Exception(eCode).Message)
            Else
                Me._keyData = Windows.Forms.Keys.None
                Me._hKeyboardHook = IntPtr.Zero
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
                Me._keyboardProc = Nothing
                'Remove the hook when component is disposing
                Me.RemoveHook()
            End If
        Catch ex As Exception
            ErrorLog.ExceptionToConsole(ex, TraceEventType.Critical)
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

#End Region

#Region " On Event "

    ''' <summary>
    ''' Raises the WindowsHookLib.KeyboardHook.StateChanged event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.StateChangedEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnStateChanged(ByVal e As WindowsHook.StateChangedEventArgs)
        RaiseEvent StateChanged(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.KeyboardHook.KeyUp event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.KeyBoardEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnKeyUp(ByVal e As WindowsHook.KeyboardEventArgs)
        RaiseEvent KeyUp(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.KeyboardHook.KeyDown event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.KeyBoardEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnKeyDown(ByVal e As WindowsHook.KeyboardEventArgs)
        RaiseEvent KeyDown(Me, e)
    End Sub

#End Region

End Class