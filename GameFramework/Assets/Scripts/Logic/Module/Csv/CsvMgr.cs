using UnityEngine;
using System.Collections;
using Swift;
public class CsvMgr
{
    public static bool isLoaded = false;
    public static void Load()
    {
        // 只能加载一次表数据
        if (isLoaded)
            return;

        CsvParser.CsvPath = ResourceConfig.ConfigPath;

        // 编辑器下需拷贝一份再加载，否则正在编辑状态会报错
#if UNITY_EDITOR
        CsvParser.CsvPath = ResourceConfig.TempPath + "Config";

#endif
        // 解析表
        CsvParser.Load();

        isLoaded = true;
    }

}
