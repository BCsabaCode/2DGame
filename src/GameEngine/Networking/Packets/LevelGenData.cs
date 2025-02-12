using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class LevelGenData : Packet
    {
        public int Seed { get; private set; }
        public int Size { get; private set; }

        public LevelGenData(int seed, int size) : base(Code.LevelGenerationData)
        {
            Seed = seed;
            Size = size;
        }
        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new LevelGenData(dataReader.ReadInt32(), dataReader.ReadInt32());
        }

        public override void WriteData(BinaryWriter dataWriter)
        {
            dataWriter.Write((int)Code);
            dataWriter.Write(Seed);
            dataWriter.Write(Size);
        }
    }
}
