using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class Input : Packet
    {
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Left { get; private set; }
        public bool Right { get; private set; }
        
        public Input(bool up, bool down, bool left, bool right) : base(Code.Input)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
        }

        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new Input(dataReader.ReadBoolean(), dataReader.ReadBoolean(), dataReader.ReadBoolean(), dataReader.ReadBoolean());
        }

        public override void WriteData(BinaryWriter dataWriter)
        {
            dataWriter.Write((int)Code);
            dataWriter.Write(Up);
            dataWriter.Write(Down);
            dataWriter.Write(Left);
            dataWriter.Write(Right);
        }
    }
}
