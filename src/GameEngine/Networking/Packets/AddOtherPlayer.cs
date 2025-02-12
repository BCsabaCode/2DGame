using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage.Streams;

namespace GameEngine.Networking.Packets
{
    class AddOtherPlayer : Packet
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public AddOtherPlayer(float x, float y) : base(Code.OtherPlayerCreationData)
        {
            X = x;
            Y = y;
        }

        public static Packet ConstructPacket(DataReader dataReader)
        {
            return new AddOtherPlayer(dataReader.ReadSingle(), dataReader.ReadSingle());
        }

        public override void WriteData(BinaryWriter dataWriter)
        {
            dataWriter.Write((int)Code);
            dataWriter.Write(X);
            dataWriter.Write(Y);
        }
    }
}
