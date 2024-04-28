using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok
{
    public class PacketBufferManager
    {
        private readonly object lockObject = new object();

        public int dataCnt;

        private byte[] _buffer;
        private int _readIndex;
        private int _writeIndex;
        private int _bufSize;
        private int _emptySize;
        private int _headerSize;

        public PacketBufferManager(int size, int headerSize)
        {
            _buffer = new byte[size];
            _readIndex = 0;
            _writeIndex = 0;
            dataCnt = 0;
            _bufSize = size;
            _emptySize = size;
            _headerSize = headerSize;
        }

        public bool WriteData(byte[] data, int dataLength)
        {
            lock (lockObject)
            {
                if (_emptySize < dataLength)
                    return false;

                if (_bufSize - _writeIndex >= dataLength)
                {
                    Buffer.BlockCopy(data, 0, _buffer, _writeIndex, dataLength);
                }
                else
                {
                    Buffer.BlockCopy(data, 0, _buffer, _writeIndex, _bufSize - _writeIndex);
                    Buffer.BlockCopy(data, _bufSize - _writeIndex, _buffer, 0, dataLength - (_bufSize - _writeIndex));
                }

                _writeIndex += dataLength;
                if (_writeIndex >= _bufSize)
                    _writeIndex -= _bufSize;

                dataCnt += dataLength;
                _emptySize -= dataLength;
            }

            return true;
        }

        public ArraySegment<byte> ReadData()
        {
            lock (lockObject)
            {
                if (dataCnt < _headerSize)
                {
                    return new ArraySegment<byte>();
                }

                byte[] retData = new byte[_headerSize];
                if (_bufSize - _readIndex >= _headerSize)
                {
                    Buffer.BlockCopy(_buffer, _readIndex, retData, 0, _headerSize);
                }
                else
                {
                    Buffer.BlockCopy(_buffer, _readIndex, retData, 0, _bufSize - _readIndex);
                    Buffer.BlockCopy(_buffer, 0, retData, _bufSize - _readIndex, _headerSize - (_bufSize - _readIndex));
                }

                int pacSize = BitConverter.ToInt16(retData, 0);

                if (dataCnt < pacSize)
                {
                    return new ArraySegment<byte>();
                }

                byte[] packetData = new byte[pacSize];

                if (_bufSize - _readIndex >= pacSize)
                {
                    Buffer.BlockCopy(_buffer, _readIndex, packetData, 0, pacSize);
                }
                else
                {
                    Buffer.BlockCopy(_buffer, _readIndex, packetData, 0, _bufSize - _readIndex);
                    Buffer.BlockCopy(_buffer, 0, packetData, _bufSize - _readIndex, pacSize - (_bufSize - _readIndex));
                }
                _readIndex += pacSize;

                if (_readIndex >= _bufSize)
                    _readIndex -= _bufSize;

                dataCnt -= pacSize;
                _emptySize += pacSize;

                return packetData;
            }
        }
    }
}
