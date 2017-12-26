namespace GatewayServer.Air.Network.Tcp
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GatewayServer.Air.Tool;
    using GatewayServer.Air.Model;

    /// <summary>
    /// TCP监听层
    /// </summary>
    public unsafe partial class GatewayTcpListener : IGatewayListener
    {

        /// <summary>
        /// 服务器监听器
        /// </summary>
        private TcpListener g_sListener = null;
        /// <summary>
        /// 客户端监听器
        /// </summary>
        private TcpListener g_cListener = null;

        /// <summary>
        /// 服务器监听套接字字典
        /// </summary>
        private Dictionary<long, ServerSocketModel> g_sdSockets = new Dictionary<long, ServerSocketModel>();
        /// <summary>
        /// 客户端监听套接字字典
        /// </summary>
        private Dictionary<long, ClientSocketModel> g_cdSockets = new Dictionary<long, ClientSocketModel>();

        /// <summary>
        /// 接收服务器工作线程
        /// </summary>
        private Thread g_recvServerThread = null;

        /// <summary>
        /// 接收客户端工作线程
        /// </summary>
        private Thread g_recvClientThread = null;

        /// <summary>
        /// 检查工作状态线程
        /// </summary>
        private Thread z_checkWorkingState = null;


        private bool g_exitServerThread = true;
        private bool g_exitClientThread = true;



        /// <summary>
        /// 数据到达
        /// </summary>
        private IList<EventHandler<MessageReceivedEventArgs>> g_evtMsgRecvs = new List<EventHandler<MessageReceivedEventArgs>>();

        /// <summary>
        /// 断开连接事件
        /// </summary>
        public event MessageHandlerDelegate Disconnected;

        /// <summary>
        /// 连接事件
        /// </summary>
        public event MessageHandlerDelegate Connected;

        /// <summary>
        /// 获取客户端套接字缓冲区数据
        /// </summary>
        /// <param name="socket"></param>
        private unsafe void RecvClientBufferMethod(object socket)
        {
            ClientSocketModel clientsocket = (ClientSocketModel)socket;
            if (clientsocket != null && clientsocket.Socket != null)
            {
                while (!g_exitClientThread)
                {
                    byte[] data = null;
                    int len = 0;
                    try
                    {
                        if (clientsocket.Socket == null || clientsocket.Socket.Available < clientsocket.g_recvWorkSize)
                            Thread.Sleep(1);
                        else
                        {
                            len = clientsocket.Socket.Receive((clientsocket.c_recvWorkHeader == null ? clientsocket.Buffer : clientsocket.g_recvWorkPacket),
                                clientsocket.g_recvWorkOfs, clientsocket.g_recvWorkSize, SocketFlags.None);

                            if (clientsocket.c_recvWorkHeader == null)
                            {
                                CLIENTWORK_PACKET_HEADER* header = (CLIENTWORK_PACKET_HEADER*)Marshal.UnsafeAddrOfPinnedArrayElement(clientsocket.Buffer, 0);
                                if (header->bkey == CLIENTWORK_PACKET_HEADER.STX)
                                {
                                    clientsocket.g_recvWorkSize = (header->wlen - len);
                                    clientsocket.g_recvWorkOfs = len;
                                }
                                clientsocket.c_recvWorkHeader = *header;
                                clientsocket.g_recvWorkPacket = new byte[header->wlen];
                                Buffer.BlockCopy(clientsocket.Buffer, 0, clientsocket.g_recvWorkPacket, 0, len);
                                if (clientsocket.g_recvWorkSize <= 0)
                                {
                                    data = clientsocket.CleanUp();
                                }
                            }
                            else if ((len - clientsocket.g_recvWorkSize) <= 0)
                            {
                                data = clientsocket.CleanUp();
                            }
                        }

                        if (clientsocket.Socket != null && !clientsocket.Connected)
                        {
                            this.Close(clientsocket.LinkNo);

                        }
                    }
                    catch
                    {
                        this.Close(clientsocket.LinkNo);
                        Thread.CurrentThread.Abort();
                    }
                    if (data != null)
                    {
                        MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                        args.Buffer = data;
                        args.Listener = this;
                        args.LocalEP = clientsocket.Socket.LocalEndPoint;
                        args.RemoteEP = clientsocket.Socket.RemoteEndPoint;
                        args.RecivedType = MessgeRecivedType.ClientMessage;
                        args.LinqNo = clientsocket.LinkNo;
                        OnReceived(args);
                    }
                }
            }
        }

        /// <summary>
        /// 获取服务器套接字缓冲区数据
        /// </summary>
        /// <param name="socket"></param>
        private unsafe void RecvServerBufferMethod(object socket)
        {
            ServerSocketModel serversocket = (ServerSocketModel)socket;
            if (serversocket != null && serversocket.Socket != null)
            {
                while (!g_exitServerThread)
                {
                    byte[] data = null;
                    int len = 0;
                    try
                    {
                        if (serversocket.Socket == null || serversocket.Socket.Available < serversocket.g_recvWorkSize)
                            Thread.Sleep(1);
                        else
                        {
                            len = serversocket.Socket.Receive((serversocket.g_recvWorkHeader == null ? serversocket.Buffer : serversocket.g_recvWorkPacket),
                                serversocket.g_recvWorkOfs, serversocket.g_recvWorkSize, SocketFlags.None);

                            if (serversocket.g_recvWorkHeader == null)
                            {
                                NETWORK_PACKET_HEADER* header = (NETWORK_PACKET_HEADER*)Marshal.UnsafeAddrOfPinnedArrayElement(serversocket.Buffer, 0);
                                if (header->bkey == NETWORK_PACKET_HEADER.STX)
                                {
                                    serversocket.g_recvWorkSize = (header->wlen - len);
                                    serversocket.g_recvWorkOfs = len;
                                }
                                serversocket.g_recvWorkHeader = *header;
                                serversocket.g_recvWorkPacket = new byte[header->wlen];
                                Buffer.BlockCopy(serversocket.Buffer, 0, serversocket.g_recvWorkPacket, 0, len);
                                if (serversocket.g_recvWorkSize <= 0)
                                {
                                    data = serversocket.CleanUp();
                                }
                            }
                            else if ((len - serversocket.g_recvWorkSize) <= 0)
                            {
                                data = serversocket.CleanUp();
                            }
                        }


                        if (serversocket.Socket != null && !serversocket.Connected)
                        {
                            this.Close(serversocket.LinkNo);
                        }

                    }
                    catch
                    {
                        this.Close(serversocket.LinkNo);
                        Thread.CurrentThread.Abort();
                    }
                    if (data != null)
                    {
                        MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                        args.Buffer = data;
                        args.Listener = this;
                        args.LocalEP = serversocket.Socket.LocalEndPoint;
                        args.RemoteEP = serversocket.Socket.RemoteEndPoint;
                        args.RecivedType = MessgeRecivedType.ServerMessage;
                        args.LinqNo = serversocket.LinkNo;
                        OnReceived(args);
                    }
                }
            }
        }


        private unsafe void RecvClientSocketMethod()
        {
            while (!g_exitClientThread)
            {
                try
                {
                    if (g_cListener == null || !g_cListener.Pending())
                        Thread.Sleep(1);
                    else
                    {
                        Socket recvClientSocket = g_cListener.AcceptSocket();
                        if (recvClientSocket != null)
                        {
                            ClientSocketModel socket = new ClientSocketModel();
                            socket.LinkNo = LinkNoTool.GetLinkNo();
                            socket.Socket = recvClientSocket;
                            socket.g_recvDataTime = DateTime.Now;
                            socket.CleanUp();
                            lock (g_cdSockets)
                            {
                                g_cdSockets.Add(socket.LinkNo, socket);
                            }
                            Thread recvthread = new Thread(new ParameterizedThreadStart(RecvClientBufferMethod));
                            recvthread.Start(socket);

                            MessageHandler handler = new MessageHandler();
                            handler.MessageCode = MessageCode.Success;
                            handler.MessageInfo = string.Format("[{0}] 客户端成功连接,连接编号:{1},[IP]:{2}", DateTime.Now.ToString("HH:mm:ss"), socket.LinkNo,socket.Socket.RemoteEndPoint.ToString());
                            if (this.Connected != null)
                            {
                                Connected(handler);
                            }

                        }
                    }
                }
                catch { }
            }
        }

        private unsafe void RecvServerSocketMethod()
        {
            while (!g_exitServerThread)
            {
                try
                {
                    if (g_sListener == null || !g_sListener.Pending())
                        Thread.Sleep(1);
                    else
                    {
                        Socket recvServerSocket = g_sListener.AcceptSocket();
                        if (recvServerSocket != null)
                        {
                            ServerSocketModel socket = new ServerSocketModel();
                            socket.LinkNo = LinkNoTool.GetLinkNo();
                            socket.Socket = recvServerSocket;
                            socket.g_recvHeartTime = DateTime.Now;
                            socket.CleanUp();
                            lock (g_sdSockets)
                            {
                                g_sdSockets.Add(socket.LinkNo, socket);
                            }
                            Thread recvthread = new Thread(new ParameterizedThreadStart(RecvServerBufferMethod));
                            recvthread.Start(socket);

                            MessageHandler handler = new MessageHandler();
                            handler.MessageCode = MessageCode.Success;
                            handler.MessageInfo = string.Format("[{0}] 服务器成功连接,连接编号:{1},ip:[{2}]", DateTime.Now.ToString("HH:mm:ss"), socket.LinkNo,socket.Socket.RemoteEndPoint.ToString());
                            if (this.Connected != null)
                            {
                                Connected(handler);
                            }

                        }
                    }
                }
                catch { }
            }
        }

        private void CheckWorkingStateMethod()
        {
            while (true)
            {
                Thread.Sleep(1000 * 10);

                if (g_cdSockets != null && g_cdSockets.Count > 0)
                {
                    IList<ClientSocketModel> valuelist = g_cdSockets.Values.ToList();
                    for (int i = 0; i < valuelist.Count; i++)
                    {
                        TimeSpan span = DateTime.Now - valuelist[i].g_recvDataTime;
                        if (span.TotalMilliseconds > 1000 * 60 * 60)
                            this.Close(valuelist[i].LinkNo);
                    }
                }

                if (g_sdSockets != null && g_sdSockets.Count > 0)
                {
                    IList<ServerSocketModel> valuelist = g_sdSockets.Values.ToList();
                    for (int i = 0; i < valuelist.Count; i++)
                    {
                        TimeSpan span = DateTime.Now - valuelist[i].g_recvHeartTime;
                        if (span.TotalMilliseconds > 1000 * 25)
                            this.Close(valuelist[i].LinkNo);
                    }
                }
            }

        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="linkNo"></param>
        private void Close(long linkNo)
        {
            if (g_cdSockets != null && g_cdSockets.ContainsKey(linkNo))
            {
                ClientSocketModel c = g_cdSockets[linkNo];
                c.Close();
                g_cdSockets.Remove(linkNo);

                if (Disconnected != null)
                {
                    MessageHandler handler = new MessageHandler();
                    handler.MessageCode = MessageCode.Warming;
                    handler.MessageInfo = string.Format("[{0}] 断开一个客户端套接字,连接编号:{1}", DateTime.Now.ToString("HH:mm:ss"), linkNo);
                    Disconnected(handler);
                }
            }

            if (g_sdSockets != null && g_sdSockets.ContainsKey(linkNo))
            {
                ServerSocketModel s = g_sdSockets[linkNo];
                s.Close();
                g_sdSockets.Remove(linkNo);

                if (Disconnected != null)
                {
                    MessageHandler handler = new MessageHandler();
                    handler.MessageCode = MessageCode.Warming;
                    handler.MessageInfo = string.Format("[{0}] 断开一个服务器套接字,连接编号:{1}", DateTime.Now.ToString("HH:mm:ss"), linkNo);
                    Disconnected(handler);
                }
            }
        }

        /// <summary>
        /// 发送数据到客户端
        /// </summary>
        /// <param name="linkNo"></param>
        /// <param name="buffer"></param>
        private void SendToClient(long linkNo, byte[] buffer)
        {
            if (buffer != null && buffer.Length > 0)
            {
                if (g_cdSockets != null && g_cdSockets.ContainsKey(linkNo))
                {
                    ClientSocketModel clientsocket = g_cdSockets[linkNo];
                    if (clientsocket != null && clientsocket.Socket != null)
                    {
                        try
                        {
                            SocketError error = SocketError.SocketError;
                            clientsocket.Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, out error, (result) =>
                            {
                                if(clientsocket.Socket != null)
                                {
                                    clientsocket.Socket.EndSend(result, out error);
                                }
                                if (error != SocketError.Success)
                                {
                                    this.Close(clientsocket.LinkNo);
                                }
                            }, null);
                            if (error != SocketError.Success)
                            {
                                this.Close(clientsocket.LinkNo);
                            }
                        }
                        catch (Exception)
                        {
                            this.Close(clientsocket.LinkNo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发送数据到服务器
        /// </summary>
        /// <param name="linkNo"></param>
        /// <param name="buffer"></param>
        private void SendToServer(long linkNo, byte[] buffer)
        {
            if (buffer != null && buffer.Length > 0)
            {
                if (g_sdSockets != null && g_sdSockets.ContainsKey(linkNo))
                {
                    ServerSocketModel serversocket = g_sdSockets[linkNo];
                    if (serversocket != null && serversocket.Socket != null)
                    {
                        try
                        {
                            SocketError error = SocketError.SocketError;
                            serversocket.Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, out error, (result) =>
                            {
                                if(serversocket.Socket != null)
                                {
                                    serversocket.Socket.EndSend(result, out error);
                                }
                                else
                                {
                                    this.Close(serversocket.LinkNo);
                                }
                                if (error != SocketError.Success)
                                {
                                    this.Close(serversocket.LinkNo);
                                }
                            }, null);
                            if (error != SocketError.Success)
                            {
                                this.Close(serversocket.LinkNo);
                            }
                        }
                        catch (Exception)
                        {
                            this.Close(serversocket.LinkNo);
                        }
                    }
                }
            }
        }

        event EventHandler<MessageReceivedEventArgs> IGatewayListener.Received
        {
            add
            {
                var buffer = g_evtMsgRecvs;
                if (!buffer.Contains(value))
                {
                    buffer.Add(value);
                }
            }
            remove
            {
                var buffer = g_evtMsgRecvs;
                int i = buffer.IndexOf(value);
                if (i >= 0)
                {
                    buffer.RemoveAt(i);
                }
            }
        }

        void IGatewayListener.ListenPort(int serverport, int clientport)
        {
            IPAddress address = Dns.GetHostAddresses(Dns.GetHostName()).Where(s => s.AddressFamily == AddressFamily.InterNetwork).First();

            IPEndPoint spoint = new IPEndPoint(address, serverport);
            IPEndPoint cpoint = new IPEndPoint(address, clientport);

            g_cListener = new TcpListener(cpoint);
            g_sListener = new TcpListener(spoint);

        }

        /// <summary>
        /// 发送到客户端
        /// </summary>
        /// <param name="buffer"></param>
        void IGatewayListener.SendToClient(byte[] buffer)
        {
            if (g_cdSockets != null && g_cdSockets.Count > 0 && buffer != null && buffer.Length > 0)
            {
                lock (g_cdSockets)
                {
                    IList<long> linklist = g_cdSockets.Keys.ToList<long>();
                    for (int i = 0; i < linklist.Count; i++)
                    {
                        this.SendToClient(linklist[i], buffer);
                    }
                }
            }
        }

        /// <summary>
        /// 发送到指定客户端
        /// </summary>
        /// <param name="linkNo"></param>
        /// <param name="buffer"></param>
        void IGatewayListener.SendToClient(long linkNo, byte[] buffer)
        {
            this.SendToClient(linkNo, buffer);
        }

        /// <summary>
        /// 发送到所有服务器
        /// </summary>
        /// <param name="buffer"></param>
        void IGatewayListener.SendToServer(byte[] buffer)
        {
            if (g_sdSockets != null && g_sdSockets.Count > 0 && buffer != null && buffer.Length > 0)
            {
                lock (g_sdSockets)
                {
                    IList<long> linklist = g_sdSockets.Keys.ToList<long>();
                    for (int i = 0; i < linklist.Count; i++)
                    {
                        this.SendToServer(linklist[i], buffer);
                    }
                }
            }
        }

        /// <summary>
        /// 发送数据到指定服务器
        /// </summary>
        /// <param name="linkNo"></param>
        /// <param name="buffer"></param>
        void IGatewayListener.SendToServer(long linkNo, byte[] buffer)
        {
            this.SendToServer(linkNo, buffer);
        }

        void IGatewayListener.Start()
        {
            lock (this)
            {
                g_exitServerThread = false;
                g_exitClientThread = false;

                g_cListener.Start();
                g_sListener.Start();

                if (g_recvServerThread == null)
                {
                    g_recvServerThread = new Thread(RecvServerSocketMethod) { IsBackground = true, Priority = ThreadPriority.Highest };
                }
                if (g_recvClientThread == null)
                {
                    g_recvClientThread = new Thread(RecvClientSocketMethod) { IsBackground = true, Priority = ThreadPriority.Highest };
                }
                if(z_checkWorkingState == null)
                {
                    z_checkWorkingState = new Thread(CheckWorkingStateMethod) { IsBackground = true, Priority = ThreadPriority.Lowest };
                }
                g_recvClientThread.Start();
                g_recvServerThread.Start();
                z_checkWorkingState.Start();

            }
        }

        void IGatewayListener.Stop()
        {
            g_exitServerThread = false;
            g_exitClientThread = false;

            g_sListener.Stop();
            g_cListener.Stop();

        }

        void IGatewayListener.CloseLink(long linkNo)
        {
            this.Close(linkNo);
        }

        void IGatewayListener.SetServerHeart(long linkNo)
        {
            if (g_sdSockets != null && g_sdSockets.ContainsKey(linkNo))
            {
                g_sdSockets[linkNo].g_recvHeartTime = DateTime.Now;
            }
            if(g_cdSockets != null && g_cdSockets.ContainsKey(linkNo))
            {
                g_cdSockets[linkNo].g_recvDataTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 数据到达通知函数可重写
        /// </summary>
        protected virtual void OnReceived(MessageReceivedEventArgs e)
        {
            var buffer = g_evtMsgRecvs;
            if (buffer.Count > 0)
            {
                foreach (EventHandler<MessageReceivedEventArgs> handler in buffer)
                {
                    handler(this, e);
                }
            }
        }
    }
}
