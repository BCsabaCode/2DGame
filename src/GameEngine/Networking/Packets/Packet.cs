using System.IO;

namespace GameEngine.Networking.Packets
{
    abstract class Packet
    {
        public Code Code { get; private set; }

        public Packet(Code code)
        {
            Code = code;
        }

        public abstract void WriteData(BinaryWriter dataWriter);
    }
}
