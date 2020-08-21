function DragDescribables(x,y,z)
	mc.DragDescribables(x,y,z)
end

function DragDescribablesPost()
	mc.DescribeAllObjects()
end

function DragDescribablesPre( )
	mc.ResetDescs()
end


function DragBuild(x,y,z)
	mc.BuildDrag(x,y,z)
end

function DragNone(x,y,z)

end

function DragPostPreNone()

end
