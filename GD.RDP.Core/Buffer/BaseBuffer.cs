namespace GD.RDP.Core.Buffers
{
    using GD.RDP.Core.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class BaseBitBuffer : IBuffer
    {
        public const int DEFAULT_INDEX_VALUE = 0;
        public const long BIT_MASK = 1;
        public const int INT_SIZE_IN_BITS = 32;
        public const int BYTE_SIZE_IN_BITS = 8;

        protected IList<byte> _buffer;
        protected int _offset;
        protected int _index;

        public BaseBitBuffer()
        {
            this._buffer = new List<byte>();

            this._index = DEFAULT_INDEX_VALUE;
            this._offset = 0;
        }

        public BaseBitBuffer(byte[] buffer)
        {
            this._buffer = new List<byte>(buffer);

            this._index = DEFAULT_INDEX_VALUE;
            this._offset = 0;
        }

        public int Count
        {
            get
            {
                return (this._buffer.Count * BYTE_SIZE_IN_BITS) + this._offset;
            }
        }

        public byte[] Value
        {
            get
            {
                return this._buffer.ToArray();
            }
        }

        public bool IsEmpty()
        {
            return this._buffer.Count == 0;
        }

        public abstract T Read<T>() where T : struct;

        public abstract IBuffer Write<T>(T value) where T : struct;

        public abstract IBuffer Write(long value, int offset, int count);

        public abstract IBuffer Write(long value, int offset);

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            var count = (this._index + 1);

            for (int i = 0; i < count; i++)
            {
                var value = this._buffer[i];

                var length = i != this._index ? BYTE_SIZE_IN_BITS : this._offset;

                for (int j = 0; j < length; j++)
                {
                    byte bit = (byte)((value & (BIT_MASK << j)) > 0 ? 1 : 0);

                    strBuilder.Append(bit);
                }
            }

            while ((strBuilder.Length - 1) % 4 != 0)
            {
                strBuilder.Append('0');
            }

            var output = BuildSpacers(strBuilder.ToString(), 4);

            return new string(output.Reverse().ToArray());
        }

        private string BuildSpacers(string input, int offset)
        {
            var paddings = input.Length / offset;

            var buffer = input.ToCharArray();
            var parts = new List<string>();

            for (int i = 0, j = 0; i < paddings; i++, j += 4)
            {
                var value = new string(buffer
                    .Skip(j)
                    .Take(4)
                    .ToArray());

                parts.Add(value);
            }

            return String.Join(" ", parts);
        }
    }
}
