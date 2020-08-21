function ActivateRPGMode()
	gmm.SwitchInterface("RPGMode")
end

function ActivateBuildMode()
	gmm.SwitchInterface("BuildMode")
end

function OpenInventory()
	gmm.ToggleInventory()
end

function ChangeUserStringData(name,vname)
	userData.SetArgValue(name,vname)
end

function ChangeUserTileAction(actionName)
	ifgr.ActivateBuildModeAction(actionName)
end