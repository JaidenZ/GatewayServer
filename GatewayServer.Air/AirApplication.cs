namespace GatewayServer.Air
{
    using GatewayServer.Air.Model;
    using GatewayServer.Air.Tool;
    using GatewayServer.Air.Network;
    using GatewayServer.Air.Network.Tcp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class AirApplication
    {
        static IGatewayListener _listener;
        static GatewayCommunication gateway;
        [STAThread]
        static void Main(string[] args)
        {
            GatewayConfig config = SystemConfigTool.Current;
            AirApplication.Prepared(config);

            Console.ReadKey(false);
        }

        private static void Prepared(GatewayConfig config)
        {

            _listener = new GatewayTcpListener();
            _listener.ListenPort(config.ServerTcpPort, config.ClientTcpPort);

            gateway = new GatewayCommunication(_listener);
            gateway.Run();


        }


    }
}
