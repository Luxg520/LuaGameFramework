using System;
using System.Collections.Generic;

namespace Swift
{
    /// <summary>
    /// 所有可序列化的数据必须以此基类。任何三列表以外的数据将无法被序列化
    /// </summary>
    public abstract class SerializableData : ISerializable
    {
        protected virtual int IntDataCount { get { return 0; } }
        protected virtual int StrDataCount { get { return 0; } }
        protected virtual int ULongDataCount { get { return 0; } }
        protected virtual int ObjDataCount { get { return 0; } }

        protected virtual int LstDataCount { get { return 0; } }
        protected virtual int IntLstDataCount { get { return 0; } }
        protected virtual int StrLstDataCount { get { return 0; } }
        protected virtual int ULongLstDataCount { get { return 0; } }

        protected virtual int IntIntDicCount { get { return 0; } }
        protected virtual int StrIntDicCount { get { return 0; } }
        protected virtual int StrULongDicCount { get { return 0; } }

        protected int[] intData
        {
            get
            {
                if (_intData == null)
                    _intData = new int[IntDataCount];

                return _intData;
            }
        } int[] _intData = null;

        protected string[] strData
        {
            get
            {
                if (_strData == null)
                    _strData = new string[StrDataCount];

                return _strData;
            }
        } string[] _strData = null;

        protected UInt64[] ulongData
        {
            get
            {
                if (_ulongData == null)
                    _ulongData = new UInt64[ULongDataCount];

                return _ulongData;
            }
        } UInt64[] _ulongData = null;

        protected ISerializable[] objData
        {
            get
            {
                if (_objData == null)
                    _objData = new ISerializable[ObjDataCount];

                return _objData;
            }
        } ISerializable[] _objData = null;

        protected List<ISerializable>[] lstData
        {
            get
            {
                if (_lstData == null)
                {
                    _lstData = new List<ISerializable>[LstDataCount];
                    for (int i = 0; i < LstDataCount; i++)
                        _lstData[i] = new List<ISerializable>();
                }

                return _lstData;
            }
        } List<ISerializable>[] _lstData = null;

        protected List<int>[] intLstData
        {
            get
            {
                if (_intLstData == null)
                {
                    _intLstData = new List<int>[IntLstDataCount];
                    for (int i = 0; i < IntLstDataCount; i++)
                        _intLstData[i] = new List<int>();
                }

                return _intLstData;
            }
        } List<int>[] _intLstData = null;

        protected List<string>[] strLstData
        {
            get
            {
                if (_strLstData == null)
                {
                    _strLstData = new List<string>[StrLstDataCount];
                    for (int i = 0; i < StrLstDataCount; i++)
                        _strLstData[i] = new List<string>();
                }

                return _strLstData;
            }
        } List<string>[] _strLstData = null;

        protected List<UInt64>[] ulongLstData
        {
            get
            {
                if (_ulongLstData == null)
                {
                    _ulongLstData = new List<UInt64>[ULongLstDataCount];
                    for (int i = 0; i < ULongLstDataCount; i++)
                        _ulongLstData[i] = new List<ulong>();
                }

                return _ulongLstData;
            }
        } List<UInt64>[] _ulongLstData = null;

        protected Dictionary<int, int>[] iiDictData
        {
            get
            {
                if (_iiDictData == null)
                {
                    _iiDictData = new Dictionary<int, int>[IntIntDicCount];
                    for (int i = 0; i < IntIntDicCount; i++)
                        _iiDictData[i] = new Dictionary<int,int>();
                }

                return _iiDictData;
            }
        } Dictionary<int, int>[] _iiDictData = null;

        protected Dictionary<string, int>[] siDictData
        {
            get
            {
                if (_siDictData == null)
                {
                    _siDictData = new Dictionary<string, int>[StrIntDicCount];
                    for (int i = 0; i < StrIntDicCount; i++)
                        _siDictData[i] = new Dictionary<string, int>();
                }

                return _siDictData;
            }
        } Dictionary<string, int>[] _siDictData = null;

