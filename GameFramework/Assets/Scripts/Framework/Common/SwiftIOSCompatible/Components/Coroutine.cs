using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Swift
{
    /// <summary>
    /// 协程接口
    /// </summary>
    public interface ICoroutine
    {
        // 协程是否已经结束
        bool Finished
        {
            get;
        }
    }

    // 协程事件等待
    public class EventWaiter
    {
        // 构造器，默认手动重置，不超时
        public EventWaiter()
            : this(false, 0)
        {
        }

        // 构造器，默认不超时
        public EventWaiter(bool autoReset)
            : this(autoReset, 0)
        {
        }

        // 构造器，指明是自动还是手动重置和超时时间（<=0表示永远不超时）
        public EventWaiter(bool autoReset, int timeout)
        {
            arst = autoReset;
            interval = timeout;
            t = interval;
        }

        // 是否已经超时
        public bool Expired
        {
            get
            {
                return expired;
            }
        }

        // 处理时间流逝
        public void TimeElapsed(int te)
        {
            if (interval <= 0 || expired)
                return;

            t -= te;
            expired = (t <= 0);
        }

        // 是否自动重置
        public bool IsAutoReset
        {
            get
            {
                return arst;
            }
        }

        // 是否已经触发
        public bool IsSet
        {
            get
            {
                return set;
            }
        }

        // 触发事件
        public void Set()
        {
            set = true;
        }

        // 重制事件
        public void Reset()
        {
            set = false;
            t = interval;
        }

        // 是否已经触发
        bool set = false;

        // 是否是自动重置
        bool arst = false;

        // 是否已经超时
        bool expired = false;

        // 超时间隔时间
        int interval = 0;

        // 超时倒计时
        int t = 0;
    }

    // 利用协程进行异步时间等待
    public class TimeWaiter
    {
        // 构造器，指明等待事件，单位 ms
        public TimeWaiter(int time)
        {
            t = time;
        }
        
        // 剩余等待时间
        public int t = 0;
    }

    // 利用协程等待指定帧数
    public class FrameWaiter
    {
        // 构造器，指明要等待的帧数
        public FrameWaiter(int frame)
        {
            f = frame;
        }

        // 剩余帧数
        public int f = 0;
    }

    // 在协程中等待一个条件结束
    public class ConditionWaiter
    {
        // 构造器，需要给出检查条件
        public ConditionWaiter(Func<bool> handler)
        {
            cch = handler;
        }

        // 条件是否结束
        public bool Finished
        {
            get
            {
                return cch();
            }
        }

        // 条件检查
        Func<bool> cch = null;
    }
    
    // 等待另外一个协程结束
    public class CoroutineWaiter : ConditionWaiter
    {
        // 构造器，指明要等待的协程
        public CoroutineWaiter(ICoroutine coroutine)
            : base(() => { return coroutine.Finished; })
        {
        }
    }

    public class YieldOp
    {
        public YieldOp(IEnumerator e)
        {
            this.e = e;
            firstTime = true;
        }

        public object Current()
        {
            return e.Current;
        }

        public bool MoveNext()
        {
            return this.e.MoveNext();
        }

		IEnumerator e;
		#pragma warning disable 414
        bool firstTime;
		#pragma warning restore 414
    }

    /// <summary>
    /// 协程管理器
    /// </summary>
    public class CoroutineManager : Component, IFrameDrived
    {
        /// <summary>
        /// 协程
        /// </summary>
        class Coroutine : ICoroutine
        {
            // 构造器，需要提供对应的迭代器
            public Coroutine(IEnumerator enumerator)
            {
                //e = enumerator;
                op = new YieldOp(enumerator);
            }

            // 推动协程
            public void Next(int te)
            {
                this.C = op.Current();

                if ((C is TimeWaiter) && ((TimeWaiter)C).t > 0)   // 等待时间
                    ((TimeWaiter)C).t -= te;
                else if ((C is FrameWaiter) && ((FrameWaiter)C).f > 0)    // 等待帧数
                    ((FrameWaiter)C).f--;
                else if ((C is ConditionWaiter) && !((ConditionWaiter)C).Finished)  // 条件等待
                    return;
                else if (C is EventWaiter)  // 事件等待
                {
                    EventWaiter ew = (EventWaiter)C;
                    if (!ew.IsSet && !ew.Expired)
                    {
                        ew.TimeElapsed(te);
                        return;
                    }
                    else
                    {
                        finished = !op.MoveNext();
                        if (ew.IsAutoReset)
                            ew.Reset();
                    }
                }
                else if ((C is Coroutine) && !((Coroutine)C).Finished) // 如果还有子协程，则先等子协程处理完
                    return;
                else
                    finished = !op.MoveNext();
            }

            // 协程是否已经结束
            public bool Finished
            {
                get
                {
                    return finished;
                }
                set
                {
                    finished = value;
                }
            }

            // 要执行的迭代器
            //IEnumerator e = null;
            YieldOp op = null;
            // 结束标记
            bool finished = false;

            object C = null; // e.Current
			
			#pragma warning disable 414
			bool firstTime = true; // 首次，即还未 Next 过（JS使用）
			#pragma warning restore 414
        }
        
        // 终止一个协程
        public void Stop(ICoroutine c)
        {
            coroutineList.Remove((Coroutine)c);
        }

        // 启动一个协程
        public ICoroutine Start(IEnumerator e)
        {
            return StartCoroutineInternal(e, false);
        }

        // 启动一个协程，第二参数表示是否立刻执行，false 表示下一帧开始执行
        public ICoroutine StartCoroutineInternal(IEnumerator e, bool runImmediately)
        {
            Coroutine c = new Coroutine(e);
            coroutineList.Add(c);

            if (runImmediately)
                c.Next(0);

            return c;
        }

        // 推动所有协程
        public void OnTimeElapsed(int te)
        {
            removed.Clear();

            // 推动每个协程
            Coroutine[] list = coroutineList.ToArray();
            foreach (Coroutine c in list)
            {
                if (removed.Contains(c))
                    continue;

                if (!c.Finished)
                    c.Next(te);

                if (c.Finished)
                    removed.Add(c);
            }

            // 把该移除的移除
            foreach (Coroutine c in removed)
                coroutineList.Remove(c);
        }

        // 所有协程
        List<Coroutine> coroutineList = new List<Coroutine>();
        List<Coroutine> removed = new List<Coroutine>();
    }
}
