Scriptname ReferenceAlias extends Alias Hidden
Function Clear() native
Function ForceRefTo(ObjectReference akNewRef) native
ObjectReference Function GetReference() native
Actor Function GetActorReference()
	Return GetReference() as Actor
EndFunction
ObjectReference Function GetRef()
	Return GetReference()
EndFunction
