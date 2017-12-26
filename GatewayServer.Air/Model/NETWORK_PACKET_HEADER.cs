namespace GatewayServer.Air.Model
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 5)]
    public struct NETWORK_PACKET_HEADER
    {
        /// <summary>
        /// 键帧
        /// </summary>
        [FieldOffset(0)]
        public byte bkey;
        /// <summary>
        /// 帧长
        /// </summary>
        [FieldOffset(1)]
        public ushort wlen;
        /// <summary>
        /// 命令
        /// </summary>
        [FieldOffset(3)]
        public ushort wcmd;

        /// <summary>
        /// 网关帧头
        /// </summary>
        public const int STX = 0x6B;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 4)]
    public struct CLIENTWORK_PACKET_HEADER
    {
        /// <summary>
        /// 键帧
        /// </summary>
        [FieldOffset(0)]
        public byte bkey;
        /// <summary>
        /// 帧长
        /// </summary>
        [FieldOffset(1)]
        public ushort wlen;
        /// <summary>
        /// 客户端类型
        /// </summary>
        [FieldOffset(3)]
        public byte btype;

        /// <summary>
        /// 客户端帧头
        /// </summary>
        public const int STX = 0x6C;
    }
}
