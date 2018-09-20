namespace GD.RDP.Core.Helpers
{
    public class BitHelper
    {
        public static byte[] ReadBits(int value, int start, int length)
        {
            var buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                byte bit = (byte)((value & (1 << start)) > 0 ? 1 : 0);

                value = value >> 1;

                buffer[(length - 1) - i] = bit;
            }

            return buffer;
        }

        public static byte[] WriteBits(byte[] value, int start, byte[] buffer)
        {
            for (int i = start, j = 0; i < value.Length; i++, j++)
            {
                buffer[i] = value[j];
            }

            return buffer;
        }
    }
}
