namespace GatewayServer.Air.Network
{
    using System;
    using System.Net;
    using Air.Model;
    public class MessageReceivedEventArgs:EventArgs
    {
        /// <summary>
        /// 收到的缓冲区数据
        /// </summary>
        public byte[] Buffer
        {
            get;
            set;
        }

        public EndPoint RemoteEP
        {
            get;
            set;
        }

        public EndPoint LocalEP
        {
            get;
            set;
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public MessgeRecivedType RecivedType
        {
            get;
            set;
        }

        /// <summary>
        /// 监听通信层
        /// </summary>
        public IGatewayListener Listener
        {
            get;
            set;
        }

        /// <summary>
        /// 连接编号
        /// </summary>
        public int LinqNo
        {
            get;
            set;
        }

    }
}
