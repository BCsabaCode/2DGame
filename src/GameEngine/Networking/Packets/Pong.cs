using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class Pong : Packet
    {
        public long Time { get; private set; }
        public Pong(long time) : base(Code.Pong)
        {
            Time = time;
        }

        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new Pong(dataReader.ReadInt64());
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write((int)Code);
            writer.Write(Time);
        }
    }
}
