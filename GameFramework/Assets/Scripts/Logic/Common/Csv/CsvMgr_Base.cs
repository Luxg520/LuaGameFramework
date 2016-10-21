using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using Swift;

public class CsvMgr_Base
{
    protected static T FindFromDict<T>(int tid, Dictionary<int, object> tbl) where T : class
    {
        object obj;
        if (tbl.TryGetValue(tid, out obj))
            return (obj as T);
        return null;
    }
}
