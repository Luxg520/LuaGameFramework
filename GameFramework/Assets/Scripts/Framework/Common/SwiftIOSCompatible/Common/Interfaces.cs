using System;
using System.Collections;

namespace Swift
{
	/// <summary>
	/// 日志接口
	/// </summary>
	public interface ILog
	{
		// 日志信息
		void Info(string str);
		
		// 日志错误
		void Error(string str);

        // 警告
        void Warn(string str);

        // 调试
        void Debug(string str);
	}
	
	/// <summary>
	/// 帧驱动接口
	/// </summary>
	public interface IFrameDrived
	{
		// 处理时间流逝，参数为上一帧到这一帧之间流逝的时间（毫秒）
		void OnTimeElapsed(int te);
	}
}
