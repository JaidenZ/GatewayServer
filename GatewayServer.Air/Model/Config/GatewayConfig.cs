namespace GatewayServer.Air.Model
{
    /// <summary>
    /// 网关配置模型
    /// </summary>
    public class GatewayConfig
    {

        /// <summary>
        /// 客户端连接端口
        /// </summary>
        public int ClientTcpPort { get; set; }

        /// <summary>
        /// 服务器连接端口
        /// </summary>
        public int ServerTcpPort { get; set; }

    }
}
