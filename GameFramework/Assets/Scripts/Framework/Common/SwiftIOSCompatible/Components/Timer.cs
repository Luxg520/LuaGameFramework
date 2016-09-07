using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swift
{
    /// <summary>
    /// 定时器组件，定时器事件会在主线程回调回来
    /// </summary>
    public class Timer : Component, IFrameDrived
    {
        // 增加一个定时器事件。name 必须唯一，fireTime 表示下一次触发时间点，interval 表示循环触发间隔，如果 interval 为 0，则表示是单次触发
        public void AddEvent(string name, long fireTime, long interval, Action<string> callback)
        {
            if (events.ContainsKey(name))
                throw new Exception("Timer event name conflict: " + name);

            events[name] = new TimerEvent(name, fireTime, interval, callback);
        }

        // 移除一个定时器事件
        public void RemoveEvent(string name)
        {
            if (events.ContainsKey(name))
            {
                events.Remove(name);
            }
        }

        // 处理时间流逝
        public void OnTimeElapsed(int te)
        {
            if (events.Count == 0)
                return;

            long now = Utils.Now;
            TimerEvent[] es = events.Values.ToArray();
            foreach (TimerEvent e in es)
            {
                if (now >= e.fireTime)
                {
                    e.callback(e.name);
                    if (e.interval == 0)
                        events.Remove(e.name);
                    else
                        e.fireTime = now + e.interval;
                }
            }
        }

        #region 保护部分

        // 定时器事件
        class TimerEvent
        {
            // 定时器名称
            public string name = null;

            // 下一次触发时间点
            public long fireTime = 0;

            // 事件重复的时间间隔，如果为 0 则表示是单次事件
            public long interval = 0;

            // 定时器回调
            public Action<string> callback = null;

            // 构造器
            public TimerEvent(string n, long t, long i, Action<string> h)
            {
                name = n;
                fireTime = t;
                interval = i;
                callback = h;
            }
        }

        // 所有定时器事件
        Dictionary<string, TimerEvent> events = new Dictionary<string, TimerEvent>();

        #endregion
    }
}
