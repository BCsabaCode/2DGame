using System.IO;

namespace GameEngine.Networking.Packets
{
    class Connecting : Packet
    {
        public Connecting() : base(Code.Connecting)
        {
            
        }

        public static Packet ConstructPacket()
        {
            return new Connecting();
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write((int)Code);
        }
    }
}