        protected Dictionary<string, ulong>[] suDictData
        {
            get
            {
                if (_suDictData == null)
                {
                    _suDictData = new Dictionary<string, ulong>[StrULongDicCount];
                    for (int i = 0; i < StrULongDicCount; i++)
                        _suDictData[i] = new Dictionary<string, ulong>();
                }

                return _suDictData;
            }
        }Dictionary<string, ulong>[] _suDictData = null;

        // 反序列化时，需要额外的函数帮助确定到底要创建什么类型的对象
        protected virtual Func<ISerializable>[] ObjDataCreators
        {
            get
            {
                return null;
            }
        }

        // 反序列化时，需要额外的函数帮助确定到底要创建什么类型的对象
        protected virtual Func<ISerializable>[] LstDataCreators
        {
            get
            {
                return null;
            }
        }

        // 深度克隆一份数据
        public virtual T Clone<T>() where T : SerializableData, new()
        {
            RingBuffer buff = new RingBuffer();
            Serialize(buff);
            T obj = new T();
            obj.Deserialize(buff);
            return obj;
        }

        public virtual void Serialize(IWriteableBuffer buff)
        {
            buff.Write(intData);
            buff.Write(strData);
            buff.Write(ulongData);

            buff.Write(objData.Length);
            for (int i = 0; i < objData.Length; i++)
                buff.Write(objData[i]);

            buff.Write(intLstData.Length);
            for (int i = 0; i < intLstData.Length; i++)
                buff.Write(intLstData[i].ToArray());

            buff.Write(ulongLstData.Length);
            for (int i = 0; i < ulongLstData.Length; i++)
                buff.Write(ulongLstData[i].ToArray());

			buff.Write(strLstData.Length);
			for (int i = 0; i < strLstData.Length; i++)
				buff.Write(strLstData[i].ToArray());

            buff.Write(lstData.Length);
            for (int i = 0; i < lstData.Length; i++)
                buff.Write(lstData[i].ToArray());

            buff.Write(iiDictData.Length);
            for (int i = 0; i < iiDictData.Length; i++)
            {
                Dictionary<int, int> dict = iiDictData[i];
                int[] ks = new int[dict.Count];
                int[] vs = new int[dict.Count];
                dict.Keys.CopyTo(ks, 0);
                dict.Values.CopyTo(vs, 0);

                buff.Write(ks);
                buff.Write(vs);
            }

            buff.Write(siDictData.Length);
            for (int i = 0; i < siDictData.Length; i++)
            {
                Dictionary<string, int> dict = siDictData[i];
                string[] ks = new string[dict.Count];
                int[] vs = new int[dict.Count];
                dict.Keys.CopyTo(ks, 0);
                dict.Values.CopyTo(vs, 0);

                buff.Write(ks);
                buff.Write(vs);
            }

            buff.Write(suDictData.Length);
            for (int i = 0; i < suDictData.Length; i++)
            {
                Dictionary<string, ulong> dict = suDictData[i];
                string[] ks = new string[dict.Count];
                ulong[] vs = new ulong[dict.Count];
                dict.Keys.CopyTo(ks, 0);
                dict.Values.CopyTo(vs, 0);

                buff.Write(ks);
                buff.Write(vs);
            }
        }

