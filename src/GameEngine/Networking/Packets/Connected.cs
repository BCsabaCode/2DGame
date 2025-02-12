using System.IO;

namespace GameEngine.Networking.Packets
{
    class Connected : Packet
    {
        public Connected() : base(Code.Connected)
        {

        }

        public static Packet ConstructPacket()
        {
            return new Connected();
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write((int)Code);
        }
    }
}
