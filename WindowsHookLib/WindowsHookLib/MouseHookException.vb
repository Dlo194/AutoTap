''' <summary>
''' Represents errors that occur in WindowsHookLib.MouseHook.
''' </summary>
<Serializable(), DebuggerNonUserCode()> _
Public Class MouseHookException
    Inherits Exception

#Region " Methods "

    Sub New()
    End Sub

    Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Sub New(ByVal message As String, ByVal ex As Exception)
        MyBase.New(message, ex)
    End Sub

    Protected Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub

#End Region

End Class
