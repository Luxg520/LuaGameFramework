using System;

namespace Swift
{
    /// <summary>
    /// 带网络消息收发支持的组件
    /// </summary>
    public abstract class NetComponent : Component
    {
        // 响应网络消息
        public virtual void OnMessage(Connection conn, IReadableBuffer data)
        {
        }

        #region 保护部分

        // 创建应答对象
        protected Responser CreateResponser(Connection conn)
        {
            return new Responser(conn);
        }

        #endregion
    }

    /// <summary>
    /// 远端组件对象的访问代理
    /// </summary>
    public class NetComponentAgent : NetComponent
    {
		// 设置远端对应的连接 id 和组件对象名
		public void Setup(Connection conn)
		{
			this.conn = conn;
		}

        // 设置远端对应的连接 id 和组件对象名
        public void Setup(Connection conn, string componentName)
        {
            this.conn = conn;
            this.compName = componentName;
        }

        // 创建消息应答对象
        protected Responser CreateResponser()
        {
            return new Responser(conn);
        }

		// 对应的远端服务器组件名称
		public string ServerComponentName
		{
			get
			{
				return compName;
			}
			set
			{
				compName = value;
			}
		}

        #region 保护部分

        // 远端连接
        protected Connection conn = null;

        // 远端组件对象名
        protected string compName = null;

        #endregion
    }
}
