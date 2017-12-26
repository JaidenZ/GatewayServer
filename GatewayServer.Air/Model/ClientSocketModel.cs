namespace GatewayServer.Air.Model
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;

    public class ClientSocketModel
    {
        /// <summary>
        /// 连接编号
        /// </summary>
        public int LinkNo { get; set; }

        /// <summary>
        /// 套接字
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// 缓冲区
        /// </summary>
        public byte[] Buffer { get; set; }
        
        /// <summary>
        /// 接收客户端的包头
        /// </summary>
        public CLIENTWORK_PACKET_HEADER? c_recvWorkHeader = null;

        /// <summary>
        /// 接收工作大小
        /// </summary>
        public int g_recvWorkSize = 5;

        /// <summary>
        /// 接受的包工作偏移
        /// </summary>
        public int g_recvWorkOfs = 0;

        /// <summary>
        /// 接受的完整一包
        /// </summary>
        public byte[] g_recvWorkPacket = null;

        /// <summary>
        /// 上次收取数据包时间
        /// </summary>
        public DateTime g_recvDataTime = DateTime.Parse("1900-01-01 00:00:00");


        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

        public bool Connected
        {
            get
            {
                int dwFlags = 0;
                if (!InternetGetConnectedState(ref dwFlags, 0))
                    return false;
                return !(((Socket.Poll(0, SelectMode.SelectRead) && Socket.Available <= 0) || !Socket.Connected));
            }
        }

        public void Close()
        {
            if (Socket != null)
            {
                Socket.Close();
                Socket = null;
            }
            this.CleanUp();
        }

        public unsafe byte[] CleanUp()
        {
            lock (this)
            {
                g_recvWorkSize = sizeof(CLIENTWORK_PACKET_HEADER);
                Buffer = new byte[g_recvWorkSize];
                g_recvWorkOfs = 0;
                byte[] data = g_recvWorkPacket;
                g_recvWorkPacket = null;
                c_recvWorkHeader = null;
                return data;
            }
        }

    }
}
