Imports System.Runtime.InteropServices

<DebuggerNonUserCode()>
Public NotInheritable Class UnsafeNativeMethods

#Region " Structures "

    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure KeyboardData
        Public vkCode As Integer
        Public scanCode As Integer
        Public flags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure

    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure MouseData
        Public pt As Drawing.Point
        Public mouseData As Integer
        Public flags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure

    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure MsInput
        Public dwType As Integer
        Public xi As MouseInput

        Public Shared Function GetSize() As Integer
            Return 4 + MouseInput.GetSize
        End Function
    End Structure

    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure MouseInput
        Public dx As Integer
        Public dy As Integer
        Public mouseData As Integer
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr

        Public Shared Function GetSize() As Integer
            Return 20 + IntPtr.Size
        End Function
    End Structure

    <Runtime.InteropServices.StructLayout(Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    Public Enum WindowLongFlags As Integer
        GWL_EXSTYLE = -20
        GWLP_HINSTANCE = -6
        GWLP_HWNDPARENT = -8
        GWL_ID = -12
        GWL_STYLE = -16
        GWL_USERDATA = -21
        GWL_WNDPROC = -4
        DWLP_USER = &H8
        DWLP_MSGRESULT = &H0
        DWLP_DLGPROC = &H4
    End Enum
#End Region

#Region " Delegates "

    ''' <summary>
    ''' Represents the method that will handle keyboard messages.
    ''' </summary>
    Public Delegate Function WKCallBack(ByVal nCode As Integer, ByVal wParam As IntPtr, ByRef lParam As KeyboardData) As IntPtr

    ''' <summary>
    ''' Represents the method that will handle mouse messages.
    ''' </summary>
    Public Delegate Function WMCallBack(ByVal nCode As Integer, ByVal wParam As IntPtr, ByRef lParam As MouseData) As IntPtr

#End Region

#Region " Constants "

    Public Const HC_ACTION As Int32 = 0

#Region " Keyboard "

    Public Const WH_KEYBOARD As Int32 = 2
    Public Const WH_KEYBOARD_LL As Int32 = 13
    Public Const WM_KEYDOWN As Int32 = &H100
    Public Const WM_KEYUP As Int32 = &H101
    Public Const WM_SYSKEYDOWN As Int32 = &H104
    Public Const WM_SYSKEYUP As Int32 = &H105

#End Region

#Region " Mouse "

    Public Const WH_MOUSE As Int32 = 7
    Public Const WH_MOUSE_LL As Int32 = 14
    Public Const WM_MOUSEMOVE As Int32 = &H200
    Public Const WM_LBUTTONDOWN As Int32 = &H201
    Public Const WM_LBUTTONUP As Int32 = &H202
    Public Const WM_LBUTTONDBLCLK As Int32 = &H203
    Public Const WM_RBUTTONDOWN As Int32 = &H204
    Public Const WM_RBUTTONUP As Int32 = &H205
    Public Const WM_RBUTTONDBLCLK As Int32 = &H206
    Public Const WM_MBUTTONDOWN As Int32 = &H207
    Public Const WM_MBUTTONUP As Int32 = &H208
    Public Const WM_MBUTTONDBLCLK As Int32 = &H209
    Public Const WM_MOUSEWHEEL As Int32 = &H20A
    Public Const WM_MOUSEHWHEEL As Int32 = &H20E
    Public Const WM_XBUTTONDOWN As Int32 = &H20B
    Public Const WM_XBUTTONUP As Int32 = &H20C
    Public Const WM_XBUTTONDBLCLK As Int32 = &H20D
    Public Const WM_NCXBUTTONDOWN As Int32 = &HAB
    Public Const WM_NCXBUTTONUP As Int32 = &HAC
    Public Const WM_NCXBUTTONDBLCLK As Int32 = &HAD

    ' These are SynthesizeMouse constants
    Public Const MOUSEEVENTF_ABSOLUTE As Int32 = &H8000
    Public Const MOUSEEVENTF_LEFTDOWN As Int32 = &H2
    Public Const MOUSEEVENTF_LEFTUP As Int32 = &H4
    Public Const MOUSEEVENTF_MIDDLEDOWN As Int32 = &H20
    Public Const MOUSEEVENTF_MIDDLEUP As Int32 = &H40
    Public Const MOUSEEVENTF_MOVE As Int32 = &H1
    Public Const MOUSEEVENTF_RIGHTDOWN As Int32 = &H8
    Public Const MOUSEEVENTF_RIGHTUP As Int32 = &H10
    Public Const MOUSEEVENTF_VIRTUALDESK As Int32 = &H4000
    Public Const MOUSEEVENTF_WHEEL As Int32 = &H800
    Public Const MOUSEEVENTF_XDOWN As Int32 = &H80
    Public Const MOUSEEVENTF_XUP As Int32 = &H100
    Public Const XBUTTON1 As Int32 = &H1
    Public Const XBUTTON2 As Int32 = &H2
    Public Const WHEEL_DELTA As Int32 = 120

#End Region

#Region " Clipboard "

    Public Const WM_DRAWCLIPBOARD As Int32 = &H308
    Public Const WM_CHANGECBCHAIN As Int32 = &H30D

#End Region

    Public Const OCR_NORMAL As Integer = 32512
#End Region

#Region " Methods "

    Private Sub New()
    End Sub

    <Runtime.InteropServices.DllImport("user32", SetLastError:=True)>
    Public Overloads Shared Function SetWindowsHookEx(
    ByVal idHook As Integer,
    ByVal lpfn As WKCallBack,
    ByVal hMod As IntPtr,
    ByVal dwThreadId As Integer) As IntPtr
    End Function

    <Runtime.InteropServices.DllImport("user32", SetLastError:=True)>
    Public Shared Function UnhookWindowsHookEx(
    ByVal hook As IntPtr) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
    End Function

    <Runtime.InteropServices.DllImport("user32")>
    Public Overloads Shared Function CallNextHookEx(
    ByVal hhk As IntPtr,
    ByVal nCode As Integer,
    ByVal wParam As IntPtr,
    ByRef lParam As KeyboardData) As IntPtr
    End Function

    <Runtime.InteropServices.DllImport("user32", SetLastError:=True)>
    Public Overloads Shared Function SetWindowsHookEx(
    ByVal idHook As Integer,
    ByVal lpfn As WMCallBack,
    ByVal hMod As IntPtr,
    ByVal dwThreadId As Integer) As IntPtr
    End Function

    <Runtime.InteropServices.DllImport("user32")>
    Public Overloads Shared Function CallNextHookEx(
    ByVal hhk As IntPtr,
    ByVal nCode As Integer,
    ByVal wParam As IntPtr,
    ByRef lParam As MouseData) As IntPtr
    End Function

    <Runtime.InteropServices.DllImport("user32", SetLastError:=True)>
    Public Shared Function SendInput(
    ByVal cInputs As Integer,
    ByRef pInputs As MsInput,
    ByVal cbSize As Integer) As Integer
    End Function

    <Runtime.InteropServices.DllImport("user32")>
    Public Shared Function WindowFromPoint(
    ByVal pt As Drawing.Point) As IntPtr
    End Function

    <Runtime.InteropServices.DllImport("user32", SetLastError:=True)>
    Public Shared Function SetClipboardViewer(
    ByVal hWndNewViewer As IntPtr) As IntPtr
    End Function

    <Runtime.InteropServices.DllImport("user32")>
    Public Shared Function ChangeClipboardChain(
    ByVal hWndRemove As IntPtr,
    ByVal hWndNewNext As IntPtr) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
    End Function


    <Runtime.InteropServices.DllImport("user32.dll", SetLastError:=True)>
    Public Shared Function GetWindowRect(hwnd As IntPtr, ByRef lpRect As RECT) As <Runtime.InteropServices.MarshalAs(Runtime.InteropServices.UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SetSystemCursor(ByVal hCursor As IntPtr, ByVal id As Integer) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function LoadCursorFromFile(ByVal fileName As String) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function CopyIcon(ByVal hIcon As IntPtr) As IntPtr
    End Function


    <DllImport("user32.dll")>
    Public Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True)>
    Public Shared Function FindWindowByCaption(ZeroOnly As IntPtr, lpWindowName As String) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SetWindowLong(hWnd As IntPtr, nIndex As Int32, dwNewLong As Int32) As Int32
    End Function


    <DllImport("user32.dll", EntryPoint:="GetWindowLong")>
    Private Shared Function GetWindowLongPtr32(ByVal hWnd As HandleRef, ByVal nIndex As Integer) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="GetWindowLongPtr")>
    Private Shared Function GetWindowLongPtr64(ByVal hWnd As HandleRef, ByVal nIndex As Integer) As IntPtr
    End Function

    ' This static method is required because Win32 does not support GetWindowLongPtr dirctly
    Public Shared Function GetWindowLongPtr(ByVal hWnd As HandleRef, ByVal nIndex As Integer) As IntPtr
        If IntPtr.Size = 8 Then
            Return GetWindowLongPtr64(hWnd, nIndex)
        Else
            Return GetWindowLongPtr32(hWnd, nIndex)
        End If
    End Function
#End Region

End Class
