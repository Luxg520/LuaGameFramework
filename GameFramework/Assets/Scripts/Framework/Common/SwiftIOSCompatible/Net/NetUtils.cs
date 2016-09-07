using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swift
{
    public sealed class NetUtils
    {
		// 暂时没别的好办法，因为 conn == null 的时候，已经无法走到 NetCore 里面去了，只能在这里进行事件通知，
		// 要将这个事件和 NetCore 中的事件完全统一起来，可能需要大调网络部分的结构。
		public static Action<bool> OnRequestExpired = null;

        // 发送消息给指定连接列表
        public static void Broadcast(string op, string toComponent, Action<IWriteableBuffer> fun, Connection[] toConnection, Connection excludeConn)
        {
            WriteBuffer tmpBuff = new WriteBuffer(true);
            if (fun != null)
                fun(tmpBuff);

            foreach (Connection c in toConnection)
            {
                if (c == null || c == excludeConn)
                    continue;

                IWriteableBuffer buff = c.BeginSend(toComponent);
                buff.Write(op);
                buff.Write(tmpBuff.Data, 0, tmpBuff.Available);
                c.End(buff);
            }
        }

        // 发送消息给指定连接列表
        public static void Broadcast(string op, string toComponent, Action<IWriteableBuffer> fun, Connection c)
        {
            if (c == null)
                return;

            IWriteableBuffer buff = c.BeginSend(toComponent);
            buff.Write(op);
            if (fun != null)
                fun(buff);
            c.End(buff);
        }

        // 发送消息给指定连接列表
        public static void Send(string op, string toComponent, Action<IWriteableBuffer> fun, params Connection[] toConnection)
        {
            Broadcast(op, toComponent, fun, toConnection, null);
        }

        // 发送请求给指定连接列表
		public static void Request(string op, string toComponent, Action<IWriteableBuffer> fun, Action<IReadableBuffer> callback, Connection c)
        {
			if (c == null || !c.IsConnected)
			{
				if (OnRequestExpired != null)
					OnRequestExpired(false);

				return;
			}

			IWriteableBuffer buff = c.BeginRequest(toComponent, callback, null);
            buff.Write(op);
            if (fun != null)
                fun(buff);
			c.End(buff);
        }

        // 发送请求给指定连接列表
        public static void Request(string op, string toComponent, Action<IWriteableBuffer> fun, Action<IReadableBuffer> callback, Action<bool> onExpired, Connection c)
        {
			if (c == null || !c.IsConnected)
			{
				if (onExpired != null)
					onExpired(false);

				return;
			}

            IWriteableBuffer buff = c.BeginRequest(toComponent, callback, onExpired);
            buff.Write(op);
            if (fun != null)
                fun(buff);
            c.End(buff);
        }
    }
}
