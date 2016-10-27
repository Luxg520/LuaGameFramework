
GameCore =
{
	-- 初始化完成
	OnInitilized = {}	
}


-- 初始化
function GameCore:Init()
	print("启动游戏")
	print("正在初始化组件...")
	
	OnInitilized = Test
	
	-- 初始化完成
	if OnInitilized ~= nil then 
		OnInitilized()
	end
	
	print("开始游戏")
end

function GameCore:Test()

	print("加载了某个组件")

end

function GameCore:Update()

	print("Update测试");

end

--创建对象
function GameCore:New(obj)
	local o = {} 
    setmetatable(o, self)  
    self.__index = self  
	return o
end  