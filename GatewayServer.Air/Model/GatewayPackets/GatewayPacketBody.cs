namespace GatewayServer.Air.Model.GatewayPackets
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;
    using Network;
    using System.Collections.Generic;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe partial class GatewayPacketBody
    {
        /// <summary>
        /// 客户端连接编号
        /// </summary>

        public IList<int> qclinkNo;
        
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

        public static GatewayPacketBody GetMessage(ref GatewayPacketHeader header, MessageReceivedEventArgs args)
        {
            byte[] message = args.Buffer;
            int offset = sizeof(GatewayPacketHeader); // 位于包头的偏移
            if (offset > message.Length)
                return null;
            GatewayPacketBody body = new GatewayPacketBody();
            if (body.qclinkNo == null)
                body.qclinkNo = new List<int>();
            for (int i = 0; i < header.qdLinkCount; i++)
            {

                fixed(byte* ptrs = &args.Buffer[offset])
                {
                    int linkno = *((int*)ptrs);
                    body.qclinkNo.Add(linkno);
                    offset += sizeof(int);
                }
            }
            body.szBuffer = message; // 客户端发过来的数据（客户端数据+网关包头）
            body.dwBufferOfs = offset; // 客户端数据开始索引
            body.szBufferLen = message.Length - offset; // 客户端数据长度
            return body;
        }
    }
}
