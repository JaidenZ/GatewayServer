namespace GatewayServer.Air.Network
{
    using Model;
    using System;

    public partial interface IGatewayListener
    {
        /// <summary>
        /// 网关监听端口
        /// </summary>
        /// <param name="serverport">服务器端口</param>
        /// <param name="clientport">客户端端口</param>
        void ListenPort(int serverport,int clientport);

        /// <summary>
        /// 开启通信
        /// </summary>
        void Start();

        /// <summary>
        /// 关闭通信
        /// </summary>
        void Stop();

        /// <summary>
        /// 发送数据包到所有客户端
        /// </summary>
        /// <param name="buffer"></param>
        void SendToClient(byte[] buffer);

        /// <summary>
        /// 发送数据包到指定客户端
        /// </summary>
        /// <param name="linkNo"></param>
        /// <param name="buffer"></param>
        void SendToClient(long linkNo, byte[] buffer);

        /// <summary>
        /// 发送数据包到所有服务器
        /// </summary>
        /// <param name="buffer"></param>
        void SendToServer(byte[] buffer);

        /// <summary>
        /// 发送数据到指定服务器
        /// </summary>
        /// <param name="linkNo"></param>
        /// <param name="buffer"></param>
        void SendToServer(long linkNo, byte[] buffer);

        /// <summary>
        /// 设置服务器心跳连接状态
        /// </summary>
        /// <param name="linkNo"></param>
        void SetServerHeart(long linkNo);

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="linkNo"></param>
        void CloseLink(long linkNo);

        /// <summary>
        /// 数据到达
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> Received;

        /// <summary>
        /// 断开连接事件
        /// </summary>
        event MessageHandlerDelegate Disconnected;

        /// <summary>
        /// 连接事件
        /// </summary>
        event MessageHandlerDelegate Connected;

    }
}
