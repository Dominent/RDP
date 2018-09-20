namespace GD.RDP.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static void CopyTo(this byte[] source, byte[] destination, int start, int length)
        {
            for (int i = start, j = 0; i < length; i++, j++)
            {
                destination[j] = source[i];
            }
        }

        public static void CopyTo(this byte[] source, byte[] destination, int offset)
        {
            for (int i = 0, j = offset; i < source.Length; i++, j++)
            {
                destination[j] = source[i];
            }
        }
    }
}
