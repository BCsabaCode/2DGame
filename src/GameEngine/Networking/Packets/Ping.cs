using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class Ping : Packet
    {
        public long Time { get; private set; }
        public Ping(long time) : base(Code.Ping)
        {
            Time = time;
        }

        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new Ping(dataReader.ReadInt64());
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write((int)Code);
            writer.Write(Time);
        }
    }
}
