using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Swift
{
    /// <summary>
    /// 计时信息
    /// </summary>
    public struct TimerInfo
    {
        public double start;
        public double end;
        public double t
        {
            get
            {
                return end == -1 ? -1 : end - start;
            }
        }
    }

    /// <summary>
    /// 高精度定时器，只能在 windows 平台使用
    /// </summary>
    public class PerformanceCounter : Component
    {
        // 每一轮触发一次
        public event Action<Dictionary<string, TimerInfo>> OnTag = null;

        public PerformanceCounter()
        {
            sw.Reset();
            sw.Start();
        }

        // 标签开始
        public void StartTag(string tag)
        {
            TimerInfo t = new TimerInfo();
            t.start = sw.Elapsed.TotalMilliseconds;
            t.end = -1;
            tags[tag] = t;
        }

        // 记录时间标签
        public void EndTag(string tag)
        {
            TimerInfo t = tags[tag];
            t.end = sw.Elapsed.TotalMilliseconds;
            tags[tag] = t;

            if (OnTag != null)
                OnTag(tags);
        }

        // 清除所有计时标签
        public void Clear()
        {
            tags.Clear();
        }

        #region 保护部分

        // 计时器
        Stopwatch sw = new Stopwatch();

        // 当前计时信息
        Dictionary<string, TimerInfo> tags = new Dictionary<string, TimerInfo>();

        #endregion
    }
}
