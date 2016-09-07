using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swift
{
    // 可读缓冲区
    public interface IReadableBuffer
    {
        bool ReadFromNetOrder
        {
            get;
            set;
        }
        int Available
        {
            get;
        }
        void Skip(int cnt);
        bool PeekInt(ref int v);
        byte ReadByte();
        byte[] ReadBytes(int cnt);
        bool ReadBool();
        bool[] ReadBoolArr();
        short ReadShort();
        short[] ReadShortArr();
        int ReadInt();
        int[] ReadIntArr();
        long ReadLong();
		long[] ReadLongArr();
		ulong ReadULong();
		ulong[] ReadULongArr();
        float ReadFloat();
        float[] ReadFloatArr();
        double ReadDouble();
        double[] ReadDoubleArr();
        char ReadChar();
        char[] ReadCharArr();
        string ReadString();
        string[] ReadStringArr();
        void TravelReplaceBytes4Read(int offsetFromReadPos, int len, Func<byte, byte> fun);
//        T Read<T>() where T : class, ISerializable, new();
//        T[] ReadArr<T>() where T : class, ISerializable, new();
//        ISerializable Read(Func<ISerializable> creator);
//        ISerializable[] ReadArr(Func<ISerializable> creator);
    }

    // 可写缓冲区
    public interface IWriteableBuffer
    {
        bool WriteToNetOrder
        {
            get;
            set;
        }
        int Available
        {
            get;
        }
        void Write(byte[] src, int offset, int cnt);
        void Write(byte[] src);
        void Write(byte v);
        void Write(bool v);
        void Write(bool[] v);
        void Write(short v);
        void Write(short[] v);
        void Write(int v);
        void Write(int[] v);
        void Write(long v);
		void Write(long[] v);
		void Write(ulong v);
		void Write(ulong[] v);
        void Write(float v);
        void Write(float[] v);
        void Write(double v);
        void Write(double[] v);
        void Write(char v);
        void Write(char[] v);
        void Write(string v);
        void Write(string[] v);
        void Write(ISerializable v);
        void Write(ISerializable[] v);
    }

    // 可保留空间的缓冲区
    public interface IReservableBuffer
    {
        int Reserve(int cnt);
        void Unreserve(int id, byte[] src);
    }
}
