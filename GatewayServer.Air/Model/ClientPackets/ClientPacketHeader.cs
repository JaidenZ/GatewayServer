namespace GatewayServer.Air.Model.ClientPackets
{
    using GatewayServer.Air.Model.GatewayPackets;
    using Network;
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 客户端数据包信息
    /// </summary>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 10)]
    public unsafe struct ClientPacketHeader
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
        /// 客户端类型
        /// </summary>
        [FieldOffset(3)]
        public ClientPacketType bClientPacketType;

        public static ClientPacketHeader GetMessage(MessageReceivedEventArgs message)
        {
            fixed (byte* ptr = message.Buffer)
            {
                return *(ClientPacketHeader*)ptr;
            }
        }
    }
}
