using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayServer.Air.Model
{

    /// <summary>
    /// 数据到达类型
    /// </summary>
    public enum MessgeRecivedType
    {
        /// <summary>
        /// 服务器消息
        /// </summary>
        ServerMessage = 0,
        /// <summary>
        /// 客户端消息
        /// </summary>
        ClientMessage = 1
    }


    public enum MessageCode
    {
    
        Success = 0,
        Warming = 1,
        Wrong = 2,
        Exception = 3


    }

}
