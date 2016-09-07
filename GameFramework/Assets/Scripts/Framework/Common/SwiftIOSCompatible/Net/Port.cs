using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swift
{
    /// <summary>
    /// 接口类，用于通信过程中的消息收发
    /// </summary>
    public class Port : NetComponent
    {
        // 注册无需应答的网络消息处理函数
        public void OnMessage(string op, Action<IReadableBuffer> callback)
        {
            MakeSureNoDuplicate(op);
            noResponseMap[op] = callback;
        }

        // 注册无需应答的网络消息处理函数
        public void OnMessage(string op, Action<Connection, IReadableBuffer> callback)
        {
            MakeSureNoDuplicate(op);
            noResponseMapWithConn[op] = callback;
        }

        // 注册需要应答的网络消息处理函数，这一类操作是需要异步完成的
        public void OnRequest(string op, Action<IReadableBuffer, IWriteableBuffer, Action> callback)
        {
            MakeSureNoDuplicate(op);
            responseMap[op] = callback;
        }

        // 注册需要应答的网络消息处理函数，这一类操作是需要异步完成的
        public void OnRequest(string op, Action<Connection, IReadableBuffer, IWriteableBuffer, Action> callback)
        {
            MakeSureNoDuplicate(op);
            responseMapWithConn[op] = callback;
        }

        // 注册需要应答的网络消息处理函数，这一类操作是同步完成的
        public void OnRequet(string op, Action<IReadableBuffer, IWriteableBuffer> callback)
        {
            MakeSureNoDuplicate(op);
            responseMapImmediately[op] = callback;
        }

        // 注册需要应答的网络消息处理函数，这一类操作是同步完成的
        public void OnRequest(string op, Action<Connection, IReadableBuffer, IWriteableBuffer> callback)
        {
            MakeSureNoDuplicate(op);
            responseMapImmediatelyWithConn[op] = callback;
        }

        // 处理网络消息的映射
        public override void OnMessage(Connection conn, IReadableBuffer data)
        {
            string op = data.ReadString();
            if (noResponseMap.ContainsKey(op))
                noResponseMap[op](data);
            else if (noResponseMapWithConn.ContainsKey(op))
                noResponseMapWithConn[op](conn, data);
            else if (responseMap.ContainsKey(op))
            {
                Responser r = CreateResponser(conn);
                IWriteableBuffer buff = r.BeginResponse();
                responseMap[op](data, buff, () => { r.End(buff); });
            }
            else if (responseMapWithConn.ContainsKey(op))
            {
                Responser r = CreateResponser(conn);
                IWriteableBuffer buff = r.BeginResponse();
                responseMapWithConn[op](conn, data, buff, () => { r.End(buff); });
            }
            else if (responseMapImmediately.ContainsKey(op))
            {
                Responser r = CreateResponser(conn);
                IWriteableBuffer buff = r.BeginResponse();
                responseMapImmediately[op](data, buff);
                r.End(buff);
            }
            else if (responseMapImmediatelyWithConn.ContainsKey(op))
            {
                Responser r = CreateResponser(conn);
                IWriteableBuffer buff = r.BeginResponse();
                responseMapImmediatelyWithConn[op](conn, data, buff);
                r.End(buff);
            }
            else
                conn.Close("unhandled message '" + op + "' in " + Name);
        }

        #region 保护部分

        // 检查重复注册的消息映射
        void MakeSureNoDuplicate(string op)
        {
            if (noResponseMap.ContainsKey(op)
                || noResponseMapWithConn.ContainsKey(op)
                || responseMap.ContainsKey(op)
                || responseMapWithConn.ContainsKey(op))
                throw new Exception("message: " + op + " has already register");
        }

        // 无需回复的消息映射表
        Dictionary<string, Action<IReadableBuffer>> noResponseMap = new Dictionary<string, Action<IReadableBuffer>>();
        Dictionary<string, Action<Connection, IReadableBuffer>> noResponseMapWithConn = new Dictionary<string, Action<Connection, IReadableBuffer>>();

        // 需要回复的消息映射表
        Dictionary<string, Action<IReadableBuffer, IWriteableBuffer, Action>> responseMap = new Dictionary<string, Action<IReadableBuffer, IWriteableBuffer, Action>>();
        Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer, Action>> responseMapWithConn = new Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer, Action>>();
        Dictionary<string, Action<IReadableBuffer, IWriteableBuffer>> responseMapImmediately = new Dictionary<string, Action<IReadableBuffer, IWriteableBuffer>>();
        Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer>> responseMapImmediatelyWithConn = new Dictionary<string, Action<Connection, IReadableBuffer, IWriteableBuffer>>();

        #endregion
    }

    /// <summary>
    /// 接口代理类，用于通信过程中的消息收发
    /// </summary>
    public class PortAgent : NetComponentAgent
    {
        // 重制连接 id
        public void Reset(Connection conn)
        {
            Setup(conn, compName);
        }

		// 发送无回执消息
        public void Send(string op, Action<IWriteableBuffer> fun)
        {
            NetUtils.Send(op, compName, fun, conn);
        }

		// 发送请求并等待请求结果
        public void Request(string op, Action<IWriteableBuffer> fun, Action<IReadableBuffer> callback)
        {
            NetUtils.Request(op, compName, fun, callback, conn);
        }

		// 发送请求并等待请求结果
        public void Request(string op, Action<IWriteableBuffer> fun, Action<IReadableBuffer> callback, Action<bool> onExpired)
		{
			NetUtils.Request(op, compName, fun, callback, onExpired, conn);
		}
    }
}
