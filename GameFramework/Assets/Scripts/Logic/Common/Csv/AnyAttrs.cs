using System;
using Swift;
using System.Collections;
using System.Collections.Generic;

public class AnyAttrs
{
    public int getI(string s)
    {
        return this[s];
    }

    public bool getB(string s)
    {
        return this[s];
    }

    public float getF(string s)
    {
        return this[s];
    }

    public string getS(string s)
    {
        return this[s];
    }

    public object getObj(string s)
    {
        return this[s].GetObj();
    }

    public void setI(string s, int v)
    {
        this[s] = v;
    }

    public void setB(string s, bool v)
    {
        this[s] = v;
    }

    public void setF(string s, float v)
    {
        this[s] = v;
    }

    public void setS(string s, string v)
    {
        this[s] = v;
    }

    public void setObj(string s, object v)
    {
        this[s] = new Any(v);
    }

    public Dictionary<string, Any> GetAttrs() { return attrs; }

    // 额外的附加属性，只支持基本类型
    Any this[string attr]
    {
        get
        {
            return attrs.ContainsKey(attr) ? attrs[attr] : Any.Empty;
        }
        set
        {
            attrs[attr] = value;
        }
    }
    Dictionary<string, Any> attrs
    {
        get
        {
            if (_attrs == null)
                _attrs = new Dictionary<string, Any>();
            return _attrs;
        }
    }
    Dictionary<string, Any> _attrs = null;
}
