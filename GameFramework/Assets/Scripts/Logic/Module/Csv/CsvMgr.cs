using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Swift;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CsvMgr : CsvMgr_Base
{
    public static bool isLoaded = false;
    public static void Load()
    {
        // 只能加载一次表数据
        if (isLoaded)
        {
            Debug.LogError("请勿重复加载表！");
            return;
        }

        CsvParser.CsvPath = ResourceConfig.ConfigPath;

        // 编辑器下需拷贝一份再加载，否则正在编辑状态会报错
#if UNITY_EDITOR        

        var sb = new System.Text.StringBuilder();
        List<string> filesToCopy = new List<string>();
        filesToCopy.AddRange(CsvParser.arrCsvName);
        filesToCopy.AddRange(CsvParser.arrTxtName);
        foreach (var p in filesToCopy)
        {
            string src = ResourceConfig.ConfigPath + p;
            string dest = ResourceConfig.TempPath + "Config/" + p;
            Directory.CreateDirectory(Path.GetDirectoryName(dest));
            File.Copy(src, dest, true);
            sb.AppendLine(string.Format("Copy: \"{0}\" -> \"{1}\"", src, dest));
        }
        Debug.Log(sb);
        // 必须要 Refresh 一下，否则第一次拷贝后，Resources.LoadAssetAtPath 会失败
        AssetDatabase.Refresh();
        CsvParser.CsvPath = ResourceConfig.TempPath + "Config";

#endif
        // 解析表
        CsvParser.Load();

        isLoaded = true;
    }

}
