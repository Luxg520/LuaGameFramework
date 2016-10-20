using System;
using Swift;
using System.Collections;
using System.Collections.Generic;


// 任意数据类型可代表的具体类型
public enum AnyValueType
{
	Int,
	Bool,
	Double,
	String,
    Object
}

// 任意数据类型：仅支持基本类型
public struct Any
{
    public static readonly Any Empty = new Any(null);

    public Any(int i)
    {
        intValue = 0;
        boolValue = false;
        doubleValue = 0;
        strValue = null;
        objValue = null;

        intValue = i;
        type = AnyValueType.Int;
    }

    public Any(bool b)
    {
        intValue = 0;
        boolValue = false;
        doubleValue = 0;
        strValue = null;
        objValue = null;

        boolValue = b;
        type = AnyValueType.Bool;
    }

    public Any(double f)
    {
        intValue = 0;
        boolValue = false;
        doubleValue = 0;
        strValue = null;
        objValue = null;

        doubleValue = f;
        type = AnyValueType.Double;
    }

    public Any(string str)
    {
        intValue = 0;
        boolValue = false;
        doubleValue = 0;
        strValue = null;
        objValue = null;

        strValue = str;
        type = AnyValueType.String;
    }

    public Any(object obj)
    {
        intValue = 0;
        boolValue = false;
        doubleValue = 0;
        strValue = null;
        objValue = null;

        objValue = obj;
        type = AnyValueType.Object;
    }

    public static implicit operator int(Any v)
    {
        return v.type == AnyValueType.Int ? v.intValue : 0;
    }

    public static implicit operator Any(int i)
    {
        return new Any(i);
    }

    public static implicit operator bool(Any v)
    {
        return v.type == AnyValueType.Bool ? v.boolValue : false;
    }

    public static implicit operator Any(bool b)
    {
        return new Any(b);
    }

    public static implicit operator float(Any v)
    {
        return v.type == AnyValueType.Double ? (float)v.doubleValue : 0;
    }

    public static implicit operator double(Any v)
    {
        return v.type == AnyValueType.Double ? v.doubleValue : 0;
    }

    public static implicit operator Any(double f)
    {
        return new Any(f);
    }

    public static implicit operator string(Any v)
    {
        return v.type == AnyValueType.String ? v.strValue : null;
    }

    public static implicit operator Any(string str)
    {
        return new Any(str);
    }

    public static bool operator ==(Any a, Any b)
    {
        if (a.type != b.type)
            return false;
        else
        {
            switch (a.type)
            {
                case AnyValueType.Bool:
                    return a.boolValue == b.boolValue;
                case AnyValueType.Double:
                    return a.doubleValue == b.doubleValue;
                case AnyValueType.String:
                    return a.strValue == b.strValue;
                case AnyValueType.Object:
                    return a.GetObj() == b.GetObj();
                default:
                    return a.intValue == b.intValue;
            }
        }
    }

    public static bool operator !=(Any a, Any b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return (obj is Any) && ((Any)obj == this);
    }

    public override int GetHashCode()
    {
        switch (type)
        {
            case AnyValueType.Bool:
                return boolValue.GetHashCode();
            case AnyValueType.Double:
                return doubleValue.GetHashCode();
            case AnyValueType.String:
                return strValue.GetHashCode();
            case AnyValueType.Object:
                return objValue.GetHashCode();
            default:
                return intValue.GetHashCode();
        }
    }

    public void SetObj(object obj)
    {
        objValue = obj;
        type = AnyValueType.Object;
    }

    public object GetObj()
    {
        return type == AnyValueType.Object ? objValue : null;
    }

    public override string ToString()
    {
        switch (type)
        {
            case AnyValueType.Bool:
                return boolValue.ToString();
            case AnyValueType.Double:
                return doubleValue.ToString();
            case AnyValueType.String:
                return strValue;
            case AnyValueType.Object:
                return objValue == null ? "{NULL}" : objValue.ToString();
            default:
                return intValue.ToString();
        }
    }

    public AnyValueType GetAnyType()
    {
        return type;
    }

    #region 保护部分

        AnyValueType type;

        int intValue;

        bool boolValue;

        double doubleValue;

        string strValue;

        object objValue;

        #endregion
}