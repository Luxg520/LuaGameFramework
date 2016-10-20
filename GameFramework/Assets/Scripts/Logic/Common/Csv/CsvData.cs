using System.Collections;
using System.Collections.Generic;

public class CsvRawData
{
    public string name;
    Dictionary<int, string> dict;

    // 有数据的最大 Col 和 Row
    public int maxC, maxR;

    public CsvRawData(string name)
    {
        this.name = name;
        dict = new Dictionary<int, string>();
        marked = new Dictionary<int, bool>();
        maxC = maxR = 0;
    }
    int makeKey(int r, int c)
    {
        return r * 100000 + c;
    }
    public void set(int r, int c, string value)
    {
        if (!string.IsNullOrEmpty(value)) // 空串就不存了。
        {
            dict.Add(makeKey(r, c), value);
            if (r > maxR) maxR = r;
            if (c > maxC) maxC = c;
        }
    }
    public string get(int r, int c)
    {
        // 找不到返回空串，不会返回null
        string v;
        if (!dict.TryGetValue(makeKey(r, c), out v))
            v = string.Empty;
        return v;
    }

    // 提供一个标记功能，外部随意使用
    // 现在是用于标记某个单元格已经使用过了
    Dictionary<int, bool> marked;
    public void clearMarked()
    {
        marked.Clear();
    }
    public bool isMarked(int r, int c)
    {
        bool v;
        if (!marked.TryGetValue(makeKey(r, c), out v))
            v = false;
        return v;
    }
    public void mark(int r, int c)
    {
        if (marked.ContainsKey(makeKey(r, c)))
            marked.Remove(makeKey(r, c));
        marked.Add(makeKey(r, c), true);
    }
}