        public virtual void Deserialize(IReadableBuffer data)
        {
            int[] intArr = data.ReadIntArr();
            intArr.CopyTo(intData, 0);

            string[] strArr = data.ReadStringArr();
            strArr.CopyTo(strData, 0);

            UInt64[] ulongArr = data.ReadULongArr();
            ulongArr.CopyTo(ulongData, 0);

            int cnt = 0;

            // objs
            cnt = data.ReadInt();
            if (cnt > ObjDataCount)
                throw new Exception("ObjDataCount overflow");

            for (int i = 0; i < cnt; i++)
                objData[i] = data.Read(ObjDataCreators[i]);

            // int list
            cnt = data.ReadInt();
            if (cnt > IntLstDataCount)
                throw new Exception("IntLstDataCount overflow");

            for (int i = 0; i < cnt; i++)
            {
                intLstData[i] = new List<int>();
                intArr = data.ReadIntArr();
                intLstData[i].Clear();
                intLstData[i].AddRange(intArr);
            }

            // ulong list
            cnt = data.ReadInt();
            if (cnt > ULongLstDataCount)
                throw new Exception("UlongDataCount overflow");

            for (int i = 0; i < cnt; i++)
            {
                ulongLstData[i] = new List<UInt64>();
                ulongArr = data.ReadULongArr();
                ulongLstData[i].Clear();
                ulongLstData[i].AddRange(ulongArr);
            }

            // string list
            cnt = data.ReadInt();
            if (cnt > StrLstDataCount)
                throw new Exception("StrDataCount overflow");

            for (int i = 0; i < cnt; i++)
            {
                strLstData[i] = new List<string>();
                strArr = data.ReadStringArr();
                strLstData[i].Clear();
                strLstData[i].AddRange(strArr);
            }

            // object list
            cnt = data.ReadInt();
            if (cnt > LstDataCount)
                throw new Exception("LstDataCount overflow");

            for (int i = 0; i < cnt; i++)
            {
                lstData[i] = new List<ISerializable>();
                ISerializable[] objArr = data.ReadArr(LstDataCreators[i]);
                lstData[i].Clear();
                lstData[i].AddRange(objArr);
            }

            // int-int dict
            cnt = data.ReadInt();
            for (int i = 0; i < cnt; i++)
            {
                int[] ks = data.ReadIntArr();
                int[] vs = data.ReadIntArr();
                Dictionary<int, int> dict = new Dictionary<int, int>();
                for (int j = 0; j < ks.Length; j++)
                    dict[ks[j]] = vs[j];

                iiDictData[i] = dict;
            }

            // string-int dict
            cnt = data.ReadInt();
            for (int i = 0; i < cnt; i++)
            {
                string[] ks = data.ReadStringArr();
                int[] vs = data.ReadIntArr();
                Dictionary<string, int> dict = new Dictionary<string, int>();
                for (int j = 0; j < ks.Length; j++)
                    dict[ks[j]] = vs[j];

                siDictData[i] = dict;
            }

            // string-ulong dict
            cnt = data.ReadInt();
            for (int i = 0; i < cnt; i++)
            {
                string[] ks = data.ReadStringArr();
                ulong[] vs = data.ReadULongArr();
                Dictionary<string, ulong> dict = new Dictionary<string, ulong>();
                for (int j = 0; j < ks.Length; j++)
                    dict[ks[j]] = vs[j];

                suDictData[i] = dict;
            }
        }

        protected ISerializable MakeSureObjIsNotNull(int pos)
        {
            if (objData[pos] == null)
                objData[pos] = ObjDataCreators[pos]();
            return objData[pos];
        }
    }

    /// <summary>
    /// 带 UniqueID 和 TID 的可序列化对象
    /// </summary>
    public abstract class SerializableDataWithID : SerializableData
    {
        public virtual UInt64 UniqueID
        {
            get
            {
                return uid;
            }
            set
            {
                uid = value;
            }
        } protected UInt64 uid;

        public virtual int TID
        {
            get
            {
                return tid;
            }
            set
            {
                tid = value;
            }
        } protected int tid;

        public override void Serialize(IWriteableBuffer buff)
        {
            buff.Write(UniqueID);
            buff.Write(TID);
            base.Serialize(buff);
        }

        public override void Deserialize(IReadableBuffer data)
        {
            UniqueID = data.ReadULong();
            TID = data.ReadInt();
            base.Deserialize(data);
        }
    }
}
