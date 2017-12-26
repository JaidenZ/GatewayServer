namespace GatewayServer.Air.Model.GatewayPackets
{
    using Network;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    /// <summary>
    /// 网关包消息
    /// </summary>
    public sealed class GatewayPacketMessage : EventArgs, IGatewayMessage
    {
        /// <summary>
        /// 网关包头
        /// </summary>
        public GatewayPacketHeader pHeader;
        /// <summary>
        /// 网关包体
        /// </summary>
        public GatewayPacketBody pBody;

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
        public long pLinkNo;
        
        private GatewayPacketMessage(GatewayPacketHeader header, GatewayPacketBody body, EndPoint remote, EndPoint local,long linkNo)
        {
            pBody = body;
            pRemoteEP = remote;
            pLocalEP = local;
            pHeader = header;
            pLinkNo = linkNo;
        }

        public static IGatewayMessage GetMessage(MessageReceivedEventArgs e)
        {
            GatewayPacketHeader header = GatewayPacketHeader.GetMessage(e); // 映射包的结构体
            GatewayPacketBody body = GatewayPacketBody.GetMessage(ref header, e);
            return new GatewayPacketMessage(header, body, e.RemoteEP, e.LocalEP,e.LinqNo);
        }

        /// <summary>
        /// 创建服务器网关消息
        /// </summary>
        /// <param name="links">服务器的连接编号</param>
        /// <param name="message">转发的数据</param>
        /// <returns></returns>
        public static byte[] CreateServerMessage(byte clienttype,long linkno,string ipaddress, GatewayCommand command, byte[] message)
        {
            using (MemoryStream ms = new MemoryStream(8096))
            {
                using (BinaryStreamWriter sw = new BinaryStreamWriter(ms))
                {
                    sw.Write(GatewayProtocolHeader.Gateway_STX);
                    sw.Write((short)0);
                    sw.Write((ushort)command);
                    sw.Write(clienttype);
                    sw.Write(linkno);
                    //发送客户端IP
                    sw.Write(ipaddress);
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


        ///// <summary>
        ///// 创建客户端网关消息
        ///// </summary>
        ///// <param name="links">客户端的连接编号</param>
        ///// <param name="message">转发的数据</param>
        ///// <param name="clienttype">客户端类型</param>
        ///// <returns></returns>
        //public static byte[] CreateClientMessage(long link, byte[] message, ClientPacketType clienttype)
        //{
        //    return GatewayPacketMessage.CreateClientMessage(new long[] { link }, message, clienttype);

        //}
        ///// <summary>
        ///// 创建客户端网关消息
        ///// </summary>
        ///// <param name="links">客户端的连接编号</param>
        ///// <param name="message">转发的数据</param>
        ///// <param name="clienttype">客户端类型</param>
        ///// <returns></returns>
        //public static byte[] CreateClientMessage(IList<long> links, byte[] message, ClientPacketType clienttype)
        //{
        //    if (links != null && links.Count > 0)
        //    {
        //        using (MemoryStream ms = new MemoryStream(8096))
        //        {
        //            using (BinaryStreamWriter sw = new BinaryStreamWriter(ms))
        //            {
        //                sw.Write(GatewayProtocolHeader.Gateway_STX);
        //                sw.Write((short)0);
        //                sw.Write((ushort)GatewayCommand.CLIENT_INFO_QUEST);
        //                sw.Write((byte)clienttype);
        //                sw.Write(links.Count);
        //                foreach (long cid in links)
        //                {
        //                    sw.Write(cid);
        //                }
        //                sw.Write(message);
        //                //
        //                byte[] buffer = ms.ToArray();
        //                buffer[1] = (byte)buffer.Length;
        //                buffer[2] = (byte)(buffer.Length >> 8);
        //                //
        //                return buffer;
        //            }
        //        }
        //    }
        //    return null;
        //}


        EndPoint IGatewayMessage.RemoteEP
        {
            get
            {
                return pRemoteEP;
            }
        }

        EndPoint IGatewayMessage.LocalEP
        {
            get
            {
                return pLocalEP;
            }
        }
        

        byte[] IGatewayMessage.GetMessage()
        {

            byte[] buffer = new byte[pBody.szBufferLen];
            Buffer.BlockCopy(pBody.szBuffer, pBody.dwBufferOfs, buffer, 0, pBody.szBufferLen);
            return buffer;
        }
        
        int IGatewayMessage.GatewayMessageType
        {
            get
            {
                return (int)pHeader.wCommands;
            }
        }


        public int ClientType
        {
            get
            {
                return (int)pHeader.bClientPacketType;
            }
        }

        byte IGatewayMessage.ClientPacketType
        {
            get
            {
                return (byte)pHeader.bClientPacketType;
            }
        }

        long IGatewayMessage.LinkNo
        {
            get
            {
                return pLinkNo;
            }
        }
    }
}
