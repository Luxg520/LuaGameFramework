using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Swift
{
    /// <summary>
    /// 网络节点
    /// </summary>
    public class Peer
    {
        // 有链接断开
        public event Action<NetConnection, string> OnDisconnected = null;

        // 开始网络监听
        public void StartListen(string localAddr, int port)
        {
            running = true;
            TcpListener l = new TcpListener(IPAddress.Parse(localAddr), port);
            l.Start();
            ls.Add(l);
            l.BeginAcceptSocket(HandleAccepted, l);
        }

        // 停止网络监听，并断开所有网络连接
        public void Stop()
        {
            running = false;

            // 清理监听
            foreach (TcpListener l in ls)
                l.Stop();

            ls.Clear();

            // 清理连接
            NetConnection[] arr = null;
            lock (connections)
            {
                arr = connections.ToArray();
                connections.Clear();
            }

            foreach (NetConnection c in arr)
                c.Close("stop peer");
        }

        // 同步连接到指定网络节点，返回该连接 ID
        public NetConnection Connect2Peer(string addr, int port)
        {
            running = true;
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(addr, port);
            client.Blocking = true;
            NetConnection nc = new NetConnection(client);
            lock (connections)
                connections.Add(nc);

            nc.OnDisconnected += HandleDisconnected;
            return nc;
        }

        class AsyncConnectArgs
        {
            public Socket c = null;
            public Action<NetConnection, string> cb = null;
            public NetConnection nc = null;
            public string reason = null;
            public bool done = false;
        }

        // 异步连接到指定网络节点
        public void Connect2Peer(string addr, int port, Action<NetConnection, string> callback)
        {
            running = true;
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Blocking = true;
            AsyncConnectArgs args = new AsyncConnectArgs();
            args.c = client;
            args.cb = callback;

            lock (connecting)
                connecting.Add(args);

            IPAddress[] ips = Dns.GetHostAddresses(addr);
            if (ips.Length == 0)
            {
                client.Close();
                return;
            }
            client.BeginConnect(ips[0], port, HandleConnected, args);
        }
				
		public void Connect2Peer(string addr, int port, AddressFamily addrFamily, Action<NetConnection, string> callback)
		{
			running = true;
			Socket client = new Socket(addrFamily, SocketType.Stream, ProtocolType.Tcp);
			client.Blocking = true;
			AsyncConnectArgs args = new AsyncConnectArgs();
			args.c = client;
			args.cb = callback;
			
			lock (connecting)
				connecting.Add(args);
			
//			IPAddress[] ips = Dns.GetHostAddresses(addr);
//			if (ips.Length == 0)
//			{
//				client.Close();
//				return;
//			}
			client.BeginConnect(addr, port, HandleConnected, args);
		}

        // 获取所有连接对象
        public NetConnection[] AllConnections
        {
            get
            {
                lock (connections)
                    return connections.ToArray();
            }
        }

        public void ProcessDoSend()
        {
            NetConnection[] arr = AllConnections;
            foreach (NetConnection conn in arr)
                    conn.DoSend();
        }

        // 处理等待中的连接操作
        public void ProcessPendingConnecting()
        {
            lock (connecting)
            {
                if (connecting.Count > 0)
                {
                    foreach (AsyncConnectArgs c in connecting.ToArray())
                    {
                        if (c.done)
                        {
                            connecting.Remove(c);
                            c.cb(c.nc, c.reason);
                        }
                    }
                }
            }

            // 处理已经断开的连接
            KeyValuePair<NetConnection, string>[] disArr = null;
            lock (disconnectedNetConnections)
            {
                if (OnDisconnected != null && disconnectedNetConnections.Count > 0)
                    disArr = disconnectedNetConnections.ToArray();

                disconnectedNetConnections.Clear();
            }

            if (disArr != null)
            {
                foreach (KeyValuePair<NetConnection, string> kv in disArr)
                    OnDisconnected(kv.Key, kv.Value);
            }
        }

        #region 保护部分

        // 网络监听器
        volatile bool running = false;
        List<TcpListener> ls = new List<TcpListener>();

        // 所有网络连接对象
        List<NetConnection> connections = new List<NetConnection>();

        // 连接断开
        void HandleDisconnected(NetConnection nc, string reason)
        {
            lock (disconnectedNetConnections)
                disconnectedNetConnections[nc] = reason;

            lock (connections)
                connections.Remove(nc);
        }

        // 网络节点接入
        void HandleAccepted(IAsyncResult ar)
        {
            if (!running)
                return;

            TcpListener l = ar.AsyncState as TcpListener;
            NetConnection nc = null;
            try
            {
                Socket c = l.EndAcceptSocket(ar);
                c.Blocking = true;
                nc = new NetConnection(c);
                lock (connections)
                    connections.Add(nc);
                nc.OnDisconnected += HandleDisconnected;
            }
            catch (Exception ex)
            {
                if (nc != null)
                {
                    lock (connections)
                        connections.Remove(nc);
                }
            }
            finally
            {
                l.BeginAcceptSocket(HandleAccepted, l);
            }
        }

        // 连接到指定节点
        void HandleConnected(IAsyncResult ar)
        {
            if (!running)
                return;

            AsyncConnectArgs args = ar.AsyncState as AsyncConnectArgs;
            Socket client = args.c;

            try
            {
                client.EndConnect(ar);
                NetConnection nc = new NetConnection(client);
                lock (connections)
                    connections.Add(nc);

                nc.OnDisconnected += HandleDisconnected;
                args.nc = nc;
                args.reason = null;
            }
            catch (Exception ex)
            {
                args.nc = null;
                args.reason = ex.Message;
            }

            args.done = true;
        }

        // 所有等待完成的连接操作
        List<AsyncConnectArgs> connecting = new List<AsyncConnectArgs>();

        // 所有等待断链的连接
        Dictionary<NetConnection, string> disconnectedNetConnections = new Dictionary<NetConnection, string>();

        #endregion
    }
}
