require "event"

GameCore = {}
OnInitilized = event("OnInitilized", true)

function GameCore:New(o)
	o = {}
    setmetatable(o, self)  
    self.__index = self  
	return o
end  

function GameCore:GetInstance()
	if self.m_pGameCore == nil then
		self.m_pGameCore = self:New()
	end
	return self.m_pGameCore
end

function GameCore:ReleaseInstance()
	if self.m_pGameCore then
		self.m_pGameCore = nil
	end
end

-- 初始化
function GameCore:Init()
	print("启动游戏")
	print("正在初始化组件...")
	
	NetCore
	
	OnInitilized:Add(
	function()
		print("加载了某个组件");
	end,self)
	
	-- 初始化完成
	if OnInitilized:Count() ~= 0 then 
		OnInitilized()
	end
	
	print("开始游戏")
end

function Test()

	print("加载了某个组件")

end