using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Swift
{
    /// <summary>
    /// 写缓冲
    /// </summary>
    public class WriteBuffer : IWriteableBuffer, IReservableBuffer
    {
        public WriteBuffer()
            : this(false)
        {
        }

        public WriteBuffer(bool writeToNetOrder)
        {
            WriteToNetOrder = writeToNetOrder;
        }

        // 读操作是否执行从本地序到网络序的转换
        public bool WriteToNetOrder
        {
            get;
            set;
        }

        // 获取裸的字节数据
        public byte[] Data
        {
            get
            {
                return data;
            }
        }

        // 有效数据长度
        public int Available
        {
            get
            {
                return wPos;
            }
        }

        // 清空缓冲区
        public void Clear()
        {
            wPos = 0;
        }

        #region 写入操作

        public int Reserve(int cnt)
        {
            MakeSureCapacity(cnt);
            wPos += cnt;
            return wPos - cnt;
        }

        public void Unreserve(int id, byte[] src)
        {
            Array.Copy(src, 0, data, id, src.Length);
        }

        public void Write(byte v)
        {
            MakeSureCapacity(1);
            data[wPos++] = v;
        }

        public void Write(byte[] src, int offset, int cnt)
        {
            MakeSureCapacity(cnt);
            Array.Copy(src, offset, data, wPos, cnt);
            wPos += cnt;
        }

        public void Write(byte[] src)
        {
            Write(src, 0, src.Length);
        }

        public void Write(bool v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(bool[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (bool v in arr)
                    Write(v);
            }
        }

        public void Write(short v)
        {
            Write(BitConverter.GetBytes(WriteToNetOrder ? IPAddress.HostToNetworkOrder(v) : v));
        }

        public void Write(short[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (short v in arr)
                    Write(v);
            }
        }

        public void Write(int v)
        {
            Write(BitConverter.GetBytes(WriteToNetOrder ? IPAddress.HostToNetworkOrder(v) : v));
        }

        public void Write(int[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (int v in arr)
                    Write(v);
            }
        }

        public void Write(long v)
        {
            Write(BitConverter.GetBytes(WriteToNetOrder ? IPAddress.HostToNetworkOrder(v) : v));
        }

		public long PeekLong(int offset)
		{
			long v = BitConverter.ToInt64(data, offset);
			return WriteToNetOrder ? IPAddress.NetworkToHostOrder(v) : v;
		}

        public void Write(long[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (long v in arr)
                    Write(v);
            }
		}
		public void Write(ulong _v)
		{
			long v = (long)_v;
			Write(BitConverter.GetBytes(WriteToNetOrder ? IPAddress.HostToNetworkOrder(v) : v));
		}
		
		public void Write(ulong[] arr)
		{
			if (arr == null)
				Write(-1);
			else
			{
				Write(arr.Length);
				foreach (ulong v in arr)
					Write(v);
			}
		}

        public void Write(float v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(float[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (float v in arr)
                    Write(v);
            }
        }

        public void Write(double v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(double[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (double v in arr)
                    Write(v);
            }
        }

        public void Write(char v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(char[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (char v in arr)
                    Write(v);
            }
        }

        public void Write(string v)
        {
            if (v == null)
                Write(-1);
            else
            {
                byte[] arr = ASCIIEncoding.UTF8.GetBytes(v);
                Write(BitConverter.GetBytes(WriteToNetOrder ? IPAddress.HostToNetworkOrder(arr.Length) : arr.Length));
                Write(arr);
            }
        }

        public void Write(string[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                foreach (string v in arr)
                    Write(v);
            }
        }

        public void Write(ISerializable v)
        {
            if (v == null)
                Write(false);
            else
            {
                Write(true);
                v.Serialize(this);
            }
        }

        public void Write(ISerializable[] arr)
        {
            if (arr == null)
                Write(-1);
            else
            {
                Write(arr.Length);
                for (int i = 0; i < arr.Length; i++)
                    Write(arr[i]);
            }
        }

        #endregion

        #region 保护部分

        // 存放裸数据的字节数组
        byte[] data = new byte[1024];

        // 当前写位置
        int wPos = 0;

        private bool _IsUsing = true;
        public bool IsUsing 
        {
            get { return _IsUsing; }
            set { _IsUsing = value; }
        }

        // 检查并保证可用空间
        void MakeSureCapacity(int cnt)
        {
            // 剩余空间够用
            if (data.Length - wPos >= cnt)
                return;

            // 如果空间不够，则重新分配
            int times = 2;
            while (data.Length * times - wPos < cnt)
                times *= 2;

            byte[] newData = new byte[data.Length * times];
            Array.Copy(data, 0, newData, 0, wPos);
            data = newData;
        }

        #endregion
    }
}
