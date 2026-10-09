Scriptname BSC_LoadAliasScript extends ReferenceAlias
{Blackshield Company - on the load-watcher quest's player alias. Papyrus drops
update registrations on every save load. This alias is what puts them back.}

BSC_CompanyScript Property Company Auto

Event OnInit()
	Company.GameLoaded()
EndEvent

Event OnPlayerLoadGame()
	Company.GameLoaded()
EndEvent
