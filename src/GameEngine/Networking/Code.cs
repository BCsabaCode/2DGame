namespace GameEngine.Networking
{
    enum Code : int
    {
        Connecting = 1,
        Connected = 2,
        Ping = 3,
        Pong = 4,
        LevelGenerationData = 5,
        Acknowledge = 6,
        OtherPlayerCreationData = 7,
        OtherPlayerID = 8,
        Input = 9,
        EntityXYCorrection = 10
    }
}
