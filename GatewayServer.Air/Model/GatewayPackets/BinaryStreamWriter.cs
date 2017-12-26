namespace GatewayServer.Air.Model.GatewayPackets
{
    using System.IO;
    using System.Text;

    public class BinaryStreamWriter : BinaryWriter
    {
        public BinaryStreamWriter(Stream stream)
            : base(stream)
        {

        }

        public override void Write(string value)
        {
            byte[] buffer = PacketTextEncoder.GetEncoding(value);
            //
            base.Write(buffer.Length);
            base.Write(buffer);
        }
    }
}
