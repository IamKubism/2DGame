function ActivateRPGMode()
	gmm.SwitchInterface("RPGMode")
end

function ActivateBuildMode()
	gmm.SwitchInterface("BuildMode")
end

function SpawnItem()
	gmm.CreateSpawnMenu()
end

function PostDrag()
	mc.DescribeAllObjects()
end