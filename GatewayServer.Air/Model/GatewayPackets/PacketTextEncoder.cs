namespace GatewayServer.Air.Model.GatewayPackets
{
    using System.Text;

    /// <summary>
    /// 包的文本编码器
    /// </summary>
    public static class PacketTextEncoder
    {
        /// <summary>
        /// 获取编码器
        /// </summary>
        /// <returns></returns>
        public static Encoding GetEncoding()
        {
            return Encoding.Default;
        }

        /// <summary>
        /// 把字符串编码
        /// </summary>
        /// <returns></returns>
        public static byte[] GetEncoding(this string value)
        {
            Encoding e = PacketTextEncoder.GetEncoding();
            return e.GetBytes(value);
        }

        /// <summary>
        /// 把字节编码
        /// </summary>
        /// <returns></returns>
        public static string GetEncoding(this byte[] value, int index, int count)
        {
            Encoding e = PacketTextEncoder.GetEncoding();
            return e.GetString(value, index, count);
        }

        /// <summary>
        /// 把字节编码
        /// </summary>
        /// <returns></returns>
        public static string GetEncoding(this byte[] value)
        {
            Encoding e = PacketTextEncoder.GetEncoding();
            return e.GetString(value);
        }
    }
}
