namespace GD.RDP.Core.Buffers
{
    using GD.RDP.Core.Contracts;
    using System;

    public class BitBuffer : BaseBitBuffer
    {
        public BitBuffer()
            : base()
        {
        }

        public BitBuffer(byte[] buffer) 
            : base(buffer)
        {
        }

        public override IBuffer Write(long value, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int bit = ((value & (BIT_MASK << (i + offset))) > 0 ? 1 : 0);

                if (bit != 0)
                {
                    if(this._buffer.Count == 0)
                    {
                        this._buffer.Add(0);
                    }

                    this._buffer[this._index] =
                        (byte)(this._buffer[this._index] | (BIT_MASK << this._offset));
                }

                this._offset += 1;

                if (this._offset >= BYTE_SIZE_IN_BITS && i != count - 1)
                {
                    this._index += 1;

                    this._buffer.Add(0);

                    this._offset = 0;
                }
            }

            return this;
        }

        public override IBuffer Write(long value, int offset)
        {
            return this.Write(value, offset, 0);
        }

        public long Read(int count)
        {
            long result = 0;

            for (int i = 0; i < count; i++)
            {
                var value = this._buffer[this._index];

                var bit = (value & (BIT_MASK << this._offset)) > 0 ? 1 : 0;

                if (bit != 0)
                {
                    result = result | (BIT_MASK << this._offset);
                }

                this._offset += 1;

                if (this._offset >= BYTE_SIZE_IN_BITS)
                {
                    this._buffer[this._index] = 0;

                    this._index += 1;

                    this._offset = 0;
                }
            }

            return result;
        }

        public override T Read<T>()
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    return (T)Convert.ChangeType(this.Read(1), typeof(T));
                case TypeCode.Char:
                    return (T)Convert.ChangeType(this.Read(4), typeof(T));
                case TypeCode.Byte:
                    return (T)Convert.ChangeType(this.Read(8), typeof(T));
                case TypeCode.Int16:
                    return (T)Convert.ChangeType(this.Read(16), typeof(T));
                case TypeCode.Int32:
                    return (T)Convert.ChangeType(this.Read(32), typeof(T));
                case TypeCode.Int64:
                    return (T)Convert.ChangeType(this.Read(64), typeof(T));
                default:
                    throw new NotImplementedException();
            }
        }

        public override IBuffer Write<T>(T value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    return this.Write((int)Convert.ChangeType(value, typeof(bool)), 1);
                case TypeCode.Char:
                    return this.Write((char)Convert.ChangeType(value, typeof(char)), 4);
                case TypeCode.Byte:
                    return this.Write((byte)Convert.ChangeType(value, typeof(byte)), 8);
                case TypeCode.Int16:
                    return this.Write((short)Convert.ChangeType(value, typeof(short)), 16);
                case TypeCode.Int32:
                    return this.Write((int)Convert.ChangeType(value, typeof(int)), 32);
                case TypeCode.Int64:
                    return this.Write((long)Convert.ChangeType(value, typeof(long)), 64);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
