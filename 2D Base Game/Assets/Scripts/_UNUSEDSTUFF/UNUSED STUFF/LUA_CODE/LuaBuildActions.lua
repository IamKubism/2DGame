function BuildModeStone()
	bmc.SetFloorMode("Stone_Rough")
end

function BuildModeEmpty()
	bmc.SetFloorMode("Empty")
end

function BuildWall(type)
	bmc.SetMode_BuildInstalledObject(type .. "wall")
end

function BuildFurniture()
	gmm.OpenFurnitureWindow()
end

function OpenInventory()
	gmm.ToggleInventory()
end

function FurnitureJob(t)

end