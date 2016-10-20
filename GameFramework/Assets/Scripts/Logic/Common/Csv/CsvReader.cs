using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

    // 读取.csv文件
    // 仅负责解析出每个单元格的文本
    //
    // 打开csv时的选择：
    // 1) 字符集：Unicode (UTF-8)
    // 2) 分隔选项/分隔/逗号 打勾
    // 3) 文字分隔符选"
public class CsvReader
{
    static readonly string LINE_SERERATOR = "\r\n";
    static readonly char SEPERATOR = ',';
    static readonly char QUOTE = '\"';

    public static Func<string, string> funcReadFile = (path) => { return File.ReadAllText(path); };

    static int string_CharCount(string str, char c)
    {
        int count = 0;
        for (var i = 0; i < str.Length; i++)
        {
            if (str[i] == c)
                count++;
        }
        return count;
    }

    // 检查 str 是否符合一个格子里字符串的格式
    // 如果不符合，返回 null
    static string formatCellString(string str)
    {
        if (str == null)
            throw new Exception("str == null");

        int L = str.Length;
        if (L == 0)
            return str;

        if (str[0] != QUOTE)
        {
            // 如果不以"开始，就直接返回str
            return str;
        }

        if (L == 1 || str[L - 1] != QUOTE)
        {
            // 如果最后不是"，不符合规则
            return null;
        }
        if (L == 2)
        {
            // 如果单元格是""，符合规则，返回空串，不过肯定是手改的
            // 正常不会走到这里
            return string.Empty;
        }
        string quotedStr = str.Substring(1, L - 2);
        //int quoteCount = quotedStr.ToCharArray().Where((c) => c == QUOTE).ToArray().Length;
        int quoteCount = string_CharCount(quotedStr, QUOTE);
        if ((quoteCount % 2) != 0)
        {
            // 如果引号个数不是偶数，不符合规则
            return null;
        }
        if (quoteCount > 0)
        {
            // "" -> "
            string ret = quotedStr.Replace("\"\"", "\"");            
            return ret;
        }
        return quotedStr;
    }
    public static CsvRawData Load(string path)
    {
        string name = Path.GetFileName(path);
        string content = funcReadFile(path);
        return Load(name, content);
    }
    public static CsvRawData Load(string csvName, string content)
    {
        CsvRawData data = new CsvRawData(csvName);

        string[] lines = content.Split(LINE_SERERATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);        
        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row];
            int col = 0;
            string[] strs = line.Split(SEPERATOR);            
            string s = string.Empty;
            foreach (var str in strs)
            {
                s += str;
                string cellString = formatCellString(s);
                if (cellString != null)
                {
                    data.set(row, col++, cellString);
                    s = string.Empty;
                }
                else
                {
                    // 如果不符合，把后面的也串起来
                    s += SEPERATOR;
                }
            }
            if (s.Length != 0)
            {
                throw new Exception("\"" + csvName + "\" has error!");
                //return null;
            }
        }
        return data;
    }
}
