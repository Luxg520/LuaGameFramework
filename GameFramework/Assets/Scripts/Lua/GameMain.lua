require "GameCore"

function Init()
	-- 游戏核心
	local go = UnityEngine.GameObject ('GameMain') 
	LuaBehaviour.Add(go,GameCore) 
	local gcLb = LuaBehaviour.Get(go,GameCore) 
	gcLb.name = "GameCore"	
end