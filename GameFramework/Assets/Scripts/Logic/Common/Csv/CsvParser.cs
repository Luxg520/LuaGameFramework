using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class CsvParser
{
    #region 常用变量

    private const string ENUMID = "Enum:";
    private const string VALUEID = "Value";
    private const string TABLEID = "Table:";

    // 表路径
    public static string CsvPath;

    // 正在处理的表名
    public static string parsingCsvName;  

    // csv表文件名
    public static string[] arrCsvName = 
    {
        "def.csv",
    };

    // txt文件名
    public static string[] arrTxtName =
    {
        "version.txt",
    };

    public static Dictionary<int, object> Items { get; private set; }    

    #endregion

    #region 解析与加载

    // 加载
    public static void Load()
    {
        dictRawData = new Dictionary<string, CsvRawData>();
        dictEnum = new Dictionary<string, object>();
        dictTables = new Dictionary<string, object>();

        // 解析def.csv表
        LoadDefRawData();
        AnalyzeDefCsv();

        // 解析其他所有csv表
        LoadAllRawDataExceptDef();   
        AnalyzeAllCsvsExceptDef();   

        // 解析剧情
        ParseConversation();

        // 初始化变量
        InitVariables();

        // 额外步骤
        ExtraSteps();

        // 清理内存
        dictRawData.Clear();
        System.GC.Collect();

    }

    #endregion

    #region 1 rawdata

    // 所有的csv数据
    // 举例："def.csv" -> CsvRawData
    private static Dictionary<string, CsvRawData> dictRawData;

    // name 举例：def.csv
    private static CsvRawData getCsvRawData(string name)
    {
        return dictRawData[name];
    }

    private static void LoadDefRawData()
    {
        string p = arrCsvName[0];
        CsvRawData rd = CsvReader.Load(CsvPath + "/" + p);
        dictRawData.Add(rd.name, rd);
    }

    // 加载所有csv的文本
    private static void LoadAllRawDataExceptDef()
    {
        CsvRawData rd = null;

        for (int i = 1 /* 跳过 def.csv */ ; i < arrCsvName.Length; i++)
        {
            string p = arrCsvName[i];
            rd = CsvReader.Load(CsvPath + "/" + p);
            dictRawData.Add(rd.name, rd);
        }
    }

    #endregion

    #region 2 Enum解析

    // 枚举名 -> (枚举项 -> 值)
    private static Dictionary<string, object> dictEnum;

    public static int getEnumValue(string enumName, string key)
    {
        if (dictEnum.ContainsKey(enumName))
        {
            Dictionary<string, int> d = (Dictionary<string, int>)dictEnum[enumName];
            if (d.ContainsKey(key))
            {
                return d[key];
            }
            else
            {
                // 枚举可以不填，默认0
                if (string.IsNullOrEmpty(key))
                    return 0;
                else
                    throw new Exception("In " + parsingCsvName + ": enum value not found: " + enumName + "." + key);
            }
        }
        else
        {
            throw new Exception("In " + parsingCsvName + ": enum not found: " + enumName);
        }
    }

    // 解析def表
    // 现在只存储枚举
    public static void AnalyzeDefCsv()
    {
        parsingCsvName = arrCsvName[0];
        CsvRawData rd = getCsvRawData(arrCsvName[0]);
        rd.clearMarked();

        string enumName = null;
        Dictionary<string, int> enumValues = null;
        bool specifyValue = false; // 用另一列指定枚举值
        int n = 0; // 没指定枚举值时，递增

        // 逐列解析
        // 为啥要+1？+1才可以到后来取到一个空串，得知已结束。
        for (int c = 0; c <= rd.maxC + 1; c++)
        {
            for (int r = 0; r <= rd.maxR + 1; r++)
            {
                string cell = rd.get(r, c);
                bool empty = string.IsNullOrEmpty(cell);
                bool isEnumID = !empty && cell.StartsWith(ENUMID);

                if (empty || isEnumID)
                {
                    if (!string.IsNullOrEmpty(enumName) && enumValues != null && enumValues.Count > 0)
                    {
                        dictEnum.Add(enumName, enumValues);
                    }

                    enumName = null;
                    enumValues = null;
                    specifyValue = false;
                    n = 0;

                    if (isEnumID)
                    {
                        enumName = cell.Substring(ENUMID.Length);
                        enumValues = new Dictionary<string, int>();
                        specifyValue = rd.get(r, c + 1).StartsWith(VALUEID);
                        if (specifyValue) rd.mark(r, c + 1);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(enumName) || enumValues == null)
                    {
                        if (!rd.isMarked(r, c))
                            throw new Exception("def.csv 配置有误");
                    }
                    else
                    {
                        int N = specifyValue ? int.Parse(rd.get(r, c + 1)) : n++;
                        enumValues.Add(cell, N);
                        if (specifyValue) rd.mark(r, c + 1);
                    }
                }
            }
        }
    }

    #endregion // Enum解析

    #region 3 表格解析

    // 第1个string是 "skill.csv" 或其他
    // 第2个：tid -> object    
    public static Dictionary<string, object> dictTables;

    private static object TryGetObject(string structName, int tid)
    {
        object o;
        if (dictTables.TryGetValue(structName, out o))
        {
            Dictionary<int, object> d = (Dictionary<int, object>)o;
            if (d.TryGetValue(tid, out o))
                return o;
        }
        return null;
    }

    private static void AddObject(string structName, int tid, object obj)
    {
        //			string csvNameR;
        //			if (!csvRedirect.TryGetValue (structName, out csvNameR))
        //				csvNameR = structName;

        Dictionary<int, object> d = null;
        object o;
        if (!dictTables.TryGetValue(structName, out o))
        {
            d = new Dictionary<int, object>();
            dictTables.Add(structName, d);
        }
        else
        {
            d = ((Dictionary<int, object>)o);
        }
        d.Add(tid, obj);
    }

    private static bool SetValue(object obj, string fieldName, object value)
    {
        if (obj != null)
        {
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }
        }
        return false;
    }

    // 设置一个对象的 field 字段值
    // 目前 typeStr 如果有值，表示枚举
    // 如果没值表示使用反射从 obj 取到字段类型。
    private static void SetFieldValue(object obj, string fieldName, string typeStr, string cell)
    {
        int iSharp = fieldName.IndexOf('#');
        if (iSharp == 1) 
        {
            CsvBase cb = (CsvBase)obj;

            char symbol = fieldName[0];
            string field = fieldName.Substring(iSharp + 1);

            if (symbol == 'i')
            {
                int v;
                if (string.IsNullOrEmpty(cell))
                    v = 0;
                else
                {
                    if (!int.TryParse(cell, out v))
                        throw new Exception("In " + parsingCsvName + ": unknown int: " + cell);
                }
                cb.setI(field, v);
            }
            else if (symbol == 'b')
                cb.setB(field, (cell == "1" || cell == "是"));
            else if (symbol == 's')
                cb.setS(field, (string)cell);
            else if (symbol == 'f')
                cb.setF(field, (float)(string.IsNullOrEmpty(cell) ? 0f : float.Parse(cell)));
            else if (symbol == 'e')
            {
                if (string.IsNullOrEmpty(typeStr))
                    throw new Exception("In " + parsingCsvName + ": XXXXXXXXX");
                int v = getEnumValue(typeStr, cell);
                cb.setI(field, v);
            }
            else if (symbol == 'a')
            {
                string[] elems = cell.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int[] arr = new int[elems.Length];
                for (int i = 0; i < elems.Length; i++)
                    arr[i] = int.Parse(elems[i]);

                cb.setObj(field, arr);
            }
            // DateTime
            else if (symbol == 'd')
            {
                cb.setObj(field, ParseDateTime(cell));
            }
            else
                throw new Exception("In " + parsingCsvName + ": Unknown any type: " + symbol);
        }
        else
        {
            object v = null;
            if (string.IsNullOrEmpty(typeStr) || typeStr == "i")
            {
                if (string.IsNullOrEmpty(cell.Trim()))
                    v = 0;
                else
                {
                    int intV = 0;
                    if (!int.TryParse(cell, out intV))
                        throw new Exception("In " + parsingCsvName + ": unknown int: " + cell);
                    else
                        v = intV;
                }
            }
            else if (typeStr == "b")
                v = (bool)(cell == "1" || cell == "是");
            else if (typeStr == "s")
                v = (string)cell;
            else if (typeStr == "f")
                v = (float)(string.IsNullOrEmpty(cell) ? 0f : float.Parse(cell));
            else if (typeStr == "a")
            {
                string[] elems = cell.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int[] arr = new int[elems.Length];
                for (int i = 0; i < elems.Length; i++)
                    arr[i] = int.Parse(elems[i]);

                v = arr;
            }
            // DateTime
            else if (typeStr == "d")
            {
                v = ParseDateTime(cell);
            }
            else
                v = getEnumValue(typeStr, cell);

            if (!SetValue(obj, fieldName, v))
            {
                throw new Exception("In " + parsingCsvName + ": field not found or error type: " + fieldName + ":" + (typeStr == null ? "i" : typeStr));
            }
        }
    }

    // str举例：
    // 2016-1-1
    // 2016-01-01
    // 2016-3-4 8
    // 2016-2-5 08:02
    // 2016-12-1 8:02:05
    private static DateTime ParseDateTime(string str)
    {
        string[] arr = str.Split(' ');
        string[] date = arr[0].Split('-');
        int year = int.Parse(date[0]);
        int month = int.Parse(date[1]);
        int day = int.Parse(date[2]);

        int hour = 0;
        int minute = 0;
        int second = 0;
        if (arr.Length > 1)
        {
            string[] time = arr[1].Split(':');
            hour = int.Parse(time[0]);
            if (time.Length > 1)
                minute = int.Parse(time[1]);
            if (time.Length > 2)
                second = int.Parse(time[2]);
        }

        return new DateTime(year, month, day, hour, minute, second);
    }

    // 分析一个.csv 里的一个部分，这个部分组成一个表
    // （一个.csv里可以写上很多个表，只要按照特定格式组织就可以）
    // csvName      csv文件名
    // rd           整个csv的数据
    // or/oc        子表的表头位置
    // structName   这个子表所使用的结构体名字
    static void AnalyzeATable(CsvRawData rd, int or, int oc, string structName)
    {
        // od下一行是字段名定义
        // 可以考虑改成往下一直找，找到不空为止，现在是必须紧接着才行
        int field_r = or + 1;
        int c = oc;
        List<string> fieldNames = new List<string>(); // 字段名字
        List<string> fieldTypes = new List<string>(); // 不为null则表示枚举名
        // 先查找一共有几列
        // 往右一直找，找到空的就表示结束了
        while (true)
        {
            string cell = rd.get(field_r, c++);
            if (string.IsNullOrEmpty(cell))
            {
                break;
            }
            string[] name_type = cell.Split(':');
            fieldNames.Add(name_type[0]);
            fieldTypes.Add(name_type.Length > 1 ? name_type[1] : null);
        }

        // 再下一行就是表数据了
        int r = or + 2;

        while (true)
        {
            string firstCell = rd.get(r, oc);
            if (string.IsNullOrEmpty(firstCell))
            {
                // 空行就结束
                break;
            }

            if (!firstCell.StartsWith("#")) // 跳过以#开始的行（这一行现在写的是中文的列名，用来看的）
            {
                int tid = int.Parse(firstCell);

                // 注意，如果这个对象已经存在了，接下去可能就覆盖他的字段值
                object obj = TryGetObject(structName, tid);

                bool shouldAdd = false;
                if (obj == null)
                {
                    shouldAdd = true;
                    obj = CsvStructures.CreateByName(structName);
                }

                if (obj == null)
                    throw new Exception("In " + parsingCsvName + ": Table structure not found: " + structName);

                //Type type = obj.GetType();

                for (c = oc; c < oc + fieldNames.Count; c++)
                {
                    // tid又会取一遍，没关系
                    string cell = rd.get(r, c);
                    string fieldName = fieldNames[c - oc];
                    string typeStr = fieldTypes[c - oc];
                    SetFieldValue(obj, fieldName, typeStr, cell);
                }

                if (shouldAdd)
                {
                    AddObject(structName, tid, obj);
                }
            }

            // 下一行
            r++;
        }
    }

    // csvName 仅用于打印
    static void AnalyzeCsv(CsvRawData rd, string csvName)
    {
        string defaultStructName = string.Empty;
        for (int c = 0; c <= rd.maxC + 1; c++)
        {
            for (int r = 0; r <= rd.maxR + 1; r++)
            {
                string cell = rd.get(r, c);
                if (cell.StartsWith(TABLEID))
                {
                    string structName = cell.Substring(TABLEID.Length);
                    if (!string.IsNullOrEmpty(structName) && string.IsNullOrEmpty(defaultStructName))
                    {
                        defaultStructName = structName;
                    }

                    if (string.IsNullOrEmpty(structName))
                        structName = defaultStructName;

                    if (string.IsNullOrEmpty(structName))
                        throw new Exception("In \"" + csvName + "\": structName missing.");

                    AnalyzeATable(rd, r, c, structName);
                }
            }
        }
    }

    static void AnalyzeAllCsvsExceptDef()
    {
        for (int i = 1 /* 跳过 def.csv */; i < arrCsvName.Length; i++)
        {
            parsingCsvName = arrCsvName[i];

            Console.WriteLine(" loading csv: " + parsingCsvName);

            CsvRawData rd = getCsvRawData(parsingCsvName);
            AnalyzeCsv(rd, parsingCsvName);
        }
    }

    #endregion

    #region 4 初始化一些变量，方便使用

    static void InitVariables()
    {
        
    }

    #endregion

    #region 5 剧情文本解析

    // 查找一下个[123]
    // 返回 ] 的索引
    // tid 是中括号里的数字
    static bool FindNextNumInBracket(string text, int start, ref int left, ref int right, ref int tid)
    {
        int l = text.IndexOf('[', start);
        if (l < 0) return false;

        int r = text.IndexOf(']', l + 1);
        if (r < 0) throw new Exception("剧情配置有误，缺少]");

        for (var i = l + 1; i < r - 1; i++)
        {
            if (!char.IsNumber(text[i]))
                throw new Exception("剧情配置有误，[]中间只能放数字");
        }
        tid = int.Parse(text.Substring(l + 1, r - l - 1));
        left = l;
        right = r;
        return true;
    }
    // 去除头尾换行符
    static string TrimLF(string str)
    {
        int L = str.Length;

        int l = 0;
        while (l <= L - 1 && str[l] == '\r' || str[l] == '\n') l++;

        int r = L - 1;
        while (r >= 0 && str[r] == '\r' || str[r] == '\n') r--;

        return str.Substring(l, r - l + 1);
    }
    static void ParseConversation()
    {
        string textName = arrTxtName[0];
        string txtPath = CsvPath + "/" + textName;

        Dictionary<int, object> dict = new Dictionary<int, object>();
        dictTables.Add(textName, dict);

        string text = CsvReader.funcReadFile(txtPath);
        int left = 0, right = 0, tid = 0;
        int leftNext = 0, rightNext = 0, tidNext = 0;
        string conversation;
        bool suc = FindNextNumInBracket(text, 0, ref left, ref right, ref tid);
        while (suc)
        {
            suc = FindNextNumInBracket(text, right + 1, ref leftNext, ref rightNext, ref tidNext);
            if (suc)
            {
                conversation = TrimLF(text.Substring(
                    right + 1,
                    leftNext - right - 1));
            }
            else
            {
                // 文件结束
                conversation = TrimLF(text.Substring(right + 1));
            }
            dict.Add(tid, conversation);

            left = leftNext;
            right = rightNext;
            tid = tidNext;
        }
    }

    #endregion

    #region 6 额外步骤

    private static void ExtraSteps()
    {

    }

    #endregion
}
