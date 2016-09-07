using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Swift
{
    /// <summary>
    /// 网络连接对象
    /// </summary>
    public class NetConnection
    {
        // 连接断开事件(该事件可能在不同的线程中被回调回来)
        public event Action<NetConnection, string> OnDisconnected = null;

        // 判断是否处于连接状态
        public bool IsConnected
        {
            get
            {
                lock (c)
                    return c.Connected;
            }
        }

        // 构造器，指明对应的 tcp 对象
        public NetConnection(Socket client)
        {
            client.NoDelay = true;
            c = client;

            // 立刻启动数据接收
            try
            {
                if (!c.Connected)
                    return;

                c.BeginReceive(rd, 0, rd.Length, SocketFlags.None, OnReceived, null);
            }
            catch (Exception ex)
            {
                Close(ex.Message);
            }
        }

        public void DoSend()
        {
            lock (c)
            {
                if (sb.Available > MaxSendBufferSize)
                    Close("message send buffer overflow");
                else if (!isSending)
                    SendAndSwap();
            }
        }

        // 写入数据
        public void SendData(byte[] data, int offset, int cnt)
        {
            lock (c)
            {
                sb.Write(data, offset, cnt);
            }
        }

        // 通信加密密钥
        public byte[] EncryptCode
        {
            get
            {
                return encryptCode;
            }
            set
            {
                encryptCode = value;
                encryptCodeOffset = 0;
            }
        } byte[] encryptCode = null;

        // 通信解密密钥
        public byte[] DecryptCode
        {
            get
            {
                return encryptCode;
            }
            set
            {
                decryptCode = value;
                decryptCodeOffset = 0;
            }
        } byte[] decryptCode = null;

        // 加密一个字节
        public byte Encrypt(byte data)
        {
            if (encryptCode == null || encryptCode.Length == 0)
                return data;

            byte c = encryptCode[encryptCodeOffset % encryptCode.Length];
            encryptCodeOffset++;
            return (byte)(data ^ c);
        }

        // 解密一个字节
        public byte Decrypt(byte data)
        {
            if (decryptCode == null || decryptCode.Length == 0)
                return data;

            byte c = decryptCode[decryptCodeOffset % decryptCode.Length];
            decryptCodeOffset++;
            return (byte)(data ^ c);
        }

        // 加密密钥偏移
        int encryptCodeOffset = 0;

        // 解密密钥偏移
        int decryptCodeOffset = 0;

        // 接收到的数据缓冲
        public IReadableBuffer ReceivedData
        {
            get
            {
                lock (c)
                {
                    if (receivingBuffer.Available > 0)
                    {
                        rb.Write(receivingBuffer.Data, 0, receivingBuffer.Available);
                        receivingBuffer.Clear();
                    }
                }

                return rb;
            }
        }

        // 关闭连接
        public void Close(string reason)
        {
            lock (c)
                c.Close();

            if (OnDisconnected != null)
                OnDisconnected(this, reason);
        }

		// 每连接的最大发送消息缓冲区尺寸，默认 512k
        public static int MaxSendBufferSize
        {
            get
            {
                return maxSendBufferSize;
            }
            set
            {
                maxSendBufferSize = value;
            }
		} static int maxSendBufferSize = 512 * 1024;

		// 每连接的最大接收消息缓冲区尺寸，默认 512k
        public static int MaxRecieveBufferSize
        {
            get
            {
                return maxRecieveBufferSize;
            }
            set
            {
                maxRecieveBufferSize = value;
            }
		} static int maxRecieveBufferSize = 512 * 1024;

        #region 保护部分

        // 标记是否在发送中
        volatile bool isSending = false;

        // 对应的 tcp 对象
        Socket c = null;
        int sendingOffset = 0;
        public string RemoteAddress
        {
            get
            {
				if (remoteAddress == null)
					remoteAddress = ((IPEndPoint)c.RemoteEndPoint).Address.ToString();

                return remoteAddress;
            }
        } string remoteAddress = null;

        // 数据发送缓冲区，两个缓冲区用来交换发送，sendingBuffer 是已经投递发送的数据，sb 是正在写入，要等待投递发送的数据
        WriteBuffer sb = new WriteBuffer(true);
        WriteBuffer sendingBuffer = new WriteBuffer(true);

        // 数据接收缓冲区，一个裸的字节数组用于投递接收请求，将接收到的数据存入缓冲区
        byte[] rd = new byte[1024];
        WriteBuffer receivingBuffer = new WriteBuffer(false);
        RingBuffer rb = new RingBuffer(true, false);

        // 发送数据
        void SendAndSwap()
        {
            // 没数据可发送
            lock (c)
            {
                if (sb.Available == 0 || !c.Connected)
                {
                    isSending = false;
                    return;
                }

                // 交换缓冲
                WriteBuffer tmp = sendingBuffer;
                sendingBuffer = sb;
                sendingOffset = 0;
                tmp.Clear();
                sb = tmp;

                isSending = true;
                try
                {
                    c.BeginSend(sendingBuffer.Data, 0, sendingBuffer.Available, SocketFlags.None, OnSent, null);
                }
                catch (Exception ex)
                {
                    Close(ex.Message);
                }                
            }
        }

        // 发送回调
        void OnSent(IAsyncResult ar)
        {
            try
            {
                lock (c)
                {
                    int cnt = c.EndSend(ar);
                    if (cnt <= 0)
                        Close("remote terminal closed");
                    else
                    {
                        sendingOffset += cnt;
                        if (sendingOffset < sendingBuffer.Available)
                            c.BeginSend(sendingBuffer.Data, sendingOffset, sendingBuffer.Available - sendingOffset, SocketFlags.None, OnSent, null);
                        else
                        {
                            if (sb.Available > 0)
                                SendAndSwap();
                            else
                                isSending = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Close(ex.Message);
            }
        }

        // 接收回调
        void OnReceived(IAsyncResult ar)
        {
            try
            {
                lock (c)
                {
                    int cnt = c.EndReceive(ar);
                    if (cnt <= 0)
                        Close("remote terminal closed");
                    else
                    {
                        receivingBuffer.Write(rd, 0, cnt);
                        if (receivingBuffer.Available > maxRecieveBufferSize)
                            Close("message recieve buffer overflow");
                        else
                            c.BeginReceive(rd, 0, rd.Length, SocketFlags.None, OnReceived, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Close(ex.Message);
            }    
        }

        #endregion
    }
}
