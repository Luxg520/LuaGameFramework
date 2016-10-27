using UnityEngine;
using System.Collections;
using LuaInterface;

public class LuaBehaviour : MonoBehaviour
{
    // Lua表
    public LuaTable table;

    // 添加Lua组件
    public static LuaTable Add(GameObject go, LuaTable tableClass)
    {
        LuaFunction fun = tableClass.GetLuaFunction("New");
        if (fun == null) 
            return null; 
        object[] rets = fun.Call (tableClass);
        if (rets.Length != 1) 
            return null;
        LuaBehaviour lb = go.AddComponent<LuaBehaviour>(); 
        lb.table = (LuaTable)rets[0]; 
        lb.CallAwake (); 
        return lb.table;
    }

    // 获取lua组件
    public static LuaTable Get(GameObject go, LuaTable table)
    {
        LuaBehaviour[] lbs = go.GetComponents<LuaBehaviour>();
        foreach (LuaBehaviour lb in lbs)
        { 
            string mat1 = table.ToString(); 
            string mat2 = lb.table.GetMetaTable().ToString();
            if (mat1 == mat2)
            { 
                return lb.table; 
            } 
        } 
        return null;
    }

    void CallAwake()
    {
        LuaFunction fun = table.GetLuaFunction("Awake");
        if (fun != null)
            fun.Call(table, gameObject);
    }

    void Start()
    {
        LuaFunction fun = table.GetLuaFunction("Start");
        if (fun != null)
            fun.Call(table, gameObject);
    }

    void Update()
    {
        // 效率问题有待测试和优化 
        // 可在lua中调用UpdateBeat替代 
        LuaFunction fun = table.GetLuaFunction("Update");
        if (fun != null) fun.Call(table, gameObject);
    }
}
