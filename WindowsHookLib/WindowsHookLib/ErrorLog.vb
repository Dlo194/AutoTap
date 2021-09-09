'Author: Arman Ghazanchyan
'Created date: 07/01/2008
'Last updated: 05/31/2009

<DebuggerNonUserCode()> _
<CLSCompliant(True)> _
Public NotInheritable Class ErrorLog

    Private Shared me_append As Boolean
    Private Shared me_title As String = My.Application.Info.Title
    Private Shared me_location As String = My.Computer.FileSystem.SpecialDirectories.Desktop
    Private Shared me_fileName As String = me_location & "\" & me_title.Replace(" ", String.Empty) & ".log"
    Private Const INDENT_SIZE As Integer = 3

#Region " Properties "

    ''' <summary>
    ''' Gets the title of the log file that leads the message(s).
    ''' </summary>
    Public Shared ReadOnly Property Title() As String
        Get
            Return me_title
        End Get
    End Property

    ''' <summary>
    ''' Gets the location (directory) for the log file.
    ''' </summary>
    Public Shared ReadOnly Property Location() As String
        Get
            Return me_location
        End Get
    End Property

    ''' <summary>
    ''' Gets the log file's full name.
    ''' </summary>
    Public Shared ReadOnly Property FileName() As String
        Get
            Return me_fileName
        End Get
    End Property

#End Region

#Region " Methods "

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Writes an exception information to the system's error and standard consoles.
    ''' </summary>
    ''' <param name="ex">The exception to write to console.</param>
    ''' <param name="severity">The type of message. By default, System.Diagnostics.TraceEventType.Error.</param>
    Public Shared Sub ExceptionToConsole(ByVal ex As Exception, ByVal severity As TraceEventType)
        If ex IsNot Nothing Then
            On Error Resume Next
            Dim sb As New System.Text.StringBuilder
            sb.AppendLine(ErrorLog.GetSystemInfoText)
            sb.AppendLine(ErrorLog.GetExText(ex, 0))
            If Not [Enum].IsDefined(GetType(TraceEventType), severity) Then
                severity = TraceEventType.Error
            End If
            ErrorLog.EntryToConsole(sb.ToString, severity)
        End If
    End Sub

    ''' <summary>
    ''' Writes an exception information to the application’s log file.
    ''' </summary>
    ''' <param name="ex">The exception to log.</param>
    ''' <param name="severity">The type of message. By default, System.Diagnostics.TraceEventType.Error.</param>
    Public Shared Sub ExceptionToFile(ByVal ex As Exception, ByVal severity As TraceEventType)
        If ex IsNot Nothing Then
            On Error Resume Next
            Dim sb As New System.Text.StringBuilder
            sb.AppendLine(ErrorLog.GetSystemInfoText)
            sb.AppendLine(ErrorLog.GetExText(ex, 0))
            If Not [Enum].IsDefined(GetType(TraceEventType), severity) Then
                severity = TraceEventType.Error
            End If
            ErrorLog.EntryToFile(sb.ToString, severity)
        End If
    End Sub

    ''' <summary>
    ''' Writes a message to the system's error and standard consoles.
    ''' </summary>
    ''' <param name="message">The message to write to consoles.</param>
    Public Shared Sub EntryToConsole(ByVal message As String)
        If message IsNot Nothing Then
            System.Console.Write(message)
            System.Console.Error.Write(message)
        End If
    End Sub

    ''' <summary>
    ''' Writes a message to the applications log file.
    ''' </summary>
    ''' <param name="message">The message to log.</param>
    Public Shared Sub EntryToFile(ByVal message As String)
        If message IsNot Nothing Then
            Using sw As New IO.StreamWriter(me_fileName, ErrorLog.me_append, System.Text.Encoding.UTF8)
                sw.WriteLine(message)
            End Using
        End If
        ErrorLog.me_append = True
    End Sub

    ''' <summary>
    ''' Writes a message to the system's error and standard consoles.
    ''' </summary>
    ''' <param name="message">The message to write to consoles. If message is Nothing, an empty string is used.</param>
    ''' <param name="severity">The type of message. By default, System.Diagnostics.TraceEventType.Information.</param>
    Public Shared Sub EntryToConsole(ByVal message As String, ByVal severity As TraceEventType)
        If message Is Nothing Then
            message = String.Empty
        End If
        If Not [Enum].IsDefined(GetType(TraceEventType), severity) Then
            severity = TraceEventType.Information
        End If
        Dim sb As New System.Text.StringBuilder()
        sb.AppendLine(me_title & "   " & severity.ToString & "   " & severity)
        sb.AppendLine(New String("="c, sb.Length))
        sb.AppendLine(message)
        System.Console.Write(sb.ToString)
        System.Console.Error.Write(sb.ToString)
    End Sub

    ''' <summary>
    ''' Writes a message to the applications log file.
    ''' </summary>
    ''' <param name="message">The message to log. If message is Nothing, an empty string is used.</param>
    ''' <param name="severity">The type of message. By default, System.Diagnostics.TraceEventType.Information.</param>
    Public Shared Sub EntryToFile(ByVal message As String, ByVal severity As TraceEventType)
        If message Is Nothing Then
            message = String.Empty
        End If
        If Not [Enum].IsDefined(GetType(TraceEventType), severity) Then
            severity = TraceEventType.Information
        End If
        Using sw As New IO.StreamWriter(me_fileName, ErrorLog.me_append, System.Text.Encoding.UTF8)
            Dim str As String = me_title & "   " & severity.ToString & "   " & severity
            sw.WriteLine(str)
            sw.WriteLine(New String("="c, str.Length))
            sw.WriteLine(message)
        End Using
        ErrorLog.me_append = True
    End Sub

    Private Shared Function GetSystemInfoText() As String
        Dim sb As New System.Text.StringBuilder
        sb.AppendLine()
        sb.AppendLine("System")
        sb.AppendLine("---------")
        sb.AppendFormat("{0,-12}", "UTC Date:")
        sb.AppendLine(Date.UtcNow.ToString(Globalization.CultureInfo.InvariantCulture))
        sb.AppendFormat("{0,-12}", "Local Date:")
        sb.AppendLine(Date.Now.ToString(Globalization.CultureInfo.InvariantCulture))
        sb.AppendFormat("{0,-12}", "Time Zone:")
        sb.Append(TimeZoneInfo.Local.DisplayName)
        sb.Append("; Current UTC Offset: ")
        sb.AppendLine(TimeZoneInfo.Local.GetUtcOffset(Date.Today).ToString)
        sb.AppendFormat("{0,-12}", "Culture:")
        sb.Append(My.Computer.Info.InstalledUICulture.EnglishName)
        sb.AppendLine("; " & My.Computer.Info.InstalledUICulture.Name)
        sb.AppendFormat("{0,-12}", "OS:")
        sb.Append(My.Computer.Info.OSFullName)
        sb.Append("; " & My.Computer.Info.OSPlatform)
        sb.Append("; " & My.Computer.Info.OSVersion)
        sb.Append("; " & Environment.OSVersion.ServicePack)
        Return sb.ToString
    End Function

    Private Shared Function GetExText(ByVal ex As Exception, ByVal iSize As Integer) As String
        Dim indent As String = String.Empty
        If iSize > 0 Then
            indent = New String(" "c, iSize)
        End If
        Dim sb As New System.Text.StringBuilder
        sb.AppendLine()
        sb.Append(indent)
        sb.AppendLine("Exception")
        sb.Append(indent)
        sb.AppendLine("---------")
        sb.Append(indent)
        sb.AppendLine(ErrorLog.GetFormattedText("Type:", ex.GetType.ToString, iSize + 12))
        sb.Append(indent)
        sb.AppendLine(ErrorLog.GetFormattedText("Source:", ex.Source, iSize + 12))
        sb.Append(indent)
        sb.AppendLine(ErrorLog.GetFormattedText("Assembly:", ex.TargetSite.DeclaringType.Assembly.FullName, iSize + 12))
        sb.Append(indent)
        sb.AppendLine(ErrorLog.GetFormattedText("Message:", ex.Message, iSize + 12))
        sb.AppendLine()
        sb.Append(indent)
        sb.AppendLine(ErrorLog.GetFormattedText("Trace:", ex.StackTrace, iSize + 12))
        If Not ex.InnerException Is Nothing Then
            sb.Append(ErrorLog.GetExText(ex.InnerException, iSize + INDENT_SIZE))
        End If
        Return sb.ToString
    End Function

    Private Shared Function GetFormattedText(ByVal leadingStr As String, ByVal str As String, ByVal iSize As Integer) As String
        Dim indent As String = String.Empty
        If iSize > 0 Then
            indent = New String(" "c, iSize)
        End If
        Dim sb As New System.Text.StringBuilder
        Dim lines() As String = str.Split(New String() {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
        If lines.Length > 0 Then
            If leadingStr <> String.Empty Then
                sb.AppendFormat("{0,-12}", leadingStr)
                sb.Append(lines(0).Trim())
            Else
                sb.Append(indent & lines(0).Trim)
            End If
            For i As Integer = 1 To lines.Length - 1
                sb.Append(Environment.NewLine & indent & lines(i).Trim)
            Next
        End If
        Return sb.ToString
    End Function

#End Region

End Class
