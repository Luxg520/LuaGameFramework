using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Swift;

public class WordFilter : Component
{
    bool banWordLoaded = false;
    bool validCharacterLoaded = false;

    HashSet<char> validChineseSet;
    FTree commonFtree = new FTree();
    FTree nameFTree = new FTree();

    public virtual void Load()
    {
        LoadBanWords();
    }

    public Func<string, byte[]> BinaryLoader = (string path) => { return File.ReadAllBytes(path); };
    public string ConfigPath = "config";
    public void LoadBanWords()
    {
        if (banWordLoaded)
            return;

        banWordLoaded = true;
        byte[] bytes = BinaryLoader(ConfigPath + "/ban_words.txt");
        string text = Encoding.UTF8.GetString(bytes);
        string[] words = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        FTree tree = null;
        foreach (string word in words)
        {
            string str = word.Trim();
            if (string.IsNullOrEmpty(str))
                continue;

            if (str[0] == '[')
            {
                if (str == "[名字]")
                    tree = nameFTree;
                else
                    tree = commonFtree;
            }

            if (tree != null)
                tree.InsertWord(str);
        }
    }

    protected virtual void LoadValidChinese()
    {
        if (validCharacterLoaded)
            return;

        validCharacterLoaded = true;        
        byte[] bytes = BinaryLoader(ConfigPath + "/valid_chinese.txt");
        string text = Encoding.UTF8.GetString(bytes);
        validChineseSet = new HashSet<char>();
        for (int i = 0; i < text.Length; ++i)
            validChineseSet.Add(text[i]);
    }

    // 是否是有效字符
//    public bool IsValidChar(char ch)
//    {
//        LoadValidChinese();
//        return validChineseSet.Contains(ch);
//    }

    // 字符串中是否全部是有效字符
//    public bool IsValidText(string text)
//    {
//        if (text == null)
//            return false;
//
//        LoadValidChinese();
//
//        for (int i = 0; i < text.Length; ++i)
//        {
//            if (!IsValidChar(text[i]))
//                return false;
//        }
//
//        return true;
//    }

    // 过滤合法字符
//    public string FiltrateInvalidText(string text)
//    {
//        if (string.IsNullOrEmpty(text))
//            return string.Empty;
//
//        StringBuilder sb = new StringBuilder();
//        for (int i = 0; i < text.Length; ++i)
//        {
//            if (IsValidChar(text[i]))
//                sb.Append(text[i]);
//        }
//
//        return sb.ToString();
//    }

    // 过滤字符串的敏感词，并用*代替，返回过滤后的字符串
    public string FilterSensitiveWord(string text)
    {
        return commonFtree.Process(text);
    }

    // 名字专用：名字是否包含敏感词或无效名字
    public bool NameHasSensitiveWord(string text)
    {
        return !nameFTree.Validate(text) || !commonFtree.Validate(text);
    }

    // 名字以外使用：是否包含敏感词
    public bool HasSensitiveWord(string text)
    {
        return !commonFtree.Validate(text);
    }
}
