using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Swift;
/// <summary>
/// 网络管理器
/// </summary>
public class NetManager : ManagerBase<NetManager>
{
    // 网络超时事件
    public event Action<bool> OnRequestExpired = null;

    // 收到网络消息事件
    public event Action OnMessageRecieved = null;

    // 网络核心组件
    private NetCore NC = null;

    // 当前服务器连接对象
    private Connection srvConn = null;
    public Connection CurrentServerConnection
    {
        get
        {
            return srvConn;
        }
    } 

    // 初始化
    public override void Init()
    {
        base.Init();

        NC = GameCore.Instance.Get<NetCore>();
        NC.OnRequestExpired += NotifyRequestExpired;
        NC.OnMessageRecieved += NotifyMessageRecieved;
        NetUtils.OnRequestExpired += NotifyRequestExpired;        
    }

    // 网络请求超时通知
    public void NotifyRequestExpired(bool connected)
    {
        if (OnRequestExpired != null)
            OnRequestExpired(connected);
    }

    // 收到网络消息通知
    public void NotifyMessageRecieved()
    {
        if (OnMessageRecieved != null)
            OnMessageRecieved();
    }

    // 连接服务器
    public void ConnectServer(string ip, int port, Action<Connection, string> callback)
    {
        NC.Close();                

        Debug.Log("ConnectServer " + ip + ":" + port);

#if UNITY_IPHONE && !UNITY_EDITOR
		String newServerIp = "";

		AddressFamily newAddressFamily = AddressFamily.InterNetwork;
		IPv6SupportMidleware.getIPType(ip, port.ToString(), out newServerIp, out newAddressFamily);

		UnityEngine.Debug.Log ("ConnectServer use IPv6 ? " + (newAddressFamily == AddressFamily.InterNetworkV6 ? "Yes" : "No"));

		NC.Connect2Peer(newServerIp, port, newAddressFamily, (Connection conn, string reason) => {
			ResetAllConnection(conn);
			callback(conn, reason);
		});
#else
        NC.Connect2Peer(ip, port, (Connection conn, string reason) =>
        {
            ResetAllConnection(conn);
            callback(conn, reason);
        });
#endif
    }

    // 关闭网络连接
    public void CloseNetConnections()
    {
        NC.Close();        
    }

    // 等待服务器连接成功后，设置好所有网络相关模块的连接对象
    private void ResetAllConnection(Connection conn)
    {
        srvConn = conn;
        PortAgent[] pas = GameCore.Instance.Gets<PortAgent>();
        for (int i = 0; i < pas.Length; i++)
        {
            PortAgent pa = pas[i];
            pa.Setup(conn);
        }
    }

}
