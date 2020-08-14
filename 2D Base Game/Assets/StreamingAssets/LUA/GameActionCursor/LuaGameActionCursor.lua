function ChangeCameraPos(V)
	MouseController.UpdateCameraPos(cursorAction.positionToStore - V)
end

function SetStoredCursorVal(V)
	cursorAction.positionToStore = V
end

function ChangeZoomVal(V)
	MouseController.UpdateZoom("Mouse ScrollWheel")
end

function SetDraggedArea(V)
	ifgr.SetActiveTiles(MouseController.GetDraggedArea(cursorAction.positionToStore,V))
end

function BuildFloorType(V)
	SetDraggedArea(V)
	igfr.SetChangeTileType(userData.Tiles, userData.userSelectedStringID)
end

function DisplayDragArea(V)
	SetDraggedArea(V)
	MouseController.PreviewImageOverTiles(userData.Tiles,MouseController.previewTile)
end

function ActivateTileAction()
	ifgr.ActivateCurrentTileFunction()
end

