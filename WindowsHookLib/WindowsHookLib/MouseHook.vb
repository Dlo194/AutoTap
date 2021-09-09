'Author: Arman Ghazanchyan
'Created date: 11/02/2006

<Assembly: CLSCompliant(True)> 

#Region " Enumarations "

''' <summary>
''' Represents mouse coordinate mapping enumatation.
''' </summary>
Public Enum MapOn As Integer
    None = 0
    ''' <summary>
    ''' Maps the absolute coordinates of the mouse on the screen. 
    ''' In a multimonitor system, the coordinates map to the primary monitor.
    ''' </summary>
    PrimaryMonitor = &H8000
    ''' <summary>
    ''' On Windows 2000/XP, maps the mouse coordinates to the entire virtual desktop (multimonitor system).
    ''' </summary>
    VirtualDesktop = &H8000 Or &H4000
End Enum

#End Region

''' <summary>
''' The MouseHook class provides functionality to hook the mouse system wide (low level).
''' </summary>
<DebuggerNonUserCode()> _
<System.ComponentModel.DefaultEvent("StateChanged"), _
Drawing.ToolboxBitmap(GetType(MouseHook), "mouse.png"), _
System.ComponentModel.Description("Component that hooks the mouse system wide and raises some useful" _
& " events. Also, the class provides methods to synthesize mouse events system wide.")> _
Public Class MouseHook
    Inherits System.ComponentModel.Component

#Region " Event Handles "

    ''' <summary>
    ''' Occurs when the MouseHook state changed.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when the MouseHook state changed.")> _
    Public Event StateChanged As EventHandler(Of WindowsHook.StateChangedEventArgs)
    ''' <summary>
    ''' Occurs when a mouse button is down.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when a mouse button is down.")> _
    Public Event MouseDown As EventHandler(Of WindowsHook.MouseEventArgs)
    ''' <summary>
    ''' Occurs when the mouse pointer is moved.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when the mouse pointer is moved.")> _
    Public Event MouseMove As EventHandler(Of WindowsHook.MouseEventArgs)
    ''' <summary>
    ''' Occurs when a mouse button is up.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when a mouse button is up.")> _
    Public Event MouseUp As EventHandler(Of WindowsHook.MouseEventArgs)
    ''' <summary>
    ''' Occurs when the mouse wheel is rotated.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when the mouse wheel is rotated.")> _
    Public Event MouseWheel As EventHandler(Of WindowsHook.MouseEventArgs)
    ''' <summary>
    ''' Occurs when a mouse button is clicked.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when a mouse button is clicked.")> _
    Public Event MouseClick As System.Windows.Forms.MouseEventHandler
    ''' <summary>
    ''' Occurs when a mouse button is double clicked.
    ''' </summary>
    <System.ComponentModel.Description("Occurs when a mouse button is double clicked.")> _
    Public Event MouseDoubleClick As System.Windows.Forms.MouseEventHandler

#End Region

#Region " Members "

    'Holds a method pointer to MouseProc for callback.
    'Needed for InstallHook method.
    <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.FunctionPtr)> _
    Private _mouseProc As New WindowsHook.UnsafeNativeMethods.WMCallBack(AddressOf MouseProc)
    'Holds the mouse hook handle. Needed 
    'for RemoveHook and MouseProc methods.
    Private _hMouseHook As IntPtr
    'Holds the mouse clicks. Needed for mouse 
    'click and mouse double click events.
    Private _clicks As Integer
    'Holds the mouse button that is currently down or up. 
    'Needed for mouse click and mouse double click events.
    Private _button As Windows.Forms.MouseButtons
    'A rectangle that contains the double click area.
    'Needed for double click event.
    Private _rectangle As New Drawing.Rectangle
    'Holds a control handle that the mouse was on previously.
    'Needed for mouse down and mouse up events.
    Private _hwnd As IntPtr
    'Holds bitwise combination of mouse buttons that 
    'are down. Needed for mouse move EventArgs object.
    Private _buttonsDown As Windows.Forms.MouseButtons
    'Control handle that the mouse event is associated.
    Private _cHandle As IntPtr
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
    ''' Gets the state of the mouse hook.
    ''' </summary>
    <System.ComponentModel.Browsable(False)> _
    Public ReadOnly Property State() As HookState
        Get
            Return Me._state
        End Get
    End Property

