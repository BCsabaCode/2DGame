using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class Acknowledge : Packet
    {
        public Code Ack { get; private set; }
        public Acknowledge(Code ack) : base(Code.Acknowledge)
        {
            Ack = ack;
        }

        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new Acknowledge((Code)dataReader.ReadInt32());
        }

        public override void WriteData(BinaryWriter dataWriter)
        {
            dataWriter.Write((int)Code);
            dataWriter.Write((int)Ack);
        }
    }
}
