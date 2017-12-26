namespace GatewayServer.Air.Model.GatewayPackets
{
    /// <summary>
    /// 网关信息指令
    /// </summary>
    public enum GatewayCommand : ushort
    {
        /// <summary>
        /// 服务器登录请求
        /// </summary>
        SERVER_LOGIN_QUEST = 0X1366,
        /// <summary>
        /// 服务器登录应答
        /// </summary>
        SERVER_LOGIN_ANSWER = 0X1367,
        /// <summary>
        /// 服务器心跳请求
        /// </summary>
        SERVER_BEATHEART_QUEST = 0X6066,
        /// <summary>
        /// 服务器心跳应答
        /// </summary>
        SERVER_BEATHEART_ANSWER = 0x6067,
        /// <summary>
        /// 客户端信息请求
        /// </summary>
        CLIENT_INFO_QUEST = 0x1B10,
    }

    /// <summary>
    /// 客户端类型
    /// </summary>
    public enum ClientPacketType : byte
    {
        /// <summary>
        /// 所有客户端
        /// </summary>
        CLIENTPACKET_ALL = 0,
        /// <summary>
        /// Iphone端
        /// </summary>
        CLIENTPACKET_IPHONE = 1,
        /// <summary>
        /// Android端
        /// </summary>
        CLIENTPACKET_ANDROID = 2,
        /// <summary>
        /// Windows端
        /// </summary>
        CLIENTPACKET_WINDOWS = 3

    }

    public partial class GatewayProtocolHeader
    {
        public const byte Gateway_STX = 0x6B;
        public const byte Clientway_STX = 0x6C;
    }
}
