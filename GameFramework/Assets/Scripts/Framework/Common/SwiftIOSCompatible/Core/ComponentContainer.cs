using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swift
{
    /// <summary>
    /// 组件容器
    /// </summary>
    public class ComponentContainer
    {
        // 添加组件对象事件
        public event Action<Component> ComponentAdded = null;

        // 移除组件对象事件
        public event Action<Component> ComponentRemoved = null;

        // 获取给定类型的组件
        public T Get<T>() where T : class
        {
            List<Component> lst = All;
            for (int i = 0; i < lst.Count; i++)
            {
                Component c = lst[i];
                if (c is T)
                    return c as T;
            }

            return null;
        }

        // 获取给定名称的组件
        public Component GetByName(string name)
        {
            if (components.ContainsKey(name))
                return components[name];
            else
                return null;
        }

        // 获取所有组件对象
        public List<Component> All
        {
            get
            {
                if (modified)
                {
                    lstComponents.Clear();

                    var ie = components.GetEnumerator();
                    try 
                    {
                        while (ie.MoveNext())
                        {
                            var KV = ie.Current;
                            lstComponents.Add(KV.Value);
                        }
                    }
                    finally
                    {
                        ie.Dispose();
                    }
                    modified = false;
                }
                return lstComponents;
            }
        }

        // 添加组件对象
        public void Add(Component c)
        {
            if (c.Name == null)
                throw new Exception("Component has no name");
            else if (components.ContainsKey(c.Name))
                throw new Exception("Component name conflicted: " + c.Name);

			components[c.Name] = c;
            modified = true;
            //c.Container = this;
			c.FuncGetComponent = this.GetByName;
            c.FuncAddComponent = this.Add;
            c.FuncRemoveComponent = this.Remove;

            if (ComponentAdded != null)
                ComponentAdded(c);
        }

        // 添加组件对象，并命名之
        public void Add(string name, Component c)
        {
            if (name != null && components.ContainsKey(name))
                throw new Exception("Component has already got name: " + c.Name);
            else if (components.ContainsKey(name))
                throw new Exception("Component name conflicted: " + c.Name);

            c.Name = name;
            components[name] = c;
            modified = true;
            //c.Container = this;
            c.FuncGetComponent = this.GetByName;
            c.FuncAddComponent = this.Add;
            c.FuncRemoveComponent = this.Remove;
            c.OnAdded();

            if (ComponentAdded != null)
                ComponentAdded(c);
        }

        // 移除指定名称组件
        public Component Remove(string name)
        {
            Component c = null;

            if (components.ContainsKey(name))
            {
                c = components[name];
                components.Remove(name);
                modified = true;
                c.OnRemoved();

                if (ComponentRemoved != null)
                    ComponentRemoved(c);
            }

            return c;
        }

        #region 保护部分

        // 所有组件对象，按名称索引
        protected Dictionary<string, Component> components = new Dictionary<string, Component>();
        List<Component> lstComponents = new List<Component>();
        bool modified = false;

        #endregion
    }
}
