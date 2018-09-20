namespace GD.RDP.Debug
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class InputHelper
    {
        public byte[] TextToByteArray(string input, string[] tokensToRemove, NumberStyles format)
        {
            var tokens = input.Split(' ');
            var isSequence = false;
            var buffer = new List<byte>();

            for (var i = 0; i < tokens.Length; ++i)
            {
                var token = tokens[i];

                if (string.IsNullOrWhiteSpace(token)) continue;

                if (token.Contains("{{")) { isSequence = true; continue; }
                if (token.Contains("}}")) { isSequence = false; continue; }

                if (!isSequence)
                {
                    tokensToRemove.ToList()
                         .ForEach(t =>
                            token = token.Replace(t, string.Empty));
                }

                var splitted = Enumerable.Range(0, token.Length)
                         .GroupBy(x => x / 2)
                         .Select(x => new string(x.Select(y => token[y]).ToArray()))
                         .ToList();

                splitted.ForEach(x => buffer.Add((byte)int.Parse(x, format)));
            }

            return buffer.ToArray();
        }
    }
}
