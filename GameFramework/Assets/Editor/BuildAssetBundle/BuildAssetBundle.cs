﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
/// <summary>
/// 打包AssetBundle工具
/// </summary>
public class BuildAssetBundle : Editor
{
    // 打包指定目录下所有资源AssetBundle
    public static void DoBuildAll(string path)
    {
        // 把Assets/Bundles目录下的所有文件都打成AssetBundle
        List<string> all = Collect(path, new string[] { "*.prefab", "*.jpg" });
        for (int i = 0; i < all.Count; i++)
        {
            Debug.Log(all[i]);
            SetAssetBundleName(all[i]);
        }

        // 如果该文件夹存在则删除
        if (Directory.Exists(ResourceConfig.BuildPath))
            Directory.Delete(ResourceConfig.BuildPath, true);

        // 重新创建一个新的，避免二次打包导致内容不一致
        Directory.CreateDirectory(ResourceConfig.BuildPath);

        // 打包AssetBundle
        BuildPipeline.BuildAssetBundles(ResourceConfig.BuildPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);

        // 刷新一下
        AssetDatabase.Refresh();
    }

    // 设置Bundle名称
    private static void SetAssetBundleName(string path)
    {
        string[] dps = AssetDatabase.GetDependencies(path);
        Debug.Log(string.Format("There are {0} dependencies", dps.Length));
        for (int i = 0; i < dps.Length; i++)
        {
            if (dps[i].Contains(".cs"))
                continue;            
            AssetImporter ai = AssetImporter.GetAtPath(dps[i]);
            if (ai.assetBundleName != null)
            {
                string abName = AssetDatabase.AssetPathToGUID(dps[i]);
                //string abName = dps[i].Replace(ResourceConfig.ResourcePath, "ab-");
                //abName = abName.Replace("/", "-");
                //abName = abName.Remove(abName.LastIndexOf("."));
                Debug.Log(string.Format("{0}，AB名称：{1}", dps[i], abName));
                ai.assetBundleName = abName;
                ai.assetBundleVariant = "ab";
            }
        }
    }

    // 收集目录下所有指定扩展名的文件路径
    private static List<string> Collect(string rootPath, string[] exs)
    {
        List<string> allFiles = new List<string>();

        if (!Directory.Exists(rootPath))
        {
            return allFiles;
        }

        foreach (var ex in exs)
        {
            string[] files = Directory.GetFiles(rootPath, ex, SearchOption.AllDirectories);
            foreach (var p in files)
            {
                allFiles.Add(p.Replace("\\", "/"));
            }
        }

        return allFiles;
    }
}
