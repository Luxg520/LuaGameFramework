using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swift
{
    /// <summary>
    /// 组件接口
    /// </summary>
    public class Component
    {
        // 组件对象名
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public Func<string, Component> FuncGetComponent = null;
		public Component GetComponent(string str)
		{
			return FuncGetComponent (str);
		}

        public Func<string, Component> FuncRemoveComponent = null;
        public Component RemoveComponent(string str)
        {
            return FuncRemoveComponent(str);
        }

        public Action<string, Component> FuncAddComponent = null;
        public void AddComponent(string str, Component com)
        {
            FuncAddComponent(str, com);
        }

        // 本对象被加入容器
        public virtual void OnAdded()
        {
        }

        // 本对象被移出容器
        public virtual void OnRemoved()
        {
        }

        // 关闭组件对象
        public virtual void Close()
        {
        }

        #region 保护部分

        // 组件对象名称
        protected string name = null;

        #endregion
    }
}
