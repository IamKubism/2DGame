function BuildFurniture(job)
	job.dest_tile.world.PlaceFurniture(job.GetComponentString("furniture_type"), job.dest_tile)
end

function DoNothing(job)
	
end

function SetTileType(job)
	job.dest_tile.TileType = MainGame.instance.TileTypes[job.GetComponentString("tile_type")]
end