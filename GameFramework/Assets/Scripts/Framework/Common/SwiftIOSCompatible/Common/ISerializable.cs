using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swift
{
    /// <summary>
    /// 可序列化接口
    /// </summary>
    public interface ISerializable
    {
        void Serialize(IWriteableBuffer w);
        void Deserialize(IReadableBuffer r);
    }
}
