using System;
using System.Collections.Generic;

public class FTreeHelper
{
    public static FNode BinaryInsert(ushort key, List<FNode> childs)
    {
        FNode node = null;
        if (childs.Count == 0)
        {
            node = new FNode(key);
            childs.Add(node);
            return node;
        }

        int l = 0;
        int r = childs.Count - 1;
        int m = childs.Count / 2;

        while (l <= r)
        {
            if (key > childs[m].key) l = m + 1;
            else if (key < childs[m].key) r = m - 1;
            else return childs[m];

            m = (l + r) / 2;
        }

        node = new FNode(key);
        if (key > childs[m].key)
            childs.Insert(m + 1, node);
        else
            childs.Insert(m, node);

        return node;
    }

    public static FNode BinarySearch(int key, List<FNode> childs)
    {
        if (childs.Count == 0)
            return null;

        int l = 0;
        int r = childs.Count - 1;
        int m = childs.Count / 2;

        while (l <= r)
        {
            if (key > childs[m].key) l = m + 1;
            else if (key < childs[m].key) r = m - 1;
            else return childs[m];

            m = (l + r) / 2;
        }

        return null;
    }
}

public class FNode
{
    public List<FNode> childs = null;
    public ushort key;
    public bool terminated; // 是否找到数据

    public FNode(ushort key)
    {
        this.key = key;
        this.terminated = false;
    }

    public FNode InsertChild(ushort key)
    {
        if (childs == null)
            childs = new List<FNode>();

        return FTreeHelper.BinaryInsert(key, childs);
    }

    public FNode BinarySearch(ushort key)
    {
        if (childs == null)
            return null;

        return FTreeHelper.BinarySearch(key, childs);
    }
}

public class FTree
{
    public List<FNode> childs = new List<FNode>();

    public void InsertWord(string word)
    {
        if (string.IsNullOrEmpty(word))
            return;

        char[] chs = word.ToCharArray();

        int index = 0;
        FNode nextNode = null;
        while (index < chs.Length)
        {
            ushort key = (ushort)chs[index++];

            if (nextNode == null)
                nextNode = FTreeHelper.BinaryInsert(key, childs);
            else
                nextNode = nextNode.InsertChild(key);
        }

        nextNode.terminated = true;
    }

    public bool Validate(string word)
    {
        if (string.IsNullOrEmpty(word))
            return true;

        char[] chs = word.ToCharArray();

        int index = 0;
        while (index < chs.Length)
        {
            int newIndex = 0;
            if (Process(chs, index, ref newIndex))
                return false;
            else
                index++;
        }

        return true;
    }

    public string Process(string word)
    {
        if (string.IsNullOrEmpty(word))
            return "";

        char[] chs = word.ToCharArray();

        int index = 0;
        while (index < chs.Length)
        {
            int newIndex = 0;
            if (Process(chs, index, ref newIndex))
                while (index < newIndex)
                    chs[index++] = '*';
            else
                index++;
        }

        return new string(chs);
    }

    public bool Process(char[] chs, int offset, ref int result)
    {
        FNode nextNode = null;
        result = -1;

        int index = offset;
        while (index < chs.Length)
        {
            ushort key = (ushort)chs[index++];

            if (nextNode == null)
                nextNode = FTreeHelper.BinarySearch(key, childs);
            else
                nextNode = nextNode.BinarySearch(key);

            if (nextNode == null)
                break;

            if (nextNode.terminated)
                result = index;
        }

        return result != -1;
    }
}