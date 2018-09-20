namespace GD.RDP.Network.Packets
{
    using GD.RDP.Contracts;
    using System;
    using System.IO;

    public abstract class BasePacketDeserializer<TPacket> 
        : IPacketDeserializer<TPacket>
    {
        protected MemoryStream memoryStream;
        protected BinaryReader binaryReader;

        protected byte[] _buffer;

        public BasePacketDeserializer(byte[] buffer)
        {
            this._buffer = buffer;

            this.memoryStream = new MemoryStream(this._buffer, 0, this._buffer.Length);
            this.binaryReader = new BinaryReader(this.memoryStream);
        }

        public abstract TPacket DeserializePacket();

        protected byte[] BuildData(int headerLength, int dataLength)
        {
            var length = (dataLength - headerLength);

            var buffer = new byte[length];

            Array.Copy(this._buffer, headerLength, buffer, 0, length);

            return buffer;
        }
    }
}
