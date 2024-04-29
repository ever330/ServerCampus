using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmokGameServer;
using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;


namespace OmokGameServer
{
    public class OmokBinaryRequestInfo : BinaryRequestInfo
    {
        public short PacketData { get; set; }
        public short PacketId { get; set; }
        public string SessionId { get; set; }

        public const int HEADER_SIZE = 4;

        public OmokBinaryRequestInfo(short packetData, short packetId, byte[] body)
            : base(null, body)
        {
            this.PacketData = packetData;
            this.PacketId = packetId;
        }
    }

    public class ReceiveFilter : FixedHeaderReceiveFilter<OmokBinaryRequestInfo>
    {
        public ReceiveFilter() : base(OmokBinaryRequestInfo.HEADER_SIZE)
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(header, offset, 2);
            }

            var totalData = BitConverter.ToInt16(header, offset);
            return totalData - OmokBinaryRequestInfo.HEADER_SIZE;
        }

        protected override OmokBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            return new OmokBinaryRequestInfo(BitConverter.ToInt16(header.Array, 0),
                                            BitConverter.ToInt16(header.Array, 2),
                                            bodyBuffer.CloneRange(offset, length));
        }
    }
}
