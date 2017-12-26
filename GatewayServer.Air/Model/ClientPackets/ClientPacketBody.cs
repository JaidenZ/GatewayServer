namespace GatewayServer.Air.Model.ClientPackets
{

    using System;
    using System.Text;
    using System.Runtime.InteropServices;
    using Network;
    using System.Collections.Generic;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe partial class ClientPacketBody
    {
        /// <summary>
        /// 缓冲区数据
        /// </summary>
        public byte[] szBuffer;

        /// <summary>
        /// 缓冲区长度
        /// </summary>
        public int szBufferLen;

        /// <summary>
        /// 缓冲区数据位于流的偏移量
        /// </summary>
        public int dwBufferOfs;

        public byte* GetBuffer()
        {
            fixed (byte* ptr = &szBuffer[dwBufferOfs])
            {
                return ptr;
            }
        }

        public static ClientPacketBody GetMessage(ref ClientPacketHeader header, MessageReceivedEventArgs args)
        {
            byte[] message = args.Buffer;
            int offset = sizeof(ClientPacketHeader); // 位于包头的偏移
            if (offset > message.Length)
                return null;
            ClientPacketBody body = new ClientPacketBody();
            body.szBuffer = message; // 客户端发过来的数据（客户端数据+网关包头）
            body.dwBufferOfs = offset; // 客户端数据开始索引
            body.szBufferLen = message.Length - offset; // 客户端数据长度
            return body;
        }


    }
}
