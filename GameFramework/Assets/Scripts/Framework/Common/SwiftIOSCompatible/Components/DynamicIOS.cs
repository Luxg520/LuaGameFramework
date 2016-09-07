using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Swift
{
    /// <summary>
    /// 提供一些动态特性的功能
    /// </summary>
    public class DynamicIOS
    {
        // 获取指定名称的类型对象
        public static Type GetTypeByName(string typeName)
        {
            Assembly asm = GetAssemblyByType(typeName);
            return asm.GetType(typeName);
        }

        // 查找指定类型所在的程序集
        public static Assembly GetAssemblyByType(string typeName)
        {
            Assembly[] asmArr = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in asmArr)
            {
                if (asm.GetType(typeName) != null)
                    return asm;
            }

            return null;
        }

        // 检查给定对象是否有指定名称的方法
        public static bool HasMethod(object obj, string fun)
        {
            MethodInfo mi = obj.GetType().GetMethod(fun, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return mi != null;
        }

        // 动态执行给定对象的指定方法，并考虑不同参数的重载问题
        public static object Invoke(object obj, string fun, object[] args)
        {
            MethodInfo[] mis = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < mis.Length; i++)
            {
                // 要依次匹配名称、参数数量和每个参数的类型

                MethodInfo mi = mis[i];

                if (mi.Name != fun)
                    continue;

                ParameterInfo[] pis = mi.GetParameters();

                if (args == null)
                {
                    if (pis.Length != 0)
                        continue;
                }
                else if (args.Length != pis.Length)
                    continue;

                object[] argsArr = pis.Length == 0 ? null : new object[pis.Length];
                for (int j = 0; j < pis.Length; j++)
                {
                    ParameterInfo pi = pis[j];
                    object arg = args[j];

                    if (arg == null)
                    {
                        if (!pi.ParameterType.IsClass)
                        {
                            mi = null;
                            break;
                        }
                        else
                            argsArr[j] = arg;
                    }
                    else
                    {
                        Type argType = arg.GetType();
                        if (!pi.ParameterType.IsAssignableFrom(argType))
                        {
                            mi = null;
                            break;
                        }
                        else
                            argsArr[j] = arg;
                    }
                }

                if (mi != null)
                {
                    try
                    {
                        return mi.Invoke(obj, argsArr);
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

            throw new Exception("No such method with matched parameters");
        }

        // 将字符串转换成给定类型的值
        public static object ParseBaseTypeValueFromString(string str, Type t)
        {
            if (t == typeof(bool))
                return bool.Parse(str);
            else if (t == typeof(byte))
                return byte.Parse(str);
            else if (t == typeof(char))
                return str[0];
            else if (t == typeof(ushort))
                return ushort.Parse(str);
            else if (t == typeof(short))
                return short.Parse(str);
            else if (t == typeof(uint))
                return uint.Parse(str);
            else if (t == typeof(int) || t.IsEnum)
                return int.Parse(str);
            else if (t == typeof(ulong))
                return ulong.Parse(str);
            else if (t == typeof(long))
                return long.Parse(str);
            else if (t == typeof(float))
                return float.Parse(str);
            else if (t == typeof(double))
                return double.Parse(str);
            else if (t == typeof(string))
                return str;
            else
                return null;
        }

        // 动态执行给定对象的指定方法，并考虑不同参数的重载问题
        public static object InvokeWithStringArgs(object obj, string fun, string[] args)
        {
            MethodInfo[] mis = obj.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < mis.Length; i++)
            {
                MethodInfo mi = mis[i];

                if (mi.Name != fun)
                    continue;

                ParameterInfo[] pis = mi.GetParameters();

                if (args == null)
                {
                    if (pis.Length != 0)
                        continue;
                }
                else if (args.Length != pis.Length)
                    continue;

                object[] argsArr = pis.Length == 0 ? null : new object[pis.Length];
                for (int j = 0; j < pis.Length; j++)
                {
                    ParameterInfo pi = pis[j];
                    string arg = args[j];
                    argsArr[j] = ParseBaseTypeValueFromString(arg, pi.ParameterType);
                }

                if (mi != null)
                    return mi.Invoke(obj, argsArr);
            }

            throw new Exception("No such method with matched parameters");
        }

        // 动态设置对象的成员
        public static void SetField(object obj, string field, object value)
        {
            FieldInfo fi = obj.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(obj, value);
        }

        // 动态获取对象的成员
        public static T GetField<T>(object obj, string field)
        {
			Type t = obj.GetType();
			FieldInfo fi = null;
			while (fi == null && t != typeof(object))
			{
				fi = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
				if (fi == null)
					t = t.BaseType;
			}

            return fi == null ? default(T) : (T)fi.GetValue(obj);
        }

        // 动态设置对象的属性
        public static void SetProperty(object obj, string field, object value)
        {
            PropertyInfo pi = obj.GetType().GetProperty(field, BindingFlags.NonPublic | BindingFlags.Instance);
            pi.SetValue(obj, value, null);
        }

        // 动态获取对象的属性
        public static T GetProperty<T>(object obj, string field)
        {
            PropertyInfo pi = obj.GetType().GetProperty(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)pi.GetValue(obj, null);
        }
    }
}
