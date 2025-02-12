using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class EntityCorrection : Packet
    {
        public long ID { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public EntityCorrection(long id, float x, float y) : base(Code.EntityXYCorrection)
        {
            ID = id;
            X = x;
            Y = y;
        }

        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new EntityCorrection(dataReader.ReadInt64(), dataReader.ReadSingle(), dataReader.ReadSingle());
        }

        public override void WriteData(BinaryWriter dataWriter)
        {
            dataWriter.Write((int)Code);
            dataWriter.Write(ID);
            dataWriter.Write(X);
            dataWriter.Write(Y);
        }
    }
}
