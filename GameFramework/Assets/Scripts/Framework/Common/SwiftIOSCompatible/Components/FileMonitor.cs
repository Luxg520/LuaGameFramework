using System;
using System.Collections.Generic;
using System.IO;
using Swift;

namespace Swift
{
    /// <summary>
    /// 监视磁盘文件，当文件发生变化时产生事件通知
    /// </summary>
    public class FileMonitor : Component, IFrameDrived, IDisposable
    {
        // 文件被创建
        public event Action<string> OnCreated = null;

        // 文件被删除
        public event Action<string> OnDeleted = null;

        // 文件更新
        public event Action<string> OnUpdated = null;

		// 启动监听
        public void Start(string path, string filter, NotifyFilters nfs, bool includeSubDirectories)
        {
            wt.NotifyFilter = nfs;
            wt.IncludeSubdirectories = includeSubDirectories;
            wt.Path = Path.Combine(Environment.CurrentDirectory, path);
            wt.Filter = filter;
            wt.EnableRaisingEvents = true;

            wt.Created += (object sender, FileSystemEventArgs e) =>
            {
                wt.EnableRaisingEvents = false;
                lock(createEvents)
                    createEvents.Add(e.FullPath);
                wt.EnableRaisingEvents = true;
            };

            wt.Deleted += (object sender, FileSystemEventArgs e) =>
            {
                wt.EnableRaisingEvents = false;
                lock (deleteEvents)
                    deleteEvents.Add(e.FullPath);
                wt.EnableRaisingEvents = true;
            };

            wt.Changed += (object sender, FileSystemEventArgs e) =>
            {
                wt.EnableRaisingEvents = false;
                lock (updateEvents)
                    updateEvents.Add(e.FullPath);
                wt.EnableRaisingEvents = true;
            };
        }

        // 通过 IFrameDrived 接口将异步的事件回调转换成同步事件回调
        public void OnTimeElapsed(int te)
        {
            lock (createEvents)
            {
                if (OnCreated != null && createEvents.Count > 0)
                {
                    foreach (string fullPath in createEvents)
                        OnCreated(fullPath.Replace(wt.Path + Path.DirectorySeparatorChar, ""));
                }

                createEvents.Clear();
            }

            lock (deleteEvents)
            {
                if (OnDeleted != null && deleteEvents.Count > 0)
                {
                    foreach (string fullPath in deleteEvents)
                        OnDeleted(fullPath.Replace(wt.Path + Path.DirectorySeparatorChar, ""));
                }

                deleteEvents.Clear();
            }

            lock (updateEvents)
            {
                if (OnUpdated != null && updateEvents.Count > 0)
                {
                    foreach (string fullPath in updateEvents)
                        OnUpdated(fullPath.Replace(wt.Path + Path.DirectorySeparatorChar, ""));
                }

                updateEvents.Clear();
            }
        }

        public void Dispose()
        {
            wt.Dispose();
        }

        #region 保护部分

        // 文件监控器
        FileSystemWatcher wt = new FileSystemWatcher();

        // 监控事件列表
        List<string> createEvents = new List<string>();
        List<string> deleteEvents = new List<string>();
        List<string> updateEvents = new List<string>();

        #endregion
    }
}
