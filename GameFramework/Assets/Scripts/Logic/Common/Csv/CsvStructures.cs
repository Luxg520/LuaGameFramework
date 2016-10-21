using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swift;

public class CsvBase
{
    public int tid;
    public string name;
    public int getI(string s) { return anyAttrs.getI(s); }
    public bool getB(string s) { return anyAttrs.getB(s); }
    public float getF(string s) { return anyAttrs.getF(s); }
    public string getS(string s) { return anyAttrs.getS(s); }
    public object getObj(string s) { return anyAttrs.getObj(s); }

    public void setI(string s, int v) { anyAttrs.setI(s, v); }
    public void setB(string s, bool v) { anyAttrs.setB(s, v); }
    public void setF(string s, float v) { anyAttrs.setF(s, v); }
    public void setS(string s, string v) { anyAttrs.setS(s, v); }
    public void setObj(string s, object v) { anyAttrs.setObj(s, v); }
    public AnyAttrs GetAnyAttrs() { return anyAttrs; }
    AnyAttrs anyAttrs = new AnyAttrs();
}

public class CsvStructures
{
    public static object CreateByName(string name)
    {
        //switch (name)
        //{

        //}
        return null;
    }
}