#End Region

#Region " Methods "

    'Default class constructor.
    Public Sub New()
        Me._rectangle = New Drawing.Rectangle(0, 0, Windows.Forms.SystemInformation _
        .DoubleClickSize.Width, Windows.Forms.SystemInformation.DoubleClickSize.Height)
    End Sub

    'This sub processes all the mouse messages and passes to the other windows
    Private Function MouseProc( _
    ByVal nCode As Integer, _
    ByVal wParam As IntPtr, _
    ByRef lParam As UnsafeNativeMethods.MouseData) As IntPtr
        If (nCode >= UnsafeNativeMethods.HC_ACTION) Then
            Me._cHandle = UnsafeNativeMethods.WindowFromPoint(lParam.pt)
            Dim m As Integer = wParam.ToInt32
            Dim e As WindowsHook.MouseEventArgs = Nothing

            If m = UnsafeNativeMethods.WM_MOUSEMOVE Then
                e = New WindowsHook.MouseEventArgs( _
                Me._buttonsDown, 0, lParam.pt.X, lParam.pt.Y, 0)
                Me.OnMouseMove(e)
            ElseIf m = UnsafeNativeMethods.WM_LBUTTONDOWN Then
                e = Me.SetMouseDownArgs(lParam, Windows.Forms.MouseButtons.Left)
            ElseIf m = UnsafeNativeMethods.WM_RBUTTONDOWN Then
                e = Me.SetMouseDownArgs(lParam, Windows.Forms.MouseButtons.Right)
            ElseIf m = UnsafeNativeMethods.WM_MBUTTONDOWN Then
                e = Me.SetMouseDownArgs(lParam, Windows.Forms.MouseButtons.Middle)
            ElseIf m = UnsafeNativeMethods.WM_XBUTTONDOWN Or m = UnsafeNativeMethods.WM_NCXBUTTONDOWN Then
                Dim hiWord As Integer = CInt(lParam.mouseData >> 16)
                If hiWord = 1 Then
                    e = Me.SetMouseDownArgs(lParam, Windows.Forms.MouseButtons.XButton1)
                ElseIf hiWord = 2 Then
                    e = Me.SetMouseDownArgs(lParam, Windows.Forms.MouseButtons.XButton2)
                End If
            ElseIf m = UnsafeNativeMethods.WM_LBUTTONUP Then
                e = Me.SetMouseUpArgs(lParam, Windows.Forms.MouseButtons.Left)
            ElseIf m = UnsafeNativeMethods.WM_RBUTTONUP Then
                e = Me.SetMouseUpArgs(lParam, Windows.Forms.MouseButtons.Right)
            ElseIf m = UnsafeNativeMethods.WM_MBUTTONUP Then
                e = Me.SetMouseUpArgs(lParam, Windows.Forms.MouseButtons.Middle)
            ElseIf m = UnsafeNativeMethods.WM_XBUTTONUP Or m = UnsafeNativeMethods.WM_NCXBUTTONUP Then
                Dim hiWord As Integer = CInt(lParam.mouseData >> 16)
                If hiWord = 1 Then
                    e = Me.SetMouseUpArgs(lParam, Windows.Forms.MouseButtons.XButton1)
                ElseIf hiWord = 2 Then
                    e = Me.SetMouseUpArgs(lParam, Windows.Forms.MouseButtons.XButton2)
                End If
            ElseIf m = UnsafeNativeMethods.WM_MOUSEWHEEL Or m = UnsafeNativeMethods.WM_MOUSEHWHEEL Then
                Dim hiWord As Integer = CInt(lParam.mouseData >> 16)
                e = New WindowsHook.MouseEventArgs( _
                Windows.Forms.MouseButtons.None, 0, lParam.pt.X, lParam.pt.Y, hiWord)
                Me.OnMouseWheel(e)
            End If
            If e IsNot Nothing AndAlso e.Handled Then
                Return CType(1, IntPtr)
            End If
        End If
        Return UnsafeNativeMethods.CallNextHookEx(Me._hMouseHook, nCode, wParam, lParam)
    End Function

    ''' <summary>
    ''' Installs the mouse hook for this application.
    ''' </summary>
    Public Sub InstallHook()
        If Me._state = HookState.Uninstalled Then
            'Dim hinstDLL As IntPtr = Runtime.InteropServices.Marshal.GetHINSTANCE( _
            'Reflection.Assembly.GetExecutingAssembly().GetModules()(0))
            Me._hMouseHook = UnsafeNativeMethods.SetWindowsHookEx(
            UnsafeNativeMethods.WH_MOUSE_LL, Me._mouseProc, IntPtr.Zero, 0)
            If Me._hMouseHook = IntPtr.Zero Then
                'Failed to hook. Throw a HookException
                Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
                Throw New MouseHookException(New System.ComponentModel.Win32Exception(eCode).Message)
            Else
                If Me._state <> HookState.Installed Then
                    Me._state = HookState.Installed
                    Me.OnStateChanged(New WindowsHook.StateChangedEventArgs(Me._state))
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Removes the mouse hook for this application.
    ''' </summary>
    Public Sub RemoveHook()
        If Me._state = HookState.Installed Then
            If Not UnsafeNativeMethods.UnhookWindowsHookEx(Me._hMouseHook) Then
                'Failed to remove the hook. Throw a HookException
                Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
                Me._hMouseHook = IntPtr.Zero
                Throw New MouseHookException(New System.ComponentModel.Win32Exception(eCode).Message)
            Else
                Me._hwnd = IntPtr.Zero
                Me._clicks = 0
                Me._button = Windows.Forms.MouseButtons.None
                Me._buttonsDown = Windows.Forms.MouseButtons.None
                Me._hMouseHook = IntPtr.Zero
                If Me._state <> HookState.Uninstalled Then
                    Me._state = HookState.Uninstalled
                    Me.OnStateChanged(New WindowsHook.StateChangedEventArgs(Me._state))
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Sets the mouse down data for the MouseEventArgs object 
    ''' and calls OnMouseDown method to raise the mouse down event.
    ''' </summary>
    ''' <param name="lParam">A MouseData structure object 
    ''' that contains the data for the mouse down event.</param>
    ''' <param name="btn">A mouse button that is down.</param>
    Private Function SetMouseDownArgs( _
    ByVal lParam As UnsafeNativeMethods.MouseData, _
    ByVal btn As Windows.Forms.MouseButtons) As WindowsHook.MouseEventArgs
        Static time As Integer = 0
        Dim e As WindowsHook.MouseEventArgs
        If Me._clicks = 1 _
        AndAlso (Me._button = btn) _
        AndAlso (Me._hwnd = Me._cHandle) _
        AndAlso (lParam.time - time <= Windows.Forms.SystemInformation.DoubleClickTime) _
        AndAlso Me._rectangle.Contains(lParam.pt.X, lParam.pt.Y) Then
            Me._clicks = 2
            e = New WindowsHook.MouseEventArgs( _
            btn, 2, lParam.pt.X, lParam.pt.Y, 0)
            Me.OnMouseDown(e)
        Else
            Me._clicks = 1
            e = New WindowsHook.MouseEventArgs( _
            btn, 1, lParam.pt.X, lParam.pt.Y, 0)
            Me.OnMouseDown(e)
        End If
        Me._button = btn
        Me._buttonsDown = Me._buttonsDown Or btn
        time = lParam.time
        Me._hwnd = Me._cHandle
        Me._rectangle.Location = New Drawing.Point( _
        CInt(lParam.pt.X - (Me._rectangle.Width / 2)), _
        CInt(lParam.pt.Y - (Me._rectangle.Height / 2)))
        Return e
    End Function

    ''' <summary>
    ''' Sets the mouse up data for the MouseEventArgs object 
    ''' and calls OnMouseUp method to raise the mouse up event. 
    ''' </summary>
    ''' <param name="lParam">A MouseData structure object 
    ''' that contains the data for the mouse up event.</param>
    ''' <param name="btn">A mouse button that is up.</param>
    Private Function SetMouseUpArgs( _
    ByVal lParam As UnsafeNativeMethods.MouseData, _
    ByVal btn As Windows.Forms.MouseButtons) As WindowsHook.MouseEventArgs
        If Me._button = btn _
        AndAlso Me._hwnd = Me._cHandle _
        AndAlso Me._clicks = 1 Then
            Me.OnMouseClick(New System.Windows.Forms.MouseEventArgs( _
            btn, 1, lParam.pt.X, lParam.pt.Y, 0))
        ElseIf Me._button = btn _
        AndAlso Me._hwnd = Me._cHandle _
        AndAlso Me._clicks = 2 Then
            Me.OnMouseDoubleClick(New System.Windows.Forms.MouseEventArgs( _
            btn, 2, lParam.pt.X, lParam.pt.Y, 0))
            Me._clicks = 0
        End If
        Me._buttonsDown = Me._buttonsDown And Not btn
        Me._button = btn
        Dim e As New WindowsHook.MouseEventArgs( _
        btn, 1, lParam.pt.X, lParam.pt.Y, 0)
        Me.OnMouseUp(e)
        Return e
    End Function

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                Me._mouseProc = Nothing
                'Remove the hook when component is disposing
                Me.RemoveHook()
            End If
        Catch ex As Exception
            ErrorLog.ExceptionToConsole(ex, TraceEventType.Critical)
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

