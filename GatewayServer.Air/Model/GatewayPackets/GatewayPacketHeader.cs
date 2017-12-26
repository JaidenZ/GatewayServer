namespace GatewayServer.Air.Model.GatewayPackets
{
    using Network;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 网关数据包信息
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 10)]
    public unsafe struct GatewayPacketHeader
    {
        /// <summary>
        /// 帧头
        /// </summary>
        [FieldOffset(0)]
        public byte bFrameHeader;

        /// <summary>
        /// 包长
        /// </summary>
        [FieldOffset(1)]
        public short wPacketLength;

        /// <summary>
        /// 网关命令
        /// </summary>
        [FieldOffset(3)]
        public GatewayCommand wCommands;

        /// <summary>
        /// 客户端类型
        /// </summary>
        [FieldOffset(5)]
        public ClientPacketType bClientPacketType;

        /// <summary>
        /// 连接编号数量
        /// </summary>
        [FieldOffset(6)]
        public int qdLinkCount;
        

        public static GatewayPacketHeader GetMessage(MessageReceivedEventArgs message)
        {
            fixed (byte* ptr = message.Buffer)
            {
                return *(GatewayPacketHeader*)ptr;
            }
        }
    }
}
