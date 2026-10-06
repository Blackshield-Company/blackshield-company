Scriptname Form Hidden
{Clean-room declaration stub (validation only). Build against the game's own sources for releases if you prefer: see tools/modkit/README.md.}
Int Function GetFormID() native
String Function GetName() native
Bool Function PlayerKnows() native
Function RegisterForSingleUpdate(Float afInterval) native
Function RegisterForUpdate(Float afInterval) native
Function UnregisterForUpdate() native
Function RegisterForSingleUpdateGameTime(Float afInterval) native
Function UnregisterForUpdateGameTime() native
Event OnInit()
EndEvent
Event OnUpdate()
EndEvent
Event OnUpdateGameTime()
EndEvent
