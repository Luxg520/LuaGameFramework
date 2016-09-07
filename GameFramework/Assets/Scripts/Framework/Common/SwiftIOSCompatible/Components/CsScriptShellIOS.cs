using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Diagnostics;

namespace Swift
{
    public interface ICsScriptShell
    {
        Component GetComponent(string name);
        Object Call(string scriptObjectName, string fun, params object[] args);
    }

    /// <summary>
    /// 脚本组件
    /// </summary>
    public abstract class ScriptObject
    {
        // 脚本对象名
        public abstract string Name
        {
            get;
        }

        // 根据名称获取组件对象
        public Component GetComponent(string name)
        {
            return _ssh.GetComponent(name);
        }

        // 脚本组件对象
        protected ICsScriptShell _ssh = null;
    }

    /// <summary>
    /// 提供脚本对象，或者从预编译 dll，或者从文件即时编译
    /// </summary>
    public interface IScriptObjectProvider
    {
        ScriptObject GetByFile(string f);

        ScriptObject GetByName(string name);

        ScriptObject CreateByFile(string f);

        ICsScriptShell CsScriptShell
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 脚本组件
    /// </summary>
    public class CsScriptShellIOS : Component, ICsScriptShell
    {
        public IScriptObjectProvider SP
        {
            get
            {
                return sp;
            }
            set
            {
                if (sp != null)
                    sp.CsScriptShell = null;

                sp = value;
                sp.CsScriptShell = this;
            }
        } IScriptObjectProvider sp = null;

        // 执行指定脚本文件中的指定函数，并提供参数列表
        public Object RunScriptA(string f, string fun, object[] args)
        {
            ScriptObject so = SP.GetByFile(f);
            return DynamicIOS.Invoke(so, fun, args);
        }

        // 执行指定脚本文件中的指定函数，并提供参数列表
        public Object RunScript(string f, string fun, params object[] args)
        {
            return RunScriptA(f, fun, args);
        }

        // 调用指定脚本对象的指定方法，并提供参数列表
        public Object Call(string scriptObjectName, string fun, params object[] args)
        {
            ScriptObject so = SP.GetByName(scriptObjectName);
            return DynamicIOS.Invoke(so, fun, args);
        }
    }
}
