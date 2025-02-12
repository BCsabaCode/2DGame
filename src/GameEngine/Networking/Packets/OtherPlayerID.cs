using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class OtherPlayerID : Packet
    {
        public long ID { get; private set; }

        public OtherPlayerID(long id) : base(Code.OtherPlayerID)
        {
            ID = id;
        }

        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new OtherPlayerID(dataReader.ReadInt64());
        }

        public override void WriteData(BinaryWriter dataWriter)
        {
            dataWriter.Write((int)Code);
            dataWriter.Write(ID);
        }
    }
}
