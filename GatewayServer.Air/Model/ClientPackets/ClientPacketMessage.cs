namespace GatewayServer.Air.Model.ClientPackets
{
    using GatewayServer.Air.Model.GatewayPackets;
    using GatewayServer.Air.Network;
    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    /// 客户端包信息
    /// </summary>
    public class ClientPacketMessage : EventArgs, IClientMessage
    {

        /// <summary>
        /// 客户端包头
        /// </summary>
        public ClientPacketHeader pHeader;

        /// <summary>
        /// 客户端包体
        /// </summary>
        public ClientPacketBody pBody;


        /// 发送数据的远程主机网路点
        /// </summary>
        public EndPoint pRemoteEP;

        /// <summary>
        /// 本地主机的网络点
        /// </summary>
        public EndPoint pLocalEP;

        /// <summary>
        /// 连接编号
        /// </summary>
        private long pLinkNo;



        private ClientPacketMessage(ClientPacketHeader header, ClientPacketBody body, EndPoint remote, EndPoint local,long linkNo)
        {
            this.pHeader = header;
            this.pBody = body;
            this.pRemoteEP = remote;
            this.pLocalEP = local;
            this.pLinkNo = linkNo;
        }

        public static IClientMessage GetMessage(MessageReceivedEventArgs args)
        {
            ClientPacketHeader header = ClientPacketHeader.GetMessage(args);
            ClientPacketBody body = ClientPacketBody.GetMessage(ref header, args);
            return new ClientPacketMessage(header, body, args.RemoteEP, args.LocalEP,args.LinqNo);
        }

        public static byte[] CreateClientMessage(byte[] message)
        {
            using (MemoryStream ms = new MemoryStream(8096))
            {
                using (BinaryStreamWriter sw = new BinaryStreamWriter(ms))
                {
                    sw.Write(GatewayProtocolHeader.Clientway_STX);
                    sw.Write((short)0);
                    sw.Write((ushort)GatewayCommand.CLIENT_INFO_QUEST);
                    sw.Write((byte)ClientPacketType.CLIENTPACKET_ALL);

                    sw.Write(message);
                    //
                    byte[] buffer = ms.ToArray();
                    buffer[1] = (byte)buffer.Length;
                    buffer[2] = (byte)(buffer.Length >> 8);
                    //
                    return buffer;
                }
            }
        }


        byte IClientMessage.ClientPacketType
        {
            get
            {
                return (byte)pHeader.bClientPacketType;
            }
        }

        EndPoint IClientMessage.RemoteEP
        {
            get
            {
                return pRemoteEP;
            }
        }

        EndPoint IClientMessage.LocalEP
        {
            get
            {
                return pLocalEP;
            }

        }

        byte[] IClientMessage.GetMessage()
        {
            byte[] buffer = new byte[pBody.szBufferLen];
            Buffer.BlockCopy(pBody.szBuffer, pBody.dwBufferOfs, buffer, 0, pBody.szBufferLen);
            return buffer;
        }
        

        long IClientMessage.LinkNo
        {
            get
            {
                return pLinkNo;
            }
        }
    }
}
