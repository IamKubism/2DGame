function OpenDoor(furna, f)
  
  open = f*furna.GetComponentFloat("openning") + furna.GetComponentFloat("openness")
  
	if ( furna.GetComponentFloat("openness") <= 1 and furna.GetComponentFloat("openness") >= 0 ) then
		furna.ChangeComponentFloat("openness", open)
	end
	
	if (furna.GetComponentFloat("openness") > 1) then
	  furna.ChangeComponentFloat("openness", 1)
	end
	
	if (furna.GetComponentFloat("openness") < 0) then
	  furna.ChangeComponentFloat("openness", 0)
	end
	
end