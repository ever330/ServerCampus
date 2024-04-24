using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;


namespace ChatServer
{
    public class EBinaryRequestInfo : BinaryRequestInfo
    {
        public short PacketData { get; set; }
        public short PacketId { get; set; }

        public const int HEADER_SIZE = 4;

        public EBinaryRequestInfo(short totalData, short packetId, byte[] body)
            : base(null, body)
        {
            this.PacketData = totalData;
            this.PacketId = packetId;
        }
    }

    public class ReceiveFilter : FixedHeaderReceiveFilter<EBinaryRequestInfo>
    {
        public ReceiveFilter() : base(PacketDefine.PACKET_HEADER)
        {
        }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(header, offset, PacketDefine.PACKET_HEADER);
            }

            var totalData = BitConverter.ToInt16(header, offset);
            return totalData - EBinaryRequestInfo.HEADER_SIZE;
        }

        protected override EBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(header.Array, 0, PacketDefine.PACKET_HEADER);
            }

            return new EBinaryRequestInfo(BitConverter.ToInt16(header.Array, 0),
                                            BitConverter.ToInt16(header.Array, 2),
                                            bodyBuffer.CloneRange(offset, length));
        }
    }
}
