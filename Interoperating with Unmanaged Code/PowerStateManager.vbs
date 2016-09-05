Dim powerStateManager
Set powerStateManager = CreateObject("InteroperatingWithUnmanagedCode.COM.PowerStateManagerCOM")

Dim result
result = "Last Sleep Time: " & powerStateManager.GetLastSleepTime() & vbCrLf
result = result & "Last Wake Time: " & powerStateManager.GetLastWakeTime() & vbCrLf

MsgBox result