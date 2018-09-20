using System.Threading;

namespace GD.RDP.GUI.PacketTracer.Models
{
    public class HistoryModel
    {
        private static int _counter = 0;
        private int _id;

        public HistoryModel()
        {
            Interlocked.Increment(ref _counter);

            this._id = _counter;
        }

        public int Id { get { return this._id; } }

        public string Address { get; set; }

        public string Port { get; set; }

        public string Data { get; set; }

        public string DataHexDisplay { get; set; }
    }
}