namespace GatewayServer.Air.Network
{
    using System.Net;

    public interface IClientMessage
    {

        /// <summary>
        /// 客户端类型
        /// </summary>
        byte ClientPacketType { get; }

        /// <summary>
        /// 远程地址
        /// </summary>
        EndPoint RemoteEP { get; }
        /// <summary>
        /// 本地地址
        /// </summary>
        EndPoint LocalEP { get; }

        /// <summary>
        /// 获取消息流
        /// </summary>
        /// <returns></returns>
        byte[] GetMessage();

        /// <summary>
        /// 连接编号
        /// </summary>
        long LinkNo { get; }
    }
}
