require "GameCore"

function Init()
	
	--local go = UnityEngine.GameObject ('GameMain') 
	--LuaBehaviour.Add(go,GameCore) 
	--local gcLb = LuaBehaviour.Get(go,GameCore) 
	--gcLb.name = "GameCore"	
	
	-- 初始游戏核心
	GameCore:GetInstance():Init()
	
	-- UpdateBeat:Add(Update,self)
end