namespace GatewayServer.Air.Model
{

    /// <summary>
    /// 消息处理委托
    /// </summary>
    /// <param name="handler"></param>
    public delegate void MessageHandlerDelegate(MessageHandler handler);

    public class MessageHandler
    {

        public MessageCode MessageCode { get; set; }

        public string MessageInfo { get; set; }

        public object Tag { get; set; }


    }
}
