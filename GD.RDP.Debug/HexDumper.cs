namespace GD.RDP.Debug
{
    using GD.RDP.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class HexDumper : IDumper
    {
        public static int BYTE_OFFSET = 16;
        public static int PADDING_LENGTH = 2;
        public static int ASCI_TABLE_LAST_SYMBOL_CODE = 127;

        public string Dump(byte[] buffer)
        {
            var strBuilder = new StringBuilder();

            var index = buffer.ToList().FindLastIndex(x => x != 0);

            buffer = buffer.ToList().GetRange(0, index + 1).ToArray();

            for (int i = 0; i < buffer.Length; i += BYTE_OFFSET)
            {
                var padding = new String(' ', 4);

                var line = buffer
                    .Skip(i)
                    .Take(BYTE_OFFSET)
                    .ToArray();

                var offset = $"{i:D4}";

                var hex = BuildHexRepresentation(line);

                var text = BuildStringRepresentation(line);

                strBuilder.AppendLine(
                    $"{offset}{padding}{hex}{padding}{text}");
            }

            return strBuilder.ToString();
        }

        private string BuildHexRepresentation(byte[] line)
        {
            var middle = (BYTE_OFFSET / 2) + BYTE_OFFSET - 1;

            var hex = BitConverter.ToString(line).Replace('-', ' ');

            var hexMiddle = hex.Length / 2;

            if (hex.Length > middle)
            {
                var padding = new String(' ', PADDING_LENGTH);

                hex = hex.Insert(middle, padding);
            }

            var length = (BYTE_OFFSET * 2) + PADDING_LENGTH + BYTE_OFFSET;

            hex = hex.PadRight(length, ' ');

            return hex;
        }

        private string BuildStringRepresentation(byte[] line)
        {
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] <= (byte)' ' ||
                    line[j] >= ASCI_TABLE_LAST_SYMBOL_CODE)
                {
                    line[j] = (byte)'.';
                }
            }

            var text = Encoding.ASCII.GetString(line);

            return text;
        }

        public byte[] Load(string dump)
        {
            var buffer = new List<byte>();

            dump.Split(' ')
                .Where(x => x != String.Empty)
                .ToList()
                .ForEach(x =>
                {
                    if (x.Length > 2)
                    {
                        buffer.Add(Convert.ToByte(x.Substring(0, 2), 16));
                        buffer.Add(Convert.ToByte(x.Substring(2, 2), 16));
                    }
                    else
                    {
                        buffer.Add(Convert.ToByte(x, 16));
                    }
                });

            return buffer.ToArray();
        }
    }
}
