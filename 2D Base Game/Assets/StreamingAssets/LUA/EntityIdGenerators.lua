function TileIdGenerator(objects)
	return "tile_" .. objects.GetString("x") .. "_" .. objects.GetString("y") .. "_" .. objects.GetString("z")
end