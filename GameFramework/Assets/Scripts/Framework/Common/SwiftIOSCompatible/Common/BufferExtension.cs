using System;
using System.Collections;

namespace Swift
{
    public static class BufferExtension
    {
        public static T Read<T>(this IReadableBuffer r) where T : class, ISerializable, new()
        {
            bool noNull = r.ReadBool();
            if (!noNull)
                return default(T);
            
            T t = new T();
            t.Deserialize(r);
            return t;
        }
        
        public static T[] ReadArr<T>(this IReadableBuffer r) where T : class, ISerializable, new()
        {
            int len = r.ReadInt();
			if (len == -1)
				return null;

            T[] arr = new T[len];
            for (int i = 0; i < len; i++)
                arr[i] = r.Read<T>();
            
            return arr;
        }

        public static ISerializable Read(this IReadableBuffer r, Func<ISerializable> creator)
        {
            bool noNull = r.ReadBool();
            if (!noNull)
                return null;
            if (creator == null)
                return null;
            ISerializable obj = creator();
            obj.Deserialize(r);
            return obj;
        }
        
        public static ISerializable[] ReadArr(this IReadableBuffer r, Func<ISerializable> creator)
        {
            int cnt = r.ReadInt();
			if (cnt == -1)
				return null;

            ISerializable[] arr = new ISerializable[cnt];
            for (int i = 0; i < cnt; i++)
                arr[i] = r.Read(creator);
            
            return arr;
        }
    }

}