#Region " Synthesize Event "

    ''' <summary>
    ''' Synthesizes a mouse down event system wide.
    ''' </summary>
    ''' <param name="button">A Windows.Forms.MouseButton that should be down.</param>
    ''' <param name="extraInfo">Specifies an additional value 
    ''' associated with the mouse event.</param>
    Public Shared Sub SynthesizeMouseDown(ByVal button As Windows.Forms.MouseButtons, ByVal extraInfo As IntPtr)
        Dim input As UnsafeNativeMethods.MsInput
        input.xi.dwExtraInfo = extraInfo
        If button = Windows.Forms.MouseButtons.Left Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_LEFTDOWN
        ElseIf button = Windows.Forms.MouseButtons.Right Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_RIGHTDOWN
        ElseIf button = Windows.Forms.MouseButtons.Middle Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_MIDDLEDOWN
        ElseIf button = Windows.Forms.MouseButtons.XButton1 Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_XDOWN
            input.xi.mouseData = UnsafeNativeMethods.XBUTTON1
        ElseIf button = Windows.Forms.MouseButtons.XButton2 Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_XDOWN
            input.xi.mouseData = UnsafeNativeMethods.XBUTTON2
        End If
        If UnsafeNativeMethods.SendInput(1, input, UnsafeNativeMethods.MsInput.GetSize) = 0 Then
            'Failed to synthesize the mouse event. Throw a HookException
            Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
            Throw New MouseHookException(New System.ComponentModel.Win32Exception(eCode).Message)
        End If
    End Sub

    ''' <summary>
    ''' Synthesizes a mouse up event system wide.
    ''' </summary>
    ''' <param name="button">A Windows.Forms.MouseButton that should be up.</param>
    ''' <param name="ExtraInfo">Specifies an additional value 
    ''' associated with the mouse event.</param>
    Public Shared Sub SynthesizeMouseUp(ByVal button As Windows.Forms.MouseButtons, ByVal extraInfo As IntPtr)
        Dim input As UnsafeNativeMethods.MsInput
        input.xi.dwExtraInfo = extraInfo
        If button = Windows.Forms.MouseButtons.Left Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_LEFTUP
        ElseIf button = Windows.Forms.MouseButtons.Right Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_RIGHTUP
        ElseIf button = Windows.Forms.MouseButtons.Middle Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_MIDDLEUP
        ElseIf button = Windows.Forms.MouseButtons.XButton1 Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_XUP
            input.xi.mouseData = UnsafeNativeMethods.XBUTTON1
        ElseIf button = Windows.Forms.MouseButtons.XButton2 Then
            input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_XUP
            input.xi.mouseData = UnsafeNativeMethods.XBUTTON2
        End If
        If UnsafeNativeMethods.SendInput(1, input, UnsafeNativeMethods.MsInput.GetSize) = 0 Then
            'Failed to synthesize the mouse event. Throw a HookException
            Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
            Throw New MouseHookException(New System.ComponentModel.Win32Exception(eCode).Message)
        End If
    End Sub

    ''' <summary>
    ''' Synthesizes a mouse wheel event system wide.
    ''' </summary>
    ''' <param name="wheelClicks">A positive value indicates that the wheel was rotated forward, 
    ''' away from the user; a negative value indicates that the wheel was rotated backward, toward 
    ''' the user. One wheel click is defined as wheel delta, which is 120.</param>
    ''' <param name="ExtraInfo">Specifies an additional value 
    ''' associated with the mouse event.</param>
    Public Shared Sub SynthesizeMouseWheel(ByVal wheelClicks As Integer, ByVal extraInfo As IntPtr)
        Dim input As UnsafeNativeMethods.MsInput
        input.xi.dwExtraInfo = extraInfo
        input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_WHEEL
        input.xi.mouseData = UnsafeNativeMethods.WHEEL_DELTA * wheelClicks
        If UnsafeNativeMethods.SendInput(1, input, UnsafeNativeMethods.MsInput.GetSize) = 0 Then
            'Failed to synthesize the mouse event. Throw a HookException
            Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
            Throw New MouseHookException(New System.ComponentModel.Win32Exception(eCode).Message)
        End If
    End Sub

    ''' <summary>
    ''' Synthesizes a mouse move event system wide.
    ''' </summary>
    ''' <param name="location">A screen location where the mouse should be moved.</param>
    ''' <param name="Mapping">Specifies where the mouse coordinates should be mapped.</param>
    ''' <param name="ExtraInfo">Specifies an additional value 
    ''' associated with the mouse event.</param>
    Public Shared Sub SynthesizeMouseMove(ByVal location As Drawing.Point, ByVal mapping As MapOn, ByVal extraInfo As IntPtr)
        Dim input As UnsafeNativeMethods.MsInput
        If mapping = MapOn.VirtualDesktop AndAlso My.Computer.Info.OSPlatform <> "Win32NT" Then
            mapping = MapOn.PrimaryMonitor
        End If
        input.xi.dx = CInt(location.X * (65535 / Windows.Forms.SystemInformation.VirtualScreen.Width)) + 1
        input.xi.dy = CInt(location.Y * (65535 / Windows.Forms.SystemInformation.VirtualScreen.Height)) + 1
        input.xi.dwExtraInfo = extraInfo
        input.xi.dwFlags = UnsafeNativeMethods.MOUSEEVENTF_MOVE Or mapping
        If UnsafeNativeMethods.SendInput(1, input, UnsafeNativeMethods.MsInput.GetSize) = 0 Then
            'Failed to synthesize the mouse event. Throw a HookException
            Dim eCode As Integer = Runtime.InteropServices.Marshal.GetLastWin32Error
            Throw New MouseHookException(New System.ComponentModel.Win32Exception(eCode).Message)
        End If
    End Sub

#End Region

#End Region

#Region " On Event "

    ''' <summary>
    ''' Raises the WindowsHookLib.MouseHook.StateChanged event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.StateChangedEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnStateChanged(ByVal e As WindowsHook.StateChangedEventArgs)
        RaiseEvent StateChanged(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.MouseHook.MouseMove event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.MouseEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnMouseMove(ByVal e As WindowsHook.MouseEventArgs)
        RaiseEvent MouseMove(Me._cHandle, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.MouseHook.MouseDown event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.MouseEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnMouseDown(ByVal e As WindowsHook.MouseEventArgs)
        RaiseEvent MouseDown(Me._cHandle, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.MouseHook.MouseUp event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.MouseEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnMouseUp(ByVal e As WindowsHook.MouseEventArgs)
        RaiseEvent MouseUp(Me._cHandle, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.MouseHook.MouseClick event.
    ''' </summary>
    ''' <param name="e">A System.Windows.Forms.MouseEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnMouseClick(ByVal e As System.Windows.Forms.MouseEventArgs)
        RaiseEvent MouseClick(Me._cHandle, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.MouseHook.MouseDoubleClick event.
    ''' </summary>
    ''' <param name="e">A System.Windows.Forms.MouseEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnMouseDoubleClick(ByVal e As System.Windows.Forms.MouseEventArgs)
        RaiseEvent MouseDoubleClick(Me._cHandle, e)
    End Sub

    ''' <summary>
    ''' Raises the WindowsHookLib.MouseHook.MouseWheel event.
    ''' </summary>
    ''' <param name="e">A WindowsHookLib.MouseEventArgs
    ''' that contains the event data.</param>
    Protected Overridable Sub OnMouseWheel(ByVal e As WindowsHook.MouseEventArgs)
        RaiseEvent MouseWheel(IntPtr.Zero, e)
    End Sub

#End Region

End Class

