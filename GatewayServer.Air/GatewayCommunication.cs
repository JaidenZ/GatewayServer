namespace GatewayServer.Air
{
    using GatewayServer.Air.Model.ClientPackets;
    using GatewayServer.Air.Model.GatewayPackets;
    using GatewayServer.Air.Network;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// 网关通信
    /// </summary>
    public class GatewayCommunication
    {
        private IGatewayListener listener = null;

        private Queue<IGatewayMessage> gatewayMessagePool = new Queue<IGatewayMessage>();

        private Queue<IClientMessage> clientMessagePool = new Queue<IClientMessage>();

        private Thread _sendClientWorkThread;
        private Thread _sendServerWorkThread;

        public IGatewayListener Listener
        {

            get
            {
                return this.listener;
            }
            private set
            {

                this.listener = value;
            }
        }



        public GatewayCommunication(IGatewayListener listener)
        {
            this.listener = listener;
            this.listener.Received += Listener_Received;
            this.listener.Disconnected += Value_Disconnected;
            this.listener.Connected += Value_Connected;
            _sendClientWorkThread = new Thread(WorkingClientSend) { IsBackground = true, Priority = ThreadPriority.Highest };
            _sendServerWorkThread = new Thread(WorkingServerSend) { IsBackground = true, Priority = ThreadPriority.Highest };

            _sendClientWorkThread.Start();
            _sendServerWorkThread.Start();


        }


        private void WorkingClientSend()
        {
            while (true)
            {
                if (clientMessagePool.Count > 0)
                {

                    IClientMessage message = clientMessagePool.Dequeue();
                    Listener.SendToServer(
                        GatewayPacketMessage.CreateServerMessage(
                            message.ClientPacketType,
                            message.LinkNo,
                            message.RemoteEP.ToString(),
                             GatewayCommand.CLIENT_INFO_QUEST,
                            message.GetMessage()));
                }
            }
        }

        /// <summary>
        /// 处理服务器数据转发
        /// </summary>
        private void WorkingServerSend()
        {
            while (true)
            {

                string test = "1234";
                //listener.SendToClient(Encoding.UTF8.GetBytes(test));
                if (gatewayMessagePool.Count > 0)
                {
                    IGatewayMessage message = gatewayMessagePool.Dequeue();

                    switch (message.GatewayMessageType)
                    {
                        case (int)GatewayCommand.CLIENT_INFO_QUEST:
                            


                            GatewayPacketMessage gatewaymessage = (GatewayPacketMessage)message;
                            if (gatewaymessage.pHeader.qdLinkCount > 0)
                            {
                                for (int i = 0; i < gatewaymessage.pBody.qclinkNo.Count; i++)
                                {
                                    listener.SendToClient(gatewaymessage.pBody.qclinkNo[i], message.GetMessage());
                                }
                            }
                            else
                            {
                                listener.SendToClient(message.GetMessage());
                            }

                            break;

                        case (int)GatewayCommand.SERVER_BEATHEART_QUEST:
                            Listener.SetServerHeart(message.LinkNo);
                            Listener.SendToServer(
                                GatewayPacketMessage.CreateServerMessage(
                                    message.ClientPacketType,
                                    message.LinkNo,
                                    message.RemoteEP.ToString(),
                                    GatewayCommand.SERVER_BEATHEART_ANSWER,
                                    message.GetMessage()));
                            break;

                        case (int)GatewayCommand.SERVER_LOGIN_QUEST:
                            Listener.SetServerHeart(message.LinkNo);
                            Listener.SendToServer(
                                GatewayPacketMessage.CreateServerMessage(
                                    message.ClientPacketType,
                                    message.LinkNo,
                                    message.RemoteEP.ToString(),
                                    GatewayCommand.SERVER_LOGIN_ANSWER,
                                    message.GetMessage()));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void Run()
        {
            this.listener.Start();
        }

        private void Value_Connected(Model.MessageHandler handler)
        {
            if (handler != null)
            {
                Console.WriteLine(handler.MessageInfo);
            }
        }

        private void Listener_Received(object sender, MessageReceivedEventArgs e)
        {
            if (e != null && e.Buffer != null)
            {
                if (e.RecivedType == Model.MessgeRecivedType.ClientMessage)
                {
                    HandlerClient(e);
                }
                else if (e.RecivedType == Model.MessgeRecivedType.ServerMessage)
                {
                    HandlerServer(e);
                }
            }
        }

        private void Value_Disconnected(Model.MessageHandler handler)
        {
            if (handler != null)
            {
                Console.WriteLine(handler.MessageInfo);
            }

        }

        private void HandlerClient(MessageReceivedEventArgs args)
        {
            try
            {
                IClientMessage message = ClientPacketMessage.GetMessage(args);
                //转发数据
                lock (clientMessagePool)
                {
                    clientMessagePool.Enqueue(message);
                }
            }
            catch
            {
                Listener.CloseLink(args.LinqNo);
            }
        }

        private void HandlerServer(MessageReceivedEventArgs args)
        {
            try
            {
                IGatewayMessage message = GatewayPacketMessage.GetMessage(args);
                lock (gatewayMessagePool)
                {
                    gatewayMessagePool.Enqueue(message);
                }
            }
            catch
            {
                Listener.CloseLink(args.LinqNo);
            }
        }


        /// <summary>
        /// 发送命令
        /// </summary>
        /// <returns></returns>
        private void SendMessage(GatewayCommand cmd)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryStreamWriter sw = new BinaryStreamWriter(ms))
                {
                    sw.Write(GatewayProtocolHeader.Gateway_STX);
                    sw.Write((short)0);
                    sw.Write((ushort)cmd);
                    sw.Write((byte)ClientPacketType.CLIENTPACKET_ALL);//客户端类型
                    //
                    //
                    byte[] message = ms.ToArray();
                    //
                    message[1] = (byte)message.Length;
                    message[2] = (byte)(message.Length << 8);
                    //
                    listener.SendToServer(message);
                }
            }
        }
    }
}
